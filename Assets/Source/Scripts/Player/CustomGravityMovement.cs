using Source.Gravity;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Source.Player
{
	public class CustomGravityMovement : MonoBehaviour
	{
		[SerializeField] private Camera playerCam;
		[SerializeField] private float movementSpeed;
		[SerializeField] private float sprintSpeed;
		
		public IGravityProvider GravityProvider { private get; set; }
		
		private Vector2 moveInput;
		private Vector3 gravity;
		private bool isSprinting;
		
		private void Start()
		{
			isSprinting = false;
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
				//TODO: move forward relative to camera on gravity up plane
				MoveRegular();
			}
			else
			{
				//TODO: enable rigidbody dynamic, push alongside gravity up plane in camera direction
				MoveHover();
			}
		}
		private void MoveRegular(){}
		private void MoveHover(){}

		private void UpdateGravity()
		{
			gravity = GravityProvider.GetGravity();
		}

		private void Rotate()
		{
			RotateTowardsMoveDirection();
			RotateToGravity();
		}

		private void RotateTowardsMoveDirection(){}

		private void RotateToGravity(){}
	}
}