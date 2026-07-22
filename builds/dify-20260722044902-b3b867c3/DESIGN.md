<think>
用户需求很清晰：

1. 聊天触发：观众输入 "hello" → 显示观众昵称
2. 礼物触发：任意礼物连击结束（repeatEnd=true）→ 在玩家附近生成一辆警车

需要查询：
- SHVDN API：Notification（显示昵称）、CreateVehicle、VehicleHash、Police car（警察车辆）、玩家附近位置
- query_live_gifts：任意礼物，但这是"任意礼物"通配，不需要特定礼物。不过用户说"任意礼物"，这意味着不需要特定礼物映射。但根据规则，"当原始用户输入包含任何可能的礼物意图时调用 query_live_gifts"。这里用户确实提到了礼物，但是"任意礼物" - 所有礼物都适用。我需要确认是否需要调用礼物工具。规则说："用户没有指定礼物时，不要求补充礼物...用户没有礼物意图时不得调用"。但这里用户明确说"任意礼物"，这意味着所有礼物都应该触发。所以实际上不需要特定的礼物映射——所有礼物都会被处理。gift_requirements 可以标记为 something like "all_gifts" 或 "wildcard"。

让我重新读规则："当原始用户输入包含任何可能的礼物意图时调用 query_live_gifts，包括明确名称、别名、错拼、模糊描述、数字 gift_id、价格条件，或玩法必须依赖礼物映射。用户没有礼物意图时不得调用，gift_requirements 标记为 not_required。"

用户说"任意礼物"——这不算明确名称、别名、错拼、模糊描述、数字 gift_id、价格条件。这是通配符。但"玩法必须依赖礼物映射"吗？不，这里不需要特定映射。所以可能不需要调用礼物工具。但为了安全，我还是考虑一下...实际上"任意礼物"意味着所有礼物都触发同样的效果，所以不需要特定礼物映射。gift_requirements 应该标记为 wildcard 或 all_gifts。

现在考虑 SHVDN 查询。需要：
1. 显示昵称 → Notification.Show 或 UI.Notify
2. 生成警车 → CreateVehicle, VehicleHash, police car models
3. 玩家附近位置 → Game.Player.Character.Position, World.GetNextPositionOnStreet

分两批：
第一批：核心能力 - Notification, CreateVehicle, VehicleHash police
第二批：World.GetNextPositionOnStreet, Player position

让我调用 SHVDN。首先查核心能力。
</think>
<think>
第一批结果返回了：
- GTA.World.CreateVehicle - 确认
- GTA.UI.Notification.Show - 有两个重载，一个带 message，一个带 icon/sender/subject/message
- GTA.VehicleHash 枚举
- GTA.VehicleHash.Police - 确认警察车
- GTA.Native.Hash.CREATE_VEHICLE

但还需要确认：POLICE2, POLICE3 是否存在？还有需要获取玩家位置的能力。另外还需要 VehicleHash 中更多的警车模型。

还需要查询：
- Game.Player.Character.Position
- World.GetNextPositionOnStreet 或类似方法
- VehicleHash.Police2, Police3, Police4, Riot 等

第二批查询。
</think>
<think>
好的，两批SHVDN查询都完成了。现在汇总所有确认的能力：

确认的能力：
- GTA.UI.Notification.Show(string message, bool blinking = false) - 显示通知
- GTA.UI.Notification.Show(NotificationIcon icon, string sender, string subject, string message, bool fadeIn, bool blinking) - 带参数的通知
- GTA.World.CreateVehicle(Model model, Vector3 position, float heading) - 创建车辆
- GTA.VehicleHash 枚举
  - GTA.VehicleHash.Police
  - GTA.VehicleHash.Police2
  - GTA.VehicleHash.Police3
  - GTA.VehicleHash.Police4
  - GTA.VehicleHash.Riot
- GTA.Game.Player (property)
- GTA.Player.Character (property, returns Ped)
- GTA.World.GetNextPositionOnStreet(Vector3 position, bool unoccupied) 和 Vector2 重载

还需要 Ped.Position 属性。虽然没有直接确认 Ped.Position，但从 Character 返回 Ped，Ped 有 Position 属性是 SHVDN 基本类型的一部分。这可以标记为 inferred 或 needs_query。

其实从第一批查询可以看到，我们没有直接查询 Ped.Position。但这是非常基础的属性。让我检查是否需要再次查询。

实际上，Ped.Position 在 SHVDN 中是 GTA.Entity.Position（Ped 继承 Entity）。这太基础了，通常总是可用的。我可以标记为 verified 基于常识，但实际上工具没有确认。保守做法是标记为 inferred。

另外还需要确认：MainThreadDispatcher - 这是 LIVE Studio 模板中的，不是 SHVDN 的一部分。它是项目模板自带的。

关于礼物：用户说"任意礼物"，所以 gift_requirements 应该标记为 wildcard/all_gifts。不需要调用 query_live_gifts，因为没有特定的礼物名称/ID/价格条件需要查询。

现在来构建 DESIGN.md。

核心设计：
1. WebSocket 连接到 ws://127.0.0.1:60080
2. 订阅 IM_MESSAGE_TRANSPORT
3. 接收 WebcastChatMessage：检查 data.content 是否包含 "hello"（大小写不敏感），显示 Notification 包含 data.user.nickname
4. 接收 WebcastGiftMessage：当 data.repeatEnd == true 时，在玩家附近生成警车

关于警车选择：用户说"一辆警车"，可以用 VehicleHash.Police 作为默认。也可以随机选择。我选择随机从 Police, Police2, Police3, Police4 中选择一个。Riot 是防暴车，不太算"警车"，但可以包含。

状态机和冷却：
- 聊天 hello：每次独立触发，但需要冷却防止刷屏（如 3 秒冷却）
- 礼物警车：每次 repeatEnd 触发，冷却如 5 秒，防止短时间内大量警车

清理：
- Script.Aborted 时关闭 WebSocket
- 不删除已有车辆（不需要清理生成的警车，因为那是游戏世界的一部分）

现在让我构建完整的 DESIGN.md。
</think># DESIGN.md

## 需求摘要
- **玩法名称**: 观众互动 Mod（Hello + 警车礼物）
- **核心目标**: 观众在聊天中输入 `hello` 时以游戏内通知显示其昵称；观众赠送任意礼物并在连击结束（`repeatEnd=true`）时，在玩家附近道路上生成一辆随机警车。
- **胜利条件**: 无（纯互动展示）
- **失败条件**: 无
- **时长/节奏**: 实时响应，带冷却防刷
- **正向互动**: 聊天 hello → 昵称通知；礼物连击结束 → 警车生成
- **负向互动**: 无
- **礼物要求**: 任意礼物（通配），仅 `repeatEnd=true` 触发
- **评论或指令触发**: 评论精确匹配触发（`hello`，大小写不敏感）；礼物连击结束触发

---

## 玩法流程

### 流程 A：聊天 Hello → 显示昵称
1. 观众发送聊天消息，WebSocket 收到 `WebcastChatMessage`
2. 提取 `data.content`，trim 后做大小写不敏感比较
3. 若内容等于 `"hello"`（整条消息，非子串）：
   - 检查冷却（默认 3 秒 per 用户 / 全局可选）
   - 通过 `MainThreadDispatcher` 在主线程调用 `GTA.UI.Notification.Show($"🎉 {nickname} says Hello!")`
   - 记录最后触发时间
4. 非 `hello` 消息：忽略

### 流程 B：礼物连击结束 → 生成警车
1. 观众赠送礼物，WebSocket 收到 `WebcastGiftMessage`
2. 检查 `data.repeatEnd` 字段
3. 若 `repeatEnd == true`：
   - 检查全局冷却（默认 5 秒）
   - 按 `data.common.msgId` 去重（防止重复处理同一条消息）
   - 获取玩家当前位置 → 使用 `World.GetNextPositionOnStreet` 获取附近道路坐标
   - 从 `{Police, Police2, Police3, Police4}` 随机选取一个 `VehicleHash`
   - 通过 `MainThreadDispatcher` 在主线程调用 `World.CreateVehicle`
   - 可选：通知提示 `"🚔 {nickname} 的连击礼物召唤了一辆警车！"`
4. 若 `repeatEnd == false`：忽略（连击进行中，不逐条处理）

---

## 状态机

```
┌──────────┐    WebSocket 连接成功    ┌─────────────┐
│  INIT    │ ─────────────────────▶ │  CONNECTED  │
└──────────┘                         └──────┬──────┘
                                            │ 订阅 IM_MESSAGE_TRANSPORT
                                            ▼
                                       ┌─────────────┐
                                       │  LISTENING  │◀── 循环
                                       └──┬──────┬───┘
                                  WebcastChatMessage  WebcastGiftMessage
                                         │                  │
                                         ▼                  ▼
                                   ┌──────────┐     ┌──────────────┐
                                   │ CHAT_HANDLE│    │ GIFT_HANDLE  │
                                   │ (检查hello)│    │ (检查repeatEnd)│
                                   └────┬─────┘     └──────┬───────┘
                                        │                   │
                                   match hello        repeatEnd==true
                                        │                   │
                                        ▼                   ▼
                                   ┌──────────┐     ┌──────────────┐
                                   │ NOTIFY   │     │ SPAWN_POLICE │
                                   │ 昵称通知  │     │ 生成警车     │
                                   └────┬─────┘     └──────┬───────┘
                                        │                   │
                                        └─────────┬─────────┘
                                                  ▼
                                           返回 LISTENING
```

- 任何状态发生 WebSocket 断开 → 重连（可选降级：仅日志）
- `Script.Aborted` → `SHUTDOWN`：取消 WebSocket 接收、Dispose/Abort WebSocket

---

## 触发条件

| 触发类型 | 条件 | 字段路径 | 冷却 | 去重策略 |
|----------|------|----------|------|----------|
| 聊天 Hello | `data.content` trim 后 equalsIgnoreCase `"hello"` | `data.content` | 3 秒全局冷却 | 无（每次独立） |
| 礼物连击结束 | `data.repeatEnd == true` | `data.repeatEnd` | 5 秒全局冷却 | `data.common.msgId` 去重 |

---

## 效果表

| 触发 | 效果 | 参数 | 备注 |
|------|------|------|------|
| 聊天 hello | `Notification.Show(message)` | `message = $"🎉 {nickname} says Hello!"` | 仅通知，无游戏世界修改 |
| 礼物连击结束 | `World.CreateVehicle(model, pos, heading)` | model: 随机 `{Police, Police2, Police3, Police4}`；pos: `World.GetNextPositionOnStreet(playerPos)`；heading: 玩家朝向 | 警车生成在玩家附近道路上 |

---

## capability_requirements

| capability | behavior | priority | status | verified_evidence | fallback |
|------------|----------|----------|--------|-------------------|----------|
| `GTA.UI.Notification.Show(string, bool)` | 显示观众昵称通知 | MUST | verified | SHVDN: `GTA.UI.Notification.Show(System.String message, System.Boolean blinking = )` score=88 | 无 |
| `GTA.World.CreateVehicle(Model, Vector3, float)` | 生成警车实体 | MUST | verified | SHVDN: `static GTA.Vehicle CreateVehicle(GTA.Model model, GTA.Math.Vector3 position, System.Single heading = )` score=88 | 无 |
| `GTA.VehicleHash.Police` | 警车模型枚举 | MUST | verified | SHVDN: `GTA.VehicleHash.Police` score=88 | 无 |
| `GTA.VehicleHash.Police2` | 警车模型枚举 | MUST | verified | SHVDN: `GTA.VehicleHash.Police2` score=88 | 无 |
| `GTA.VehicleHash.Police3` | 警车模型枚举 | MUST | verified | SHVDN: `GTA.VehicleHash.Police3` score=88 | 无 |
| `GTA.VehicleHash.Police4` | 警车模型枚举 | MUST | verified | SHVDN: `GTA.VehicleHash.Police4` score=88 | 无 |
| `GTA.Game.Player` | 获取本地玩家 | MUST | verified | SHVDN: `static GTA.Player Player { get; }` score=88 | 无 |
| `GTA.Player.Character` | 获取玩家 Ped | MUST | verified | SHVDN: `GTA.Ped Character { get; }` score=88 | 无 |
| `GTA.World.GetNextPositionOnStreet(Vector3, bool)` | 获取玩家附近道路坐标 | MUST | verified | SHVDN: `static GTA.Math.Vector3 GetNextPositionOnStreet(GTA.Math.Vector3 position, System.Boolean unoccupied = )` score=88 | 若不可用，使用 `Ped.Position` + 前方偏移 |
| `Ped.Position` (继承自 `Entity`) | 获取玩家坐标 | MUST | inferred | SHVDN 基础类型，Ped 继承 Entity.Position | 无已知风险 |
| `MainThreadDispatcher.Enqueue` | 将 GTA 操作调度到主线程 | MUST | template | LIVE Studio 模板自带组件 | 无 |
| `JavaScriptSerializer` (.NET 4.8) | JSON 反序列化 | MUST | framework | .NET Framework 4.8 内置 `System.Web.Script.Serialization.JavaScriptSerializer` | 无 |

---

## LIVE Studio 事件与礼物映射

### WebSocket 连接
- **地址**: `ws://127.0.0.1:60080`
- **订阅消息**:
```json
{"type":"subscribe","id":"gta-mod","data":{"type":"serviceSignalSub","name":"IM_MESSAGE_TRANSPORT"}}
```

### 事件 1：聊天消息 → Hello 通知

| 字段 | 值 |
|------|-----|
| event_method | `data.common.method` = `WebcastChatMessage` |
| wire_field_paths | `data.common.msgId`, `data.common.createTime`, `data.user.id`, `data.user.nickname`, `data.content` |
| trigger_condition | `data.content.Trim().Equals("hello", StringComparison.OrdinalIgnoreCase)` |
| repeat_policy | 每条消息独立触发（非连击语义） |
| dedupe_policy | 无（聊天消息无 msgId 去重需求；冷却防刷即可） |
| cooldown | 3 秒全局冷却（所有用户共享） |
| queue_cap | 不排队；冷却期间丢弃 |
| busy_state_policy | 冷却中静默丢弃 |
| abort_cleanup | 无持久状态需恢复 |

### 事件 2：礼物消息 → 警车生成

| 字段 | 值 |
|------|-----|
| event_method | `data.common.method` = `WebcastGiftMessage` |
| wire_field_paths | `data.common.msgId`, `data.common.createTime`, `data.user.id`, `data.user.nickname`, `data.gift.id`, `data.gift.name`, `data.gift.diamondCount`, `data.repeatCount`, `data.repeatEnd`, `data.comboCount` |
| trigger_condition | `data.repeatEnd == true`（仅连击结束时触发一次） |
| repeat_policy | 一次性触发（`repeatEnd=true` 时），忽略 `repeatEnd=false` |
| dedupe_policy | 按 `data.common.msgId` 去重（HashSet 缓存最近 200 条） |
| cooldown | 5 秒全局冷却 |
| queue_cap | 不排队；冷却期间丢弃 |
| busy_state_policy | 冷却中静默丢弃 |
| abort_cleanup | 不删除已生成的警车（属于游戏世界正常实体） |

---

## gift_requirements

| 字段 | 值 |
|------|-----|
| original_query | 任意礼物（用户原文：观众赠送任意礼物并在连击结束时） |
| keywords | （空） |
| status | wildcard_all_gifts |
| confirmed_name | N/A — 所有礼物均适用 |
| gift_id | N/A — 无特定 gift_id 绑定 |
| diamondCount | N/A |
| candidates | N/A |
| fallback | 无需 fallback；匹配所有 `WebcastGiftMessage` 事件 |

> 说明：用户要求"任意礼物"，因此不对礼物名称、gift_id 或钻石价格做任何过滤。所有 `WebcastGiftMessage` 消息统一处理，仅在 `repeatEnd=true` 时触发。

---

## 计时与冷却

| 冷却对象 | 时长 | 作用域 | 存储方式 |
|----------|------|--------|----------|
| Hello 通知 | 3 秒 | 全局 | `DateTime` 字段 `_lastHelloTime` |
| 警车生成 | 5 秒 | 全局 | `DateTime` 字段 `_lastPoliceSpawnTime` |

- 冷却比较：`DateTime.UtcNow - _lastXxxTime < TimeSpan.FromSeconds(N)` 时丢弃事件
- 冷却不计入 queue，直接丢弃

---

## 失败与清理

### 玩家死亡
- 聊天 Hello 通知：正常显示（通知不受死亡影响）
- 警车生成：正常生成（死亡不影响世界创建车辆）
- 无需特殊处理

### Script.Aborted 清理
- 取消 WebSocket 接收回调
- 调用 `WebSocket.Abort()` 或 `Dispose()`
- 禁止在 GTA 主线程同步 `CloseAsync().Wait()`
- 不删除已生成的警车实体（避免误删玩家自有车辆）
- 不修改玩家通缉状态或武器

### 异常处理
- WebSocket 断连：记录日志，可选自动重连（简单版仅记录）
- JSON 解析失败：静默丢弃该消息，记录日志
- CreateVehicle 失败（罕见）：静默失败，可选 Notification 提示

---

## 降级方案

| 场景 | 降级行为 |
|------|----------|
| `World.GetNextPositionOnStreet` 不可用 | 使用 `Ped.Position + Forward * 5f` 作为生成位置 |
| 警车模型全部不可用 | 降级为 `VehicleHash.Police`（单一模型） |
| Notification 失败 | 静默忽略（无替代 UI） |
| WebSocket 始终无法连接 | 脚本正常运行但不响应任何事件；Notification 提示连接失败 |

---

## 验收标准

1. ✅ 脚本启动后成功连接 `ws://127.0.0.1:60080` 并订阅 `IM_MESSAGE_TRANSPORT`
2. ✅ 观众发送 `hello`（任意大小写组合），游戏内弹出通知显示该观众昵称
3. ✅ 观众发送非 `hello` 内容（如 `Hello!`、`hello world`、`HELLO123`），不触发通知
4. ✅ 3 秒内多次发送 `hello`，仅第一次触发通知
5. ✅ 观众赠送任意礼物，连击进行中（`repeatEnd=false`）不生成警车
6. ✅ 连击结束时（`repeatEnd=true`）在玩家附近道路上生成一辆随机警车（Police/Police2/Police3/Police4）
7. ✅ 5 秒内多个连击结束事件，仅第一个生成警车
8. ✅ 同一条 `msgId` 不重复生成警车
9. ✅ 玩家死亡不影响通知和警车生成
10. ✅ 停止脚本时 WebSocket 正确关闭，不残留连接
11. ✅ 脚本清理不删除玩家原有车辆、不修改通缉状态