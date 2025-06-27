using System;
using System.Collections.Generic;
using UnityEngine;

namespace Source.AI.UtilityAI
{
	[RequireComponent(typeof(SphereCollider))]
	public class Sensor : MonoBehaviour
	{
		public float detectionRadius = 10f;
		public List<string> targetTags = new();

		private readonly List<Transform> detectedObjects = new(10);
		private SphereCollider sphereCollider;

		private void Start()
		{
			sphereCollider = GetComponent<SphereCollider>();
			sphereCollider.isTrigger = true;
			sphereCollider.radius = detectionRadius;

			Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius);
			foreach (var other in colliders)
			{
				ProcessTrigger(other, item => detectedObjects.Add(item));
			}
			
		}

		private void OnTriggerEnter(Collider other)
		{
			ProcessTrigger(other, item => detectedObjects.Add(item));
		}

		private void OnTriggerExit(Collider other)
		{
			ProcessTrigger(other, item => detectedObjects.Remove(item));
		}

		void ProcessTrigger(Collider other, Action<Transform> action)
		{
			if (other.CompareTag("Untagged")) return;

			foreach (var tag in targetTags)
			{
				if (other.CompareTag(tag))
				{
					action(other.transform);
				}
			}
		}

		public Transform GetClosestTarget(string tag)
		{
			if (detectedObjects.Count == 0) return null;

			Transform closestTarget = null;
			float closestDistanceSquared = Mathf.Infinity;
			Vector3 currentPosition = transform.position;

			foreach (var potentialTarget in detectedObjects)
			{
				if (potentialTarget.CompareTag(tag))
				{
					Vector3 directionToTarget = potentialTarget.position - currentPosition;
					float distanceSquared = directionToTarget.sqrMagnitude;
					if (distanceSquared < closestDistanceSquared)
					{
						closestDistanceSquared = distanceSquared;
						closestTarget = potentialTarget;
					}
				}
			}

			return closestTarget;
		}
	}
}