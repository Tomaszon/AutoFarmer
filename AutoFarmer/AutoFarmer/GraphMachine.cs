using System.Drawing;
using System.Linq;
using System.Threading;

namespace AutoFarmer
{
	public class GraphMachine
	{
		public Graph Graph { get; set; }

		public ImageMatchFinder ImageMatchFinder { get; set; }

		public InputSimulator InputSimulator { get; set; } = new InputSimulator();

		public GraphMachine(Graph graph)
		{
			ImageMatchFinder = ImageMatchFinder.FromConfig();

			Graph = graph;
		}

		public void Process()
		{
			while (GetNextStartNode(out var currentNode))
			{
				do
				{
					ProcessNode(currentNode, MouseSafetyMeasures.Instance.GetCursorCurrentPosition());

					InputSimulator.MouseEvent(MouseSafetyMeasures.Instance.MouseSafePosition);

					ConditionEdge currentEdge;

					while (GetNextOutgoingEdge(currentNode, out currentEdge))
					{
						if (ProcessEdge(currentEdge))
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

				ProcessNode(currentNode, MouseSafetyMeasures.Instance.GetCursorCurrentPosition());

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
			node = Graph.StartNodes.FirstOrDefault(n => !n.IsVisited);

			Logger.Log($"Start node selected: {(node is null ? "none" : node.Name)}");

			return node != null;
		}

		private void ProcessNode(ActionNode node, Point actionPosition)
		{
			node.IsVisited = true;

			if (node.Actions is null) return;

			InputSimulator.Simulate(node.Actions.InputActionNames, actionPosition, node.Actions.AdditionalDelayBetweenActions);

			Thread.Sleep(node.Actions.AdditionalDelayAfterLastAction);
		}

		private bool ProcessEdge(ConditionEdge edge)
		{
			if (edge.Conditions is null) return true;

			var preRes = ProcessConditon(edge.Conditions.PreCondition);

			Logger.Log($"Precondition processed: {preRes}");

			if (edge.Conditions.PreCondition?.Equals(edge.Conditions.PostCondition) == true)
			{
				if (preRes) edge.CurrentCrossing++;

				return preRes;
			}
			else
			{
				if (preRes is false) return false;

				var postRes = ProcessConditon(edge.Conditions.PostCondition);

				Logger.Log($"Postcondition processed: {postRes}");

				if (postRes)
				{
					edge.CurrentCrossing++;

					return true;
				}
				else
				{
					throw new AutoFarmerException($"Stuck in {edge.Name} condition edge!");
				}
			}
		}

		private bool ProcessConditon(ImageFindCondition condition)
		{
			if (condition is null) return true;

			int retry = 0;

			Logger.Log($"Attempring to find {condition.SearchRectangleName} of {condition.TemplateName} max {condition.MaxRetry + 1} times");

			while (retry <= condition.MaxRetry)
			{
				try
				{


					MouseSafetyMeasures.Instance.CheckForIntentionalEmergencyStop();

					var sourceImage = ScreenshotMaker.CreateScreenshot();

					var clickPoint = ImageMatchFinder.FindClickPointForTemplate(sourceImage, condition.TemplateName, condition.SearchRectangleName);

					MouseSafetyMeasures.Instance.CheckForIntentionalEmergencyStop();

					InputSimulator.MouseEvent(clickPoint);

					break;
				}
				catch (ImageMatchNotFoundException)
				{
					retry++;

					Logger.Log($"Match not found for the {retry}. time!", NotificationType.Error);

					Thread.Sleep(condition.RetryDelay);
				}
				catch (ImageMatchAmbiguousException ex)
				{
					throw new AutoFarmerException("Automatic emergency stop!", ex);
				}
			}

			return retry <= condition.MaxRetry;
		}
	}
}
