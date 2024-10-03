using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Source.ContextSteering
{
	public class EnemyAI : MonoBehaviour
	{
		[SerializeField] private List<SteeringBehavior> steeringBehaviors;
		[SerializeField] private List<Detector> detectors;
		[SerializeField] private AIData aiData;
		[SerializeField] private ContextSolver contextSolver;
		[SerializeField] private float detectionRate = 0.1f;
		[SerializeField] private float aiUpdateRate = 0.06f;

		public UnityEvent<Vector2> OnMove, OnTarget;
		
		private Vector2 movementDirection;
		private bool isFollowing;
		
		private void Start()
		{
			InvokeRepeating("RunDetectors", 0, detectionRate);
		}
		
		private void RunDetectors()
		{
			foreach (var detector in detectors)
			{
				detector.Detect(aiData);
			}
		}

		private void Update()
		{
			if (aiData.currentTarget)
			{
				OnTarget?.Invoke(aiData.currentTarget.position);
				if (!isFollowing)
				{
					isFollowing = true;
					StartCoroutine(Follow());
				}
			}
			else if (aiData.GetTargetsCount() > 0)
			{
				aiData.currentTarget = aiData.targets[0];
			}
			OnMove?.Invoke(movementDirection);
		}

		private IEnumerator Follow()
		{
			if (!aiData.currentTarget)
			{
				movementDirection = Vector2.zero;
				isFollowing = false;
				yield break;
			}
			
			movementDirection = contextSolver.GetMovementDirection(steeringBehaviors, aiData);
			yield return new WaitForSeconds(aiUpdateRate);
			StartCoroutine(Follow());
		}
	}
}