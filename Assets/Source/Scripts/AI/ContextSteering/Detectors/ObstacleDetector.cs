using UnityEngine;

namespace Source.AI.ContextSteering
{
	public class ObstacleDetector : Detector
	{
		[SerializeField] private float detectionRadius = 1f;
		[SerializeField] private LayerMask layerMask;
		[SerializeField] private bool showGizmos;
		
		Collider2D[] _detectedColliders;
		
		public override void Detect(AIData aiData)
		{
			_detectedColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, layerMask);
			aiData.obstacles = _detectedColliders;
		}

		private void OnDrawGizmos()
		{
			if (!showGizmos) return;
			if (!Application.isPlaying) return;
			if (_detectedColliders == null) return;
			Gizmos.color = Color.red;
			foreach (var detectedCollider in _detectedColliders)
			{
				Gizmos.DrawWireSphere(detectedCollider.transform.position, 0.2f);
			}
		}
	}
}