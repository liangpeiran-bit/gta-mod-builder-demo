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