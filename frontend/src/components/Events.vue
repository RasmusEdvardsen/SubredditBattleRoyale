<script setup lang="ts">
import type { BackendData } from '@/types';

// Define Props
const { backendData } = defineProps<{
    backendData: BackendData | undefined;
}>();
</script>

<!-- todo: comment out buyer for now to incentivize competition -->
<template>
    <div id="events-table">
        <h3>Blockchain Events</h3>
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
                <tr v-for="transaction in [...backendData.tokensPurchased, ...backendData.tokensBurned].sort((a, b) => b.blockNumber - a.blockNumber)"
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
</template>

<style scoped>
#events-table {
  max-height: 800px; /* Adjust as needed */
  max-width: 90%;
  width: 90%;
  overflow-y: scroll;
  border: 2px solid #444; /* Ensure border is always visible */
  padding: 10px;
  background-color: #222; /* Dark background */
}

table {
  width: 100%;
  border-collapse: collapse;
}

th, td {
  padding: 8px;
  text-align: left;
  border: 1px solid #555; /* Border for visibility */
}

thead {
  background-color: #333; /* Header background */
  position: sticky;
  top: 0;
  z-index: 2; /* Keep header above scrolling rows */
}

tbody tr:nth-child(even) {
  background-color: #2a2a2a; /* Darker row */
}

tbody tr:nth-child(odd) {
  background-color: #1e1e1e; /* Slightly lighter row */
}
</style>
