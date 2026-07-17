<think>
用户要求：把 repair 修复比例从 12% 改为 30%，boost 持续时间从 4 秒改为 6 秒；其他全部保持不变。

这是 REVISE 模式。我需要基于上一版 DESIGN.md 进行增量修改。

上一版 DESIGN.md 的关键参数：
- repair: 引擎血量 +12%（基于最大引擎血量 1000.0f，即 +120），冷却 8 秒
- boost: EnginePowerMultiplier 设为 1.8，持续 4 秒，冷却 8 秒

本轮变化：
- repair 修复比例由 12% 改为 30%
- boost 持续时间由 4 秒改为 6 秒

其余不变：
- boost 倍率 1.8
- 两个指令 8 秒冷却
- 精确的 service/method 订阅 JSON
- WebcastChatMessage 解析
- 重连与 Aborted 清理

我不需要调用任何工具，因为这些 API 能力在上一版已经验证过了。我只是修改参数值。

现在输出修订版 DESIGN.md。
</think># DESIGN.md

## 需求摘要

- **玩法名称**: LIVE Studio 评论指令 Mod（Repair & Boost）— 修订版
- **核心目标**: 监听 LIVE Studio 真实评论，解析 `repair` 与 `boost` 文本指令，在 GTA5 中对玩家当前车辆施加引擎修复或临时动力提升效果。
- **胜利条件**: 无（纯互动效果 Mod）
- **失败条件**: 无
- **时长/节奏**: 指令冷却 8 秒；boost 效果持续 6 秒后自动恢复原始倍率
- **正向互动**: `repair` → 引擎血量 +30%（基于最大引擎血量）；`boost` → EnginePowerMultiplier 临时设为 1.8 倍
- **负向互动**: 无
- **礼物要求**: 不需要
- **评论/指令触发**: 仅处理评论正文匹配 `repair` 或 `boost`（大小写不敏感、trim 后精确匹配）；忽略礼物、点赞、关注、入场、分享等所有其他消息类型

## change_summary

| 项目 | 旧值 | 新值 | 变更原因 |
|---|---|---|---|
| repair 修复比例 | 12%（即 MaxEngineHealth × 0.12） | 30%（即 MaxEngineHealth × 0.30） | 用户要求 |
| boost 持续时间 | 4 秒（BoostDurationMs = 4000） | 6 秒（BoostDurationMs = 6000） | 用户要求 |

## preserved_behaviors

| 行为 | 说明 |
|---|---|
| boost 倍率 1.8 | `EnginePowerMultiplier` 临时设为 1.8，保持不变 |
| repair 冷却 8 秒 | `lastRepairTick` 独立冷却，保持不变 |
| boost 冷却 8 秒 | `lastBoostTick` 独立冷却，保持不变 |
| WebSocket 端点与订阅 | `ws://127.0.0.1:60080`，连接后发送 `{"service":"serviceSignalSub","method":"IM_MESSAGE_TRANSPORT"}`，保持不变 |
| WebcastChatMessage 解析 | 从 JSON 提取 `content`/`comment`/`text` 字段，保持不变 |
| 重连策略 | 断开后等待 5 秒重连，保持不变 |
| Aborted 清理 | 取消任务、关闭 WebSocket、恢复 boost 状态、释放资源，保持不变 |
| 玩家不在车辆中 | 静默忽略指令，不进入冷却，保持不变 |
| 最大引擎血量 fallback | `1000.0f`，保持不变 |
| 无礼物/点赞/关注/入场/分享处理 | 全部忽略，保持不变 |
| 精确匹配逻辑 | `trim + ToLowerInvariant` 后精确匹配 `repair` / `boost`，保持不变 |

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
    │       │                     │       ├─ == "repair" ──► 检查冷却 → 执行 repair（+30%）
    │       │                     │       │
    │       │                     │       └─ == "boost"  ──► 检查冷却 → 执行 boost（1.8× 持续 6s）
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
| `RepairExecuting` | 收到 `repair` 评论且冷却已过 | 读取 CurrentVehicle.EngineHealth，计算目标值（当前 + 最大×30%），设置 EngineHealth | 设置完成，回到 Idle |
| `BoostActive` | 收到 `boost` 评论且冷却已过 | 记录原始 EnginePowerMultiplier，设为 1.8，启动 6 秒计时器 | 6 秒到期或 Aborted |
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
| `repair` | 引擎修复 | `Vehicle.EngineHealth` (set) | 当前值 + (最大引擎血量 × 0.30)，上限为最大引擎血量 | 即时 | 8 秒 |
| `boost` | 动力提升 | `Vehicle.EnginePowerMultiplier` (set) | 1.8 | 6 秒后恢复 | 8 秒 |

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
| repair 冷却 | 8 秒 | `Environment.TickCount` 记录上次执行时间，执行前检查差值 ≥ 8000ms |
| boost 冷却 | 8 秒 | 同上，独立于 repair |
| boost 持续时间 | 6 秒 | `boostEndTick = now + 6000`，OnTick 中检查到期后恢复 |
| 冷却精度 | 毫秒级 | 使用 `Environment.TickCount`（unchecked 减法容忍溢出） |

## 失败与清理

| 场景 | 清理行为 |
|---|---|
| WebSocket 连接失败 | 记录日志，释放 socket，脚本继续运行（降级为空闲等待）；重试 3 次，间隔 5 秒 |
| WebSocket 异常断开 | 触发重连或进入降级模式；确保旧连接释放 |
| Script.Aborted 触发 | 1. 取消所有异步 Task（CancellationTokenSource.Cancel） 2. 等待 receiveTask 完成（最多 3 秒） 3. 关闭 WebSocket 连接（CloseAsync + Dispose） 4. 如果 boost 处于激活状态，恢复 EnginePowerMultiplier 为原始值 5. 释放 CancellationTokenSource |
| 玩家不在车辆中 | `CurrentVehicle` 为 null 时静默忽略指令，不执行效果，不进入冷却 |
| 引擎血量超限 | `EngineHealth` 设置时 clamp 到 `[0, MaxEngineHealth]` 范围，使用 fallback 最大值 1000.0f |
| boost 恢复时车辆已销毁 | 检查 `boostedVehicle.Exists()`，不存在则跳过恢复 |

## 降级方案

| 场景 | 降级行为 |
|---|---|
| WebSocket 不可用 | 脚本以无功能模式运行，仅保持 Script.Wait 循环，不崩溃 |
| `MaxEngineHealth` 不可用 | 使用固定值 `1000.0f` 作为最大引擎血量 |
| `WebcastChatMessage` 格式未知 | 尝试按常见 LIVE Studio JSON 格式解析（查找 `content`、`comment`、`text` 字段），均失败则忽略该消息 |
| 冷却/计时因系统时间跳变异常 | 使用 `Environment.TickCount`（毫秒级单调递增）替代 `DateTime` |

## 验收标准

1. 脚本启动后成功连接 `ws://127.0.0.1:60080`，发送订阅消息
2. 收到评论文本为 `repair`（任意大小写）时，玩家当前车辆 EngineHealth 增加 30%（基于最大引擎血量 1000.0f，即 +300），且不超过最大值
3. 收到评论文本为 `boost`（任意大小写）时，EnginePowerMultiplier 设为 1.8，6 秒后精确恢复为原始值
4. 同一指令 8 秒内重复触发被忽略（不执行、不重置冷却）
5. repair 和 boost 冷却互不干扰
6. 非 `repair`/`boost` 评论被忽略
7. 玩家不在车辆中时，指令被静默忽略
8. Script.Aborted / 脚本卸载时，WebSocket 连接关闭、异步任务取消、boost 状态恢复
9. WebSocket 连接失败时脚本不崩溃，进入降级模式
10. 不使用任何礼物、点赞、关注、入场、分享或 AI 功能