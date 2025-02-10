<script setup lang="ts">
import { computed } from 'vue';

const { aggregatedData } = defineProps<{
    aggregatedData: Record<string, number>;
}>();

const sortedData = computed(() => {
    return Object.entries(aggregatedData)
        .sort(([, a], [, b]) => b - a)
        .reduce((acc, [key, value]) => {
            acc[key] = value;
            return acc;
        }, {} as Record<string, number>);
});
</script>

<!-- todo: a bit ugly. change to filterable barchart with aggregatedData -->
<template>
    <h3>Subreddit Balances</h3>
    <table v-if="aggregatedData">
        <thead>
            <tr>
                <th>Subreddit</th>
                <th>Balance</th>
            </tr>
        </thead>
        <tbody>
            <tr v-for="(balance, subreddit) in sortedData" :key="subreddit">
                <td>{{ subreddit }}</td>
                <td>{{ balance }}</td>
            </tr>
        </tbody>
    </table>
</template>