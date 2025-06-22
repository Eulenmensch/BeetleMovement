using CrashKonijn.Goap.Behaviours;
using CrashKonijn.Goap.Interfaces;
using UnityEngine;

namespace Source.AI.GOAP
{
	[RequireComponent(typeof(AgentBehaviour), typeof(Rigidbody2D))]
	public class AgentMoveBehavior : MonoBehaviour
	{
		[SerializeField] private float speed;
		
		private AgentBehaviour agentBehaviour;
		private ITarget currentTarget;
		private Vector3 lastTarget;
		private float minMoveDistance;
		private Rigidbody2D rigidBody2D;
        
		private void Awake()
		{
			agentBehaviour = GetComponent<AgentBehaviour>();
			rigidBody2D = GetComponent<Rigidbody2D>();
		}

		private void OnEnable()
		{
			agentBehaviour.Events.OnTargetInRange += EventsOnTargetInRange;
			agentBehaviour.Events.OnTargetOutOfRange += EventsOnTargetOutOfRange;
			agentBehaviour.Events.OnTargetChanged += EventsOnTargetChanged;
		}

		private void OnDisable()
		{
			agentBehaviour.Events.OnTargetInRange -= EventsOnTargetInRange;
			agentBehaviour.Events.OnTargetOutOfRange -= EventsOnTargetOutOfRange;
			agentBehaviour.Events.OnTargetChanged -= EventsOnTargetChanged;
		}

		private void FixedUpdate()
		{
			if (currentTarget == null) return;

			if (Vector3.Distance(currentTarget.Position, lastTarget) >= minMoveDistance)
			{
				lastTarget = currentTarget.Position;
				var moveDirection = (currentTarget.Position - transform.position).normalized;
				moveDirection *= speed;
				rigidBody2D.velocity = moveDirection;
			}
		}

		private void EventsOnTargetInRange(ITarget target)
		{
		}

		private void EventsOnTargetOutOfRange(ITarget target)
		{
		}

		private void EventsOnTargetChanged(ITarget target, bool inrange)
		{
			currentTarget = target;
			lastTarget = currentTarget.Position;
		}
	}
}