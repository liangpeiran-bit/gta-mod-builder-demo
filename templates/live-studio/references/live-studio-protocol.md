# LIVE Studio Interaction Protocol

This mod uses the TikTok LIVE Studio local WebSocket bridge.

## Endpoint

```text
ws://127.0.0.1:60080
```

## Subscription

Send this after connecting:

```json
{
  "type": "subscribe",
  "id": "gta-mod",
  "data": {
    "type": "serviceSignalSub",
    "name": "IM_MESSAGE_TRANSPORT"
  }
}
```

## Supported Events

The template supports two LIVE Studio IM methods:

- `WebcastChatMessage`
- `WebcastGiftMessage`

Do not implement follow / like / join / share / room-state triggers unless LIVE Studio explicitly exposes those signals through this local bridge.

## Chat Shape

Relevant fields:

```text
data.common.method == "WebcastChatMessage"
data.common.msgId
data.common.createTime
data.user.id
data.user.nickname
data.content
```

The template turns this into:

```csharp
ChatEvent {
  UserId,
  Nickname,
  Content,
  MsgId,
  CreateTime
}
```

## Gift Shape

Relevant fields:

```text
data.common.method == "WebcastGiftMessage"
data.common.msgId
data.common.createTime
data.user.id
data.user.nickname
data.gift.id
data.gift.name
data.gift.diamondCount
data.repeatCount
data.repeatEnd
data.comboCount
```

The template turns this into:

```csharp
GiftEvent {
  UserId,
  Nickname,
  GiftId,
  GiftName,
  DiamondCount,
  RepeatCount,
  RepeatEnd,
  ComboCount,
  MsgId,
  CreateTime
}
```

## Combo Semantics

When a viewer sends a combo of N gifts, LIVE Studio can emit N `GiftEvent`s. `RepeatCount` is progress within the combo. `RepeatEnd` is true on the final message.

Default behavior for most mods:

- Use the fixed `TriggerGiftOnce` helper. It fires on `RepeatEnd` and has a 1200 ms quiet-period fallback when a LIVE Studio build omits the terminal event.
- Fire progressive effects on every event only if the design explicitly asks for combo scaling.

## Threading

WebSocket receive and parsing run on a background thread. The fixed runtime queues parsed events and invokes all generated hooks on the Tick main thread.

Correct pattern:

```csharp
partial void OnGift(GiftEvent gift)
{
    TriggerGiftOnce(gift, "5655", matchedGift =>
    {
        GTA.UI.Notification.Show(matchedGift.Nickname + " sent " + matchedGift.GiftName);
    });
}
```

Do not generate WebSocket callbacks, dispatchers, tasks, threads, or timers. Generated hooks already run on the GTA main thread.
