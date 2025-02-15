import type { BubbleCloudEntry } from "@/types";
import * as d3 from "d3";

export const renderBubbleCloud = (svgElement: SVGSVGElement | null, nodes: BubbleCloudEntry[]) => {
  if (!svgElement || nodes.length === 0) return;

  const width = window.innerWidth;
  const height = 800;

  const svg = d3.select(svgElement);
  svg.selectAll("*").remove(); // Clear previous SVG content

  // Get min/max radius for scaling colors
  const extent = d3.extent(nodes, (d) => d.radius) as [number, number] | [undefined, undefined];
  const minRadius = extent[0] ?? 5;
  const maxRadius = extent[1] ?? 50;

  // Scale color based on radius
  const colorScale = d3.scaleSequential(d3.interpolatePlasma).domain([minRadius, maxRadius]);

  // Restart the simulation
  const simulation = d3
    .forceSimulation<BubbleCloudEntry>(nodes)
    .force("charge", d3.forceManyBody().strength(5))
    .force("collide", d3.forceCollide<BubbleCloudEntry>().radius((d) => d.radius + 2))
    .force("center", d3.forceCenter(width / 2, height / 2));

  const bubbles = svg
    .selectAll<SVGCircleElement, BubbleCloudEntry>("circle")
    .data(nodes)
    .enter()
    .append("circle")
    .attr("r", (d) => d.radius)
    .attr("fill", (d) => {
      const baseColor = d3.color(colorScale(d.radius));
      return baseColor ? baseColor.darker(0.2).toString() : "#666";
    })
    .attr("stroke", "#222")
    .attr("stroke-width", 1.5);

  // Add text labels with a black outline for visibility
  const labels = svg
    .selectAll<SVGTextElement, BubbleCloudEntry>("text")
    .data(nodes)
    .enter()
    .append("text")
    .attr("text-anchor", "middle")
    .attr("dy", 4)
    .attr("font-size", (d) => Math.max(d.radius / 3, 10) + "px")
    .attr("fill", "white")
    .attr("stroke", "black")
    .attr("stroke-width", 2)
    .attr("paint-order", "stroke")
    .style("pointer-events", "none")
    .text((d) => d.id);

  simulation.on("tick", () => {
    bubbles.attr("cx", (d) => d.x || 0).attr("cy", (d) => d.y || 0);
    labels.attr("x", (d) => d.x || 0).attr("y", (d) => d.y || 0);
  });
};
