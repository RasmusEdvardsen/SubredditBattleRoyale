<script setup lang="ts">
import { onMounted, ref, watch, computed, nextTick } from "vue";
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
  aggregatedData: Record<string, number>;
}>();

// Ref for the SVG container
const svgRef = ref<SVGSVGElement | null>(null);

// Convert aggregatedData into a reactive node list
const nodes = computed<DataEntry[]>(() =>
  Object.entries(props.aggregatedData).map(([id, value]) => ({
    id,
    value,
    radius: Math.sqrt(value) * 2, // Scale the bubble size
  }))
);

let simulation: d3.Simulation<DataEntry, undefined> | null = null;

// todo: move out into separate d3 function/file
const renderChart = async () => {
  if (!svgRef.value) return;

  const width = window.innerWidth;
  const height = 600;

  const svg = d3.select(svgRef.value);
  svg.selectAll("*").remove(); // Clear previous SVG content

  // Get min/max radius for scaling colors
  const extent = d3.extent(nodes.value, d => d.radius) as [number, number] | [undefined, undefined];
  const minRadius = extent[0] ?? 5; // Default to 5 if undefined
  const maxRadius = extent[1] ?? 50; // Default to 50 if undefined

  const colorScale = d3.scaleSequential(d3.interpolateMagma).domain([minRadius, maxRadius]);

  // Restart the simulation
  simulation = d3
    .forceSimulation<DataEntry>(nodes.value)
    .force("charge", d3.forceManyBody().strength(5))
    .force(
      "collide",
      d3.forceCollide<DataEntry>().radius((d) => d.radius + 2)
    )
    .force("center", d3.forceCenter(width / 2, height / 2));

  const bubbles = svg
    .selectAll<SVGCircleElement, DataEntry>("circle")
    .data(nodes.value)
    .enter()
    .append("circle")
    .attr("r", (d) => d.radius)
    .attr("fill", (d) => {
      // Get color based on radius
      let baseColor = d3.color(colorScale(d.radius));
      return baseColor ? baseColor.darker(0.2).toString() : "#666";
    })
    .attr("stroke", "#222")
    .attr("stroke-width", 1.5);

  const labels = svg
    .selectAll<SVGTextElement, DataEntry>("text")
    .data(nodes.value)
    .enter()
    .append("text")
    .attr("text-anchor", "middle")
    .attr("dy", 4)
    .attr("font-size", (d) => Math.max(d.radius / 3, 10) + "px") // Scale text size
    .attr("fill", "white") // White text
    .attr("stroke", "black") // Black border
    .attr("stroke-width", 2) // Thickness of the border
    .attr("paint-order", "stroke") // Ensures stroke is applied outside the text
    .style("pointer-events", "none")
    .text((d) => d.id);


  simulation.on("tick", () => {
    bubbles.attr("cx", (d) => d.x || 0).attr("cy", (d) => d.y || 0);
    labels.attr("x", (d) => d.x || 0).attr("y", (d) => d.y || 0);
  });
};

// Watch for aggregatedData changes and re-render the chart
watch(() => props.aggregatedData, async () => {
  await nextTick(); // Ensure Vue updates the DOM before D3 renders again
  renderChart();
}, { deep: true });

onMounted(renderChart);
</script>

<template>
  <svg ref="svgRef" width="100%" height="800"></svg>
</template>
