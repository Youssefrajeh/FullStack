<template>
  <div class="text-center q-mt-md">
    <div class="text-h4">Find 3 Closest Branches To:</div>
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
      label="FIND 3"
      @click="findClosest"
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
import { fetcher } from "../utils/apiutil";

export default {
  name: "BranchLocator",
  setup() {
    const mapRef = ref(null);
    let state = reactive({
      status: "",
      address: "",
      showmap: false,
    });

    const findClosest = async () => {
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
          zoom: 11,
        });
        map.addControl(new tt.FullscreenControl());
        map.addControl(new tt.NavigationControl());

        // Get 3 closest branches from our API
        let threeBranches = await fetcher(`Branch/${lat}/${lon}`);

        if (!threeBranches || threeBranches.length === 0) {
          state.status = 'No branches found. Please make sure to load branch data first.';
          return;
        }

        // Create bounds to fit all markers
        const bounds = new tt.LngLatBounds();

        // Add markers and popups for each branch
        threeBranches.forEach((branch) => {
          let markerCoords = [branch.longitude, branch.latitude];

          // Extend bounds to include this branch
          bounds.extend(markerCoords);

          let marker = new tt.Marker()
            .setLngLat(markerCoords)
            .addTo(map);

          let popupOffset = 25;
          let popup = new tt.Popup({ offset: popupOffset });
          let popupContent = `<div id="popup">Branch#: ${branch.id}</div><div>${branch.street}, ${branch.city}<br/>${branch.distance.toFixed(2)} km</div>`;
          popup.setHTML(popupContent);
          marker.setPopup(popup);
        });

        // Fit map to show all markers with padding
        map.fitBounds(bounds, {
          padding: 50,
          maxZoom: 15
        });
      } catch (err) {
        console.log(err);
        state.status = `Error has occurred: ${err.message}`;
      }
    };

    return {
      mapRef,
      state,
      findClosest,
    };
  },
};
</script>
