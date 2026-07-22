# LIVE Studio Template Contract v2

This contract describes the only surface Dify-generated gameplay may use. The repository owns the WebSocket client, parser, event routing, dispatcher, Script lifecycle, and build project.

## Generated File

Dify generates only `ModProject/GeneratedGameplay.cs` plus `DESIGN.md`.

Required shape:

```csharp
using System;
using ModProject.LiveStudio;

namespace ModProject
{
    public partial class Mod
    {
        partial void OnChat(ChatEvent chat) { }
        partial void OnGift(GiftEvent gift) { }
    }
}
```

Do not generate a constructor, inherit from `Script`, implement `HandleEvent`, or declare any LIVE Studio transport, parser, event, or dispatcher type.

## Available Hooks

Implement only the hooks needed by `DESIGN.md`:

```csharp
partial void InitializeGameplay();
partial void OnGameplayTick();
partial void OnGameplayAborted();
partial void OnChat(ChatEvent chat);
partial void OnGift(GiftEvent gift);
```

`OnGameplayTick` and `OnGameplayAborted` already run on the GTA main thread. `OnChat` and `OnGift` run on the LIVE Studio background thread, so GTA work from those handlers must be scheduled with:

```csharp
EnqueueGameplay(() =>
{
    // GTA world and UI work
});
```

`LogGameplay(string message)` is also available for diagnostic UI feedback.

## Event API

`ChatEvent` exposes `UserId`, `Nickname`, `Content`, `MsgId`, and `CreateTime`.

`GiftEvent` exposes `UserId`, `Nickname`, `GiftId`, `GiftName`, `DiamondCount`, `RepeatCount`, `RepeatEnd`, `ComboCount`, `MsgId`, and `CreateTime`.

`GiftId` is a string. `RepeatEnd` is normalized by the fixed parser from `0/1`, booleans, or boolean strings. Compare gift ids as quoted strings, and require `RepeatEnd` for one-shot gift effects unless the design says otherwise.

## SHVDN Constraints

- Target GTA5 Legacy, ScriptHookVDotNet v3, .NET Framework 4.8, and C# 7.3.
- Track spawned vehicles and peds as entity objects, not integer handles.
- Do not use `new Vehicle(handle)`.
- `World.GetNextPositionOnStreet(...)` returns a `Vector3`; it is not a bool-returning `out` API.
- Use the three-argument `World.CreateVehicle(modelOrHash, position, heading)` overload.
- Use `GTA.UI.Notification.Show(...)` or `GTA.UI.Screen.ShowSubtitle(...)`.
- Add cooldowns, bounded collections, and cleanup required by `DESIGN.md`.

## Forbidden Generated Content

- `LiveStudioClient`, `LiveStudioParser`, `LiveStudioEvent`, or `MainThreadDispatcher` declarations
- `ClientWebSocket`, `JavaScriptSerializer`, `Newtonsoft.Json`, or `System.Text.Json`
- `ws://127.0.0.1:60080`, `serviceSignalSub`, or `IM_MESSAGE_TRANSPORT`
- a `Mod` constructor, `class Mod : Script`, or `HandleEvent(...)`
- `chat.Message`, `chat.Text`, or integer comparisons such as `gift.GiftId == 5655`
