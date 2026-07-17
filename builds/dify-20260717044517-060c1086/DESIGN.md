<think>
我们正在处理一个首次生成的任务。需要生成一个最小可玩的单文件 GTA5 LIVE Studio Mod。需求很清楚：

- 评论 "repair" → 将玩家当前载具引擎生命值恢复至最大值的12%
- 评论 "boost" → 将当前载具动力临时提升到1.8倍，持续4秒
- 两个指令分别设置8秒冷却
- 必须使用真实 ws://127.0.0.1:60080 LIVE Studio 消息连接
- 脚本中止时清理连接、任务与所有临时状态
- 不要使用礼物、点赞、关注或其他事件

现在我们需要先调用工具来确认 API 能力。我们需要查询 SHVDN 相关能力：

1. 首先需要知道如何获取玩家当前载具 (Game.Player.Character.CurrentVehicle)
2. 引擎生命值相关 (Vehicle.EngineHealth)
3. 载具动力修改 - 这可能涉及 Vehicle.Acceleration 或通过 native 调用修改引擎动力乘数
4. WebSocket 客户端连接 - 可能需要 System.Net.WebSockets 或类似
5. 脚本生命周期 - Script.Wait, Tick, Aborted 等

另外，关于礼物，用户明确说不要使用礼物，所以 gift_requirements 标记为 not_required。

让我们先调用 SHVDN API 能力检索。

第一批查询：核心能力 - 载具相关（获取当前载具、引擎生命值、动力修改）和 WebSocket 连接。

让我们仔细思考需要什么 API：

1. 获取玩家当前载具：`Player.Character.CurrentVehicle` 或 `Game.Player.Character.CurrentVehicle`
2. 载具引擎生命值：`Vehicle.EngineHealth` 属性
3. 载具动力修改：可能需要 `Vehicle.Acceleration` 或使用 native 函数。在 SHVDN 中，可能有 `Vehicle.EnginePowerMultiplier` 或类似属性，或者需要调用 `SET_VEHICLE_ENGINE_POWER_MULTIPLIER` native。
4. WebSocket 连接：`System.Net.WebSockets.ClientWebSocket` 在 .NET 中可用，但需要确认在 SHVDN 环境中是否可用。也可能有 `WebSocket` 相关类。
5. 脚本生命周期：`Script` 基类，`KeyDown`、`Tick`、`Aborted` 事件等。

我们需要查询这些。让我们写两个 query。

第一批：核心载具能力和 WebSocket。

第二批：如果需要更多细节。

先来第一批：

Query 1: "Player Character CurrentVehicle Vehicle EngineHealth EnginePowerMultiplier SET_VEHICLE_ENGINE_POWER_MULTIPLIER Vehicle Repair"

这应该能覆盖载具相关能力。

Query 2（如果需要）：WebSocket 相关。

但我们可以先只调用一次，看看返回什么。根据规则，我们最多调用 2 次 SHVDN。第一批查核心能力，如果需要再查第二批。

但我们也需要确认 WebSocket 在 SHVDN 中的可用性。这可能是一个独立的域。

让我们设计两个不重叠的查询：

Query 1: 载具相关能力
"CurrentVehicle Vehicle.EngineHealth engine power multiplier SET_VEHICLE_ENGINE_POWER_MULTIPLIER repair vehicle acceleration"

Query 2: WebSocket 和脚本生命周期
"ClientWebSocket WebSocket connect async Script Tick Aborted KeyDown"

但总长度限制 256 字符。

Query 1 应该可以覆盖载具能力。Query 2 覆盖 WebSocket 和脚本生命周期。

另外，我们不需要调用 query_live_gifts，因为用户明确说不要使用礼物。

好，让我们先调用两个 SHVDN 查询。

但是等等，规则说："SHVDN 工具在本节点最多调用 2 次。允许按互不重叠的能力域分批：第一批查询决定玩法可实现性的核心能力，第二批仅查询剩余的独立能力"

所以我们可以同时发起两个查询。但平台可能是按顺序执行的。我们可以同时发起，如果平台支持的话。

让我们先同时发起两个查询。

对于礼物，用户明确说不要礼物，所以 gift_requirements 标记为 not_required，不调用 query_live_gifts。

现在，整理 query：

Query 1 (载具相关):
"CurrentVehicle Vehicle EngineHealth EnginePowerMultiplier acceleration SET_VEHICLE_ENGINE_POWER_MULTIPLIER repair vehicle"

长度检查：这大约是 100 字符左右，安全。

Query 2 (WebSocket和脚本生命周期):
"ClientWebSocket WebSocket connect async Script Tick Aborted KeyDown unsubscribe"

长度检查：大约 70 字符，安全。

好，发起调用。
</think>
<think>
现在让我分析两个查询的结果：

第一个查询（载具相关）返回了：
- GTA.Ped.CurrentVehicle → GTA.Vehicle (score 88) - 确认
- GTA.Vehicle.EngineHealth → System.Single { get; set; } (score 88) - 确认
- GTA.Vehicle.EnginePowerMultiplier → System.Single { get; set; } (score 88) - 确认

这些是关键能力，状态是 partial，但匹配分数都很高。这足够确认载具能力。

第二个查询（WebSocket 和脚本生命周期）返回了：
- GTA.Script.KeyDown (event) - 确认
- GTA.Script.Tick (event) - 确认

但没有返回 WebSocket 相关类、Aborted 事件等。状态是 partial。

这意味着我们还需要确认：
- WebSocket 连接能力（ClientWebSocket 或类似）
- Script.Aborted 事件
- 协程/异步支持 (Script.Wait, Yield, etc.)

但我们已经用了 2 次 SHVDN 调用，不能再调用了。对于未确认的能力，我们需要标记为 needs_query 或 fallback。

对于 WebSocket，SHVDN 环境通常允许使用 .NET 的 System.Net.WebSockets.ClientWebSocket，因为 SHVDN 运行在完整 .NET Framework 上。这可以作为 fallback。

对于 Script.Aborted 事件，虽然没有在结果中显示，但 SHVDN 的 Script 基类通常有 Aborted 事件。这可以作为 fallback。

现在，礼物工具：用户明确说不要使用礼物，所以 gift_requirements 标记为 not_required。

现在我们可以生成 DESIGN.md。

总结能力表：

Verified:
- GTA.Ped.CurrentVehicle → 获取玩家当前载具
- GTA.Vehicle.EngineHealth → 引擎生命值 get/set
- GTA.Vehicle.EnginePowerMultiplier → 引擎动力倍率 get/set
- GTA.Script.Tick → 每帧触发事件
- GTA.Script.KeyDown → 按键事件

Needs_query / Fallback:
- WebSocket 客户端连接 → System.Net.WebSockets.ClientWebSocket (fallback: .NET Framework 标准库)
- Script.Aborted → fallback: SHVDN Script 基类标准事件
- Script.Wait / Yield → fallback: SHVDN 标准协程支持
- 定时器/冷却管理 → fallback: DateTime.UtcNow 或 Game.GameTime

现在设计玩法：

状态机：
- IDLE：等待评论
- REPAIR_COOLDOWN：repair 指令冷却中（8秒）
- BOOST_COOLDOWN：boost 指令冷却中（8秒）
- BOOST_ACTIVE：boost 效果生效中（4秒）

实际上，冷却和效果可以独立追踪。我们可以使用更简单的设计：

两个独立冷却计时器：
- lastRepairTime
- lastBoostTime

和一个 boost 效果追踪：
- boostEndTime
- originalPowerMultiplier（用于恢复）

流程：
1. 脚本启动 → 连接 ws://127.0.0.1:60080
2. 在 Tick 中接收 WebSocket 消息
3. 解析消息中的评论内容
4. 如果评论是 "repair"（不区分大小写），检查冷却 → 如果冷却已过，执行修复并更新冷却时间
5. 如果评论是 "boost"，检查冷却 → 如果冷却已过，执行加速并更新冷却时间和效果结束时间
6. 在 Tick 中检查 boost 效果是否到期 → 恢复原始动力倍率
7. 脚本中止时 → 关闭 WebSocket 连接，恢复所有临时状态

关键数值：
- repair：引擎生命值 = 最大值 × 0.12 → 但需求说"恢复至最大值的12%"，这有两种解读：
  a) 恢复当前引擎生命值的12%（即增加 12% 当前值）
  b) 将引擎生命值设置为最大值的12%

  用户说"将玩家当前载具的引擎生命值恢复最大值的 12%"，我理解为将引擎生命值设置为最大值的12%（即修复到12%的健康状态）。但通常 repair 应该是恢复一些生命值，而不是设置。让我重新读一下："将玩家当前载具的引擎生命值恢复最大值的 12%" —— 这意思是恢复到最大值的12%。也就是说，如果最大值是1000，就设置引擎生命值为120。

  这有点奇怪，因为通常引擎生命值1000是满的，12%就是120，这意味着反而让引擎更差。但也许在GTA5中引擎生命值越低越好？不，在GTA5中，EngineHealth 默认是1000，当车辆受损时降低。所以"恢复最大值的12%"意味着设置 EngineHealth = 1000 * 0.12 = 120，这反而损坏了引擎。这不太合理。

  也许用户意思是"恢复引擎生命值，恢复量为最大值的12%"，即增加12%的最大值。但用户说的是"恢复最大值的 12%"，语义模糊。我倾向于按字面理解：设置引擎生命值为最大值的12%。或者理解为"恢复（动词）最大值的12%"，即增加最大值的12%。

  看英文思维："将玩家当前载具的引擎生命值恢复最大值的 12%" → 恢复（动词）... 最大值的 12%。可以理解为 "restore 12% of the max engine health"，也就是增加当前引擎生命值，增加量为最大值的12%。

  我认为比较合理的解读是：每次 repair 指令增加 EngineHealth，增加量为 MaxEngineHealth 的 12%（即增加 120，如果默认最大值是1000）。但 Vehicle 类有 MaxEngineHealth 吗？可能没有直接属性。EngineHealth 最大值默认是 1000。

  或者更简单的实现：直接设置 EngineHealth += 120（最大值的12%）。但需要获取最大值。通常 EngineHealth 最大值是 1000。我们可以硬编码 1000，或者使用 Vehicle.MaxHealth 之类的。

  等等，实际上在 SHVDN 中可能有 Vehicle.MaxHealth 或者类似属性。但我们没有确认。我们可以采用固定值 1000。

  让我重新解读："恢复最大值的 12%"。我觉得最自然的理解是：将引擎生命值恢复（增加）最大值的12%。也就是说 EngineHealth += 0.12 * maxEngineHealth。其中 maxEngineHealth 通常为 1000。

  但为了安全，我将在 DESIGN.md 中明确写出两种可能的解读，并选择一种。我选择"每次 repair 将当前引擎生命值增加最大引擎生命值（默认1000）的12%"，即 EngineHealth += 120。但需确保不超过最大值。

  实际上，更可能是"设置引擎生命值为最大值的12%"，但这不合理。等等，也许是"恢复到最大值的12%"，意味着如果引擎坏了（0），修复到12%。这在生存类玩法中可能有意义——每次 repair 只能修复一点点。

  我认为比较合理的解释：EngineHealth = Math.Min(EngineHealth + maxEngineHealth * 0.12, maxEngineHealth)。默认 maxEngineHealth = 1000。

  我们就这样处理。

- boost：EnginePowerMultiplier = 1.8，持续4秒后恢复原始值。

冷却：8秒，两个指令独立冷却。

WebSocket 消息格式：需要了解 LIVE Studio 的消息格式。典型格式可能是 JSON，包含 type、data、comment 等字段。由于没有具体文档，我们需要做一个合理的假设。LIVE Studio 可能发送类似：
{"type":"comment","data":{"text":"repair","user":"..."}}
或其他格式。

在 fallback 中，我们假设消息格式为 JSON，包含 "content" 或 "text" 字段用于评论内容。

现在生成 DESIGN.md。

capability_requirements 表：

| capability | behavior | priority | status | verified_evidence | fallback |
|---|---|---|---|---|---|
| 获取玩家当前载具 | 通过 Game.Player.Character.CurrentVehicle | P0 | verified | GTA.Ped.CurrentVehicle → GTA.Vehicle | - |
| 读取/设置引擎生命值 | Vehicle.EngineHealth { get; set; } | P0 | verified | GTA.Vehicle.EngineHealth → System.Single | - |
| 读取/设置引擎动力倍率 | Vehicle.EnginePowerMultiplier { get; set; } | P0 | verified | GTA.Vehicle.EnginePowerMultiplier → System.Single | - |
| 脚本 Tick 事件 | 每帧处理消息与计时 | P0 | verified | GTA.Script.Tick → System.EventHandler | - |
| WebSocket 客户端连接 | 连接 ws://127.0.0.1:60080 接收消息 | P0 | needs_query | 未在 SHVDN 目录中匹配 | System.Net.WebSockets.ClientWebSocket (.NET Framework) |
| 脚本中止清理 | Aborted 事件中关闭连接、恢复状态 | P0 | needs_query | 未在 SHVDN 目录中匹配 | GTA.Script.Aborted 事件 (SHVDN 标准) |
| 协程/异步支持 | 消息接收循环 | P1 | needs_query | 未匹配 | Script.Wait() + Task.Run 或同步轮询 |
| 最大引擎生命值 | 计算12%修复量 | P1 | needs_query | 未匹配 | 硬编码 1000.0f |

gift_requirements 表：

| field | value |
|---|---|
| original_query | N/A |
| keywords | N/A |
| status | not_required |
| confirmed_name | N/A |
| gift_id | N/A |
| diamondCount | N/A |
| candidates | N/A |
| fallback | N/A |

现在生成完整 DESIGN.md。
</think># DESIGN.md

## 需求摘要

- **玩法名称**: Repair & Boost Mod
- **核心目标**: 通过直播间评论指令实时控制玩家当前载具的修理与加速
- **胜利条件**: 无（自由玩法）
- **失败条件**: 无
- **时长/节奏**: 两个指令独立冷却 8 秒；boost 效果持续 4 秒
- **正向互动**:
  - 评论 `repair` → 将玩家当前载具引擎生命值增加最大值的 12%（默认最大 1000，即 +120，不超过上限）
  - 评论 `boost` → 将当前载具引擎动力倍率临时设为 1.8 倍，持续 4 秒
- **负向互动**: 无
- **礼物要求**: 不需要（不使用礼物、点赞、关注等事件）
- **触发方式**: 仅通过 `ws://127.0.0.1:60080` 接收 LIVE Studio 评论消息，匹配关键词 `repair` 和 `boost`

## change_summary

- 首次生成，无变更历史。

## preserved_behaviors

- 首次生成，无保留行为。

---

## 玩法流程

1. 脚本启动 → 初始化 WebSocket 连接到 `ws://127.0.0.1:60080`。
2. 进入 IDLE 状态，持续监听 WebSocket 消息。
3. 收到评论消息后解析文本内容，与 `repair` / `boost`（不区分大小写）匹配。
4. **repair 命中** → 检查 repair 冷却是否已过（距上次 repair ≥ 8 秒）。若冷却未过则忽略；若已过则：
   - 获取玩家当前载具 `CurrentVehicle`。
   - 若当前无载具则忽略。
   - 计算修复量：`maxEngineHealth * 0.12`（`maxEngineHealth` 默认 1000.0f）。
   - 设置 `EngineHealth = Min(EngineHealth + 修复量, maxEngineHealth)`。
   - 更新 `lastRepairTime` 为当前时间。
5. **boost 命中** → 检查 boost 冷却是否已过（距上次 boost ≥ 8 秒）。若冷却未过则忽略；若已过则：
   - 获取玩家当前载具 `CurrentVehicle`。
   - 若当前无载具则忽略。
   - 保存原始动力倍率 `originalPowerMultiplier`。
   - 设置 `EnginePowerMultiplier = 1.8f`。
   - 记录 `boostEndTime = 当前时间 + 4 秒`。
   - 更新 `lastBoostTime` 为当前时间。
6. 每帧 Tick 中检查 boost 效果是否到期：若 `boostEndTime` 已到达且未恢复，则恢复 `EnginePowerMultiplier = originalPowerMultiplier`，清除 boost 状态。
7. 脚本中止（Aborted）时 → 关闭 WebSocket 连接，恢复所有临时载具状态（动力倍率），清理所有计时器与资源。

---

## 状态机

| 状态 | 描述 |
|---|---|
| **IDLE** | 等待 WebSocket 消息，无活跃效果 |
| **REPAIR_COOLDOWN** | repair 指令 8 秒冷却中，期间忽略 repair 评论 |
| **BOOST_COOLDOWN** | boost 指令 8 秒冷却中，期间忽略 boost 评论 |
| **BOOST_ACTIVE** | boost 效果生效中（4 秒倒计时），动力倍率 = 1.8x |

> 注：REPAIR_COOLDOWN 与 BOOST_COOLDOWN/BOOST_ACTIVE 可同时存在（独立冷却）。BOOST_COOLDOWN 和 BOOST_ACTIVE 互斥。

---

## 触发条件

| 触发源 | 条件 | 动作 |
|---|---|---|
| WebSocket 消息 | 消息文本匹配 `repair`（忽略大小写、前后空白） | 进入 repair 处理流程 |
| WebSocket 消息 | 消息文本匹配 `boost`（忽略大小写、前后空白） | 进入 boost 处理流程 |
| Tick 轮询 | `boostEndTime` 已到达且未恢复 | 恢复原始动力倍率，回到 IDLE |

---

## 效果表

| 效果 | 目标 | 属性修改 | 持续时间 | 冷却 | 可叠加 |
|---|---|---|---|---|---|
| repair | 玩家当前载具 | `EngineHealth += maxEngineHealth * 0.12`（上限 `maxEngineHealth`） | 即时 | 8 秒 | 每次独立判定 |
| boost | 玩家当前载具 | `EnginePowerMultiplier = 1.8f` | 4 秒 | 8 秒（从指令触发时计） | 不可叠加（新 boost 覆盖旧 boost） |

---

## capability_requirements

| capability | behavior | priority | status | verified_evidence | fallback |
|---|---|---|---|---|---|
| 获取玩家当前载具 | `Game.Player.Character.CurrentVehicle` | P0 | verified | `GTA.Ped.CurrentVehicle → GTA.Vehicle { get; }` (score 88) | — |
| 读取/设置引擎生命值 | `Vehicle.EngineHealth { get; set; }` | P0 | verified | `GTA.Vehicle.EngineHealth → System.Single { get; set; }` (score 88) | — |
| 读取/设置引擎动力倍率 | `Vehicle.EnginePowerMultiplier { get; set; }` | P0 | verified | `GTA.Vehicle.EnginePowerMultiplier → System.Single { get; set; }` (score 88) | — |
| 脚本 Tick 事件 | 每帧处理消息接收与计时 | P0 | verified | `GTA.Script.Tick → System.EventHandler` (score 88) | — |
| 脚本按键事件 | 保留但本玩法不使用 | P2 | verified | `GTA.Script.KeyDown → KeyEventHandler` (score 88) | — |
| WebSocket 客户端连接 | `ws://127.0.0.1:60080` 接收 LIVE Studio 消息 | P0 | needs_query | SHVDN 目录未匹配到 `ClientWebSocket` | `System.Net.WebSockets.ClientWebSocket`（.NET Framework 4.8 标准库） |
| 脚本中止清理 | `Aborted` 事件中关闭连接、恢复动力倍率 | P0 | needs_query | SHVDN 目录未匹配 | `GTA.Script.Aborted` 事件（SHVDN v3 标准事件） |
| 异步消息接收循环 | WebSocket 接收在后台持续运行 | P1 | needs_query | 未匹配 | `Task.Run` + `CancellationToken`，或 Tick 中同步轮询 `WebSocket.ReceiveAsync` 带短超时 |
| 最大引擎生命值常量 | 计算 12% 修复量基准 | P1 | needs_query | 未匹配 `MaxEngineHealth` | 硬编码 `1000.0f`（GTA5 默认引擎生命值上限） |
| 时间源 | 冷却计时与 boost 到期判定 | P1 | needs_query | 未匹配 `Game.GameTime` | `DateTime.UtcNow` 或 `Environment.TickCount` |

---

## LIVE Studio 事件与礼物映射

### 事件订阅

| 事件类型 | 订阅 | 处理 |
|---|---|---|
| WebSocket 消息（评论） | 连接 `ws://127.0.0.1:60080`，接收文本帧 | 解析 JSON，提取评论文本字段，匹配指令 |
| 礼物 | 不订阅 | — |
| 点赞 | 不订阅 | — |
| 关注 | 不订阅 | — |

### gift_requirements

| 字段 | 值 |
|---|---|
| original_query | N/A |
| keywords | N/A |
| status | **not_required** |
| confirmed_name | N/A |
| gift_id | N/A |
| diamondCount | N/A |
| candidates | N/A |
| fallback | N/A |

---

## 计时与冷却

| 计时器 | 触发条件 | 时长 | 行为 |
|---|---|---|---|
| `repairCooldown` | repair 指令执行后 | 8 秒 | 期间忽略所有 repair 评论 |
| `boostCooldown` | boost 指令执行后 | 8 秒 | 期间忽略所有 boost 评论 |
| `boostDuration` | boost 指令执行后 | 4 秒 | 到期后恢复 `EnginePowerMultiplier` 为原始值 |

冷却检查使用统一时间源（`DateTime.UtcNow` 或 `Game.GameTime`，优先 `Game.GameTime` 若可用）。

---

## 失败与清理

### 运行时异常

| 场景 | 处理 |
|---|---|
| WebSocket 连接失败 | 重试连接（间隔 3 秒，最多 5 次），失败后脚本继续运行但不处理消息 |
| WebSocket 断线 | 自动重连（间隔 3 秒），重连期间忽略消息 |
| 玩家不在载具中 | 忽略 repair/boost 指令，不消耗冷却 |
| 消息解析失败（非 JSON） | 静默忽略该消息 |
| 引擎生命值已是最大值 | 不修改，但仍消耗 repair 冷却 |

### 脚本中止清理（Aborted）

1. 关闭 WebSocket 连接（`CloseAsync` + `Dispose`）。
2. 若 boost 效果仍生效 → 恢复 `EnginePowerMultiplier` 为原始值。
3. 取消所有异步任务（CancellationTokenSource.Cancel）。
4. 清空所有计时器与状态变量。

---

## 降级方案

| 场景 | 降级策略 |
|---|---|
| `System.Net.WebSockets` 不可用 | 改用 `System.Net.Sockets.TcpClient` 手动实现 WebSocket 握手与帧解析（复杂度显著增加） |
| `Script.Aborted` 不可用 | 改用 `Script.Tick` 中检测 `Script.IsAborted` 或在 `Dispose` 中清理 |
| `Game.GameTime` 不可用 | 改用 `DateTime.UtcNow` |
| `Vehicle.EnginePowerMultiplier` 不可用 | 改用 Native `SET_VEHICLE_ENGINE_POWER_MULTIPLIER`（hash 可能需要手动计算） |
| `Vehicle.EngineHealth` 只读 | 改用 Native `SET_VEHICLE_ENGINE_HEALTH` |

---

## 验收标准

1. 脚本启动后成功连接到 `ws://127.0.0.1:60080`。
2. 发送评论 `repair` → 玩家当前载具引擎生命值增加约 120（默认最大 1000 下），且 8 秒内重复发送无效。
3. 发送评论 `boost` → 玩家当前载具加速明显提升（1.8 倍动力），4 秒后自动恢复；8 秒内重复发送无效。
4. 评论大小写不敏感：`REPAIR`、`Repair`、`Boost`、`BOOST` 均有效。
5. 评论含前后空格（如 `  repair  `）仍可匹配。
6. 玩家不在载具中时，指令不生效且不消耗冷却。
7. 两个指令冷却独立：repair 冷却中仍可触发 boost（若 boost 未冷却），反之亦然。
8. boost 效果期间新 boost 冷却到期后可重新触发，新的 boost 覆盖旧的（重置 4 秒计时器）。
9. 脚本中止后 WebSocket 连接关闭，载具动力倍率恢复。
10. 无礼物、点赞、关注等事件处理逻辑。