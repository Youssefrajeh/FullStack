<template>
  <q-layout view="lHh Lpr lFf">
    <q-header elevated>
      <q-toolbar>
        <q-toolbar-title>Info3181 Case Study- Youssef Rajeh</q-toolbar-title>

        <q-btn flat round dense icon="reorder" class="q-mr-xs">
          <q-menu>
            <q-list style="min-width: 100px">
              <q-item clickable v-close-popup to="/">
                <q-item-section>Home</q-item-section>
              </q-item>
              <q-item v-if="isLoggedIn" clickable v-close-popup to="/brands">
                <q-item-section>Brands</q-item-section>
              </q-item>
              <q-item v-if="isLoggedIn" clickable v-close-popup to="/Cart">
                <q-item-section>Cart</q-item-section>
              </q-item>
              <q-item v-if="isLoggedIn" clickable v-close-popup to="/order-history">
                <q-item-section>Order History</q-item-section>
              </q-item>
              <q-item clickable v-close-popup to="/maps">
                <q-item-section>TomTom Ex1</q-item-section>
              </q-item>
              <q-item clickable v-close-popup to="/maps2">
                <q-item-section>TomTom Ex2</q-item-section>
              </q-item>
              <q-item clickable v-close-popup to="/maps3">
                <q-item-section>TomTom Ex3</q-item-section>
              </q-item>
              <q-item clickable v-close-popup to="/branches">
                <q-item-section>Branch Locator</q-item-section>
              </q-item>
              <q-item clickable v-close-popup to="/utility">
                <q-item-section>Data Utils</q-item-section>
              </q-item>
              <q-separator />
              <!-- Show Login/Register when not logged in -->
              <q-item v-if="!isLoggedIn" clickable v-close-popup to="/register">
                <q-item-section>Register</q-item-section>
              </q-item>
              <q-item v-if="!isLoggedIn" clickable v-close-popup to="/login">
                <q-item-section>Login</q-item-section>
              </q-item>
              <!-- Show Logout when logged in -->
              <q-item v-if="isLoggedIn" clickable v-close-popup to="/logout">
                <q-item-section>Logout</q-item-section>
              </q-item>
            </q-list>
          </q-menu>
        </q-btn>
      </q-toolbar>
    </q-header>

    <q-page-container>
      <router-view />
    </q-page-container>

    <q-footer elevated class="bg-grey-8 text-white">
      <q-toolbar>
        <div class="text-subtitle2 full-width text-center">
          Youssef Rajeh-1196323 — INFO3181 &copy;2025
        </div>
      </q-toolbar>
    </q-footer>
  </q-layout>
</template>

<script>
import { defineComponent, ref, onMounted, onUnmounted } from 'vue'
export default defineComponent({
  name: 'MainLayout',
  setup() {
    const isLoggedIn = ref(false)

    const checkLoginStatus = () => {
      isLoggedIn.value = !!sessionStorage.getItem('customer')
    }

    onMounted(() => {
      checkLoginStatus()
      // Listen for storage changes to update login status
      window.addEventListener('storage', checkLoginStatus)

      // Listen for custom login/logout events
      window.addEventListener('user-login', checkLoginStatus)
      window.addEventListener('user-logout', checkLoginStatus)

      // Check login status when returning to this layout
      window.addEventListener('focus', checkLoginStatus)
    })

    onUnmounted(() => {
      // Clean up event listeners to prevent memory leaks
      window.removeEventListener('storage', checkLoginStatus)
      window.removeEventListener('user-login', checkLoginStatus)
      window.removeEventListener('user-logout', checkLoginStatus)
      window.removeEventListener('focus', checkLoginStatus)
    })

    return {
      isLoggedIn
    }
  },
})
</script>

<style scoped>
/* Component-specific styles */
</style>
