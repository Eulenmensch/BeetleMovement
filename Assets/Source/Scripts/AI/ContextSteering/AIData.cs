using System.Collections.Generic;
using UnityEngine;

namespace Source.AI.ContextSteering
{
	public class AIData : MonoBehaviour
	{
		public List<Transform> targets;
		public Collider2D[] obstacles;

		public Transform currentTarget;
		
		public int GetTargetsCount() => targets == null ? 0 : targets.Count;
	}
}