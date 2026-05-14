using UnityEngine;

namespace Source.Player
{
    // Raycasts downward each frame to track the surface normal beneath the character.
    // Does NOT modify transform.rotation — rotation is the controller's responsibility,
    // so there is only one owner of rotation and no cross-rate fighting.
    public class SurfaceAligner : MonoBehaviour
    {
        [SerializeField] private float     castDistance      = 1.5f;
        [SerializeField] private float     castOriginOffset  = 0.2f;
        [SerializeField] private LayerMask surfaceLayers     = ~0;

        public Vector3 SurfaceNormal { get; private set; } = Vector3.up;
        public bool    OnSurface     { get; private set; }

        private void Update()
        {
            Vector3 origin = transform.position + transform.up * castOriginOffset;
            OnSurface = Physics.Raycast(origin, -transform.up, out RaycastHit hit,
                                        castDistance + castOriginOffset, surfaceLayers);
            if (OnSurface)
                SurfaceNormal = hit.normal;
        }
    }
}
