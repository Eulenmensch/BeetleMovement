using Source.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Source.Player
{
	public class PlayerMovement : MonoBehaviour
	{
		[SerializeField] private float moveSpeed;
		[SerializeField] private float moveAcceleration;
		[SerializeField] private float rotationSpeed;
		[SerializeField] private GameObject raycastOrigin;
		[SerializeField] private int arcAngle;
		[SerializeField] private LayerMask layerMask;
		[SerializeField] private bool drawGizmos;


		private Rigidbody _rigidbody;
		private Vector3 _moveDirection;
		private float _moveInputMagnitude;
		private bool _moving;
		private float _currentAcceleration;

		private void Start()
		{
			_rigidbody = GetComponent<Rigidbody>();
		}

		private void FixedUpdate()
		{
			if (_moving)
			{
				if(_currentAcceleration <= 1) {_currentAcceleration += moveAcceleration;}
				var target = Vector3.Lerp(transform.position, GetMoveGroundPos(_moveDirection), _currentAcceleration);
				_rigidbody.MovePosition(target);
			}
		}

		public void Move(InputAction.CallbackContext context)
		{
			if (context.performed)
			{
				var inputDirection = context.ReadValue<Vector2>();
				var direction = new Vector3(inputDirection.x, 0, inputDirection.y);
				_moveDirection = direction;
				_moveInputMagnitude = direction.magnitude;
				_moving = true;
			}

			if (context.canceled)
			{
				_moveInputMagnitude = 0f;
				_currentAcceleration = 0f;
				_moving = false;
			}
		}

		private Vector3 GetMoveGroundPos(Vector3 direction)
		{
			var rotation = transform.rotation * Quaternion.LookRotation(direction, Vector3.up);
			var hitSomething = ArcCast.Cast(raycastOrigin.transform.position, rotation, arcAngle, moveSpeed * _moveInputMagnitude, 8, layerMask, out RaycastHit hit, drawGizmos);
			Debug.Log(hitSomething);
			if (!hitSomething)
			{
				return transform.position;
			}
			return hit.point;
		}
	}
}