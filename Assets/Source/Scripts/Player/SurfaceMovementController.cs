using UnityEngine;

namespace Source.Player
{
    // Kinematic rigidbody controller. Reads WASD/gamepad input and moves the player along
    // the surface plane reported by SurfaceAligner. Camera-relative so the player moves
    // in the direction the camera faces on the current surface.
    [RequireComponent(typeof(Rigidbody), typeof(SurfaceAligner))]
    public class SurfaceMovementController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed  = 5f;
        [SerializeField] private float turnSpeed  = 10f;

        private Rigidbody      _rb;
        private SurfaceAligner _aligner;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.isKinematic  = true;
            _rb.interpolation = RigidbodyInterpolation.Interpolate;
            _aligner = GetComponent<SurfaceAligner>();
        }

        private void FixedUpdate()
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            if (Mathf.Approximately(h, 0f) && Mathf.Approximately(v, 0f)) return;

            // Project camera axes onto the surface plane so movement always feels grounded.
            Vector3 normal   = _aligner.SurfaceNormal;
            Vector3 camFwd   = Vector3.ProjectOnPlane(Camera.main.transform.forward, normal).normalized;
            Vector3 camRight = Vector3.ProjectOnPlane(Camera.main.transform.right,   normal).normalized;
            Vector3 moveDir  = (camFwd * v + camRight * h).normalized;

            _rb.MovePosition(_rb.position + moveDir * (moveSpeed * Time.fixedDeltaTime));

            if (moveDir.sqrMagnitude > 0.001f)
            {
                Quaternion look = Quaternion.LookRotation(moveDir, normal);
                transform.rotation = Quaternion.Slerp(transform.rotation, look, turnSpeed * Time.fixedDeltaTime);
            }
        }
    }
}
