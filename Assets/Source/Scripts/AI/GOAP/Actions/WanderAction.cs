using CrashKonijn.Goap.Behaviours;
using CrashKonijn.Goap.Classes;
using CrashKonijn.Goap.Enums;
using CrashKonijn.Goap.Interfaces;

namespace Source.AI.GOAP
{
	public class WanderAction : ActionBase<CommonData>
	{
		public override void Created() {}

		public override void Start(IMonoAgent agent, CommonData data)
		{
			data.Timer = 0.25f;
		}

		public override ActionRunState Perform(IMonoAgent agent, CommonData data, ActionContext context)
		{
			data.Timer -= context.DeltaTime;

			return data.Timer > 0 ? ActionRunState.Continue : ActionRunState.Stop;
		}

		public override void End(IMonoAgent agent, CommonData data) {}
	}
}