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

        [Header("Debug")]
        [SerializeField] private bool  _showDebug   = false;
        [SerializeField] private float _debugHeight = 1.5f;  // world units above the beetle
        [SerializeField] private float _boxWidth    = 185f;

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

        // --- Debug overlay state ---

        private struct DebugEntry { public string Name; public float Score; public bool Valid; }
        private readonly List<DebugEntry> _debugEntries    = new();
        private string                    _currentActionName = string.Empty;

        private static GUIStyle _headerStyle;
        private static GUIStyle _activeStyle;
        private static GUIStyle _normalStyle;
        private static GUIStyle _invalidStyle;

        // ---

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

            if (_showDebug) _debugEntries.Clear();

            foreach (var action in _actions)
            {
                bool  valid = action.IsValid(Board);
                float score = valid ? action.Score(Board) : float.NegativeInfinity;

                if (_showDebug)
                    _debugEntries.Add(new DebugEntry { Name = action.Name, Score = score, Valid = valid });

                if (valid && score > topScore) { topScore = score; best = action; }
            }

            if (_showDebug) _currentActionName = best?.Name ?? "None";
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

        // --- Debug overlay ---

        private void OnGUI()
        {
            if (!_showDebug || Camera.main == null || _debugEntries.Count == 0) return;

            InitStyles();

            // Project the label anchor to screen space.
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + transform.up * _debugHeight);
            if (screenPos.z < 0f) return; // behind camera

            const float lineH   = 16f;
            const float padding = 4f;
            float       boxH    = (_debugEntries.Count + 2) * lineH + padding * 2f;

            // GUI origin is top-left; screen origin is bottom-left.
            float x = screenPos.x - _boxWidth * 0.5f;
            float y = Screen.height - screenPos.y;

            GUI.Box(new Rect(x, y, _boxWidth, boxH), GUIContent.none);

            float cx = x + padding;
            float cy = y + padding;
            float cw = _boxWidth - padding * 2f;

            GUI.Label(new Rect(cx, cy, cw, lineH),
                      $"{GetType().Name}  |  {Hunger.Value:P0} hungry", _headerStyle);
            cy += lineH;

            GUI.Label(new Rect(cx, cy, cw, lineH), new string('─', 24), _normalStyle);
            cy += lineH;

            foreach (var entry in _debugEntries)
            {
                bool   isCurrent = entry.Name == _currentActionName;
                string prefix    = isCurrent ? "► " : "  ";
                string scoreStr  = entry.Valid ? entry.Score.ToString("F2") : "—";
                string line      = $"{prefix}{entry.Name,-16}{scoreStr,6}";

                GUI.Label(new Rect(cx, cy, cw, lineH), line,
                          isCurrent ? _activeStyle : (entry.Valid ? _normalStyle : _invalidStyle));
                cy += lineH;
            }
        }

        // Lazily initialise styles inside OnGUI where GUI.skin is available.
        private static void InitStyles()
        {
            if (_headerStyle != null) return;

            _headerStyle              = new GUIStyle(GUI.skin.label);
            _headerStyle.fontStyle    = FontStyle.Bold;
            _headerStyle.fontSize     = 11;

            _activeStyle              = new GUIStyle(GUI.skin.label);
            _activeStyle.fontStyle    = FontStyle.Bold;
            _activeStyle.fontSize     = 10;
            var activeState           = _activeStyle.normal;
            activeState.textColor     = Color.yellow;
            _activeStyle.normal       = activeState;

            _normalStyle              = new GUIStyle(GUI.skin.label);
            _normalStyle.fontSize     = 10;

            _invalidStyle             = new GUIStyle(GUI.skin.label);
            _invalidStyle.fontSize    = 10;
            var invalidState          = _invalidStyle.normal;
            invalidState.textColor    = new Color(0.5f, 0.5f, 0.5f);
            _invalidStyle.normal      = invalidState;
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
