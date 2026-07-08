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