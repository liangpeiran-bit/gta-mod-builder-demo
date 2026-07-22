# LIVE Studio Template Contract v2

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

`OnChat` and `OnGift` run on a background thread. Schedule GTA calls with `EnqueueGameplay(Action)`. Tick and abort hooks already run on the GTA main thread.

Use `ChatEvent.Content`. `GiftEvent.GiftId` is a string. `GiftEvent.RepeatEnd` is a normalized boolean and one-shot gift effects normally require it.

Never generate `LiveStudioClient`, `LiveStudioParser`, `LiveStudioEvent`, `MainThreadDispatcher`, WebSocket code, JSON parsing, a `Mod` constructor, `class Mod : Script`, or `HandleEvent`.
