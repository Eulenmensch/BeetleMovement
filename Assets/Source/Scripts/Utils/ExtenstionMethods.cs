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
		
		public static bool InRangeOf(this Transform source, Transform target, float maxDistance, float maxAngle) {
			Vector3 directionToTarget = (target.position - source.position).With(y: 0); //TODO: currently filters out y axis
			return directionToTarget.magnitude <= maxDistance && Vector3.Angle(source.forward, directionToTarget) <= maxAngle / 2;
		}
		
		public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null) {
			return new Vector3(x ?? vector.x, y ?? vector.y, z ?? vector.z);
		}
	}
}