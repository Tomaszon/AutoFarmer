using AutoFarmer.Models.Common;
using AutoFarmer.Models.Graph;
using AutoFarmer.Models.Graph.ActionNodes;
using AutoFarmer.Models.Graph.ConditionEdges;
using AutoFarmer.Services.InputHandling;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AutoFarmer.Services
{
	public class GraphMachine
	{
		public ActionGraph Graph { get; set; }

		public InputSimulator InputSimulator { get; set; } = new InputSimulator();

		public GraphMachine(ActionGraph graph)
		{
			Graph = graph;
		}

		public void Process()
		{
			while (GetNextStartNode(out var currentNode))
			{
				MouseSafetyMeasures.Instance.LastActionPosition = MouseSafetyMeasures.GetCursorCurrentPosition();

				List<SerializablePoint> actionPoints = new List<SerializablePoint>();

				do
				{
					ProcessNode(currentNode, actionPoints.ToArray());

					ConditionEdge currentEdge;

					while (GetNextOutgoingEdge(currentNode, out currentEdge))
					{
						if (ProcessEdge(currentEdge, actionPoints))
						{
							currentNode = Graph.GetNextNode(currentEdge);

							Logger.Log($"Next node selected: {currentNode.Name}");

							break;
						}
						else
						{
							currentEdge.Disable();
						}
					}

					if (!currentNode.IsEndNode && currentEdge is null) throw new AutoFarmerException("Can not move to the next node!");
				}
				while (!currentNode.IsEndNode);

				ProcessNode(currentNode);

				Graph.ResetStates();
			}
		}

		private bool GetNextOutgoingEdge(ActionNode currentNode, out ConditionEdge edge)
		{
			edge = Graph.GetNextEdge(currentNode);

			Logger.Log($"Next edge selected: {(edge is null ? "none" : edge.Name)}");

			return edge != null;
		}

		private bool GetNextStartNode(out ActionNode node)
		{
			node = Graph.ActiveStartNodes.FirstOrDefault(n => !n.IsVisited);

			Logger.Log($"Start node selected: {(node is null ? "none" : node.Name)}");

			return node != null;
		}

		private void ProcessNode(ActionNode node, params SerializablePoint[] actionPositions)
		{
			node.IsVisited = true;

			if (node.ActionNames is null)
			{
				Thread.Sleep(node.AdditionalDelayAfterLastAction);

				return;
			}

			if (actionPositions.Length == 0)
			{
				actionPositions = new[] { MouseSafetyMeasures.GetCursorCurrentPosition() };
			}

			foreach (var actionPosition in actionPositions)
			{
				InputSimulator.MoveMouseTo(actionPosition);

				InputSimulator.Simulate(node.ActionNames, actionPosition, node.AdditionalDelayBetweenActions);

				Thread.Sleep(node.AdditionalDelayAfterLastAction);
			}
		}

		private bool ProcessEdge(ConditionEdge edge, List<SerializablePoint> actionPoints)
		{
			if (edge.Condition is null)
			{
				actionPoints.Clear();

				return true;
			}

			return edge.ProcessCondition(actionPoints);
		}
	}
}
