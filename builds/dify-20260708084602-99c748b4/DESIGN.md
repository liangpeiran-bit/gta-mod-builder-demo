# DESIGN.md for ChaosBarrageReward

## Mod Summary

ChaosBarrageReward adds two LIVE Studio interactions for GTA V Story Mode:

1. When a viewer sends a chat message containing the word "ping" (case‑insensitive), the script shows a notification `Ping received! +25 Armor` and immediately adds 25 points to the player’s current armor (capped at 100).
2. When a viewer sends the "rose" gift, the script spawns an invincible police car (`POLICE`) near the player. The previous police car spawned by this mod is deleted whenever a new one appears to prevent clutter.

Both actions are throttled by independent cooldowns: 5 seconds for the ping bonus and 15 seconds for the rose reward. The mod operates entirely in Story Mode and does not touch any Online‑exclusive features.

## Interaction Map

| Trigger Type | Keyword / Gift ID | Condition / RepeatEnd | Cooldown | Effect Description | Notes |
|--------------|-------------------|------------------------|----------|--------------------|-------|
| Chat Message | `ping` (case‑insensitive, contains) | – | 5 seconds | Show notification subtitle: `Ping received! +25 Armor`.<br>Add 25 armor to player (max 100). | Uses `ChatEvent.Content.ToLower().Contains("ping")`. <br>Only fires when cooldown elapsed. |
| Gift | `"rose"` | `GiftEvent.RepeatEnd == false` | 15 seconds | Spawn an invincible police car (model `POLICE`) 5 metres in front of the player.<br>Delete any previously spawned police car from this mod first. | `GiftEvent.GiftId` compared case‑sensitively to `"rose"`. <br>RepeatEnd check avoids multi‑spam from repeated gift bursts. |

*All game logic is dispatched to the main thread via `MainThreadDispatcher.Enqueue` because LIVE Studio events may arrive on a background thread.*

## Gameplay Rules

1. **Ping armor reward**
   - Reads current player armor via `Game.Player.Character.Armor`.
   - New armor = `Math.Min(currentArmor + 25, 100)`.
   - Notification shown with `GTA.UI.Notification.Show("Ping received! +25 Armor")`.
   - Cooldown measured in wall‑clock seconds (`DateTime.UtcNow`). If another `ping` arrives within 5 seconds, it is silently ignored.

2. **Rose gift reward – invincible police car**
   - Vehicle model: `VehicleHash.Police` (string `"POLICE"`).
   - Spawn location: player’s position + forward offset `player.ForwardVector * 5f` at ground height.
   - The vehicle is immediately set as `IsInvincible = true`.
   - A class‑level field (`_lastPoliceVehicle`) tracks the last spawned police car. Before spawning a new one, call `_lastPoliceVehicle?.Delete()` and then assign the new instance.
   - Cooldown: 15 seconds. If a new rose gift arrives before the cooldown expires, it is ignored.
   - Respects `GiftEvent.RepeatEnd`: the spawn logic only runs when `RepeatEnd` is `false`. This prevents additional spawns at the end of a repeated gift combo.

3. **Cooldown management**
   - Two separate `DateTime` fields (`_lastPingTime`, `_lastRoseTime`).
   - Cooldown check: `(DateTime.UtcNow - last).TotalSeconds < cooldownSeconds`.

4. **LIVE Studio lifecycle**
   - Event handlers registered inside the script’s constructor (or an `OnStart` equivalent).
   - `LiveStudioClient` is initialised once and all event subscriptions are cleaned up with `Dispose` when the script unloads (if the template provides a dispose mechanism).

## Game Effects

| Effect | Implementation details |
|--------|-------------------------|
| Armor increase | `Game.Player.Character.Armor = newArmor;` <br>Value always clamped to 0‑100. |
| Subtitle notification | `Notification.Show("Ping received! +25 Armor")` – this is a standard GTA V notification that appears in the top‑left corner. It disappears automatically after a few seconds. |
| Police car spawn | `World.CreateVehicle(new Model("POLICE"), spawnPos, playerHeading)` <br>`vehicle.IsInvincible = true;` <br>Spawn coordinates: `player.Position + player.ForwardVector * 5f`. <br>Previous vehicle deleted with `oldVehicle?.Delete()`. |
| No effect on cooldown | Completely silent – no notification, no world change. |

## Edge Cases & Boundaries

- **Player is dead or in a cutscene**: The armor increase is still applied to the character’s armor value (NPC character remains synced). The police car spawn is suppressed if `player.IsAlive` is false because spawning a vehicle on a dead player may cause placement issues.
- **Player has maximum armor (100)**: The notification still appears but armor stays at 100 (Math.Min ensures no visual bar glitch).
- **Vehicle model fails to load**: `new Model("POLICE")` might return an invalid model if assets are missing (highly unlikely in Story Mode). The script must check `model.IsValid` and skip spawn if invalid, optionally showing a debug notification.
- **Rapid chat spam**: Cooldown prevents effect spam. Even if hundreds of “ping” messages arrive within one second, only one armor bonus is applied every 5 seconds.
- **Gift received with RepeatEnd = true alone**: The mod ignores it, preventing a car from spawning purely on the combo‑end signal (which might come without a preceding start signal in some platforms).
- **Invincible police car persistence**: The car remains invincible until destroyed by script or gameplay (e.g., explosions that push it into water). If it gets stuck or becomes obsolete, the next rose reward will delete it and create a fresh one. No automatic cleanup timer is implemented to keep the car available as a reward trophy, but this can be added later.
- **No player character**: Script should check `Game.Player.Character` is not null before accessing armor or position. If null, log warning and return.

## Design Notes

- **Notification vs. Subtitle**: `Notification.Show` was chosen over `GTA.UI.Subtitle.Show` because it is a non‑blocking, standard in‑game toast that disappears automatically and works reliably with multiple rapid calls (cooldown notwithstanding).
- **Cooldown timing**: Based on `DateTime.UtcNow` rather than `Game.GameTime` to avoid issues with game pause/‑ slow‑motion. This keeps the cooldown consistent in real time.
- **Police car model**: `POLICE` is the standard LSPD patrol car, always available in Story Mode without any DLC requirement. No custom models are required.
- **Single car management**: Keeping a reference and deleting the old car avoids littering the world with invincible police cruisers. This also respects memory and performance.
- **LIVE Studio threading**: Because LIVE Studio may deliver events on a non‑UI thread, all world and UI calls are wrapped in `MainThreadDispatcher.Enqueue`. This prevents crashes and is a documented requirement of the template.
- **Gift ID choice**: `"rose"` is assumed as the gift identifier. If the actual platform uses a numeric ID or a different string, only that string constant needs to be changed in the code.
- **RepeatEnd handling**: Many platforms send a `RepeatEnd = true` gift event when a multi‑send combo finishes. By spawning only on `RepeatEnd == false` we replicate exactly one police car per logical gift act, not one per individual repeat. If the platform never sends repeat events, this condition is harmless (RepeatEnd will be false).

## Change Log

| Version | Date | Changes |
|---------|------|---------|
| v0.1 | (initial) | Initial design: ping armor reward, rose gift invincible police car, cooldowns, repeat‑end filtering. |

## Template API Notes

The following symbols and patterns are part of the LIVE Studio template contract for this mod and must be used when implementing the C# script:

- `ChatEvent.Content` – string containing the chat message; used to check for “ping”.
- `GiftEvent.GiftId` – string gift identifier; compared against `"rose"`.
- `GiftEvent.RepeatEnd` – boolean indicating if this event is the end of a repeated gift combo; checked to suppress duplicate spawns.
- `MainThreadDispatcher.Enqueue(Action)` – required to marshal game‑world calls (armor, vehicle spawn, notifications) from the background event thread to the main game thread.
- `LiveStudioClient.Dispose()` – called in the script’s cleanup/dispose method to unsubscribe events and release resources.
- `GTA.Math.Vector3` – used when calculating the spawn position (`player.Position + player.ForwardVector * 5f`).
- `HandleEvent(LiveStudioEvent evt)` – assumed method signature for event dispatch (chat or gift).
- `List<Vehicle>` – although the design uses a single vehicle reference, a list may be used if extension to multiple vehicles is desired.
- No `new Vehicle(handle)` – vehicles must always be created through `World.CreateVehicle` to ensure proper wrapper and avoid orphaned handles.
- `GTA.UI.Notification.Show` – used for the “Ping received!” message.