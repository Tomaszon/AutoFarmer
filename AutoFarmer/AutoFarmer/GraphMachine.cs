using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace AutoFarmer
{
	public class GraphMachine
	{
		public Graph Graph { get; set; }

		public MouseSafetyMeasures MouseSafetyMeasures { get; set; }

		public ImageMatchFinder ImageMatchFinder { get; set; }

		public InputSimulator InputSimulator { get; set; } = new InputSimulator();

		private int _currentStartNodeIndex = 0;

		public GraphMachine(Config config, Graph graph)
		{
			MouseSafetyMeasures = config.MouseSafetyMeasures;

			ImageMatchFinder = ImageMatchFinder.FromConfig(config);

			Graph = graph;
		}

		public void Process()
		{
			while (GetNextStartNode(out var currentNode))
			{
				InputSimulator.MouseEvent(MouseSafetyMeasures.MouseSafePosition);

				bool processedAnOutgoingEdge = false;
				ConditionEdge currentEdge;

				do
				{
					ProcessNode(currentNode);

					while (GetNextOutgoingEdge(currentNode, out currentEdge))
					{
						if (ProcessEdge(currentEdge))
						{
							currentNode = Graph.GetNextNode(currentEdge);
							processedAnOutgoingEdge = true;
							break;
						}
					}

					if (!processedAnOutgoingEdge) throw new AutoFarmerException("Can not move to the next node!");
				}
				while (currentEdge != null);
			}
		}

		private bool GetNextOutgoingEdge(ActionNode currentNode, out ConditionEdge edge)
		{

		}

		private bool GetNextStartNode(out ActionNode node)
		{
			string name = Graph.StartNodeNames.Length > _currentStartNodeIndex ? Graph.StartNodeNames[_currentStartNodeIndex] : null;

			node = name != null ? Graph.ActionNodes[name] : null;

			_currentStartNodeIndex++;

			return node != null;
		}

		private void ProcessNode(ActionNode node)
		{
			if (node.Actions is null) return;

			InputSimulator.Simulate(node.Actions.InputActionNames, node.Actions.AdditionalDelayBetweenActions);

			Thread.Sleep(node.Actions.AdditionalDelayAfterLastAction);
		}

		private bool ProcessEdge(ConditionEdge edge)
		{
			try
			{

			}
			catch (ImageMatchNotFoundException ex)
			{

			}
			catch (ImageMatchAmbiguousException ex)
			{
				throw new AutoFarmerException("Automatic emergency stop!", ex);
			}
		}
	}
}
