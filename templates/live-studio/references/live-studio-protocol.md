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

- Fire simple one-shot effects only on `RepeatEnd` to avoid N repeated effects.
- Fire progressive effects on every event only if the design explicitly asks for combo scaling.

## Threading

WebSocket receive / parse / event callback run on a background thread. SHVDN game APIs must run on the Tick main thread.

Correct pattern:

```csharp
private void OnGift(GiftEvent gift)
{
    MainThreadDispatcher.Enqueue(() =>
    {
        GTA.UI.Notification.Show(gift.Nickname + " sent " + gift.GiftName);
    });
}
```

Do not call `World.*`, `Game.*`, `Game.Player.*`, `Notification.*`, or entity APIs directly from the WebSocket callback.
