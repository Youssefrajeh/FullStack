<template>
  <div class="text-center q-mt-md">
    <div class="text-h4">TomTom Map Ex3</div>
    <div class="text-h6">show map with marker</div>
    <div>
      <q-input
        class="q-ma-lg text-h5"
        placeholder="enter current address"
        id="address"
        v-model="state.address"
      />
      <br />
    </div>
    <q-btn
      label="Gen Map"
      @click="genMap"
      color="primary"
      class="q-mb-md"
      style="width: 30vw"
    />
    <div
      style="height: 50vh; width: 90vw; margin-left: 5vw; border: solid"
      ref="mapRef"
      v-show="state.showmap === true"
    ></div>
  </div>
</template>

<script>
import { ref, reactive } from "vue";

export default {
  name: "MapEx3",
  setup() {
    const mapRef = ref(null);
    let state = reactive({
      status: "",
      address: "",
      showmap: false,
    });

    const genMap = async () => {
      try {
        state.showmap = true;
        const tt = window.tt;

        // First get lat/lon for entered address
        let url = `https://api.tomtom.com/search/2/geocode/${state.address}.json?key=VDc5zSCqsmy5KOdRK5LGISiYPa9qzaZH`;
        let response = await fetch(url);
        let payload = await response.json();
        let lat = payload.results[0].position.lat;
        let lon = payload.results[0].position.lon;

        // Create the map centered on the address
        let map = tt.map({
          key: "VDc5zSCqsmy5KOdRK5LGISiYPa9qzaZH",
          container: mapRef.value,
          source: "vector/1/basic-main",
          center: [lon, lat],
          zoom: 13,
        });
        map.addControl(new tt.FullscreenControl());
        map.addControl(new tt.NavigationControl());

        // Add a marker at the address location
        let marker = new tt.Marker().setLngLat([lon, lat]).addTo(map);

        // Add a popup to the marker
        let popupOffset = 25;
        let popup = new tt.Popup({ offset: popupOffset });
        popup.setHTML(
          `<div id="popup">Location Details</div><div>${state.address}</div>`
        );
        marker.setPopup(popup);
      } catch (err) {
        console.log(err);
        state.status = `Error has occurred: ${err.message}`;
      }
    };

    return {
      mapRef,
      state,
      genMap,
    };
  },
};
</script>
