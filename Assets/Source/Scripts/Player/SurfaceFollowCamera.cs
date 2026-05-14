using UnityEngine;
using UnityEngine.InputSystem;

namespace Source.Player
{
    // Third-person orbit camera. Yaw and pitch are applied relative to the target's up vector
    // so the player always appears visually upright regardless of which surface face they occupy.
    //
    // The orbit reference direction is a fixed world-space vector, NOT target.forward.
    // Using target.forward creates a feedback loop: the movement controller rotates the character
    // toward the camera's forward, which shifts the reference frame, which shifts the camera,
    // which changes the movement direction, causing uncontrolled spinning at any turn speed > 0.
    public class SurfaceFollowCamera : MonoBehaviour
    {
        [SerializeField] private Transform            target;
        [SerializeField] private float                distance     = 6f;
        [SerializeField] private float                sensitivity  = 0.15f;
        [SerializeField] private float                pitchMin     = -15f;
        [SerializeField] private float                pitchMax     = 75f;
        [SerializeField] private float                positionLerp = 10f;
        [SerializeField] private InputActionReference lookAction;

        private float _yaw;
        private float _pitch = 20f;

        private void Start()
        {
            if (target == null) return;
            // Set initial yaw so the camera opens behind the character rather than at a fixed
            // world angle. After this, _yaw accumulates look input only — never reads target.forward.
            Vector3 charFwd = Vector3.ProjectOnPlane(target.forward, target.up).normalized;
            _yaw = Vector3.SignedAngle(WorldRef(target.up), charFwd, target.up);
        }

        private void OnEnable()  => lookAction.action.Enable();
        private void OnDisable() => lookAction.action.Disable();

        private void LateUpdate()
        {
            if (target == null) return;

            Vector2 look = lookAction.action.ReadValue<Vector2>();
            _yaw   += look.x * sensitivity;
            _pitch -= look.y * sensitivity;
            _pitch  = Mathf.Clamp(_pitch, pitchMin, pitchMax);

            Quaternion referenceFrame = Quaternion.LookRotation(WorldRef(target.up), target.up);
            Quaternion orbitRotation  = referenceFrame * Quaternion.Euler(_pitch, _yaw, 0f);

            Vector3 desiredPos = target.position + orbitRotation * new Vector3(0f, 0f, -distance);
            transform.position = Vector3.Lerp(transform.position, desiredPos, positionLerp * Time.deltaTime);
            transform.LookAt(target.position, target.up);
        }

        // A stable world-space direction on the surface tangent plane that does not depend
        // on where the character is facing. Falls back to world right if forward is parallel
        // to the surface normal (e.g. character standing on the +Z face of a sphere).
        private static Vector3 WorldRef(Vector3 surfaceUp)
        {
            Vector3 r = Vector3.ProjectOnPlane(Vector3.forward, surfaceUp);
            if (r.sqrMagnitude < 0.001f)
                r = Vector3.ProjectOnPlane(Vector3.right, surfaceUp);
            return r.normalized;
        }
    }
}
