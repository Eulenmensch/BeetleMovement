using Sirenix.OdinInspector;
using Source.Gravity;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Source.Player
{
	[RequireComponent(typeof(Rigidbody))]
	public class CustomGravityMovement : SerializedMonoBehaviour
	{
		[SerializeField] private Camera playerCam;
		[SerializeField] private IGravityProvider startGravityProvider;
		[SerializeField] private float movementSpeed;
		[SerializeField] private float sprintSpeed;
		[SerializeField, Range(0,1)] private float moveInputDeadZone;
		[SerializeField] private float gravityAcceleration;
		
		public IGravityProvider GravityProvider { private get; set; }
		
		private Vector2 moveInput;
		private Vector3 gravityDirection;
		private bool isSprinting;
		
		private Rigidbody rb;
		
		private void Awake()
		{
			isSprinting = false;
			rb = GetComponent<Rigidbody>();
		}

		private void Start()
		{
			GravityProvider = startGravityProvider;
		}

		private void FixedUpdate()
		{
			UpdateGravity();
			Rotate();
			Move();
		}

		#region Input Handling

		public void HandleMoveInput(InputAction.CallbackContext context)
		{
			if (context.performed)
			{
				moveInput = context.ReadValue<Vector2>();
			}

			if (context.canceled)
			{
				moveInput = Vector2.zero;
			}
		}

		public void HandleSprintInput(InputAction.CallbackContext context)
		{
			if (context.started)
				isSprinting = true;
			if (context.canceled)
				isSprinting = false;
		}

		#endregion

		private void Move()
		{
			if (!isSprinting)
			{
				MoveRegular();
			}
			else
			{
				MoveHover();
			}
		}

		private void MoveRegular()
		{
			//TODO: move forward relative to camera on gravityDirection up plane
			var cam = playerCam.transform;
			var move = cam.forward * moveInput.y + cam.right * moveInput.x;
			move = Vector3.ProjectOnPlane(move, transform.up);
			rb.MovePosition(transform.position + move * movementSpeed);
		}

		private void MoveHover()
		{
			//TODO: enable rigidbody dynamic, push alongside gravityDirection up plane in camera direction
		}

		private void UpdateGravity()
		{
			gravityDirection = GravityProvider.GetGravityDirection(transform.position);
		}

		private void Rotate()
		{
			// RotateTowardsMoveDirection();
			RotateToGravity();
		}

		private void RotateTowardsMoveDirection()
		{
			var targetRotation = Quaternion.LookRotation(rb.linearVelocity, transform.up);
			rb.MoveRotation(targetRotation);
			
		}

		private void RotateToGravity()
		{
			var targetRotation = Quaternion.LookRotation(transform.forward, -gravityDirection);
			// if (moveInput.magnitude > moveInputDeadZone)
			// {
			// 	var lateralVelocity = Vector3.ProjectOnPlane(rb.linearVelocity, transform.up);
			// 	targetRotation = Quaternion.LookRotation(lateralVelocity, -gravityDirection);	
			// }
			
			rb.MoveRotation(targetRotation);
		}
	}
}