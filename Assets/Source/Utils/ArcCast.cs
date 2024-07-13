using UnityEngine;

namespace Source.Utils
{
	public class ArcCast
	{
		static public bool Cast(Vector3 center, Quaternion rotation, float angle, float radius, int resolution, LayerMask layer, out RaycastHit hit)
		{
			rotation *= Quaternion.Euler(-angle / 2, 0, 0);
			for (int i =0; i<resolution; i++)
			{
				Vector3 pointStart = center + rotation * Vector3.forward * radius;
				rotation *= Quaternion.Euler(angle/resolution, 0,0);
				Vector3 pointEnd = center + rotation * Vector3.forward * radius;
				Vector3 arcSegment = pointEnd - pointStart;

				if (Physics.Raycast(pointStart, arcSegment, out hit, arcSegment.magnitude * 1.001f, layer))
					return true;
			}

			hit = new RaycastHit();
			return false;
		}
	}
}