# DESIGN.md — RoseGiftCarSpawn

## Mod Summary
The mod listens for a “Rose” gift event from the LIVE Studio stream.  
When a viewer sends a Rose (gift id `5655`), a sports car spawns near the player’s current location in GTA V Story Mode.

---

## Interaction Map

| Trigger Type | Trigger Value (exact) | Exact Gift ID | Cooldown (seconds) | RepeatEnd Handling | Game Effect |
|--------------|-----------------------|---------------|---------------------|---------------------|-------------|
| Gift         | Rose (gift id `5655`) | 5655          | 0 (no cooldown)     | N/A — single gift, not a combo | Spawn a sports car (default model **Comet** / `COMET2`) on the nearest road close to the player |

---

## Gameplay Rules
- The vehicle model is a **Comet** (`COMET2`), representing a classic sports car.
- The spawn location is calculated as:
  1. Get the player character’s current position.
  2. Find the nearest safe road node within a 50‑meter radius using `GTA.Native.Function.Call` with `GET_SAFE_COORD_FOR_CHAR` style call (or `GET_NTH_CLOSEST_VEHICLE_NODE` + `GET_STREET_NAME_AT_COORD`).
  3. If no safe node is found, fall back to the player’s position with a small forward offset.
- The spawned vehicle is placed with its heading aligned to the road.
- The engine is turned on (`IsEngineRunning = true`).
- A notification is displayed: `"A sports car has been gifted!"`.
- No ownership, blip, or invincibility is applied; the car is a normal world vehicle.
- The mod does **not** track or clean up spawned vehicles. Repeated gifts will keep adding cars to the session.

---

## Game Effects
- **Immediate visual**: A red or metallic Comet appears near the player.
- **Audio**: Standard vehicle engine sound after spawn.
- **Notification**: On‑screen message `"A sports car has been gifted!"` using `GTA.UI.Notification.Show`.
- **Gameplay**: The player can enter and drive the car normally.

---

## Edge Cases & Boundaries
- **Player inside interior / safehouse**: The road‑node search may fail. Fallback spawns the car at the player’s feet with a small offset, which may clip into geometry.
- **Player in water or mid‑air**: The vehicle will spawn on the nearest road if one exists; otherwise it will fall from the player’s position (risk of falling into water).
- **Player dead / wasted**: The event still triggers (the player character still exists), so the car spawns anyway.
- **Multiple rapid gifts**: No cooldown means many cars can spawn instantly, potentially causing performance issues or traffic congestion. No built‑in cleanup.
- **Mod unloading**: Any spawned vehicles remain in the world. The mod does not remove them on disposal.

---

## Design Notes
- The mod follows the **fixed LIVE Studio template** and extends `LiveStudioMod`.
- Event handling occurs in `HandleEvent(LiveStudioEvent evt)`:
  - Filter for `GiftEvent` and compare `gift.GiftId == "5655"`.
  - Cooldown is set to `0`, so every Rose triggers a spawn.
- Vehicle spawning is executed on the **main thread** via `MainThreadDispatcher.Enqueue`.
- Vehicle creation uses `World.CreateVehicle` (which returns a `Vehicle`), never `new Vehicle(handle)`.
- The mod references **`GTA.Math.Vector3`** for position calculations.
- `LiveStudioClient.Dispose` is called in the mod’s `Unload()` or `Dispose()` method to properly clean up the LIVE Studio connection.
- No external assets, OpenIV modifications, or custom models are required.
- The sports car model (`COMET2`) can be changed by editing a constant inside `Mod.cs` if desired.

---

## Change Log
- **v0.1** – Initial implementation: Rose gift → spawn Comet sports car.

---

## Template API Notes
The following symbols and patterns are guaranteed to be available in the LIVE Studio template and must be used where appropriate:

- `ChatEvent.Content`
- `GiftEvent.GiftId` (string, e.g. `"5655"`)
- `GiftEvent.RepeatEnd`
- `MainThreadDispatcher.Enqueue`
- `LiveStudioClient.Dispose`
- `GTA.Math.Vector3`
- `HandleEvent(LiveStudioEvent evt)`
- `List<Vehicle>` (used if vehicle tracking is needed)
- Never use `new Vehicle(handle)` when vehicle cleanup is required; use `World.CreateVehicle` and store the `Vehicle` reference.
- `GTA.UI.Notification.Show` for displaying in‑game notifications.