<template>
  <div class="text-h4 text-center q-mt-md q-mb-md text-primary">Logout</div>
  <div class="text-title2 text-center text-positive text-bold q-mt-sm">You have been successfully logged out.</div>
  <q-card class="q-ma-md q-pa-md text-center">
    <p>Thank you for using our application!</p>
    <q-btn label="Go to Login" color="primary" to="/login" />
  </q-card>
</template>

<script>
import { onMounted } from "vue";
import { useRouter } from "vue-router";

export default {
  setup() {
    const router = useRouter();

        onMounted(() => {
      // Clear session storage on logout
      sessionStorage.removeItem("customer");
      sessionStorage.removeItem("tray"); // Also clear cart
      sessionStorage.clear();

      // Dispatch custom event to update UI
      window.dispatchEvent(new CustomEvent('user-logout'))

      // Redirect to login page after a short delay
      setTimeout(() => {
        router.push("/login");
      }, 2000); // 2 second delay to show the logout message
    });

    return {};
  },
};
</script>
