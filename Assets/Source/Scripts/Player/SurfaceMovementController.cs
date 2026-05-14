using UnityEngine;
using UnityEngine.InputSystem;

namespace Source.Player
{
    // Kinematic rigidbody controller. Reads the Move action from the new Input System and
    // moves the player along the surface plane reported by SurfaceAligner.
    // Depenetration runs every physics step to prevent clipping into walls and geometry.
    //
    // CapsuleCollider direction must be set to Y-Axis (default) so the capsule aligns with
    // transform.up as the character rotates to match the surface normal.
    [RequireComponent(typeof(Rigidbody), typeof(SurfaceAligner), typeof(CapsuleCollider))]
    public class SurfaceMovementController : MonoBehaviour
    {
        [SerializeField] private float               moveSpeed   = 5f;
        [SerializeField] private float               turnSpeed   = 10f;
        [SerializeField] private InputActionReference moveAction;

        private Rigidbody       _rb;
        private SurfaceAligner  _aligner;
        private CapsuleCollider _capsule;

        private readonly Collider[] _overlapBuffer = new Collider[8];

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.isKinematic   = true;
            _rb.interpolation = RigidbodyInterpolation.Interpolate;
            _aligner = GetComponent<SurfaceAligner>();
            _capsule = GetComponent<CapsuleCollider>();
        }

        private void OnEnable()  => moveAction.action.Enable();
        private void OnDisable() => moveAction.action.Disable();

        private void FixedUpdate()
        {
            Vector2 input      = moveAction.action.ReadValue<Vector2>();
            Vector3 desiredPos = _rb.position;

            if (input.sqrMagnitude > 0.001f)
            {
                // Project camera axes onto the surface plane so WASD always feels grounded.
                Vector3 normal   = _aligner.SurfaceNormal;
                Vector3 camFwd   = Vector3.ProjectOnPlane(Camera.main.transform.forward, normal).normalized;
                Vector3 camRight = Vector3.ProjectOnPlane(Camera.main.transform.right,   normal).normalized;
                Vector3 moveDir  = (camFwd * input.y + camRight * input.x).normalized;

                desiredPos += moveDir * (moveSpeed * Time.fixedDeltaTime);

                if (moveDir.sqrMagnitude > 0.001f)
                {
                    Quaternion look = Quaternion.LookRotation(moveDir, normal);
                    transform.rotation = Quaternion.Slerp(transform.rotation, look, turnSpeed * Time.fixedDeltaTime);
                }
            }

            _rb.MovePosition(Depenetrate(desiredPos));
        }

        // Resolves any overlap between the capsule at desiredPos and nearby colliders,
        // returning a corrected position that avoids intersection.
        private Vector3 Depenetrate(Vector3 desiredPos)
        {
            // Capsule axis is transform.up (requires CapsuleCollider direction = Y-Axis).
            // Half-length is how far each sphere center is from the capsule center.
            float   halfLength    = Mathf.Max(0f, _capsule.height * 0.5f - _capsule.radius);
            Vector3 capsuleCenter = desiredPos + transform.rotation * _capsule.center;
            Vector3 p1            = capsuleCenter - transform.up * halfLength;
            Vector3 p2            = capsuleCenter + transform.up * halfLength;

            int count = Physics.OverlapCapsuleNonAlloc(p1, p2, _capsule.radius,
                                                       _overlapBuffer, ~0,
                                                       QueryTriggerInteraction.Ignore);
            Vector3 correction = Vector3.zero;
            for (int i = 0; i < count; i++)
            {
                if (_overlapBuffer[i] == _capsule) continue;

                if (Physics.ComputePenetration(
                        _capsule,               desiredPos,                  transform.rotation,
                        _overlapBuffer[i],      _overlapBuffer[i].transform.position,
                        _overlapBuffer[i].transform.rotation,
                        out Vector3 dir, out float dist))
                {
                    correction += dir * dist;
                }
            }
            return desiredPos + correction;
        }
    }
}
