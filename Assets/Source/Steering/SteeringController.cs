using UnityEngine;

namespace Source.Steering
{
	[RequireComponent(typeof(Rigidbody))]
	public class SteeringController : MonoBehaviour
	{
		[field: SerializeField] public float MaxAcceleration { get; set; }
		[field: SerializeField] public float MaxAngularAcceleration { get; set; }
		[field: SerializeField] public float Drag { get; set; }
		private SteeringBehavior[] _steerings;
		public Rigidbody RigidB { get; set; }

		private void Start()
		{
			RigidB = GetComponent<Rigidbody>();
			_steerings = GetComponents<SteeringBehavior>();
			RigidB.linearDamping = Drag;
		}

		private void FixedUpdate()
		{
			Vector3 acceleration = Vector3.zero;
			float rotation = 0f;
			foreach (var steering in _steerings)
			{
				SteeringData steeringData = steering.GetSteering(this);
				acceleration += steeringData.Linear * steering.Weight;
				rotation += steeringData.Angular *steering.Weight;
			}

			if (acceleration.magnitude >= MaxAcceleration)
			{
				acceleration.Normalize();
				acceleration *= MaxAcceleration;
			}
			
			RigidB.AddForce(acceleration);
			
			// make the agent face the direction it is moving
			rotation = Mathf.Atan2(RigidB.linearVelocity.y, RigidB.linearVelocity.x) * Mathf.Rad2Deg;
			// smooth the rotation based on the max angular acceleration
			rotation = Mathf.MoveTowardsAngle(RigidB.rotation.eulerAngles.z, rotation, MaxAngularAcceleration);
			
			if (rotation != 0)
			{
				RigidB.rotation = Quaternion.Euler(0, 0, rotation);
			}
		}
	}
}