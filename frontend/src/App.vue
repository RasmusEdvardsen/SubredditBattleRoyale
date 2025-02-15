<script setup lang="ts">
import { ref, watch, onMounted, onUnmounted } from "vue";

import { fetchData } from "@/utils/axios";

import Wallet from "@/components/Wallet.vue";
import BubbleCloud from "@/components/BubbleCloud.vue";
import Events from "@/components/Events.vue";
import SubredditBalances from "@/components/SubredditBalances.vue";

import type { BackendData } from "@/types";

const backendData = ref<BackendData>();
const aggregatedData = ref<Record<string, number>>({});

onMounted(() => {
  fetchData(backendData); // Fetch initially

  const intervalId = setInterval(() => fetchData(backendData), 15_000); // Poll every 15 seconds

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
  <BubbleCloud :aggregatedData="aggregatedData" />

  <div id="intro-and-title">
    <h1>Subreddit Battle Royale</h1>
    <p>
      Welcome to the Subreddit Battle Royale!
      In this game, subreddits compete against each other by purchasing and burning tokens.
      The goal is to accumulate the more tokens than the void token count, thereby winning the season.
      <br>We refresh blockchain data from the backend every 15 seconds to keep the competition up-to-date.
      <br>The cost of a token is 0.0001 Ether. We recommend buying at least 10 tokens per transaction to save on gas
      fees.
      <br>Good luck and may the best subreddit win!
    </p>
  </div>

  <Wallet />

  <!-- todo: move in to component as well -->
  <div v-if="backendData" id="void-token-count">
    <h3>Void Token Count: {{ backendData.voidTokenCount.balance }}</h3>
  </div>

  <!-- todo: move in to component as well -->
  <div v-if="backendData && backendData.seasonWon.length > 0">
    <h3>Seasons Won</h3>
    <ul>
      <li v-for="seasonWon in backendData.seasonWon" :key="seasonWon.id">
        Season {{ seasonWon.season }} was won by {{ seasonWon.subreddit }}.
      </li>
    </ul>
  </div>
  <div v-else>
    <h3>No seasons won yet.</h3>
  </div>

  <SubredditBalances :aggregatedData="aggregatedData" />

  <Events :backendData="backendData"/>
</template>

<style scoped>
#intro-and-title {
  max-width: 80%;
  text-align: center;
}
</style>
