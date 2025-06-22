using UnityEngine;

namespace Source.AI.GOAP
{
	[CreateAssetMenu(fileName = "Attack Config", menuName = "AI/Attack Config", order = 0)]
	public class AttackConfig : ScriptableObject
	{
		public float SensorRadius = 10f;
		public float MeleeAttackRadus = 1f;
		public int MeleeAttackCost = 1;
		public float AttackDelay = 1f;
		public LayerMask AttackableLayerMask;
		public ContactFilter2D contactFilter;
	}
}