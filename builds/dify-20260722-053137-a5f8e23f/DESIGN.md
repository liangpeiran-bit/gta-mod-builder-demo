<think>
用户要求生成一个 GTA5 LIVE Studio Mod，包含：

1. 聊天指令 `backup` - 显示观众昵称（如"[昵称] 请求支援"），按用户冷却 3 秒
2. 礼物触发 - 观众赠送任意礼物并在连击结束时，在玩家附近生成一辆救护车，全局冷却 5 秒

需要查询：
- SHVDN API：Notification（显示通知）、Vehicle（生成车辆）、VehicleHash（救护车）、World.CreateVehicle、安全位置生成等
- 礼物：用户说"任意礼物"，所以需要查询礼物工具来了解礼物系统，但实际上"任意礼物"不需要特定礼物映射。不过根据规则，当用户提到礼物意图时，需要调用 query_live_gifts。这里用户说的是"任意礼物"，所以可能需要确认礼物系统的运作方式。但规则说"当原始用户输入包含任何可能的礼物意图时调用 query_live_gifts"，这里确实有礼物意图。不过"任意礼物"意味着我们不需要特定礼物 ID 映射——只需要检查礼物消息的 data.gift.id、data.gift.name、data.gift.diamondCount 等字段。但用户没有指定特定礼物名称，所以礼物查询可能不会返回有意义的结果。

让我仔细想想：用户说"观众赠送任意礼物并在连击结束时，在玩家附近生成一辆救护车"。这意味着我们监听 WebcastGiftMessage，等待 repeatEnd=true（连击结束），然后生成救护车。不需要特定礼物 ID 映射。

但根据 LIVE_STUDIO_CONTRACT_V1 第 6 条：礼物字段包含 data.gift.id、data.gift.name、data.gift.diamondCount、data.repeatCount、data.repeatEnd、data.comboCount。所以我们可以直接使用这些字段。

对于礼物查询：用户说"任意礼物"，没有特定名称或 ID。我应该调用 query_live_gifts 吗？规则说"当原始用户输入包含任何可能的礼物意图时调用"，用户确实提到了礼物意图。但 query 应该是"任意礼物"相关的东西。这可能返回 not_found 因为没有具体礼物名称。但按照规则，我应该调用它。不过"任意礼物"意味着不依赖特定礼物映射，gift_requirements 可以标记为 not_required（因为不需要特定礼物）。等等，规则说"用户没有礼物意图时不得调用，gift_requirements 标记为 not_required"。但这里有礼物意图。再读一下："用户没有指定礼物时，不要求补充礼物，不编造 gift_id；可使用评论触发、计时触发、系统触发或待解析礼物触发。" 所以当用户说"任意礼物"，没有指定具体礼物名称或 ID，我们不需要特定礼物映射。但礼物仍然是玩法的一部分。我想我应该调用 query_live_gifts 看看，但 query 传入"任意礼物"可能不会匹配到任何具体礼物。

让我重新读规则：第4条"当原始用户输入包含任何可能的礼物意图时调用 query_live_gifts，包括明确名称、别名、错拼、模糊描述、数字 gift_id、价格条件，或玩法必须依赖礼物映射。用户没有礼物意图时不得调用，gift_requirements 标记为 not_required。"

这里用户明确提到了礼物（"观众赠送任意礼物并在连击结束时"），所以有礼物意图。但"任意礼物"不是特定名称、别名或 gift_id。这是"玩法必须依赖礼物映射"的情况吗？实际上玩法依赖的是礼物消息的接收，而不是特定礼物映射。我们只需要监听 WebcastGiftMessage 并在 repeatEnd=true 时触发。

我认为应该调用 query_live_gifts，传入"任意礼物"或类似内容。但返回可能是 not_found。我们可以在 gift_requirements 表中记录这一点。

对于 SHVDN API 查询，我需要：
- 通知显示：Notification.Show 或 UI.Notify
- 救护车生成：VehicleHash.Ambulance, World.CreateVehicle
- 安全位置生成：玩家位置附近，World.GetNextPositionOnStreet 或类似

让我先设计查询。

第一批 SHVDN：核心能力 - Notification, Vehicle spawn, Ambulance hash
第二批 SHVDN：如果需要的话 - 安全位置生成

实际上让我一次性查询所有核心能力。

查询1: "Notification.Show UI.Notify World.CreateVehicle VehicleHash.Ambulance spawn vehicle safe position near player"

同时调用 query_live_gifts。

让我先同时发起两个调用。
</think>
<think>
SHVDN 查询返回了 partial 状态，但已经找到了核心 API：
- `GTA.World.CreateVehicle` - 创建车辆
- `GTA.UI.Notification.Show` - 显示通知（两个重载）
- `GTA.VehicleHash` 枚举
- `GTA.VehicleHash.Ambulance` - 救护车

但我还需要安全位置生成。我可能需要 `World.GetNextPositionOnStreet` 或类似的方法。让我再查询一次。

另外，可能还需要：
- `Game.Player.Character.Position` 获取玩家位置
- `World.GetSafeCoordForPed` 或类似方法获取安全位置
- 或者简单地使用玩家附近位置

另外还需要确认 `Model` 类型用于 `CreateVehicle`。

让我发起第二次 SHVDN 查询。

另外礼物查询返回 not_found，这是预期的，因为"任意礼物"不是特定礼物名称。我们将标记 gift_requirements 为 not_found。

第二次 SHVDN 查询：安全位置生成、Model 类、Vector3、玩家位置
</think>
<think>
好的，第二次查询也返回了 partial，但找到了关键 API：

- `GTA.Player.Character` - 获取玩家的 Ped
- `GTA.Game.Player` - 获取当前玩家
- `GTA.World.GetNextPositionOnStreet` - 两个重载（Vector2 和 Vector3）
- `GTA.World.GetSafeCoordForPed` - 获取安全坐标

现在我有足够的信息来设计：

核心 API 总结：
1. `GTA.UI.Notification.Show(string message, bool blinking = false)` - 显示通知
2. `GTA.World.CreateVehicle(GTA.Model model, GTA.Math.Vector3 position, float heading = 0)` - 创建车辆
3. `GTA.VehicleHash.Ambulance` - 救护车哈希
4. `GTA.Game.Player` - 当前玩家
5. `GTA.Player.Character` - 玩家的 Ped
6. `GTA.World.GetNextPositionOnStreet(Vector3 position, bool unoccupied = false)` - 获取街道上的下一个位置
7. `GTA.World.GetSafeCoordForPed(Vector3 position, bool sidewalk = true, int flags = 0)` - 获取安全坐标

对于 Ped.Position，我还需要确认。通常 Ped 有 Position 属性。但从查询结果中没有直接确认。不过 `Ped` 继承自 `Entity`，而 `Entity` 有 `Position` 属性。我可以假设这个存在，但严格来说没有被确认。让我标注为 needs_query 或 fallback。

实际上，从 SHVDN 的常用 API 来看，`Ped.Position` 应该是存在的。但根据规则，我不能凭记忆编造。让我看看是否可以在第一次查询中已经覆盖了。没有直接确认 Ped.Position，但我可以 fallback 使用 `Game.Player.Character.Position`，这在 SHVDN 中是标准的。

对于 Model 类：`CreateVehicle` 接受 `GTA.Model model`。`GTA.Model` 可以从 `VehicleHash` 构造，例如 `new Model(VehicleHash.Ambulance)` 或直接使用 `VehicleHash.Ambulance` 隐式转换。这在第一次查询中没有直接确认，但这是标准行为。

现在我可以构建 DESIGN.md 了。

关于礼物：用户说"任意礼物并在连击结束时"。根据 LIVE_STUDIO_CONTRACT_V1 第 7 条："默认一次性礼物效果仅在 repeatEnd=true 时触发；按 msgId 去重。" 这正好符合需求。

礼物查询返回 not_found（因为"任意礼物"不是具体礼物名称）。gift_requirements 表应反映这一点。

现在设计流程：

**聊天指令 backup：**
- 监听 WebcastChatMessage
- 检查 data.content（忽略大小写）是否为 "backup"
- 显示通知：如 "[昵称] 请求支援"
- 按用户冷却 3 秒：使用 Dictionary 记录每个用户最近一次触发时间

**礼物触发：**
- 监听 WebcastGiftMessage
- 仅在 data.repeatEnd = true 时触发
- 按 msgId 去重
- 全局冷却 5 秒
- 在玩家附近安全位置生成救护车

现在输出 DESIGN.md。

我还需要确认一点：关于 `Ped.Position`。我将在 capability_requirements 中标记为 fallback（因为未被直接确认但可以通过 `GetSafeCoordForPed` 配合玩家位置使用）。实际上，`Game.Player.Character` 返回 `Ped`，而 `Ped` 继承 `Entity`，应该有 `Position`。但我可以用 `World.GetSafeCoordForPed(Game.Player.Character.Position)` 来获取安全位置，其中 `Game.Player.Character.Position` 可能未被确认。让我标注为 needs_query。

不过实际上，我可以使用 `GetNextPositionOnStreet` 来获取玩家附近街道位置，这已被确认。

设计：
1. 获取玩家位置：`Game.Player.Character.Position`（fallback，但 SHVDN 标准中 Ped 有 Position 属性）
2. 使用 `World.GetNextPositionOnStreet(playerPosition)` 获取附近街道位置
3. 使用 `World.GetSafeCoordForPed(streetPosition, true, 0)` 获取安全坐标
4. 使用 `World.CreateVehicle(new Model(VehicleHash.Ambulance), safeCoord, heading)` 创建救护车

对于 Model，`new Model(VehicleHash.Ambulance)` 应该可行。但未确认 `Model` 构造函数接受 `VehicleHash`。让我标注为 fallback。

实际上在 SHVDN 中，`VehicleHash` 可以隐式转换为 `Model`。但这未在查询中确认。

让我简化：在 capability_requirements 中将未直接确认但普遍可用的标记为 fallback。

现在生成 DESIGN.md。
</think># DESIGN.md

## 需求摘要

| 项目 | 内容 |
|------|------|
| 玩法名称 | 观众互动支援模组 |
| 核心目标 | 观众通过聊天指令 `backup` 或赠送任意礼物（连击结束时）触发游戏内反馈，提升直播互动性 |
| 胜利/失败条件 | 无（纯互动效果，持续运行） |
| 时长/节奏 | 持续运行，无固定时长 |
| 正向互动 | ① 聊天 `backup` → 显示 "[昵称] 请求支援" 通知；② 任意礼物连击结束 → 玩家附近生成救护车 |
| 负向互动 | 无 |
| 原始用户输入 | 请生成一个 GTA5 LIVE Studio Mod：观众在聊天中输入 backup 时显示其昵称；观众赠送任意礼物并在连击结束时，在玩家附近生成一辆救护车。要求聊天按用户冷却 3 秒，礼物全局冷却 5 秒。 |

## 玩法流程

```
┌──────────────────────────────────────────────────┐
│                 Script 启动                        │
│  1. 连接 ws://127.0.0.1:60080                     │
│  2. 发送订阅结构                                   │
│  3. 启动消息接收循环（后台线程）                     │
│  4. Tick 消费 MainThreadDispatcher 队列            │
└──────────────────────────────────────────────────┘
                        │
          ┌─────────────┴─────────────┐
          ▼                           ▼
┌──────────────────┐       ┌──────────────────────┐
│ WebcastChatMessage│       │ WebcastGiftMessage    │
│ data.content      │       │ data.repeatEnd = true │
│ == "backup"(忽略  │       │ (连击结束)             │
│  大小写)           │       └──────────┬───────────┘
└────────┬─────────┘                  │
         ▼                            ▼
┌──────────────────┐       ┌──────────────────────┐
│ 按用户冷却 3 秒   │       │ 全局冷却 5 秒         │
│ (per-user dict)  │       │ (单一时间戳)           │
└────────┬─────────┘       └──────────┬───────────┘
         ▼                            ▼
┌──────────────────┐       ┌──────────────────────┐
│ 显示通知          │       │ 玩家附近生成救护车     │
│ "[昵称] 请求支援" │       │ (安全位置, 朝玩家方向) │
└──────────────────┘       └──────────────────────┘
```

## 状态机

```
States: IDLE → CONNECTING → CONNECTED → RUNNING
         │          │            │           │
         │    (连接失败/断开)      │     (持续处理消息)
         ▼          ▼            ▼           ▼
      IDLE ←── 重试延迟 5s    IDLE      ABORTED
                                       (Script.Aborted)
```

- **IDLE**: 初始状态，未连接
- **CONNECTING**: WebSocket 正在连接
- **CONNECTED**: 已连接并订阅，等待消息
- **RUNNING**: 正常接收和处理消息
- 连接断开时自动回到 IDLE 并延迟 5 秒重连

## 触发条件

### 聊天指令触发

| 条件 | 说明 |
|------|------|
| 事件类型 | `WebcastChatMessage` |
| 匹配字段 | `data.content`（字符串） |
| 匹配规则 | 忽略大小写，Trim 后精确等于 `"backup"` |
| 冷却策略 | **按用户冷却**：每个 `data.user.id` 独立计时 3 秒 |
| 冷却实现 | `Dictionary<string, DateTime>` 记录每用户最后触发时间 |
| busy_state_policy | 冷却中忽略该用户的新 backup 消息 |

### 礼物触发

| 条件 | 说明 |
|------|------|
| 事件类型 | `WebcastGiftMessage` |
| 匹配规则 | **任意礼物**（不限 gift_id、名称或价格） |
| 触发时机 | 仅当 `data.repeatEnd == true`（连击结束） |
| 去重策略 | 按 `data.common.msgId` 去重（HashSet 记录已处理 msgId） |
| 冷却策略 | **全局冷却** 5 秒（单一 `DateTime` 时间戳） |
| busy_state_policy | 冷却中忽略所有新的礼物连击结束事件 |

## 效果表

| 触发类型 | 效果 | 参数 | 说明 |
|----------|------|------|------|
| 聊天 `backup` | 显示通知 | `"[昵称] 请求支援"` | 使用 `data.user.nickname`；`GTA.UI.Notification.Show(message)` |
| 礼物连击结束 | 生成救护车 | `VehicleHash.Ambulance` | 在玩家附近安全街道位置生成；车头朝向玩家方向 |

### 救护车生成逻辑

1. 获取玩家当前位置：`Game.Player.Character.Position`
2. 调用 `World.GetNextPositionOnStreet(playerPosition, unoccupied: true)` 获取附近空街道位置
3. 若上一步返回零向量，fallback：在玩家位置偏移 (3, 0, 0) 处使用 `World.GetSafeCoordForPed`
4. 计算 heading 使车头朝向玩家
5. 调用 `World.CreateVehicle(model, safePosition, heading)` 生成车辆
6. 将车辆标记为持久（`vehicle.IsPersistent = false` 或让其自然消失）

## Capability Requirements

| Capability | Behavior | Priority | Status | Verified Evidence | Fallback |
|------------|----------|----------|--------|-------------------|----------|
| `GTA.UI.Notification.Show(string, bool)` | 显示游戏内通知 | MUST | verified | SHVDN: `static System.Int32 Show(System.String message, System.Boolean blinking = )` score=88 | 无 |
| `GTA.World.CreateVehicle(Model, Vector3, float)` | 生成车辆 | MUST | verified | SHVDN: `static GTA.Vehicle CreateVehicle(GTA.Model model, GTA.Math.Vector3 position, System.Single heading = )` score=88 | 无 |
| `GTA.VehicleHash.Ambulance` | 救护车模型哈希 | MUST | verified | SHVDN: `literal: GTA.VehicleHash.Ambulance` score=88 | 无 |
| `GTA.World.GetNextPositionOnStreet(Vector3, bool)` | 获取玩家附近街道位置 | MUST | verified | SHVDN: `static GTA.Math.Vector3 GetNextPositionOnStreet(GTA.Math.Vector3 position, System.Boolean unoccupied = )` score=88 | `GetSafeCoordForPed` |
| `GTA.World.GetSafeCoordForPed(Vector3, bool, int)` | 获取步行安全坐标 | SHOULD | verified | SHVDN: `static GTA.Math.Vector3 GetSafeCoordForPed(GTA.Math.Vector3 position, System.Boolean sidewalk = , System.Int32 flags = )` score=88 | 直接使用偏移位置 |
| `GTA.Game.Player` | 获取当前玩家 | MUST | verified | SHVDN: `static GTA.Player Player { get; }` score=88 | 无 |
| `GTA.Player.Character` | 获取玩家 Ped | MUST | verified | SHVDN: `GTA.Ped Character { get; }` score=88 | 无 |
| `Ped.Position` (Entity.Position) | 获取玩家坐标 | MUST | needs_query | 未直接确认；Entity 基类属性，SHVDN 标准 | 使用 `GetSafeCoordForPed` 以零向量为输入并依赖其内部逻辑 |
| `Model` 从 `VehicleHash` 构造 | 模型类型转换 | MUST | needs_query | 未直接确认隐式转换或构造函数 | 若编译失败，使用 `new Model((int)VehicleHash.Ambulance)` 或 `Function.Call<int>(Hash.GET_HASH_KEY, "ambulance")` |
| WebSocket `ClientWebSocket` | WebSocket 连接 | MUST | needs_query | 未查询；.NET Framework 4.8 内置 `System.Net.WebSockets.ClientWebSocket` | 无替代 |
| `JavaScriptSerializer` | JSON 解析 | MUST | needs_query | 未查询；.NET Framework 4.8 内置 `System.Web.Script.Serialization.JavaScriptSerializer` | 无替代 |
| `MainThreadDispatcher` / `Script.Tick` | 主线程调度 | MUST | needs_query | 未查询；LIVE Studio 模板标准组件 | 手动实现队列 + Tick 消费 |

## LIVE Studio 事件与礼物映射

### 事件 1: WebcastChatMessage（backup 指令）

| 字段 | 路径 | 用途 |
|------|------|------|
| 消息 ID | `data.common.msgId` | 日志/调试 |
| 用户 ID | `data.user.id` | 冷却字典 Key |
| 用户昵称 | `data.user.nickname` | 通知内容 |
| 聊天内容 | `data.content` | 匹配 "backup" |
| 事件方法标识 | `data.common.method` | 区分事件类型（`WebcastChatMessage`） |

- **repeat_policy**: 每条消息独立处理（无连击概念）
- **dedupe_policy**: 无需去重（聊天消息不重复）；可用 msgId 做幂等保护
- **cooldown**: 按用户 3 秒（`Dictionary<userId, lastTriggerTime>`）
- **queue_cap**: 10（冷却中的消息丢弃）
- **busy_state_policy**: 冷却中忽略

### 事件 2: WebcastGiftMessage（任意礼物连击结束）

| 字段 | 路径 | 用途 |
|------|------|------|
| 消息 ID | `data.common.msgId` | 去重 Key |
| 用户 ID | `data.user.id` | 日志记录 |
| 用户昵称 | `data.user.nickname` | 日志记录 |
| 礼物 ID | `data.gift.id` | 日志记录 |
| 礼物名称 | `data.gift.name` | 日志记录 |
| 钻石数 | `data.gift.diamondCount` | 日志记录 |
| 连击结束标志 | `data.repeatEnd` | **必须为 true 才触发** |
| 连击次数 | `data.comboCount` | 日志记录 |
| 事件方法标识 | `data.common.method` | 区分事件类型（`WebcastGiftMessage`） |

- **repeat_policy**: 仅 `repeatEnd == true` 时触发（LIVE_STUDIO_CONTRACT_V1 §7）
- **dedupe_policy**: 按 `msgId` 去重（HashSet 缓存已处理的 msgId，容量上限 500）
- **cooldown**: 全局 5 秒
- **queue_cap**: 5（冷却和去重后的有效事件排队）
- **busy_state_policy**: 全局冷却中丢弃

### Gift Requirements

| 字段 | 值 |
|------|-----|
| original_query | 任意礼物 |
| keywords | any gift, all gifts |
| status | not_found |
| confirmed_name | N/A |
| gift_id | N/A |
| diamondCount | N/A |
| candidates | 空 |
| fallback | 不依赖特定礼物映射；监听所有 WebcastGiftMessage，仅使用 `data.repeatEnd` 判定触发 |

## 计时与冷却

| 冷却类型 | 范围 | 时长 | 实现方式 |
|----------|------|------|----------|
| 聊天 backup | 按用户 | 3 秒 | `Dictionary<string, DateTime>`; 每次触发后更新；检查时比对 `DateTime.UtcNow` |
| 礼物生成 | 全局 | 5 秒 | `DateTime _lastGiftTriggerTime`; 触发后更新 |

- 冷却检查在 `MainThreadDispatcher.Enqueue` 的回调中执行（已在主线程）
- 时间比较使用 `(DateTime.UtcNow - lastTime).TotalSeconds >= cooldownSeconds`

## 失败与清理

### 玩家死亡处理
- 聊天 backup：正常显示通知（玩家死亡不影响通知）
- 礼物生成救护车：若玩家死亡，仍在玩家最后已知位置生成救护车

### WebSocket 断开
- 捕获异常，设置状态为 IDLE
- 清理现有连接资源
- 延迟 5 秒后重连
- 重连后重新发送订阅结构

### Script.Aborted 清理
- 取消 WebSocket 接收循环（CancellationTokenSource.Cancel）
- 调用 `webSocket.Abort()` 并 Dispose
- 清空 MainThreadDispatcher 队列
- 清空冷却字典和去重 HashSet
- 不删除已生成的车辆（自然消失）
- 不修改玩家状态（武器、通缉等）

## 降级方案

| 场景 | 降级策略 |
|------|----------|
| WebSocket 无法连接 | 每 5 秒重试；通知用户"LIVE Studio 连接中..."（仅一次） |
| JSON 解析失败 | 记录日志，跳过该消息 |
| 救护车生成位置不安全 | 使用多级 fallback：`GetNextPositionOnStreet` → `GetSafeCoordForPed` → 玩家位置偏移 (5, 0, 0) |
| 救护车模型无效 | 尝试 `VehicleHash.Burrito` 作为备选模型 |
| 通知显示失败 | 静默忽略（通知 API 高度可靠） |
| 冷却字典/去重集合内存增长 | 定期清理超过 60 秒未活跃的冷却条目；msgId HashSet 上限 500，超出时移除最旧条目 |

## 验收标准

1. ✅ 观众发送 `backup` 聊天消息后，游戏内显示 "[昵称] 请求支援" 通知
2. ✅ 同一观众在 3 秒内重复发送 `backup`，仅第一次触发
3. ✅ 不同观众可在各自冷却期外独立触发 backup
4. ✅ 观众赠送任意礼物并在 `repeatEnd=true` 时，玩家附近生成一辆救护车
5. ✅ 礼物触发遵循全局 5 秒冷却
6. ✅ 礼物触发按 msgId 去重，同一连击不会重复生成
7. ✅ 救护车生成在玩家附近可通行的街道位置
8. ✅ Script 卸载时清理所有资源，不残留影响
9. ✅ WebSocket 断线自动重连
10. ✅ 大小写不敏感匹配 `backup`（如 `Backup`、`BACKUP` 均可触发）