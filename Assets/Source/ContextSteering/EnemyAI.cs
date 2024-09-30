using System;
using System.Collections.Generic;
using UnityEngine;

namespace Source.ContextSteering
{
	public class EnemyAI : MonoBehaviour
	{
		[SerializeField] private List<SteeringBehavior> steeringBehaviors;
		[SerializeField] private List<Detector> detectors;
		[SerializeField] private AIData aiData;
		[SerializeField] private float detectionInterval = 0.1f;

		private void Start()
		{
			InvokeRepeating("RunDetectors", 0, detectionInterval);
		}
		
		private void RunDetectors()
		{
			foreach (var detector in detectors)
			{
				detector.Detect(aiData);
			}

			float[] danger = new float[8];
			float[] interest = new float[8];

			foreach (var behavior in steeringBehaviors)
			{
				(danger, interest) = behavior.GetSteering(danger, interest, aiData);
			}
		}
	}
}