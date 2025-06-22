using System.Collections.Generic;
using UnityEngine;

namespace Source.AI.ContextSteering
{
	public class BoidDetector : Detector
	{
		[SerializeField] private float targetDetectionRadius = 1f;
		[SerializeField] private LayerMask AgentMask;

		private List<Transform> targets;
		
		public override void Detect(AIData aiData)
		{
			var results = Physics2D.OverlapCircleAll(transform.position, targetDetectionRadius, AgentMask);
			foreach (var detectedCollider in results)
			{
				if (detectedCollider.GetComponentInChildren<BoidDetector>() && detectedCollider.gameObject != gameObject)
				{
					targets.Add(detectedCollider.transform);
				}
			}

			aiData.targets = targets;
		}
	}
}