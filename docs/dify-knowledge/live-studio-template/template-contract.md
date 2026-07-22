# LIVE Studio Template Contract v3

Dify generates `ModProject/GeneratedGameplay.cs`, not the fixed `Mod.cs` runtime shell.

Required shape:

```csharp
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

Available hooks: `InitializeGameplay`, `OnGameplayTick`, `OnGameplayAborted`, `OnChat`, and `OnGift`, all declared as `partial void`.

Every generated hook runs on the GTA `Script.Tick` main thread. The fixed runtime performs the WebSocket-thread-to-main-thread dispatch before calling `OnChat` or `OnGift`. Call GTA/SHVDN APIs directly inside hooks. Never use `EnqueueGameplay`, `Task.Run`, custom threads, thread-pool work, or timers.

Use `ChatEvent.Content`. For one-shot gifts, use `TriggerGiftOnce(gift, "5655", matchedGift => { ... })`. It owns gift-id matching, combo-end handling, a 1200 ms missing-terminal fallback, and diagnostics. Pass `"*"` only when the confirmed design explicitly says any gift. Use `TriggerGiftEveryEvent(...)` only when the design explicitly requires an effect for every combo event. Do not compare `GiftId` or inspect `RepeatEnd` directly.

Never generate or reference `LiveStudioClient`, `LiveStudioParser`, `LiveStudioEvent`, `SubscriptionEvent`, `MainThreadDispatcher`, WebSocket code, JSON parsing, a `Mod` constructor, `class Mod : Script`, or `HandleEvent`.
