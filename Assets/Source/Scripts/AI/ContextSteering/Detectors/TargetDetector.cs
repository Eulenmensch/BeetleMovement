using System.Collections.Generic;
using UnityEngine;

namespace Source.AI.ContextSteering
{
	public class TargetDetector : Detector
	{
		[SerializeField] private float targetDetectionRadius = 1f;
		[SerializeField] private LayerMask obstacleLayerMask, playerLayerMask;
		[SerializeField] private bool showGizmos;
		
		private List<Transform> _detectedTargets;
		
		public override void Detect(AIData aiData)
		{
			Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, targetDetectionRadius, playerLayerMask);
			if(playerCollider != null)
			{
				Vector2 direction = (playerCollider.transform.position - transform.position).normalized;
				RaycastHit2D hit  = Physics2D.Raycast(transform.position, direction, targetDetectionRadius, obstacleLayerMask);
				
				if (hit.collider != null && (playerLayerMask & (1 << hit.collider.gameObject.layer)) != 0)
				{
					Debug.DrawRay(transform.position, direction * targetDetectionRadius, Color.red);
					_detectedTargets = new List<Transform> {playerCollider.transform};
				}
				else
				{
					_detectedTargets = null;
				}
			}
			else
			{
				_detectedTargets = null;
			}
			aiData.targets = _detectedTargets;
		}

		private void OnDrawGizmosSelected()
		{
			if (!showGizmos) return;
			Gizmos.DrawWireSphere(transform.position, targetDetectionRadius);
			if (_detectedTargets == null) return;
			Gizmos.color = Color.green;
			foreach (var target in _detectedTargets)
			{
				Gizmos.DrawWireSphere(target.position, 0.3f);
			}
		}
	}
}