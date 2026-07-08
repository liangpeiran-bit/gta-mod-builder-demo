# LIVE Studio Template Contract

This file documents the stable API surface that generated `ModProject/Mod.cs` may use from the fixed LIVE Studio template.

Dify must treat this contract as authoritative. These types are not SHVDN APIs; they are repository template APIs. Do not infer alternate field names from generic live-chat examples.

## Required File Shape

Generated code must replace only `ModProject/Mod.cs`.

Required structure:

```csharp
namespace ModProject
{
    public class Mod : Script
    {
    }
}
```

Do not regenerate or edit:

- `LiveStudioClient`
- `LiveStudioParser`
- `LiveStudioEvents`
- `MainThreadDispatcher`
- `ModProject.csproj`
- `Directory.Build.props`
- `scripts/build-mod.ps1`

## Namespaces

Common imports for generated `Mod.cs`:

```csharp
using System;
using System.Collections.Generic;
using GTA;
using GTA.Math;
using GTA.Native;
using GTA.UI;
using ModProject.LiveStudio;
```

Add `using GTA.Math;` when using `Vector3`.

## LiveStudioClient

Available constructor:

```csharp
new LiveStudioClient(Action<LiveStudioEvent> onEvent, Action<string> onLog = null)
```

Available members:

- `Start()`
- `Stop()`
- `Dispose()`
- `IsRunning`

Recommended lifecycle:

```csharp
_client = new LiveStudioClient(
    onEvent: HandleEvent,
    onLog: msg => MainThreadDispatcher.Enqueue(() => Notification.Show("~y~" + msg)));
_client.Start();
```

Cleanup should call `_client?.Dispose();`.

## LiveStudioEvent

Base properties:

- `MsgId` (`string`)
- `CreateTime` (`long`)

`LiveStudioClient` expects `Action<LiveStudioEvent>`, so generated handlers must take exactly one `LiveStudioEvent` parameter:

```csharp
private void HandleEvent(LiveStudioEvent evt)
```

Do not write event-style handlers such as `HandleEvent(object sender, object e)` for `LiveStudioClient`.

Generated code should switch on concrete event types:

```csharp
private void HandleEvent(LiveStudioEvent evt)
{
    switch (evt)
    {
        case ChatEvent chat:
            OnChat(chat);
            break;
        case GiftEvent gift:
            OnGift(gift);
            break;
    }
}
```

## ChatEvent

Properties:

- `UserId` (`string`)
- `Nickname` (`string`)
- `Content` (`string`)
- inherited `MsgId` (`string`)
- inherited `CreateTime` (`long`)

Important:

- `ChatEvent.Content` is the chat message text.
- Use `chat.Content` for the chat text.
- Do not use `chat.Message`; it does not exist.
- Do not use `chat.Text`; it does not exist.

Example:

```csharp
private void OnChat(ChatEvent chat)
{
    if (string.IsNullOrEmpty(chat.Content)) return;

    if (chat.Content.Trim().Equals("ping", StringComparison.OrdinalIgnoreCase))
    {
        MainThreadDispatcher.Enqueue(() =>
        {
            GTA.UI.Screen.ShowSubtitle("Ping received");
            Game.Player.Character.Armor = Math.Min(100, Game.Player.Character.Armor + 25);
        });
    }
}
```

## GiftEvent

Properties:

- `UserId` (`string`)
- `Nickname` (`string`)
- `GiftId` (`string`)
- `GiftName` (`string`)
- `DiamondCount` (`int`)
- `RepeatCount` (`int`)
- `RepeatEnd` (`bool`)
- `ComboCount` (`int`)
- inherited `MsgId` (`string`)
- inherited `CreateTime` (`long`)

Important:

- `GiftEvent.GiftId` is a string.
- Compare gift ids as strings, for example `gift.GiftId == "5655"`.
- Do not compare `gift.GiftId` with an integer, for example `gift.GiftId == 5655`.
- For one-shot gift effects, check `gift.RepeatEnd` unless the design explicitly says otherwise.

Example:

```csharp
private void OnGift(GiftEvent gift)
{
    if (gift.GiftId == "5655" && gift.RepeatEnd)
    {
        MainThreadDispatcher.Enqueue(() =>
        {
            GTA.UI.Screen.ShowSubtitle(gift.Nickname + " sent Rose");
        });
    }
}
```

## Threading Contract

LIVE Studio callbacks run on a background WebSocket thread.

Any GTA world mutation or UI call triggered by chat/gift handling must run through:

```csharp
MainThreadDispatcher.Enqueue(() =>
{
    // GTA world or UI calls here.
});
```

`OnTick` must drain queued work:

```csharp
private void OnTick(object sender, EventArgs e)
{
    MainThreadDispatcher.DrainOnTick();
}
```

## Entity Tracking

Track spawned vehicles and peds as entity objects, not integer handles.

Good:

```csharp
private readonly List<Vehicle> _spawnedVehicles = new List<Vehicle>();
_spawnedVehicles.Add(vehicle);
```

Invalid:

```csharp
private readonly HashSet<int> _spawnedVehicles = new HashSet<int>();
_spawnedVehicles.Add(vehicle.Handle);
Vehicle vehicle = new Vehicle(handle);
```

Do not use `new Vehicle(handle)`. SHVDN v3 does not expose a public `Vehicle` constructor that takes a handle in generated code.

## Prohibited Patterns

Do not use:

- `chat.Message`
- `chat.Text`
- `gift.GiftId == 5655`
- `HandleEvent(object sender, object e)` for `LiveStudioClient`
- `new Vehicle(handle)`
- `GTA.KeyEventArgs`
- `UI.Notify`
- `Entity.CreatedAt`, `Ped.CreatedAt`, `Vehicle.CreatedAt`
- `Newtonsoft.Json`
- `System.Text.Json`
- Custom WebSocket clients, parsers, or dispatcher classes in generated `Mod.cs`
