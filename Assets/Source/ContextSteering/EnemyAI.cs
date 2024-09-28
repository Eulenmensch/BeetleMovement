using System;
using System.Collections.Generic;
using UnityEngine;

namespace Source.ContextSteering
{
	public class EnemyAI : MonoBehaviour
	{
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
		}
	}
}