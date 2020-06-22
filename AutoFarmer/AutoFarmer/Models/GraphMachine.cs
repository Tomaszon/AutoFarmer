using AutoFarmer.Models.GraphNamespace;
using AutoFarmer.Models.ImageMatching;
using AutoFarmer.Models.InputHandling;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace AutoFarmer.Models
{
	public class GraphMachine
	{
		public Graph Graph { get; set; }

		public InputSimulator InputSimulator { get; set; } = new InputSimulator();

		public GraphMachine(Graph graph)
		{
			Graph = graph;
		}

		public void Process()
		{
			while (GetNextStartNode(out var currentNode))
			{
				MouseSafetyMeasures.Instance.LastActionPosition = MouseSafetyMeasures.GetCursorCurrentPosition();

				List<Point> actionPoints = new List<Point>() { MouseSafetyMeasures.GetCursorCurrentPosition() };

				do
				{
					ProcessNode(currentNode, actionPoints.ToArray());

					ConditionEdge currentEdge;

					while (GetNextOutgoingEdge(currentNode, out currentEdge))
					{
						if (ProcessEdge(currentEdge, out actionPoints))
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

				ProcessNode(currentNode, MouseSafetyMeasures.Instance.LastActionPosition);

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

		private void ProcessNode(ActionNode node, params Point[] actionPositions)
		{
			node.IsVisited = true;

			if (node.Actions is null) return;

			foreach (var actionPosition in actionPositions)
			{
				InputSimulator.MoveMouseTo(actionPosition);

				InputSimulator.Simulate(node.Actions.InputActionNames, actionPosition, node.Actions.AdditionalDelayBetweenActions);

				Thread.Sleep(node.Actions.AdditionalDelayAfterLastAction);
			}
		}

		private bool ProcessEdge(ConditionEdge edge, out List<Point> actionPoints)
		{
			actionPoints = new List<Point>() { MouseSafetyMeasures.Instance.LastActionPosition };

			if (edge.Conditions is null) return true;

			return edge.Process(out actionPoints);
		}
	}
}
