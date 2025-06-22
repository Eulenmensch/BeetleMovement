using System.Collections.Generic;
using UnityEngine;

namespace Source.AI.ContextSteering
{
	public static class Directions
	{
		public static List<Vector2> eight = new List<Vector2>
		{
			new Vector2(0, 1).normalized,
			new Vector2(1, 1).normalized,
			new Vector2(1, 0).normalized,
			new Vector2(1, -1).normalized,
			new Vector2(0, -1).normalized,
			new Vector2(-1, -1).normalized,
			new Vector2(-1, 0).normalized,
			new Vector2(-1, 1).normalized
		};
	}
}