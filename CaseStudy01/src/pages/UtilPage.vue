<template>
  <div class="text-center q-mt-lg">
    <div class="text-h4">Data Utility</div>
    <q-btn
      class="q-ma-sm"
      color="white"
      text-color="black"
      label="Load Branches"
      @click="loadBranches"
    />
    <div class="status q-mt-md text-subtitle2 text-negative" text-color="red">
      {{ state.status }}
    </div>
  </div>
</template>

<script>
import { reactive } from "vue";
import { fetcher } from "../utils/apiutil";

export default {
  setup() {
    let state = reactive({
      status: "",
    });

    const loadBranches = async () => {
      try {
        state.status = "loading branch data ...";
        let payload = await fetcher("Data/loadbranches");
        state.status = payload;
      } catch (err) {
        console.log(err);
        state.status = `Error has occured: ${err.message}`;
      }
    };

    return {
      loadBranches,
      state,
    };
  },
};
</script>
