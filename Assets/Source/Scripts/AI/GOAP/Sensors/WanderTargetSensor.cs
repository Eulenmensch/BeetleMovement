using CrashKonijn.Goap.Classes;
using CrashKonijn.Goap.Interfaces;
using CrashKonijn.Goap.Sensors;
using UnityEngine;

namespace Source.AI.GOAP
{
	public class WanderTargetSensor : LocalTargetSensorBase
	{
		public override void Created() {}

		public override void Update() {}

		public override ITarget Sense(IMonoAgent agent, IComponentReference references)
		{
			var position = GetRandomPosition(agent);
			return new PositionTarget(position);
		}
		
		Vector3 GetRandomPosition(IMonoAgent agent)
		{
			var random = Random.insideUnitCircle * 1f;
			var offset = agent.gameObject.GetComponent<Rigidbody2D>().velocity.normalized * 2;
			var position = agent.transform.position + (Vector3)offset + (Vector3)random;
			Debug.Log(offset);
			return position;
		}
	}
}