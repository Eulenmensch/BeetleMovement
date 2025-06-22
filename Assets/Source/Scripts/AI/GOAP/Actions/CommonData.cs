using CrashKonijn.Goap.Interfaces;

namespace Source.AI.GOAP
{
	public class CommonData : IActionData
	{
		public ITarget Target { get; set; }
		public float Timer { get; set; }
	}
}