# SHVDN v3 API Index

Snapshot: 2026-05-19
SHVDN version: 3.6.0.0

Use this file for first-pass discovery only. Before writing any SHVDN API call or enum literal that is not already present in the LiveStudio template, run `.\scripts\query-shvdn-api.ps1 ...` to verify the exact signature, overload, inherited declaring type, and enum value.

## Domain Index

- UI And HUD: 24 types
- Math And Geometry: 4 types
- Native Interop: 12 types
- Vehicles: 44 types
- Weapons: 11 types
- Player And Ped: 16 types
- World And Environment: 14 types
- Camera And Visual Effects: 5 types
- Audio: 2 types
- Entities And Objects: 16 types
- Scripting And Input: 10 types
- Other: 117 types

## UI And HUD

- `GTA.Blip` (class; 1 ctor, 32 props, 7 methods)
- `GTA.BlipCategoryType` (enum; 5 values)
- `GTA.BlipColor` (enum; 84 values)
- `GTA.BlipDisplayType` (enum; 6 values)
- `GTA.BlipSprite` (enum; 689 values)
- `GTA.Scaleform` (class; 1 ctor, 4 props, 7 methods) - A class which handles rendering of Scaleform elements.
- `GTA.ScaleformArgumentTXD` (class; 1 ctor)
- `GTA.UI.Alignment` (enum; 3 values)
- `GTA.UI.ContainerElement` (class; 4 ctor, 6 props, 4 methods)
- `GTA.UI.CursorSprite` (enum; 12 values) - An enumeration of all possible cursor sprites.
- `GTA.UI.CustomSprite` (class; 4 ctor, 6 props, 4 methods) - A sprite element using a custom image texture.
- `GTA.UI.Font` (enum; 9 values) - An enumeration of fonts the game supports.
- `GTA.UI.Hud` (static class; 4 props, 4 methods) - Methods to manipulate the HUD (heads-up-display) of the game.
- `GTA.UI.HudComponent` (enum; 51 values) - An enumeration of all possible component of the HUD.
- `GTA.UI.IElement` (interface; 4 props, 4 methods)
- `GTA.UI.ISpriteElement` (interface; 2 props)
- `GTA.UI.LoadingPrompt` (static class; 1 props, 2 methods) - Methods to manage the display of a loading spinner prompt.
- `GTA.UI.LoadingSpinnerType` (enum; 5 values) - An enumeration of possible loading spinner styles.
- `GTA.UI.Notification` (static class; 3 methods) - Methods to manage the display of notifications above the minimap.
- `GTA.UI.NotificationIcon` (enum; 167 values)
- `GTA.UI.Screen` (static class; 2 fields, 9 props, 12 methods) - Methods to handle UI actions that affect the whole screen.
- `GTA.UI.ScreenEffect` (enum; 81 values) - An enumeration of possible screen effects.
- `GTA.UI.Sprite` (class; 4 ctor, 6 props, 5 methods) - A sprite element using a built-in texture.
- `GTA.UI.TextElement` (class; 6 ctor, 13 props, 6 methods)

## Math And Geometry

- `GTA.Math.Matrix` (struct; 1 ctor, 16 fields, 6 props, 30 methods) - Defines a 4x4 matrix.
- `GTA.Math.Quaternion` (struct; 2 ctor, 4 fields, 5 props, 39 methods)
- `GTA.Math.Vector2` (struct; 1 ctor, 2 fields, 9 props, 29 methods)
- `GTA.Math.Vector3` (struct; 1 ctor, 3 fields, 18 props, 40 methods)

## Native Interop

- `GTA.MaterialHash` (enum; 214 values)
- `GTA.Native.Function` (static class; 36 methods) - A static class which handles script function execution.
- `GTA.Native.GlobalVariable` (struct; 1 props, 10 methods) - A value class which handles access to global script variables.
- `GTA.Native.Hash` (enum; 6439 values)
- `GTA.Native.INativeValue` (interface; 1 props)
- `GTA.Native.InputArgument` (class; 3 ctor, 1 methods) - An input argument passed to a script function.
- `GTA.Native.OutputArgument` (class; 2 ctor, 2 methods) - An output argument passed to a script function.
- `GTA.PedHash` (enum; 1019 values)
- `GTA.VehicleHash` (enum; 793 values)
- `GTA.VehicleWeaponHash` (enum; 20 values)
- `GTA.WeaponComponentHash` (enum; 425 values)
- `GTA.WeaponHash` (enum; 110 values)

## Vehicles

- `GTA.DrivingStyle` (enum; 6 values)
- `GTA.EnterVehicleFlags` (enum; 16 values) - Set of flags to define the behaviour of the enter and exit vehicle tasks. Shares the same flags with GTA.LeaveVehicleFlags .
- `GTA.HandlingData` (class; 51 props, 4 methods) - This class has most regular handling data. Currently compatible with 1.0.2060.0 or later. Note that this class gets data from or sets data to the CHandlingData instance as is, and thus not all the handling values don't match the equivalent values in the handling.meta file. The game multiplies or divides some values after reading values from the handling.meta file.
- `GTA.LeaveVehicleFlags` (enum; 13 values) - Set of flags to define the behaviour of the enter and exit vehicle tasks. Shares the same flags with GTA.EnterVehicleFlags .
- `GTA.LicensePlateStyle` (enum; 6 values)
- `GTA.LicensePlateType` (enum; 4 values)
- `GTA.Model` (struct; 5 ctor, 44 props, 9 methods)
- `GTA.NaturalMotion.AdaptiveMode` (enum; 4 values)
- `GTA.NaturalMotion.MirrorMode` (enum; 3 values)
- `GTA.NaturalMotion.SetWeaponModeHelper` (class; 1 ctor, 1 props) - Use this message to set the character's weapon mode. This is an alternativeto the setWeaponMode public function.
- `GTA.NaturalMotion.TorqueFilterMode` (enum; 3 values)
- `GTA.NaturalMotion.TorqueMode` (enum; 3 values)
- `GTA.NaturalMotion.TorqueSpinMode` (enum; 3 values)
- `GTA.NaturalMotion.WeaponMode` (enum; 7 values)
- `GTA.PlayerTargetingMode` (enum; 4 values)
- `GTA.RadioStation` (enum; 28 values)
- `GTA.SpeechModifier` (enum; 37 values)
- `GTA.Vehicle` (class; 134 props, 42 methods)
- `GTA.VehicleClass` (enum; 23 values)
- `GTA.VehicleColor` (enum; 161 values)
- `GTA.VehicleDoor` (class; 7 props, 3 methods)
- `GTA.VehicleDoorCollection` (class; 1 props, 3 methods)
- `GTA.VehicleDoorIndex` (enum; 6 values)
- `GTA.VehicleDrivingFlags` (enum; 44 values)
- `GTA.VehicleLandingGearState` (enum; 5 values)
- `GTA.VehicleLockStatus` (enum; 14 values)
- `GTA.VehicleMissionType` (enum; 19 values)
- `GTA.VehicleMod` (class; 7 props, 1 methods)
- `GTA.VehicleModCollection` (class; 27 props, 10 methods)
- `GTA.VehicleModType` (enum; 42 values)
- `GTA.VehicleNeonLight` (enum; 4 values)
- `GTA.VehicleRoofState` (enum; 4 values)
- `GTA.VehicleSeat` (enum; 20 values)
- `GTA.VehicleToggleMod` (class; 4 props, 1 methods)
- `GTA.VehicleToggleModType` (enum; 3 values)
- `GTA.VehicleType` (enum; 16 values)
- `GTA.VehicleWheel` (class; 16 props, 4 methods)
- `GTA.VehicleWheelBoneId` (enum; 11 values)
- `GTA.VehicleWheelCollection` (class; 4 props, 3 methods)
- `GTA.VehicleWheelType` (enum; 10 values)
- `GTA.VehicleWindow` (class; 3 props, 5 methods)
- `GTA.VehicleWindowCollection` (class; 2 props, 1 methods)
- `GTA.VehicleWindowIndex` (enum; 8 values)
- `GTA.VehicleWindowTint` (enum; 8 values)

## Weapons

- `GTA.NaturalMotion.FireWeaponHelper` (class; 1 ctor, 6 props) - One shot message apply a force to the hand as we fire the gun that should be in this hand.
- `GTA.NaturalMotion.RegisterWeaponHelper` (class; 1 ctor, 9 props) - Use this message to register weapon. This is an alternativeto the registerWeapon public function.
- `GTA.Projectile` (class; 3 props, 2 methods)
- `GTA.Weapon` (class; 16 props, 3 methods)
- `GTA.WeaponAsset` (struct; 3 ctor, 7 props, 7 methods)
- `GTA.WeaponAttachmentPoint` (enum; 18 values)
- `GTA.WeaponCollection` (class; 4 props, 11 methods)
- `GTA.WeaponComponent` (class; 5 props, 1 methods)
- `GTA.WeaponComponentCollection` (class; 8 props, 9 methods)
- `GTA.WeaponGroup` (enum; 16 values)
- `GTA.WeaponTint` (enum; 8 values)

## Player And Ped

- `GTA.Gender` (enum; 2 values)
- `GTA.IPedVariation` (interface; 8 props, 2 methods)
- `GTA.NaturalMotion.PedalLegsHelper` (class; 1 ctor, 20 props)
- `GTA.Ped` (class; 131 props, 36 methods)
- `GTA.PedBone` (class; 1 props)
- `GTA.PedBoneCollection` (class; 5 props, 2 methods)
- `GTA.PedComponent` (class; 9 props, 3 methods)
- `GTA.PedComponentType` (enum; 12 values)
- `GTA.PedGroup` (class; 2 ctor, 4 props, 12 methods)
- `GTA.PedProp` (class; 9 props, 3 methods)
- `GTA.PedPropType` (enum; 10 values)
- `GTA.Player` (class; 37 props, 17 methods)
- `GTA.Relationship` (enum; 8 values)
- `GTA.RelationshipGroup` (struct; 2 ctor, 2 props, 8 methods)
- `GTA.TaskInvoker` (class; 89 methods)
- `GTA.TaskSequence` (class; 2 ctor, 4 props, 3 methods)

## World And Environment

- `GTA.Checkpoint` (class; 1 ctor, 11 props, 4 methods)
- `GTA.CheckpointCustomIcon` (struct; 1 ctor, 3 props, 2 methods)
- `GTA.CheckpointCustomIconStyle` (enum; 13 values)
- `GTA.CheckpointIcon` (enum; 47 values)
- `GTA.ExplosionType` (enum; 79 values)
- `GTA.MarkerType` (enum; 29 values)
- `GTA.RaycastResult` (struct; 1 ctor, 6 props)
- `GTA.ShapeTest` (static class; 9 methods)
- `GTA.ShapeTestHandle` (struct; 3 props, 7 methods) - Represents a shape test handle. You need to call GetResult or GetResultIncludingMaterial every frame until one of the methods returns GTA.ShapeTestStatus.Ready .
- `GTA.ShapeTestOptions` (enum; 4 values)
- `GTA.ShapeTestResult` (struct; 3 props, 1 methods) - Represents a shape test result.
- `GTA.ShapeTestStatus` (enum; 3 values)
- `GTA.Weather` (enum; 16 values)
- `GTA.World` (static class; 31 props, 106 methods)

## Camera And Visual Effects

- `GTA.Camera` (class; 1 ctor, 19 props, 16 methods)
- `GTA.CameraShake` (enum; 11 values)
- `GTA.GameplayCamera` (static class; 18 props, 6 methods)
- `GTA.ParticleEffect` (class; 10 props, 6 methods)
- `GTA.ParticleEffectAsset` (struct; 1 ctor, 3 props, 7 methods)

## Audio

- `GTA.Audio` (static class; 12 methods) - Methods to manipulate audio.
- `GTA.AudioFlags` (enum; 35 values) - An enumeration of all possible audio flags.

## Entities And Objects

- `GTA.Bone` (enum; 358 values)
- `GTA.Entity` (class; 67 props, 32 methods)
- `GTA.EntityBone` (class; 15 props, 6 methods)
- `GTA.EntityBoneCollection` (class; 4 props, 3 methods)
- `GTA.EntityDamageRecord` (struct; 4 props, 5 methods)
- `GTA.EntityDamageRecordCollection` (class; 2 methods)
- `GTA.EntityPopulationType` (enum; 11 values)
- `GTA.EntityType` (enum; 4 values)
- `GTA.NaturalMotion.DefineAttachedObjectHelper` (class; 1 ctor, 3 props)
- `GTA.NaturalMotion.ForceLeanTowardsObjectHelper` (class; 1 ctor, 5 props)
- `GTA.NaturalMotion.HipsLeanTowardsObjectHelper` (class; 1 ctor, 4 props)
- `GTA.NaturalMotion.LeanTowardsObjectHelper` (class; 1 ctor, 4 props)
- `GTA.Pickup` (class; 1 ctor, 2 props, 5 methods)
- `GTA.PickupType` (enum; 71 values)
- `GTA.PoolObject` (class; 2 props, 2 methods) - An object that resides in one of the available object pools.
- `GTA.Prop` (class; 1 methods)

## Scripting And Input

- `GTA.Button` (enum; 12 values)
- `GTA.Control` (enum; 363 values)
- `GTA.Game` (static class; 24 props, 29 methods)
- `GTA.GameVersion` (enum; 82 values)
- `GTA.GameVersionNotSupportedException` (class; 1 props, 1 methods)
- `GTA.InputMethod` (enum; 2 values)
- `GTA.RequireScript` (class; 1 ctor)
- `GTA.Script` (class; 1 ctor, 7 props, 8 methods, 4 events) - A base class for all user scripts to inherit. Only scripts that inherit directly from this class and have a default (parameterless) public constructor will be detected and started.
- `GTA.ScriptAttributes` (class; 1 ctor, 4 fields)
- `GTA.ScriptSettings` (class; 5 methods)

## Other

- `GTA.AnimatedBuilding` (class; 7 props, 4 methods)
- `GTA.AnimationFlags` (enum; 34 values)
- `GTA.BoatMissionFlags` (enum; 13 values)
- `GTA.Building` (class; 7 props, 4 methods)
- `GTA.CargobobHook` (enum; 2 values)
- `GTA.DrawBoxFlags` (enum; 3 values)
- `GTA.EntityBoneCollection.Enumerator` (class; 1 ctor, 1 props, 2 methods)
- `GTA.EulerRotationOrder` (enum; 6 values) - Enums for the order in which to apply rotations in local space, just like how Rockstar Games define EULER_ROT_ORDER .
- `GTA.FiringPattern` (enum; 22 values)
- `GTA.ForceType` (enum; 6 values)
- `GTA.Formation` (enum; 9 values)
- `GTA.HeliMissionFlags` (enum; 17 values)
- `GTA.Helmet` (enum; 3 values)
- `GTA.IDeletable` (interface; 1 methods) - An object that can be deleted from the world.
- `GTA.IExistable` (interface; 1 methods) - An object that can exist in the world.
- `GTA.ISpatial` (interface; 2 props) - An object with position and rotation information.
- `GTA.InteriorInstance` (class; 8 props, 4 methods)
- `GTA.InteriorProxy` (class; 8 props, 11 methods)
- `GTA.IntersectFlags` (enum; 14 values)
- `GTA.InvertAxisFlags` (enum; 4 values)
- `GTA.Language` (enum; 13 values)
- `GTA.MeasurementSystem` (enum; 2 values)
- `GTA.NaturalMotion.ActivePoseHelper` (class; 1 ctor, 3 props)
- `GTA.NaturalMotion.AnimPoseHelper` (class; 1 ctor, 35 props)
- `GTA.NaturalMotion.AnimSource` (enum; 3 values)
- `GTA.NaturalMotion.ApplyBulletImpulseHelper` (class; 1 ctor, 6 props)
- `GTA.NaturalMotion.ApplyImpulseHelper` (class; 1 ctor, 7 props)
- `GTA.NaturalMotion.ArmDirection` (enum; 3 values)
- `GTA.NaturalMotion.ArmsWindmillAdaptiveHelper` (class; 1 ctor, 17 props)
- `GTA.NaturalMotion.ArmsWindmillHelper` (class; 1 ctor, 29 props)
- `GTA.NaturalMotion.BalancerCollisionsReactionHelper` (class; 1 ctor, 46 props)
- `GTA.NaturalMotion.BodyBalanceHelper` (class; 1 ctor, 52 props)
- `GTA.NaturalMotion.BodyFoetalHelper` (class; 1 ctor, 6 props)
- `GTA.NaturalMotion.BodyRelaxHelper` (class; 1 ctor, 5 props) - Set the amount of relaxation across the whole body; Used to collapse the character into a rag-doll-like state.
- `GTA.NaturalMotion.BodyRollUpHelper` (class; 1 ctor, 12 props)
- `GTA.NaturalMotion.BodyWritheHelper` (class; 1 ctor, 28 props)
- `GTA.NaturalMotion.BraceForImpactHelper` (class; 1 ctor, 47 props)
- `GTA.NaturalMotion.BuoyancyHelper` (class; 1 ctor, 8 props) - Simple buoyancy model. No character movement just fluid forces/torques added to parts.
- `GTA.NaturalMotion.CarriedHelper` (class; 1 ctor) - Carried.
- `GTA.NaturalMotion.CatchFallHelper` (class; 1 ctor, 9 props)
- `GTA.NaturalMotion.ConfigureBalanceHelper` (class; 1 ctor, 64 props) - This single message allows you to configure various parameters used on any behavior that uses the dynamic balance.
- `GTA.NaturalMotion.ConfigureBalanceResetHelper` (class; 1 ctor) - Reset the values configurable by the Configure Balance message to their defaults.
- `GTA.NaturalMotion.ConfigureBulletsExtraHelper` (class; 1 ctor, 55 props)
- `GTA.NaturalMotion.ConfigureBulletsHelper` (class; 1 ctor, 59 props)
- `GTA.NaturalMotion.ConfigureConstraintsHelper` (class; 1 ctor, 7 props) - One shot to give state of constraints on character and response to constraints.
- `GTA.NaturalMotion.ConfigureLimitsHelper` (class; 1 ctor, 10 props) - Enable/disable/edit character limits in real time. This adjusts limits in RAGE-native space and will *not* reorient the joint.
- `GTA.NaturalMotion.ConfigureSelfAvoidanceHelper` (class; 1 ctor, 9 props) - This single message allows to configure self avoidance for the character.BBDD Self avoidance tech.
- `GTA.NaturalMotion.ConfigureShotInjuredArmHelper` (class; 1 ctor, 11 props) - This single message allows you to configure the injured arm reaction during shot.
- `GTA.NaturalMotion.ConfigureShotInjuredLegHelper` (class; 1 ctor, 10 props) - This single message allows you to configure the injured leg reaction during shot.
- `GTA.NaturalMotion.ConfigureSoftLimitHelper` (class; 1 ctor, 6 props)
- `GTA.NaturalMotion.CustomHelper` (class; 10 methods) - A helper class for building a GTA.NaturalMotion.Message and sending it to a given GTA.Ped .
- `GTA.NaturalMotion.DangleHelper` (class; 1 ctor, 2 props) - Dangle.
- `GTA.NaturalMotion.ElectrocuteHelper` (class; 1 ctor, 21 props)
- `GTA.NaturalMotion.Euphoria` (class; 83 props)
- `GTA.NaturalMotion.FallOverWallHelper` (class; 1 ctor, 28 props)
- `GTA.NaturalMotion.FallType` (enum; 4 values)
- `GTA.NaturalMotion.ForceLeanInDirectionHelper` (class; 1 ctor, 3 props)
- `GTA.NaturalMotion.ForceLeanRandomHelper` (class; 1 ctor, 5 props)
- `GTA.NaturalMotion.ForceLeanToPositionHelper` (class; 1 ctor, 3 props)
- `GTA.NaturalMotion.ForceToBodyPartHelper` (class; 1 ctor, 3 props) - Apply an impulse to a named body part.
- `GTA.NaturalMotion.GrabHelper` (class; 1 ctor, 40 props)
- `GTA.NaturalMotion.Hand` (enum; 2 values)
- `GTA.NaturalMotion.HeadLookHelper` (class; 1 ctor, 10 props)
- `GTA.NaturalMotion.HighFallHelper` (class; 1 ctor, 37 props)
- `GTA.NaturalMotion.HipsLeanInDirectionHelper` (class; 1 ctor, 2 props)
- `GTA.NaturalMotion.HipsLeanRandomHelper` (class; 1 ctor, 4 props)
- `GTA.NaturalMotion.HipsLeanToPositionHelper` (class; 1 ctor, 2 props)
- `GTA.NaturalMotion.IncomingTransformsHelper` (class; 1 ctor)
- `GTA.NaturalMotion.InjuredOnGroundHelper` (class; 1 ctor, 11 props) - InjuredOnGround.
- `GTA.NaturalMotion.LeanInDirectionHelper` (class; 1 ctor, 2 props)
- `GTA.NaturalMotion.LeanRandomHelper` (class; 1 ctor, 4 props)
- `GTA.NaturalMotion.LeanToPositionHelper` (class; 1 ctor, 2 props)
- `GTA.NaturalMotion.Message` (class; 1 ctor, 12 methods) - A base class for manually building a GTA.NaturalMotion.Message .
- `GTA.NaturalMotion.OnFireHelper` (class; 1 ctor, 15 props)
- `GTA.NaturalMotion.PointArmHelper` (class; 1 ctor, 18 props) - BEHAVIOURS REFERENCED: AnimPose - allows animPose to override body parts: Arms (useLeftArm, useRightArm).
- `GTA.NaturalMotion.PointGunExtraHelper` (class; 1 ctor, 9 props) - Seldom set parameters for pointGun - just to keep number of parameters in any message less than or equal to 64.
- `GTA.NaturalMotion.PointGunHelper` (class; 1 ctor, 67 props)
- `GTA.NaturalMotion.RbTwistAxis` (enum; 2 values)
- `GTA.NaturalMotion.RollDownStairsHelper` (class; 1 ctor, 34 props)
- `GTA.NaturalMotion.SetCharacterCollisionsHelper` (class; 1 ctor, 9 props) - SetCharacterCollisions:.
- `GTA.NaturalMotion.SetCharacterDampingHelper` (class; 1 ctor, 6 props) - Damp out cartwheeling and somersaulting above a certain threshold.
- `GTA.NaturalMotion.SetCharacterHealthHelper` (class; 1 ctor, 1 props) - Sets character's health on the dead-to-alive scale: [0..1].
- `GTA.NaturalMotion.SetCharacterStrengthHelper` (class; 1 ctor, 1 props) - Sets character's strength on the dead-granny-to-healthy-terminator scale: [0..1].
- `GTA.NaturalMotion.SetCharacterUnderwaterHelper` (class; 1 ctor, 5 props) - Sets viscosity applied to damping limbs.
- `GTA.NaturalMotion.SetFallingReactionHelper` (class; 1 ctor, 21 props) - Sets the type of reaction if catchFall is called.
- `GTA.NaturalMotion.SetFrictionScaleHelper` (class; 1 ctor, 4 props) - SetFrictionScale:.
- `GTA.NaturalMotion.SetMuscleStiffnessHelper` (class; 1 ctor, 2 props) - Use this message to manually set the muscle stiffness values -before using Active Pose to drive to an animated pose, for example.
- `GTA.NaturalMotion.SetStiffnessHelper` (class; 1 ctor, 3 props) - Use this message to manually set the body stiffness values -before using Active Pose to drive to an animated pose, for example.
- `GTA.NaturalMotion.ShotConfigureArmsHelper` (class; 1 ctor, 37 props) - Configure the arm reactions in shot.
- `GTA.NaturalMotion.ShotFallToKneesHelper` (class; 1 ctor, 25 props) - Configure the fall to knees shot.
- `GTA.NaturalMotion.ShotFromBehindHelper` (class; 1 ctor, 11 props) - Configure the shot from behind reaction.
- `GTA.NaturalMotion.ShotHeadLookHelper` (class; 1 ctor, 6 props)
- `GTA.NaturalMotion.ShotHelper` (class; 1 ctor, 57 props)
- `GTA.NaturalMotion.ShotInGutsHelper` (class; 1 ctor, 8 props) - Configure the shot in guts reaction.
- `GTA.NaturalMotion.ShotNewBulletHelper` (class; 1 ctor, 5 props) - Send new wound information to the shot. Can cause shot to restart it's performance in part or in whole.
- `GTA.NaturalMotion.ShotRelaxHelper` (class; 1 ctor, 2 props)
- `GTA.NaturalMotion.ShotShockSpinHelper` (class; 1 ctor, 14 props) - Configure the shockSpin effect in shot. Spin/Lift the character using cheat torques/forces.
- `GTA.NaturalMotion.ShotSnapHelper` (class; 1 ctor, 20 props)
- `GTA.NaturalMotion.SmartFallHelper` (class; 1 ctor, 56 props) - Clone of High Fall with a wider range of operating conditions.
- `GTA.NaturalMotion.StaggerFallHelper` (class; 1 ctor, 42 props)
- `GTA.NaturalMotion.StayUprightHelper` (class; 1 ctor, 28 props)
- `GTA.NaturalMotion.StopAllBehaviorsHelper` (class; 1 ctor) - Send this message to immediately stop all behaviors from executing.
- `GTA.NaturalMotion.Synchroisation` (enum; 3 values)
- `GTA.NaturalMotion.TeeterHelper` (class; 1 ctor, 10 props)
- `GTA.NaturalMotion.TurnType` (enum; 3 values)
- `GTA.NaturalMotion.UpperBodyFlinchHelper` (class; 1 ctor, 17 props)
- `GTA.NaturalMotion.YankedHelper` (class; 1 ctor, 40 props)
- `GTA.ParachuteLandingType` (enum; 10 values)
- `GTA.ParachuteState` (enum; 5 values)
- `GTA.ParachuteTint` (enum; 15 values)
- `GTA.PedBoneCollection.Enumerator` (class; 1 ctor, 1 props, 2 methods)
- `GTA.PedGroup.Enumerator` (class; 1 ctor, 1 props, 2 methods)
- `GTA.RagdollType` (enum; 7 values)
- `GTA.Rope` (class; 1 ctor, 2 props, 13 methods)
- `GTA.RopeType` (enum; 2 values)
- `GTA.Style` (class; 2 props, 8 methods)
- `GTA.WindowTitle` (enum; 17 values)
