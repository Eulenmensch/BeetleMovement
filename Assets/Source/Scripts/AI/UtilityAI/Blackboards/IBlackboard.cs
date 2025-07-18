namespace Source.AI.UtilityAI
{
	public interface IBlackboard
	{
		void Set<T>(string key, T value);
		T Get<T>(string key);
		bool Has(string key);
	}
}