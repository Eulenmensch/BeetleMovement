﻿using UnityEngine;

namespace Source.AI.Steering
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class SteeringController : MonoBehaviour
	{
		[field: SerializeField] public float MaxAcceleration { get; set; }
		[field: SerializeField] public float MaxAngularAcceleration { get; set; }
		[field: SerializeField] public float Drag { get; set; }
		public Rigidbody2D RigidB { get; set; }

		private void Start()
		{
			RigidB = GetComponent<Rigidbody2D>();
 			RigidB.drag = Drag;
		}

		public void Move(Vector2 moveDirection)
		{
			var acceleration = Vector2.zero;

			acceleration += moveDirection;
			
			if (acceleration.magnitude >= MaxAcceleration)
			{
				acceleration.Normalize();
				acceleration *= MaxAcceleration;
			}
			RigidB.AddForce(acceleration, ForceMode2D.Impulse);
		}

		// public void Rotate(Vector2 faceDirection)
		// {
		// 	var lookAt = new Vector3(faceDirection.x, 0, faceDirection.y);
		// 	transform.LookAt(lookAt);
		// }

		private void FixedUpdate()
		{
			var rotation =
				// make the agent face the direction it is moving
				Mathf.Atan2(RigidB.velocity.y, RigidB.velocity.x) * Mathf.Rad2Deg;
			// smooth the rotation based on the max angular acceleration
			rotation = Mathf.MoveTowardsAngle(RigidB.rotation, rotation, MaxAngularAcceleration);
			
			if (rotation != 0)
			{
				RigidB.rotation = rotation;
			}
		}
	}
}