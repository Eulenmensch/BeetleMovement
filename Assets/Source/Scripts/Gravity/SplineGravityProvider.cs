using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Source.Gravity
{
	[RequireComponent(typeof(SplineContainer))]
	public class SplineGravityProvider : MonoBehaviour, IGravityProvider
	{
		private Spline spline;

		private void Start()
		{
			spline = GetComponent<SplineContainer>().Spline;
		}

		public Vector3 GetGravityDirection(Vector3 objectPosition)
		{
			SplineUtility.GetNearestPoint(spline, objectPosition, out float3 point, out float t);
			var nearestPointOnSpline = new Vector3(point.x, point.y, point.z);
			return (nearestPointOnSpline - objectPosition).normalized;
		}
	}
}