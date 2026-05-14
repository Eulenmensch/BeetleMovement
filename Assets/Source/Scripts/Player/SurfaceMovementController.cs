using UnityEngine;
using UnityEngine.InputSystem;

namespace Source.Player
{
    // Kinematic rigidbody controller. Reads the Move action from the new Input System and
    // moves the player along the surface plane reported by SurfaceAligner.
    //
    // Owns all rotation: surface alignment and turn-to-face are computed together once per
    // physics step and applied via MoveRotation, so nothing else writes rotation at a
    // different rate and causes jitter.
    //
    // CapsuleCollider direction must be Y-Axis (default) so the capsule aligns with
    // transform.up as the character tilts to match the surface normal.
    [RequireComponent(typeof(Rigidbody), typeof(SurfaceAligner), typeof(CapsuleCollider))]
    public class SurfaceMovementController : MonoBehaviour
    {
        [SerializeField] private float                moveSpeed   = 5f;
        [SerializeField] private float                turnSpeed   = 10f;
        [SerializeField] private float                alignSpeed  = 8f;  // surface-normal alignment when idle
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
            Vector2 input  = moveAction.action.ReadValue<Vector2>();
            Vector3 normal = _aligner.SurfaceNormal;

            Vector3    desiredPos = _rb.position;
            Quaternion desiredRot;

            if (input.sqrMagnitude > 0.001f)
            {
                // Project camera axes onto the surface plane so WASD always feels grounded.
                Vector3 camFwd   = Vector3.ProjectOnPlane(Camera.main.transform.forward, normal).normalized;
                Vector3 camRight = Vector3.ProjectOnPlane(Camera.main.transform.right,   normal).normalized;
                Vector3 moveDir  = (camFwd * input.y + camRight * input.x).normalized;

                desiredPos += moveDir * (moveSpeed * Time.fixedDeltaTime);

                // Turn toward movement direction, up = surface normal.
                desiredRot = Quaternion.Slerp(_rb.rotation,
                                              Quaternion.LookRotation(moveDir, normal),
                                              turnSpeed * Time.fixedDeltaTime);
            }
            else
            {
                // When idle, re-align up to the surface normal while preserving forward direction.
                Vector3 fwdOnSurface = Vector3.ProjectOnPlane(_rb.rotation * Vector3.forward, normal);
                if (fwdOnSurface.sqrMagnitude < 0.001f)
                    fwdOnSurface = Vector3.ProjectOnPlane(Vector3.forward, normal);

                desiredRot = Quaternion.Slerp(_rb.rotation,
                                              Quaternion.LookRotation(fwdOnSurface.normalized, normal),
                                              alignSpeed * Time.fixedDeltaTime);
            }

            _rb.MovePosition(Depenetrate(desiredPos, desiredRot));
            _rb.MoveRotation(desiredRot);
        }

        private Vector3 Depenetrate(Vector3 desiredPos, Quaternion desiredRot)
        {
            // Capsule axis is local Y rotated to world space (requires CapsuleCollider direction = Y-Axis).
            float   halfLength    = Mathf.Max(0f, _capsule.height * 0.5f - _capsule.radius);
            Vector3 capsuleUp     = desiredRot * Vector3.up;
            Vector3 capsuleCenter = desiredPos + desiredRot * _capsule.center;
            Vector3 p1            = capsuleCenter - capsuleUp * halfLength;
            Vector3 p2            = capsuleCenter + capsuleUp * halfLength;

            int count = Physics.OverlapCapsuleNonAlloc(p1, p2, _capsule.radius,
                                                       _overlapBuffer, ~0,
                                                       QueryTriggerInteraction.Ignore);
            Vector3 correction = Vector3.zero;
            for (int i = 0; i < count; i++)
            {
                if (_overlapBuffer[i] == _capsule) continue;

                if (Physics.ComputePenetration(
                        _capsule,          desiredPos,                           desiredRot,
                        _overlapBuffer[i], _overlapBuffer[i].transform.position,
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
