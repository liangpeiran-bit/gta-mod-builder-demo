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