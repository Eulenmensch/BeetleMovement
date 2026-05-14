using UnityEngine;

namespace Source.Player
{
    // Detects the surface beneath the character via downward raycast and smoothly rotates
    // transform.up toward the hit normal. Used by the player controller.
    public class SurfaceAligner : MonoBehaviour
    {
        [SerializeField] private float alignSpeed       = 8f;
        [SerializeField] private float castDistance     = 1.5f;
        [SerializeField] private float castOriginOffset = 0.2f; // lift origin above self to avoid self-intersection
        [SerializeField] private LayerMask surfaceLayers = ~0;

        public Vector3 SurfaceNormal { get; private set; } = Vector3.up;
        public bool    OnSurface     { get; private set; }

        private void Update()
        {
            Vector3 origin = transform.position + transform.up * castOriginOffset;
            if (Physics.Raycast(origin, -transform.up, out RaycastHit hit,
                                castDistance + castOriginOffset, surfaceLayers))
            {
                SurfaceNormal = hit.normal;
                OnSurface     = true;
            }
            else
            {
                OnSurface = false;
            }

            // Rotate so transform.up matches the surface normal, preserving forward direction.
            Quaternion target = Quaternion.FromToRotation(transform.up, SurfaceNormal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, target, alignSpeed * Time.deltaTime);
        }
    }
}
