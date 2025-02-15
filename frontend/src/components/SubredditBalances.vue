<script setup lang="ts">
import { renderSubredditBalances } from '@/utils/d3';
import { computed, onMounted, ref, watch } from 'vue';

const { aggregatedData } = defineProps<{
    aggregatedData: Record<string, number>;
}>();

const sortedData = computed(() => {
    return Object.entries(aggregatedData)
        .sort(([, a], [, b]) => b - a)
        .map(([key, value]) => ({ subreddit: key, balance: value }));
});

const filteredData = ref<{ subreddit: string; balance: number; }[]>(sortedData.value);
const searchQuery = ref("");
const svgRef = ref<SVGSVGElement | null>(null);

// Redraw chart when data changes
watch([sortedData, searchQuery], (params) => {
    filteredData.value = sortedData.value.filter(d =>
        d.subreddit.toLowerCase().includes(params[1].toLowerCase())
    );
    renderSubredditBalances(svgRef.value, filteredData.value);
}, { deep: true });

onMounted(renderSubredditBalances);
</script>

<template>
    <div id="subreddit-balances">
        <h3>Subreddit Balances</h3>
        <input type="text" v-model="searchQuery" placeholder="Filter subreddits..." />
        <div id="chart-container">
            <svg ref="svgRef"></svg>
        </div>
    </div>
</template>


<style scoped>
#subreddit-balances {
    max-width: 90%;
    width: 90%;
    padding: 10px;
    border: 2px solid #444;
    margin-bottom: 10px;
    background-color: #222;
}

#chart-container {
    max-height: 400px;
    overflow-y: scroll;
    border: 2px solid #444;
    padding: 5px;
}

input {
    display: block;
    margin-bottom: 10px;
    padding: 5px;
    border: 1px solid #555;
    background-color: #333;
    color: white;
}
</style>
