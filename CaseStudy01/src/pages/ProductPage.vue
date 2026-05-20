<template>
  <div class="text-center q-pa-md">
    <div class="logo-container">
      <img src="/img/2.png" alt="Logo" class="logo" />
    </div>
    <div class="text-h2 q-mt-md">All Products</div>
    <div class="q-mt-md text-negative" v-if="state.status">
      {{ state.status }}
    </div>

    <!-- Products Grid View -->
    <div v-if="state.products.length > 0" class="q-mt-lg">
      <div class="row q-col-gutter-md">
        <div
          v-for="product in state.products"
          :key="product.id"
          class="col-12 col-sm-6 col-md-4 col-lg-3 q-mb-lg"
        >
          <q-card class="product-card">
            <q-img
              :src="`/img/${product.graphicName}`"
              :ratio="0.75"
              spinner-color="primary"
              spinner-size="42px"
              :error-src="'/img/2.png'"
              class="product-image"
              style="max-height: 180px"
            >
              <div class="absolute-top text-subtitle2 text-right q-pa-xs bg-transparent">
                <q-badge color="primary" v-if="product.qtyOnHand > 0">
                  In Stock: {{ product.qtyOnHand }}
                </q-badge>
              </div>
            </q-img>
            <q-separator />
            <q-card-section>
              <div class="text-h6 ellipsis-2-lines">{{ product.productName }}</div>
              <div class="text-subtitle1 text-weight-bold q-mt-sm text-primary">
                ${{ product.msrp ? product.msrp.toFixed(2) : '0.00' }}
              </div>
              <div class="q-mt-xs">
                <q-badge color="teal" v-if="product.brandId">
                  Brand ID: {{ product.brandId }}
                </q-badge>
                <q-badge color="orange" class="q-ml-sm" v-if="product.qtyOnBackOrder > 0">
                  On Backorder: {{ product.qtyOnBackOrder }}
                </q-badge>
              </div>
            </q-card-section>
            <q-card-section class="q-pt-none">
              <div class="text-caption ellipsis-3-lines">
                {{ product.description || 'No description available' }}
              </div>
            </q-card-section>
            <q-card-actions align="right">
              <q-btn flat color="primary" label="Details" />
            </q-card-actions>
          </q-card>
        </div>
      </div>
    </div>
    <div v-else-if="!state.status" class="text-h6 q-mt-xl">Loading products...</div>
  </div>
</template>

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

.product-card {
  height: 100%;
  display: flex;
  flex-direction: column;
  transition: transform 0.2s;
}

.product-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.1);
}

.product-image {
  background: #f7f7f7;
  object-fit: contain;
  max-height: 180px;
  margin: 0 auto;
}

.ellipsis-2-lines {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.ellipsis-3-lines {
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>

<script>
import { reactive, onMounted, watch } from 'vue'
import { fetcher } from '../utils/apiutils.js'

export default {
  setup() {
    const state = reactive({
      status: '',
      products: [],
      selectedProductId: null,
      selectedProduct: null,
    })

    const testConnection = async () => {
      try {
        const response = await fetch('https://localhost:7016/api/Product')
        return response.ok
      } catch {
        return false
      }
    }

    const loadProducts = async () => {
      try {
        state.status = 'Testing connection to API server...'
        const isConnected = await testConnection()
        if (!isConnected) {
          state.status = 'Cannot connect to API server. Please ensure the server is running.'
          return
        }

        state.status = 'Loading products...'
        const response = await fetcher(`Product`)
        if (Array.isArray(response)) {
          state.products = response
          state.status = ''
        } else {
          state.status = 'Invalid response format from API'
        }
      } catch (err) {
        state.status = `Error: ${err.message}`
      }
    }

    onMounted(loadProducts)

    // ✅ Watch selectedProductId to update selectedProduct
    watch(
      () => state.selectedProductId,
      (newId) => {
        state.selectedProduct = state.products.find((p) => p.id === newId) || null
      },
    )

    return {
      state,
    }
  },
}
</script>
