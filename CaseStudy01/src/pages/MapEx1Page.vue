<template>
  <div class="text-center">
    <div class="text-h4 q-mt-lg">TomTom Map Ex1</div>
    <div class="text-h6">get lat and lon</div>
    <div>
      <q-input
        class="q-ma-lg text-h5"
        placeholder="enter current address"
        id="address"
        v-model="state.address"
      />
      <br />
      <q-btn
        label="Get Lat/Lon"
        @click="getLatLon"
        color="primary"
        style="width: 30vw"
      />
    </div>
    <div class="text-h6 q-mt-lg" v-if="state.latlon">
      {{ state.latlon }}
    </div>
    <div class="text-negative text-subtitle1" v-if="state.status">
      {{ state.status }}
    </div>
  </div>
</template>

<script>
import { reactive } from "vue";

export default {
  setup() {
    let state = reactive({
      status: "",
      address: "",
      latlon: "",
    });

    const getLatLon = async () => {
      try {
        let url = `https://api.tomtom.com/search/2/geocode/${state.address}.json?key=VDc5zSCqsmy5KOdRK5LGISiYPa9qzaZH`;
        let response = await fetch(url);
        let payload = await response.json();

        if (payload.results && payload.results.length > 0) {
          let result = payload.results[0];
          let lat = result.position.lat;
          let lon = result.position.lon;
          let address = result.address;

          // Format the full address
          let fullAddress = [
            address.streetNumber,
            address.streetName,
            address.municipality,
            address.countrySubdivision,
            address.country
          ].filter(Boolean).join(', ');

          state.latlon = `${fullAddress}\nLatitude: ${lat}\nLongitude: ${lon}`;
          state.status = "";
        } else {
          state.status = "Address not found";
          state.latlon = "";
        }
      } catch (err) {
        state.status = err.message;
      }
    };

    return {
      state,
      getLatLon,
    };
  },
};
</script>
