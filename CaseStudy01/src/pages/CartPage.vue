<template>
  <q-page padding>
    <div class="logo-container">
      <img src="/img/2.png" alt="Logo" class="logo" />
    </div>

    <div class="text-h4 text-center q-mt-md q-mb-md text-primary">Cart Contents</div>
    <div class="text-center q-mb-md">
      <q-icon name="shopping_cart" size="md" color="primary" />
    </div>

    <!-- Stock Warnings -->
    <div v-if="state.stockWarnings.length > 0" class="q-mb-md">
      <q-banner class="bg-orange-2 text-orange-8 q-mx-auto" style="max-width: 600px">
        <template v-slot:avatar>
          <q-icon name="warning" color="orange" />
        </template>
        <div class="text-weight-bold">Stock Availability Notice:</div>
        <div class="q-mt-xs">
          <div v-for="warning in state.stockWarnings" :key="warning.productName" class="q-mb-xs">
            • <strong>{{ warning.productName }}</strong>: {{ warning.message }}
          </div>
        </div>
      </q-banner>
    </div>

    <!-- When cart has items -->
    <div v-if="state.cart.length > 0">
      <q-table
        flat
        bordered
        class="q-mx-auto"
        style="max-width: 600px"
        :rows="state.cart"
        :columns="columns"
        row-key="id"
        hide-pagination
      >
        <template v-slot:body-cell-name="props">
          <q-td>{{ props.row.item.productName }}</q-td>
        </template>
        <template v-slot:body-cell-qty="props">
          <q-td>{{ props.row.qty }}</q-td>
        </template>
        <template v-slot:body-cell-stock="props">
          <q-td>
            <span v-if="state.stockInfo[props.row.item.id]">
              {{ state.stockInfo[props.row.item.id].QtyOnHand }}
              <q-icon
                v-if="props.row.qty > state.stockInfo[props.row.item.id].QtyOnHand"
                name="warning"
                color="orange"
                size="sm"
              />
            </span>
            <span v-else>--</span>
          </q-td>
        </template>
        <template v-slot:body-cell-msrp="props">
          <q-td>${{ props.row.item.msrp.toFixed(2) }}</q-td>
        </template>
        <template v-slot:body-cell-extended="props">
          <q-td>${{ (props.row.qty * props.row.item.msrp).toFixed(2) }}</q-td>
        </template>

        <!-- Totals appended to the bottom of the table -->
        <template v-slot:bottom-row>
          <q-tr>
            <q-td colspan="3" class="text-right"><b>Sub:</b></q-td>
            <q-td>${{ subtotal.toFixed(2) }}</q-td>
          </q-tr>
          <q-tr>
            <q-td colspan="3" class="text-right"><b>Tax(13%):</b></q-td>
            <q-td>${{ tax.toFixed(2) }}</q-td>
          </q-tr>
          <q-tr>
            <q-td colspan="3" class="text-right text-primary text-h6"><b>Total:</b></q-td>
            <q-td class="text-primary text-h6"
              ><b>${{ total.toFixed(2) }}</b></q-td
            >
          </q-tr>
        </template>
      </q-table>

      <!-- Action Buttons -->
      <div class="q-mt-lg text-center">
        <q-btn
          color="primary"
          label="EMPTY CART"
          class="q-mr-sm"
          @click="clearCart"
        />
        <q-btn
          color="secondary"
          label="SAVE ORDER"
          @click="saveCart"
          :loading="state.saving"
          :disable="state.saving"
        />
      </div>
    </div>

    <!-- When cart is empty -->
    <div v-else-if="!state.status" class="text-blue text-h6 q-mt-md text-center">Cart emptied</div>

    <!-- Status Message -->
    <div v-if="state.status" class="text-center q-mt-md">
      <div class="text-green text-h6">{{ state.status }}</div>
    </div>
  </q-page>
</template>

<script>
import { reactive, computed, onMounted } from 'vue'
import { poster } from '../utils/apiutils.js'

export default {
  name: 'CartPage',
  setup() {
    const state = reactive({
      cart: [],
      status: '',
      saving: false,
      stockInfo: {},
      stockWarnings: [],
    })

    const columns = [
      { name: 'name', label: 'Name', align: 'left', field: 'name' },
      { name: 'qty', label: '#', align: 'center', field: 'qty' },
      { name: 'stock', label: 'Stock', align: 'center', field: 'stock' },
      { name: 'msrp', label: 'MSRP', align: 'right', field: 'msrp' },
      { name: 'extended', label: 'Extended', align: 'right', field: 'extended' },
    ]

    const subtotal = computed(() => state.cart.reduce((sum, i) => sum + i.item.msrp * i.qty, 0))
    const tax = computed(() => subtotal.value * 0.13)
    const total = computed(() => subtotal.value + tax.value)

    const clearCart = () => {
      sessionStorage.removeItem('tray')
      state.cart = []
      state.status = 'Cart cleared.'
    }

    // Function to decode JWT token and extract customer ID
    const getCustomerIdFromToken = (token) => {
      try {
        const payload = JSON.parse(atob(token.split('.')[1]))
        return payload.unique_name || payload.name || payload.sub
      } catch {
        return null
      }
    }

    // Function to check current stock levels
    const checkStockLevels = async () => {
      if (state.cart.length === 0) return

      try {
        const productIds = state.cart.map(item => item.item.id)
        const response = await poster('Product/checkstock', productIds)

        if (response && !response.error) {
          state.stockInfo = response

          // Generate stock warnings
          state.stockWarnings = []
          state.cart.forEach(cartItem => {
            const stock = state.stockInfo[cartItem.item.id]
            if (stock && cartItem.qty > stock.QtyOnHand) {
              if (stock.QtyOnHand === 0) {
                state.stockWarnings.push({
                  productName: cartItem.item.productName,
                  requested: cartItem.qty,
                  available: 0,
                  message: 'Out of stock - will be backordered'
                })
              } else {
                state.stockWarnings.push({
                  productName: cartItem.item.productName,
                  requested: cartItem.qty,
                  available: stock.QtyOnHand,
                  backorder: cartItem.qty - stock.QtyOnHand,
                  message: `Only ${stock.QtyOnHand} available, ${cartItem.qty - stock.QtyOnHand} will be backordered`
                })
              }
            }
          })
        }
      } catch (error) {
        console.error('Error checking stock:', error)
      }
    }

    const saveCart = async () => {
      if (state.cart.length === 0) {
        state.status = 'Cannot save empty cart.'
        return
      }

      // Check stock levels first
      await checkStockLevels()

      try {
        state.saving = true
        state.status = 'Saving order to database...'

        // Get customer info from session
        const customerData = sessionStorage.getItem('customer')
        if (!customerData) {
          state.status = 'Error: User not logged in'
          return
        }

        const customer = JSON.parse(customerData)

        // Extract customer ID from JWT token
        const customerId = getCustomerIdFromToken(customer.token)
        if (!customerId) {
          state.status = 'Error: Could not get customer ID'
          return
        }

        // Transform cart items to OrderSelectionDTO format
        const orderSelections = state.cart.map(cartItem => ({
          ProductId: cartItem.item.id,
          Qty: cartItem.qty,
          MSRP: cartItem.item.msrp
        }))

        // Call the Order API
        const response = await poster(`Order/addorder/${customerId}`, orderSelections)

        if (response && response.orderId) {
          // Clear the cart after successful save
          sessionStorage.removeItem('tray')
          state.cart = []
          state.stockWarnings = []

          // Show detailed success message
          let successMessage = `Order saved successfully! Order ID: ${response.orderId}`
          if (response.backOrderedItems && response.backOrderedItems.length > 0) {
            successMessage += '\n\nBackordered items:'
            response.backOrderedItems.forEach(item => {
              successMessage += `\n• ${item.productName}: ${item.qtyBackOrdered} units backordered`
            })
          }

          state.status = successMessage
        } else if (response && response.error) {
          state.status = `Error saving order: ${response.error}`
        } else {
          state.status = 'Error saving order to database'
        }
      } catch (error) {
        state.status = `Error saving order: ${error.message}`
      } finally {
        state.saving = false
      }
    }

    onMounted(async () => {
      const cartData = sessionStorage.getItem('tray')
      if (cartData) {
        state.cart = JSON.parse(cartData)
        // Check stock levels when cart loads
        await checkStockLevels()
      }
    })

    return {
      state,
      columns,
      subtotal,
      tax,
      total,
      clearCart,
      saveCart,
      checkStockLevels,
    }
  },
}
</script>

<style scoped>
.logo-container {
  display: flex;
  justify-content: center;
  align-items: center;
  margin-bottom: 1rem;
  height: 150px;
}

.logo {
  max-width: 300px;
  height: 300px;
  object-fit: cover;
  margin-top: 60px;
}
</style>
