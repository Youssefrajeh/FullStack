<template>
    <q-page class="q-pa-md">
    <div class="row items-center justify-between q-mb-md">
      <div class="text-h4">Order History</div>
      <q-btn
        v-if="state.orders.length > 0"
        color="negative"
        icon="delete_forever"
        label="Clear History"
        @click="showClearDialog = true"
      />
    </div>

    <div v-if="state.orders.length > 0">
      <div class="text-subtitle1 q-mb-sm">loaded {{ state.orders.length }} orders</div>

      <q-list>
        <q-item clickable v-for="order in state.orders" :key="order.id" @click="selectOrder(order.id)">
          <q-item-section>
            Order #{{ order.id }}
          </q-item-section>
          <q-item-section>
            {{ formatDate(order.orderDate) }}
          </q-item-section>
          <q-item-section>
            ${{ order.orderAmount.toFixed(2) }}
          </q-item-section>
        </q-item>
      </q-list>
    </div>

    <div v-else class="text-center q-mt-lg">
      <q-icon name="shopping_cart" size="4rem" color="grey-5" />
      <div class="text-h6 q-mt-md text-grey-6">No orders found</div>
    </div>

        <!-- Order Details Dialog -->
    <q-dialog v-model="state.showDialog" persistent>
      <q-card style="min-width: 350px">
        <q-card-section class="row items-center">
          <div class="text-h6">Order #{{ state.selectedOrderId }}</div>
          <q-space />
          <q-btn icon="close" flat round dense v-close-popup />
        </q-card-section>

        <q-card-section v-if="state.orderDetails.length > 0">
          <div class="text-subtitle2 q-mb-sm">
            {{ state.orderDetails[0].orderDate }}
          </div>

          <q-list>
            <q-item v-for="item in state.orderDetails" :key="item.productId">
              <q-item-section>
                <q-item-label>{{ item.productName }}</q-item-label>
                <q-item-label caption>
                  Qty Ordered: {{ item.qtyOrdered }} |
                  Qty Sold: {{ item.qtySold }} |
                  Qty Back Ordered: {{ item.qtyBackOrdered }}
                </q-item-label>
              </q-item-section>
              <q-item-section side>
                <q-item-label>${{ item.sellingPrice.toFixed(2) }}</q-item-label>
              </q-item-section>
            </q-item>
          </q-list>

          <q-separator class="q-my-md" />

          <div class="text-right">
            <div class="text-h6">Total: ${{ state.orderDetails[0].orderAmount.toFixed(2) }}</div>
          </div>
        </q-card-section>

        <q-card-actions align="right">
          <q-btn flat label="Close" color="primary" v-close-popup />
        </q-card-actions>
      </q-card>
    </q-dialog>

    <!-- Clear History Confirmation Dialog -->
    <q-dialog v-model="showClearDialog" persistent>
      <q-card style="min-width: 350px">
        <q-card-section class="row items-center">
          <q-avatar icon="warning" color="negative" text-color="white" />
          <span class="q-ml-sm text-h6">Clear Order History</span>
        </q-card-section>

        <q-card-section>
          Are you sure you want to clear all your order history? This action cannot be undone.
        </q-card-section>

        <q-card-actions align="right">
          <q-btn flat label="Cancel" color="primary" v-close-popup />
          <q-btn
            flat
            label="Clear All"
            color="negative"
            @click="clearOrderHistory"
            :loading="state.clearing"
          />
        </q-card-actions>
      </q-card>
    </q-dialog>
  </q-page>
</template>

<script>
import { reactive, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { fetcher } from 'src/utils/apiutils'
import { formatDate } from 'src/utils/formatutils'

// Import serverURL and buildHeaders from apiutils
const serverURL = 'https://localhost:7016/api/'

const buildHeaders = () => {
  const myHeaders = new Headers()
  const customerData = sessionStorage.getItem('customer')
  if (customerData) {
    const customer = JSON.parse(customerData)
    myHeaders.append('Content-Type', 'application/json')
    myHeaders.append('Authorization', 'Bearer ' + customer.token)
  } else {
    myHeaders.append('Content-Type', 'application/json')
  }
  return myHeaders
}

export default {
  name: 'OrderHistoryPage',
  setup () {
    const router = useRouter()
    const state = reactive({
      orders: [],
      orderDetails: [],
      showDialog: false,
      selectedOrderId: null,
      clearing: false
    })

    const showClearDialog = ref(false)

    const loadOrders = async () => {
      try {
        const customer = JSON.parse(sessionStorage.getItem('customer'))
        if (!customer) {
          router.push('/login')
          return
        }

        const response = await fetcher(`order/${customer.email}`)
        state.orders = response
      } catch (error) {
        console.error('Error loading orders:', error)
      }
    }

    const selectOrder = async (orderId) => {
      try {
        const customer = JSON.parse(sessionStorage.getItem('customer'))
        const response = await fetcher(`order/${orderId}/${customer.email}`)
        state.orderDetails = response
        state.selectedOrderId = orderId
        state.showDialog = true
      } catch (error) {
        console.error('Error loading order details:', error)
      }
    }

    const clearOrderHistory = async () => {
      try {
        state.clearing = true
        const customer = JSON.parse(sessionStorage.getItem('customer'))

        const response = await fetch(`${serverURL}order/clear/${customer.email}`, {
          method: 'DELETE',
          headers: buildHeaders()
        })

        if (response.ok) {
          alert('Order history cleared successfully')
          state.orders = []
          showClearDialog.value = false
        } else {
          const errorData = await response.json()
          alert(errorData.message || 'Failed to clear order history')
        }
      } catch (error) {
        alert('Error clearing order history')
        console.error('Error clearing orders:', error)
      } finally {
        state.clearing = false
      }
    }

    onMounted(() => {
      loadOrders()
    })

    return {
      state,
      showClearDialog,
      formatDate,
      selectOrder,
      clearOrderHistory
    }
  }
}
</script>
