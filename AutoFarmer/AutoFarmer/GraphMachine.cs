using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text.RegularExpressions;
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

				Graph.ResetStates();
			}
		}

		private bool GetNextOutgoingEdge(ActionNode currentNode, out ConditionEdge edge)
		{
			edge = Graph.GetNextEdge(currentNode);

			return edge != null;
		}

		private bool GetNextStartNode(out ActionNode node)
		{
			node = Graph.StartNodes.FirstOrDefault(n => !n.IsVisited);

			return node != null;
		}

		private void ProcessNode(ActionNode node, Point actionPosition)
		{
			if (node.Actions is null) return;

			InputSimulator.Simulate(node.Actions.InputActionNames, actionPosition, node.Actions.AdditionalDelayBetweenActions);

			Thread.Sleep(node.Actions.AdditionalDelayAfterLastAction);
		}

		private bool ProcessEdge(ConditionEdge edge)
		{
			var preRes = ProcessConditon(edge.Conditions.PreCondition);

			if (edge.Conditions.PreCondition.Equals(edge.Conditions.PostCondition))
			{
				if (preRes) edge.CurrentCrossing++;

				return preRes;
			}
			else
			{
				if (preRes is false) return false;

				var postRes = ProcessConditon(edge.Conditions.PostCondition);

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
			int retry = 0;

			while (retry <= condition.MaxRetry)
			{
				try
				{
					var template = ImageMatchFinder.Templates.First(t => t.Name == condition.TemplateName);

					var searchRectangle = template.SearchRectangles[condition.SearchRectangleName];

					MouseSafetyMeasures.Instance.CheckForIntentionalEmergencyStop();

					var sourceImage = ScreenshotMaker.CreateScreenshot();

					var clickPoint = ImageMatchFinder.FindClickPointForTemplate(sourceImage, template.Bitmap, searchRectangle);

					Logger.GraphicalLog(sourceImage, clickPoint, searchRectangle, condition.TemplateName, condition.SearchRectangleName);

					MouseSafetyMeasures.Instance.CheckForIntentionalEmergencyStop();

					InputSimulator.MouseEvent(clickPoint);

					break;
				}
				catch (ImageMatchNotFoundException)
				{
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
