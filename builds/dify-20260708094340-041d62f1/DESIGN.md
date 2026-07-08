# DESIGN.md
## GTA5 Story Mode Live Interaction Mod

### Mod Summary
**Mod Name:** `GTA5StoryLiveInteraction`  
**Purpose:** Enables live audience interaction in GTA V Story Mode via chat and gift events. When a viewer sends a danmaku message containing “ping”, a subtitle is shown and the player receives 25 armour. When a viewer sends a rose gift, an invincible police car spawns near the player.

### Interaction Map
| Trigger Type | Keyword / Gift ID | Exact Gift ID | RepeatEnd Behaviour | Cooldown (seconds) | Intended GTA Story Mode Effect |
|--------------|-------------------|---------------|---------------------|--------------------|--------------------------------|
| ChatEvent | `ping` (case‑insensitive, contained anywhere in the message) | N/A | N/A | 0 (no cooldown) | Display a subtitle “Ping received! +25 Armor” and add 25 armour to the player (respecting max armour cap). |
| GiftEvent | Rose gift | `"rose"` | On `RepeatEnd`, spawn **one** additional invincible police car for each repetition (combo gift). Each repeat triggers a new spawn. | 0 (no cooldown) | Spawn an invincible police vehicle (`POLICE`) near the player’s current position. The vehicle is marked as invincible and stays until the player leaves the session or it is manually removed. |

### Gameplay Rules
1. **Armour on “ping”**  
   - The player’s current armour is read via `Game.Player.Character.Armor`.  
   - If adding 25 would exceed the maximum armour (100), the value is clamped to 100.  
   - If the player is dead or in a cutscene, no armour is given and no subtitle is shown.

2. **Invincible Police Car on Rose Gift**  
   - A model of type `VehicleHash.Police` is loaded and a vehicle is spawned at a position **5 metres in front** of the player’s current location (using `Game.Player.Character.GetOffsetPosition(new Vector3(0, 5, 0))`).  
   - The vehicle is made invincible via `vehicle.IsInvincible = true`.  
   - The vehicle is **not** locked and can be entered by the player.  
   - If the ground position is invalid (e.g., under the map), the vehicle is spawned at the nearest safe road position using `World.GetSafeCoordForPed()`.  
   - If a previous police car from the same gift session still exists, the new one is spawned regardless (no automatic cleanup).

3. **General**  
   - All effects are executed on the main thread using `MainThreadDispatcher.Enqueue`.  
   - Notification text is shown with `GTA.UI.Notification.Show`.

### Game Effects
- **Subtitle:** Displayed for 3 seconds at the bottom of the screen.  
- **Armour:** The player’s armour bar visibly increases. If the player had no armour, the blue bar appears.  
- **Police Car:** An indestructible LSPD cruiser appears. It can be used freely by the player; tyres can be popped but the body never deforms and the vehicle never catches fire or explodes.

### Edge Cases & Boundaries
- **Player is inside a vehicle when armour is given:** Armour is still applied; no special handling.  
- **Player is inside a vehicle when a police car spawns:** The new car may clip into the player’s vehicle. Offset is always 5m forward to mitigate this; if collision occurs, the vehicle will be pushed.  
- **Multiple rapid “ping” messages:** Each one is processed sequentially. If armour is already at 100, the subtitle still shows but armour stays at 100.  
- **Rose gift combo (RepeatEnd):** Each time `RepeatEnd` fires, a new police car is spawned. The previous ones are **not** deleted. Up to `int.MaxValue` cars can exist, which may cause performance issues. **Design note:** A future iteration could limit the number of police cars.  
- **Player is dead / wasted:** No effects are applied for either trigger.  
- **Player is in a mission that disables spawning:** The police car spawn may fail silently; no retry.  
- **Gift ID mismatch:** Only the exact string `"rose"` triggers the effect. Unknown gift IDs are ignored.  
- **Cooldown:** None implemented. Excessive triggers can be handled externally by the live platform.

### Design Notes
- This mod is built against the **fixed LIVE Studio template**. It does not include any external WebSocket or file‑watch code; all events are received through the provided `HandleEvent(LiveStudioEvent evt)` override.  
- For debugging, the same effects can be triggered by keyboard keys (e.g., F6 for ping, F7 for rose) if the template supports that, but the DESIGN focuses on the live event flow.  
- The `GiftEvent.GiftId` is assumed to be `"rose"`. If the actual live platform sends a different identifier (e.g., `"gift_rose"` or a numeric ID), the design must be updated.  
- No custom assets or OpenIV modifications are required.  
- The mod uses only vanilla GTA V vehicles and ScriptHookVDotNet v3 API.  
- All variable names, gift IDs, commands, file names, and in‑game text are in English.

### Change Log
| Version | Date | Description |
|---------|------|-------------|
| 1.0.0 | (initial) | Initial design – ping armour and rose police car. |

### Template API Notes
| Symbol / API | Usage in this mod |
|--------------|-------------------|
| `ChatEvent.Content` | Read to check if the string contains “ping” (case‑insensitive). |
| `GiftEvent.GiftId` (string) | Compared against `"rose"` to identify the correct gift. |
| `GiftEvent.RepeatEnd` | Handles combo gift repetitions; spawns an additional police car each time it fires. |
| `MainThreadDispatcher.Enqueue` | Schedules the armour/police car actions to run safely on the main game thread. |
| `LiveStudioClient.Dispose` | Not used in this mod; retained for completeness if resource cleanup is needed. |
| `GTA.Math.Vector3` | Used to compute the spawn position offset from the player (e.g., `new Vector3(0, 5, 0)`). |
| `HandleEvent(LiveStudioEvent evt)` | Main entry point where chat and gift events are dispatched. |
| `GTA.UI.Notification.Show` | Displays the armour notification as a small HUD banner. |
| `List<Vehicle>` | Could be used to track spawned police cars for potential cleanup, but currently not implemented. |
| `new Vehicle(handle)` avoidance | Vehicles are created with `World.CreateVehicle()` which returns a `Vehicle` directly; no raw handle is used, avoiding the need for manual cleanup. |