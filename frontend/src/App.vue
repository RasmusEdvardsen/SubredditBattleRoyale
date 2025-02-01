<script setup lang="ts">
import { ref } from "vue";
import Wallet from "./components/Wallet.vue";
import { onMounted, onUnmounted } from "vue";
import axios from "axios";

const backendData = ref<any>();

const setBackendData = (data: any) => {
  backendData.value = data;
};

onMounted(() => {
  const fetchData = async () => {
    try {
      const response = await axios.get(import.meta.env.VITE_BACKEND_URI);
      setBackendData(response.data);
    } catch (error) {
      console.error(error);
    }
  };

  // Fetch initially
  fetchData();

  // Set interval for polling
  const intervalId = setInterval(fetchData, 15_000); // Poll every 15 seconds

  onUnmounted(() => clearInterval(intervalId)); // Cleanup on unmount
});
</script>

<template>

  <body>
    <div className="App">
      <h1>Subreddit Battle Royale</h1>

      <!-- todo: say somewhere that we refresh blockchain data from the backend every 15s -->

      <Wallet />

      <!-- todo: do better v-ifs here (check further in on each props) -->
      <!-- todo: create actual BackendData type -->

      <div v-if="backendData">
        <h3>Void Token Count</h3>
        <p>{{ backendData.voidTokenCount.balance }}</p>
      </div>

      <!-- todo: show all these 3 types of transactions together in a table -->
      <div v-if="backendData">
        <h3>Tokens Purchased</h3>
        <li v-for="tokensPurchased in backendData.tokensPurchased" :key="tokensPurchased.id">
          {{ tokensPurchased.id }}
        </li>
      </div>

      <div v-if="backendData">
        <h3>Tokens Burned</h3>
        <li v-for="tokensBurned in backendData.tokensBurned" :key="tokensBurned.id">
          {{ tokensBurned.id }}
        </li>
      </div>

      <div v-if="backendData">
        <h3>Season Won</h3>
        <li v-for="seasonWon in backendData.seasonWon" :key="seasonWon.id">
          {{ seasonWon.id }}
        </li>
      </div>
    </div>
  </body>
</template>

<style scoped>
header {
  line-height: 1.5;
}

.logo {
  display: block;
  margin: 0 auto 2rem;
}

@media (min-width: 1024px) {
  header {
    display: flex;
    place-items: center;
    padding-right: calc(var(--section-gap) / 2);
  }

  .logo {
    margin: 0 2rem 0 0;
  }

  header .wrapper {
    display: flex;
    place-items: flex-start;
    flex-wrap: wrap;
  }
}
</style>
