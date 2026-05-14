using System.Collections.Generic;
using Pathfinding;
using Source.Beetles.Actions;
using UnityEngine;

namespace Source.Beetles
{
    // Abstract base for cattle and predator beetles. Owns:
    //   - Surface snapping (raycast + up-vector alignment each frame)
    //   - A* path requesting and waypoint following
    //   - Boid-style separation to prevent stacking
    //   - Staggered utility-AI ticks so not all beetles update on the same frame
    //
    // Subclasses only need to implement IsPredator and CreateActions().
    [RequireComponent(typeof(BeetleHunger), typeof(BeetleSensor), typeof(Seeker))]
    public abstract class BeetleBrain : MonoBehaviour
    {
        [Header("Surface")]
        [SerializeField] private float     castDistance      = 1.5f;
        [SerializeField] private float     castOriginOffset  = 0.2f;
        [SerializeField] private float     alignSpeed        = 8f;
        [SerializeField] private LayerMask surfaceLayers     = ~0;

        [Header("Locomotion")]
        [SerializeField] private float     moveSpeed              = 3f;
        [SerializeField] private float     waypointReachDistance  = 0.4f;
        [SerializeField] private float     separationRadius       = 1.2f;
        [SerializeField] private float     separationWeight       = 0.5f;
        [SerializeField] private LayerMask beetleLayer;

        [Header("Stuck Detection")]
        [SerializeField] private float stuckCheckInterval = 2f;
        [SerializeField] private float stuckThreshold     = 0.1f;

        [Header("AI")]
        [SerializeField, Range(0.1f, 0.5f)] private float _updateInterval = 0.2f;

        // Exposed so actions can reference the current surface normal and update interval.
        public Vector3 SurfaceNormal   { get; private set; } = Vector3.up;
        public float   UpdateInterval  => _updateInterval;
        public BeetleBlackboard Board  { get; private set; }
        public BeetleHunger     Hunger { get; private set; }

        private BeetleSensor       _sensor;
        private Seeker             _seeker;
        private List<BeetleAction> _actions;

        private List<Vector3> _waypoints    = new();
        private int           _waypointIndex;
        private bool          _pendingPath;
        private Vector3       _requestedDestination;

        private Vector3 _lastPosition;
        private float   _nextStuckCheck;
        private float   _nextAIUpdate;

        private readonly Collider[] _sepBuffer = new Collider[16];

        protected abstract bool              IsPredator    { get; }
        protected abstract List<BeetleAction> CreateActions();

        private void Awake()
        {
            Board   = new BeetleBlackboard();
            Hunger  = GetComponent<BeetleHunger>();
            _sensor = GetComponent<BeetleSensor>();
            _seeker = GetComponent<Seeker>();

            _sensor.Init(Board, IsPredator);

            // Random initial offset so beetles don't all tick on frame 0.
            _nextAIUpdate = Time.time + Random.Range(0f, _updateInterval);
        }

        private void Start()
        {
            _actions = CreateActions();
            _lastPosition = transform.position;
        }

        private void Update()
        {
            if (Hunger.IsMaxHunger) return;

            SnapToSurface();
            CheckStuck();

            if (Time.time >= _nextAIUpdate)
            {
                _nextAIUpdate = Time.time + _updateInterval;
                TickAI();
            }

            FollowPath();
        }

        // --- Surface snapping ---

        private void SnapToSurface()
        {
            Vector3 origin = transform.position + transform.up * castOriginOffset;
            if (!Physics.Raycast(origin, -transform.up, out RaycastHit hit,
                                 castDistance + castOriginOffset, surfaceLayers)) return;

            SurfaceNormal      = hit.normal;
            transform.position = hit.point;

            // Align up vector to surface normal while preserving current forward.
            Quaternion aligned = Quaternion.FromToRotation(transform.up, SurfaceNormal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, aligned, alignSpeed * Time.deltaTime);
        }

        // --- AI ---

        private void TickAI()
        {
            _sensor.Sense();
            Board.Set(BK.Hunger, Hunger.Value);

            BeetleAction best     = null;
            float        topScore = float.MinValue;

            foreach (var action in _actions)
            {
                if (!action.IsValid(Board)) continue;
                float score = action.Score(Board);
                if (score > topScore) { topScore = score; best = action; }
            }

            best?.Execute(this, Board);
        }

        // --- Path following ---

        // Request a new A* path. Skips if a path is already in-flight, or if the
        // destination hasn't moved enough to need a new one (unless ClearPath was called first).
        public void RequestPath(Vector3 destination)
        {
            const float refreshSqr = 2f * 2f;
            if (_pendingPath) return;
            if (_waypoints.Count > 0
                && (destination - _requestedDestination).sqrMagnitude < refreshSqr) return;

            _requestedDestination = destination;
            _pendingPath          = true;
            _seeker.StartPath(transform.position, destination, OnPathComplete);
        }

        // Clear current waypoints so the next RequestPath always issues a fresh query.
        public void ClearPath()
        {
            _waypoints.Clear();
            _waypointIndex = 0;
        }

        private void OnPathComplete(Path p)
        {
            _pendingPath = false;
            if (p.error) return;
            _waypoints     = p.vectorPath;
            _waypointIndex = 0;
        }

        private void FollowPath()
        {
            if (_waypoints == null || _waypointIndex >= _waypoints.Count) return;

            Vector3 toTarget = _waypoints[_waypointIndex] - transform.position;

            if (toTarget.magnitude < waypointReachDistance)
            {
                _waypointIndex++;
                return;
            }

            // Project desired direction onto the surface plane, then blend in boid separation.
            Vector3 moveDir = Vector3.ProjectOnPlane(toTarget.normalized, SurfaceNormal);
            moveDir += ComputeSeparation() * separationWeight;
            moveDir  = Vector3.ProjectOnPlane(moveDir, SurfaceNormal).normalized;

            transform.position += moveDir * (moveSpeed * Time.deltaTime);

            if (moveDir.sqrMagnitude > 0.001f)
            {
                Quaternion look = Quaternion.LookRotation(moveDir, SurfaceNormal);
                transform.rotation = Quaternion.Slerp(transform.rotation, look, 10f * Time.deltaTime);
            }
        }

        private Vector3 ComputeSeparation()
        {
            int     count = Physics.OverlapSphereNonAlloc(transform.position, separationRadius, _sepBuffer, beetleLayer);
            Vector3 sep   = Vector3.zero;
            for (int i = 0; i < count; i++)
            {
                if (_sepBuffer[i].gameObject == gameObject) continue;
                sep += (transform.position - _sepBuffer[i].transform.position).normalized;
            }
            return Vector3.ProjectOnPlane(sep, SurfaceNormal);
        }

        // --- Stuck detection ---

        private void CheckStuck()
        {
            if (Time.time < _nextStuckCheck) return;
            _nextStuckCheck = Time.time + stuckCheckInterval;

            // If the beetle has barely moved while trying to follow a path, give up on the
            // current destination and let the AI choose a new one on the next tick.
            if (_waypoints.Count > 0
                && (transform.position - _lastPosition).magnitude < stuckThreshold)
                ClearPath();

            _lastPosition = transform.position;
        }
    }
}
