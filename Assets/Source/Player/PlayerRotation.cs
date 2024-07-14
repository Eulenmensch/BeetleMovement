using System.Collections.Generic;
using Source.Utils;
using UnityEngine;

namespace Source.Player
{
	public class PlayerRotation : MonoBehaviour
	{
		[SerializeField] private int rayDirections;
		[SerializeField] private int raysPerDirection;
		[SerializeField] private float raysRadius;
		[SerializeField] private int rayResolution;
		[SerializeField] private float arcAngle = 270f;
		[SerializeField] private LayerMask layerMask;
		[SerializeField] private GameObject raycastOrigin;
		[SerializeField] private bool drawGizmos;
		
		private List<RaycastHit> hits;
		
		private void Start()
		{
			hits = new List<RaycastHit>();
		}

		private void FixedUpdate()
		{
			CheckGroundNormals();
			SetRotation();
		}

		private void CheckGroundNormals()
		{
			hits.Clear();
			for (int i = 0; i < rayDirections; i++)
			{
				var yRotation = 360 / rayDirections * i;
				var rotation = transform.rotation * Quaternion.Euler(0, yRotation, 0);
				for (int j = 0; j < raysPerDirection; j++)
				{
					var radius = raysRadius - raysRadius / raysPerDirection * j;
					ArcCast.Cast(raycastOrigin.transform.position, rotation, arcAngle, radius, rayResolution, layerMask,
						out RaycastHit hit, drawGizmos);
					hits.Add(hit);
				}
			}
		}

		private void SetRotation()
		{
			var normalsSum = Vector3.zero;
			foreach (var hit in hits)
			{
				normalsSum += hit.normal;
			}

			var averageUpDirection = new Vector3(normalsSum.x / hits.Count, normalsSum.y / hits.Count,
				normalsSum.z / hits.Count);
			transform.rotation = Quaternion.FromToRotation(transform.up, averageUpDirection) * transform.rotation;
		}

		private void OnDrawGizmos()
		{
			if (!Application.isPlaying) return;
			foreach (var hit in hits)
			{
				Gizmos.color = new Color(161, 191, 78);
				Gizmos.DrawSphere(hit.point, 0.1f);
				Gizmos.color = Color.magenta;
				Gizmos.DrawLine(hit.point, hit.point + hit.normal * 0.2f);
			}
		}
	}
}