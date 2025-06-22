using CrashKonijn.Goap.Behaviours;
using UnityEngine;

namespace Source.AI.GOAP
{
	[RequireComponent(typeof(AgentBehaviour))]
	public class GoapSetBinder : MonoBehaviour
	{
		[SerializeField] private GoapRunnerBehaviour goapRunner;

		private void Awake()
		{
			AgentBehaviour agent = GetComponent<AgentBehaviour>();
			agent.GoapSet = goapRunner.GetGoapSet("LlamaSet");
		}
	}
}