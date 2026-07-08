# SHVDN Quick Reference

Target runtime: ScriptHookVDotNet v3, compile reference pinned to `ScriptHookVDotNet3` `3.6.0`.

## Environment

- C# 7.3
- .NET Framework 4.8
- Class library output
- User copies the compiled dll into `GTA5/scripts/`

Avoid C# 8+ features: nullable reference types, records, `init`, `required`, file-scoped namespaces, switch expressions that require newer language versions.

## Common Namespaces

```csharp
using GTA;
using GTA.Math;
using GTA.Native;
using GTA.UI;
```

## Common APIs

Notifications:

```csharp
Notification.Show("message");
```

Player:

```csharp
var player = Game.Player;
var ped = Game.Player.Character;
ped.Health = ped.MaxHealth;
ped.Armor = 100;
Game.Player.WantedLevel = 0;
```

Weather / time:

```csharp
World.Weather = Weather.Clear;
World.CurrentDayTime = new TimeSpan(12, 0, 0);
```

Spawn vehicle:

```csharp
var model = new Model(VehicleHash.Krieger);
model.Request(1000);
if (model.IsLoaded)
{
    var player = Game.Player.Character;
    var spawn = player.Position + player.ForwardVector * 5f;
    var vehicle = World.CreateVehicle(model, spawn, player.Heading);
    model.MarkAsNoLongerNeeded();
}
```

Give weapon:

```csharp
Game.Player.Character.Weapons.Give(WeaponHash.CarbineRifle, 120, true, true);
```

Explosion:

```csharp
World.AddExplosion(position, ExplosionType.Grenade, 1.0f, 0.0f);
```

## Robustness Rules

- Check `Game.Player.Character` before using it if the effect may run during death / loading / cutscene states.
- Use cooldowns for audience-triggered effects.
- Prefer `RepeatEnd` for gift combo effects unless the design asks for every gift tick to trigger.
- Call `MarkAsNoLongerNeeded()` after using loaded models.
- Avoid long blocking waits on Tick.
- Do not read set-only vehicle multiplier properties. `Vehicle.EnginePowerMultiplier` and `Vehicle.EngineTorqueMultiplier` may be set; reset them to `1.0f` instead of reading original values.
- Use `KeyEventArgs` from `System.Windows.Forms`, not `GTA.KeyEventArgs`.

## Capability Boundary

Supported:

- Built-in vehicles, peds, props, weapons.
- Health / armor / wanted level.
- Weather and time.
- Notifications and simple UI.
- Explosions, simple forces, simple camera or screen effects.

Not supported:

- Custom models or textures.
- Character skins.
- OpenIV, RPF, DLC packs.
- OBS overlays.
- External web UI inside the game.

If the user asks for an unsupported asset pipeline, propose a built-in GTA equivalent.
