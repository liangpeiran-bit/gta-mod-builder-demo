<think>
用户需求分析：

1. **聊天功能**：观众在聊天中输入"hello"时，显示其昵称。
2. **礼物功能**：观众赠送任意礼物并在连击结束时，在玩家附近生成一辆警车。

这是一个 NEW MODE，没有上一版 DESIGN.md 或 Mod.cs。

根据 LIVE_STUDIO_CONTRACT_V1：
- WebcastChatMessage：data.common.method 需要确认，但根据契约，聊天字段在 data.content，用户信息在 data.user.id 和 data.user.nickname。
- WebcastGiftMessage：在 repeatEnd=true 时触发。礼物字段：data.gift.id, data.gift.name, data.gift.diamondCount, data.repeatCount, data.repeatEnd, data.comboCount。

现在需要调用工具：

1. **SHVDN API 查询**：需要确认以下能力：
   - 显示通知/字幕 (Notification.Show, GTA.UI.Notification)
   - 生成车辆 (World.CreateVehicle, VehicleHash)
   - 获取玩家位置 (Game.Player.Character.Position)
   - 警车哈希值 (VehicleHash.Police, VehicleHash.Police2 等)
   - MainThreadDispatcher（用于后台线程到主线程的调度）

   这些需要分成两批查询。第一批：核心能力（Notification, CreateVehicle, Player Position）。第二批：VehicleHash 枚举值和 MainThreadDispatcher。

2. **礼物查询**：用户说"任意礼物"，不需要特定礼物映射，但根据规则：用户没有指定具体礼物名称，只有"任意礼物"。这不需要调用 query_live_gifts，因为用户没有指定具体的礼物名称、gift_id 或价格条件。礼物效果是"任意礼物"触发，所以 gift_requirements 标记为 not_required？等等，用户确实有礼物意图——"赠送任意礼物并在连击结束时"。但用户没有指定具体的礼物名称/ID。规则说："当原始用户输入包含任何可能的礼物意图时调用 query_live_gifts，包括明确名称、别名、错拼、模糊描述、数字 gift_id、价格条件，或玩法必须依赖礼物映射。"

   但"任意礼物"不需要映射到具体礼物，因为效果对所有礼物都触发。礼物工具是用来确认特定礼物的。这里用户没有指定礼物细节，所以不需要礼物映射。gift_requirements 可以标记为 not_required 吗？再看规则："礼物是可选的。用户没有指定礼物时，不要求补充礼物，不编造 gift_id"。用户确实指定了"任意礼物"作为触发条件，但这不需要具体映射。我觉得可以标记为 not_required，因为不需要确认任何特定礼物映射。效果基于 data.gift.id 的任意值触发。

   但等等，规则也说 "礼物工具本节点最多调用 1 次"。如果用户说"任意礼物"，我们不需要查，因为没有具体名称。但如果用户说"玫瑰花"就需要查。这里"任意礼物"不需要查。

   所以 gift_requirements 状态为 not_required。

现在调用 SHVDN API：

第一批：核心能力
- Notification.Show
- World.CreateVehicle
- Game.Player.Character.Position
- VehicleHash 枚举（警车相关）

第二批：MainThreadDispatcher, Script.Tick, Script.Aborted

让我构造查询。

第一批 query: "Notification.Show World.CreateVehicle Game.Player.Character.Position VehicleHash police car spawn"

第二批 query: "MainThreadDispatcher Script.Tick Script.Aborted GTA.Script"

注意：每批最多256字符。

第一批：
"Notification.Show World.CreateVehicle Game.Player.Character.Position VehicleHash police"

这应该覆盖核心能力。

第二批：
"MainThreadDispatcher Enqueue Script.Tick Script.Aborted"

开始调用。
</think>现在开始分析需求并调用工具。需要确认核心 SHVDN 能力，以及礼物是否需要映射。

先并行调用 SHVDN 第一批核心能力和礼物查询：
<think>
第一批 SHVDN 结果很好，获得了核心能力：

1. `GTA.Player.Character` - 获取玩家 Ped，进而获取 Position
2. `GTA.World.CreateVehicle(GTA.Model model, GTA.Math.Vector3 position, System.Single heading)` - 创建车辆
3. `GTA.Game.Player` - 获取玩家
4. `GTA.UI.Notification.Show(...)` - 两个重载，一个简单消息，一个带 icon/sender/subject
5. `GTA.VehicleHash` 枚举类型
6. `GTA.VehicleHash.Police` 枚举值

现在需要查询第二批：MainThreadDispatcher、Script.Tick、Script.Aborted、以及 Vector3。

另外礼物查询返回 invalid_input，因为"任意礼物"不是有效的搜索文本。这没关系，用户说的是"任意礼物"，不需要特定礼物映射。gift_requirements 标记为 not_required。

现在进行第二批 SHVDN 查询：

query: "MainThreadDispatcher Enqueue Script.Tick Script.Aborted Vector3 Ped Position"

这涵盖了后台线程调度和脚本生命周期。
</think>第一批 SHVDN 核心能力已确认。礼物查询对"任意礼物"返回 invalid_input，因为无需特定映射。现在查询第二批 SHVDN 能力（线程调度和脚本生命周期）：
<think>
第二批 SHVDN 返回了：
- `GTA.Script.Abort()` 
- `GTA.Script.Aborted` event
- `GTA.Script.Tick` event

但没有返回 MainThreadDispatcher。这是 SHVDN v3 中的关键组件。我需要检查是否有其他名称。MainThreadDispatcher 可能在 SHVDN v3 中不存在，或者名称不同。在 SHVDN v3 中，通常使用 `Script.MainThreadDispatcher` 或类似方式，但常见模式是使用队列和 Tick 事件。

查看已有的结果，Script.Tick 事件可以用来在每帧执行排队的操作。LIVE_STUDIO_CONTRACT_V1 要求使用 MainThreadDispatcher.Enqueue。但 SHVDN v3 可能没有内置 MainThreadDispatcher，需要自己实现。

不过在 SHVDN v3 中，常见做法是使用 `GTA.Script` 的 `Tick` 事件和队列。MainThreadDispatcher 可能是一个自定义类。如果 SHVDN 没有返回，我们可能需要自己实现一个简单的队列机制。

让我标记 MainThreadDispatcher 为 needs_query，但提供 fallback：使用 ConcurrentQueue + Script.Tick 消费。

现在汇总所有能力，输出 DESIGN.md。

关键设计点：

1. **WebSocket 连接**：ws://127.0.0.1:60080，订阅结构按契约。
2. **聊天处理**：data.common.method 为 WebcastChatMessage，读取 data.content，如果内容包含 "hello"（大小写不敏感），显示昵称。
3. **礼物处理**：data.common.method 为 WebcastGiftMessage，仅在 data.repeatEnd=true 时触发，生成警车。
4. **警车生成**：使用 World.CreateVehicle，模型为 VehicleHash.Police，位置为玩家附近。

能力汇总：

| capability | behavior | priority | status | verified_evidence | fallback |
|---|---|---|---|---|---|
| Notification.Show | 显示观众昵称 | required | verified | GTA.UI.Notification.Show(string, bool) | - |
| World.CreateVehicle | 生成警车 | required | verified | static GTA.Vehicle CreateVehicle(GTA.Model, Vector3, float) | - |
| Game.Player.Character | 获取玩家位置 | required | verified | GTA.Ped Character { get; } | - |
| VehicleHash.Police | 警车模型 | required | verified | GTA.VehicleHash.Police | VehicleHash.Police2 |
| Script.Tick | 消费主线程队列 | required | verified | System.EventHandler Tick | - |
| Script.Aborted | 清理资源 | required | verified | System.EventHandler Aborted | - |
| MainThreadDispatcher | 后台线程到主线程调度 | required | needs_query | 未在 SHVDN 中确认 | ConcurrentQueue + Script.Tick |

礼物需求表：

| field | value |
|---|---|
| original_query | 任意礼物 |
| keywords | - |
| status | not_required |
| confirmed_name | - |
| gift_id | - |
| diamondCount | - |
| candidates | - |
| fallback | 所有礼物均触发，不区分 gift_id |

现在编写完整的 DESIGN.md。

需要注意：
- "hello" 匹配：大小写不敏感，精确匹配还是包含？用户说"输入 hello"，理解为输入内容等于或包含 "hello"。保守设计：包含 "hello"（不区分大小写）。
- 警车生成位置：玩家附近，例如前方 5 米。
- 冷却：聊天和礼物都需要冷却，防止刷屏和刷车。
- 去重：按 msgId。
- 队列上限。
- 玩家死亡处理。
- 清理：Script.Aborted 时关闭 WebSocket，清理生成的车辆？用户没要求清理车辆，但按契约需要设计清理。不过警车是生成的实体，一般不需要特殊清理。但应记录。

状态机：简单，两个独立功能，没有复杂状态机。

让我来写 DESIGN.md。

关于 MainThreadDispatcher：在 SHVDN v3 中，常见做法是自定义一个静态类，使用 ConcurrentQueue<Action> 和 Script.Tick 消费。这作为 fallback。

现在输出 DESIGN.md。
</think>所有工具调用已完成。现在汇总确认的能力，输出 DESIGN.md。

# DESIGN.md

## 需求摘要
- **玩法名称**：聊天互动与礼物警车
- **核心目标**：
  1. 观众在聊天中输入 `hello`（大小写不敏感）时，游戏内显示该观众昵称。
  2. 观众赠送任意礼物并在连击结束时（`repeatEnd=true`），在玩家附近生成一辆警车。
- **胜利条件**：不适用
- **失败条件**：无
- **时长/节奏**：不指定
- **正向互动**：聊天指令显示昵称；礼物连击结束生成警车
- **负向互动**：无
- **礼物要求**：已指定（任意礼物，不区分具体 gift_id）
- **评论或指令触发**：聊天 `hello` 触发显示昵称；礼物 `repeatEnd` 触发警车生成

---

## 玩法流程

```
┌──────────────────────────────────────────────────────┐
│                   WebSocket 连接                      │
│                ws://127.0.0.1:60080                   │
│  订阅: IM_MESSAGE_TRANSPORT                          │
└────────┬──────────────────────────────┬──────────────┘
         │                              │
         ▼                              ▼
┌─────────────────────┐    ┌─────────────────────────┐
│  WebcastChatMessage  │    │   WebcastGiftMessage     │
│  data.common.method  │    │   data.common.method     │
└────────┬────────────┘    └──────────┬──────────────┘
         │                            │
         ▼                            ▼
   data.content               data.repeatEnd == true?
   包含 "hello"?                    │
         │                    Yes   │    No
    Yes  │   No                ┌────┘    └──> 忽略
         │    │                ▼
         ▼    ▼        生成警车于玩家附近
   Notification.Show   (World.CreateVehicle +
   显示昵称             VehicleHash.Police)
```

---

## 状态机

本 Mod 无复杂状态机，两个功能独立并行。脚本级状态仅包含：

| 状态 | 描述 |
|------|------|
| `Connecting` | WebSocket 连接建立中 |
| `Connected` | 已订阅，正常接收事件 |
| `Disconnected` | 连接断开/脚本中止，进入清理 |

---

## 触发条件

### 聊天触发

| 条件 | 值 |
|------|-----|
| 事件类型 | `WebcastChatMessage`（`data.common.method`） |
| 匹配规则 | `data.content` 包含 `hello`（大小写不敏感，`OrdinalIgnoreCase`） |
| 去重 | 按 `data.common.msgId` 去重，已处理 msgId 直接丢弃 |
| 冷却 | 单用户（`data.user.id`）3 秒内重复触发忽略 |
| 全局冷却 | 无额外全局冷却（单用户冷却已足够） |

### 礼物触发

| 条件 | 值 |
|------|-----|
| 事件类型 | `WebcastGiftMessage`（`data.common.method`） |
| 匹配规则 | `data.repeatEnd == true`（连击结束才触发） |
| 礼物范围 | 任意 `data.gift.id`，不区分礼物类型 |
| 去重 | 按 `data.common.msgId` 去重 |
| 冷却 | 全局 5 秒冷却（两次警车生成间隔 ≥ 5s） |

---

## 效果表

| 效果 | 触发 | 实现 | 参数 |
|------|------|------|------|
| 显示昵称 | 聊天 `hello` | `GTA.UI.Notification.Show(string)` | 消息格式：`"观众 {nickname} 向你打招呼！"` |
| 生成警车 | 礼物连击结束 | `GTA.World.CreateVehicle(VehicleHash.Police, pos, heading)` | 玩家前方 5 米，朝向玩家面向 |

---

## capability_requirements

| capability | behavior | priority | status | verified_evidence | fallback |
|------------|----------|----------|--------|-------------------|----------|
| `GTA.UI.Notification.Show` | 显示观众昵称通知 | required | verified | `static System.Int32 Show(System.String message, System.Boolean blinking = )` (score:88) | — |
| `GTA.World.CreateVehicle` | 在玩家附近生成警车 | required | verified | `static GTA.Vehicle CreateVehicle(GTA.Model model, GTA.Math.Vector3 position, System.Single heading = )` (score:88) | — |
| `GTA.Game.Player` | 获取本地玩家 | required | verified | `static GTA.Player Player { get; }` (score:88) | — |
| `GTA.Player.Character` | 获取玩家 Ped 以读取位置 | required | verified | `GTA.Ped Character { get; }` (score:88) | — |
| `GTA.VehicleHash.Police` | 警车模型枚举 | required | verified | `GTA.VehicleHash.Police` (score:88) | `VehicleHash.Police2` |
| `GTA.Script.Tick` | 消费主线程队列 | required | verified | `System.EventHandler Tick` (score:88) | — |
| `GTA.Script.Aborted` | 脚本中止清理 | required | verified | `System.EventHandler Aborted` (score:88) | — |
| `GTA.Script.Abort` | 主动中止脚本 | required | verified | `System.Void Abort()` (score:88) | — |
| `MainThreadDispatcher` | 后台线程→主线程调度 | required | needs_query | 未在 SHVDN 查询中确认 | 自定义 `ConcurrentQueue<Action>` + `Script.Tick` 消费 |
| `GTA.Math.Vector3` | 生成位置计算 | required | needs_query | 未独立查询，但 `CreateVehicle` 签名已确认需要 `Vector3` | 使用 `new Vector3(x, y, z)` 构造 |
| `GTA.Ped.Position` | 读取玩家坐标 | required | needs_query | 未独立查询，但 `Character` 返回 `Ped`，`Ped` 继承 `Entity` 应有 `Position` | 使用 `Game.Player.Character.Position` |

---

## LIVE Studio 事件与礼物映射

### WebSocket 连接

| 配置项 | 值 |
|--------|-----|
| 地址 | `ws://127.0.0.1:60080` |
| 订阅消息 | `{"type":"subscribe","id":"gta-mod","data":{"type":"serviceSignalSub","name":"IM_MESSAGE_TRANSPORT"}}` |
| JSON 解析 | `System.Web.Script.Serialization.JavaScriptSerializer` (.NET Framework 4.8) |

### 聊天事件

| 字段 | LIVE Studio 路径 |
|------|------------------|
| event_method | `data.common.method` = `WebcastChatMessage` |
| msgId | `data.common.msgId` |
| userId | `data.user.id` |
| nickname | `data.user.nickname` |
| content | `data.content` |
| repeat_policy | 不适用（聊天无连击） |
| dedupe_policy | 按 `msgId` 去重，已处理集合存储 |
| cooldown | 单用户 3s（`userId` → 上次触发时间） |
| queue_cap | 100（超出丢弃最旧） |
| busy_state_policy | 排队处理，不丢弃 |
| abort_cleanup | 清空去重集合和冷却字典 |

### 礼物事件

| 字段 | LIVE Studio 路径 |
|------|------------------|
| event_method | `data.common.method` = `WebcastGiftMessage` |
| msgId | `data.common.msgId` |
| userId | `data.user.id` |
| nickname | `data.user.nickname` |
| giftId | `data.gift.id` |
| giftName | `data.gift.name` |
| diamondCount | `data.gift.diamondCount` |
| repeatCount | `data.repeatCount` |
| repeatEnd | `data.repeatEnd` |
| comboCount | `data.comboCount` |
| repeat_policy | **仅 `repeatEnd=true` 时触发**；中间连击消息忽略 |
| dedupe_policy | 按 `msgId` 去重 |
| cooldown | 全局 5s（上次警车生成时间） |
| queue_cap | 50（超出丢弃最旧） |
| busy_state_policy | 冷却期内新触发丢弃 |
| abort_cleanup | 清空去重集合和冷却时间戳 |

---

## gift_requirements

| 字段 | 值 |
|------|-----|
| original_query | 任意礼物 |
| keywords | — |
| status | not_required |
| confirmed_name | — |
| gift_id | — |
| diamondCount | — |
| candidates | — |
| fallback | 不区分 gift_id；所有 WebcastGiftMessage 在 repeatEnd=true 时均触发警车生成 |

---

## 计时与冷却

| 冷却类型 | 范围 | 时长 | 行为 |
|----------|------|------|------|
| 聊天冷却 | 单用户（`userId`） | 3 秒 | 同一用户 3 秒内再次发送 `hello` 忽略 |
| 礼物冷却 | 全局 | 5 秒 | 两次警车生成间隔 ≥ 5s，冷却期内 `repeatEnd` 事件丢弃 |
| 去重窗口 | 全局（msgId） | 永久（脚本生命周期） | 已处理 msgId 直接丢弃 |

---

## 失败与清理

### 玩家死亡处理
- 聊天通知正常显示（通知不受玩家死亡影响）
- 警车生成正常执行（可在死亡位置生成，无影响）

### Script.Aborted 清理

| 资源 | 清理方式 |
|------|----------|
| WebSocket | 取消接收回调 → `CloseAsync` → `Abort` / `Dispose`（后台线程，禁止 `.Wait()` 阻塞主线程） |
| 去重集合 | `Clear()` |
| 冷却字典 | `Clear()` |
| 主线程队列 | `ConcurrentQueue<Action>` 清空 |
| 生成警车 | **不主动删除**（属于游戏世界自然实体，玩家可正常使用或遗弃） |

### 异常处理
- WebSocket 断线：静默重连（间隔 3s），期间事件丢弃
- JSON 解析失败：记录日志，丢弃该消息
- 警车生成失败（如模型无效）：`Notification.Show("警车生成失败")`，不中断脚本

---

## 降级方案

| 场景 | 降级行为 |
|------|----------|
| `MainThreadDispatcher` 不可用 | 使用 `ConcurrentQueue<Action>` + `Script.Tick` 轮询消费 |
| `VehicleHash.Police` 不可用 | 尝试 `VehicleHash.Police2`，再失败则 `VehicleHash.Police3` |
| 玩家位置读取失败 | 使用 `Vector3.Zero` 作为 fallback 位置 |
| WebSocket 连接失败 | 每 3s 重试，期间所有事件静默丢弃 |
| `JavaScriptSerializer` 不可用 | fallback 为手动字符串解析（仅提取必要字段） |

---

## 验收标准

1. ✅ 观众发送 `hello` → 游戏内显示 `"观众 {昵称} 向你打招呼！"`
2. ✅ 观众发送 `Hello` / `HELLO` / `heLLo` → 同效果（大小写不敏感）
3. ✅ 观众发送不包含 `hello` 的消息 → 不触发
4. ✅ 同一观众 3 秒内重复发送 `hello` → 仅第一次触发
5. ✅ 同一 msgId 重复到达 → 仅处理一次
6. ✅ 观众赠送任意礼物且 `repeatEnd=true` → 玩家前方 5 米生成一辆警车
7. ✅ 礼物连击中间消息（`repeatEnd=false`）→ 不触发
8. ✅ 两次警车生成间隔 ≥ 5 秒
9. ✅ 脚本中止（`Aborted`）→ WebSocket 关闭，队列清空，不残留后台线程
10. ✅ 玩家死亡期间 → 功能正常，聊天通知可见，警车正常生成