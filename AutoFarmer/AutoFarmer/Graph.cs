﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace AutoFarmer
{
	public class Graph
	{
		public List<ActionNode> StartNodes
		{
			get { return ActionNodes.Where(a => a.IsStartNode).ToList(); }
		}

		public List<ActionNode> EndNodes
		{
			get { return ActionNodes.Where(a => a.IsEndNode).ToList(); }
		}

		public List<ActionNode> ActionNodes { get; set; } = new List<ActionNode>();

		public List<ConditionEdge> ConditionEdges { get; set; } = new List<ConditionEdge>();

		public static Graph FromConfig()
		{
			var graph = new Graph();

			foreach (var file in Directory.GetFiles(Config.Instance.ActionNodesDirectory))
			{
				graph.ActionNodes.Add(ActionNode.FromJsonFile(file));
			}
			foreach (var file in Directory.GetFiles(Config.Instance.ConditionEdgesDirectory))
			{
				graph.ConditionEdges.Add(ConditionEdge.FromJsonFile(file));
			}

			return graph;
		}

		public ActionNode GetNextNode(ConditionEdge conditionEdge)
		{
			return ActionNodes.First(n => conditionEdge.EndNodeName == n.Name);
		}

		public ConditionEdge GetNextEdge(ActionNode actionNode)
		{
			return ConditionEdges.Where(e =>
				e.StartNodeName == actionNode.Name && e.IsEnabled).OrderBy(e =>
					e.Order).FirstOrDefault();
		}

		public void ResetStates()
		{
			foreach (var edge in ConditionEdges)
			{
				edge.ResetState();
			}

			foreach (var node in ActionNodes)
			{
				node.ResetState();
			}
		}
	}
}
