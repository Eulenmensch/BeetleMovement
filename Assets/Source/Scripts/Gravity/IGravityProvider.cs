using UnityEngine;

namespace Source.Gravity
{
	public interface IGravityProvider
	{
		public Vector3 GetGravityDirection(Vector3 objectPosition);
	}
}