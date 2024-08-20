using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Source.Player
{
	public class PlayerMovement2D : MonoBehaviour
	{
		[SerializeField] private float acceleration;
		[SerializeField] private float maxSpeed;
		
		private Rigidbody _rigidbody;
		private Vector3 _moveDirection;
		private float _moveInputMagnitude;
		private bool _moving;

		private Rigidbody2D rb;
		
		private void Start()
		{
			rb = GetComponent<Rigidbody2D>();
		}

		private void FixedUpdate()
		{
			if (_moving)
			{
				var force = _moveDirection * (acceleration * _moveInputMagnitude);
				rb.AddForce(force, ForceMode2D.Impulse);
			}
			else
			{
				rb.AddForce(-rb.velocity, ForceMode2D.Impulse);
			}
			if (rb.velocity.magnitude > maxSpeed)
			{
				rb.velocity = rb.velocity.normalized * maxSpeed;
			}
			
			if(!_moving) return;
			rb.SetRotation( Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg);
		}

		public void Move(InputAction.CallbackContext context)
		{
			if (context.performed)
			{
				var inputDirection = context.ReadValue<Vector2>();
				_moveDirection = inputDirection;
				_moveInputMagnitude = inputDirection.magnitude;
				_moving = true;
			}

			if (context.canceled)
			{
				_moveInputMagnitude = 0f;
				_moving = false;
			}
		}
	}
}