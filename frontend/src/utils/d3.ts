import type { BubbleCloudEntry } from "@/types";
import * as d3 from "d3";

export const renderBubbleCloud = (svgElement: SVGSVGElement | null, nodes: BubbleCloudEntry[]) => {
  if (!svgElement || nodes.length === 0) return;

  const width = window.innerWidth;
  const height = 800;

  const svg = d3.select(svgElement);
  svg.selectAll("*").remove(); // Clear previous content

  // Create a container group for zooming and panning
  const container = svg.append("g");

  // Get min/max radius for color scaling
  const extent = d3.extent(nodes, (d) => d.radius) as [number, number];
  const minRadius = extent[0] ?? 5;
  const maxRadius = extent[1] ?? 50;
  const colorScale = d3.scaleSequential(d3.interpolatePlasma).domain([minRadius, maxRadius]);

  // Force Simulation
  d3.forceSimulation<BubbleCloudEntry>(nodes)
    .force("charge", d3.forceManyBody().strength(5))
    .force("collide", d3.forceCollide<BubbleCloudEntry>().radius((d) => d.radius + 2))
    .force("center", d3.forceCenter(width / 2, height / 2))
    .on("tick", ticked);

  // Add bubbles
  const bubbles = container
    .selectAll<SVGCircleElement, BubbleCloudEntry>("circle")
    .data(nodes)
    .enter()
    .append("circle")
    .attr("r", (d) => d.radius)
    .attr("fill", (d) => d3.color(colorScale(d.radius))?.darker(0.2).toString() || "#666")
    .attr("stroke", "#222")
    .attr("stroke-width", 1.5);

  // Add labels
  const labels = container
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

  function ticked() {
    bubbles.attr("cx", (d) => d.x ?? 0).attr("cy", (d) => d.y ?? 0);
    labels.attr("x", (d) => d.x ?? 0).attr("y", (d) => d.y ?? 0);
  }
};

//todo: should take up fixed height. when limiting search, the bar just takes up the whole chart.
export const renderSubredditBalances = (svgElement: SVGSVGElement | null, filteredData: { subreddit: string; balance: number; }[]) => {
  if (!svgElement || filteredData.length === 0) return;

  const width = window.innerWidth;
  const height = 600;
  const margin = { top: 20, right: 20, bottom: 50, left: 100 };

  const svg = d3.select(svgElement);
  svg.selectAll("*").remove(); // Clear previous chart

  svg.attr("width", width).attr("height", height);

  // X Scale
  const x = d3.scaleLinear()
    .domain([0, d3.max(filteredData, (d) => d.balance) || 1])
    .range([0, width - margin.left - margin.right]);

  // Y Scale
  const y = d3.scaleBand()
    .domain(filteredData.map((d) => d.subreddit))
    .range([0, height - margin.top - margin.bottom])
    .padding(0.2);

  const g = svg.append("g").attr("transform", `translate(${margin.left},${margin.top})`);

  // Bars
  g.selectAll("rect")
    .data(filteredData)
    .enter()
    .append("rect")
    .attr("y", (d) => y(d.subreddit)!)
    .attr("width", (d) => x(d.balance))
    .attr("height", y.bandwidth())
    .attr("fill", "#334d37");

  // Labels
  g.selectAll("text")
    .data(filteredData)
    .enter()
    .append("text")
    .attr("x", (d) => x(d.balance) - 5)
    .attr("y", (d) => y(d.subreddit)! + y.bandwidth() / 2)
    .attr("dy", "0.35em")
    .attr("text-anchor", "end")
    .attr("fill", "white")
    .text((d) => d.balance);

  // Axes
  g.append("g").call(d3.axisLeft(y));
  g.append("g")
    .attr("transform", `translate(0,${height - margin.top - margin.bottom})`)
    .call(d3.axisBottom(x));
};
