<script setup lang="ts">
import { ref, watch } from "vue";
import Wallet from "./components/Wallet.vue";
import BubbleCloud from "./components/BubbleCloud.vue";
import { onMounted, onUnmounted } from "vue";
import axios from "axios";
import type { BackendData } from "./types";

const backendData = ref<BackendData>();
const aggregatedData = ref<Record<string, number>>({});

onMounted(() => {
  const fetchData = async () => {
    try {
      const response = await axios.get<BackendData>(import.meta.env.VITE_BACKEND_URI);
      backendData.value = response.data;
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

watch(backendData, (newData) => {
  if (newData) {
    // Aggregate data
    const aggregated = [...newData.tokensPurchased, ...newData.tokensBurned].reduce((acc: Record<string, number>, curr) => {
      if (!acc[curr.subreddit]) {
        acc[curr.subreddit] = 0;
      }

      acc[curr.subreddit] += curr.eventType == "TokensPurchased" ? curr.tokens : -curr.tokens;

      return acc;
    }, {});

    aggregatedData.value = aggregated;
  }
});
</script>

<template>

  <body>
    <div className="App">
      <!-- todo: say somewhere that we refresh blockchain data from the backend every 15s -->
      <!-- todo: do better v-ifs (check further in on each props) -->

      <BubbleCloud :rawData="aggregatedData" />

      <h1>Subreddit Battle Royale</h1>

      <Wallet />

      <div v-if="backendData">
        <h3>Void Token Count</h3>
        <p>{{ backendData.voidTokenCount.balance }}</p>
      </div>

      <div v-if="backendData">
        <h3>Seasons Won</h3>
        <li v-for="seasonWon in backendData.seasonWon" :key="seasonWon.id">
          {{ seasonWon.id }}
        </li>
      </div>

      <!-- todo: a bit ugly. change to filterable barchart with aggregatedData -->
      <!-- todo: move to separate component -->
      <h3>Subreddit Balances</h3>
      <table v-if="aggregatedData">
        <thead>
          <tr>
            <th>Subreddit</th>
            <th>Balance</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(balance, subreddit) in aggregatedData" :key="subreddit">
            <td>{{ subreddit }}</td>
            <td>{{ balance }}</td>
          </tr>
        </tbody>
      </table>

      <!-- todo: move to separate component -->
      <h3>Events</h3>
      <table v-if="backendData">
        <thead>
          <tr>
            <th>Event Type</th>
            <th>Block No.</th>
            <th>Transaction (#)</th>
            <th>Subreddit</th>
            <th>Amount</th>
            <th>Buyer (#)</th>
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="transaction in [...backendData.tokensPurchased, ...backendData.tokensBurned].sort((a, b) => b.blockNumber - a.blockNumber)"
            :key="transaction.id">
            <td>{{ transaction.eventType }}</td>
            <td>{{ transaction.blockNumber }}</td>
            <td>{{ transaction.transactionHash }}</td>
            <td>{{ transaction.subreddit }}</td>
            <td>{{ transaction.tokens }}</td>
            <td>{{ transaction.buyer }}</td>
          </tr>
        </tbody>
      </table>
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
