<template>
  <div class="text-center q-pa-md">
    <div class="logo-container">
      <img src="/img/2.png" alt="Logo" class="logo" />
    </div>

    <!-- Brand Selector -->
    <div v-if="state.brands && state.brands.length > 0" class="q-mt-lg">
      <q-select
        v-model="state.selectedBrandId"
        :options="state.brands"
        option-label="name"
        option-value="id"
        label="Select a Brand"
        emit-value
        map-options
        outlined
        class="brand-selector q-mx-auto"
      />

      <div v-if="state.selectedBrand" class="text-h5 text-bold text-primary q-mt-md q-mb-md">
        {{ state.selectedBrand.name }} Products
      </div>

      <div
        v-if="state.brandProducts && state.brandProducts.length > 0"
        class="product-list q-mt-md"
      >
        <q-list bordered separator class="bg-white">
          <q-item
            v-for="product in state.brandProducts"
            :key="product.id"
            class="product-item"
            clickable
            @click="selectProduct(product.id)"
          >
            <q-item-section avatar>
              <q-img
                v-if="product.graphicName"
                :src="'/img/' + product.graphicName"
                spinner-color="primary"
                spinner-size="42px"
                :error-src="'/img/2.png'"
                style="width: 80px; height: 80px"
                class="product-image"
              />
            </q-item-section>
            <q-item-section>
              <q-item-label class="text-subtitle1">{{ product.productName }}</q-item-label>
            </q-item-section>
          </q-item>
        </q-list>
      </div>

      <div v-else-if="state.selectedBrand" class="text-h6 text-center q-mt-md">
        No products found for this brand
      </div>
    </div>

    <div v-if="state.status" class="q-mt-md text-negative status-text">
      {{ state.status }}
    </div>

    <!-- Dialog for Product Details -->
    <q-dialog v-model="state.productDialog">
      <q-card>
        <q-card-actions align="right">
          <q-btn flat label="X" color="primary" v-close-popup />
        </q-card-actions>

                        <q-card-section class="text-center">
          <q-img
            v-if="state.selectedProduct.graphicName"
            :src="'/img/' + state.selectedProduct.graphicName"
            style="width: 100px; height: 100px"
          />
        </q-card-section>

        <q-card-section>
          <div class="text-subtitle2 text-center">
            {{ state.selectedProduct.description }}
          </div>
        </q-card-section>

        <q-card-section>
          <q-chip
            color="primary"
            text-color="white"
            icon="bookmark"
            label="Details"
            @mouseenter="state.detailsPanel = true"
          >
            <q-menu
              v-model="state.detailsPanel"
              anchor="bottom middle"
              self="top middle"
              :offset="[0, 10]"
              @hide="state.detailsPanel = false"
            >
              <q-card style="min-width: 300px; max-width: 400px; background: white; border: 1px solid #ddd; border-radius: 8px; box-shadow: 0 4px 12px rgba(0,0,0,0.15);">
                <q-card-section class="q-pa-md">
                  <div style="font-weight: 600; margin-bottom: 12px; color: #1976d2; font-size: 16px;">Product Details</div>
                  <div class="q-gutter-sm" style="color: #333;">
                    <div><strong>ID:</strong> {{ state.selectedProduct.id }}</div>
                    <div><strong>Name:</strong> {{ state.selectedProduct.productName }}</div>
                    <div><strong>Price:</strong> ${{ state.selectedProduct.msrp }}</div>
                    <div><strong>Cost:</strong> ${{ state.selectedProduct.costPrice }}</div>
                    <div>
                      <strong>In Stock:</strong>
                      <span :class="state.selectedProduct.qtyOnHand === 0 ? 'text-red' : state.selectedProduct.qtyOnHand < 5 ? 'text-orange' : 'text-green'">
                        {{ state.selectedProduct.qtyOnHand }}
                        <q-icon v-if="state.selectedProduct.qtyOnHand === 0" name="warning" size="sm" color="red" />
                        <q-icon v-else-if="state.selectedProduct.qtyOnHand < 5" name="warning" size="sm" color="orange" />
                      </span>
                    </div>
                    <div><strong>Backorder:</strong> {{ state.selectedProduct.qtyOnBackOrder }}</div>
                    <div><strong>Description:</strong> {{ state.selectedProduct.description }}</div>
                  </div>
                </q-card-section>
              </q-card>
            </q-menu>
          </q-chip>
        </q-card-section>

        <q-card-section>
          <div class="column q-gutter-md items-center">
                                                                        <!-- Quantity Input with Custom Native-Style Spinners -->
            <div class="native-number-input">
              <q-input
                v-model.number="state.qty"
                type="number"
                outlined
                dense
                placeholder="Qty"
                style="max-width: 100px"
                class="text-center"
                :min="0"
                hide-spin-buttons
                :error="state.qty > state.selectedProduct.qtyOnHand"
                :error-message="state.qty > state.selectedProduct.qtyOnHand ? `Only ${state.selectedProduct.qtyOnHand} in stock` : ''"
              />
              <div class="native-spinners">
                <button
                  type="button"
                  class="spinner-up"
                  @click="incrementQty"
                >
                  ▲
                </button>
                <button
                  type="button"
                  class="spinner-down"
                  @click="decrementQty"
                  :disabled="state.qty <= 0"
                >
                  ▼
                </button>
              </div>
            </div>
            <!-- Button Row -->
            <div class="row q-gutter-sm justify-center full-width">
              <q-btn
                color="primary"
                label="ADD TO CART"
                @click="addToTray"
                class="col-5 col-sm-auto"
                no-caps
              />
              <q-btn
                color="secondary"
                label="VIEW CART"
                @click="viewCart"
                class="col-5 col-sm-auto"
                no-caps
              />
            </div>
          </div>
        </q-card-section>

        <q-card-section
          class="text-center"
          :class="state.dialogStatus.includes('removed') ? 'text-negative' : 'text-positive'"
        >
          {{ state.dialogStatus }}
        </q-card-section>
      </q-card>
    </q-dialog>
  </div>
</template>

<script>
import { fetcher, testConnection } from '../utils/apiutils.js'
import { reactive, onMounted, watch } from 'vue'
import { useRouter } from 'vue-router'

export default {
  setup() {
    const router = useRouter()

    const state = reactive({
      status: '',
      brands: [],
      selectedBrandId: null,
      selectedBrand: null,
      brandProducts: [],
      selectedProduct: {},
      productDialog: false,
      detailsPanel: false,
      dialogStatus: '',
      qty: 0,
      tray: [],
    })

    const loadBrands = async () => {
      try {
        state.status = 'Testing connection to API server...'
        const isConnected = await testConnection()
        if (!isConnected) {
          state.status = 'Cannot connect to API server. Please ensure the server is running.'
          return
        }

        state.status = 'Loading brands...'
        const response = await fetcher('Brand')
        if (Array.isArray(response)) {
          state.brands = response
          state.status = ''
        } else {
          state.status = 'Invalid response format from API'
        }
      } catch (err) {
        state.status = `Error: ${err.message}`
      }
    }

    const getBrandProducts = async () => {
      try {
        state.selectedBrand = state.brands.find((b) => b.id === state.selectedBrandId)
        if (!state.selectedBrand) {
          state.status = 'Selected brand not found.'
          return
        }

        state.status = `Loading products...`
        const response = await fetcher(`Product/brand/${state.selectedBrandId}`)
        if (Array.isArray(response)) {
          state.brandProducts = response
          state.status = ''
        } else {
          state.status = 'Invalid response format from API'
        }
      } catch (err) {
        state.status = `Error: ${err.message}`
      }
    }

    const selectProduct = (productId) => {
      state.selectedProduct = state.brandProducts.find((p) => p.id === productId)
      state.qty = 0
      state.dialogStatus = ''
      state.productDialog = true

      if (sessionStorage.getItem('tray')) {
        state.tray = JSON.parse(sessionStorage.getItem('tray'))
      }
    }

    const addToTray = () => {
      let index = -1
      if (state.tray.length > 0) {
        index = state.tray.findIndex((item) => item.id === state.selectedProduct.id)
      }

      if (state.qty > 0) {
        // Check stock availability
        const availableStock = state.selectedProduct.qtyOnHand || 0
        let statusMessage = `${state.qty} item(s) added`

        if (state.qty > availableStock) {
          if (availableStock === 0) {
            statusMessage += ` (Out of stock - will be backordered)`
          } else {
            const backorderQty = state.qty - availableStock
            statusMessage += ` (${availableStock} in stock, ${backorderQty} will be backordered)`
          }
        }

        const newItem = {
          id: state.selectedProduct.id,
          qty: state.qty,
          item: state.selectedProduct,
        }

        if (index === -1) {
          state.tray.push(newItem)
        } else {
          state.tray[index] = newItem
        }

        state.dialogStatus = statusMessage
      } else {
        if (index !== -1) {
          state.tray.splice(index, 1)
          state.dialogStatus = `item(s) removed`
        } else {
          state.dialogStatus = `item not found in tray`
        }
      }

      sessionStorage.setItem('tray', JSON.stringify(state.tray))
      state.qty = 0
    }

    const viewCart = () => {
      router.push('/cart')
    }

    const incrementQty = () => {
      state.qty = (state.qty || 0) + 1
    }

    const decrementQty = () => {
      if (state.qty > 0) {
        state.qty = state.qty - 1
      }
    }

    onMounted(() => {
      loadBrands()
    })

    watch(
      () => state.selectedBrandId,
      async (newVal) => {
        if (newVal) {
          await getBrandProducts()
        } else {
          state.brandProducts = []
          state.selectedBrand = null
        }
      },
    )

    return {
      state,
      selectProduct,
      addToTray,
      viewCart,
      incrementQty,
      decrementQty,
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

.brand-selector {
  width: 80%;
  max-width: 400px;
}

.product-list {
  max-width: 600px;
  margin: 0 auto;
}

.product-item {
  padding: 10px 5px;
}

.product-image {
  object-fit: contain;
}

.status-text {
  position: fixed;
  bottom: 60px;
  left: 0;
  right: 0;
  font-size: 14px;
}

/* Native Windows-style number input spinners */
.native-number-input {
  position: relative;
  display: inline-block;
}

.native-number-input .q-field__control {
  padding-right: 20px !important;
}

.native-spinners {
  position: absolute;
  right: 1px;
  top: 1px;
  bottom: 1px;
  display: flex;
  flex-direction: column;
  width: 17px;
  border-left: 1px solid #d3d3d3;
}

.spinner-up,
.spinner-down {
  background: linear-gradient(to bottom, #ffffff 0%, #f1f1f1 50%, #e1e1e1 51%, #f6f6f6 100%);
  border: none;
  width: 16px;
  height: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 10px;
  cursor: pointer;
  padding: 0;
  margin: 0;
  color: #333;
  font-family: 'Segoe UI', Arial, sans-serif;
}

.spinner-up {
  border-bottom: 1px solid #d3d3d3;
  border-radius: 0 2px 0 0;
}

.spinner-down {
  border-radius: 0 0 2px 0;
}

.spinner-up:hover,
.spinner-down:hover {
  background: linear-gradient(to bottom, #e3f7ff 0%, #d9f0fc 50%, #bde6fd 51%, #a7d9f5 100%);
}

.spinner-up:active,
.spinner-down:active {
  background: linear-gradient(to bottom, #c4e5f6 0%, #b7dffd 50%, #a8d8f5 51%, #94cbf2 100%);
}

.spinner-down:disabled {
  background: #f5f5f5;
  color: #ccc;
  cursor: not-allowed;
}
</style>
