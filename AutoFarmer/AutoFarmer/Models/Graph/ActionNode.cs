using System.Collections.Generic;

namespace AutoFarmer.Models.GraphNamespace
{
	public class ActionNode
	{
		public string Name { get; set; }

		public NodeActions Actions { get; set; }

		public bool IsStartNode { get; set; }

		public bool IsEndNode { get; set; }

		public bool IsVisited { get; set; }

		public void ResetState()
		{
			if (!IsStartNode)
			{
				IsVisited = false;
			}
		}

		public static List<ActionNode> FromOptions(ActionNodeOptions options)
		{
			List<ActionNode> result = new List<ActionNode>();

			foreach (var name in options.Names)
			{
				result.Add(new ActionNode()
				{
					Name = name,
					Actions = options.Actions,
					IsEndNode = options.IsEndNode,
					IsVisited = options.IsVisited,
					IsStartNode = options.IsStartNode
				});
			}

			return result;
		}
	}
}
