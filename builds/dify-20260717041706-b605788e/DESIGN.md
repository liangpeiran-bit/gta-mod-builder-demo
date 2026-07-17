<think>
用户需求是生成一个最小可玩的单文件 GTA5 LIVE Studio Mod。需要：

1. 监听 LIVE Studio 聊天评论（WebSocket ws://127.0.0.1:60080）
2. 评论 "repair" → 修复当前载具引擎生命值至最大值的 12%
3. 评论 "boost" → 当前载具动力提升到 1.8 倍，持续 4 秒
4. 两个指令各有 8 秒冷却
5. 不需要礼物、点赞、关注等事件
6. 脚本中止时清理连接、任务与临时状态

我需要查询 SHVDN API 来确认以下能力：
- WebSocket 客户端连接（可能是 System.Net.WebSockets 或类似）
- 获取玩家当前载具 (Game.Player.Character.CurrentVehicle)
- 引擎生命值 (Vehicle.EngineHealth)
- 载具动力/引擎功率 (Vehicle.EnginePowerMultiplier 或类似)
- 定时器/任务管理 (Script.Wait, 或类似)

让我先查询核心能力。先查询 SHVDN API。

第一批查询：获取载具、引擎生命值、引擎动力倍数、WebSocket 连接。

这些都是决定玩法能否成立的核心能力。
</think>
<think>
第一批查询结果：
- GTA.Ped.CurrentVehicle → GTA.Vehicle ✓
- GTA.Vehicle.EngineHealth → float (get/set) ✓
- GTA.Vehicle.EnginePowerMultiplier → float (get/set) ✓

状态是 partial，但三个核心 API 都已确认。现在需要第二批查询：WebSocket 连接、Script 生命周期、Game.Player、定时器/任务。

需要查询：
- WebSocket ClientWebSocket 或类似
- Game.Player
- Script.Wait / Yield / Task
- 清理/Abort

第二批查询。
</think>
<think>
好的，已确认：
- Game.Player (static property, returns GTA.Player)
- GTA.Player.Character (returns GTA.Ped)
- GTA.Script.Wait(int ms)

但对于 WebSocket，SHVDN 工具没有返回匹配。这是因为 WebSocket 是 .NET 标准库的一部分 (System.Net.WebSockets.ClientWebSocket)，不是 SHVDN 特有的 API。SHVDN v3 基于 .NET Framework 4.8，可以使用 System.Net.WebSockets。但工具没有返回它，可能是因为它不在 SHVDN 特定的目录中。

对于 Script 生命周期（KeyDown、OnAbort 等），SHVDN v3 中 Script 基类有 OnAbort 虚拟方法和 Tick/KeyDown 等。工具只返回了 Wait，但没有返回 Abort/OnAbort。

这些都在 SHVDN v3 的已知能力范围内。由于 SHVDN 工具只能查到它目录中的内容，而 WebSocket 是 .NET 框架的一部分，我需要标记这些。

现在信息已经足够来生成 DESIGN.md。

汇总已知能力：
- GTA.Game.Player → Player
- GTA.Player.Character → Ped
- GTA.Ped.CurrentVehicle → Vehicle
- GTA.Vehicle.EngineHealth → float (get/set)
- GTA.Vehicle.EnginePowerMultiplier → float (get/set)
- GTA.Script.Wait(int ms)

未确认但在 .NET Framework 4.8 / SHVDN v3 中可用的：
- System.Net.WebSockets.ClientWebSocket（.NET 标准库）
- Script.OnAbort / Script.Abort()（SHVDN 生命周期）
- System.Threading.Tasks 或类似异步机制

对于 WebSocket，SHVDN v3 基于 .NET Framework 4.8，ClientWebSocket 是可用标准库。我会标记为 fallback（因为工具未确认，但属于 .NET 标准能力）。

对于 Script 生命周期 - KeyDown、Tick、OnAbort 这些都是 SHVDN v3 的基本 Script 类方法。工具只返回了 Wait，但这不意味着其他方法不存在。

好，我不需要再调用礼物工具，因为用户明确说不需要礼物。

现在生成 DESIGN.md。

关于 EngineHealth：用户要求恢复到"最大值的 12%"。EngineHealth 的最大值通常是 1000（GTA5 中引擎健康值范围通常是 -4000 到 1000，1000 表示完美状态）。但用户说"最大值的 12%"，我需要理解：通常 Vehicle.EngineHealth 最大值是 1000.0f。所以 12% 是 120.0f。但用户说"恢复至最大值的 12%"，这意思是把当前引擎生命值设置为 1000 * 0.12 = 120？还是说"恢复"的意思是增加当前值的 12%？原文是"将玩家当前载具的引擎生命值恢复最大值的 12%"，我认为意思是：将引擎生命值设置为最大值的 12%（即 1000 * 0.12 = 120）。

但实际上这样引擎仍然是损坏状态（120 远低于 1000）。也许用户的意思是恢复 12% 的最大值，即增加 120 点？看看原始输入："用户评论 repair 时，将玩家当前载具的引擎生命值恢复最大值的 12%"。中文"恢复最大值的 12%"可能有歧义。但按字面理解，应该是将引擎生命值设置为最大值的 12%。

不过在 GTA5 中，1000 是最大引擎生命值。12% 即 120。这确实是一个"修复"操作，但只修复了一小部分。也许这就是设计意图——需要多次 repair 才能完全修复。

另一种解读："恢复"可能意味着恢复到"最大值的12%"，即设置为 120。我认为这就是字面意思。但在实现中需要注意：如果当前引擎生命值已经高于 120，repair 不应该降低它。应该 Math.Max(current, maxHealth * 0.12f)。

实际上仔细想想，"恢复最大值的 12%"更自然的理解是：恢复量 = 最大值的 12%，即如果引擎已损坏，修复 12%。但这仍然意味着设置到某个值。我认为比较合理的实现是：将引擎生命值设置为 Max(current, 1000 * 0.12)。但这样第一次 repair 后是 120，第二次也是 120... 这没有累积效果。

也许用户意思是每次 repair 增加最大值的 12%（即 +120），但不超过最大值。这在 GTA5 mod 中很常见——逐步修复引擎。我认为应该这样理解：每次 repair 增加引擎生命值 120 点（最大值的 12%），上限为 1000。

等等，让我重新读："将玩家当前载具的引擎生命值恢复最大值的 12%"。这可能是："恢复(至)最大值的12%"，也可能是"恢复量=最大值的12%"。我觉得更合理的游戏设计是每次 repair 恢复最大值的 12%（即 +120 点），这样需要约 8-9 次 repair 才能从 0 修复到 1000。

我将在 DESIGN.md 中采用这个解释：每次 repair 增加引擎生命值 120 点，上限 1000。

对于 boost：将 EnginePowerMultiplier 设置为 1.8，4 秒后恢复为 1.0。

现在我整理 capability_requirements 表。

关于 WebSocket：虽然 SHVDN 工具未确认，但它是 .NET Framework 4.8 标准库的一部分。我将标记为 fallback，并说明使用 System.Net.WebSockets.ClientWebSocket。

Script 的生命周期方法（OnAbort, Tick, KeyDown 等）：SHVDN 工具未返回，但它们是 SHVDN v3 Script 基类的标准方法。标记为 fallback。

现在我可以生成完整的 DESIGN.md。

关于 LIVE Studio 消息格式：用户说使用 ws://127.0.0.1:60080。LIVE Studio 的 WebSocket 消息通常是 JSON 格式。评论事件通常包含 type 字段（如 "comment"）、data 字段包含评论内容。我需要根据 LIVE Studio 的标准协议来定义。

让我假设消息格式类似：
```json
{"type":"comment","data":{"content":"repair","user":"..."}}
```

或
```json
{"cmd":"comment","content":"repair"}
```

由于用户没有指定具体格式，我将在设计中说明需要匹配 LIVE Studio 的评论消息格式，并在 capability_requirements 中标记。

实际上，参考 LIVE Studio 模板，消息格式通常是：
- type: "comment" 表示评论
- data.content 或类似字段包含评论文本

我将在设计中采用合理的假设。

现在输出 DESIGN.md。
</think># DESIGN.md

## 需求摘要

- **玩法名称**：评论控制载具修理与加速 Mod（Repair & Boost by Comment）
- **核心目标**：通过 LIVE Studio 聊天评论操控玩家当前载具的引擎修理与短时动力提升
- **胜利条件**：无（纯功能性模组，无固定胜利/失败条件）
- **失败条件**：无（模组正常运行时无失败状态）
- **时长/节奏**：持续运行；repair 与 boost 指令各有 8 秒独立冷却
- **正向互动**：
  - 评论 `repair` → 玩家当前载具引擎生命值 +120 点（最大值的 12%），上限 1000
  - 评论 `boost` → 玩家当前载具 EnginePowerMultiplier 设为 1.8×，持续 4 秒后恢复 1.0×
- **负向互动**：无
- **礼物要求**：不需要
- **触发来源**：仅 LIVE Studio WebSocket 聊天评论文本匹配 `repair` 或 `boost`（大小写不敏感、Trim 后精确匹配），忽略礼物、点赞、关注等所有非评论事件

---

## 玩法流程

1. 脚本启动 → 建立 `ws://127.0.0.1:60080` WebSocket 连接
2. 持续接收 LIVE Studio 推送的 JSON 消息
3. 过滤：仅处理 `type` 为 `comment` 的消息
4. 提取评论文本 `content`，Trim 并转为小写
5. 若文本为 `repair`：
   - 检查 repair 冷却是否已过（上次 repair 时间 + 8s ≤ 当前时间）
   - 获取 `Game.Player.Character.CurrentVehicle`
   - 若载具存在：`EngineHealth = Math.Min(EngineHealth + 120f, 1000f)`
   - 更新 repair 冷却时间戳
6. 若文本为 `boost`：
   - 检查 boost 冷却是否已过（上次 boost 时间 + 8s ≤ 当前时间）
   - 获取当前载具
   - 若载具存在：`EnginePowerMultiplier = 1.8f`
   - 启动 4 秒定时器，到期后恢复 `EnginePowerMultiplier = 1.0f`（仅当载具仍存在且 multiplier 仍为 1.8）
   - 更新 boost 冷却时间戳
7. 不匹配的评论静默忽略
8. 脚本中止（OnAbort / 异常）时：关闭 WebSocket、取消所有进行中的定时器、恢复所有修改过的载具 multiplier

---

## 状态机

```
┌─────────┐     连接成功     ┌──────────┐
│  INIT   │ ───────────────→ │  ACTIVE  │
└─────────┘                  └──────────┘
                                  │
                           收到 JSON 消息
                                  │
                         ┌───────┴────────┐
                         │ type==comment? │
                         └───────┬────────┘
                            是 │         │ 否
                    ┌──────────┴──┐      │
                    │ content 匹配?│      │
                    └──────┬──────┘      │
                 repair/boost│           │
                    ┌───────┴───────┐    │
                    │ 冷却检查通过?  │    │
                    └───────┬───────┘    │
                       是 │    │ 否      │
                   ┌──────┴┐   │         │
                   │执行效果│   │         │
                   └──────┬┘   │         │
                          │    │         │
                          └────┴─────────┘
                               │
                               ↓
                          忽略，继续监听
```

- 连接断开时尝试重连（间隔 3 秒，最多无限重试）
- 任何状态收到 Abort → 进入 CLEANUP → 终止

---

## 触发条件

| 触发类型 | 条件 | 备注 |
|---------|------|------|
| 评论 `repair` | 评论文本 Trim→小写 后精确等于 `"repair"`，且 repair 冷却已过 | 大小写不敏感，不含空格/标点容错 |
| 评论 `boost` | 评论文本 Trim→小写 后精确等于 `"boost"`，且 boost 冷却已过 | 同上 |
| 其他评论 | 静默忽略 | — |
| 非评论事件 | `type != "comment"` 的消息全部忽略 | 包括 gift、like、follow 等 |

---

## 效果表

| 指令 | 效果 | 参数 | 持续时间 | 冷却 |
|------|------|------|---------|------|
| `repair` | 引擎生命值 +120（最大值的 12%），上限 1000 | `EngineHealth = Min(current + 120, 1000)` | 即时生效，永久 | 8 秒 |
| `boost` | 引擎动力倍率设为 1.8× | `EnginePowerMultiplier = 1.8f` | 4 秒后恢复 1.0× | 8 秒 |

**repair 细节**：
- 最大引擎生命值硬编码为 1000.0f（GTA5 默认最大值）
- 若当前 EngineHealth 已 ≥ 1000，repair 无效果但不报错
- 若玩家不在任何载具中，repair 无效果但不报错

**boost 细节**：
- 玩家必须在载具中才生效
- 4 秒定时器到期时检查：载具仍然存在 且 EnginePowerMultiplier 仍为 1.8 才恢复为 1.0
- 若在 4 秒内玩家换车，原载具 multiplier 保持 1.8（由清理逻辑在脚本中止时统一恢复）
- boost 期间再次收到 boost 且冷却已过：重置 multiplier 为 1.8，重置 4 秒定时器

---

## capability_requirements

| capability | behavior | priority | status | verified_evidence | fallback |
|-----------|----------|----------|--------|-------------------|----------|
| `GTA.Game.Player` | 获取本地玩家实例 | critical | **verified** | `static GTA.Player Player { get; }` (score 88) | — |
| `GTA.Player.Character` | 获取玩家 Ped | critical | **verified** | `GTA.Ped Character { get; }` (score 88) | — |
| `GTA.Ped.CurrentVehicle` | 获取玩家当前载具 | critical | **verified** | `GTA.Vehicle CurrentVehicle { get; }` (score 88) | — |
| `GTA.Vehicle.EngineHealth` | 读写引擎生命值 | critical | **verified** | `System.Single EngineHealth { get; set; }` (score 88) | — |
| `GTA.Vehicle.EnginePowerMultiplier` | 读写引擎动力倍率 | critical | **verified** | `System.Single EnginePowerMultiplier { get; set; }` (score 88) | — |
| `GTA.Script.Wait(int ms)` | 协程延时 | essential | **verified** | `static System.Void Wait(System.Int32 ms)` (score 88) | — |
| `System.Net.WebSockets.ClientWebSocket` | WebSocket 客户端连接 LIVE Studio | critical | **fallback** | SHVDN 工具未返回匹配（不在其索引范围），但属于 .NET Framework 4.8 标准库 | 使用 `System.Net.WebSockets.ClientWebSocket` + `System.Threading.Tasks` |
| `GTA.Script.OnAbort` / 脚本中止清理 | 释放 WebSocket、取消定时器、恢复载具状态 | essential | **fallback** | SHVDN 工具未返回 Abort 相关成员，但属于 SHVDN v3 Script 基类标准生命周期方法 | 使用 `protected override void OnAbort()` 或 `event EventHandler Aborted` |
| `System.Threading.CancellationTokenSource` | 取消异步 WebSocket 接收循环 | essential | **fallback** | .NET Framework 4.8 标准库 | 使用 CancellationTokenSource 控制接收循环退出 |
| LIVE Studio WebSocket 消息格式 | 解析 JSON 评论消息 | critical | **fallback** | 未在 SHVDN 工具中查询；基于 LIVE Studio 标准协议假设消息格式 `{"type":"comment","data":{"content":"..."}}` | 实际格式以 LIVE Studio 文档为准，代码中做防御性解析 |

---

## LIVE Studio 事件与礼物映射

### 监听事件

| 事件类型 | 条件 | 用途 |
|---------|------|------|
| `comment` | `type == "comment"`，`data.content` 字段存在 | 提取评论文本匹配 `repair` / `boost` |

### 忽略事件

- `gift` / `like` / `follow` / `subscribe` / `share` / 其他所有非 `comment` 类型事件

### gift_requirements

| 字段 | 值 |
|------|-----|
| original_query | — |
| keywords | — |
| status | **not_required** |
| confirmed_name | — |
| gift_id | — |
| diamondCount | — |
| candidates | — |
| fallback | — |

---

## 计时与冷却

| 参数 | 值 | 说明 |
|------|-----|------|
| repair 冷却 | 8000 ms | 从上次 repair 生效时刻起算 |
| boost 冷却 | 8000 ms | 从上次 boost 生效时刻起算 |
| boost 持续时间 | 4000 ms | 到期自动恢复 EnginePowerMultiplier → 1.0 |
| WebSocket 重连间隔 | 3000 ms | 连接断开后等待 3 秒重试 |
| 主循环轮询间隔 | 100 ms | `Script.Wait(100)` |

冷却使用 `DateTime.UtcNow` 或 `Environment.TickCount` 计算时间差。repair 与 boost 冷却独立计时，互不影响。

---

## 失败与清理

### 运行时异常处理

| 场景 | 处理 |
|------|------|
| WebSocket 连接失败 | 每 3 秒重试，不阻塞主 Tick |
| WebSocket 消息解析失败 | 捕获异常，丢弃该消息，继续监听 |
| 玩家不在载具中触发 repair/boost | 静默跳过，不消耗冷却 |
| 载具在执行中被删除 | EngineHealth/EnginePowerMultiplier 设置前做 `!= null` 与 `Exists()` 检查 |
| boost 定时器到期时载具已消失 | 跳过恢复操作 |

### 脚本中止清理（OnAbort）

1. 将 `_cancellationTokenSource.Cancel()` 通知 WebSocket 接收循环退出
2. 关闭并释放 `ClientWebSocket`（`CloseAsync` + `Dispose`）
3. 遍历 `_activeBoostVehicles`（记录所有被 boost 修改过 multiplier 且尚未恢复的载具），逐一恢复 `EnginePowerMultiplier = 1.0f`
4. 清空所有集合与引用
5. 不做文件 I/O 或持久化

---

## 降级方案

| 风险 | 降级措施 |
|------|---------|
| WebSocket 不可用（端口未开/LIVE Studio 未运行） | 脚本静默等待，每 3 秒重连，不影响 GTA5 正常运行 |
| `ClientWebSocket` 在目标 .NET 版本不可用 | 降级为 `System.Net.WebSockets` 命名空间下的 `ClientWebSocket`（需 .NET 4.5+）；SHVDN v3 基于 .NET Framework 4.8，完全兼容 |
| LIVE Studio 消息格式与假设不同 | 代码中对 `type`、`data`、`content` 字段做防御性读取，缺失字段时静默跳过 |
| 高频率评论导致性能问题 | 冷却机制天然限制效果触发频率；WebSocket 接收使用异步非阻塞模式 |
| EnginePowerMultiplier 未在 SHVDN v3 中暴露 | 该属性已验证存在（score 88），若运行时抛异常则 catch 并跳过 boost 效果 |

---

## 验收标准

1. **连接验证**：启动脚本后，LIVE Studio 运行在 `ws://127.0.0.1:60080` 时可成功建立 WebSocket 连接
2. **repair 指令**：在聊天中发送 `repair`（大小写任意），当前载具引擎生命值增加 120 点，上限 1000；8 秒内重复发送不叠加
3. **boost 指令**：在聊天中发送 `boost`，当前载具加速明显（1.8×），4 秒后自动恢复；8 秒内重复发送不叠加
4. **冷却独立性**：repair 和 boost 冷却互不影响，可在 repair 冷却期间正常触发 boost
5. **无载具时安全**：玩家步行时发送指令，脚本不崩溃、不报错、不消耗冷却
6. **非评论事件隔离**：发送礼物、点赞等不触发任何载具效果
7. **清理正确**：按 `Insert` 或 Reload Scripts 中止脚本后，WebSocket 连接关闭，所有载具 multiplier 恢复 1.0，无残留任务
8. **单文件独立**：整个 Mod 为单个 `.cs` 文件，无外部依赖、无配置文件、无额外程序集引用（除 SHVDN v3 标准 API 和 .NET Framework 4.8 标准库）