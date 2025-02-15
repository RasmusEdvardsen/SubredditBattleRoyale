<script setup lang="ts">
import { onMounted, ref, watch, computed, nextTick } from "vue";
import { renderBubbleCloud } from "@/utils/d3";
import type { BubbleCloudEntry } from "@/types";

// Define Props
const props = defineProps<{aggregatedData: Record<string, number>}>();

// Ref for the SVG container
const svgRef = ref<SVGSVGElement | null>(null);

// Convert aggregatedData into a reactive node list
const nodes = computed<BubbleCloudEntry[]>(() =>
  Object.entries(props.aggregatedData).map(([id, value]) => ({
    id,
    value,
    radius: Math.sqrt(value) * 2, // Scale the bubble size
  }))
);

// Watch for aggregatedData changes and re-render the chart
watch(() => props.aggregatedData, async () => {
  await nextTick(); // Ensure Vue updates the DOM before D3 renders again
  renderBubbleCloud(svgRef.value, nodes.value);
}, { deep: true });

onMounted(renderBubbleCloud);
</script>

<template>
  <svg ref="svgRef" width="100%" height="800"></svg>
</template>
