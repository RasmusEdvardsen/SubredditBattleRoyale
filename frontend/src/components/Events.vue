<script setup lang="ts">
import type { BackendData } from '@/types';

// Define Props
const { backendData } = defineProps<{
    backendData: BackendData | undefined;
}>();
</script>

<template>
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
</template>