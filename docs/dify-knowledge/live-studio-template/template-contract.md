# Dify Knowledge: LIVE Studio Template Contract

This document is for Dify Knowledge Base retrieval. It describes the fixed LIVE Studio template API exposed to generated `ModProject/Mod.cs`.

These APIs are repository template APIs, not SHVDN APIs. When this document conflicts with generic chat/gift examples, use this document.

## Generated File Boundary

Dify generates only:

- `DESIGN.md`
- `ModProject/Mod.cs`

Dify must not regenerate:

- `LiveStudioClient`
- `LiveStudioParser`
- `LiveStudioEvents`
- `MainThreadDispatcher`
- `ModProject.csproj`
- `Directory.Build.props`
- `scripts/build-mod.ps1`

Generated `Mod.cs` must use:

```csharp
namespace ModProject
{
    public class Mod : Script
    {
    }
}
```

## Required Template Types

Use these template classes from `ModProject.LiveStudio`:

- `LiveStudioClient`
- `LiveStudioEvent`
- `ChatEvent`
- `GiftEvent`
- `MainThreadDispatcher`

Recommended imports:

```csharp
using System;
using System.Collections.Generic;
using GTA;
using GTA.Math;
using GTA.Native;
using GTA.UI;
using ModProject.LiveStudio;
```

If generated code uses `Vector3`, it must include `using GTA.Math;`.

For UI feedback, prefer fully qualified calls:

- `GTA.UI.Screen.ShowSubtitle(...)`
- `GTA.UI.Notification.Show(...)`

If using unqualified `Notification.Show(...)`, include `using GTA.UI;`.

## LiveStudioClient API

Constructor:

```csharp
new LiveStudioClient(Action<LiveStudioEvent> onEvent, Action<string> onLog = null)
```

Members:

- `Start()`
- `Stop()`
- `Dispose()`
- `IsRunning`

Recommended lifecycle:

```csharp
_client = new LiveStudioClient(
    onEvent: HandleEvent,
    onLog: msg => MainThreadDispatcher.Enqueue(() => GTA.UI.Notification.Show("~y~" + msg)));
_client.Start();
```

Cleanup:

```csharp
_client?.Dispose();
```

`LiveStudioClient` expects `Action<LiveStudioEvent>`, so event handlers must take exactly one `LiveStudioEvent` parameter:

```csharp
private void HandleEvent(LiveStudioEvent evt)
```

Do not use `HandleEvent(object sender, object e)` for `LiveStudioClient`.

## ChatEvent API

Properties:

- `UserId` (`string`)
- `Nickname` (`string`)
- `Content` (`string`)
- `MsgId` (`string`, inherited)
- `CreateTime` (`long`, inherited)

Rules:

- Use `ChatEvent.Content` for chat message text.
- Do not use `ChatEvent.Message`; it does not exist.
- Do not use `ChatEvent.Text`; it does not exist.

Example:

```csharp
if (!string.IsNullOrEmpty(chat.Content) &&
    chat.Content.Trim().Equals("ping", StringComparison.OrdinalIgnoreCase))
{
    MainThreadDispatcher.Enqueue(() =>
    {
        GTA.UI.Screen.ShowSubtitle("Ping received");
        Game.Player.Character.Armor = Math.Min(100, Game.Player.Character.Armor + 25);
    });
}
```

## GiftEvent API

Properties:

- `UserId` (`string`)
- `Nickname` (`string`)
- `GiftId` (`string`)
- `GiftName` (`string`)
- `DiamondCount` (`int`)
- `RepeatCount` (`int`)
- `RepeatEnd` (`bool`)
- `ComboCount` (`int`)
- `MsgId` (`string`, inherited)
- `CreateTime` (`long`, inherited)

Rules:

- `GiftEvent.GiftId` is a string.
- Compare gift ids as string literals: `gift.GiftId == "5655"`.
- Never compare gift ids as integers: `gift.GiftId == 5655` is invalid.
- For one-shot gift effects, check `gift.RepeatEnd` unless `DESIGN.md` explicitly says otherwise.

Example:

```csharp
if (gift.GiftId == "5655" && gift.RepeatEnd)
{
    MainThreadDispatcher.Enqueue(() =>
    {
        GTA.UI.Screen.ShowSubtitle(gift.Nickname + " sent Rose");
    });
}
```

## Threading Rules

LIVE Studio callbacks run on a background WebSocket thread.

All GTA world changes and UI calls from chat/gift handlers must be scheduled with:

```csharp
MainThreadDispatcher.Enqueue(() =>
{
    // GTA world or UI calls here.
});
```

`OnTick` must call:

```csharp
MainThreadDispatcher.DrainOnTick();
```

## Entity Tracking

Track spawned entities as SHVDN entity objects, not integer handles.

Good vehicle tracking:

```csharp
private readonly List<Vehicle> _spawnedVehicles = new List<Vehicle>();
_spawnedVehicles.Add(vehicle);
```

Invalid vehicle tracking:

```csharp
private readonly HashSet<int> _spawnedVehicles = new HashSet<int>();
_spawnedVehicles.Add(vehicle.Handle);
Vehicle vehicle = new Vehicle(handle);
```

Do not use `new Vehicle(handle)`. SHVDN v3 generated code should keep and clean up the `Vehicle` object returned by `World.CreateVehicle(...)`.

## SHVDN World API Constraints

Use SHVDN v3 overloads that exist in the template build.

Good:

```csharp
Vector3 spawnPos = World.GetNextPositionOnStreet(player.Position.Around(2.0f));
float heading = player.Heading;
Vehicle vehicle = World.CreateVehicle(VehicleHash.Police, spawnPos, heading);
```

Invalid:

```csharp
Vector3 streetPos;
float heading;
if (!World.GetNextPositionOnStreet(player.Position, out streetPos, out heading)) return;
Vehicle vehicle = World.CreateVehicle(VehicleHash.Police, streetPos, heading, VehicleLockStatus.None);
```

Rules:

- Do not treat `World.GetNextPositionOnStreet(...)` as a bool-returning `out` API. It returns a `Vector3`.
- Do not call `World.CreateVehicle(VehicleHash, Vector3, float, VehicleLockStatus)`. Use 3 arguments: model/hash, position, heading.

## Invalid Patterns

Generated `Mod.cs` must not contain:

- `chat.Message`
- `chat.Text`
- `gift.GiftId == 5655`
- `HandleEvent(object sender, object e)`
- `new Vehicle(handle)`
- `World.GetNextPositionOnStreet(..., out ..., out ...)`
- `World.CreateVehicle(..., ..., ..., VehicleLockStatus.None)`
- `GTA.KeyEventArgs`
- `UI.Notify`
- unqualified `Notification.Show(...)` without `using GTA.UI;`
- `Entity.CreatedAt`, `Ped.CreatedAt`, `Vehicle.CreatedAt`
- custom WebSocket client, parser, or dispatcher implementations
- `Newtonsoft.Json`
- `System.Text.Json`
