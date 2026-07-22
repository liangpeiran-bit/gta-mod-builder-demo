# GTA5 SHVDN v3 Reference for Dify

This is the single-file import bundle generated from the repository SHVDN snapshot.


---

# Dify SHVDN Knowledge Pack

This folder is generated from the repository source snapshot in `templates/live-studio/references/shvdn/`.

Import these Markdown files into a Dify Knowledge Base such as `GTA5 SHVDN v3 Reference`.

Recommended imports:

- `shvdn-core-rules.md`
- `shvdn-domain-index.md`
- `shvdn-world-ui.md`
- `shvdn-vehicles.md`
- `shvdn-peds.md`

Do not assume Dify can read repository files directly. Dify LLM nodes should use the Knowledge Base retrieval output as their SHVDN context.

Snapshot: 2026-05-19
SHVDN version: 3.6.0.0


---

# SHVDN Core Rules for Dify

Use these rules in `Mod.cs Generator` and `QA Fixer`.

## Runtime

- Target ScriptHookVDotNet v3.
- Target C# 7.3 and .NET Framework 4.8.
- Generate only `ModProject/GeneratedGameplay.cs`; do not regenerate the fixed LIVE Studio template.
- The fixed template v3 dispatches all generated hooks on the GTA main thread. Call SHVDN APIs directly inside hooks; do not create dispatchers, tasks, threads, or timers.

## Known Compile Pitfalls

- Do not use `GTA.KeyEventArgs`; use `System.Windows.Forms.KeyEventArgs` / `KeyEventArgs`.
- Do not use unqualified `Screen.ShowSubtitle`; use `GTA.UI.Screen.ShowSubtitle` or `GTA.UI.Notification.Show`.
- Do not read set-only vehicle multiplier properties. `Vehicle.EnginePowerMultiplier` and `Vehicle.EngineTorqueMultiplier` may be set; reset them to `1.0f` instead of storing original values.
- Do not use `CreatedAt` properties; track expiry with `DateTime` dictionaries or small tracked object classes.
- Do not use C# 8+ syntax: records, file-scoped namespaces, nullable reference types, target-typed `new`, top-level statements.

## Unsupported Scope

- GTA Online, online economy, anti-cheat bypass, money/rank/XP manipulation.
- OpenIV, RPF, DLC packs, custom models, custom textures, OBS overlays.
- Third-party precompiled binaries or arbitrary external assets.

## Generation Rule

If a requested SHVDN type/member/enum is not present in retrieved Knowledge Base context and not already used in the fixed template, do not invent it. Prefer a simpler supported effect.

---

# SHVDN Domain Index

Snapshot: 2026-05-19
SHVDN version: 3.6.0.0

Use this file to orient retrieval. For detailed members, retrieve the domain-specific files.

## UI And HUD
- GTA.Blip
- GTA.BlipCategoryType
- GTA.BlipColor
- GTA.BlipDisplayType
- GTA.BlipSprite
- GTA.Scaleform
- GTA.ScaleformArgumentTXD
- GTA.UI.Alignment
- GTA.UI.ContainerElement
- GTA.UI.CursorSprite
- GTA.UI.CustomSprite
- GTA.UI.Font
- GTA.UI.Hud
- GTA.UI.HudComponent
- GTA.UI.IElement
- GTA.UI.ISpriteElement
- GTA.UI.LoadingPrompt
- GTA.UI.LoadingSpinnerType
- GTA.UI.Notification
- GTA.UI.NotificationIcon
- GTA.UI.Screen
- GTA.UI.ScreenEffect
- GTA.UI.Sprite
- GTA.UI.TextElement

## Math And Geometry
- GTA.Math.Matrix
- GTA.Math.Quaternion
- GTA.Math.Vector2
- GTA.Math.Vector3

## Native Interop
- GTA.MaterialHash
- GTA.Native.Function
- GTA.Native.GlobalVariable
- GTA.Native.Hash
- GTA.Native.INativeValue
- GTA.Native.InputArgument
- GTA.Native.OutputArgument
- GTA.PedHash
- GTA.VehicleHash
- GTA.VehicleWeaponHash
- GTA.WeaponComponentHash
- GTA.WeaponHash

## Vehicles
- GTA.DrivingStyle
- GTA.EnterVehicleFlags
- GTA.HandlingData
- GTA.LeaveVehicleFlags
- GTA.LicensePlateStyle
- GTA.LicensePlateType
- GTA.Model
- GTA.NaturalMotion.AdaptiveMode
- GTA.NaturalMotion.MirrorMode
- GTA.NaturalMotion.SetWeaponModeHelper
- GTA.NaturalMotion.TorqueFilterMode
- GTA.NaturalMotion.TorqueMode
- GTA.NaturalMotion.TorqueSpinMode
- GTA.NaturalMotion.WeaponMode
- GTA.PlayerTargetingMode
- GTA.RadioStation
- GTA.SpeechModifier
- GTA.Vehicle
- GTA.VehicleClass
- GTA.VehicleColor
- GTA.VehicleDoor
- GTA.VehicleDoorCollection
- GTA.VehicleDoorIndex
- GTA.VehicleDrivingFlags
- GTA.VehicleLandingGearState
- GTA.VehicleLockStatus
- GTA.VehicleMissionType
- GTA.VehicleMod
- GTA.VehicleModCollection
- GTA.VehicleModType
- GTA.VehicleNeonLight
- GTA.VehicleRoofState
- GTA.VehicleSeat
- GTA.VehicleToggleMod
- GTA.VehicleToggleModType
- GTA.VehicleType
- GTA.VehicleWheel
- GTA.VehicleWheelBoneId
- GTA.VehicleWheelCollection
- GTA.VehicleWheelType
- GTA.VehicleWindow
- GTA.VehicleWindowCollection
- GTA.VehicleWindowIndex
- GTA.VehicleWindowTint

## Weapons
- GTA.NaturalMotion.FireWeaponHelper
- GTA.NaturalMotion.RegisterWeaponHelper
- GTA.Projectile
- GTA.Weapon
- GTA.WeaponAsset
- GTA.WeaponAttachmentPoint
- GTA.WeaponCollection
- GTA.WeaponComponent
- GTA.WeaponComponentCollection
- GTA.WeaponGroup
- GTA.WeaponTint

## Player And Ped
- GTA.Gender
- GTA.IPedVariation
- GTA.NaturalMotion.PedalLegsHelper
- GTA.Ped
- GTA.PedBone
- GTA.PedBoneCollection
- GTA.PedComponent
- GTA.PedComponentType
- GTA.PedGroup
- GTA.PedProp
- GTA.PedPropType
- GTA.Player
- GTA.Relationship
- GTA.RelationshipGroup
- GTA.TaskInvoker
- GTA.TaskSequence

## World And Environment
- GTA.Checkpoint
- GTA.CheckpointCustomIcon
- GTA.CheckpointCustomIconStyle
- GTA.CheckpointIcon
- GTA.ExplosionType
- GTA.MarkerType
- GTA.RaycastResult
- GTA.ShapeTest
- GTA.ShapeTestHandle
- GTA.ShapeTestOptions
- GTA.ShapeTestResult
- GTA.ShapeTestStatus
- GTA.Weather
- GTA.World

## Camera And Visual Effects
- GTA.Camera
- GTA.CameraShake
- GTA.GameplayCamera
- GTA.ParticleEffect
- GTA.ParticleEffectAsset

## Audio
- GTA.Audio
- GTA.AudioFlags

## Entities And Objects
- GTA.Bone
- GTA.Entity
- GTA.EntityBone
- GTA.EntityBoneCollection
- GTA.EntityDamageRecord
- GTA.EntityDamageRecordCollection
- GTA.EntityPopulationType
- GTA.EntityType
- GTA.NaturalMotion.DefineAttachedObjectHelper
- GTA.NaturalMotion.ForceLeanTowardsObjectHelper
- GTA.NaturalMotion.HipsLeanTowardsObjectHelper
- GTA.NaturalMotion.LeanTowardsObjectHelper
- GTA.Pickup
- GTA.PickupType
- GTA.PoolObject
- GTA.Prop

## Scripting And Input
- GTA.Button
- GTA.Control
- GTA.Game
- GTA.GameVersion
- GTA.GameVersionNotSupportedException
- GTA.InputMethod
- GTA.RequireScript
- GTA.Script
- GTA.ScriptAttributes
- GTA.ScriptSettings

## Other
- GTA.AnimatedBuilding
- GTA.AnimationFlags
- GTA.BoatMissionFlags
- GTA.Building
- GTA.CargobobHook
- GTA.DrawBoxFlags
- GTA.EntityBoneCollection.Enumerator
- GTA.EulerRotationOrder
- GTA.FiringPattern
- GTA.ForceType
- GTA.Formation
- GTA.HeliMissionFlags
- GTA.Helmet
- GTA.IDeletable
- GTA.IExistable
- GTA.ISpatial
- GTA.InteriorInstance
- GTA.InteriorProxy
- GTA.IntersectFlags
- GTA.InvertAxisFlags
- GTA.Language
- GTA.MeasurementSystem
- GTA.NaturalMotion.ActivePoseHelper
- GTA.NaturalMotion.AnimPoseHelper
- GTA.NaturalMotion.AnimSource
- GTA.NaturalMotion.ApplyBulletImpulseHelper
- GTA.NaturalMotion.ApplyImpulseHelper
- GTA.NaturalMotion.ArmDirection
- GTA.NaturalMotion.ArmsWindmillAdaptiveHelper
- GTA.NaturalMotion.ArmsWindmillHelper
- GTA.NaturalMotion.BalancerCollisionsReactionHelper
- GTA.NaturalMotion.BodyBalanceHelper
- GTA.NaturalMotion.BodyFoetalHelper
- GTA.NaturalMotion.BodyRelaxHelper
- GTA.NaturalMotion.BodyRollUpHelper
- GTA.NaturalMotion.BodyWritheHelper
- GTA.NaturalMotion.BraceForImpactHelper
- GTA.NaturalMotion.BuoyancyHelper
- GTA.NaturalMotion.CarriedHelper
- GTA.NaturalMotion.CatchFallHelper
- GTA.NaturalMotion.ConfigureBalanceHelper
- GTA.NaturalMotion.ConfigureBalanceResetHelper
- GTA.NaturalMotion.ConfigureBulletsExtraHelper
- GTA.NaturalMotion.ConfigureBulletsHelper
- GTA.NaturalMotion.ConfigureConstraintsHelper
- GTA.NaturalMotion.ConfigureLimitsHelper
- GTA.NaturalMotion.ConfigureSelfAvoidanceHelper
- GTA.NaturalMotion.ConfigureShotInjuredArmHelper
- GTA.NaturalMotion.ConfigureShotInjuredLegHelper
- GTA.NaturalMotion.ConfigureSoftLimitHelper
- GTA.NaturalMotion.CustomHelper
- GTA.NaturalMotion.DangleHelper
- GTA.NaturalMotion.ElectrocuteHelper
- GTA.NaturalMotion.Euphoria
- GTA.NaturalMotion.FallOverWallHelper
- GTA.NaturalMotion.FallType
- GTA.NaturalMotion.ForceLeanInDirectionHelper
- GTA.NaturalMotion.ForceLeanRandomHelper
- GTA.NaturalMotion.ForceLeanToPositionHelper
- GTA.NaturalMotion.ForceToBodyPartHelper
- GTA.NaturalMotion.GrabHelper
- GTA.NaturalMotion.Hand
- GTA.NaturalMotion.HeadLookHelper
- GTA.NaturalMotion.HighFallHelper
- GTA.NaturalMotion.HipsLeanInDirectionHelper
- GTA.NaturalMotion.HipsLeanRandomHelper
- GTA.NaturalMotion.HipsLeanToPositionHelper
- GTA.NaturalMotion.IncomingTransformsHelper
- GTA.NaturalMotion.InjuredOnGroundHelper
- GTA.NaturalMotion.LeanInDirectionHelper
- GTA.NaturalMotion.LeanRandomHelper
- GTA.NaturalMotion.LeanToPositionHelper
- GTA.NaturalMotion.Message
- GTA.NaturalMotion.OnFireHelper
- GTA.NaturalMotion.PointArmHelper
- GTA.NaturalMotion.PointGunExtraHelper
- GTA.NaturalMotion.PointGunHelper
- GTA.NaturalMotion.RbTwistAxis
- GTA.NaturalMotion.RollDownStairsHelper
- GTA.NaturalMotion.SetCharacterCollisionsHelper
- GTA.NaturalMotion.SetCharacterDampingHelper
- GTA.NaturalMotion.SetCharacterHealthHelper
- GTA.NaturalMotion.SetCharacterStrengthHelper
- GTA.NaturalMotion.SetCharacterUnderwaterHelper
- GTA.NaturalMotion.SetFallingReactionHelper
- GTA.NaturalMotion.SetFrictionScaleHelper
- GTA.NaturalMotion.SetMuscleStiffnessHelper
- GTA.NaturalMotion.SetStiffnessHelper
- GTA.NaturalMotion.ShotConfigureArmsHelper
- GTA.NaturalMotion.ShotFallToKneesHelper
- GTA.NaturalMotion.ShotFromBehindHelper
- GTA.NaturalMotion.ShotHeadLookHelper
- GTA.NaturalMotion.ShotHelper
- GTA.NaturalMotion.ShotInGutsHelper
- GTA.NaturalMotion.ShotNewBulletHelper
- GTA.NaturalMotion.ShotRelaxHelper
- GTA.NaturalMotion.ShotShockSpinHelper
- GTA.NaturalMotion.ShotSnapHelper
- GTA.NaturalMotion.SmartFallHelper
- GTA.NaturalMotion.StaggerFallHelper
- GTA.NaturalMotion.StayUprightHelper
- GTA.NaturalMotion.StopAllBehaviorsHelper
- GTA.NaturalMotion.Synchroisation
- GTA.NaturalMotion.TeeterHelper
- GTA.NaturalMotion.TurnType
- GTA.NaturalMotion.UpperBodyFlinchHelper
- GTA.NaturalMotion.YankedHelper
- GTA.ParachuteLandingType
- GTA.ParachuteState
- GTA.ParachuteTint
- GTA.PedBoneCollection.Enumerator
- GTA.PedGroup.Enumerator
- GTA.RagdollType
- GTA.Rope
- GTA.RopeType
- GTA.Style
- GTA.WindowTitle

---

# SHVDN World, UI, Native, and Model APIs

High-frequency APIs for notifications, subtitles, world state, spawning, native calls, and model loading.

Snapshot: 2026-05-19
SHVDN version: 3.6.0.0

Use this as Dify Knowledge Base context. Do not paste the full source JSON into prompts.

## GTA.UI.Notification

- Kind: static class
- Namespace: GTA.UI
- Domain: UI And HUD
- Summary: Methods to manage the display of notifications above the minimap.

### Constructors

- None

### Properties

- None

### Methods
- static System.Void Hide(System.Int32 handle) - Hides a GTA.UI.Notification instantly.
- static System.Int32 Show(GTA.UI.NotificationIcon icon, System.String sender, System.String subject, System.String message, System.Boolean fadeIn = , System.Boolean blinking = ) - Creates a more advanced (SMS-alike) GTA.UI.Notification above the minimap showing a sender icon, subject and the message.
- static System.Int32 Show(System.String message, System.Boolean blinking = ) - Creates a GTA.UI.Notification above the minimap with the given message.

### Enum Values

## GTA.UI.Screen

- Kind: static class
- Namespace: GTA.UI
- Domain: UI And HUD
- Summary: Methods to handle UI actions that affect the whole screen.

### Constructors

- None

### Properties
- static System.Boolean AreScreenKillEffectsEnabled { get; } - Gets a value indicating whether screen kill effects are enabled.
- static System.Single AspectRatio { get; } - Gets the current screen aspect ratio
- static System.Boolean IsFadedIn { get; } - Gets a value indicating whether the screen is faded in.
- static System.Boolean IsFadedOut { get; } - Gets a value indicating whether the screen is faded out.
- static System.Boolean IsFadingIn { get; } - Gets a value indicating whether the screen is fading in.
- static System.Boolean IsFadingOut { get; } - Gets a value indicating whether the screen is fading out.
- static System.Boolean IsHelpTextDisplayed { get; } - Gets a value indicating whether a help message is currently displayed.
- static System.Drawing.Size Resolution { get; } - Gets the actual screen resolution the game is being rendered at
- static System.Single ScaledWidth { get; } - Gets the screen width scaled against a 720pixel height base.

### Methods
- static System.Void FadeIn(System.Int32 time) - Fades the screen in over a specific time, useful for transitioning
- static System.Void FadeOut(System.Int32 time) - Fades the screen out over a specific time, useful for transitioning
- static System.Boolean IsEffectActive(GTA.UI.ScreenEffect effectName) - Gets a value indicating whether the specific screen effect is running.
- static System.Void ShowHelpText(System.String helpText, System.Int32 duration = , System.Boolean beep = , System.Boolean looped = ) - Displays a help message in the top corner of the screen infinitely.
- static System.Void ShowHelpTextThisFrame(System.String helpText) - Displays a help message in the top corner of the screen this frame. Beeping sound will be played.
- static System.Void ShowHelpTextThisFrame(System.String helpText, System.Boolean beep) - Displays a help message in the top corner of the screen this frame. Specify whether beeping sound plays.
- static System.Void ShowSubtitle(System.String message, System.Int32 duration = ) - Shows a subtitle at the bottom of the screen for a given time
- static System.Void ShowSubtitle(System.String message, System.Int32 duration, System.Boolean drawImmediately = ) - Shows a subtitle at the bottom of the screen for a given time
- static System.Void StartEffect(GTA.UI.ScreenEffect effectName, System.Int32 duration = , System.Boolean looped = ) - Starts applying the specified effect to the screen.
- static System.Void StopEffect(GTA.UI.ScreenEffect effectName) - Stops applying the specified effect to the screen.
- static System.Void StopEffects() - Stops all currently running effects.
- static System.Drawing.PointF WorldToScreen(GTA.Math.Vector3 position, System.Boolean scaleWidth = ) - Translates a point in WorldSpace to its given Coordinates on the GTA.UI.Screen

### Enum Values

## GTA.World

- Kind: static class
- Namespace: GTA
- Domain: World And Environment

### Constructors

- None

### Properties
- static System.Int32 AnimatedBuildingCapacity { get; } - The total number of GTA.AnimatedBuilding s that can exist in the world.
- static System.Int32 AnimatedBuildingCount { get; } - A fast way to get the total number of GTA.AnimatedBuilding s spawned in the world.
- static System.Boolean Blackout { set; } - Sets a value indicating whether lights in the GTA.World should be rendered.
- static System.Int32 BuildingCapacity { get; } - The total number of GTA.Building s that can exist in the world.
- static System.Int32 BuildingCount { get; } - A fast way to get the total number of GTA.Building s spawned in the world.
- static System.DateTime CurrentDate { get; set; } - Gets or sets the current date and time in the GTA World.
- static System.TimeSpan CurrentTimeOfDay { get; set; } - Gets or sets the current time of day in the GTA World.
- static System.Int32 EntityColliderCapacity { get; } - The total number of GTA.Entity colliders can be used. The return value can be different in different versions. When GTA.World.EntityColliderCount reaches this value, no more GTA.Entity will not be able to be physically moved and GTA.Vehicle s and GTA.Prop s will not be able to detach fragment parts properly.
- static System.Int32 EntityColliderCount { get; } - Returns the total number of GTA.Entity colliders used.
- static System.Single GravityLevel { get; set; } - Sets the gravity level for all GTA.World objects.
- static System.Int32 InteriorInstanceCapacity { get; } - The total number of GTA.InteriorInstance s that can exist in the world.
- static System.Int32 InteriorInstanceCount { get; } - A fast way to get the total number of GTA.InteriorInstance s spawned in the world.
- static System.Int32 InteriorProxyCapacity { get; } - The total number of GTA.InteriorProxy s the game can manage at the same time in the GTA.InteriorProxy pool.
- static System.Int32 InteriorProxyCount { get; } - A fast way to get the total number of GTA.InteriorProxy s managed in the GTA.InteriorProxy pool.
- static System.Boolean IsClockPaused { get; set; } - Gets or sets a value indicating whether the in-game clock is paused.
- static System.Int32 MillisecondsPerGameMinute { get; set; } - Gets or sets how many milliseconds in the real world one game minute takes.
- static GTA.Weather NextWeather { get; set; } - Gets or sets the next weather.
- static System.Int32 PedCapacity { get; } - The total number of GTA.Ped s that can exist in the world.
- static System.Int32 PedCount { get; } - A fast way to get the total number of GTA.Ped s spawned in the world.
- static System.Int32 PickupObjectCapacity { get; } - The total number of GTA.Prop s in the world associated with a GTA.Pickup that can exist in the world.
- static System.Int32 PickupObjectCount { get; } - A fast way to get the total number of GTA.Prop s in the world associated with a GTA.Pickup .
- static System.Int32 ProjectileCapacity { get; } - The total number of GTA.Projectile s that can exist in the world. Always returns 50 currently since the limit is hard-coded in the exe.
- static System.Int32 ProjectileCount { get; } - A fast way to get the total number of GTA.Projectile s spawned in the world.
- static System.Int32 PropCapacity { get; } - The total number of GTA.Prop s that can exist in the world.
- static System.Int32 PropCount { get; } - A fast way to get the total number of GTA.Prop s spawned in the world.
- static GTA.Camera RenderingCamera { get; set; } - Gets or sets the rendering camera.
- static System.Int32 VehicleCapacity { get; } - The total number of GTA.Vehicle s that can exist in the world.
- static System.Int32 VehicleCount { get; } - A fast way to get the total number of GTA.Vehicle s spawned in the world.
- static GTA.Blip WaypointBlip { get; } - Gets the waypoint blip.
- static GTA.Math.Vector3 WaypointPosition { get; set; } - Gets or sets the waypoint position.
- static GTA.Weather Weather { get; set; } - Gets or sets the weather.

### Methods
- static System.Void AddExplosion(GTA.Math.Vector3 position, GTA.ExplosionType type, System.Single radius, System.Single cameraShake, GTA.Ped owner = , System.Boolean aubidble = , System.Boolean invisible = ) - Creates an explosion in the world
- static GTA.RelationshipGroup AddRelationshipGroup(System.String name) - Creates a GTA.RelationshipGroup with the given name.
- static GTA.Rope AddRope(GTA.RopeType type, GTA.Math.Vector3 position, GTA.Math.Vector3 rotation, System.Single length, System.Single minLength, System.Boolean breakable) - Spawns a GTA.Rope .
- static System.Single CalculateTravelDistance(GTA.Math.Vector3 origin, GTA.Math.Vector3 destination) - Calculates the travel distance using roads and paths between 2 positions.
- static GTA.Prop CreateAmbientPickup(GTA.PickupType type, GTA.Math.Vector3 position, GTA.Model model, System.Int32 value) - Spawns a pickup GTA.Prop at the specified position.
- static GTA.Blip CreateBlip(GTA.Math.Vector3 position) - Creates a GTA.Blip at the given position on the map.
- static GTA.Blip CreateBlip(GTA.Math.Vector3 position, System.Single radius) - Creates a GTA.Blip for a circular area at the given position on the map.
- static GTA.Camera CreateCamera(GTA.Math.Vector3 position, GTA.Math.Vector3 rotation, System.Single fov) - Creates a GTA.Camera , use GTA.World.RenderingCamera to switch to this camera
- static GTA.Checkpoint CreateCheckpoint(GTA.CheckpointCustomIcon icon, GTA.Math.Vector3 position, GTA.Math.Vector3 pointTo, System.Single radius, System.Drawing.Color color) - Creates a GTA.Checkpoint in the world.
- static GTA.Checkpoint CreateCheckpoint(GTA.CheckpointIcon icon, GTA.Math.Vector3 position, GTA.Math.Vector3 pointTo, System.Single radius, System.Drawing.Color color) - Creates a GTA.Checkpoint in the world.
- static GTA.ParticleEffect CreateParticleEffect(GTA.ParticleEffectAsset asset, System.String effectName, GTA.Entity entity, GTA.Math.Vector3 offset = , GTA.Math.Vector3 rotation = , System.Single scale = , GTA.InvertAxisFlags invertAxis = ) - Creates a GTA.ParticleEffect on an GTA.Entity that runs looped.
- static GTA.ParticleEffect CreateParticleEffect(GTA.ParticleEffectAsset asset, System.String effectName, GTA.EntityBone entityBone, GTA.Math.Vector3 offset = , GTA.Math.Vector3 rotation = , System.Single scale = , GTA.InvertAxisFlags invertAxis = ) - Creates a GTA.ParticleEffect on an GTA.EntityBone that runs looped.
- static GTA.ParticleEffect CreateParticleEffect(GTA.ParticleEffectAsset asset, System.String effectName, GTA.Math.Vector3 position, GTA.Math.Vector3 rotation = , System.Single scale = , GTA.InvertAxisFlags invertAxis = ) - Creates a GTA.ParticleEffect at a position that runs looped.
- static System.Boolean CreateParticleEffectNonLooped(GTA.ParticleEffectAsset asset, System.String effectName, GTA.Entity entity, GTA.Math.Vector3 off = , GTA.Math.Vector3 rot = , System.Single scale = , GTA.InvertAxisFlags invertAxis = ) - Starts a Particle Effect on an GTA.Entity that runs once then is destroyed.
- static System.Boolean CreateParticleEffectNonLooped(GTA.ParticleEffectAsset asset, System.String effectName, GTA.EntityBone entityBone, GTA.Math.Vector3 off = , GTA.Math.Vector3 rot = , System.Single scale = , GTA.InvertAxisFlags invertAxis = ) - Starts a Particle Effect on an GTA.EntityBone that runs once then is destroyed.
- static System.Boolean CreateParticleEffectNonLooped(GTA.ParticleEffectAsset asset, System.String effectName, GTA.Math.Vector3 pos, GTA.Math.Vector3 rot = , System.Single scale = , GTA.InvertAxisFlags invertAxis = ) - Starts a Particle Effect that runs once at a given position then is destroyed.
- static GTA.Ped CreatePed(GTA.Model model, GTA.Math.Vector3 position, System.Single heading = ) - Spawns a GTA.Ped of the given GTA.Model at the position and heading specified.
- static GTA.Pickup CreatePickup(GTA.PickupType type, GTA.Math.Vector3 position, GTA.Math.Vector3 rotation, GTA.Model model, System.Int32 value) - Spawns a GTA.Pickup at the specified position.
- static GTA.Pickup CreatePickup(GTA.PickupType type, GTA.Math.Vector3 position, GTA.Model model, System.Int32 value) - Spawns a GTA.Pickup at the specified position.
- static GTA.Prop CreateProp(GTA.Model model, GTA.Math.Vector3 position, GTA.Math.Vector3 rotation, System.Boolean dynamic, System.Boolean placeOnGround) - Spawns a GTA.Prop of the given GTA.Model at the specified position.
- static GTA.Prop CreateProp(GTA.Model model, GTA.Math.Vector3 position, System.Boolean dynamic, System.Boolean placeOnGround) - Spawns a GTA.Prop of the given GTA.Model at the specified position.
- static GTA.Prop CreatePropNoOffset(GTA.Model model, GTA.Math.Vector3 position, GTA.Math.Vector3 rotation, System.Boolean dynamic) - Spawns a GTA.Prop of the given GTA.Model at the specified position without any offset.
- static GTA.Prop CreatePropNoOffset(GTA.Model model, GTA.Math.Vector3 position, System.Boolean dynamic) - Spawns a GTA.Prop of the given GTA.Model at the specified position without any offset.
- static GTA.Ped CreateRandomPed(GTA.Math.Vector3 position) - Spawns a GTA.Ped of a random GTA.Model at the position specified.
- static GTA.Ped CreateRandomPed(GTA.Math.Vector3 position, System.Single heading, System.Func<GTA.Model, System.Boolean> predicate = ) - Spawns a GTA.Ped of a random GTA.Model at the position specified.
- static GTA.Vehicle CreateRandomVehicle(GTA.Math.Vector3 position, System.Single heading = , System.Func<GTA.Model, System.Boolean> predicate = ) - Spawns a GTA.Vehicle of a random GTA.Model at the position specified.
- static GTA.Vehicle CreateVehicle(GTA.Model model, GTA.Math.Vector3 position, System.Single heading = ) - Spawns a GTA.Vehicle of the given GTA.Model at the position and heading specified.
- static System.Void DestroyAllCameras() - Destroys all user created GTA.Camera s.
- static System.Void DrawBoxForAngledArea(GTA.Math.Vector3 originEdge, GTA.Math.Vector3 extentEdge, System.Single width, System.Drawing.Color color, GTA.DrawBoxFlags drawFlags = ) - Draws a box that occupies the angled area. An angled area is an X-Z oriented rectangle with three parameters: origin, extent, and width.
- static System.Void DrawLightWithRange(GTA.Math.Vector3 position, System.Drawing.Color color, System.Single range, System.Single intensity) - Draws light around a region.
- static System.Void DrawLine(GTA.Math.Vector3 start, GTA.Math.Vector3 end, System.Drawing.Color color)
- static System.Void DrawMarker(GTA.MarkerType type, GTA.Math.Vector3 pos, GTA.Math.Vector3 dir, GTA.Math.Vector3 rot, GTA.Math.Vector3 scale, System.Drawing.Color color, System.Boolean bobUpAndDown = , System.Boolean faceCamera = , System.Boolean rotateY = , System.String textueDict = , System.String textureName = , System.Boolean drawOnEntity = ) - Draws a marker in the world, this needs to be done on a per frame basis
- static System.Void DrawPolygon(GTA.Math.Vector3 vertexA, GTA.Math.Vector3 vertexB, GTA.Math.Vector3 vertexC, System.Drawing.Color color)
- static System.Void DrawSpotLight(GTA.Math.Vector3 pos, GTA.Math.Vector3 dir, System.Drawing.Color color, System.Single distance, System.Single brightness, System.Single roundness, System.Single radius, System.Single fadeout)
- static System.Void DrawSpotLightWithShadow(GTA.Math.Vector3 pos, GTA.Math.Vector3 dir, System.Drawing.Color color, System.Single distance, System.Single brightness, System.Single roundness, System.Single radius, System.Single fadeout)
- static GTA.AnimatedBuilding[] GetAllAnimatedBuildings()
- static GTA.Blip[] GetAllBlips(GTA.BlipSprite[] blipTypes) - Gets an array of all the GTA.Blip s on the map with a given GTA.BlipSprite .
- static GTA.Building[] GetAllBuildings()
- static GTA.Checkpoint[] GetAllCheckpoints() - Gets an array of all the GTA.Checkpoint s.
- static GTA.Entity[] GetAllEntities() - Gets an array of all GTA.Entity s in the World.
- static GTA.InteriorInstance[] GetAllInteriorInstances()
- static GTA.InteriorProxy[] GetAllInteriorProxies()
- static GTA.Ped[] GetAllPeds(GTA.Model[] models) - Gets an array of all GTA.Ped s in the World.
- static GTA.Prop[] GetAllPickupObjects() - Gets an array of all GTA.Prop s in the World associated with a GTA.Pickup .
- static GTA.Projectile[] GetAllProjectiles() - Gets an array of all GTA.Projectile s in the World.
- static GTA.Prop[] GetAllProps(GTA.Model[] models) - Gets an array of all GTA.Prop s in the World.
- static GTA.Vehicle[] GetAllVehicles(GTA.Model[] models) - Gets an array of all GTA.Vehicle s in the World.
- static GTA.AnimatedBuilding GetClosest(GTA.Math.Vector2 position, GTA.AnimatedBuilding[] animatedBuildings) - Gets the closest GTA.AnimatedBuilding to a given position in the World ignoring height.
- static GTA.AnimatedBuilding GetClosest(GTA.Math.Vector3 position, GTA.AnimatedBuilding[] animatedBuildings) - Gets the closest GTA.AnimatedBuilding to a given position in the World.
- static GTA.Building GetClosest(GTA.Math.Vector2 position, GTA.Building[] buildings) - Gets the closest GTA.Building to a given position in the World ignoring height.
- static GTA.Building GetClosest(GTA.Math.Vector3 position, GTA.Building[] buildings) - Gets the closest GTA.Building to a given position in the World.
- static GTA.InteriorInstance GetClosest(GTA.Math.Vector2 position, GTA.InteriorInstance[] interiorInstances) - Gets the closest GTA.InteriorInstance to a given position in the World ignoring height.
- static GTA.InteriorInstance GetClosest(GTA.Math.Vector3 position, GTA.InteriorInstance[] interiorInstances) - Gets the closest GTA.InteriorInstance to a given position in the World.
- static GTA.InteriorProxy GetClosest(GTA.Math.Vector2 position, GTA.InteriorProxy[] interiorProxies) - Gets the closest GTA.InteriorProxy to a given position in the World ignoring height.
- static GTA.InteriorProxy GetClosest(GTA.Math.Vector3 position, GTA.InteriorProxy[] interiorProxies) - Gets the closest GTA.InteriorProxy to a given position in the World.
- static T GetClosest<T>(GTA.Math.Vector2 position, T[] spatials) - Gets the closest GTA.ISpatial to a given position in the World ignoring height.
- static T GetClosest<T>(GTA.Math.Vector3 position, T[] spatials) - Gets the closest GTA.ISpatial to a given position in the World.
- static GTA.AnimatedBuilding GetClosestAnimatedBuilding(GTA.Math.Vector3 position, System.Single radius)
- static GTA.Building GetClosestBuilding(GTA.Math.Vector3 position, System.Single radius)
- static GTA.InteriorInstance GetClosestInteriorInstance(GTA.Math.Vector3 position, System.Single radius)
- static GTA.InteriorProxy GetClosestInteriorProxy(GTA.Math.Vector3 position, System.Single radius)
- static GTA.Ped GetClosestPed(GTA.Math.Vector3 position, System.Single radius, GTA.Model[] models) - Gets the closest GTA.Ped to a given position in the World.
- static GTA.Prop GetClosestPickupObject(GTA.Math.Vector3 position, System.Single radius) - Gets the closest GTA.Prop to a given position in the World associated with a GTA.Pickup .
- static GTA.Projectile GetClosestProjectile(GTA.Math.Vector3 position, System.Single radius) - Gets the closest GTA.Projectile to a given position in the World.
- static GTA.Prop GetClosestProp(GTA.Math.Vector3 position, System.Single radius, GTA.Model[] models) - Gets the closest GTA.Prop to a given position in the World.
- static GTA.Vehicle GetClosestVehicle(GTA.Math.Vector3 position, System.Single radius, GTA.Model[] models) - Gets the closest GTA.Vehicle to a given position in the World.
- static GTA.RaycastResult GetCrosshairCoordinates() - Determines where the crosshair intersects with the world.
- static GTA.RaycastResult GetCrosshairCoordinates(GTA.IntersectFlags intersectOptions = , GTA.Entity ignoreEntity = ) - Determines where the crosshair intersects with the world.
- static System.Single GetDistance(GTA.Math.Vector3 origin, GTA.Math.Vector3 destination) - Gets the straight line distance between 2 positions.
- static System.Single GetGroundHeight(GTA.Math.Vector2 position) - Gets the height of the ground at a given position.
- static System.Single GetGroundHeight(GTA.Math.Vector3 position) - Gets the height of the ground at a given position. Note : If the Vector3 is already below the ground, this will return 0. You may want to use the other overloaded function to be safe.
- static GTA.AnimatedBuilding[] GetNearbyAnimatedBuildings(GTA.Math.Vector3 position, System.Single radius)
- static GTA.Blip[] GetNearbyBlips(GTA.Math.Vector3 position, System.Single radius, GTA.BlipSprite[] blipTypes) - Gets an array of all GTA.Blip s in a given region in the World.
- static GTA.Building[] GetNearbyBuildings(GTA.Math.Vector3 position, System.Single radius)
- static GTA.Entity[] GetNearbyEntities(GTA.Math.Vector3 position, System.Single radius) - Gets an array of all GTA.Entity s in a given region in the World.
- static GTA.InteriorInstance[] GetNearbyInteriorInstances(GTA.Math.Vector3 position, System.Single radius)
- static GTA.InteriorProxy[] GetNearbyInteriorProxies(GTA.Math.Vector3 position, System.Single radius)
- static GTA.Ped[] GetNearbyPeds(GTA.Math.Vector3 position, System.Single radius, GTA.Model[] models) - Gets an array of all GTA.Ped s in a given region in the World.
- static GTA.Ped[] GetNearbyPeds(GTA.Ped ped, System.Single radius, GTA.Model[] models) - Gets an array of all GTA.Ped s near a given GTA.Ped in the world
- static GTA.Prop[] GetNearbyPickupObjects(GTA.Math.Vector3 position, System.Single radius) - Gets an array of all GTA.Prop s in a given region in the World associated with a GTA.Pickup .
- static GTA.Projectile[] GetNearbyProjectiles(GTA.Math.Vector3 position, System.Single radius) - Gets an array of all GTA.Projectile s in a given region in the World.
- static GTA.Prop[] GetNearbyProps(GTA.Math.Vector3 position, System.Single radius, GTA.Model[] models) - Gets an array of all GTA.Prop s in a given region in the World.
- static GTA.Vehicle[] GetNearbyVehicles(GTA.Math.Vector3 position, System.Single radius, GTA.Model[] models) - Gets an array of all GTA.Vehicle s in a given region in the World.
- static GTA.Vehicle[] GetNearbyVehicles(GTA.Ped ped, System.Single radius, GTA.Model[] models) - Gets an array of all GTA.Vehicle s near a given GTA.Ped in the world
- static GTA.Math.Vector3 GetNextPositionOnSidewalk(GTA.Math.Vector2 position) - Gets the next position on the street where a GTA.Ped can be placed.
- static GTA.Math.Vector3 GetNextPositionOnSidewalk(GTA.Math.Vector3 position) - Gets the next position on the street where a GTA.Ped can be placed.
- static GTA.Math.Vector3 GetNextPositionOnStreet(GTA.Math.Vector2 position, System.Boolean unoccupied = ) - Gets the next position on the street where a GTA.Vehicle can be placed.
- static GTA.Math.Vector3 GetNextPositionOnStreet(GTA.Math.Vector3 position, System.Boolean unoccupied = ) - Gets the next position on the street where a GTA.Vehicle can be placed.
- static GTA.Math.Vector3 GetSafeCoordForPed(GTA.Math.Vector3 position, System.Boolean sidewalk = , System.Int32 flags = ) - Gets the nearest safe coordinate to position a GTA.Ped .
- static System.String GetStreetName(GTA.Math.Vector2 position) - Determines the name of the street which is the closest to the given coordinates.
- static System.String GetStreetName(GTA.Math.Vector3 position) - Determines the name of the street which is the closest to the given coordinates.
- static System.String GetStreetName(GTA.Math.Vector3 position, out System.String crossingRoadName) - Determines the name of the street which is the closest to the given coordinates.
- static System.String GetZoneDisplayName(GTA.Math.Vector2 position) - Gets the display name of the a zone in the map. Use GTA.Game.GetLocalizedString(System.String) to convert to the localized name.
- static System.String GetZoneDisplayName(GTA.Math.Vector3 position) - Gets the display name of the a zone in the map. Use GTA.Game.GetLocalizedString(System.String) to convert to the localized name.
- static System.String GetZoneLocalizedName(GTA.Math.Vector2 position) - Gets the localized name of the a zone in the map.
- static System.String GetZoneLocalizedName(GTA.Math.Vector3 position) - Gets the localized name of the a zone in the map.
- static System.Boolean IsPointInAngledArea(GTA.Math.Vector3 point, GTA.Math.Vector3 originEdge, GTA.Math.Vector3 extentEdge, System.Single width, System.Boolean includeZAxis = ) - Determines whether the specified point is in the angled area. An angled area is an X-Z oriented rectangle with three parameters: origin, extent, and width.
- static System.Void PauseClock(System.Boolean value) - Pauses or resumes the in-game clock.
- static GTA.RaycastResult Raycast(GTA.Math.Vector3 source, GTA.Math.Vector3 direction, System.Single maxDistance, GTA.IntersectFlags options, GTA.Entity ignoreEntity = ) - Creates a raycast between 2 points.
- static GTA.RaycastResult Raycast(GTA.Math.Vector3 source, GTA.Math.Vector3 target, GTA.IntersectFlags options, GTA.Entity ignoreEntity = ) - Creates a raycast between 2 points.
- static GTA.RaycastResult RaycastCapsule(GTA.Math.Vector3 source, GTA.Math.Vector3 direction, System.Single maxDistance, System.Single radius, GTA.IntersectFlags options, GTA.Entity ignoreEntity = ) - Creates a 3D raycast between 2 points.
- static GTA.RaycastResult RaycastCapsule(GTA.Math.Vector3 source, GTA.Math.Vector3 target, System.Single radius, GTA.IntersectFlags options, GTA.Entity ignoreEntity = ) - Creates a 3D raycast between 2 points.
- static System.Void RemoveAllParticleEffectsInRange(GTA.Math.Vector3 pos, System.Single range) - Stops all particle effects in a range.
- static System.Void RemoveWaypoint() - Removes the waypoint.
- static System.Void ShootBullet(GTA.Math.Vector3 sourcePosition, GTA.Math.Vector3 targetPosition, GTA.Ped owner, GTA.WeaponAsset weaponAsset, System.Int32 damage, System.Single speed = ) - Fires a single bullet in the world
- static System.Void TransitionToWeather(GTA.Weather weather, System.Single duration) - Transitions to weather.

### Enum Values

## GTA.Weather

- Kind: enum
- Namespace: GTA
- Domain: World And Environment

### Constructors

- None

### Properties

- None

### Methods

- None

### Enum Values
- GTA.Weather.Blizzard
- GTA.Weather.Christmas
- GTA.Weather.Clear
- GTA.Weather.Clearing
- GTA.Weather.Clouds
- GTA.Weather.ExtraSunny
- GTA.Weather.Foggy
- GTA.Weather.Halloween
- GTA.Weather.Neutral
- GTA.Weather.Overcast
- GTA.Weather.Raining
- GTA.Weather.Smog
- GTA.Weather.Snowing
- GTA.Weather.Snowlight
- GTA.Weather.ThunderStorm
- GTA.Weather.Unknown

## GTA.Model

- Kind: struct
- Namespace: GTA
- Domain: Vehicles

### Constructors
- GTA.Model(GTA.PedHash hash)
- GTA.Model(GTA.VehicleHash hash)
- GTA.Model(GTA.WeaponHash hash)
- GTA.Model(System.Int32 hash)
- GTA.Model(System.String name)

### Properties
- System.ValueTuple<GTA.Math.Vector3, GTA.Math.Vector3> Dimensions { get; } - Gets the dimensions of this GTA.Model .
- System.Int32 Hash { get; set; } - Gets the hash for this GTA.Model .
- System.Boolean IsAmphibiousCar { get; } - Gets a value indicating whether this GTA.Model is an amphibious car.
- System.Boolean IsAmphibiousQuadBike { get; } - Gets a value indicating whether this GTA.Model is an amphibious quad bike.
- System.Boolean IsAmphibiousVehicle { get; } - Gets a value indicating whether this GTA.Model is an amphibious vehicle.
- System.Boolean IsAnimalPed { get; } - Gets a value indicating whether this GTA.Model is a animal pedestrian.
- System.Boolean IsBicycle { get; } - Gets a value indicating whether this GTA.Model is a bicycle.
- System.Boolean IsBigVehicle { get; } - Gets a value indicating whether this GTA.Model is a big vehicle whose vehicle flag has "FLAG_BIG".
- System.Boolean IsBike { get; } - Gets a value indicating whether this GTA.Model is a bike (either a motorcycle or a bicycle).
- System.Boolean IsBlimp { get; } - Gets a value indicating whether this GTA.Model is a blimp.
- System.Boolean IsBoat { get; } - Gets a value indicating whether this GTA.Model is a boat.
- System.Boolean IsBus { get; } - Gets a value indicating whether this GTA.Model is an emergency vehicle.
- System.Boolean IsCar { get; } - Gets a value indicating whether this GTA.Model is a car.
- System.Boolean IsCargobob { get; } - Gets a value indicating whether this GTA.Model is a cargobob.
- System.Boolean IsCollisionLoaded { get; } - Gets a value indicating whether the collision for this GTA.Model is loaded.
- System.Boolean IsDonk { get; } - Gets a value indicating whether this GTA.Model is a donk car.
- System.Boolean IsElectricVehicle { get; } - Gets a value indicating whether this GTA.Model is an electric vehicle.
- System.Boolean IsEmergencyVehicle { get; } - Gets a value indicating whether this GTA.Model is an emergency vehicle.
- System.Boolean IsFemalePed { get; } - Gets a value indicating whether this GTA.Model is a female pedestrian.
- System.Boolean IsGangPed { get; } - Gets a value indicating whether this GTA.Model is a gangster pedestrian.
- System.Boolean IsHelicopter { get; } - Gets a value indicating whether this GTA.Model is a helicopter.
- System.Boolean IsHumanPed { get; } - Gets a value indicating whether this GTA.Model is a human pedestrian.
- System.Boolean IsInCdImage { get; } - Gets a value indicating whether this GTA.Model is in the CD image.
- System.Boolean IsJetSki { get; } - Gets a value indicating whether this GTA.Model is a jet ski.
- System.Boolean IsLawEnforcementVehicle { get; } - Gets a value indicating whether this GTA.Model is a law enforcement vehicle.
- System.Boolean IsLoaded { get; } - Gets a value indicating whether this GTA.Model is loaded so it can be spawned.
- System.Boolean IsLowrider { get; } - Gets a value indicating whether this GTA.Model is a regular lowrider.
- System.Boolean IsMalePed { get; } - Gets a value indicating whether this GTA.Model is a male pedestrian. Without modding pedpersonality.ymt , returns true if the GTA.Model.Hash is one of the animal hashes.
- System.Boolean IsMlo { get; } - Gets a value indicating whether this GTA.Model is a movable interior loader (also known as MLO or MILO).
- System.Boolean IsMotorcycle { get; } - Gets a value indicating whether this GTA.Model is a motorcycle.
- System.Boolean IsOffRoadVehicle { get; } - Gets a value indicating whether this GTA.Model is an off-road vehicle.
- System.Boolean IsPed { get; } - Gets a value indicating whether this GTA.Model is a pedestrian.
- System.Boolean IsPlane { get; } - Gets a value indicating whether this GTA.Model is a plane.
- System.Boolean IsProp { get; } - Gets a value indicating whether this GTA.Model is a prop.
- System.Boolean IsQuadBike { get; } - Gets a value indicating whether this GTA.Model is a quad bike.
- System.Boolean IsSubmarine { get; } - Gets a value indicating whether this GTA.Model is a submarine.
- System.Boolean IsSubmarineCar { get; } - Gets a value indicating whether this GTA.Model is a submarine car.
- System.Boolean IsTank { get; } - Gets a value indicating whether this GTA.Model is a tank.
- System.Boolean IsTrailer { get; } - Gets a value indicating whether this GTA.Model is a trailer.
- System.Boolean IsTrain { get; } - Gets a value indicating whether this GTA.Model is a train.
- System.Boolean IsValid { get; } - Gets if this GTA.Model is valid.
- System.Boolean IsVan { get; } - Gets a value indicating whether this GTA.Model is a van.
- System.Boolean IsVehicle { get; } - Gets a value indicating whether this GTA.Model is a vehicle.
- System.UInt64 NativeValue { get; set; } - Gets the native representation of this GTA.Model .

### Methods
- System.Boolean Equals(GTA.Model model)
- System.Boolean Equals(System.Object obj)
- System.Int32 GetHashCode()
- System.Void MarkAsNoLongerNeeded() - Tells the game we have finished using this GTA.Model and it can be freed from memory.
- System.Boolean Request(System.Int32 timeout) - Attempts to load this GTA.Model into memory for a given period of time.
- System.Void Request() - Attempts to load this GTA.Model into memory.
- System.Boolean RequestCollision(System.Int32 timeout) - Attempts to load this GTA.Model 's collision into memory for a given period of time.
- System.Void RequestCollision() - Attempts to load this GTA.Model 's collision into memory.
- System.String ToString()

### Enum Values

## GTA.Native.Function

- Kind: static class
- Namespace: GTA.Native
- Domain: Native Interop
- Summary: A static class which handles script function execution.

### Constructors

- None

### Properties

- None

### Methods
- static System.Void Call(GTA.Native.Hash hash) - Calls the specified native script function and ignores its return value.
- static System.Void Call(GTA.Native.Hash hash, GTA.Native.InputArgument argument0) - Calls the specified native script function and ignores its return value.
- static System.Void Call(GTA.Native.Hash hash, GTA.Native.InputArgument argument0, GTA.Native.InputArgument argument1) - Calls the specified native script function and ignores its return value.
- static System.Void Call(GTA.Native.Hash hash, GTA.Native.InputArgument argument0, GTA.Native.InputArgument argument1, GTA.Native.InputArgument argument2) - Calls the specified native script function and ignores its return value.
- static System.Void Call(GTA.Native.Hash hash, GTA.Native.InputArgument argument0, GTA.Native.InputArgument argument1, GTA.Native.InputArgument argument2, GTA.Native.InputArgument argument3) - Calls the specified native script function and ignores its return value.
- static System.Void Call(GTA.Native.Hash hash, GTA.Native.InputArgument argument0, GTA.Native.InputArgument argument1, GTA.Native.InputArgument argument2, GTA.Native.InputArgument argument3, GTA.Native.InputArgument argument4) - Calls the specified native script function and ignores its return value.
- static System.Void Call(GTA.Native.Hash hash, GTA.Native.InputArgument argument0, GTA.Native.InputArgument argument1, GTA.Native.InputArgument argument2, GTA.Native.InputArgument argument3, GTA.Native.InputArgument argument4, GTA.Native.InputArgument argument5) - Calls the specified native script function and ignores its return value.
- static System.Void Call(GTA.Native.Hash hash, GTA.Native.InputArgument argument0, GTA.Native.InputArgument argument1, GTA.Native.InputArgument argument2, GTA.Native.InputArgument argument3, GTA.Native.InputArgument argument4, GTA.Native.InputArgument argument5, GTA.Native.InputArgument argument6) - Calls the specified native script function and ignores its return value.
- static System.Void Call(GTA.Native.Hash hash, GTA.Native.InputArgument argument0, GTA.Native.InputArgument argument1, GTA.Native.InputArgument argument2, GTA.Native.InputArgument argument3, GTA.Native.InputArgument argument4, GTA.Native.InputArgument argument5, GTA.Native.InputArgument argument6, GTA.Native.InputArgument argument7) - Calls the specified native script function and ignores its return value.
- static System.Void Call(GTA.Native.Hash hash, GTA.Native.InputArgument argument0, GTA.Native.InputArgument argument1, GTA.Native.InputArgument argument2, GTA.Native.InputArgument argument3, GTA.Native.InputArgument argument4, GTA.Native.InputArgument argument5, GTA.Native.InputArgument argument6, GTA.Native.InputArgument argument7, GTA.Native.InputArgument argument8) - Calls the specified native script function and ignores its return value.
- static System.Void Call(GTA.Native.Hash hash, GTA.Native.InputArgument argument0, GTA.Native.InputArgument argument1, GTA.Native.InputArgument argument2, GTA.Native.InputArgument argument3, GTA.Native.InputArgument argument4, GTA.Native.InputArgument argument5, GTA.Native.InputArgument argument6, GTA.Native.InputArgument argument7, GTA.Native.InputArgument argument8, GTA.Native.InputArgument argument9) - Calls the specified native script function and ignores its return value.
- static System.Void Call(GTA.Native.Hash hash, GTA.Native.InputArgument argument0, GTA.Native.InputArgument argument1, GTA.Native.InputArgument argument2, GTA.Native.InputArgument argument3, GTA.Native.InputArgument argument4, GTA.Native.InputArgument argument5, GTA.Native.InputArgument argument6, GTA.Native.InputArgument argument7, GTA.Native.InputArgument argument8, GTA.Native.InputArgument argument9, GTA.Native.InputArgument argument10) - Calls the specified native script function and ignores its return value.
- static System.Void Call(GTA.Native.Hash hash, GTA.Native.InputArgument argument0, GTA.Native.InputArgument argument1, GTA.Native.InputArgument argument2, GTA.Native.InputArgument argument3, GTA.Native.InputArgument argument4, GTA.Native.InputArgument argument5, GTA.Native.InputArgument argument6, GTA.Native.InputArgument argument7, GTA.Native.InputArgument argument8, GTA.Native.InputArgument argument9, GTA.Native.InputArgument argument10, GTA.Native.InputArgument argument11) - Calls the specified native script function and ignores its return value.
- static System.Void Call(GTA.Native.Hash hash, GTA.Native.InputArgument argument0, GTA.Native.InputArgument argument1, GTA.Native.InputArgument argument2, GTA.Native.InputArgument argument3, GTA.Native.InputArgument argument4, GTA.Native.InputArgument argument5, GTA.Native.InputArgument argument6, GTA.Native.InputArgument argument7, GTA.Native.InputArgument argument8, GTA.Native.InputArgument argument9, GTA.Native.InputArgument argument10, GTA.Native.InputArgument argument11, GTA.Native.InputArgument argument12) - Calls the specified native script function and ignores its return value.
- static System.Void Call(GTA.Native.Hash hash, GTA.Native.InputArgument argument0, GTA.Native.InputArgument argument1, GTA.Native.InputArgument argument2, GTA.Native.InputArgument argument3, GTA.Native.InputArgument argument4, GTA.Native.InputArgument argument5, GTA.Native.InputArgument argument6, GTA.Native.InputArgument argument7, GTA.Native.InputArgument argument8, GTA.Native.InputArgument argument9, GTA.Native.InputArgument argument10, GTA.Native.InputArgument argument11, GTA.Native.InputArgument argument12, GTA.Native.InputArgument argument13) - Calls the specified native script function and ignores its return value.
- static System.Void Call(GTA.Native.Hash hash, GTA.Native.InputArgument argument0, GTA.Native.InputArgument argument1, GTA.Native.InputArgument argument2, GTA.Native.InputArgument argument3, GTA.Native.InputArgument argument4, GTA.Native.InputArgument argument5, GTA.Native.InputArgument argument6, GTA.Native.InputArgument argument7, GTA.Native.InputArgument argument8, GTA.Native.InputArgument argument9, GTA.Native.InputArgument argument10, GTA.Native.InputArgument argument11, GTA.Native.InputArgument argument12, GTA.Native.InputArgument argument13, GTA.Native.InputArgument argument14) - Calls the specified native script function and ignores its return value.
- static System.Void Call(GTA.Native.Hash hash, GTA.Native.InputArgument argument0, GTA.Native.InputArgument argument1, GTA.Native.InputArgument argument2, GTA.Native.InputArgument argument3, GTA.Native.InputArgument argument4, GTA.Native.InputArgument argument5, GTA.Native.InputArgument argument6, GTA.Native.InputArgument argument7, GTA.Native.InputArgument argument8, GTA.Native.InputArgument argument9, GTA.Native.InputArgument argument10, GTA.Native.InputArgument argument11, GTA.Native.InputArgument argument12, GTA.Native.InputArgument argument13, GTA.Native.InputArgument argument14, GTA.Native.InputArgument argument15) - Calls the specified native script function and ignores its return value.
- static System.Void Call(GTA.Native.Hash hash, GTA.Native.InputArgument[] arguments) - Calls the specified native script function and ignores its return value.
- static T Call<T>(GTA.Native.Hash hash) - Calls the specified native script function and returns its return value.
- static T Call<T>(GTA.Native.Hash hash, GTA.Native.InputArgument argument) - Calls the specified native script function and ignores its return value.
- static T Call<T>(GTA.Native.Hash hash, GTA.Native.InputArgument argument0, GTA.Native.InputArgument argument1) - Calls the specified native script function and ignores its return value.
- static T Call<T>(GTA.Native.Hash hash, GTA.Native.InputArgument argument0, GTA.Native.InputArgument argument1, GTA.Native.InputArgument argument2) - Calls the specified native script function and ignores its return value.
- static T Call<T>(GTA.Native.Hash hash, GTA.Native.InputArgument argument0, GTA.Native.InputArgument argument1, GTA.Native.InputArgument argument2, GTA.Native.InputArgument argument3) - Calls the specified native script function and ignores its return value.
- static T Call<T>(GTA.Native.Hash hash, GTA.Native.InputArgument argument0, GTA.Native.InputArgument argument1, GTA.Native.InputArgument argument2, GTA.Native.InputArgument argument3, GTA.Native.InputArgument argument4) - Calls the specified native script function and ignores its return value.
- static T Call<T>(GTA.Native.Hash hash, GTA.Native.InputArgument argument0, GTA.Native.InputArgument argument1, GTA.Native.InputArgument argument2, GTA.Native.InputArgument argument3, GTA.Native.InputArgument argument4, GTA.Native.InputArgument argument5) - Calls the specified native script function and ignores its return value.
- static T Call<T>(GTA.Native.Hash hash, GTA.Native.InputArgument argument0, GTA.Native.InputArgument argument1, GTA.Native.InputArgument argument2, GTA.Native.InputArgument argument3, GTA.Native.InputArgument argument4, GTA.Native.InputArgument argument5, GTA.Native.InputArgument argument6) - Calls the specified native script function and ignores its return value.
- static T Call<T>(GTA.Native.Hash hash, GTA.Native.InputArgument argument0, GTA.Native.InputArgument argument1, GTA.Native.InputArgument argument2, GTA.Native.InputArgument argument3, GTA.Native.InputArgument argument4, GTA.Native.InputArgument argument5, GTA.Native.InputArgument argument6, GTA.Native.InputArgument argument7) - Calls the specified native script function and ignores its return value.
- static T Call<T>(GTA.Native.Hash hash, GTA.Native.InputArgument argument0, GTA.Native.InputArgument argument1, GTA.Native.InputArgument argument2, GTA.Native.InputArgument argument3, GTA.Native.InputArgument argument4, GTA.Native.InputArgument argument5, GTA.Native.InputArgument argument6, GTA.Native.InputArgument argument7, GTA.Native.InputArgument argument8) - Calls the specified native script function and ignores its return value.
- static T Call<T>(GTA.Native.Hash hash, GTA.Native.InputArgument argument0, GTA.Native.InputArgument argument1, GTA.Native.InputArgument argument2, GTA.Native.InputArgument argument3, GTA.Native.InputArgument argument4, GTA.Native.InputArgument argument5, GTA.Native.InputArgument argument6, GTA.Native.InputArgument argument7, GTA.Native.InputArgument argument8, GTA.Native.InputArgument argument9) - Calls the specified native script function and ignores its return value.
- static T Call<T>(GTA.Native.Hash hash, GTA.Native.InputArgument argument0, GTA.Native.InputArgument argument1, GTA.Native.InputArgument argument2, GTA.Native.InputArgument argument3, GTA.Native.InputArgument argument4, GTA.Native.InputArgument argument5, GTA.Native.InputArgument argument6, GTA.Native.InputArgument argument7, GTA.Native.InputArgument argument8, GTA.Native.InputArgument argument9, GTA.Native.InputArgument argument10) - Calls the specified native script function and ignores its return value.
- static T Call<T>(GTA.Native.Hash hash, GTA.Native.InputArgument argument0, GTA.Native.InputArgument argument1, GTA.Native.InputArgument argument2, GTA.Native.InputArgument argument3, GTA.Native.InputArgument argument4, GTA.Native.InputArgument argument5, GTA.Native.InputArgument argument6, GTA.Native.InputArgument argument7, GTA.Native.InputArgument argument8, GTA.Native.InputArgument argument9, GTA.Native.InputArgument argument10, GTA.Native.InputArgument argument11) - Calls the specified native script function and ignores its return value.
- static T Call<T>(GTA.Native.Hash hash, GTA.Native.InputArgument argument0, GTA.Native.InputArgument argument1, GTA.Native.InputArgument argument2, GTA.Native.InputArgument argument3, GTA.Native.InputArgument argument4, GTA.Native.InputArgument argument5, GTA.Native.InputArgument argument6, GTA.Native.InputArgument argument7, GTA.Native.InputArgument argument8, GTA.Native.InputArgument argument9, GTA.Native.InputArgument argument10, GTA.Native.InputArgument argument11, GTA.Native.InputArgument argument12) - Calls the specified native script function and ignores its return value.
- static T Call<T>(GTA.Native.Hash hash, GTA.Native.InputArgument argument0, GTA.Native.InputArgument argument1, GTA.Native.InputArgument argument2, GTA.Native.InputArgument argument3, GTA.Native.InputArgument argument4, GTA.Native.InputArgument argument5, GTA.Native.InputArgument argument6, GTA.Native.InputArgument argument7, GTA.Native.InputArgument argument8, GTA.Native.InputArgument argument9, GTA.Native.InputArgument argument10, GTA.Native.InputArgument argument11, GTA.Native.InputArgument argument12, GTA.Native.InputArgument argument13) - Calls the specified native script function and ignores its return value.
- static T Call<T>(GTA.Native.Hash hash, GTA.Native.InputArgument argument0, GTA.Native.InputArgument argument1, GTA.Native.InputArgument argument2, GTA.Native.InputArgument argument3, GTA.Native.InputArgument argument4, GTA.Native.InputArgument argument5, GTA.Native.InputArgument argument6, GTA.Native.InputArgument argument7, GTA.Native.InputArgument argument8, GTA.Native.InputArgument argument9, GTA.Native.InputArgument argument10, GTA.Native.InputArgument argument11, GTA.Native.InputArgument argument12, GTA.Native.InputArgument argument13, GTA.Native.InputArgument argument14) - Calls the specified native script function and ignores its return value.
- static T Call<T>(GTA.Native.Hash hash, GTA.Native.InputArgument argument0, GTA.Native.InputArgument argument1, GTA.Native.InputArgument argument2, GTA.Native.InputArgument argument3, GTA.Native.InputArgument argument4, GTA.Native.InputArgument argument5, GTA.Native.InputArgument argument6, GTA.Native.InputArgument argument7, GTA.Native.InputArgument argument8, GTA.Native.InputArgument argument9, GTA.Native.InputArgument argument10, GTA.Native.InputArgument argument11, GTA.Native.InputArgument argument12, GTA.Native.InputArgument argument13, GTA.Native.InputArgument argument14, GTA.Native.InputArgument argument15) - Calls the specified native script function and ignores its return value.
- static T Call<T>(GTA.Native.Hash hash, GTA.Native.InputArgument[] arguments) - Calls the specified native script function and returns its return value.

### Enum Values

## GTA.Native.Hash

- Kind: enum
- Namespace: GTA.Native
- Domain: Native Interop

### Constructors

- None

### Properties

- None

### Methods

- None

### Enum Values
- GTA.Native.Hash.ABORT_VEHICLE_CREW_EMBLEM_REQUEST
- GTA.Native.Hash.ABSF
- GTA.Native.Hash.ABSI
- GTA.Native.Hash.ACOS
- GTA.Native.Hash.ACTION_MANAGER_ENABLE_ACTION
- GTA.Native.Hash.ACTIVATE_AUDIO_SLOWMO_MODE
- GTA.Native.Hash.ACTIVATE_DAMAGE_TRACKER_ON_NETWORK_ID
- GTA.Native.Hash.ACTIVATE_DAMAGE_TRACKER_ON_PLAYER
- GTA.Native.Hash.ACTIVATE_FRONTEND_MENU
- GTA.Native.Hash.ACTIVATE_INTERIOR_ENTITY_SET
- GTA.Native.Hash.ACTIVATE_INTERIOR_GROUPS_USING_CAMERA
- GTA.Native.Hash.ACTIVATE_PHYSICS
- GTA.Native.Hash.ACTIVATE_ROCKSTAR_EDITOR
- GTA.Native.Hash.ACTIVITY_FEED_ACTION_START_WITH_COMMAND_LINE
- GTA.Native.Hash.ACTIVITY_FEED_ACTION_START_WITH_COMMAND_LINE_ADD
- GTA.Native.Hash.ACTIVITY_FEED_ADD_INT_TO_CAPTION
- GTA.Native.Hash.ACTIVITY_FEED_ADD_LITERAL_SUBSTRING_TO_CAPTION
- GTA.Native.Hash.ACTIVITY_FEED_ADD_SUBSTRING_TO_CAPTION
- GTA.Native.Hash.ACTIVITY_FEED_CREATE
- GTA.Native.Hash.ACTIVITY_FEED_LARGE_IMAGE_URL
- GTA.Native.Hash.ACTIVITY_FEED_ONLINE_PLAYED_WITH_POST
- GTA.Native.Hash.ACTIVITY_FEED_POST
- GTA.Native.Hash.ADD_AMMO_TO_PED
- GTA.Native.Hash.ADD_ARMOUR_TO_PED
- GTA.Native.Hash.ADD_BLIP_FOR_AREA
- GTA.Native.Hash.ADD_BLIP_FOR_COORD
- GTA.Native.Hash.ADD_BLIP_FOR_ENTITY
- GTA.Native.Hash.ADD_BLIP_FOR_PICKUP
- GTA.Native.Hash.ADD_BLIP_FOR_RADIUS
- GTA.Native.Hash.ADD_CAM_SPLINE_NODE
- GTA.Native.Hash.ADD_CAM_SPLINE_NODE_USING_CAMERA
- GTA.Native.Hash.ADD_CAM_SPLINE_NODE_USING_CAMERA_FRAME
- GTA.Native.Hash.ADD_CAM_SPLINE_NODE_USING_GAMEPLAY_FRAME
- GTA.Native.Hash.ADD_COVER_BLOCKING_AREA
- GTA.Native.Hash.ADD_COVER_POINT
- GTA.Native.Hash.ADD_DECAL
- GTA.Native.Hash.ADD_DISPATCH_SPAWN_ANGLED_BLOCKING_AREA
- GTA.Native.Hash.ADD_DISPATCH_SPAWN_SPHERE_BLOCKING_AREA
- GTA.Native.Hash.ADD_DOOR_TO_SYSTEM
- GTA.Native.Hash.ADD_ENTITY_ICON
- GTA.Native.Hash.ADD_ENTITY_TO_AUDIO_MIX_GROUP
- GTA.Native.Hash.ADD_EXPLOSION
- GTA.Native.Hash.ADD_EXPLOSION_WITH_USER_VFX
- GTA.Native.Hash.ADD_EXTENDED_PICKUP_PROBE_AREA
- GTA.Native.Hash.ADD_EXTRA_CALMING_QUAD
- GTA.Native.Hash.ADD_HOSPITAL_RESTART
- GTA.Native.Hash.ADD_LINE_TO_CONVERSATION
- GTA.Native.Hash.ADD_MODEL_TO_CREATOR_BUDGET
- GTA.Native.Hash.ADD_NAVMESH_BLOCKING_OBJECT
- GTA.Native.Hash.ADD_NAVMESH_REQUIRED_REGION
- GTA.Native.Hash.ADD_NEXT_MESSAGE_TO_PREVIOUS_BRIEFS
- GTA.Native.Hash.ADD_OIL_DECAL
- GTA.Native.Hash.ADD_OWNED_EXPLOSION
- GTA.Native.Hash.ADD_PATROL_ROUTE_LINK
- GTA.Native.Hash.ADD_PATROL_ROUTE_NODE
- GTA.Native.Hash.ADD_PED_AMMO_BY_TYPE
- GTA.Native.Hash.ADD_PED_DECORATION_FROM_HASHES
- GTA.Native.Hash.ADD_PED_DECORATION_FROM_HASHES_IN_CORONA
- GTA.Native.Hash.ADD_PED_TO_CONVERSATION
- GTA.Native.Hash.ADD_PETROL_DECAL
- GTA.Native.Hash.ADD_PETROL_TRAIL_DECAL_INFO
- GTA.Native.Hash.ADD_PICKUP_TO_INTERIOR_ROOM_BY_NAME
- GTA.Native.Hash.ADD_PLAYER_TARGETABLE_ENTITY
- GTA.Native.Hash.ADD_POINT_TO_GPS_CUSTOM_ROUTE
- GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE
- GTA.Native.Hash.ADD_POLICE_RESTART
- GTA.Native.Hash.ADD_POP_MULTIPLIER_AREA
- GTA.Native.Hash.ADD_POP_MULTIPLIER_SPHERE
- GTA.Native.Hash.ADD_RELATIONSHIP_GROUP
- GTA.Native.Hash.ADD_REPLAY_STAT_VALUE
- GTA.Native.Hash.ADD_ROAD_NODE_SPEED_ZONE
- GTA.Native.Hash.ADD_ROPE
- GTA.Native.Hash.ADD_SCENARIO_BLOCKING_AREA
- GTA.Native.Hash.ADD_SCRIPTED_COVER_AREA
- GTA.Native.Hash.ADD_SCRIPT_TO_RANDOM_PED
- GTA.Native.Hash.ADD_SHOCKING_EVENT_AT_POSITION
- GTA.Native.Hash.ADD_SHOCKING_EVENT_FOR_ENTITY
- GTA.Native.Hash.ADD_STUNT_JUMP
- GTA.Native.Hash.ADD_STUNT_JUMP_ANGLED
- GTA.Native.Hash.ADD_TACTICAL_NAV_MESH_POINT
- GTA.Native.Hash.ADD_TCMODIFIER_OVERRIDE
- GTA.Native.Hash.ADD_TEXT_COMPONENT_FLOAT
- GTA.Native.Hash.ADD_TEXT_COMPONENT_FORMATTED_INTEGER
- GTA.Native.Hash.ADD_TEXT_COMPONENT_INTEGER
- GTA.Native.Hash.ADD_TEXT_COMPONENT_SUBSTRING_BLIP_NAME
- GTA.Native.Hash.ADD_TEXT_COMPONENT_SUBSTRING_KEYBOARD_DISPLAY
- GTA.Native.Hash.ADD_TEXT_COMPONENT_SUBSTRING_PHONE_NUMBER
- GTA.Native.Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME
- GTA.Native.Hash.ADD_TEXT_COMPONENT_SUBSTRING_TEXT_LABEL
- GTA.Native.Hash.ADD_TEXT_COMPONENT_SUBSTRING_TEXT_LABEL_HASH_KEY
- GTA.Native.Hash.ADD_TEXT_COMPONENT_SUBSTRING_TIME
- GTA.Native.Hash.ADD_TEXT_COMPONENT_SUBSTRING_WEBSITE
- GTA.Native.Hash.ADD_TO_CLOCK_TIME
- GTA.Native.Hash.ADD_TO_ITEMSET
- GTA.Native.Hash.ADD_VALID_VEHICLE_HIT_HASH
- GTA.Native.Hash.ADD_VEHICLE_COMBAT_ANGLED_AVOIDANCE_AREA
- GTA.Native.Hash.ADD_VEHICLE_CREW_EMBLEM
- GTA.Native.Hash.ADD_VEHICLE_PHONE_EXPLOSIVE_DEVICE
- GTA.Native.Hash.ADD_VEHICLE_STUCK_CHECK_WITH_WARP
- GTA.Native.Hash.ADD_VEHICLE_SUBTASK_ATTACK_COORD
- GTA.Native.Hash.ADD_VEHICLE_SUBTASK_ATTACK_PED
- GTA.Native.Hash.ADD_VEHICLE_UPSIDEDOWN_CHECK
- GTA.Native.Hash.ADJUST_AMBIENT_PED_SPAWN_DENSITIES_THIS_FRAME
- GTA.Native.Hash.ADJUST_NEXT_POS_SIZE_AS_NORMALIZED_16_9
- GTA.Native.Hash.ADVANCE_CLOCK_TIME_TO
- GTA.Native.Hash.ALLOW_ALL_PLAYERS_TO_COLLECT_PICKUPS_OF_TYPE
- GTA.Native.Hash.ALLOW_ALTERNATIVE_SCRIPT_CONTROLS_LAYOUT
- GTA.Native.Hash.ALLOW_AMBIENT_VEHICLES_TO_AVOID_ADVERSE_CONDITIONS
- GTA.Native.Hash.ALLOW_BOAT_BOOM_TO_ANIMATE
- GTA.Native.Hash.ALLOW_DAMAGE_EVENTS_FOR_NON_NETWORKED_OBJECTS
- GTA.Native.Hash.ALLOW_DISPLAY_OF_MULTIPLAYER_CASH_TEXT
- GTA.Native.Hash.ALLOW_EVASION_HUD_IF_DISABLING_HIDDEN_EVASION_THIS_FRAME
- GTA.Native.Hash.ALLOW_MISSION_CREATOR_WARP
- GTA.Native.Hash.ALLOW_MOTION_BLUR_DECAY
- GTA.Native.Hash.ALLOW_PAUSE_WHEN_NOT_IN_STATE_OF_PLAY_THIS_FRAME
- GTA.Native.Hash.ALLOW_PICKUP_ARROW_MARKER_WHEN_UNCOLLECTABLE
- GTA.Native.Hash.ALLOW_PICKUP_BY_NONE_PARTICIPANT
- GTA.Native.Hash.ALLOW_PLAYER_SWITCH_ASCENT
- GTA.Native.Hash.ALLOW_PLAYER_SWITCH_DESCENT
- GTA.Native.Hash.ALLOW_PLAYER_SWITCH_OUTRO
- GTA.Native.Hash.ALLOW_PLAYER_SWITCH_PAN
- GTA.Native.Hash.ALLOW_PORTABLE_PICKUP_TO_MIGRATE_TO_NON_PARTICIPANTS
- GTA.Native.Hash.ALLOW_SONAR_BLIPS
- GTA.Native.Hash.ALLOW_TRAIN_TO_BE_REMOVED_BY_POPULATION
- GTA.Native.Hash.ANIMATED_SHAKE_CAM
- GTA.Native.Hash.ANIMATED_SHAKE_SCRIPT_GLOBAL
- GTA.Native.Hash.ANIMPOSTFX_GET_CURRENT_TIME
- GTA.Native.Hash.ANIMPOSTFX_IS_RUNNING
- GTA.Native.Hash.ANIMPOSTFX_PLAY
- GTA.Native.Hash.ANIMPOSTFX_STOP
- GTA.Native.Hash.ANIMPOSTFX_STOP_ALL
- GTA.Native.Hash.ANIMPOSTFX_STOP_AND_FLUSH_REQUESTS
- GTA.Native.Hash.APPLY_DAMAGE_TO_PED
- GTA.Native.Hash.APPLY_FORCE_TO_ENTITY
- GTA.Native.Hash.APPLY_FORCE_TO_ENTITY_CENTER_OF_MASS
- GTA.Native.Hash.APPLY_IMPULSE_TO_CLOTH
- GTA.Native.Hash.APPLY_PED_BLOOD
- GTA.Native.Hash.APPLY_PED_BLOOD_BY_ZONE
- GTA.Native.Hash.APPLY_PED_BLOOD_DAMAGE_BY_ZONE
- GTA.Native.Hash.APPLY_PED_BLOOD_SPECIFIC
- GTA.Native.Hash.APPLY_PED_DAMAGE_DECAL
- GTA.Native.Hash.APPLY_PED_DAMAGE_PACK
- GTA.Native.Hash.APP_CLEAR_BLOCK
- GTA.Native.Hash.APP_CLOSE_APP
- GTA.Native.Hash.APP_CLOSE_BLOCK
- GTA.Native.Hash.APP_DATA_VALID
- GTA.Native.Hash.APP_DELETE_APP_DATA
- GTA.Native.Hash.APP_GET_DELETED_FILE_STATUS
- GTA.Native.Hash.APP_GET_FLOAT
- GTA.Native.Hash.APP_GET_INT
- GTA.Native.Hash.APP_GET_STRING
- GTA.Native.Hash.APP_HAS_LINKED_SOCIAL_CLUB_ACCOUNT
- GTA.Native.Hash.APP_HAS_SYNCED_DATA
- GTA.Native.Hash.APP_SAVE_DATA
- GTA.Native.Hash.APP_SET_APP
- GTA.Native.Hash.APP_SET_BLOCK
- GTA.Native.Hash.APP_SET_FLOAT
- GTA.Native.Hash.APP_SET_INT
- GTA.Native.Hash.APP_SET_STRING
- GTA.Native.Hash.ARE_ALL_NAVMESH_REGIONS_LOADED
- GTA.Native.Hash.ARE_ALL_VEHICLE_WINDOWS_INTACT
- GTA.Native.Hash.ARE_ANY_CCS_PENDING
- GTA.Native.Hash.ARE_ANY_VEHICLE_SEATS_FREE
- GTA.Native.Hash.ARE_CUTSCENE_ENTITIES_NETWORKED
- GTA.Native.Hash.ARE_ENTITIES_ENTIRELY_INSIDE_GARAGE
- GTA.Native.Hash.ARE_FOLDING_WINGS_DEPLOYED
- GTA.Native.Hash.ARE_NODES_LOADED_FOR_AREA
- GTA.Native.Hash.ARE_ONLINE_POLICIES_UP_TO_DATE
- GTA.Native.Hash.ARE_PLANE_CONTROL_PANELS_INTACT
- GTA.Native.Hash.ARE_PLANE_PROPELLERS_INTACT
- GTA.Native.Hash.ARE_PLAYER_FLASHING_STARS_ABOUT_TO_DROP
- GTA.Native.Hash.ARE_PLAYER_STARS_GREYED_OUT
- GTA.Native.Hash.ARE_PROFILE_SETTINGS_VALID
- GTA.Native.Hash.ARE_STRINGS_EQUAL
- GTA.Native.Hash.ARE_WIDESCREEN_BORDERS_ACTIVE
- GTA.Native.Hash.ARE_WINGS_OF_PLANE_INTACT
- GTA.Native.Hash.ASIN
- GTA.Native.Hash.ASSISTED_MOVEMENT_CLOSE_ROUTE
- GTA.Native.Hash.ASSISTED_MOVEMENT_FLUSH_ROUTE
- GTA.Native.Hash.ASSISTED_MOVEMENT_IS_ROUTE_LOADED
- GTA.Native.Hash.ASSISTED_MOVEMENT_OVERRIDE_LOAD_DISTANCE_THIS_FRAME
- GTA.Native.Hash.ASSISTED_MOVEMENT_REMOVE_ROUTE
- GTA.Native.Hash.ASSISTED_MOVEMENT_REQUEST_ROUTE
- GTA.Native.Hash.ASSISTED_MOVEMENT_SET_ROUTE_PROPERTIES
- GTA.Native.Hash.ATAN
- GTA.Native.Hash.ATAN2
- GTA.Native.Hash.ATTACH_CAM_TO_ENTITY
- GTA.Native.Hash.ATTACH_CAM_TO_PED_BONE
- GTA.Native.Hash.ATTACH_CAM_TO_VEHICLE_BONE
- GTA.Native.Hash.ATTACH_CONTAINER_TO_HANDLER_FRAME_WHEN_LINED_UP
- GTA.Native.Hash.ATTACH_ENTITIES_TO_ROPE
- GTA.Native.Hash.ATTACH_ENTITY_BONE_TO_ENTITY_BONE
- GTA.Native.Hash.ATTACH_ENTITY_BONE_TO_ENTITY_BONE_Y_FORWARD
- GTA.Native.Hash.ATTACH_ENTITY_TO_CARGOBOB
- GTA.Native.Hash.ATTACH_ENTITY_TO_ENTITY
- GTA.Native.Hash.ATTACH_ENTITY_TO_ENTITY_PHYSICALLY
- GTA.Native.Hash.ATTACH_PORTABLE_PICKUP_TO_PED
- GTA.Native.Hash.ATTACH_ROPE_TO_ENTITY
- GTA.Native.Hash.ATTACH_SYNCHRONIZED_SCENE_TO_ENTITY
- GTA.Native.Hash.ATTACH_TV_AUDIO_TO_ENTITY
- GTA.Native.Hash.ATTACH_VEHICLE_ON_TO_TRAILER
- GTA.Native.Hash.ATTACH_VEHICLE_TO_CARGOBOB
- GTA.Native.Hash.ATTACH_VEHICLE_TO_TOW_TRUCK
- GTA.Native.Hash.ATTACH_VEHICLE_TO_TRAILER
- GTA.Native.Hash.AUDIO_IS_MUSIC_PLAYING
- GTA.Native.Hash.AUDIO_IS_SCRIPTED_MUSIC_PLAYING
- GTA.Native.Hash.BAD_SPORT_PLAYER_LEFT_DETECTED
- GTA.Native.Hash.BEGIN_CREATE_LOW_QUALITY_COPY_OF_PHOTO
- GTA.Native.Hash.BEGIN_CREATE_MISSION_CREATOR_PHOTO_PREVIEW
- GTA.Native.Hash.BEGIN_REPLAY_STATS
- GTA.Native.Hash.BEGIN_SCALEFORM_MOVIE_METHOD
- GTA.Native.Hash.BEGIN_SCALEFORM_MOVIE_METHOD_ON_FRONTEND
- GTA.Native.Hash.BEGIN_SCALEFORM_MOVIE_METHOD_ON_FRONTEND_HEADER
- GTA.Native.Hash.BEGIN_SCALEFORM_SCRIPT_HUD_MOVIE_METHOD
- GTA.Native.Hash.BEGIN_SRL
- GTA.Native.Hash.BEGIN_TAKE_HIGH_QUALITY_PHOTO
- GTA.Native.Hash.BEGIN_TAKE_MISSION_CREATOR_PHOTO
- GTA.Native.Hash.BEGIN_TEXT_COMMAND_ADD_DIRECTLY_TO_PREVIOUS_BRIEFS
- GTA.Native.Hash.BEGIN_TEXT_COMMAND_BUSYSPINNER_ON
- GTA.Native.Hash.BEGIN_TEXT_COMMAND_CLEAR_PRINT
- ... truncated; query the source snapshot for more members.

## GTA.ExplosionType

- Kind: enum
- Namespace: GTA
- Domain: World And Environment

### Constructors

- None

### Properties

- None

### Methods

- None

### Enum Values
- GTA.ExplosionType.AirDefense
- GTA.ExplosionType.ApcShell
- GTA.ExplosionType.BZGas
- GTA.ExplosionType.Barrel
- GTA.ExplosionType.Bike
- GTA.ExplosionType.BirdCrap
- GTA.ExplosionType.Blimp
- GTA.ExplosionType.Blimp2
- GTA.ExplosionType.Boat
- GTA.ExplosionType.BombCluster
- GTA.ExplosionType.BombClusterSecondary
- GTA.ExplosionType.BombGas
- GTA.ExplosionType.BombIncendiary
- GTA.ExplosionType.BombStandard
- GTA.ExplosionType.BombStandardWide
- GTA.ExplosionType.BombWater
- GTA.ExplosionType.BombWaterSecondary
- GTA.ExplosionType.BombushkaCannon
- GTA.ExplosionType.Bullet
- GTA.ExplosionType.BuriedMine
- GTA.ExplosionType.Car
- GTA.ExplosionType.EmpLauncherEmp
- GTA.ExplosionType.ExplosiveAmmo
- GTA.ExplosionType.ExplosiveAmmoShotgun
- GTA.ExplosionType.Extinguisher
- GTA.ExplosionType.FireWork
- GTA.ExplosionType.Flame
- GTA.ExplosionType.FlameExplode
- GTA.ExplosionType.Flare
- GTA.ExplosionType.GasCanister
- GTA.ExplosionType.GasCanister2
- GTA.ExplosionType.GasTank
- GTA.ExplosionType.Grenade
- GTA.ExplosionType.GrenadeL
- GTA.ExplosionType.HiOctane
- GTA.ExplosionType.HunterBarrage
- GTA.ExplosionType.HunterCannon
- GTA.ExplosionType.MineUnderwater
- GTA.ExplosionType.Molotov1
- GTA.ExplosionType.MortarKinetic
- GTA.ExplosionType.Oppressor2Cannon
- GTA.ExplosionType.OrbitalCannon
- GTA.ExplosionType.PetrolPump
- GTA.ExplosionType.PipeBomb
- GTA.ExplosionType.Plane
- GTA.ExplosionType.PlaneRocket
- GTA.ExplosionType.ProgramAR
- GTA.ExplosionType.Propane
- GTA.ExplosionType.ProxMine
- GTA.ExplosionType.RCTankRocket
- GTA.ExplosionType.Railgun
- GTA.ExplosionType.RayGun
- GTA.ExplosionType.Rocket
- GTA.ExplosionType.RogueCannon
- GTA.ExplosionType.ScriptDrone
- GTA.ExplosionType.ScriptMissile
- GTA.ExplosionType.ScriptMissileLarge
- GTA.ExplosionType.ShipDestroy
- GTA.ExplosionType.SmokeG
- GTA.ExplosionType.SmokeGL
- GTA.ExplosionType.SnowBall
- GTA.ExplosionType.Steam
- GTA.ExplosionType.StickyBomb
- GTA.ExplosionType.SubmarineBig
- GTA.ExplosionType.TankShell
- GTA.ExplosionType.Tanker
- GTA.ExplosionType.Torpedo
- GTA.ExplosionType.TorpedoUnderwater
- GTA.ExplosionType.Train
- GTA.ExplosionType.Truck
- GTA.ExplosionType.Valkyrie
- GTA.ExplosionType.VehicleBullet
- GTA.ExplosionType.VehicleMine
- GTA.ExplosionType.VehiclemineEmp
- GTA.ExplosionType.VehiclemineKinetic
- GTA.ExplosionType.VehiclemineSlick
- GTA.ExplosionType.VehiclemineSpike
- GTA.ExplosionType.VehiclemineTar
- GTA.ExplosionType.WaterHydrant

---

# SHVDN Vehicle APIs

Vehicle APIs and common enums for vehicle-based LIVE Studio effects.

Snapshot: 2026-05-19
SHVDN version: 3.6.0.0

Use this as Dify Knowledge Base context. Do not paste the full source JSON into prompts.

## GTA.Vehicle

- Kind: class
- Namespace: GTA
- Domain: Vehicles

### Constructors

- None

### Properties
- System.Single Acceleration { get; } - Gets the acceleration of this GTA.Vehicle .
- System.Int32 AlarmTimeLeft { get; set; } - Gets or sets time left before this GTA.Vehicle alarm stops. If greater than zero, the vehicle alarm will be sounding. the value is up to 65534.
- System.Boolean AllowRappel { get; } - Gets a value indicating whether this GTA.Vehicle allows GTA.Ped s to rappel.
- System.Boolean AreBrakeLightsOn { set; } - Gets or sets a value indicating whether this GTA.Vehicle has its brake light on.
- System.Boolean AreHighBeamsOn { get; set; } - Gets or sets a value indicating whether this GTA.Vehicle has its high beams on.
- System.Boolean AreLightsOn { get; set; } - Gets or sets a value indicating whether this GTA.Vehicle has its lights on.
- System.Single BodyHealth { get; set; } - Gets or sets this GTA.Vehicle s body health.
- System.Single BrakePower { get; set; } - Gets or sets the current brake power of this GTA.Vehicle .
- System.Boolean CanBeVisiblyDamaged { set; }
- System.Boolean CanEngineDegrade { set; }
- System.Boolean CanJump { get; } - Gets a value indicating whether this GTA.Vehicle can jump.
- System.Boolean CanPretendOccupants { get; set; } - Gets or sets a value indicating whether this GTA.Vehicle can pretend it has the same GTA.Ped s. Set to false to prevent this GTA.Vehicle from creating new GTA.Ped s as its occupants.
- System.Boolean CanStandOnTop { get; } - Gets a value indicating whether GTA.Ped s can stand on this GTA.Vehicle regardless of GTA.Vehicle s speed.
- System.Boolean CanTiresBurst { get; set; }
- System.Boolean CanWheelsBreak { get; set; }
- System.String ClassDisplayName { get; } - Gets the display name of this GTA.Vehicle s GTA.VehicleClass . Use GTA.Game.GetLocalizedString(System.String) to get the localized class name.
- System.String ClassLocalizedName { get; } - Gets the localized name of this GTA.Vehicle s GTA.VehicleClass .
- GTA.VehicleClass ClassType { get; } - Gets the class of this GTA.Vehicle .
- System.Single Clutch { get; set; } - Gets or sets the current clutch of this GTA.Vehicle .
- System.Int32 CurrentGear { get; set; } - Gets or sets the current gear this GTA.Vehicle is using.
- System.Single CurrentRPM { get; set; } - Gets or sets the current RPM of this GTA.Vehicle .
- System.Single DirtLevel { get; set; }
- System.String DisplayName { get; } - Gets the display name of this GTA.Vehicle . Use GTA.Game.GetLocalizedString(System.String) to get the localized name.
- GTA.VehicleDoorCollection Doors { get; }
- GTA.Ped Driver { get; }
- System.Boolean DropsMoneyOnExplosion { get; set; } - Gets or sets a value indicating whether this GTA.Vehicle drops money when destroyed. Only works when the vehicle model is a car, quad bikes or trikes (strictly when the internal vehicle class is CAutomobile or derived class from CAutomobile).
- System.Single EngineHealth { get; set; } - Gets or sets this GTA.Vehicle engine health.
- System.Single EnginePowerMultiplier { get; set; }
- System.Single EngineTemperature { get; } - Gets the engine temperature of this GTA.Vehicle .
- System.Single EngineTorqueMultiplier { set; }
- System.Single ForwardSpeed { set; } - Sets this GTA.Vehicle s forward speed.
- System.Single FuelLevel { get; set; } - Gets or sets this GTA.Vehicle fuel level.
- System.Int32 Gears { get; set; } - Gets or sets the gears value of this GTA.Vehicle .
- GTA.HandlingData HandlingData { get; }
- System.Boolean HasBombBay { get; }
- System.Boolean HasBulletProofGlass { get; }
- System.Boolean HasDonkHydraulics { get; }
- System.Boolean HasForks { get; } - Gets a value indicating whether this GTA.Vehicle has forks.
- System.Boolean HasLowriderHydraulics { get; }
- System.Boolean HasParachute { get; }
- System.Boolean HasRocketBoost { get; }
- System.Boolean HasRoof { get; }
- System.Boolean HasSiren { get; } - Gets a value indicating whether this GTA.Vehicle has a siren.
- System.Boolean HasTowArm { get; }
- System.Single HeliBladesSpeed { get; set; } - Gets or sets the blades speed for this heli.
- System.Single HeliEngineHealth { get; set; } - Gets or sets the engine health for this heli.
- System.Single HeliMainRotorHealth { get; set; } - Gets or sets the main rotor health for this heli.
- System.Single HeliTailRotorHealth { get; set; } - Gets or sets the tail rotor health for this heli.
- System.Int32 HighGear { get; set; }
- System.Boolean IsAircraft { get; } - Gets a value indicating whether this GTA.Vehicle is an aircraft.
- System.Boolean IsAlarmSet { get; set; } - Sets a value indicating whether this GTA.Vehicle has an alarm set.
- System.Boolean IsAlarmSounding { get; } - Gets a value indicating whether this GTA.Vehicle is sounding its alarm.
- System.Boolean IsAmphibious { get; } - Gets a value indicating whether this GTA.Vehicle is an amphibious vehicle.
- System.Boolean IsAmphibiousAutomobile { get; } - Gets a value indicating whether this GTA.Vehicle is an amphibious automobile.
- System.Boolean IsAmphibiousQuadBike { get; } - Gets a value indicating whether this GTA.Vehicle is an amphibious quad bike.
- System.Boolean IsAutomobile { get; } - Gets a value indicating whether this GTA.Vehicle is an automobile.
- System.Boolean IsAxlesStrong { set; }
- System.Boolean IsBeingBroughtToHalt { get; } - Checks if this GTA.Vehicle is being brought to a halt.
- System.Boolean IsBicycle { get; } - Gets a value indicating whether this GTA.Vehicle is a bicycle.
- System.Boolean IsBig { get; }
- System.Boolean IsBike { get; } - Gets a value indicating whether this GTA.Vehicle is a bike.
- System.Boolean IsBlimp { get; } - Gets a value indicating whether this GTA.Vehicle is a helicopter.
- System.Boolean IsBoat { get; } - Gets a value indicating whether this GTA.Vehicle is a boat.
- System.Boolean IsBurnoutForced { set; }
- System.Boolean IsConsideredDestroyed { get; set; } - Gets or sets a value indicating whether this GTA.Vehicle is considered destroyed. Will be set to true when GTA.Vehicle s are exploded or sinking for a short time. GTA.Entity.IsDead will return true and GTA.Vehicle.IsDriveable will return false if this value is set to true . Does not affect if this GTA.Vehicle will rendered scorched.
- System.Boolean IsConvertible { get; }
- System.Boolean IsDamaged { get; }
- System.Boolean IsDriveable { get; set; }
- System.Boolean IsEngineRunning { get; set; } - Gets or sets a value indicating whether this GTA.Vehicle s engine is running.
- System.Boolean IsEngineStarting { get; } - Gets or sets a value indicating whether this GTA.Vehicle s engine is currently starting.
- System.Boolean IsFrontBumperBrokenOff { get; }
- System.Boolean IsHandbrakeForcedOn { set; } - Sets a value indicating whether the Handbrake on this GTA.Vehicle is forced on.
- System.Boolean IsHelicopter { get; } - Gets a value indicating whether this GTA.Vehicle is a helicopter.
- System.Boolean IsInBurnout { get; }
- System.Boolean IsInteriorLightOn { get; set; } - Gets or sets a value indicating whether this GTA.Vehicle has its interior lights on.
- System.Boolean IsLeftHeadLightBroken { get; set; }
- System.Boolean IsLeftIndicatorLightOn { get; set; } - Gets or sets a value indicating whether this GTA.Vehicle has its left indicator light on.
- System.Boolean IsMotorcycle { get; } - Gets a value indicating whether this GTA.Vehicle is a motorcycle.
- System.Boolean IsOnAllWheels { get; }
- System.Boolean IsParachuteDeployed { get; }
- System.Boolean IsPlane { get; } - Gets a value indicating whether this GTA.Vehicle is a plane.
- System.Boolean IsQuadBike { get; } - Gets a value indicating whether this GTA.Vehicle is a quad bike.
- System.Boolean IsRadioEnabled { set; } - Turns this GTA.Vehicle s radio on or off
- System.Boolean IsRearBumperBrokenOff { get; }
- System.Boolean IsRegularAutomobile { get; } - Gets a value indicating whether this GTA.Vehicle is a regular automobile.
- System.Boolean IsRegularQuadBike { get; } - Gets a value indicating whether this GTA.Vehicle is a regular quad bike.
- System.Boolean IsRightHeadLightBroken { get; set; }
- System.Boolean IsRightIndicatorLightOn { get; set; } - Gets or sets a value indicating whether this GTA.Vehicle has its right indicator light on.
- System.Boolean IsRocketBoostActive { get; set; }
- System.Boolean IsSearchLightOn { get; set; } - Gets or sets a value indicating whether this GTA.Vehicle has its search light on.
- System.Boolean IsSirenActive { get; set; } - Gets or sets a value indicating whether this GTA.Vehicle has its siren turned on.
- System.Boolean IsSirenSilent { set; } - Sets a value indicating whether the siren on this GTA.Vehicle plays sounds.
- System.Boolean IsStolen { get; set; } - Gets or sets a value indicating whether this GTA.Vehicle was stolen.
- System.Boolean IsStopped { get; }
- System.Boolean IsStoppedAtTrafficLights { get; }
- System.Boolean IsSubmarine { get; } - Gets a value indicating whether this GTA.Vehicle is a submarine.
- System.Boolean IsSubmarineCar { get; } - Gets a value indicating whether this GTA.Vehicle is a submarine car.
- System.Boolean IsTaxiLightOn { get; set; } - Gets or sets a value indicating whether this GTA.Vehicle has its taxi light on.
- System.Boolean IsTrailer { get; } - Gets a value indicating whether this GTA.Vehicle is a trailer.
- System.Boolean IsTrain { get; } - Gets a value indicating whether this GTA.Vehicle is a train.
- System.Boolean IsWanted { get; set; } - Gets or sets a value indicating whether this GTA.Vehicle is wanted by the police.
- GTA.VehicleLandingGearState LandingGearState { get; set; }
- System.Single LightsMultiplier { get; set; }
- System.String LocalizedName { get; } - Gets the localized name of this GTA.Vehicle
- GTA.VehicleLockStatus LockStatus { get; set; }
- System.Single LodMultiplier { get; set; }
- System.Single MaxBraking { get; } - Gets the maximum brake power of this GTA.Vehicle .
- System.Single MaxTraction { get; } - Gets the maximum traction of this GTA.Vehicle .
- GTA.VehicleModCollection Mods { get; }
- System.Boolean NeedsToBeHotwired { get; set; } - Gets or sets a value indicating whether this GTA.Vehicle needs to be hotwired to start.
- System.Int32 NextGear { get; set; } - Gets or sets the next gear value of this GTA.Vehicle .
- GTA.Ped[] Occupants { get; }
- System.Single OilLevel { get; set; } - Gets or sets this GTA.Vehicle oil level. If this value is above zero, this value decreases instead of GTA.Vehicle.EngineHealth when the engine emits black smoke.
- System.Single OilVolume { get; } - Gets the oil volume of this GTA.Vehicle .
- System.Int32 PassengerCapacity { get; }
- System.Int32 PassengerCount { get; }
- GTA.Ped[] Passengers { get; }
- System.Single PetrolTankHealth { get; set; } - Gets or sets this GTA.Vehicle petrol tank health.
- System.Single PetrolTankVolume { get; } - Gets the petrol tank volume of this GTA.Vehicle .
- System.Boolean PreviouslyOwnedByPlayer { get; set; } - Gets or sets a value indicating whether this GTA.Vehicle was previously owned by a GTA.Player .
- ... truncated; query the source snapshot for more members.

### Methods
- System.Void ApplyDamage(GTA.Math.Vector3 position, System.Single damageAmount, System.Single radius)
- System.Void BringToHalt(System.Single stoppingDistance, System.Int32 timeToStopFor, System.Boolean controlVerticalVelocity = ) - Starts the task to decelerate this GTA.Vehicle until it comes to rest, possibly in an unphysically short distance.
- System.Void CargoBobMagnetGrabVehicle()
- System.Void CargoBobMagnetReleaseVehicle()
- System.Void CloseBombBay()
- GTA.Ped CreatePedOnSeat(GTA.VehicleSeat seat, GTA.Model model)
- GTA.Ped CreateRandomPedOnSeat(GTA.VehicleSeat seat)
- System.Void DetachFromTowTruck()
- System.Void DetachTowedVehicle()
- System.Void DropCargobobHook(GTA.CargobobHook hook)
- System.Boolean Exists() - Determines if this GTA.Vehicle exists. You should ensure GTA.Vehicle s still exist before manipulating them or getting some values for them on every tick, since some native functions may crash the game if invalid entity handles are passed.
- System.Void Explode() - Explode this GTA.Vehicle instantaneously.
- System.Boolean ExtraExists(System.Int32 extra)
- GTA.VehicleMissionType GetActiveMissionType() - Gets active vehicle mission type.
- static GTA.VehicleHash[] GetAllLoadedModelsAppropriateForAmbientVehicles() - Gets an array of all loaded GTA.VehicleHash s that is appropriate to spawn as ambient vehicles. All the model hashes of the elements are loaded and the GTA.Vehicle s with the model hashes can be spawned immediately.
- static System.Int32[] GetAllModelValues()
- static GTA.VehicleHash[] GetAllModels()
- static GTA.VehicleHash[] GetAllModelsOfClass(GTA.VehicleClass vehicleClass)
- static GTA.VehicleHash[] GetAllModelsOfType(GTA.VehicleType vehicleType)
- static System.String GetClassDisplayName(GTA.VehicleClass vehicleClass)
- static GTA.VehicleClass GetModelClass(GTA.Model vehicleModel)
- static System.String GetModelDisplayName(GTA.Model vehicleModel)
- static System.String GetModelMakeName(GTA.Model vehicleModel)
- static GTA.VehicleType GetModelType(GTA.Model vehicleModel)
- GTA.Ped GetPedOnSeat(GTA.VehicleSeat seat)
- System.Boolean IsCargobobHookActive()
- System.Boolean IsCargobobHookActive(GTA.CargobobHook hook)
- System.Boolean IsExtraOn(System.Int32 extra)
- System.Boolean IsSeatFree(GTA.VehicleSeat seat)
- System.Void OpenBombBay()
- System.Boolean PlaceOnGround()
- System.Void PlaceOnNextStreet()
- System.Void Repair() - Repair all damage to this GTA.Vehicle instantaneously.
- System.Void RetractCargobobHook()
- System.Void SetHeliYawPitchRollMult(System.Single mult)
- System.Void SoundHorn(System.Int32 duration) - Sounds the horn on this GTA.Vehicle .
- System.Void StartAlarm() - Starts sounding the alarm on this GTA.Vehicle .
- System.Void StartParachuting(System.Boolean allowPlayerToCancel) - Open the vehicle's parachute (if any)
- System.Void StopBringingToHalt() - Stops bringing this GTA.Vehicle to a halt.
- System.Void ToggleExtra(System.Int32 extra, System.Boolean toggle)
- System.Void TowVehicle(GTA.Vehicle vehicle, System.Boolean rear)
- System.Void Wash()

### Enum Values

## GTA.VehicleHash

- Kind: enum
- Namespace: GTA
- Domain: Native Interop

### Constructors

- None

### Properties

- None

### Methods

- None

### Enum Values
- GTA.VehicleHash.Adder
- GTA.VehicleHash.Airbus
- GTA.VehicleHash.Airtug
- GTA.VehicleHash.Akula
- GTA.VehicleHash.Akuma
- GTA.VehicleHash.Alkonost
- GTA.VehicleHash.Alpha
- GTA.VehicleHash.AlphaZ1
- GTA.VehicleHash.Ambulance
- GTA.VehicleHash.Annihilator
- GTA.VehicleHash.Annihilator2
- GTA.VehicleHash.Apc
- GTA.VehicleHash.Ardent
- GTA.VehicleHash.ArmyTanker
- GTA.VehicleHash.ArmyTrailer
- GTA.VehicleHash.ArmyTrailer2
- GTA.VehicleHash.Asbo
- GTA.VehicleHash.Asea
- GTA.VehicleHash.Asea2
- GTA.VehicleHash.Asterope
- GTA.VehicleHash.Astron
- GTA.VehicleHash.Autarch
- GTA.VehicleHash.Avarus
- GTA.VehicleHash.Avenger
- GTA.VehicleHash.Avenger2
- GTA.VehicleHash.Avisa
- GTA.VehicleHash.BF400
- GTA.VehicleHash.BJXL
- GTA.VehicleHash.BType
- GTA.VehicleHash.BType2
- GTA.VehicleHash.BType3
- GTA.VehicleHash.Bagger
- GTA.VehicleHash.BaleTrailer
- GTA.VehicleHash.Baller
- GTA.VehicleHash.Baller2
- GTA.VehicleHash.Baller3
- GTA.VehicleHash.Baller4
- GTA.VehicleHash.Baller5
- GTA.VehicleHash.Baller6
- GTA.VehicleHash.Baller7
- GTA.VehicleHash.Banshee
- GTA.VehicleHash.Banshee2
- GTA.VehicleHash.Barracks
- GTA.VehicleHash.Barracks2
- GTA.VehicleHash.Barracks3
- GTA.VehicleHash.Barrage
- GTA.VehicleHash.Bati
- GTA.VehicleHash.Bati2
- GTA.VehicleHash.Benson
- GTA.VehicleHash.Besra
- GTA.VehicleHash.BestiaGTS
- GTA.VehicleHash.BfInjection
- GTA.VehicleHash.Biff
- GTA.VehicleHash.Bifta
- GTA.VehicleHash.Bison
- GTA.VehicleHash.Bison2
- GTA.VehicleHash.Bison3
- GTA.VehicleHash.Blade
- GTA.VehicleHash.Blazer
- GTA.VehicleHash.Blazer2
- GTA.VehicleHash.Blazer3
- GTA.VehicleHash.Blazer4
- GTA.VehicleHash.Blazer5
- GTA.VehicleHash.Blimp
- GTA.VehicleHash.Blimp2
- GTA.VehicleHash.Blimp3
- GTA.VehicleHash.Blista
- GTA.VehicleHash.Blista2
- GTA.VehicleHash.Blista3
- GTA.VehicleHash.Bmx
- GTA.VehicleHash.BoatTrailer
- GTA.VehicleHash.BobcatXL
- GTA.VehicleHash.Bodhi2
- GTA.VehicleHash.Bombushka
- GTA.VehicleHash.Boor
- GTA.VehicleHash.Boxville
- GTA.VehicleHash.Boxville2
- GTA.VehicleHash.Boxville3
- GTA.VehicleHash.Boxville4
- GTA.VehicleHash.Boxville5
- GTA.VehicleHash.Brawler
- GTA.VehicleHash.Brickade
- GTA.VehicleHash.Brickade2
- GTA.VehicleHash.Brioso
- GTA.VehicleHash.Brioso2
- GTA.VehicleHash.Brioso3
- GTA.VehicleHash.Bruiser
- GTA.VehicleHash.Bruiser2
- GTA.VehicleHash.Bruiser3
- GTA.VehicleHash.Brutus
- GTA.VehicleHash.Brutus2
- GTA.VehicleHash.Brutus3
- GTA.VehicleHash.Buccaneer
- GTA.VehicleHash.Buccaneer2
- GTA.VehicleHash.Buffalo
- GTA.VehicleHash.Buffalo2
- GTA.VehicleHash.Buffalo3
- GTA.VehicleHash.Buffalo4
- GTA.VehicleHash.Bulldozer
- GTA.VehicleHash.Bullet
- GTA.VehicleHash.Burrito
- GTA.VehicleHash.Burrito2
- GTA.VehicleHash.Burrito3
- GTA.VehicleHash.Burrito4
- GTA.VehicleHash.Burrito5
- GTA.VehicleHash.Bus
- GTA.VehicleHash.Buzzard
- GTA.VehicleHash.Buzzard2
- GTA.VehicleHash.CableCar
- GTA.VehicleHash.Caddy
- GTA.VehicleHash.Caddy2
- GTA.VehicleHash.Caddy3
- GTA.VehicleHash.Calico
- GTA.VehicleHash.Camper
- GTA.VehicleHash.Caracara
- GTA.VehicleHash.Caracara2
- GTA.VehicleHash.CarbonRS
- GTA.VehicleHash.Carbonizzare
- GTA.VehicleHash.CargoPlane
- GTA.VehicleHash.CargoPlane2
- GTA.VehicleHash.Cargobob
- GTA.VehicleHash.Cargobob2
- GTA.VehicleHash.Cargobob3
- GTA.VehicleHash.Cargobob4
- GTA.VehicleHash.Casco
- GTA.VehicleHash.Cavalcade
- GTA.VehicleHash.Cavalcade2
- GTA.VehicleHash.Cerberus
- GTA.VehicleHash.Cerberus2
- GTA.VehicleHash.Cerberus3
- GTA.VehicleHash.Champion
- GTA.VehicleHash.Cheburek
- GTA.VehicleHash.Cheetah
- GTA.VehicleHash.Cheetah2
- GTA.VehicleHash.Chernobog
- GTA.VehicleHash.Chimera
- GTA.VehicleHash.Chino
- GTA.VehicleHash.Chino2
- GTA.VehicleHash.Cinquemila
- GTA.VehicleHash.Cliffhanger
- GTA.VehicleHash.Clique
- GTA.VehicleHash.Club
- GTA.VehicleHash.Coach
- GTA.VehicleHash.Cog55
- GTA.VehicleHash.Cog552
- GTA.VehicleHash.CogCabrio
- GTA.VehicleHash.Cognoscenti
- GTA.VehicleHash.Cognoscenti2
- GTA.VehicleHash.Comet2
- GTA.VehicleHash.Comet3
- GTA.VehicleHash.Comet4
- GTA.VehicleHash.Comet5
- GTA.VehicleHash.Comet6
- GTA.VehicleHash.Comet7
- GTA.VehicleHash.Conada
- GTA.VehicleHash.Contender
- GTA.VehicleHash.Coquette
- GTA.VehicleHash.Coquette2
- GTA.VehicleHash.Coquette3
- GTA.VehicleHash.Coquette4
- GTA.VehicleHash.Corsita
- GTA.VehicleHash.Cruiser
- GTA.VehicleHash.Crusader
- GTA.VehicleHash.Cuban800
- GTA.VehicleHash.Cutter
- GTA.VehicleHash.Cyclone
- GTA.VehicleHash.Cypher
- GTA.VehicleHash.DLoader
- GTA.VehicleHash.Daemon
- GTA.VehicleHash.Daemon2
- GTA.VehicleHash.DeathBike
- GTA.VehicleHash.DeathBike2
- GTA.VehicleHash.DeathBike3
- GTA.VehicleHash.Defiler
- GTA.VehicleHash.Deity
- GTA.VehicleHash.Deluxo
- GTA.VehicleHash.Deveste
- GTA.VehicleHash.Deviant
- GTA.VehicleHash.Diablous
- GTA.VehicleHash.Diablous2
- GTA.VehicleHash.Dilettante
- GTA.VehicleHash.Dilettante2
- GTA.VehicleHash.Dinghy
- GTA.VehicleHash.Dinghy2
- GTA.VehicleHash.Dinghy3
- GTA.VehicleHash.Dinghy4
- GTA.VehicleHash.Dinghy5
- GTA.VehicleHash.DockTrailer
- GTA.VehicleHash.Docktug
- GTA.VehicleHash.Dodo
- GTA.VehicleHash.Dominator
- GTA.VehicleHash.Dominator2
- GTA.VehicleHash.Dominator3
- GTA.VehicleHash.Dominator4
- GTA.VehicleHash.Dominator5
- GTA.VehicleHash.Dominator6
- GTA.VehicleHash.Dominator7
- GTA.VehicleHash.Dominator8
- GTA.VehicleHash.Double
- GTA.VehicleHash.Drafter
- GTA.VehicleHash.Draugur
- GTA.VehicleHash.Dubsta
- GTA.VehicleHash.Dubsta2
- GTA.VehicleHash.Dubsta3
- GTA.VehicleHash.Dukes
- GTA.VehicleHash.Dukes2
- GTA.VehicleHash.Dukes3
- GTA.VehicleHash.Dump
- GTA.VehicleHash.Dune
- GTA.VehicleHash.Dune2
- GTA.VehicleHash.Dune3
- GTA.VehicleHash.Dune4
- GTA.VehicleHash.Dune5
- GTA.VehicleHash.Duster
- GTA.VehicleHash.Dynasty
- GTA.VehicleHash.Elegy
- GTA.VehicleHash.Elegy2
- GTA.VehicleHash.Ellie
- GTA.VehicleHash.Emerus
- GTA.VehicleHash.Emperor
- ... truncated; query the source snapshot for more members.

## GTA.VehicleSeat

- Kind: enum
- Namespace: GTA
- Domain: Vehicles

### Constructors

- None

### Properties

- None

### Methods

- None

### Enum Values
- GTA.VehicleSeat.Any
- GTA.VehicleSeat.Driver
- GTA.VehicleSeat.ExtraSeat1
- GTA.VehicleSeat.ExtraSeat10
- GTA.VehicleSeat.ExtraSeat11
- GTA.VehicleSeat.ExtraSeat12
- GTA.VehicleSeat.ExtraSeat2
- GTA.VehicleSeat.ExtraSeat3
- GTA.VehicleSeat.ExtraSeat4
- GTA.VehicleSeat.ExtraSeat5
- GTA.VehicleSeat.ExtraSeat6
- GTA.VehicleSeat.ExtraSeat7
- GTA.VehicleSeat.ExtraSeat8
- GTA.VehicleSeat.ExtraSeat9
- GTA.VehicleSeat.LeftFront
- GTA.VehicleSeat.LeftRear
- GTA.VehicleSeat.None
- GTA.VehicleSeat.Passenger
- GTA.VehicleSeat.RightFront
- GTA.VehicleSeat.RightRear

## GTA.VehicleDoorIndex

- Kind: enum
- Namespace: GTA
- Domain: Vehicles

### Constructors

- None

### Properties

- None

### Methods

- None

### Enum Values
- GTA.VehicleDoorIndex.BackLeftDoor
- GTA.VehicleDoorIndex.BackRightDoor
- GTA.VehicleDoorIndex.FrontLeftDoor
- GTA.VehicleDoorIndex.FrontRightDoor
- GTA.VehicleDoorIndex.Hood
- GTA.VehicleDoorIndex.Trunk

## GTA.VehicleClass

- Kind: enum
- Namespace: GTA
- Domain: Vehicles

### Constructors

- None

### Properties

- None

### Methods

- None

### Enum Values
- GTA.VehicleClass.Boats
- GTA.VehicleClass.Commercial
- GTA.VehicleClass.Compacts
- GTA.VehicleClass.Coupes
- GTA.VehicleClass.Cycles
- GTA.VehicleClass.Emergency
- GTA.VehicleClass.Helicopters
- GTA.VehicleClass.Industrial
- GTA.VehicleClass.Military
- GTA.VehicleClass.Motorcycles
- GTA.VehicleClass.Muscle
- GTA.VehicleClass.OffRoad
- GTA.VehicleClass.OpenWheel
- GTA.VehicleClass.Planes
- GTA.VehicleClass.SUVs
- GTA.VehicleClass.Sedans
- GTA.VehicleClass.Service
- GTA.VehicleClass.Sports
- GTA.VehicleClass.SportsClassics
- GTA.VehicleClass.Super
- GTA.VehicleClass.Trains
- GTA.VehicleClass.Utility
- GTA.VehicleClass.Vans

---

# SHVDN Player, Ped, Weapon, and Task APIs

Player, ped, weapon, model hash, and task APIs for NPC spawning, healing, combat, and follow behavior.

Snapshot: 2026-05-19
SHVDN version: 3.6.0.0

Use this as Dify Knowledge Base context. Do not paste the full source JSON into prompts.

## GTA.Player

- Kind: class
- Namespace: GTA
- Domain: Player And Ped

### Constructors

- None

### Properties
- System.Boolean CanControlCharacter { get; set; } - Gets or sets a value indicating whether this GTA.Player can control its GTA.Ped .
- System.Boolean CanControlRagdoll { set; } - Sets a value indicating whether this GTA.Player can control ragdoll.
- System.Boolean CanLeaveParachuteSmokeTrail { set; } - Sets a value indicating whether this GTA.Player can leave a parachute smoke trail.
- System.Boolean CanStartMission { get; } - Gets a value indicating whether this GTA.Player can start a mission.
- System.Boolean CanUseCover { set; } - Sets a value indicating whether this GTA.Player can use cover.
- GTA.Ped Character { get; } - Gets the GTA.Ped this GTA.Player is controlling.
- System.Boolean DispatchsCops { set; } - Sets a value indicating whether cops will be dispatched for this GTA.Player
- System.Boolean ForcedAim { set; } - Sets a value indicating whether the player is forced to aim.
- System.Int32 Handle { get; set; }
- System.Boolean IgnoredByEveryone { set; } - Sets a value indicating whether this GTA.Player is ignored by everyone.
- System.Boolean IgnoredByPolice { set; } - Sets a value indicating whether this GTA.Player is ignored by the police.
- System.Boolean IsAiming { get; } - Gets a value indicating whether this GTA.Player is aiming.
- System.Boolean IsAlive { get; } - Gets a value indicating whether this GTA.Player is alive.
- System.Boolean IsClimbing { get; } - Gets a value indicating whether this GTA.Player is climbing.
- System.Boolean IsDead { get; } - Gets a value indicating whether this GTA.Player is dead.
- System.Boolean IsInvincible { get; set; } - Gets or sets a value indicating whether this GTA.Player is invincible.
- System.Boolean IsPlaying { get; } - Gets a value indicating whether this GTA.Player is playing.
- System.Boolean IsPressingHorn { get; } - Gets a value indicating whether this GTA.Player is pressing a horn.
- System.Boolean IsRidingTrain { get; } - Gets a value indicating whether this GTA.Player is riding a train.
- System.Boolean IsSpecialAbilityActive { get; } - Gets a value indicating whether this GTA.Player is using their special ability.
- System.Boolean IsSpecialAbilityEnabled { get; set; } - Gets or sets a value indicating whether this GTA.Player can use their special ability.
- System.Boolean IsTargetingAnything { get; } - Gets a value indicating whether this GTA.Player is targeting anything.
- GTA.Vehicle LastVehicle { get; } - Gets the last GTA.Vehicle this GTA.Player used.
- GTA.Entity LockedOnEntity { get; } - Gets the GTA.Entity this GTA.Player is locking on when they are aiming with a firearm using a controller or they are locking on unarmed or with a melee weapon.
- System.Int32 MaxArmor { get; set; } - Gets or sets the maximum amount of armor this GTA.Player can carry.
- System.Int32 Money { get; set; } - Gets or sets how much money this GTA.Player has. Only works if current player is GTA.PedHash.Michael , GTA.PedHash.Franklin or GTA.PedHash.Trevor
- System.String Name { get; } - Gets the Social Club name of this GTA.Player .
- System.UInt64 NativeValue { get; set; }
- System.Drawing.Color ParachuteSmokeTrailColor { get; set; } - Gets or sets the color of the parachute smoke trail for this GTA.Player .
- GTA.ParachuteTint PrimaryParachuteTint { get; set; } - Gets or sets the primary parachute tint for this GTA.Player .
- System.Single RemainingSprintStamina { get; } - Gets how much sprint stamina this GTA.Player currently has.
- System.Single RemainingSprintTime { get; } - Gets how long this GTA.Player can remain sprinting for.
- System.Single RemainingUnderwaterTime { get; } - Gets how long this GTA.Player can stay underwater before they start losing health.
- GTA.ParachuteTint ReserveParachuteTint { get; set; } - Gets or sets the reserve parachute tint for this GTA.Player .
- GTA.Entity TargetedEntity { get; } - Gets the GTA.Entity this GTA.Player is free aiming.
- GTA.Math.Vector3 WantedCenterPosition { get; set; } - Gets or sets the wanted center position for this GTA.Player .
- System.Int32 WantedLevel { get; set; } - Gets or sets the wanted level for this GTA.Player .

### Methods
- System.Boolean ChangeModel(GTA.Model model) - Attempts to change the GTA.Model of this GTA.Player .
- System.Void ChargeSpecialAbility(System.Int32 absoluteAmount) - Charges the special ability for this GTA.Player .
- System.Void ChargeSpecialAbility(System.Single normalizedRatio) - Charges the special ability for this GTA.Player .
- System.Void DepleteSpecialAbility() - Depletes the special ability for this GTA.Player .
- System.Void DisableFiringThisFrame() - Prevents this GTA.Player firing this frame.
- System.Boolean Equals(System.Object obj) - Determines if an System.Object refers to the same player as this GTA.Player .
- System.Int32 GetHashCode()
- System.Boolean IsTargeting(GTA.Entity entity) - Determines whether this GTA.Player is targeting the specified GTA.Entity .
- System.Void RefillSpecialAbility() - Refills the special ability for this GTA.Player .
- System.Void SetExplosiveAmmoThisFrame() - Makes this GTA.Player shoot explosive bullets this frame.
- System.Void SetExplosiveMeleeThisFrame() - Makes this GTA.Player have an explosive melee attack this frame.
- System.Void SetFireAmmoThisFrame() - Makes this GTA.Player shoot fire bullets this frame.
- System.Void SetMayNotEnterAnyVehicleThisFrame() - Blocks this GTA.Player from entering any GTA.Vehicle this frame.
- System.Void SetMayOnlyEnterThisVehicleThisFrame(GTA.Vehicle vehicle) - Only lets this GTA.Player enter a specific GTA.Vehicle this frame.
- System.Void SetRunSpeedMultThisFrame(System.Single mult) - Sets the run speed multiplier for this GTA.Player this frame.
- System.Void SetSuperJumpThisFrame() - Lets this GTA.Player jump really high this frame.
- System.Void SetSwimSpeedMultThisFrame(System.Single mult) - Sets the swim speed multiplier for this GTA.Player this frame.

### Enum Values

## GTA.Ped

- Kind: class
- Namespace: GTA
- Domain: Player And Ped

### Constructors

- None

### Properties
- System.Int32 Accuracy { get; set; } - Gets or sets how accurate this GTA.Ped s shooting ability is. The higher the value of this property is, the more likely it is that this GTA.Ped will shoot at exactly where they are aiming at.
- System.Boolean AlwaysKeepTask { set; } - Sets whether this GTA.Ped keeps their tasks when they are marked as no longer needed by GTA.Entity.MarkAsNoLongerNeeded or gets cleaned up by the mission script. Despite the property name, this property does not determine whether permanent events can interrupt the GTA.Ped 's tasks (e.g. seeing hated peds or getting shot at). If set to false , this GTA.Ped 's task will be immediately cleared and start some ambient tasks (most likely start wandering) when they are marked as no longer needed. If set to true , this GTA.Ped will keep their task until they have nothing to do (where their task stacks only contains CTaskDoNothing ). Once this GTA.Ped has nothing to do, their task will clear and they'll start some ambient tasks (one-time-only).
- System.Int32 Armor { get; set; } - Gets or sets how much armor this GTA.Ped is wearing as an System.Int32 .
- System.Single ArmorFloat { get; set; } - Gets or sets how much Armor this GTA.Ped is wearing as a System.Single .
- System.Boolean BlockPermanentEvents { set; } - Sets whether permanent events are blocked for this GTA.Ped . If set to true , this GTA.Ped will no longer react to permanent events and will only do as they're told. For example, the GTA.Ped will not flee when get shot at and they will not begin combat even if the decision maker specifies that seeing a hated ped should. However, the GTA.Ped will still respond to temporary events like walking around other peds or vehicles even if this property is set to true .
- GTA.PedBoneCollection Bones { get; } - Gets a collection of the GTA.PedBone s in this GTA.Ped .
- System.Boolean CanBeDraggedOutOfVehicle { set; }
- System.Boolean CanBeKnockedOffBike { set; }
- System.Boolean CanBeShotInVehicle { set; }
- System.Boolean CanBeTargetted { get; set; }
- System.Boolean CanFlyThroughWindscreen { get; set; }
- System.Boolean CanPlayGestures { set; }
- System.Boolean CanRagdoll { get; set; }
- System.Boolean CanSufferCriticalHits { get; set; } - Gets or Sets whether this GTA.Ped can suffer critical damage (which deals 1000 times base damages to non-player characters with default weapon configs) when bullets hit this GTA.Ped 's head bone or its child bones. If this GTA.Ped can't suffer critical damage, they will take base damage of weapons when bullets hit their head bone or its child bones, just like when bullets hit a bone other than their head bone, its child bones, or limb bones.
- System.Boolean CanSwitchWeapons { set; } - Sets if this GTA.Ped can switch between different weapons.
- System.Boolean CanWearHelmet { set; }
- System.Boolean CanWrithe { get; set; }
- GTA.WeaponHash CauseOfDeath { get; } - Gets the GTA.WeaponHash that this GTA.Ped is killed with. The return value is not necessarily a weapon hash for a human GTA.Ped s (e.g. can be the hash of WEAPON_COUGAR ).
- GTA.Vehicle CurrentVehicle { get; } - Gets the current GTA.Vehicle this GTA.Ped is using.
- System.Boolean DiesInstantlyInWater { set; }
- System.Boolean DiesOnLowHealth { set; }
- System.Single DrivingSpeed { set; }
- GTA.DrivingStyle DrivingStyle { set; }
- System.Boolean DropsEquippedWeaponOnDeath { get; set; } - Sets whether this GTA.Ped will drop the equipped weapon when they get killed. Note that GTA.Ped s will drop only their equipped weapon when they get killed.
- System.Boolean DrownsInSinkingVehicle { set; }
- System.Boolean DrownsInWater { set; }
- GTA.NaturalMotion.Euphoria Euphoria { get; } - Opens a list of GTA.NaturalMotion.Euphoria Helpers which can be applied to this GTA.Ped .
- System.Single FatalInjuryHealthThreshold { get; set; } - Gets or sets the fatal injury health threshold for this GTA.Ped . The pedestrian health will be set to 0.0 when it drops below this value.
- GTA.FiringPattern FiringPattern { get; set; } - Gets of sets the pattern this GTA.Ped uses to fire weapons.
- GTA.Gender Gender { get; } - Gets the gender of this GTA.Ped .
- System.Single HearingRange { get; set; }
- System.Single InjuryHealthThreshold { get; set; } - Gets or sets the injury health threshold for this GTA.Ped . The pedestrian is considered injured when its health drops below this value. The pedestrian dies on attacks when its health is below this value.
- System.Boolean IsAiming { get; }
- System.Boolean IsAimingFromCover { get; }
- System.Boolean IsAmbientSpeechEnabled { get; }
- System.Boolean IsAmbientSpeechPlaying { get; }
- System.Boolean IsAnySpeechPlaying { get; }
- System.Boolean IsBeingJacked { get; }
- System.Boolean IsBeingStealthKilled { get; }
- System.Boolean IsBeingStunned { get; }
- System.Boolean IsClimbing { get; }
- System.Boolean IsCuffed { get; }
- System.Boolean IsDiving { get; }
- System.Boolean IsDoingDriveBy { get; }
- System.Boolean IsDucking { get; set; }
- System.Boolean IsEnemy { set; }
- System.Boolean IsFalling { get; }
- System.Boolean IsFleeing { get; }
- System.Boolean IsGettingIntoVehicle { get; }
- System.Boolean IsGettingUp { get; }
- System.Boolean IsGoingIntoCover { get; }
- System.Boolean IsHuman { get; } - Gets a value indicating whether this GTA.Ped is human.
- System.Boolean IsIdle { get; }
- System.Boolean IsInBoat { get; }
- System.Boolean IsInCombat { get; }
- System.Boolean IsInCover { get; }
- System.Boolean IsInCoverFacingLeft { get; }
- System.Boolean IsInFlyingVehicle { get; }
- System.Boolean IsInGroup { get; } - Gets if this GTA.Ped is in a GTA.Ped.PedGroup .
- System.Boolean IsInHeli { get; }
- System.Boolean IsInMeleeCombat { get; }
- System.Boolean IsInParachuteFreeFall { get; }
- System.Boolean IsInPlane { get; }
- System.Boolean IsInPoliceVehicle { get; }
- System.Boolean IsInStealthMode { get; }
- System.Boolean IsInSub { get; }
- System.Boolean IsInTaxi { get; }
- System.Boolean IsInTrain { get; }
- System.Boolean IsInjured { get; } - Gets a value indicating whether this GTA.Ped is injured ( GTA.Entity.Health of the GTA.Ped is lower than GTA.Ped.InjuryHealthThreshold ) or does not exist. Can be called safely to check if GTA.Ped s exist and are not injured without calling GTA.Ped.Exists .
- System.Boolean IsJacking { get; }
- System.Boolean IsJumping { get; }
- System.Boolean IsJumpingOutOfVehicle { get; } - Gets a value indicating whether this GTA.Ped is jumping out of their vehicle.
- System.Boolean IsOnBike { get; }
- System.Boolean IsOnFoot { get; }
- System.Boolean IsPainAudioEnabled { set; }
- System.Boolean IsPerformingStealthKill { get; }
- System.Boolean IsPlantingBomb { get; }
- System.Boolean IsPlayer { get; }
- System.Boolean IsPriorityTargetForEnemies { set; }
- System.Boolean IsProne { get; }
- System.Boolean IsRagdoll { get; }
- System.Boolean IsReloading { get; }
- System.Boolean IsRunning { get; }
- System.Boolean IsScriptedSpeechPlaying { get; }
- System.Boolean IsShooting { get; }
- System.Boolean IsSprinting { get; }
- System.Boolean IsStopped { get; }
- System.Boolean IsSwimming { get; }
- System.Boolean IsSwimmingUnderWater { get; }
- System.Boolean IsTryingToEnterALockedVehicle { get; }
- System.Boolean IsVaulting { get; }
- System.Boolean IsWalking { get; }
- System.Boolean IsWearingHelmet { get; }
- GTA.Ped JackTarget { get; }
- GTA.Ped Jacker { get; }
- GTA.Entity Killer { get; } - Gets the GTA.Entity that killed this GTA.Ped .
- GTA.Vehicle LastVehicle { get; } - Gets the last GTA.Vehicle this GTA.Ped used.
- GTA.Math.Vector3 LastWeaponImpactPosition { get; }
- System.Single MaxDrivingSpeed { set; } - Sets the maximum driving speed this GTA.Ped can drive at.
- System.Int32 MaxHealth { get; set; } - Gets or sets the maximum health of this GTA.Ped as an System.Int32 .
- GTA.Ped MeleeTarget { get; }
- System.Int32 Money { get; set; } - Gets or sets how much money this GTA.Ped is carrying.
- System.String MovementAnimationSet { set; } - Sets the animation dictionary or set this GTA.Ped should use or null to clear it.
- System.Boolean NeverLeavesGroup { set; }
- GTA.ParachuteLandingType ParachuteLandingType { get; }
- GTA.ParachuteState ParachuteState { get; }
- GTA.PedGroup PedGroup { get; } - Gets the PedGroup this GTA.Ped is in.
- GTA.RelationshipGroup RelationshipGroup { get; set; }
- GTA.VehicleSeat SeatIndex { get; } - Gets the GTA.VehicleSeat this GTA.Ped is in.
- System.Single SeeingRange { get; set; }
- System.Int32 ShootRate { set; } - Sets the rate this GTA.Ped will shoot at.
- System.Boolean StaysInVehicleWhenJacked { set; } - Sets a value indicating whether this GTA.Ped will stay in the vehicle when the driver gets jacked.
- GTA.Style Style { get; } - Opens a list of clothing and prop configurations that this GTA.Ped can wear.
- System.Single Sweat { get; set; } - Gets or sets the how much sweat should be rendered on this GTA.Ped .
- GTA.TaskInvoker Task { get; } - Opens a list of GTA.TaskInvoker that this GTA.Ped can carry out.
- System.Int32 TaskSequenceProgress { get; } - Gets the stage of the GTA.TaskSequence this GTA.Ped is currently executing.
- System.Int32 TimeOfDeath { get; } - Gets the time when this GTA.Ped is killed. This value determines how this GTA.Ped is rendered when GTA.Game.IsThermalVisionActive is true and the GTA.Ped is dead.
- GTA.VehicleDrivingFlags VehicleDrivingFlags { set; }
- GTA.Vehicle VehicleTryingToEnter { get; } - Gets the GTA.Vehicle this GTA.Ped is trying to enter.
- GTA.VehicleWeaponHash VehicleWeapon { get; set; } - Gets the vehicle weapon this GTA.Ped is using. The vehicle weapon, returns GTA.VehicleWeaponHash.Invalid if this GTA.Ped isnt using a vehicle weapon.
- ... truncated; query the source snapshot for more members.

### Methods
- System.Void ApplyDamage(System.Int32 damageAmount)
- System.Void CancelRagdoll()
- System.Void ClearBloodDamage()
- System.Void ClearCauseOfDeathRecord() - Clears the record of the cause of death that killed this GTA.Ped with. Can be useful after resurrecting this GTA.Ped . Internally, when a GTA.Ped killed and the value for the cause of death in the instance of this GTA.Ped is not 0 , the game does not write the weapon hash value for the cause of death.
- System.Void ClearKillerRecord() - Clears the GTA.Entity record that killed this GTA.Ped . Can be useful after resurrecting this GTA.Ped . Internally, when a GTA.Ped killed and the value for the source of death in the instance of this GTA.Ped is not 0 (not null ), the game does not write the memory address of the GTA.Ped that killed this GTA.Ped .
- System.Void ClearLastWeaponDamage()
- System.Void ClearTimeOfDeathRecord() - Clears the time record when this GTA.Ped is killed. Can be useful after resurrecting this GTA.Ped . Internally, when a GTA.Ped killed and the value for the time of death in the instance of this GTA.Ped is not 0 , the game does not write the game time value for the time of death.
- System.Void ClearVisibleDamage()
- GTA.Ped Clone(System.Single heading = ) - Spawn an identical clone of this GTA.Ped .
- System.Boolean Exists() - Determines if this GTA.Ped exists. You should ensure GTA.Ped s still exist before manipulating them or getting some values for them on every tick, since some native functions may crash the game if invalid entity handles are passed.
- static GTA.PedHash[] GetAllLoadedModelsAppropriateForAmbientPeds() - Gets an array of all loaded GTA.PedHash s that is appropriate to spawn as ambient vehicles. The result array can contains animal hashes, which CREATE_RANDOM_PED excludes to spawn. All the model hashes of the elements are loaded and the GTA.Ped s with the model hashes can be spawned immediately.
- static GTA.PedHash[] GetAllModels()
- System.Boolean GetConfigFlag(System.Int32 flagID)
- GTA.Relationship GetRelationshipWithPed(GTA.Ped ped)
- System.Void GiveHelmet(System.Boolean canBeRemovedByPed, GTA.Helmet helmetType, System.Int32 textureIndex)
- System.Boolean HasBeenDamagedBy(GTA.WeaponHash weapon)
- System.Boolean HasBeenDamagedByAnyMeleeWeapon()
- System.Boolean HasBeenDamagedByAnyWeapon()
- System.Boolean IsHeadtracking(GTA.Entity entity)
- System.Boolean IsInCombatAgainst(GTA.Ped target)
- System.Boolean IsInVehicle()
- System.Boolean IsInVehicle(GTA.Vehicle vehicle)
- System.Boolean IsSittingInVehicle()
- System.Boolean IsSittingInVehicle(GTA.Vehicle vehicle)
- System.Void Kill() - Kills this GTA.Ped immediately.
- System.Void LeaveGroup()
- System.Void OpenParachute()
- System.Void PlayAmbientSpeech(System.String speechName, GTA.SpeechModifier modifier = )
- System.Void PlayAmbientSpeech(System.String speechName, System.String voiceName, GTA.SpeechModifier modifier = )
- System.Void Ragdoll(System.Int32 duration = , GTA.RagdollType ragdollType = ) - Enables this GTA.Ped 's ragdoll by starting a ragdoll task and applying to this GTA.Ped . If ragdollType is not set to GTA.RagdollType.Relax or GTA.RagdollType.ScriptControl , the ragdoll behavior for GTA.RagdollType.Balance will be used.
- System.Void RemoveHelmet(System.Boolean instantly)
- System.Void ResetConfigFlag(System.Int32 flagID)
- System.Void Resurrect() - Resurrects this GTA.Ped from death.
- System.Void SetConfigFlag(System.Int32 flagID, System.Boolean value)
- System.Void SetIntoVehicle(GTA.Vehicle vehicle, GTA.VehicleSeat seat)
- System.Void SetIsPersistentNoClearTask(System.Boolean value) - Sets a value indicating whether this GTA.Entity is persistent. Unlike GTA.Entity.IsPersistent , calling this method does not affect assigned tasks.

### Enum Values

## GTA.PedHash

- Kind: enum
- Namespace: GTA
- Domain: Native Interop

### Constructors

- None

### Properties

- None

### Methods

- None

### Enum Values
- GTA.PedHash.ARY - The first in-game model of Andre Romelle Young, known professionally as Dr. Dre.
- GTA.PedHash.ARY02 - The second in-game model of Andre Romelle Young, known professionally as Dr. Dre.
- GTA.PedHash.ARY02Cutscene - The second cutscene model of Andre Romelle Young, known professionally as Dr. Dre.
- GTA.PedHash.ARYCutscene - The first cutscene model of Andre Romelle Young, known professionally as Dr. Dre.
- GTA.PedHash.Abigail
- GTA.PedHash.AbigailCutscene
- GTA.PedHash.Abner
- GTA.PedHash.AcidLabCook
- GTA.PedHash.Acult01AMM
- GTA.PedHash.Acult01AMO
- GTA.PedHash.Acult01AMY
- GTA.PedHash.Acult02AMO
- GTA.PedHash.Acult02AMY
- GTA.PedHash.AfriAmer01AMM
- GTA.PedHash.Agatha
- GTA.PedHash.AgathaCutscene
- GTA.PedHash.Agent
- GTA.PedHash.Agent02
- GTA.PedHash.Agent14
- GTA.PedHash.Agent14Cutscene
- GTA.PedHash.AgentCutscene
- GTA.PedHash.Airhostess01SFY
- GTA.PedHash.AirworkerSMY
- GTA.PedHash.AlDiNapoli
- GTA.PedHash.AlanJeromeCutscene
- GTA.PedHash.AmandaTownley
- GTA.PedHash.AmandaTownleyCutscene
- GTA.PedHash.AmmuCountrySMM
- GTA.PedHash.Ammucity01SMY
- GTA.PedHash.Andreas
- GTA.PedHash.AndreasCutscene
- GTA.PedHash.AnitaCutscene
- GTA.PedHash.AntonCutscene
- GTA.PedHash.Antonb
- GTA.PedHash.ArmBoss01GMM
- GTA.PedHash.ArmGoon01GMM
- GTA.PedHash.ArmGoon02GMY
- GTA.PedHash.ArmLieut01GMM
- GTA.PedHash.Armoured01
- GTA.PedHash.Armoured01SMM
- GTA.PedHash.Armoured02SMM
- GTA.PedHash.Armymech01SMY
- GTA.PedHash.Ashley
- GTA.PedHash.AshleyCutscene
- GTA.PedHash.Autopsy01SMY
- GTA.PedHash.Autoshop01SFM
- GTA.PedHash.Autoshop01SMM
- GTA.PedHash.Autoshop02SMM
- GTA.PedHash.Autoshop03SMM
- GTA.PedHash.Avery
- GTA.PedHash.AveryCutscene
- GTA.PedHash.AviSchwartzman
- GTA.PedHash.AviSchwartzman02
- GTA.PedHash.AviSchwartzman02Cutscene
- GTA.PedHash.AviSchwartzmanCutscene
- GTA.PedHash.Avon
- GTA.PedHash.AvonCutscene
- GTA.PedHash.AvonGoon
- GTA.PedHash.Azteca01GMY
- GTA.PedHash.Babyd
- GTA.PedHash.BallaEast01GMY
- GTA.PedHash.BallaOrig01GMY
- GTA.PedHash.BallaSout01GMY
- GTA.PedHash.Ballas01GFY
- GTA.PedHash.BallasLeader
- GTA.PedHash.BallasLeaderCutscene
- GTA.PedHash.Ballasog
- GTA.PedHash.BallasogCutscene
- GTA.PedHash.BankRobber01AMM
- GTA.PedHash.Bankman
- GTA.PedHash.Bankman01
- GTA.PedHash.BankmanCutscene
- GTA.PedHash.Barman01SMY
- GTA.PedHash.Barry
- GTA.PedHash.BarryCutscene
- GTA.PedHash.Bartender01SFY
- GTA.PedHash.Baygor
- GTA.PedHash.Baywatch01SFY
- GTA.PedHash.Baywatch01SMY
- GTA.PedHash.Beach01AFM
- GTA.PedHash.Beach01AFY
- GTA.PedHash.Beach01AMM
- GTA.PedHash.Beach01AMO
- GTA.PedHash.Beach01AMY
- GTA.PedHash.Beach02AFY
- GTA.PedHash.Beach02AMM
- GTA.PedHash.Beach02AMO
- GTA.PedHash.Beach02AMY
- GTA.PedHash.Beach03AMY
- GTA.PedHash.Beach04AMY
- GTA.PedHash.BeachBarStaff01SFY
- GTA.PedHash.Beachvesp01AMY
- GTA.PedHash.Beachvesp02AMY
- GTA.PedHash.Benny
- GTA.PedHash.Benny02
- GTA.PedHash.BennyMech01F
- GTA.PedHash.Bestmen
- GTA.PedHash.BethFemaleYoung01
- GTA.PedHash.Beverly
- GTA.PedHash.BeverlyCutscene
- GTA.PedHash.Bevhills01AFM
- GTA.PedHash.Bevhills01AFY
- GTA.PedHash.Bevhills01AMM
- GTA.PedHash.Bevhills01AMY
- GTA.PedHash.Bevhills02AFM
- GTA.PedHash.Bevhills02AFY
- GTA.PedHash.Bevhills02AMM
- GTA.PedHash.Bevhills02AMY
- GTA.PedHash.Bevhills03AFY
- GTA.PedHash.Bevhills04AFY
- GTA.PedHash.Bevhills05AFY
- GTA.PedHash.BikeHire01
- GTA.PedHash.BikerChic
- GTA.PedHash.Billionaire
- GTA.PedHash.BillionaireCutscene
- GTA.PedHash.Blackops01SMY
- GTA.PedHash.Blackops02SMY
- GTA.PedHash.Blackops03SMY
- GTA.PedHash.BlaneMaleMiddleAge
- GTA.PedHash.Boar
- GTA.PedHash.BoatStaff01F
- GTA.PedHash.BoatStaff01M
- GTA.PedHash.Bodybuild01AFM
- GTA.PedHash.BogdanCutscene
- GTA.PedHash.BogdanGoon
- GTA.PedHash.Bouncer01SMM
- GTA.PedHash.Bouncer02SMM
- GTA.PedHash.Brad
- GTA.PedHash.BradCadaverCutscene
- GTA.PedHash.BradCutscene
- GTA.PedHash.Breakdance01AMY
- GTA.PedHash.Bride
- GTA.PedHash.BrideCutscene
- GTA.PedHash.Brucie2
- GTA.PedHash.Brucie2Cutscene
- GTA.PedHash.BryonyCutscene
- GTA.PedHash.BurgerDrug
- GTA.PedHash.BurgerDrugCutscene
- GTA.PedHash.Busboy01SMY
- GTA.PedHash.Busicas01AMY
- GTA.PedHash.Business01AFY
- GTA.PedHash.Business01AMM
- GTA.PedHash.Business01AMY
- GTA.PedHash.Business02AFM
- GTA.PedHash.Business02AFY
- GTA.PedHash.Business02AMY
- GTA.PedHash.Business03AFY
- GTA.PedHash.Business03AMY
- GTA.PedHash.Business04AFY
- GTA.PedHash.Busker01SMO
- GTA.PedHash.CCrew01SMM
- GTA.PedHash.CalebMaleYoung
- GTA.PedHash.Car3Guy1
- GTA.PedHash.Car3Guy1Cutscene
- GTA.PedHash.Car3Guy2
- GTA.PedHash.Car3Guy2Cutscene
- GTA.PedHash.CarBuyerCutscene
- GTA.PedHash.CarClub01AFY
- GTA.PedHash.CarClub01AMY
- GTA.PedHash.CarDesignFemale01
- GTA.PedHash.CarolFemaleOld
- GTA.PedHash.CartelGuards01GMM
- GTA.PedHash.CartelGuards02GMM
- GTA.PedHash.CasRN01GMM
- GTA.PedHash.Casey
- GTA.PedHash.CaseyCutscene
- GTA.PedHash.Casino01SFY
- GTA.PedHash.Casino01SMY
- GTA.PedHash.CasinoCashFemaleMiddleAge01
- GTA.PedHash.CasinoShopFemaleMiddleAge01
- GTA.PedHash.Cat
- GTA.PedHash.Celeb01
- GTA.PedHash.Celeb01Cutscene
- GTA.PedHash.Chef
- GTA.PedHash.Chef01SMY
- GTA.PedHash.Chef2
- GTA.PedHash.Chef2Cutscene
- GTA.PedHash.Chef3
- GTA.PedHash.Chef3Cutscene
- GTA.PedHash.ChefCutscene
- GTA.PedHash.ChemSec01SMM
- GTA.PedHash.ChemWork01GMM
- GTA.PedHash.ChiBoss01GMM
- GTA.PedHash.ChiCold01GMM
- GTA.PedHash.ChiGoon01GMM
- GTA.PedHash.ChiGoon02GMM
- GTA.PedHash.ChickenHawk
- GTA.PedHash.Chimp
- GTA.PedHash.Chimp2
- GTA.PedHash.ChinGoonCutscene
- GTA.PedHash.Chip
- GTA.PedHash.Chop
- GTA.PedHash.Chop2
- GTA.PedHash.CiaSec01SMM
- GTA.PedHash.Claude01
- GTA.PedHash.Clay
- GTA.PedHash.ClayCutscene
- GTA.PedHash.Claypain
- GTA.PedHash.Cletus
- GTA.PedHash.CletusCutscene
- GTA.PedHash.Clown01SMY
- GTA.PedHash.ClubBar01SFY
- GTA.PedHash.ClubBar01SMY
- GTA.PedHash.ClubBar02SFY
- GTA.PedHash.ClubCust01AFY
- GTA.PedHash.ClubCust01AMY
- GTA.PedHash.ClubCust02AFY
- GTA.PedHash.ClubCust02AMY
- GTA.PedHash.ClubCust03AFY
- GTA.PedHash.ClubCust03AMY
- GTA.PedHash.ClubCust04AFY
- GTA.PedHash.ClubCust04AMY
- GTA.PedHash.ClubhouseBar01
- GTA.PedHash.Cntrybar01SMM
- GTA.PedHash.CocaineFemale01
- GTA.PedHash.CocaineMale01
- GTA.PedHash.ComJane
- GTA.PedHash.Construct01SMY
- GTA.PedHash.Construct02SMY
- GTA.PedHash.Cop01SFY
- ... truncated; query the source snapshot for more members.

## GTA.WeaponHash

- Kind: enum
- Namespace: GTA
- Domain: Native Interop

### Constructors

- None

### Properties

- None

### Methods

- None

### Enum Values
- GTA.WeaponHash.APPistol
- GTA.WeaponHash.AcidPackage
- GTA.WeaponHash.AdvancedRifle
- GTA.WeaponHash.AssaultRifle
- GTA.WeaponHash.AssaultSMG
- GTA.WeaponHash.AssaultShotgun
- GTA.WeaponHash.AssaultrifleMk2
- GTA.WeaponHash.BZGas
- GTA.WeaponHash.Ball
- GTA.WeaponHash.Bat
- GTA.WeaponHash.BattleAxe
- GTA.WeaponHash.Bottle
- GTA.WeaponHash.BullpupRifle
- GTA.WeaponHash.BullpupRifleMk2
- GTA.WeaponHash.BullpupShotgun
- GTA.WeaponHash.CandyCane
- GTA.WeaponHash.CarbineRifle
- GTA.WeaponHash.CarbineRifleMk2
- GTA.WeaponHash.CeramicPistol
- GTA.WeaponHash.CombatMG
- GTA.WeaponHash.CombatMGMk2
- GTA.WeaponHash.CombatPDW
- GTA.WeaponHash.CombatPistol
- GTA.WeaponHash.CombatShotgun
- GTA.WeaponHash.CompactEMPLauncher
- GTA.WeaponHash.CompactGrenadeLauncher
- GTA.WeaponHash.CompactRifle
- GTA.WeaponHash.Crowbar
- GTA.WeaponHash.Dagger
- GTA.WeaponHash.DoubleActionRevolver
- GTA.WeaponHash.DoubleBarrelShotgun
- GTA.WeaponHash.FertilizerCan
- GTA.WeaponHash.FireExtinguisher
- GTA.WeaponHash.Firework
- GTA.WeaponHash.Flare
- GTA.WeaponHash.FlareGun
- GTA.WeaponHash.Flashlight
- GTA.WeaponHash.GolfClub
- GTA.WeaponHash.Grenade
- GTA.WeaponHash.GrenadeLauncher
- GTA.WeaponHash.GrenadeLauncherSmoke
- GTA.WeaponHash.Gusenberg
- GTA.WeaponHash.Hammer
- GTA.WeaponHash.Hatchet
- GTA.WeaponHash.HazardousJerryCan
- GTA.WeaponHash.HeavyPistol
- GTA.WeaponHash.HeavyRifle
- GTA.WeaponHash.HeavyShotgun
- GTA.WeaponHash.HeavySniper
- GTA.WeaponHash.HeavySniperMk2
- GTA.WeaponHash.HomingLauncher
- GTA.WeaponHash.Knife
- GTA.WeaponHash.KnuckleDuster
- GTA.WeaponHash.MG
- GTA.WeaponHash.Machete
- GTA.WeaponHash.MachinePistol
- GTA.WeaponHash.MarksmanPistol
- GTA.WeaponHash.MarksmanRifle
- GTA.WeaponHash.MarksmanRifleMk2
- GTA.WeaponHash.MetalDetector
- GTA.WeaponHash.MicroSMG
- GTA.WeaponHash.MilitaryRifle
- GTA.WeaponHash.MiniSMG
- GTA.WeaponHash.Minigun
- GTA.WeaponHash.Molotov
- GTA.WeaponHash.Musket
- GTA.WeaponHash.NavyRevolver
- GTA.WeaponHash.NightVision
- GTA.WeaponHash.Nightstick
- GTA.WeaponHash.Parachute
- GTA.WeaponHash.PericoPistol
- GTA.WeaponHash.PetrolCan
- GTA.WeaponHash.PipeBomb
- GTA.WeaponHash.Pistol
- GTA.WeaponHash.Pistol50
- GTA.WeaponHash.PistolMk2
- GTA.WeaponHash.PoolCue
- GTA.WeaponHash.PrecisionRifle
- GTA.WeaponHash.ProximityMine
- GTA.WeaponHash.PumpShotgun
- GTA.WeaponHash.PumpShotgunMk2
- GTA.WeaponHash.RPG
- GTA.WeaponHash.Railgun
- GTA.WeaponHash.RailgunXmas3
- GTA.WeaponHash.Revolver
- GTA.WeaponHash.RevolverMk2
- GTA.WeaponHash.SMG
- GTA.WeaponHash.SMGMk2
- GTA.WeaponHash.SNSPistol
- GTA.WeaponHash.SNSPistolMk2
- GTA.WeaponHash.SawnOffShotgun
- GTA.WeaponHash.ServiceCarbine
- GTA.WeaponHash.SmokeGrenade
- GTA.WeaponHash.SniperRifle
- GTA.WeaponHash.Snowball
- GTA.WeaponHash.SpecialCarbine
- GTA.WeaponHash.SpecialCarbineMk2
- GTA.WeaponHash.StickyBomb
- GTA.WeaponHash.StoneHatchet
- GTA.WeaponHash.StunGun
- GTA.WeaponHash.StunGunMultiplayer
- GTA.WeaponHash.SweeperShotgun
- GTA.WeaponHash.SwitchBlade
- GTA.WeaponHash.Unarmed
- GTA.WeaponHash.UnholyHellbringer
- GTA.WeaponHash.UpNAtomizer
- GTA.WeaponHash.VintagePistol
- GTA.WeaponHash.WM29Pistol
- GTA.WeaponHash.Widowmaker
- GTA.WeaponHash.Wrench

## GTA.WeaponCollection

- Kind: class
- Namespace: GTA
- Domain: Weapons

### Constructors

- None

### Properties
- GTA.Weapon BestWeapon { get; }
- GTA.Weapon Current { get; }
- GTA.Prop CurrentWeaponObject { get; }
- GTA.Weapon Item { get; }

### Methods
- System.Void Drop()
- GTA.Weapon Give(GTA.WeaponHash weaponHash, System.Int32 ammoCount, System.Boolean equipNow, System.Boolean isAmmoLoaded) - Gives the speficied weapon if the owner GTA.Ped does not have one, or selects the weapon if they have one and equipNow is set to true .
- GTA.Weapon Give(System.String name, System.Int32 ammoCount, System.Boolean equipNow, System.Boolean isAmmoLoaded)
- System.Boolean HasWeapon(GTA.WeaponHash weaponHash)
- System.Boolean IsWeaponValid(GTA.WeaponHash hash)
- System.Void Remove(GTA.Weapon weapon)
- System.Void Remove(GTA.WeaponHash weaponHash)
- System.Void RemoveAll()
- System.Boolean Select(GTA.Weapon weapon)
- System.Boolean Select(GTA.WeaponHash weaponHash)
- System.Boolean Select(GTA.WeaponHash weaponHash, System.Boolean equipNow)

### Enum Values

## GTA.TaskInvoker

- Kind: class
- Namespace: GTA
- Domain: Player And Ped

### Constructors

- None

### Properties

- None

### Methods
- System.Void AchieveHeading(System.Single heading, System.Int32 timeout = )
- System.Void AimAt(GTA.Entity target, System.Int32 duration)
- System.Void AimAt(GTA.Math.Vector3 target, System.Int32 duration)
- System.Void Arrest(GTA.Ped ped)
- System.Void ChaseWithGroundVehicle(GTA.Ped target)
- System.Void ChaseWithHelicopter(GTA.Ped target, GTA.Math.Vector3 offset)
- System.Void ChaseWithPlane(GTA.Ped target, GTA.Math.Vector3 offset)
- System.Void ChatTo(GTA.Ped ped)
- System.Void ClearAll()
- System.Void ClearAllImmediately()
- System.Void ClearAnimation(System.String animSet, System.String animName)
- System.Void ClearLookAt()
- System.Void ClearSecondary()
- System.Void Climb()
- System.Void ClimbLadder()
- System.Void Cower(System.Int32 duration)
- System.Void CruiseWithVehicle(GTA.Vehicle vehicle, System.Single speed, GTA.DrivingStyle style = )
- System.Void DriveTo(GTA.Vehicle vehicle, GTA.Math.Vector3 target, System.Single radius, System.Single speed, GTA.DrivingStyle style = )
- System.Void EnterAnyVehicle(GTA.VehicleSeat seat = , System.Int32 timeout = , System.Single speed = , GTA.EnterVehicleFlags flag = )
- System.Void EnterVehicle(GTA.Vehicle vehicle, GTA.VehicleSeat seat = , System.Int32 timeout = , System.Single speed = , GTA.EnterVehicleFlags flag = )
- static System.Void EveryoneLeaveVehicle(GTA.Vehicle vehicle)
- System.Void FightAgainst(GTA.Ped target)
- System.Void FightAgainst(GTA.Ped target, System.Int32 duration)
- System.Void FightAgainstHatedTargets(System.Single radius)
- System.Void FightAgainstHatedTargets(System.Single radius, System.Int32 duration)
- System.Void FleeFrom(GTA.Math.Vector3 position, System.Int32 duration = )
- System.Void FleeFrom(GTA.Ped ped, System.Int32 duration = )
- System.Void FollowPointRoute(GTA.Math.Vector3[] points)
- System.Void FollowPointRoute(System.Single movementSpeed, GTA.Math.Vector3[] points)
- System.Void FollowToOffsetFromEntity(GTA.Entity target, GTA.Math.Vector3 offset, System.Single movementSpeed, System.Int32 timeout = , System.Single distanceToFollow = , System.Boolean persistFollowing = )
- System.Void GoStraightTo(GTA.Math.Vector3 position, System.Int32 timeout = , System.Single targetHeading = , System.Single distanceToSlide = )
- System.Void GoTo(GTA.Entity target, GTA.Math.Vector3 offset = , System.Int32 timeout = )
- System.Void GoTo(GTA.Math.Vector3 position, System.Int32 timeout = )
- System.Void GuardCurrentPosition()
- System.Void HandsUp(System.Int32 duration)
- System.Void Jump()
- System.Void LandPlane(GTA.Math.Vector3 startPosition, GTA.Math.Vector3 touchdownPosition, GTA.Vehicle plane = )
- System.Void LeaveVehicle(GTA.LeaveVehicleFlags flags = )
- System.Void LeaveVehicle(GTA.Vehicle vehicle, GTA.LeaveVehicleFlags flags)
- System.Void LeaveVehicle(GTA.Vehicle vehicle, System.Boolean closeDoor)
- System.Void LookAt(GTA.Entity target, System.Int32 duration = )
- System.Void LookAt(GTA.Math.Vector3 position, System.Int32 duration = )
- System.Void ParachuteTo(GTA.Math.Vector3 position)
- System.Void ParkVehicle(GTA.Vehicle vehicle, GTA.Math.Vector3 position, System.Single heading, System.Single radius = , System.Boolean keepEngineOn = )
- System.Void PerformSequence(GTA.TaskSequence sequence)
- System.Void PlayAnimation(System.String animDict, System.String animName)
- System.Void PlayAnimation(System.String animDict, System.String animName, System.Single blendInSpeed, System.Int32 duration, GTA.AnimationFlags flags)
- System.Void PlayAnimation(System.String animDict, System.String animName, System.Single blendInSpeed, System.Single blendOutSpeed, System.Int32 duration, GTA.AnimationFlags flags, System.Single playbackRate)
- System.Void PlayAnimation(System.String animDict, System.String animName, System.Single speed, System.Int32 duration, System.Single playbackRate)
- System.Void PutAwayMobilePhone()
- System.Void PutAwayParachute()
- System.Void RappelFromHelicopter()
- System.Void ReactAndFlee(GTA.Ped ped)
- System.Void ReloadWeapon()
- System.Void RunTo(GTA.Math.Vector3 position, System.Boolean ignorePaths = , System.Int32 timeout = )
- System.Void ShootAt(GTA.Math.Vector3 position, System.Int32 duration = , GTA.FiringPattern pattern = )
- System.Void ShootAt(GTA.Ped target, System.Int32 duration = , GTA.FiringPattern pattern = )
- System.Void ShuffleToNextVehicleSeat(GTA.Vehicle vehicle = )
- System.Void Skydive()
- System.Void SlideTo(GTA.Math.Vector3 position, System.Single heading)
- System.Void StandStill(System.Int32 duration)
- System.Void StartBoatMission(GTA.Vehicle boat, GTA.Math.Vector3 target, GTA.VehicleMissionType missionType, System.Single cruiseSpeed, GTA.VehicleDrivingFlags drivingFlags, System.Single targetReachedDist, GTA.BoatMissionFlags missionFlags) - Gives the boat a mission.
- System.Void StartBoatMission(GTA.Vehicle boat, GTA.Ped target, GTA.VehicleMissionType missionType, System.Single cruiseSpeed, GTA.VehicleDrivingFlags drivingFlags, System.Single targetReachedDist, GTA.BoatMissionFlags missionFlags) - Gives the boat a mission.
- System.Void StartBoatMission(GTA.Vehicle boat, GTA.Vehicle target, GTA.VehicleMissionType missionType, System.Single cruiseSpeed, GTA.VehicleDrivingFlags drivingFlags, System.Single targetReachedDist, GTA.BoatMissionFlags missionFlags) - Gives the boat a mission.
- System.Void StartHeliMission(GTA.Vehicle heli, GTA.Math.Vector3 target, GTA.VehicleMissionType missionType, System.Single cruiseSpeed, System.Single targetReachedDist, System.Int32 flightHeight, System.Int32 minHeightAboveTerrain, System.Single heliOrientation = , System.Single slowDownDistance = , GTA.HeliMissionFlags missionFlags = ) - Gives the helicopter a mission.
- System.Void StartHeliMission(GTA.Vehicle heli, GTA.Ped target, GTA.VehicleMissionType missionType, System.Single cruiseSpeed, System.Single targetReachedDist, System.Int32 flightHeight, System.Int32 minHeightAboveTerrain, System.Single heliOrientation = , System.Single slowDownDistance = , GTA.HeliMissionFlags missionFlags = ) - Gives the helicopter a mission.
- System.Void StartHeliMission(GTA.Vehicle heli, GTA.Vehicle target, GTA.VehicleMissionType missionType, System.Single cruiseSpeed, System.Single targetReachedDist, System.Int32 flightHeight, System.Int32 minHeightAboveTerrain, System.Single heliOrientation = , System.Single slowDownDistance = , GTA.HeliMissionFlags missionFlags = ) - Gives the helicopter a mission.
- System.Void StartPlaneMission(GTA.Vehicle plane, GTA.Math.Vector3 target, GTA.VehicleMissionType missionType, System.Single cruiseSpeed, System.Single targetReachedDist, System.Int32 flightHeight, System.Int32 minHeightAboveTerrain, System.Single planeOrientation = , System.Boolean precise = ) - Gives the plane a mission.
- System.Void StartPlaneMission(GTA.Vehicle plane, GTA.Ped target, GTA.VehicleMissionType missionType, System.Single cruiseSpeed, System.Single targetReachedDist, System.Int32 flightHeight, System.Int32 minHeightAboveTerrain, System.Single planeOrientation = , System.Boolean precise = ) - Gives the plane a mission.
- System.Void StartPlaneMission(GTA.Vehicle plane, GTA.Vehicle target, GTA.VehicleMissionType missionType, System.Single cruiseSpeed, System.Single targetReachedDist, System.Int32 flightHeight, System.Int32 minHeightAboveTerrain, System.Single planeOrientation = , System.Boolean precise = ) - Gives the plane a mission.
- System.Void StartScenario(System.String name, GTA.Math.Vector3 position, System.Single heading)
- System.Void StartScenario(System.String name, System.Single heading)
- System.Void StartVehicleMission(GTA.Vehicle vehicle, GTA.Math.Vector3 target, GTA.VehicleMissionType missionType, System.Single cruiseSpeed, GTA.VehicleDrivingFlags drivingFlags, System.Single targetReachedDist, System.Single straightLineDist, System.Boolean driveAgainstTraffic = ) - Tells the GTA.Ped to target a coord with a GTA.Vehicle .
- System.Void StartVehicleMission(GTA.Vehicle vehicle, GTA.Ped target, GTA.VehicleMissionType missionType, System.Single cruiseSpeed, GTA.VehicleDrivingFlags drivingFlags, System.Single targetReachedDist, System.Single straightLineDist, System.Boolean driveAgainstTraffic = ) - Tells the GTA.Ped to target another ped with a vehicle.
- System.Void StartVehicleMission(GTA.Vehicle vehicle, GTA.Vehicle target, GTA.VehicleMissionType missionType, System.Single cruiseSpeed, GTA.VehicleDrivingFlags drivingFlags, System.Single targetReachedDist, System.Single straightLineDist, System.Boolean driveAgainstTraffic = ) - Tells the GTA.Ped to perform a task when in a GTA.Vehicle against another GTA.Vehicle .
- System.Void SwapWeapon()
- System.Void TurnTo(GTA.Entity target, System.Int32 duration = )
- System.Void TurnTo(GTA.Math.Vector3 position, System.Int32 duration = )
- static System.Void UpdateParachuteTarget(GTA.Ped ped, GTA.Math.Vector3 position)
- System.Void UseMobilePhone()
- System.Void UseMobilePhone(System.Int32 duration)
- System.Void UseParachute()
- System.Void VehicleChase(GTA.Ped target)
- System.Void VehicleShootAtPed(GTA.Ped target)
- System.Void Wait(System.Int32 duration)
- System.Void WanderAround()
- System.Void WanderAround(GTA.Math.Vector3 position, System.Single radius)
- System.Void WarpIntoVehicle(GTA.Vehicle vehicle, GTA.VehicleSeat seat)
- System.Void WarpOutOfVehicle(GTA.Vehicle vehicle)

### Enum Values

## GTA.RelationshipGroup

- Kind: struct
- Namespace: GTA
- Domain: Player And Ped

### Constructors
- GTA.RelationshipGroup(System.Int32 hash)
- GTA.RelationshipGroup(System.UInt32 hash)

### Properties
- System.Int32 Hash { get; set; } - Gets the hash for this GTA.RelationshipGroup .
- System.UInt64 NativeValue { get; set; } - Gets the native representation of this GTA.RelationshipGroup .

### Methods
- System.Void ClearRelationshipBetweenGroups(GTA.RelationshipGroup targetGroup, GTA.Relationship relationship, System.Boolean bidirectionally = )
- System.Boolean Equals(GTA.RelationshipGroup group)
- System.Boolean Equals(System.Object obj)
- System.Int32 GetHashCode()
- GTA.Relationship GetRelationshipBetweenGroups(GTA.RelationshipGroup targetGroup)
- System.Void Remove()
- System.Void SetRelationshipBetweenGroups(GTA.RelationshipGroup targetGroup, GTA.Relationship relationship, System.Boolean bidirectionally = )
- System.String ToString()

### Enum Values

## GTA.Relationship

- Kind: enum
- Namespace: GTA
- Domain: Player And Ped

### Constructors

- None

### Properties

- None

### Methods

- None

### Enum Values
- GTA.Relationship.Companion - The correct relationship name for this enum would be Respect .
- GTA.Relationship.Dead
- GTA.Relationship.Dislike - The correct relationship name for this enum would be Wanted . Will be used for cops towards the player relationship group when the player is wanted.
- GTA.Relationship.Hate
- GTA.Relationship.Like - The correct relationship name for this enum would be Ignore .
- GTA.Relationship.Neutral - The correct relationship name for this enum would be Dislike .
- GTA.Relationship.Pedestrians - The correct relationship name for this enum would be None .
- GTA.Relationship.Respect - The correct relationship name for this enum would be Like .