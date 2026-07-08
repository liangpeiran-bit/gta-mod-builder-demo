# DESIGN.md — Viewer Rose Gift Sports Car Spawn

## Mod Summary
When a viewer sends a "Rose" gift (via live platform integration), a random sports car spawns near the player in GTA5 Story Mode.  
For development and testing without a live platform, a keyboard chat command (`!rosecar`) simulates the Rose gift event.  
The mod enforces a 30‑second cooldown to prevent spam and runs entirely within ScriptHookVDotNet v3 on a single `.cs` file.

---

## Interaction Map

| Trigger Type | Keyword / Gift ID | Exact Gift ID | RepeatEnd Behavior | Cooldown (sec) | Intended GTA Story Mode Effect |
|--------------|------------------|---------------|-------------------|----------------|--------------------------------|
| Chat Command | `!rosecar`       | —             | —                 | 30             | Spawns a random sports car 5 m in front of the player. |
| Gift Event   | Rose gift        | `rose`        | Ignored (single‑fire gift, no combo) | 30             | Same as chat command. |

**Notes:**
- The chat command `!rosecar` is the keyboard fallback for development; in production the live platform sends `GiftEvent` with `GiftId = "rose"`.
- `RepeatEnd` is not used because the Rose gift is treated as a single‑fire event. If the platform later sends a combo gift with multiple `GiftEvent` calls, the mod ignores all but the first per cooldown window.
- The 30‑second cooldown is shared across both trigger types (i.e., a gift resets the chat command cooldown and vice versa).

---

## Gameplay Rules

1. **Spawn location**  
   - The vehicle is placed 5 m in front of the player’s current facing direction.  
   - If the calculated position is blocked (e.g., inside a wall), the spawn shifts sideways up to 3 m left/right to find a clear spot.  
   - If no clear spot exists within 3 m, the spawn is aborted and the player sees a notification.

2. **Vehicle category**  
   - A random model is chosen from the GTA5 “sports car” class: `COMET`, `TURISMOR`, `CHEETAH`, `INFERNUS`, `BANSHEE`, `COQUETTE`.  
   - The same model may appear consecutively (true random, no history check).

3. **Player state**  
   - Spawn is allowed only when the player is on foot and not in an interior.  
   - If the player is in a vehicle or an interior, the event is queued for 2 seconds. If the player is still not on foot outdoors after the delay, the event is discarded.

4. **Cooldown**  
   - Starts when a vehicle spawns successfully (not when the event is received).  
   - While the cooldown is active, any received Rose gift or `!rosecar` command is silently ignored.

---

## Game Effects

### Primary Effect: `SpawnRandomSportsCar`
- **Method:** `World.CreateVehicle(model, position + forward * 5f + upward * 0.5f, heading)`  
- **Vehicle model:** Random from the predefined list (see above).  
- **Vehicle state:** Engine running, doors unlocked, player is **not** warped into the vehicle.  
- **Notification:** `GTA.UI.Notification.Show("A sports car has been delivered!")` (always shown on successful spawn).  
- **Cleanup:** No automatic cleanup. The vehicle remains in the world and follows normal GTA despawn rules (i.e., it may disappear when the player moves far away).

### Debug / Feedback
- When the cooldown is active and a valid trigger is received, the mod logs to the ScriptHookVDotNet console: `[RoseCar] Cooldown active – event ignored.`  
- When spawn fails due to blocked position, the notification shows: `"No space to spawn the car!"`

---

## Edge Cases & Boundaries

- **Player in interior or vehicle:** Event is held for 2 seconds, then discarded if the player’s state hasn’t changed.  
- **Blocked spawn position:** Lateral shuffle up to ±3 m; if still blocked, abort with notification.  
- **Gift floods / spam:** The 30‑second cooldown starts on successful spawn and blocks all duplicate triggers.  
- **Mod disables during cooldown:** Cooldown is a simple timestamp; if the mod is reloaded, the cooldown resets.  
- **Multiple Rose gifts in a combo (RepeatEnd != true):** Only the first event that passes the cooldown check triggers a spawn; subsequent events in the same cooldown window are ignored.  
- **Invalid vehicle model (edge case):** The predefined list contains only valid GTA5 Story Mode models; if a model fails to load (e.g., due to a corrupted game file), the spawn is silently aborted and logged.  
- **Performance:** No heavy processing; vehicle creation is a single `World.CreateVehicle` call on the main thread via `MainThreadDispatcher.Enqueue`.

---

## Design Notes

- **Why a separate chat command?**  
  To develop and test without a live platform, `!rosecar` mimics a GiftEvent with `GiftId = "rose"`. The handler logic is shared.

- **Why 30‑second cooldown?**  
  A default balancing measure; not specified by the user but prevents screen clutter and performance issues from spawn flooding.

- **Why no player warp?**  
  The user request only stated “刷新一辆跑车” (spawn a sports car). No instruction to put the player inside it, so the car spawns unlocked nearby.

- **Vehicle cleanup:**  
  No custom cleanup logic is implemented. Standard GTA5 despawn rules apply. This avoids accidental collection of `Vehicle` references and keeps memory simple.

- **Tier 0 constraints:**  
  The mod is a single `.cs` file using ScriptHookVDotNet v3, C# 7.3, GTA5 Story Mode only. No GTA Online, OpenIV, custom assets, or external dependencies.

---

## Change Log

| Version | Date | Changes |
|---------|------|---------|
| 1.0.0   | 2025-03-21 | Initial design: Rose gift / `!rosecar` spawns random sports car near player with 30 s cooldown. |

---

## Template API Notes

Symbols and patterns this mod must reference to comply with the LIVE Studio template contract:

| Symbol / Pattern | Usage in this mod |
|------------------|-------------------|
| `ChatEvent.Content` | Used to detect the `!rosecar` command fallback. |
| `GiftEvent.GiftId` string | Compared against `"rose"` to detect the live‑platform Rose gift. |
| `GiftEvent.RepeatEnd` | Checked only if the platform sends combo gifts; when `true` it signals the end of a combo, but our single‑fire logic ignores it. |
| `MainThreadDispatcher.Enqueue` | All vehicle spawn and notification calls are queued to the main thread. |
| `LiveStudioClient.Dispose` | Not explicitly needed; the mod does not hold external resources that require disposal. |
| `GTA.Math.Vector3` | Used to calculate spawn position: `player.Position + player.ForwardVector * 5f`. |
| `HandleEvent(LiveStudioEvent evt)` | The entry point where the mod receives both `ChatEvent` and `GiftEvent` and forwards them to the spawn logic. |
| `List<Vehicle>` | Not used; the mod does not track spawned vehicles (no cleanup list). |
| “never `new Vehicle(handle)` when vehicle cleanup is needed” | Not applicable; no vehicle handles are stored or disposed by the mod. |
| `GTA.UI.Notification.Show` | Called to display success/failure messages to the player. |