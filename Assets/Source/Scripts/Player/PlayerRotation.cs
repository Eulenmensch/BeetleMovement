using System.Collections.Generic;
using Source.Utils;
using Unity.Mathematics;
using UnityEngine;

namespace Source.Player
{
	[RequireComponent(typeof(Rigidbody))]
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
		[SerializeField] private GameObject visuals;
		[SerializeField] private float visualsSlerpTime;
		
		private List<RaycastHit> hits;
		private Rigidbody _rigidbody;
		
		private void Start()
		{
			hits = new List<RaycastHit>();
			_rigidbody = GetComponent<Rigidbody>();
		}

		private void FixedUpdate()
		{
			CheckGroundNormals();
			SetPlayerRotation();
			SetVisualsRotation();
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

		private void SetPlayerRotation()
		{
			var normalsSum = Vector3.zero;
			foreach (var hit in hits)
			{
				normalsSum += hit.normal;
			}

			var averageUpDirection = new Vector3(normalsSum.x / hits.Count, normalsSum.y / hits.Count,
				normalsSum.z / hits.Count);
			_rigidbody.MoveRotation(Quaternion.FromToRotation(transform.up, averageUpDirection) * transform.rotation);
		}

		private void SetVisualsRotation()
		{
			if (_rigidbody.linearVelocity.magnitude <= 0) return;
			var lookRotation= Quaternion.LookRotation(_rigidbody.linearVelocity, transform.up) * Quaternion.Euler(0, 90, 0);
			visuals.transform.rotation = Quaternion.Slerp(visuals.transform.rotation, lookRotation, visualsSlerpTime);
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