# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

Unity 6 (6000.4.6f1) game project exploring beetle/insect AI and movement mechanics. Uses Universal Render Pipeline (URP 17.4).

## Key Packages

- **CrashKonijn GOAP** (2.1.22) — external GOAP framework used for enemy AI
- **Utility AI System** (net.devoe.ai.uas) — utility AI framework used for prey AI
- **Unity Input System** (1.19.0) — new input system, not legacy
- **Unity Splines** — used by the gravity system
- **NavMesh** (com.unity.ai.navigation 2.0.12) — used by AI locomotion
- **Cinemachine** (3.1.4) — camera follow
- **Odin Inspector** (Sirenix) — custom editor attributes throughout scripts

## Architecture

### Movement Systems

Three independent 3D movement implementations coexist (different scenes use different ones):

- **PlayerMovement.cs** — `ArcCast`-based spherical movement; positions via raycast against ground
- **PlayerRotation.cs** — configures multi-directional raycasts to calculate average surface normal for orientation
- **CustomGravityMovement.cs** — uses `IGravityProvider` for pluggable gravity direction; paired with `GroundDetection`

`GroundDetection` (namespace `ECM.Components`) is the shared ground detection implementation: sphere/capsule casts with ledge detection, step validation, and slope limits.

`ArcCast` (Utils/) is a multi-ray casting utility used by PlayerMovement instead of a single raycast.

### Gravity System

`IGravityProvider` interface decouples gravity direction from movement. `SplineGravityProvider` pulls toward the nearest point on a Unity Spline, enabling curved-surface walking.

### AI Systems

Three distinct AI architectures are implemented to compare approaches:

**1. GOAP — Llama (enemy)**
Entry point: `LlamaBrain.cs`. Uses `CrashKonijn.Goap` agent. Goals (`WanderGoal`, `KillPlayerGoal`) and Actions (`WanderAction`, `MeleeAttackAction`) are separate classes. `PlayerSensor` fires events on enter/exit to trigger goal switching. `AttackConfig` is a ScriptableObject for tuning. `DependencyInjector` wires config into actions at runtime.

**2. Utility AI — Aphid (prey)**
Entry point: `AphidBrain.cs` (extends abstract `Brain`). Each frame scores all `AIAction` subclasses by multiplying their `Consideration` utilities, then executes the highest scorer. `AphidBlackboard` (implements `IBlackboard`) is a shared key-value store read by both `AphidSensor` and the brain. `Hunger.cs` increments over time, writes to the blackboard, and kills the agent at max hunger.

**3. Context Steering — flock/simple agents**
Entry point: `ContextAI.cs`. `Detector` subclasses populate `AIData` with targets and obstacles. `SteeringBehavior` subclasses write into 8-direction interest/danger float arrays. `ContextSolver` combines the arrays into a final direction. `SteeringController` applies the result as forces to a `Rigidbody2D`. Standard boid behaviors (separation, alignment, cohesion) plus seek and obstacle avoidance are implemented.

### Perception

`Sensor` (abstract, `Assets/Source/Scripts/AI/`) uses a `SphereCollider` trigger to track objects by tag and exposes `GetClosestTarget()`. `AphidSensor` extends this to compute threat level and direction. `FoodSource` implements `IPerceivable` and creates a `Percept` for AI consumption; it auto-refills and scales visually with remaining amount.

## Scenes

| Scene | Purpose |
|---|---|
| Movement.unity | Player movement prototyping |
| Shepherding.unity | Multi-agent coordination |
| Tree Test.unity | AI behavior testing |
| UtilityAITest.unity | Utility AI system testing |

## Working in This Project

Unity projects are edited primarily through the Unity Editor. Scripts live in `Assets/Source/Scripts/`. There is no build CLI — open the project in Unity 6 and use the Editor's Play mode to test.

When adding a new AI agent type, follow the existing pattern: create a `Brain` subclass, define `AIAction` and `Consideration` subclasses, implement a `Sensor` subclass, and wire a `Blackboard` between them.

When adding a new movement mode, implement `IGravityProvider` if custom gravity is needed and pair it with `GroundDetection` for ground state.
