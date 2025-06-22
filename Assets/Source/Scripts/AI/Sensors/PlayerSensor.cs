using System;
using Source.Player;
using UnityEngine;

namespace Source.AI.Sensors
{
	[RequireComponent(typeof(CircleCollider2D))]
	public class PlayerSensor : MonoBehaviour
	{
		[field: SerializeField] public CircleCollider2D Collider { get; set; }
		
		public event Action<Transform> OnPlayerEnter;
		public event Action<Vector3> OnPlayerExit;
		
		private void Awake()
		{
			Collider = GetComponent<CircleCollider2D>();
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.TryGetComponent(out PlayerMovement2D player)) OnPlayerEnter?.Invoke(player.transform);
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if (other.TryGetComponent(out PlayerMovement2D player)) OnPlayerExit?.Invoke(player.transform.position);
		}
	}
}