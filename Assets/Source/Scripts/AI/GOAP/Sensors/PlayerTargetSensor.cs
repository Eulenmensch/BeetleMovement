using CrashKonijn.Goap.Classes;
using CrashKonijn.Goap.Interfaces;
using CrashKonijn.Goap.Sensors;
using UnityEngine;

namespace Source.AI.GOAP
{
	public class PlayerTargetSensor : LocalTargetSensorBase, IInjectable
	{
		private AttackConfig attackConfig;
		private Collider2D[] colliders = new Collider2D[1];
		
		public override void Created() { }

		public override void Update() { }

		public override ITarget Sense(IMonoAgent agent, IComponentReference references)
		{
			if (Physics2D.OverlapCircle(agent.transform.position, attackConfig.SensorRadius, attackConfig.contactFilter, colliders) > 0)
			{
				return new TransformTarget(colliders[0].transform);
			}

			return null;
		}

		public void Inject(DependencyInjector injector)
		{
			attackConfig = injector.AttackConfig;
		}
	}
}