using CrashKonijn.Goap.Behaviours;
using CrashKonijn.Goap.Classes.Builders;
using CrashKonijn.Goap.Configs.Interfaces;
using CrashKonijn.Goap.Enums;
using CrashKonijn.Goap.Resolver;
using UnityEngine;

namespace Source.AI.GOAP
{
	[RequireComponent(typeof(DependencyInjector))]
	public class GoapSetConfigFactory : GoapSetFactoryBase
	{
		private DependencyInjector injector;
		
		public override IGoapSetConfig Create()
		{
			injector = GetComponent<DependencyInjector>();
			
			GoapSetBuilder builder = new("LlamaSet");

			BuildGoals(builder);
			BuildActions(builder);
			BuildSensors(builder);
			
			return builder.Build();
		}

		private void BuildGoals(GoapSetBuilder builder)
		{
			builder.AddGoal<WanderGoal>()
				.AddCondition<IsWandering>(Comparison.GreaterThanOrEqual, 1);
			
			builder.AddGoal<KillPlayerGoal>()
				.AddCondition<PlayerHealth>(Comparison.SmallerThanOrEqual, 0);
		}

		private void BuildActions(GoapSetBuilder builder)
		{
			builder.AddAction<WanderAction>()
				.SetTarget<WanderTarget>()
				.AddEffect<IsWandering>(EffectType.Increase)
				.SetBaseCost(5).SetInRange(10);

			builder.AddAction<MeleeAttackAction>()
				.SetTarget<PlayerTarget>()
				.AddEffect<PlayerHealth>(EffectType.Decrease)
				.SetBaseCost(injector.AttackConfig.MeleeAttackCost)
				.SetInRange(injector.AttackConfig.SensorRadius);
		}

		private void BuildSensors(GoapSetBuilder builder)
		{
			builder.AddTargetSensor<WanderTargetSensor>()
				.SetTarget<WanderTarget>();

			builder.AddTargetSensor<PlayerTargetSensor>()
				.SetTarget<PlayerTarget>();
		}
	}
}