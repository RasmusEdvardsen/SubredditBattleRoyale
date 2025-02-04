<script setup lang="ts">
import { onMounted, ref, computed } from "vue";
import * as d3 from "d3";

// Define the type for dataset entries
interface DataEntry {
  id: string;
  value: number;
  radius: number;
  x?: number;
  y?: number;
}

// Define Props
const props = defineProps<{
  rawData: Record<string, number>;
}>();

// Ref for the SVG container
const svgRef = ref<SVGSVGElement | null>(null);

// Convert rawData into a reactive node list
const nodes = computed<DataEntry[]>(() =>
  Object.entries(props.rawData).map(([id, value]) => ({
    id,
    value,
    radius: Math.sqrt(value) * 2, // Scale the bubble size
  }))
);

onMounted(() => {
  if (!svgRef.value) return;

  const width = 800;
  const height = 600;

  // D3 Force Simulation
  const simulation = d3
    .forceSimulation<DataEntry>(nodes.value)
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
    .data(nodes.value)
    .enter()
    .append("circle")
    .attr("r", (d) => d.radius)
    .attr("fill", (d) => d3.interpolateCool(d.value / 4093))
    .attr("stroke", "#333")
    .attr("stroke-width", 2);

  const labels = svg
    .selectAll<SVGTextElement, DataEntry>("text")
    .data(nodes.value)
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
