using AutoFarmer.Models.Graph.ActionNodes;
using AutoFarmer.Models.Graph.ConditionEdges;
using AutoFarmer.Services;
using AutoFarmer.Services.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AutoFarmer.Models.Graph
{
	public class ActionGraph
	{
		public List<ActionNode> ActiveStartNodes
		{
			get
			{
				var activeStartNodes = ActionNodes.Where(a => a.IsStartNode && Config.Instance.ActiveStartNodes.Contains(a.Name));

				return activeStartNodes.OrderBy(n => Config.Instance.ActiveStartNodes.ToList().IndexOf(n.Name)).ToList();
			}
		}

		public List<ActionNode> EndNodes
		{
			get { return ActionNodes.Where(a => a.IsEndNode).ToList(); }
		}

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

			return graph;
		}

		public bool TryGetNextNode(ConditionEdge conditionEdge, out ActionNode nextNode)
		{
			using var log = Logger.LogBlock();

			nextNode = ActionNodes.FirstOrDefault(n => conditionEdge.EndNodeName == n.Name);

			return nextNode != null;
		}

		public bool TryGetNextEdge(ActionNode actionNode, out ConditionEdge nextEdge)
		{
			using var log = Logger.LogBlock();

			Random r = new Random();

			double minimumProbability = r.Next(1, 100) / 100d;

			var potentialEdges = ConditionEdges.Where(e => e.StartNodeName == actionNode.Name && e.IsEnabled);

			nextEdge = potentialEdges.Where(e => 
				e.ConsiderationProbability >= minimumProbability).OrderBy(e =>
					e.Order).FirstOrDefault();

			return nextEdge != null;
		}

		public bool TryGetNextStartNode(out ActionNode nextStartNode)
		{
			nextStartNode = ActiveStartNodes.FirstOrDefault(n => !n.IsVisited);

			return nextStartNode != null;
		}

		public void Reset(bool complete = false)
		{
			using var log = Logger.LogBlock();

			foreach (var edge in ConditionEdges)
			{
				edge.ResetState();
			}

			foreach (var node in ActionNodes)
			{
				node.ResetState(complete);
			}
		}
	}
}
