﻿using AutoFarmer.Models.Common;
using AutoFarmer.Models.Graph;
using AutoFarmer.Models.Graph.ActionNodes;
using AutoFarmer.Models.Graph.ConditionEdges;
using AutoFarmer.Services.InputHandling;
using AutoFarmer.Services.Logging;
using System.Collections.Generic;
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
			using var log = Logger.LogBlock();

			while (Graph.TryGetNextStartNode(out var currentNode))
			{
				Logger.Log($"Start node selected: {currentNode.Name}");

				MouseSafetyMeasures.Instance.LastActionPosition = MouseSafetyMeasures.GetCursorCurrentPosition();

				List<SerializablePoint> actionPoints = new List<SerializablePoint>();

				do
				{
					ProcessNode(currentNode, actionPoints.ToArray());

					ConditionEdge? currentEdge = null;

					while (Graph.TryGetNextEdge(currentNode, out currentEdge))
					{
						Logger.Log($"Next edge selected: {currentEdge.Name}");

						if (ProcessEdge(currentEdge, actionPoints))
						{
							ReportBuilder.ReportBuilder.Commit(ReportMessageType.Success);

							Graph.TryGetNextNode(currentEdge, ref currentNode);

							Logger.Log($"Next node selected: {currentNode.Name}");

							break;
						}
						else
						{
							ReportBuilder.ReportBuilder.Commit(ReportMessageType.Fail);
						}
					}

					if (currentNode.IsNot(ActionNodeFlags.EndNode) && currentEdge is null)
					{
						throw new AutoFarmerException($"Can not move to the next node from {currentNode.Name}");
					}
				}
				while (currentNode.IsNot(ActionNodeFlags.EndNode));

				ProcessNode(currentNode);

				Graph.ResetState();
			}
		}

		private void ProcessNode(ActionNode node, params SerializablePoint[] actionPositions)
		{
			using var log = Logger.LogBlock();

			if(node.Is(ActionNodeFlags.ResetTrigger))
			{
				Graph.ResetState();
			}

			if (node.Actions is null)
			{
				Thread.Sleep(node.AdditionalDelayAfterLastAction);

				node.CurrentCrossing++;

				return;
			}

			if (actionPositions.Length > 0)
			{
				foreach (var actionPosition in actionPositions)
				{
					InputSimulator.MoveEvent(actionPosition);

					InputSimulator.Simulate(node.Actions, actionPosition, node.AdditionalDelayBetweenActions);

					Thread.Sleep(node.AdditionalDelayAfterLastAction);
				}
			}
			else
			{
				InputSimulator.Simulate(node.Actions, null, node.AdditionalDelayBetweenActions);
			}

			node.CurrentCrossing++;
		}

		private bool ProcessEdge(ConditionEdge edge, List<SerializablePoint> actionPoints)
		{
			using var log = Logger.LogBlock();

			return edge.ProcessCondition(actionPoints);
		}
	}
}
