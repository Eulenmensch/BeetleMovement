using System;
using System.Collections.Generic;
using UnityEngine;

namespace Source.AI.UtilityAI
{
	[RequireComponent(typeof(SphereCollider))]
	public abstract class Sensor : MonoBehaviour
	{
		public float detectionRadius = 10f;
		public List<string> targetTags = new();

		public Brain Brain { protected get; set; }
		
		protected readonly List<Transform> detectedObjects = new(10);
		private SphereCollider sphereCollider;

		protected virtual void Start()
		{
			sphereCollider = GetComponent<SphereCollider>();
			sphereCollider.isTrigger = true;
			sphereCollider.radius = detectionRadius;

			Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius);
			foreach (var other in colliders)
			{
				if (other.gameObject == gameObject) continue;
				ProcessTrigger(other, item => detectedObjects.Add(item));
			}
			
		}

		protected virtual void OnTriggerEnter(Collider other)
		{
			ProcessTrigger(other, item => detectedObjects.Add(item));
		}

		protected virtual void OnTriggerExit(Collider other)
		{
			ProcessTrigger(other, item => detectedObjects.Remove(item));
		}

		private void ProcessTrigger(Collider other, Action<Transform> action)
		{
			if (other.CompareTag("Untagged")) return;

			foreach (var otherTag in targetTags)
			{
				if (other.CompareTag(otherTag))
				{
					action(other.transform);
				}
			}
		}

		public Transform GetClosestTarget(string targetTag)
		{
			if (detectedObjects.Count == 0) return null;

			Transform closestTarget = null;
			float closestDistanceSquared = Mathf.Infinity;
			Vector3 currentPosition = transform.position;

			foreach (var potentialTarget in detectedObjects)
			{
				if (potentialTarget.CompareTag(targetTag))
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