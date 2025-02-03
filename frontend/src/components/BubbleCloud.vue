<script setup lang="ts">
import { onMounted, ref } from "vue";
import * as d3 from "d3";

// Define the type for dataset entries
interface DataEntry {
  id: string;
  value: number;
  radius: number;
  x?: number;
  y?: number;
}

// Original dataset
const rawData: Record<string, number> = {
  "/r/ethereum": 576,
  "/r/solana": 1036,
  "/r/bitcoin": 329,
  "/r/dogecoin": 298,
  "/r/shib": 207,
  "/r/liverpoolfc": 938,
  "/r/mcfc": 857,
  "/r/chelseafc": 913,
  "/r/dota2": 1329,
  "/r/leagueoflegends": 1091,
  "/r/globaloffensive": 387,
  "/r/pathofexile": 997,
  "/r/diablo4": 378,
  "/r/rust": 2029,
  "/r/csharp": 1290,
  "/r/python": 3829,
  "/r/cpp": 1027,
  "/r/java": 1726,
  "/r/solidity": 27,
  "/r/javascript": 4093,
  "/r/golang": 827,
};

// Ref for the SVG container
const svgRef = ref<SVGSVGElement | null>(null);

onMounted(() => {
  if (!svgRef.value) return;

  const width = 800;
  const height = 600;

  // Transform raw data into nodes with computed radius
  const nodes: DataEntry[] = Object.entries(rawData).map(([id, value]) => ({
    id,
    value,
    radius: Math.sqrt(value) * 2, // Scale the bubble size
  }));

  const simulation = d3
    .forceSimulation<DataEntry>(nodes)
    .force("charge", d3.forceManyBody().strength(5))
    .force(
      "collide",
      d3.forceCollide<DataEntry>().radius((d) => d.radius + 2)
    )
    .force("center", d3.forceCenter(width / 2, height / 2));

  const svg = d3
    .select(svgRef.value)
    .attr("width", width)
    .attr("height", height);

  const bubbles = svg
    .selectAll<SVGCircleElement, DataEntry>("circle")
    .data(nodes)
    .enter()
    .append("circle")
    .attr("r", (d) => d.radius)
    .attr("fill", (d) => d3.interpolateCool(d.value / 4093))
    .attr("stroke", "#333")
    .attr("stroke-width", 2);

  const labels = svg
    .selectAll<SVGTextElement, DataEntry>("text")
    .data(nodes)
    .enter()
    .append("text")
    .attr("text-anchor", "middle")
    .attr("dy", 4)
    .attr("font-size", "12px")
    .attr("fill", "white")
    .style("pointer-events", "none")
    .text((d) => d.id);

  simulation.on("tick", () => {
    bubbles.attr("cx", (d) => d.x || 0).attr("cy", (d) => d.y || 0);
    labels.attr("x", (d) => d.x || 0).attr("y", (d) => d.y || 0);
  });
});
</script>

<template>
  <svg ref="svgRef"></svg>
</template>

<style scoped>
svg {
  display: block;
  margin: auto;
  background-color: #222;
}
</style>
