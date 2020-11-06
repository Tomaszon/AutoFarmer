using AutoFarmer.Models.Common;
using AutoFarmer.Models.Graph.ActionNodes;
using AutoFarmer.Models.Graph.ConditionEdges;
using AutoFarmer.Services;
using AutoFarmer.Services.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AutoFarmer.Models.Graph
{
	public class ActionGraph
	{
		public List<ActionNode> ActionNodes { get; set; } = new List<ActionNode>();

		public List<ConditionEdge> ConditionEdges { get; set; } = new List<ConditionEdge>();

		public static ActionGraph FromConfig()
		{
			var graph = new ActionGraph();

			foreach (var file in Directory.GetFiles(Config.Instance.ActionNodesDirectory))
			{
				graph.ActionNodes.AddRange(ActionNodeOptions.FromJsonFile(file));
			}
			foreach (var file in Directory.GetFiles(Config.Instance.ConditionEdgesDirectory))
			{
				graph.ConditionEdges.AddRange(ConditionEdgeOptions.FromJsonFile(file));
			}

			var config = ActionGraphConfig.FromJsonFile(Config.Instance.ActionGraphConfigPath);

			foreach (var item in config.AddFlags.Nodes)
			{
				graph.ActionNodes.Single(n => n.Name == item.Key).AddFlags(item.Value);
			}
			foreach (var item in config.AddFlags.Edges)
			{
				graph.ConditionEdges.Single(e => e.Name == item.Key).AddFlags(item.Value);
			}
			foreach (var item in config.RemoveFlags.Nodes)
			{
				graph.ActionNodes.Single(n => n.Name == item.Key).RemoveFlags(item.Value);
			}
			foreach (var item in config.RemoveFlags.Edges)
			{
				graph.ConditionEdges.Single(e => e.Name == item.Key).RemoveFlags(item.Value);
			}

			foreach (var item in config.StartNodeVisitCounts)
			{
				graph.ActionNodes.Single(n => n.Name == item.Key && n.Is(ActionNodeFlags.StartNode)).MaxCrossing = item.Value;
			}

			return graph;
		}

		public bool TryGetNextNode(ConditionEdge conditionEdge, ref ActionNode nextNode)
		{
			using var log = Logger.LogBlock();

			var asd = nextNode.Name;

			foreach (var edge in ConditionEdges.Where(e => e.StartNodeName == asd))
			{
				edge.RemoveFlags(ConditionEdgeFlags.Tried);
			}

			nextNode = ActionNodes.FirstOrDefault(n => conditionEdge.EndNodeName == n.Name);

			return nextNode is { };
		}

		public bool TryGetNextEdge(ActionNode actionNode, out ConditionEdge nextEdge)
		{
			using var log = Logger.LogBlock();

			Random r = new Random();

			double minimumProbability = r.Next(1, 100) / 100d;

			var potentialEdges = ConditionEdges.Where(e =>
				e.StartNodeName == actionNode.Name && e.Crossable && e.IsNot(ConditionEdgeFlags.Tried) &&
				(e.Is(ConditionEdgeFlags.Enabled | ConditionEdgeFlags.Switch) || e.IsNot(ConditionEdgeFlags.Switch)) &&
				e.ConsiderationProbability >= minimumProbability);

			var minPreferredOrder = potentialEdges.Count() > 0 ? potentialEdges.Min(x => x.PreferredOrder) : 0;

			potentialEdges = potentialEdges.OrderBy(e => e.Order).ThenBy(e =>
				e.PreferredOrder == minPreferredOrder ? 0 : r.Next(1, 100));

			nextEdge = potentialEdges.FirstOrDefault();

			if (nextEdge is { })
			{
				nextEdge.AddFlags(ConditionEdgeFlags.Tried);

				Logger.Log($"{nextEdge.Name} is marked for current iteration");

				return true;
			}
			else
			{
				return false;
			}
		}

		public bool TryGetNextStartNode(out ActionNode nextStartNode)
		{
			nextStartNode = ActionNodes.FirstOrDefault(n => n.Crossable && n.Is(ActionNodeFlags.Enabled | ActionNodeFlags.StartNode));

			return nextStartNode is { };
		}

		public void ResetState(bool complete = false)
		{
			using var log = Logger.LogBlock();

			foreach (var edge in ConditionEdges)
			{
				edge.ResetState(complete);
			}

			foreach (var node in ActionNodes)
			{
				node.ResetState(complete);
			}
		}
	}
}
