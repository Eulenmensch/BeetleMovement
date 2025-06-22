using UnityEngine;

namespace Source.Utils
{
	public static class ExtenstionMethods
	{
		public static T GetOrAdd<T>(this GameObject go) where T : Component
		{
			//Attempt to find the component on the object
			T newComponent = go.GetComponent<T>();

			//If it doesn't exist, create a new one
			if(newComponent == null)
			{
				newComponent = go.AddComponent<T>();
			}

			//Return the component
			return newComponent;
		}
	}
}