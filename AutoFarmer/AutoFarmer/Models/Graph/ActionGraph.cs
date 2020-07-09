using AutoFarmer.Models.Common;
using AutoFarmer.Models.Graph.ActionNodes;
using AutoFarmer.Models.Graph.ConditionEdges;
using AutoFarmer.Services.Logging;
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

		public ActionNode GetNextNode(ConditionEdge conditionEdge)
		{
			using var log = Logger.LogBlock();

			return ActionNodes.First(n => conditionEdge.EndNodeName == n.Name);
		}

		public ConditionEdge GetNextEdge(ActionNode actionNode)
		{
			using var log = Logger.LogBlock();

			return ConditionEdges.Where(e =>
				e.StartNodeName == actionNode.Name && e.IsEnabled).OrderBy(e =>
					e.Order).FirstOrDefault();
		}

		public void ResetStates()
		{
			using var log = Logger.LogBlock();

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
