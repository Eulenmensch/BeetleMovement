using UnityEngine;

namespace Source.Steering
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class SteeringController : MonoBehaviour
	{
		[field: SerializeField] public float MaxAcceleration { get; set; }
		[field: SerializeField] public float MaxAngularAcceleration { get; set; }
		[field: SerializeField] public float Drag { get; set; }
		private SteeringBehavior[] _steerings;
		public Rigidbody2D RigidB { get; set; }

		private void Start()
		{
			RigidB = GetComponent<Rigidbody2D>();
			_steerings = GetComponents<SteeringBehavior>();
			RigidB.drag = Drag;
		}

		public void Move(Vector2 moveDirection)
		{
			Vector2 acceleration = Vector2.zero;

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
			float rotation;
		
			// make the agent face the direction it is moving
			rotation = Mathf.Atan2(RigidB.velocity.y, RigidB.velocity.x) * Mathf.Rad2Deg;
			// smooth the rotation based on the max angular acceleration
			rotation = Mathf.MoveTowardsAngle(RigidB.rotation, rotation, MaxAngularAcceleration);
			
			if (rotation != 0)
			{
				RigidB.rotation = rotation;
			}
		}
	}
}