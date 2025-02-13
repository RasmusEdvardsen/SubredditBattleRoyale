<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue';
import * as d3 from 'd3';

const { aggregatedData } = defineProps<{
    aggregatedData: Record<string, number>;
}>();

const sortedData = computed(() => {
    return Object.entries(aggregatedData)
        .sort(([, a], [, b]) => b - a)
        .map(([key, value]) => ({ subreddit: key, balance: value }));
});

const filteredData = ref(sortedData.value);
const searchQuery = ref("");

// Redraw chart when data changes
watch([sortedData, searchQuery], (params) => {
    filteredData.value = sortedData.value.filter(d =>
        d.subreddit.toLowerCase().includes(params[1].toLowerCase())
    );
    drawChart();
}, { deep: true });

// todo: move out into separate d3 function/file
// Function to draw the bar chart
const drawChart = () => {
    const svg = d3.select("#chart");
    svg.selectAll("*").remove(); // Clear previous chart

    const width = window.innerWidth;
    const height = 600;
    const margin = { top: 20, right: 20, bottom: 50, left: 100 };

    svg.attr("width", width).attr("height", height);

    const x = d3.scaleLinear()
        .domain([0, d3.max(filteredData.value, d => d.balance) || 1])
        .range([0, width - margin.left - margin.right]);

    const y = d3.scaleBand()
        .domain(filteredData.value.map(d => d.subreddit))
        .range([0, height - margin.top - margin.bottom])
        .padding(0.2);

    const g = svg.append("g")
        .attr("transform", `translate(${margin.left},${margin.top})`);

    // Bars
    g.selectAll("rect")
        .data(filteredData.value)
        .enter().append("rect")
        .attr("y", d => y(d.subreddit)!)
        .attr("width", d => x(d.balance))
        .attr("height", y.bandwidth())
        .attr("fill", "#334d37");

    // Labels
    g.selectAll("text")
        .data(filteredData.value)
        .enter().append("text")
        .attr("x", d => x(d.balance) - 5)
        .attr("y", d => y(d.subreddit)! + y.bandwidth() / 2)
        .attr("dy", "0.35em")
        .attr("text-anchor", "end")
        .attr("fill", "white")
        .text(d => d.balance);

    // Axes
    g.append("g")
        .call(d3.axisLeft(y));

    g.append("g")
        .attr("transform", `translate(0,${height - margin.top - margin.bottom})`)
        .call(d3.axisBottom(x));
};

onMounted(drawChart);
</script>

<template>
    <div id="subreddit-balances">
        <h3>Subreddit Balances</h3>
        <input type="text" v-model="searchQuery" placeholder="Filter subreddits..." />
        <div id="chart-container">
            <svg id="chart"></svg>
        </div>
    </div>
</template>


<style scoped>
#subreddit-balances {
    max-width: 90%;
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
