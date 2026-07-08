# Gameplay Patterns

Use these patterns when the mod is a custom gameplay mode, not just a trigger-to-effect mapping.

## One-Shot Triggered Effect

Audience action immediately causes an effect.

Examples:

- Rose heals the player.
- Chat `car` spawns a vehicle.
- Gift `Galaxy` changes weather and wanted level.

Design focus:

- Trigger.
- Effect.
- Cooldown / combo handling.

## Running Game Mode

The mod has persistent state and changes over time.

Examples:

- Survival mode: each gift spawns a wave; score increases for surviving waves.
- Voting mode: chat votes choose the next vehicle, weapon, or weather every 60 seconds.
- Chaos timer: every 30 seconds one random enabled effect fires, with gifts adding stronger effects to the pool.
- Boss fight: gifts damage or heal a boss represented by an in-game vehicle / ped / prop.
- Race challenge: chat commands start checkpoints, gifts add obstacles, timer tracks completion.

Design focus:

- Start / stop condition.
- State variables.
- Timer or Tick behavior.
- Score / progress.
- Round transitions.
- Failure and recovery behavior.

## Implementation Shape

Typical fields in `Mod.cs`:

```csharp
private bool _modeActive;
private int _score;
private DateTime _roundEndsAt;
private readonly Random _random = new Random();
```

Typical Tick pattern:

```csharp
private void OnTick(object sender, EventArgs e)
{
    MainThreadDispatcher.DrainOnTick();

    if (!_modeActive) return;

    if (DateTime.UtcNow >= _roundEndsAt)
    {
        EndRound();
    }
}
```

LIVE Studio events should update the mode state, then schedule any GTA work through `MainThreadDispatcher`.

## Guardrails

- Keep gameplay state inside the mod assembly; do not add external databases or services.
- Avoid unbounded spawning. Track spawned entities and delete or reuse them when reasonable.
- Keep per-tick work small. Do not scan the whole world every tick unless necessary.
- Put randomization bounds in `DESIGN.md`.
- If the user asks for assets outside GTA5 built-ins, propose a built-in substitute.
