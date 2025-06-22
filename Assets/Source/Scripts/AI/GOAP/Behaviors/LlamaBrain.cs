using System;
using CrashKonijn.Goap.Behaviours;
using Source.AI.Sensors;
using UnityEngine;

namespace Source.AI.GOAP
{
	[RequireComponent(typeof(AgentBehaviour))]
	public class LlamaBrain : MonoBehaviour
	{
		[SerializeField] private PlayerSensor playerSensor;
		[SerializeField] private AttackConfig attackConfig;
		
		private AgentBehaviour agentBehaviour;
		private void Awake()
		{
			agentBehaviour = GetComponent<AgentBehaviour>();
		}

		private void OnEnable()
		{
			playerSensor.OnPlayerEnter += SetKillPlayerGoal;
			playerSensor.OnPlayerExit += SetWanderGoal;
		}

		private void OnDisable()
		{
			playerSensor.OnPlayerEnter -= SetKillPlayerGoal;
			playerSensor.OnPlayerExit -= SetWanderGoal;
		}

		private void Start()
		{
			agentBehaviour.SetGoal<WanderGoal>(false);

			playerSensor.Collider.radius = attackConfig.SensorRadius;
		}

		private void SetKillPlayerGoal(Transform player)
		{
			agentBehaviour.SetGoal<KillPlayerGoal>(true);
		}

		private void SetWanderGoal(Vector3 lastKnownPosition)
		{
			agentBehaviour.SetGoal<WanderGoal>(true);
		}
	}
}