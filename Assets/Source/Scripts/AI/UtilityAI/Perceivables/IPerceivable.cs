namespace Source.AI.UtilityAI
{
	public interface IPerceivable
	{
		string PerceptType { get; }
		Percept GetPercept();
	}
}