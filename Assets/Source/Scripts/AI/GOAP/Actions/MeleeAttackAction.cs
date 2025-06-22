using CrashKonijn.Goap.Behaviours;
using CrashKonijn.Goap.Classes;
using CrashKonijn.Goap.Enums;
using CrashKonijn.Goap.Interfaces;
using UnityEngine;

namespace Source.AI.GOAP
{
	public class MeleeAttackAction : ActionBase<AttackData>, IInjectable
	{
		private AttackConfig attackConfig;
		public override void Created() { }

		public override void Start(IMonoAgent agent, AttackData data)
		{
			data.Timer = attackConfig.AttackDelay;
		}

		public override ActionRunState Perform(IMonoAgent agent, AttackData data, ActionContext context)
		{
			data.Timer -= context.DeltaTime;
			bool shouldAttack = data.Target != null && 
			                    Vector3.Distance(data.Target.Position, agent.transform.position) <= attackConfig.MeleeAttackRadus;

			if (shouldAttack)
			{
				agent.transform.LookAt(data.Target.Position);
			}

			return data.Timer > 0 ? ActionRunState.Continue : ActionRunState.Stop;
		}

		public override void End(IMonoAgent agent, AttackData data) { }

		public void Inject(DependencyInjector injector)
		{
			attackConfig = injector.AttackConfig;
		}
	}
}