<think>
用户要求生成一个新的MOD，基于LIVE Studio评论指令。让我分析需求：

1. 核心功能：处理LIVE Studio评论中的`repair`和`boost`指令
2. `repair`：修复引擎血量12%（使用`Game.Player.Character.CurrentVehicle.EngineHealth`）
3. `boost`：临时设置引擎动力倍率为1.8倍，4秒后恢复（使用`Vehicle.EnginePowerMultiplier`）
4. 两个指令各冷却8秒
5. WebSocket连接：`ws://127.0.0.1:60080`，使用`serviceSignalSub`和`IM_MESSAGE_TRANSPORT`订阅并解析`WebcastChatMessage`
6. Aborted时关闭连接、取消任务并恢复临时状态
7. 不要礼物、点赞、关注、入场、分享、复杂AI或世界持久化

现在我需要：
- 调用SHVDN API来确认所需的能力
- 礼物不需要，所以gift_requirements为not_required

让我先列出需要查询的SHVDN能力：

核心能力：
1. `Game.Player.Character.CurrentVehicle` - 获取当前车辆
2. `Vehicle.EngineHealth` - 引擎血量
3. `Vehicle.EnginePowerMultiplier` - 引擎动力倍率
4. WebSocket相关 - 但这是.NET的，可能不需要SHVDN查询
5. `Script.Wait` - 等待
6. `Game.Player.Character` - 获取玩家角色

WebSocket是.NET标准库（System.Net.WebSockets），这应该在C#中可用。

SHVDN特有的：
- `Vehicle.EngineHealth` 
- `Vehicle.EnginePowerMultiplier`
- `Vehicle.MaxEngineHealth` 或类似
- `Game.Player.Character.CurrentVehicle`

让我分批查询。第一批查询核心能力，第二批查询其他。

第一批：检查Vehicle.EngineHealth, Vehicle.EnginePowerMultiplier, CurrentVehicle, Game.Player.Character
第二批：检查Script.Wait, Ped, Vehicle相关

实际上应该更精确。让我设计查询：

第一批：`Vehicle.EngineHealth EnginePowerMultiplier CurrentVehicle Game.Player.Character`
第二批：可能不需要，如果第一批覆盖了所有。

实际上我应该更精确地查询。让我使用准确的API名称：

第一批查询：`Vehicle.EngineHealth Vehicle.EnginePowerMultiplier CurrentVehicle Game.Player.Character`

这应该足够了。WebSocket、Task、CancellationToken等都是.NET标准库。

让我也考虑是否需要确认MaxEngineHealth或类似属性。

查询1：`Vehicle.EngineHealth Vehicle.EnginePowerMultiplier CurrentVehicle`

这涵盖了核心能力。

现在，礼物不需要，所以`gift_requirements`标记为`not_required`。

让我开始工具调用。
</think>
<think>
SHVDN查询返回了5个匹配项，全部确认：

1. `GTA.Game.Player` - static property, returns `GTA.Player`
2. `GTA.Player.Character` - property, returns `GTA.Ped`
3. `GTA.Ped.CurrentVehicle` - property, returns `GTA.Vehicle`
4. `GTA.Vehicle.EngineHealth` - property, `System.Single { get; set; }`
5. `GTA.Vehicle.EnginePowerMultiplier` - property, `System.Single { get; set; }`

这些全部验证通过。但是我还需要确认是否有`Vehicle.MaxEngineHealth`或类似属性来获取最大引擎血量。用户说"修复最大引擎生命值的12%"，这意味着需要知道最大引擎生命值。让我查询这个。

另外，用户提到`WebcastChatMessage`，但这可能不是SHVDN的一部分，而是LIVE Studio SDK的一部分。这个在SHVDN中可能不存在。

让我第二次查询：`Vehicle.MaxEngineHealth` 或类似属性。

另外可能还需要确认`Script.Wait`和`Script.Aborted`。

查询：`Vehicle MaxEngineHealth Script Wait Aborted`
</think>
<think>
第二次查询确认了：
- `GTA.Script.Abort()` - 方法
- `GTA.Script.Aborted` - 事件
- `GTA.Script.Wait(int ms)` - 静态方法
- `GTA.Vehicle.EngineHealth` - 再次确认

但没有找到 `Vehicle.MaxEngineHealth`。这意味着可能没有直接的`MaxEngineHealth`属性。需要用其他方式获取最大引擎血量。

在GTA5中，引擎的最大血量通常是1000.0（对于大部分车辆）。但这不是通过SHVDN API直接暴露的。可能需要使用Native调用来获取。不过SHVDN没有返回`MaxEngineHealth`，让我考虑一下...

实际上用户说"修复最大引擎生命值的12%"，这意味着需要知道最大引擎生命值。如果没有`MaxEngineHealth`属性，可以：
1. 使用固定值（如1000.0，这是GTA5中大多数车辆的标准最大引擎血量）
2. 或者使用Native调用

但SHVDN工具没有返回MaxEngineHealth，所以我会标记为fallback并使用1000.0作为默认最大引擎血量。

现在让我也思考一下WebSocket部分。用户提到`WebcastChatMessage`和`serviceSignalSub`/`IM_MESSAGE_TRANSPORT`。这些是LIVE Studio SDK的一部分，不在SHVDN范围内。不过WebSocket连接本身是.NET标准库的一部分。

现在用户没有礼物意图，所以gift_requirements为not_required。

现在我已经完成了所有工具调用（SHVDN两次，礼物不需要），让我输出DESIGN.md。

总结确认的能力：

已验证：
- `GTA.Game.Player` → `GTA.Player`
- `GTA.Player.Character` → `GTA.Ped`
- `GTA.Ped.CurrentVehicle` → `GTA.Vehicle`
- `GTA.Vehicle.EngineHealth` (float, get/set)
- `GTA.Vehicle.EnginePowerMultiplier` (float, get/set)
- `GTA.Script.Wait(int ms)` (static void)
- `GTA.Script.Abort()` 
- `GTA.Script.Aborted` (event)

未确认/fallback：
- `Vehicle.MaxEngineHealth` - 未找到，使用固定值1000.0作为fallback
- LIVE Studio WebSocket消息格式（WebcastChatMessage）- 不在SHVDN范围内，属于LIVE Studio SDK

现在编写DESIGN.md。
</think># DESIGN.md

## 需求摘要

- **玩法名称**: LIVE Studio 评论指令 Mod（Repair & Boost）
- **核心目标**: 监听 LIVE Studio 真实评论，解析 `repair` 与 `boost` 文本指令，在 GTA5 中对玩家当前车辆施加引擎修复或临时动力提升效果。
- **胜利条件**: 无（纯互动效果 Mod）
- **失败条件**: 无
- **时长/节奏**: 指令冷却 8 秒；boost 效果持续 4 秒后自动恢复原始倍率
- **正向互动**: `repair` → 引擎血量 +12%（基于最大引擎血量）；`boost` → EnginePowerMultiplier 临时设为 1.8 倍
- **负向互动**: 无
- **礼物要求**: 不需要
- **评论/指令触发**: 仅处理评论正文匹配 `repair` 或 `boost`（大小写不敏感、trim 后精确匹配）；忽略礼物、点赞、关注、入场、分享等所有其他消息类型

## change_summary

- 首次生成，无变更历史。

## preserved_behaviors

- 首次生成，无保留行为。

## 玩法流程

```
[脚本启动]
    │
    ├─► 注册 Script.Aborted 回调（用于清理）
    │
    ├─► 建立 WebSocket 连接 ws://127.0.0.1:60080
    │       │
    │       ├─ 成功 ──► 发送 serviceSignalSub 订阅 IM_MESSAGE_TRANSPORT
    │       │              │
    │       │              └─► 进入消息循环（Task.Run 异步）
    │       │                     │
    │       │                     ├─ 收到 WebcastChatMessage
    │       │                     │       │
    │       │                     │       ├─ 提取评论正文（trim + ToLowerInvariant）
    │       │                     │       │
    │       │                     │       ├─ == "repair" ──► 检查冷却 → 执行 repair
    │       │                     │       │
    │       │                     │       └─ == "boost"  ──► 检查冷却 → 执行 boost
    │       │                     │
    │       │                     └─ 忽略其他消息类型
    │       │
    │       └─ 失败 ──► 记录日志，脚本继续等待或重试（有限次数后降级）
    │
    └─► 主循环 Script.Wait 保持脚本存活
```

## 状态机

| 状态 | 进入条件 | 行为 | 退出条件 |
|---|---|---|---|
| `Idle` | 脚本启动、WebSocket 连接就绪 | 等待评论消息，维护冷却计时器 | 收到匹配指令 |
| `RepairExecuting` | 收到 `repair` 评论且冷却已过 | 读取 CurrentVehicle.EngineHealth，计算目标值（当前 + 最大×12%），设置 EngineHealth | 设置完成，回到 Idle |
| `BoostActive` | 收到 `boost` 评论且冷却已过 | 记录原始 EnginePowerMultiplier，设为 1.8，启动 4 秒计时器 | 4 秒到期或 Aborted |
| `BoostRestoring` | Boost 计时器到期或 Aborted | 恢复 EnginePowerMultiplier 为原始值 | 恢复完成，回到 Idle |
| `Aborting` | Script.Aborted 触发 | 关闭 WebSocket，取消异步任务，恢复 EnginePowerMultiplier（如需要），释放资源 | 脚本终止 |

## 触发条件

| 触发方式 | 条件 | 备注 |
|---|---|---|
| WebSocket 消息 | 接收到 `WebcastChatMessage` 类型的消息 | 需要 LIVE Studio SDK 消息格式 |
| 评论匹配 `repair` | 评论正文 trim + ToLowerInvariant 后 == "repair" | 精确匹配，不含前后空格 |
| 评论匹配 `boost` | 评论正文 trim + ToLowerInvariant 后 == "boost" | 精确匹配，不含前后空格 |
| 冷却检查 | 同一指令距上次执行 ≥ 8 秒 | repair 和 boost 各自独立冷却 |

## 效果表

| 指令 | 效果 | API | 参数 | 持续时间 | 冷却 |
|---|---|---|---|---|---|
| `repair` | 引擎修复 | `Vehicle.EngineHealth` (set) | 当前值 + (最大引擎血量 × 0.12)，上限为最大引擎血量 | 即时 | 8 秒 |
| `boost` | 动力提升 | `Vehicle.EnginePowerMultiplier` (set) | 1.8 | 4 秒后恢复 | 8 秒 |

## capability_requirements

| capability | behavior | priority | status | verified_evidence | fallback |
|---|---|---|---|---|---|
| `GTA.Game.Player` | 获取当前玩家实例 | critical | verified | `static GTA.Player Player { get; }` | — |
| `GTA.Player.Character` | 获取玩家 Ped | critical | verified | `GTA.Ped Character { get; }` | — |
| `GTA.Ped.CurrentVehicle` | 获取玩家当前驾驶的车辆 | critical | verified | `GTA.Vehicle CurrentVehicle { get; }` | — |
| `GTA.Vehicle.EngineHealth` | 读写引擎血量 | critical | verified | `System.Single EngineHealth { get; set; }` | — |
| `GTA.Vehicle.EnginePowerMultiplier` | 读写引擎动力倍率 | critical | verified | `System.Single EnginePowerMultiplier { get; set; }` | — |
| `GTA.Script.Wait(int ms)` | 主循环挂起 | critical | verified | `static System.Void Wait(System.Int32 ms)` | — |
| `GTA.Script.Aborted` | 脚本中止事件 | critical | verified | `System.EventHandler Aborted` | — |
| `GTA.Script.Abort()` | 主动中止脚本 | secondary | verified | `System.Void Abort()` | — |
| `Vehicle.MaxEngineHealth` | 获取车辆最大引擎血量 | critical | needs_query | 未在 SHVDN 目录中找到 | 使用固定值 `1000.0f` 作为默认最大引擎血量 |
| WebSocket `ClientWebSocket` | .NET 标准 WebSocket 客户端连接 | critical | fallback | .NET BCL，非 SHVDN 范围 | 使用 `System.Net.WebSockets.ClientWebSocket` |
| LIVE Studio `WebcastChatMessage` | 解析评论消息体 | critical | fallback | LIVE Studio SDK，非 SHVDN 范围 | 手动解析 JSON：提取 `content` 或 `comment` 字段作为评论文本 |
| `serviceSignalSub` / `IM_MESSAGE_TRANSPORT` | LIVE Studio 订阅协议 | critical | fallback | LIVE Studio SDK，非 SHVDN 范围 | 按 LIVE Studio 协议手动构造 JSON 订阅消息 |

## LIVE Studio 事件与礼物映射

### gift_requirements

| 字段 | 值 |
|---|---|
| original_query | — |
| keywords | — |
| status | not_required |
| confirmed_name | — |
| gift_id | — |
| diamondCount | — |
| candidates | — |
| fallback | — |

### 事件订阅

| 事件类型 | 订阅方式 | 处理 |
|---|---|---|
| `WebcastChatMessage` | 通过 `serviceSignalSub` 订阅 `IM_MESSAGE_TRANSPORT` | 解析 JSON，提取评论文本，匹配 `repair` / `boost` |
| 其他消息（礼物、点赞、关注、入场、分享） | — | 忽略，不做任何处理 |

## 计时与冷却

| 项目 | 值 | 实现方式 |
|---|---|---|
| repair 冷却 | 8 秒 | `DateTime` 或 `Stopwatch` 记录上次执行时间，执行前检查差值 ≥ 8s |
| boost 冷却 | 8 秒 | 同上，独立于 repair |
| boost 持续时间 | 4 秒 | `Task.Delay(4000)` 或 `Script.Wait` 循环累加，到期后恢复原始 EnginePowerMultiplier |
| 冷却精度 | 秒级 | 使用 `DateTime.UtcNow` 或 `Environment.TickCount` |

## 失败与清理

| 场景 | 清理行为 |
|---|---|
| WebSocket 连接失败 | 记录日志，释放 socket，脚本继续运行（降级为空闲等待）；可配置重试次数（建议 3 次，间隔 5 秒） |
| WebSocket 异常断开 | 触发重连或进入降级模式；确保旧连接释放 |
| Script.Aborted 触发 | 1. 取消所有异步 Task（CancellationTokenSource.Cancel） 2. 关闭 WebSocket 连接（CloseAsync + Dispose） 3. 如果 boost 处于激活状态，恢复 EnginePowerMultiplier 为原始值 4. 释放 CancellationTokenSource |
| 玩家不在车辆中 | `CurrentVehicle` 为 null 时静默忽略指令，不执行效果，不进入冷却 |
| 引擎血量超限 | `EngineHealth` 设置时 clamp 到 `[0, MaxEngineHealth]` 范围，使用 fallback 最大值 1000.0f |
| boost 恢复时车辆已销毁 | 检查 `CurrentVehicle` 是否仍存在且为同一实例，不存在则跳过恢复 |

## 降级方案

| 场景 | 降级行为 |
|---|---|
| WebSocket 不可用 | 脚本以无功能模式运行，仅保持 Script.Wait 循环，不崩溃 |
| `MaxEngineHealth` 不可用 | 使用固定值 `1000.0f` 作为最大引擎血量 |
| `WebcastChatMessage` 格式未知 | 尝试按常见 LIVE Studio JSON 格式解析（查找 `content`、`comment`、`text` 字段），均失败则忽略该消息 |
| 冷却/计时因系统时间跳变异常 | 使用 `Environment.TickCount`（毫秒级单调递增）替代 `DateTime` |

## 验收标准

1. 脚本启动后成功连接 `ws://127.0.0.1:60080`，发送订阅消息
2. 收到评论文本为 `repair`（任意大小写）时，玩家当前车辆 EngineHealth 增加 12%（基于最大引擎血量 1000.0f，即 +120），且不超过最大值
3. 收到评论文本为 `boost`（任意大小写）时，EnginePowerMultiplier 设为 1.8，4 秒后精确恢复为原始值
4. 同一指令 8 秒内重复触发被忽略（不执行、不重置冷却）
5. repair 和 boost 冷却互不干扰
6. 非 `repair`/`boost` 评论被忽略
7. 玩家不在车辆中时，指令被静默忽略
8. Script.Aborted / 脚本卸载时，WebSocket 连接关闭、异步任务取消、boost 状态恢复
9. WebSocket 连接失败时脚本不崩溃，进入降级模式
10. 不使用任何礼物、点赞、关注、入场、分享或 AI 功能