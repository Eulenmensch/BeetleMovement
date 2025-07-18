using System.Collections.Generic;
using UnityEngine;

namespace Source.AI.UtilityAI
{
	public class AphidSensor : Sensor
	{
		public List<string> threatTags = new();
		
		protected override void Start()
		{
			base.Start();
			DetectThreat();
		}

		protected override void OnTriggerEnter(Collider other)
		{
			base.OnTriggerEnter(other);
			DetectThreat();
		}

		protected override void OnTriggerExit(Collider other)
		{
			base.OnTriggerExit(other);
			DetectThreat();
		}

		private void DetectThreat()
		{
			var threats = new List<Transform>();
			foreach (var detectedObject in detectedObjects)
			{
				if (threatTags.Contains(detectedObject.tag))
				{
					threats.Add(detectedObject.transform);
				}
			}

			var threatDirection = Vector3.zero;
			foreach (var threat in threats)
			{
				threatDirection += threat.position - transform.position;
			}
			threatDirection =  threatDirection.normalized;
			Brain.agentBlackboard.Set(AphidKeys.ThreatDirection , threatDirection);

			var threatLevel = 0f;
			foreach (var threat in threats)
			{
				var threatDistance = Vector3.Distance(transform.position, threat.position);
				var threatProximity = detectionRadius - threatDistance;
				var normalizedThreat = threatProximity / detectionRadius;
				threatLevel += normalizedThreat;
			}
			Brain.agentBlackboard.Set(AphidKeys.ThreatLevel , threatLevel);
		}
	}
}