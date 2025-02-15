<script setup lang="ts">
import type { BackendData } from '@/types';

// Define Props
const { backendData } = defineProps<{
    backendData: BackendData | undefined;
}>();
</script>

<template>
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
</template>