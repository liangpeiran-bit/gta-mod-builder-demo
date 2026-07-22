# LIVE Studio Template Contract v3

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

All generated hooks run on the GTA `Script.Tick` main thread. The fixed `Mod.cs` runtime receives LIVE Studio events on the WebSocket thread and performs the thread hop before calling `OnChat` or `OnGift`. Generated gameplay calls GTA and SHVDN APIs directly; it must not create another dispatcher, background task, thread, or timer.

For one-shot gift effects, use the fixed helper:

```csharp
TriggerGiftOnce(gift, "5655", matchedGift =>
{
    // GTA world and UI work. This already runs on the main thread.
});
```

`TriggerGiftOnce` matches the gift id, waits for `RepeatEnd`, deduplicates a pending combo by viewer and gift, and executes after a 1200 ms quiet-period fallback if a LIVE Studio version omits the terminal event.

Pass `"*"` as `expectedGiftId` only when `DESIGN.md` explicitly says any gift. The fixed runtime still keys pending combos by the actual received gift id.

Use `TriggerGiftEveryEvent(gift, "5655", action)` only when `DESIGN.md` explicitly requires an effect for every combo event.

`LogGameplay(string message)` is also available for diagnostic UI feedback.

## Event API

`ChatEvent` exposes `UserId`, `Nickname`, `Content`, `MsgId`, and `CreateTime`.

`GiftEvent` exposes `UserId`, `Nickname`, `GiftId`, `GiftName`, `DiamondCount`, `RepeatCount`, `RepeatEnd`, `ComboCount`, `MsgId`, and `CreateTime`.

`GiftId` is a string. `RepeatEnd` is normalized by the fixed parser from `0/1`, booleans, or boolean strings. Generated code does not compare `GiftId` or inspect `RepeatEnd` directly; it chooses `TriggerGiftOnce` or `TriggerGiftEveryEvent` and passes the confirmed id as a quoted string.

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
- `EnqueueGameplay(...)`, `Task.Run(...)`, custom threads, thread-pool work, or timers
- direct `gift.GiftId` comparisons or direct `gift.RepeatEnd` checks
- `chat.Message`, `chat.Text`, or integer comparisons such as `gift.GiftId == 5655`
