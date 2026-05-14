using UnityEngine;

namespace Source.Player
{
    // Third-person orbit camera. Yaw and pitch are applied relative to the target's up vector
    // so the player always appears visually upright regardless of which surface face they occupy.
    public class SurfaceFollowCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float distance         = 6f;
        [SerializeField] private float sensitivity      = 3f;
        [SerializeField] private float pitchMin         = -15f;
        [SerializeField] private float pitchMax         = 75f;
        [SerializeField] private float positionLerp     = 10f;

        private float _yaw;
        private float _pitch = 20f;

        private void LateUpdate()
        {
            if (target == null) return;

            _yaw   += Input.GetAxis("Mouse X") * sensitivity;
            _pitch -= Input.GetAxis("Mouse Y") * sensitivity;
            _pitch  = Mathf.Clamp(_pitch, pitchMin, pitchMax);

            // Build a reference frame aligned to the target's surface up, then apply yaw/pitch within it.
            // ProjectOnPlane handles the case where target.forward drifts out of the surface tangent plane.
            Vector3 fwdOnSurface = Vector3.ProjectOnPlane(target.forward, target.up);
            if (fwdOnSurface.sqrMagnitude < 0.001f) fwdOnSurface = target.forward;

            Quaternion referenceFrame = Quaternion.LookRotation(fwdOnSurface, target.up);
            Quaternion orbitRotation  = referenceFrame * Quaternion.Euler(_pitch, _yaw, 0f);

            Vector3 desiredPos = target.position + orbitRotation * new Vector3(0f, 0f, -distance);
            transform.position = Vector3.Lerp(transform.position, desiredPos, positionLerp * Time.deltaTime);

            // Always look at the target using the target's surface up so the horizon stays level.
            transform.LookAt(target.position, target.up);
        }
    }
}
