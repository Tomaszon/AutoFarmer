using AutoFarmer.Models.Common;
using AutoFarmer.Models.Graph;
using AutoFarmer.Models.Graph.ActionNodes;
using AutoFarmer.Models.Graph.ConditionEdges;
using AutoFarmer.Services.InputHandling;
using AutoFarmer.Services.Logging;
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
			using var log = Logger.LogBlock();

			while (Graph.TryGetNextStartNode(out var currentNode))
			{
				Logger.Log($"Start node selected: {currentNode.Name}");

				MouseSafetyMeasures.Instance.LastActionPosition = MouseSafetyMeasures.GetCursorCurrentPosition();

				List<SerializablePoint> actionPoints = new List<SerializablePoint>();

				do
				{
					ProcessNode(currentNode, actionPoints.ToArray());

					ConditionEdge currentEdge = null;

					while (Graph.TryGetNextEdge(currentNode, out currentEdge))
					{
						Logger.Log($"Next edge selected: {currentEdge.Name}");

						if (ProcessEdge(currentEdge, actionPoints))
						{
							ReportBuilder.ReportBuilder.Commit(ReportMessageType.Success);

							Graph.TryGetNextNode(currentEdge, out currentNode);

							Logger.Log($"Next node selected: {currentNode.Name}");

							break;
						}
						else
						{
							ReportBuilder.ReportBuilder.Commit(ReportMessageType.Fail);

							currentEdge.Disable();
						}
					}

					Logger.Log($"No edge left to select!");

					if (!currentNode.IsEndNode && currentEdge is null) throw new AutoFarmerException($"Can not move to the next node from {currentNode.Name}!");
				}
				while (!currentNode.IsEndNode);

				ProcessNode(currentNode);

				Graph.ResetStates();
			}
		}

		private void ProcessNode(ActionNode node, params SerializablePoint[] actionPositions)
		{
			using var log = Logger.LogBlock();

			node.IsVisited = true;

			if (node.ActionNames is null)
			{
				Thread.Sleep(node.AdditionalDelayAfterLastAction);

				return;
			}

			if(actionPositions.Length > 0)
			{
				foreach (var actionPosition in actionPositions)
				{
					InputSimulator.MoveMouseTo(actionPosition);

					InputSimulator.Simulate(node.ActionNames, actionPosition, node.AdditionalDelayBetweenActions);

					Thread.Sleep(node.AdditionalDelayAfterLastAction);
				}
			}
			else
			{
				InputSimulator.Simulate(node.ActionNames, null, node.AdditionalDelayBetweenActions);
			}
		}

		private bool ProcessEdge(ConditionEdge edge, List<SerializablePoint> actionPoints)
		{
			using var log = Logger.LogBlock();

			if (edge.Condition is null)
			{
				actionPoints.Clear();

				return true;
			}

			return edge.ProcessCondition(actionPoints);
		}
	}
}
