<think>
用户需求很明确：这是一个 REVISE（修订）请求。需要将 repair 指令的车辆引擎修复量从 10% 改为 25%，其他所有行为保持不变。

让我回顾一下：

- 上一版 DESIGN.md：repair 修复 10%（+100，上限 1000）
- 上一版 Mod.cs：repair 修复量是 100f（10% of 1000）
- 本轮变化：改为 25%，即 1000 * 25% = 250

现在我需要：
1. 确认 25% 对应多少 EngineHealth 值。EngineHealth 上限是 1000，所以 25% = 250。
2. 确认其他所有行为保持不变：冷却 10 秒、boost 3 秒、boost EnginePowerMultiplier 2.0x、事件监听、清理逻辑等。

这是一个简单的数值修改。不需要重新查询 SHVDN API（因为所有能力已经在上一版中确认），也不需要查询礼物（因为用户明确不需要礼物）。

所以我不需要调用任何工具！直接生成修订后的 DESIGN.md。

让我整理一下变化：

- change_summary：将 repair 的引擎修复量从 10%（+100）改为 25%（+250）
- preserved_behaviors：冷却 10 秒、boost 3 秒持续时间、EnginePowerMultiplier 2.0x、评论匹配规则、LIVE Studio 事件、清理逻辑全部保持不变

现在我需要输出修订版的 DESIGN.md。关键修改点：

1. 需求摘要中的正向互动：repair 从 "10%（+100，上限 1000）" 改为 "25%（+250，上限 1000）"
2. 效果表中的 repair 参数从 "+100 HP" 改为 "+250 HP"
3. Repair 流程中的计算从 `Math.Min(currentHealth + 100f, 1000f)` 改为 `Math.Min(currentHealth + 250f, 1000f)`
4. 验收标准中的 repair 描述从 "增加 100" 改为 "增加 250"
5. HandleRepair 伪代码中的 RepairAmount 从 100f 改为 250f

capability_requirements 保持不变，因为所有能力已经验证过。gift_requirements 保持 not_required。

现在输出完整的 DESIGN.md。

注意：输出第一行必须是 "# DESIGN.md"，文档结束后不得有任何文字。

另外关于 SHVDN 查询和礼物查询：不需要。因为这是纯粹的数值修改，不涉及新能力。之前已经验证过所有需要的 API。

好的，直接输出修订版 DESIGN.md。
</think># DESIGN.md

## 需求摘要

- **玩法名称**: 载具指令修复与加速 Mod
- **核心目标**: 通过特定聊天评论触发玩家当前载具的引擎修复或短暂加速。
- **胜利条件**: 无（纯功能性交互，非竞速/得分型玩法）
- **失败条件**: 无
- **时长/节奏**: 每个指令独立冷却 10 秒；加速持续 3 秒；脚本中止时自动清理所有计时器和状态。
- **正向互动**: 
  - 评论 `repair` → 恢复玩家当前载具引擎生命值 25%（+250，上限 1000）
  - 评论 `boost` → 使玩家当前载具引擎功率临时提升至 2.0 倍，持续 3 秒
- **负向互动**: 无
- **礼物要求**: 不使用礼物、点赞、关注或其他事件
- **评论触发**: 仅通过聊天评论 `repair` 和 `boost` 触发（大小写不敏感，精确匹配）
- **需要避免的复杂实现**: 不引入多层状态机、不依赖外部资源、不使用除 `repair`/`boost` 外的任何指令或事件

## change_summary

| 变更项 | 上一版 | 本轮 |
|--------|--------|------|
| repair 引擎修复量 | 10%（+100 HP） | **25%（+250 HP）** |
| RepairAmount 常量 | 100f | **250f** |

## preserved_behaviors

| 保留行为 | 说明 |
|-----------|------|
| repair 冷却时间 | 10 秒，独立冷却 |
| boost 冷却时间 | 10 秒，独立冷却 |
| boost 持续时间 | 3 秒 |
| boost 引擎功率倍率 | EnginePowerMultiplier = 2.0f，到期恢复原始值 |
| 评论匹配规则 | Trim + ToLowerInvariant，精确匹配 "repair" / "boost" |
| 无效载具处理 | 无载具或载具无效时忽略指令，不触发冷却 |
| EngineHealth 上限 | 钳制到 1000f |
| 加速期间切换载具 | 旧载具正确恢复，新载具不受影响 |
| 脚本中止清理 | 恢复 EnginePowerMultiplier，取消所有事件订阅 |
| LIVE Studio 事件 | 仅监听评论事件，不处理点赞/关注/礼物 |
| 礼物映射 | not_required |

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
5. 计算新值: `newHealth = Math.Min(currentHealth + 250f, 1000f)`
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
| `repair` | 引擎生命值恢复 25% | +250 HP，上限 1000 | 瞬时 | 10 秒 | 仅对玩家当前载具有效 |
| `boost` | 引擎功率倍增至 2.0x | EnginePowerMultiplier=2.0 | 3 秒 | 10 秒 | 到期自动恢复原始倍率 |

---

## capability_requirements

| capability | behavior | priority | status | verified_evidence | fallback |
|------------|----------|----------|--------|-------------------|----------|
| 获取玩家当前载具 | `Game.Player.Character.CurrentVehicle` → `GTA.Vehicle` | critical | verified | `GTA.Game.Player`(static), `GTA.Player.Character`→`GTA.Ped`, `GTA.Ped.CurrentVehicle`→`GTA.Vehicle` | N/A |
| 读取/设置引擎生命值 | `Vehicle.EngineHealth { get; set; }` (System.Single) | critical | verified | `GTA.Vehicle.EngineHealth` get/set, score 88 | N/A |
| 读取/设置引擎功率倍率 | `Vehicle.EnginePowerMultiplier { get; set; }` (System.Single) | critical | verified | `GTA.Vehicle.EnginePowerMultiplier` get/set, score 88 | N/A |
| 异步延迟（3 秒加速恢复） | OnTick 帧计数器检查 `DateTime.UtcNow` | critical | verified | C# 标准库 `System.DateTime` + SHVDN `Script.Tick` 事件, score 88 | 使用 `Task.Delay` + async |
| 脚本中止清理 | `Script.Aborted` 事件 | high | verified | `GTA.Script.Aborted` EventHandler, score 88 | 在 `Dispose` 或 `OnAbort` 中手动清理 |
| LIVE Studio 评论接收 | 监听聊天评论事件，获取评论文本 | critical | needs_query | 未在 SHVDN 标准 API 中确认；LIVE Studio 模板通常提供 `OnComment` 或 `OnChatMessage` 事件 | 假设存在 `LIVEStudio.OnCommentReceived(string)` 事件；若不存在则改用轮询 |
| 冷却计时 | 使用 `DateTime.UtcNow` 记录时间戳 | high | verified | C# 标准库 `System.DateTime`，无需 SHVDN 能力 | N/A |
| 脚本入口 | 继承 `GTA.Script` 并订阅 `Tick` 事件 | critical | verified | SHVDN v3 `GTA.Script` 基类 | N/A |

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
| `_boostRestoreCheck` | boost 执行时在 OnTick 中检查 | 3 秒到期恢复 | 到期恢复 `EnginePowerMultiplier` |

冷却检查逻辑：
```csharp
bool CanRepair => (DateTime.UtcNow - _lastRepairTime).TotalSeconds >= 10.0;
bool CanBoost  => (DateTime.UtcNow - _lastBoostTime).TotalSeconds >= 10.0;
```

加速恢复逻辑（OnTick 帧检查）：
```
若 _boostActive 且 (DateTime.UtcNow - _boostStartTime).TotalSeconds >= 3.0:
    若 _boostVehicle 存在且有效:
        _boostVehicle.EnginePowerMultiplier = _originalMultiplier
    _boostActive = false; _boostVehicle = null
```

---

## 失败与清理

### 运行时边界条件

| 条件 | 处理 |
|------|------|
| 玩家不在载具中 (`CurrentVehicle == null`) | 忽略指令，不触发冷却 |
| 载具已损坏/已删除 (`!vehicle.Exists()`) | 忽略指令，不触发冷却 |
| EngineHealth 已满 (≥1000) | repair 仍可执行，但值不变（钳制到 1000） |
| EngineHealth 极低（接近 -4000） | repair 正常加 250 |
| 加速期间玩家切换载具 | 旧载具恢复原始 EnginePowerMultiplier；新载具不受影响 |
| 加速期间玩家下车 | 原载具仍恢复原始 EnginePowerMultiplier |
| 冷却期间收到重复指令 | 静默忽略 |

### 脚本中止清理

在 `Script.Aborted` 事件处理中执行：

1. 若存在活跃的 boost 效果，立即恢复原始 `EnginePowerMultiplier`
2. 清空所有计时器引用和状态标志
3. 移除所有事件订阅（`LIVEStudio.OnCommentReceived`、`Tick`、`Aborted`）

清理逻辑确保：
- 不会残留修改过的 `EnginePowerMultiplier` 在玩家载具上
- 所有事件处理器被正确解除订阅，避免内存泄漏

---

## 降级方案

| 场景 | 降级策略 |
|------|----------|
| LIVE Studio 评论事件不可用 | 使用轮询方式定期检查新评论（如每 500ms 通过 `LIVEStudio` API 拉取），功能不变但响应延迟增加 |
| `Script.Wait` 在同步上下文中不可用 | 使用 OnTick + `DateTime.UtcNow` 帧计数器（当前实现已采用此方案） |
| `EnginePowerMultiplier` 设置无效 | 改用 `Vehicle.ForwardSpeed` 瞬时提速（设置当前 ForwardSpeed × 1.3），一次性生效无持续 3 秒效果 |
| 载具不存在时冷却误触发 | 仅在成功执行效果后才记录时间戳，无效载具不消耗冷却 |

---

## 验收标准

1. **repair 指令**：观众在聊天中发送 `repair`，玩家当前载具引擎生命值增加 250（上限 1000），10 秒内重复发送被忽略。
2. **boost 指令**：观众在聊天中发送 `boost`，玩家当前载具引擎功率变为 2 倍，3 秒后自动恢复原值，10 秒内重复发送被忽略。
3. **大小写不敏感**：`Repair`、`REPAIR`、`repair` 均等效触发。
4. **冷却独立**：repair 和 boost 冷却互不影响，可交错触发。
5. **无载具时安全**：玩家步行时发送指令不报错、不崩溃、不触发冷却。
6. **脚本中止清理**：卸载/重载脚本时，所有修改过的载具属性恢复默认，无残留效果。
7. **最小实现**：单文件 `.cs`，仅依赖 SHVDN v3 和 LIVE Studio 模板，无额外程序集或资源。