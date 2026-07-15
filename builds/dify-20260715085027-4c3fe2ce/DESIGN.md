<think>
用户需求明确：

1. 创建一个最小可玩的 GTA5 LIVE Studio Mod
2. 当用户评论 "repair" 时，玩家当前载具引擎生命值恢复 10%
3. 当用户评论 "boost" 时，当前载具短暂加速 3 秒
4. 两个指令分别有 10 秒冷却
5. 脚本中止时清理所有临时状态
6. 不使用礼物、点赞、关注或其他事件

首先，我需要确定 SHVDN API 能力。需要查询的核心能力：

1. 获取玩家当前载具 (Game.Player.Character.CurrentVehicle 或类似)
2. 修复载具引擎生命值 (Vehicle.EngineHealth 或 Vehicle.Repair)
3. 载具加速能力 (可能是设置速度、修改引擎扭矩、或应用力)
4. 冷却计时 (可以用 .NET 的 Timer 或游戏内时间)
5. 脚本中止清理 (Script.Aborted 事件或类似)
6. 聊天消息监听（LIVE Studio 的 OnComment 事件）

让我分两批查询：

第一批：核心载具操作能力 - 获取当前载具、引擎生命值修复、加速相关
第二批：LIVE Studio 事件、冷却管理和清理

先查询 SHVDN 和礼物。用户明确说不要使用礼物，所以礼物工具不需要调用。gift_requirements 标记为 not_required。

现在开始 SHVDN 查询。

第一批：载具相关核心能力
</think>
<think>
第一批结果：

已确认：
- GTA.Game.Player (static property, returns GTA.Player)
- GTA.Player.Character (property, returns GTA.Ped)
- GTA.Ped.CurrentVehicle (property, returns GTA.Vehicle)
- GTA.Vehicle.EngineHealth (property, System.Single, get/set)
- GTA.Vehicle.Repair() (method, void)
- GTA.Vehicle.Acceleration (property, get only, System.Single)

需要第二批查询：
- 加速相关：设置速度、应用力、forward speed、set forward speed、Speed property on Vehicle
- LIVE Studio 事件：OnComment 或 chat message listener
- 脚本中止清理：Script.Aborted 事件
- 冷却计时：.NET Timer 或 Yield 或 Wait

让我查询第二批。

我需要查询：
1. 加速设置 - Vehicle.Speed, Vehicle.SetForwardSpeed, Vehicle.MaxSpeed, 或者 Native 调用
2. LIVE Studio 评论事件 - 可能是 OnChatMessage 或类似
3. 脚本中止 - Script.Aborted 或 KeyDown/KeyUp 相关
4. 计时器 - 可能用 Coroutine/Yield 或 System.Threading.Timer

需要确认 Vehicle 有没有 Speed 属性可以 set，或者是否可以通过其他方式加速。另外需要确认 Vehicle.Acceleration 是只读的，所以不能直接设置加速。可能需要用其他方式：比如增加引擎扭矩、设置速度倍率、或者直接修改速度向量。

对于 "短暂加速 3 秒"，一个常见做法是修改 Vehicle 的引擎能力乘数 (EnginePowerMultiplier) 或在短时间内提升速度。

查询第二批：

- Vehicle.Speed
- Vehicle.EnginePowerMultiplier
- Vehicle.SetForwardSpeed
- Script.Aborted
- OnComment / chat event
- Wait / Yield / Timer
</think>
<think>
第二批结果确认：

- GTA.Vehicle.EnginePowerMultiplier (System.Single, get/set) - 可用于修改引擎功率
- GTA.Vehicle.ForwardSpeed (System.Single, set only) - 设置前进速度
- GTA.Entity.MaxSpeed (System.Single, set only) - 设置最大速度
- GTA.Script.Aborted (event, System.EventHandler) - 脚本中止事件
- GTA.Script.Wait (static void, int ms) - 等待

现在还需要确认 LIVE Studio 的评论事件。这需要查询 SHVDN 中的 LIVE Studio 相关 API。LIVE Studio 可能有一些特定的事件处理，比如 OnComment 或 OnChatMessage。让我思考一下...

在 SHVDN + LIVE Studio 模板中，通常有一个 `LIVEStudio` 类或类似的，可能有 `OnComment` 事件。但这是 LIVE Studio 特定的，可能不在标准 SHVDN API 中。通常 LIVE Studio 集成是通过 `GTA.Script` 基类的事件或自定义类来处理的。

让我尝试查询 LIVE Studio 相关 API。

但我已经用了 2 次 SHVDN 查询预算。不过 SHVDN 工具说最多调用 2 次，我已经用了 2 次。不能再调用 SHVDN 了。

对于 LIVE Studio 的评论事件，这通常是通过模板提供的。在 LIVE Studio Mod 模板中，通常使用 `LIVEStudio.OnComment` 或类似机制。由于这不在标准 SHVDN API 中，我可以标记为 needs_query，然后在 DESIGN.md 中说明。

但让我再看看：用户要求是 "SHVDN API 能力检索" 来确认。我已经用了 2 次。现在需要处理剩下的需求。

对于 LIVE Studio 事件，在 LIVE Studio 模板中，通常做法是重写 Script 的 OnChatMessage 或使用事件。但是标准 SHVDN 可能没有直接的 OnComment 事件。

实际上在 GTA5 LIVE Studio 中，评论处理通常是通过 `LIVEStudio.Comments` 或类似机制。由于这无法通过 SHVDN 工具确认，我需要在 capability_requirements 中标记为 needs_query，同时提供 fallback 方案（比如假设存在 `OnCommentReceived` 事件或通过轮询方式）。

不过，根据经验，LIVE Studio 的 Script 模板通常有一个 `OnComment` 方法或事件。在设计文档中，我可以描述这个需求但标记状态。

现在汇总所有信息：

已确认的 API：
- GTA.Game.Player → GTA.Player
- GTA.Player.Character → GTA.Ped
- GTA.Ped.CurrentVehicle → GTA.Vehicle
- GTA.Vehicle.EngineHealth (get/set, float)
- GTA.Vehicle.Repair() (void)
- GTA.Vehicle.EnginePowerMultiplier (get/set, float)
- GTA.Vehicle.ForwardSpeed (set, float)
- GTA.Entity.MaxSpeed (set, float)
- GTA.Vehicle.Acceleration (get, float, readonly)
- GTA.Script.Aborted (event)
- GTA.Script.Wait(int ms) (static void)

未确认但需要的：
- LIVE Studio 评论接收事件（OnComment 或类似）

对于加速效果，有几种策略：
1. 使用 EnginePowerMultiplier 临时提升（比如设成 2.0 或 3.0），3 秒后恢复
2. 使用 ForwardSpeed 直接设置速度
3. 组合使用 MaxSpeed 和 EnginePowerMultiplier

最简单且最自然的方式是使用 EnginePowerMultiplier：临时设置为一个高值（如 2.0 或更高），3 秒后恢复默认值（通常是 1.0）。

或者也可以使用 ForwardSpeed 直接增加速度。但 EnginePowerMultiplier 更加自然，因为它让引擎本身更强力。

对于 "短暂加速 3 秒"，我的设计是：
- boost 触发时，将当前载具的 EnginePowerMultiplier 设置为 2.0（或类似值）
- 启动一个 3 秒计时器
- 3 秒后恢复 EnginePowerMultiplier 到原始值（1.0）

对于 repair：
- 获取当前 EngineHealth
- 设置为 min(EngineHealth + 10% of max, maxEngineHealth)
- 或者简单设置为 EngineHealth + 100（如果默认最大是 1000）

实际上 EngineHealth 范围通常是 0 到 1000（1000 = 满），所以 10% = 100。

现在来写 DESIGN.md。

关于 LIVE Studio 事件，我需要标记为 needs_query。在 fallback 中，我会描述假设的事件模型。

礼物方面：用户明确不要礼物，所以 gift_requirements 标记为 not_required。

现在来组织 DESIGN.md：

1. 需求摘要
2. change_summary（首次生成，标注为 N/A 或首次）
3. preserved_behaviors（首次生成为空）
4. 玩法流程
5. 状态机
6. 触发条件
7. 效果表
8. capability_requirements 表
9. LIVE Studio 事件与礼物映射
10. 计时与冷却
11. 失败与清理
12. 降级方案
13. 验收标准

对于加速策略，我选择 EnginePowerMultiplier 方案，因为：
- 它是最自然的加速方式
- 不需要每帧更新速度
- 只需在开始时设置，结束时恢复

但我也需要考虑：如果玩家在加速期间下车或切换载具怎么办？需要在清理时处理。

另一个方案：使用 ForwardSpeed。设置 ForwardSpeed 为当前速度的某个倍数。但这需要在一次调用中完成，不会持续。

EnginePowerMultiplier 方案更好，因为它是持续的。但 3 秒后需要恢复。

对于计时，使用：
- DateTime 或 Stopwatch 检查冷却
- Script.Wait 或 Coroutine 处理 3 秒延迟

由于这是单文件 Mod，我们可以使用简单的 DateTime 比较来处理冷却，使用 Script.Wait 或 Task.Delay 来处理 3 秒加速持续时间。

但在 SHVDN 中，Script.Wait 会暂停整个脚本执行。更好的做法可能是：
- 使用一个计时变量和 OnTick 来检查
- 或者使用 C# 的 async/await 与 Task.Delay

但 SHVDN v3 的 Script 类支持 async，可以使用 await Script.Wait(3000) 在异步上下文中。

对于冷却，我可以使用 Dictionary<string, DateTime> 来跟踪每个指令的最后使用时间。

关于 LIVE Studio 评论事件：在没有确认 API 的情况下，我假设存在类似 `LIVEStudio.OnComment` 事件或 `OnChatCommand` 事件。常见的 LIVE Studio 模板中，Script 子类可以重写或订阅评论事件。

让我为这个标记为 needs_query。

现在编写完整的 DESIGN.md。

关于 EngineHealth：修复 10%。EngineHealth 的范围通常是 -4000 到 1000（在 GTA5 中，1000 是满健康）。但通常最大是 1000。修复 10% 意味着 +100（如果最大是 1000）。

但更准确的做法是：获取当前 EngineHealth，计算 10% 的 max（1000），然后加到当前值，上限为 1000。

公式：newHealth = Math.Min(currentHealth + 100, 1000)。

OK，现在写 DESIGN.md。
</think># DESIGN.md

## 需求摘要

- **玩法名称**: 载具指令修复与加速 Mod
- **核心目标**: 通过特定聊天评论触发玩家当前载具的引擎修复或短暂加速。
- **胜利条件**: 无（纯功能性交互，非竞速/得分型玩法）
- **失败条件**: 无
- **时长/节奏**: 每个指令独立冷却 10 秒；加速持续 3 秒；脚本中止时自动清理所有计时器和状态。
- **正向互动**: 
  - 评论 `repair` → 恢复玩家当前载具引擎生命值 10%（+100，上限 1000）
  - 评论 `boost` → 使玩家当前载具引擎功率临时提升至 2.0 倍，持续 3 秒
- **负向互动**: 无
- **礼物要求**: 不使用礼物、点赞、关注或其他事件
- **评论触发**: 仅通过聊天评论 `repair` 和 `boost` 触发（大小写不敏感，精确匹配或包含匹配）
- **需要避免的复杂实现**: 不引入多层状态机、不依赖外部资源、不使用除 `repair`/`boost` 外的任何指令或事件

## change_summary

首次生成，无变更。

## preserved_behaviors

首次生成，无保留行为。

---

## 玩法流程

```
[观众发送评论] 
    ↓
[LIVE Studio 评论事件触发 OnCommentReceived(string comment)]
    ↓
[标准化评论：Trim + ToLowerInvariant]
    ↓
[匹配指令]
    ├── "repair" → 进入 Repair 流程
    └── "boost"  → 进入 Boost 流程
```

### Repair 流程

1. 检查 `repair` 冷却是否已过 10 秒；若冷却中则忽略
2. 获取 `Game.Player.Character.CurrentVehicle`
3. 若载具不存在或已损坏/无效，忽略
4. 读取当前 `Vehicle.EngineHealth`
5. 计算新值: `newHealth = Math.Min(currentHealth + 100f, 1000f)`
6. 设置 `Vehicle.EngineHealth = newHealth`
7. 记录本次触发时间戳，开始 10 秒冷却

### Boost 流程

1. 检查 `boost` 冷却是否已过 10 秒；若冷却中则忽略
2. 获取 `Game.Player.Character.CurrentVehicle`
3. 若载具不存在或已损坏/无效，忽略
4. 保存原始 `EnginePowerMultiplier` 值
5. 设置 `Vehicle.EnginePowerMultiplier = 2.0f`
6. 记录本次触发时间戳，开始 10 秒冷却
7. 启动 3 秒异步延迟 (`await Script.Wait(3000)`)
8. 3 秒后恢复 `EnginePowerMultiplier` 为原始值（若载具仍存在且为同一辆）

---

## 状态机

| 状态 | 描述 | 转移条件 |
|------|------|----------|
| `Idle` | 等待评论 | 收到评论 → 匹配指令 |
| `RepairCooldown` | repair 冷却中（10 秒） | 冷却到期 → Idle |
| `BoostActive` | boost 加速生效中（3 秒） | 加速到期 → 恢复 EnginePowerMultiplier → Idle |
| `BoostCooldown` | boost 冷却中（10 秒） | 冷却到期 → Idle |

- **repair** 与 **boost** 冷却独立，互不影响。
- `BoostActive` 与 `BoostCooldown` 可重叠：加速 3 秒期间冷却也在倒计时。
- 同一指令在冷却期间收到的新评论被静默忽略。

---

## 触发条件

| 触发源 | 条件 | 行为 |
|--------|------|------|
| LIVE Studio 评论事件 | 标准化后的评论 == "repair" | 执行 Repair 流程 |
| LIVE Studio 评论事件 | 标准化后的评论 == "boost" | 执行 Boost 流程 |
| LIVE Studio 评论事件 | 其他任意内容 | 忽略 |
| 点赞/关注/礼物事件 | 任意 | 不监听、不处理 |

评论匹配规则：对评论字符串执行 `Trim()` + `ToLowerInvariant()`，然后与 `"repair"` 和 `"boost"` 做精确字符串比较。不接受子串匹配（避免 `repairing`、`booster` 等误触发）。

---

## 效果表

| 指令 | 效果 | 参数 | 持续时间 | 冷却 | 备注 |
|------|------|------|----------|------|------|
| `repair` | 引擎生命值恢复 10% | +100 HP，上限 1000 | 瞬时 | 10 秒 | 仅对玩家当前载具有效 |
| `boost` | 引擎功率倍增至 2.0x | EnginePowerMultiplier=2.0 | 3 秒 | 10 秒 | 到期自动恢复原始倍率 |

---

## capability_requirements

| capability | behavior | priority | status | verified_evidence | fallback |
|------------|----------|----------|--------|-------------------|----------|
| 获取玩家当前载具 | `Game.Player.Character.CurrentVehicle` → `GTA.Vehicle` | critical | verified | `GTA.Game.Player`(static), `GTA.Player.Character`→`GTA.Ped`, `GTA.Ped.CurrentVehicle`→`GTA.Vehicle` | N/A |
| 读取/设置引擎生命值 | `Vehicle.EngineHealth { get; set; }` (System.Single) | critical | verified | `GTA.Vehicle.EngineHealth` get/set, score 88 | N/A |
| 读取/设置引擎功率倍率 | `Vehicle.EnginePowerMultiplier { get; set; }` (System.Single) | critical | verified | `GTA.Vehicle.EnginePowerMultiplier` get/set, score 88 | N/A |
| 异步延迟（3 秒加速） | `Script.Wait(int ms)` 静态方法 | critical | verified | `GTA.Script.Wait` static void, score 88 | 使用 `System.Threading.Tasks.Task.Delay` + async |
| 脚本中止清理 | `Script.Aborted` 事件 | high | verified | `GTA.Script.Aborted` EventHandler, score 88 | 在 `Dispose` 或 `OnAbort` 中手动清理 |
| LIVE Studio 评论接收 | 监听聊天评论事件，获取评论文本 | critical | needs_query | 未在 SHVDN 标准 API 中确认；LIVE Studio 模板通常提供 `OnComment` 或 `OnChatMessage` 事件 | 假设存在 `LIVEStudio.OnCommentReceived(string)` 事件；若不存在则改用轮询 `LIVEStudio.GetRecentComments()` |
| 冷却计时 | 使用 `DateTime.UtcNow` 或 `Stopwatch` 记录时间戳 | high | verified | C# 标准库 `System.DateTime`，无需 SHVDN 能力 | N/A |
| 脚本入口 | 继承 `GTA.Script` 并 override `OnTick` 或使用 async Main | critical | verified | SHVDN v3 `GTA.Script` 基类 | N/A |

---

## LIVE Studio 事件与礼物映射

### 事件监听

| 事件 | 用途 | 状态 |
|------|------|------|
| 评论接收事件 | 获取观众评论文本，触发 repair/boost | needs_query（假设 `OnCommentReceived` 或等效事件） |
| 点赞事件 | 不监听 | not_required |
| 关注事件 | 不监听 | not_required |
| 礼物事件 | 不监听 | not_required |

### 礼物映射

gift_requirements: **not_required** — 用户明确禁止使用礼物。

| 字段 | 值 |
|------|-----|
| original_query | N/A |
| keywords | N/A |
| status | not_required |
| confirmed_name | N/A |
| gift_id | N/A |
| diamondCount | N/A |
| candidates | N/A |
| fallback | N/A |

---

## 计时与冷却

| 计时器 | 触发条件 | 时长 | 行为 |
|--------|----------|------|------|
| `_lastRepairTime` | repair 执行时记录 `DateTime.UtcNow` | 冷却 10 秒 | 冷却期间忽略 repair 评论 |
| `_lastBoostTime` | boost 执行时记录 `DateTime.UtcNow` | 冷却 10 秒 | 冷却期间忽略 boost 评论 |
| `_boostRestoreTask` | boost 执行时启动 | 3 秒异步延迟 | 到期恢复 `EnginePowerMultiplier` |

冷却检查逻辑：
```csharp
bool CanRepair => (DateTime.UtcNow - _lastRepairTime).TotalSeconds >= 10.0;
bool CanBoost  => (DateTime.UtcNow - _lastBoostTime).TotalSeconds >= 10.0;
```

加速恢复逻辑（伪代码）：
```
保存 originalMultiplier = vehicle.EnginePowerMultiplier
设置 vehicle.EnginePowerMultiplier = 2.0f
await Script.Wait(3000)
如果 vehicle 仍然存在且为同一实例:
    vehicle.EnginePowerMultiplier = originalMultiplier
```

---

## 失败与清理

### 运行时边界条件

| 条件 | 处理 |
|------|------|
| 玩家不在载具中 (`CurrentVehicle == null`) | 忽略指令，不触发冷却 |
| 载具已损坏/已删除 (`!vehicle.Exists()`) | 忽略指令，不触发冷却 |
| EngineHealth 已满 (≥1000) | repair 仍可执行，但值不变（钳制到 1000） |
| EngineHealth 极低（接近 -4000） | repair 正常加 100 |
| 加速期间玩家切换载具 | 旧载具恢复原始 EnginePowerMultiplier；新载具不受影响 |
| 加速期间玩家下车 | 原载具仍恢复原始 EnginePowerMultiplier |
| 冷却期间收到重复指令 | 静默忽略 |

### 脚本中止清理

在 `Script.Aborted` 事件处理中执行：

1. 若存在活跃的 boost 恢复任务，取消并立即恢复原始 `EnginePowerMultiplier`
2. 清空所有计时器引用
3. 移除所有事件订阅

清理逻辑确保：
- 不会残留修改过的 `EnginePowerMultiplier` 在玩家载具上
- 所有事件处理器被正确解除订阅，避免内存泄漏

---

## 降级方案

| 场景 | 降级策略 |
|------|----------|
| LIVE Studio 评论事件不可用 | 使用轮询方式定期检查新评论（如每 500ms 通过 `LIVEStudio` API 拉取），功能不变但响应延迟增加 |
| `Script.Wait` 在同步上下文中不可用 | 使用 `Task.Delay` + `async/await`；若仍不可用，使用 `OnTick` 帧计数器模拟 3 秒延迟 |
| `EnginePowerMultiplier` 设置无效 | 改用 `Vehicle.ForwardSpeed` 瞬时提速（设置当前 ForwardSpeed × 1.3），一次性生效无持续 3 秒效果 |
| 载具不存在时冷却误触发 | 仅在成功执行效果后才记录时间戳，无效载具不消耗冷却 |

---

## 验收标准

1. **repair 指令**：观众在聊天中发送 `repair`，玩家当前载具引擎生命值增加 100（上限 1000），10 秒内重复发送被忽略。
2. **boost 指令**：观众在聊天中发送 `boost`，玩家当前载具引擎功率变为 2 倍，3 秒后自动恢复原值，10 秒内重复发送被忽略。
3. **大小写不敏感**：`Repair`、`REPAIR`、`repair` 均等效触发。
4. **冷却独立**：repair 和 boost 冷却互不影响，可交错触发。
5. **无载具时安全**：玩家步行时发送指令不报错、不崩溃、不触发冷却。
6. **脚本中止清理**：卸载/重载脚本时，所有修改过的载具属性恢复默认，无残留效果。
7. **最小实现**：单文件 `.cs`，仅依赖 SHVDN v3 和 LIVE Studio 模板，无额外程序集或资源。