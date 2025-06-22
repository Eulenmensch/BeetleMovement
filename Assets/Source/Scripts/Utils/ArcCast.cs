using UnityEngine;

namespace Source.Utils
{
	public static class ArcCast
	{
		public static bool Cast(Vector3 center, Quaternion rotation, float angle, float radius, int resolution, LayerMask layer, out RaycastHit hit, bool drawGizmos = false)
		{
			rotation *= Quaternion.Euler(-angle/2, 0, 0);

			float dAngle = angle / resolution;
			Vector3 forwardRadius = Vector3.forward * radius;

			Vector3 A, B, AB;
			A = forwardRadius;
			B = Quaternion.Euler(dAngle, 0, 0) * forwardRadius;
			AB = B - A;
			float AB_magnitude = AB.magnitude * 1.001f;
			
			for (int i =0; i<resolution; i++)
			{
				A = center + rotation * forwardRadius;
				rotation *= Quaternion.Euler(dAngle, 0, 0);
				B = center + rotation * forwardRadius;
				AB = B - A;

				if (Physics.Raycast(A, AB, out hit, AB_magnitude, layer))
				{
					if (drawGizmos) {Debug.DrawLine(A, hit.point, Color.cyan);}
					return true;
				}
				if (drawGizmos) {Debug.DrawLine(A, B, Color.blue);}
			}

			hit = new RaycastHit();
			return false;
		}
	}
}