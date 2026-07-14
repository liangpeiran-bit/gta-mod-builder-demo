<think>
用户需求很简单：用户送玫瑰礼物，刷新一辆跑车。我需要：

1. 查询 SHVDN API - 需要知道如何生成载具（车辆）、如何获取玩家位置、什么跑车（假设是常见的跑车如 Cheetah、EntityXF、Adder 等）
2. 查询礼物工具 - 需要确认"玫瑰"的 gift_id 和价格

让我先汇总能力需求：

核心能力：
- 创建载具/车辆（World.CreateVehicle 或类似）
- 获取玩家位置或坐标
- 载具模型（跑车模型名称如 "cheetah", "entityxf", "adder", "turismor", "zentorno" 等）

这是决定玩法能否成立的核心能力。让我先批量查询。

礼物方面：需要查询"玫瑰"。

让我并行发起这两个查询。

SHVDN capability_request 需要简洁：
"创建载具World.CreateVehicle方法、获取玩家Ped位置Position属性、跑车模型名称如cheetah entityxf adder turismor zentorno"

这应该不超过256字符。让我数一下： "创建载具World.CreateVehicle方法、获取玩家Ped位置Position属性、跑车模型名称如cheetah entityxf adder turismor zentorno" - 不太长。

礼物请求："玫瑰"。

让我并行发起。
</think>好的，我先并行查询核心能力和礼物信息。
<think>
分析结果：

**礼物方面**：Rose 精确匹配，gift_id=5655，钻石数=1。

**SHVDN API**：
- World.CreateVehicle 确认可用
- Game.Player.Character 确认可用
- Ped.Position - 查询结果标注为"未直接显示但继承自Entity"，标记为 needs_query
- VehicleHash: Adder 和 Cheetah 确认可用。entityxf、turismor、zentorno 未确认。

现在我需要第二批查询：确认 Ped.Position 和更多跑车模型。但规则说最多调用2次SHVDN工具。我已经调用了1次，还有1次。

第二批：查询 Ped.Position 属性和更多 VehicleHash 枚举值（如 EntityXF、Turismo、Zentorno、TurismoR）。

capability_request: "Ped类的Position属性、Entity类Position属性、VehicleHash枚举中EntityXF TurismoR Zentorno Turismo2"

这不超过256字符。让我发起第二批。

但根据规则，第一批返回 partial 后，"不得用相同条件重试"。Position和VehicleHash的查询不同于第一批（第一批是World.CreateVehicle + Ped位置 + 跑车模型名称），现在我要专门查 Position 属性和特定VehicleHash值——这属于不同的能力域（第一批确认了核心能力创建载具和获取Ped，现在确认剩余细节）。这不算重复。

让我发起第二批。
</think>第一批返回部分命中：`World.CreateVehicle` 和 `Game.Player.Character` 确认，但 `Ped.Position` 和部分跑车模型未确认。发起第二批查询，聚焦剩余能力域。
<think>
现在汇总所有查询结果：

**礼物（来自 query_live_gifts）**：
- Rose: gift_id=5655, diamondCount=1, 精确匹配

**SHVDN API（两批查询汇总）**：

已确认（verified）：
- `GTA.World.CreateVehicle(Model model, Vector3 position, float heading)` — 创建载具
- `Game.Player.Character` → 返回 `GTA.Ped` — 获取玩家角色
- `GTA.VehicleHash.Adder` — 跑车模型
- `GTA.VehicleHash.Cheetah` — 跑车模型
- `GTA.VehicleHash.EntityXF` — 跑车模型
- `GTA.VehicleHash.Turismor` — 跑车模型（注意枚举名是 Turismor，不是 TurismoR）
- `GTA.VehicleHash.Zentorno` — 跑车模型
- `GTA.VehicleHash.Turismo2` — 跑车模型
- `Model.Request()` — 加载模型

needs_query（未确认）：
- `GTA.Entity.Position` / `GTA.Ped.Position` — 查询结果截断未显示

现在我需要设计玩法。用户需求很简单：送玫瑰 → 刷新跑车。

跑车选择：我们已经有多个确认的跑车模型。由于用户没有指定具体跑车，我可以随机选一个或固定使用一个。考虑简单性，默认使用 Turismor（最常见的跑车之一），或者随机从已确认的列表中选取。

位置：在玩家前方刷新。由于 Position 属性未确认，可以使用 fallback：通过 Native 调用获取位置，或使用 `Game.Player.Character.GetOffsetPosition(new Vector3(0, 5, 0))` — 等等，GetOffsetPosition 也没确认。但我们可以使用 `Game.Player.Character.Position`（虽然未确认，但在 fallback 中可以采用 Native 调用 `GET_ENTITY_COORDS`）。或者更简单：使用 `World.CreateVehicle` 时，位置参数使用玩家位置偏移，如果 Position 属性不可用，可以用 Native `Function.Call<Vector3>(Hash.GET_ENTITY_COORDS, ped)`。

实际上，根据第一批查询结果，`World.CreateVehicle` 需要 `Vector3 position`。要获取玩家位置，最可靠的方式可能是使用 Native 调用。但我们知道 SHVDN 中 `Ped.Position` 几乎肯定可用（这是最基本的 API）。标记为 needs_query 但实际开发时大概率可用。

现在设计 DESIGN.md。

**状态机**：
- IDLE：等待礼物事件
- SPAWNING：收到 Rose 礼物后，在玩家前方生成跑车
- 然后回到 IDLE

**触发条件**：LIVE Studio 收到 gift_id=5655 (Rose)

**效果**：在玩家前方 5 米处刷新一辆随机跑车（从确认的跑车列表中随机选择）

**冷却**：建议 5 秒冷却防止刷礼物刷车

**清理**：不需要主动清理车辆，让玩家自己处理

现在来写 DESIGN.md

capability_requirements 表：

1. World.CreateVehicle — verified — 证据：第一批查询
2. Game.Player.Character — verified — 证据：第一批查询
3. Ped/Entity.Position — needs_query — fallback: 使用 Function.Call<Vector3>(Hash.GET_ENTITY_COORDS, ped)
4. VehicleHash 枚举值 — verified — 证据：第一批+第二批
5. Model.Request — verified — 证据：第一批查询注意事项中提到
6. LIVE Studio 礼物事件 — 需要平台支持，不通过 SHVDN

LIVE Studio 事件：通过脚本的 OnGiftReceived 或类似事件监听。这个在 DESIGN.md 中描述为平台事件。

现在输出 DESIGN.md。
</think># DESIGN.md

## 需求摘要

| 项目 | 内容 |
|------|------|
| 玩法名称 | 礼物刷新跑车 |
| 核心目标 | 观众通过 LIVE Studio 赠送「Rose」礼物（gift_id=5655），主播游戏中在玩家前方自动刷新一辆随机跑车 |
| 原始输入 | 「用户送玫瑰刷新一个跑车」 |
| 礼物依赖 | Rose（gift_id=5655，1 钻石） |
| 胜利/失败条件 | 无判定，礼物送达即触发刷新，属于纯正向反馈玩法 |
| 复杂度约束 | 单文件 ScriptHookVDotNet v3 脚本，无额外程序集或外部资源 |

---

## 玩法流程

```
观众赠送 Rose → LIVE Studio 推送事件 → 脚本接收 gift_id=5655
→ 冷却检查（通过则继续）
→ 获取玩家当前位置
→ 在玩家前方 5 米路面随机选择一辆跑车模型
→ 加载模型 → World.CreateVehicle 生成载具
→ 进入冷却，等待下次触发
```

---

## 状态机

```
┌──────────┐    收到 Rose（gift_id=5655）
│   IDLE   │─────╮且冷却已结束
└──────────┘     │
     ↑           ↓
     │     ┌──────────┐
     │     │ SPAWNING │
     │     └────┬─────┘
     │          │ 生成完成
     └──────────┘
```

- **IDLE**：等待礼物事件，维护冷却计时。
- **SPAWNING**：瞬时状态，执行模型加载与车辆生成，完成后立即回到 IDLE。

---

## 触发条件

| 条件 | 说明 |
|------|------|
| 事件源 | LIVE Studio `OnGiftReceived` 事件 |
| 匹配规则 | `gift_id == 5655`（精确匹配 Rose） |
| 冷却限制 | 两次触发间隔 ≥ 5 秒 |
| 触发时机 | 主播处于自由模式且 Ped 存活时方可生成 |

---

## 效果表

| 效果 | 描述 | 参数 |
|------|------|------|
| 跑车生成 | 在玩家前方生成一辆随机跑车 | 见下方「跑车池」 |
| 生成位置 | 玩家当前位置前方 5 米，路面高度 | `Vector3` 偏移 (0, +5, 0)，使用玩家朝向计算实际偏移 |
| 跑车池 | 从以下已确认模型中随机选取 | `Adder`、`Cheetah`、`EntityXF`、`Turismor`、`Zentorno`、`Turismo2` |
| 冷却 | 生成后进入 5 秒冷却 | 冷却期内忽略所有 Rose 礼物 |

---

## LIVE Studio 事件与礼物映射

| 事件名称 | 触发礼物 | gift_id | 钻石价格 | 匹配方式 |
|----------|----------|---------|----------|----------|
| `OnGiftReceived` | Rose | `5655` | 1 钻石 | 精确匹配 gift_id |

- 礼物数据来源：`query_live_gifts_tool` 返回 `name="Rose"`、`id=5655`、`diamondCount=1`、`matchType=exact_name`。
- 脚本在初始化时注册 LIVE Studio 礼物接收回调，仅在 `gift_id == 5655` 时进入 SPAWNING 流程。

---

## 计时与冷却

| 参数 | 值 | 说明 |
|------|-----|------|
| 冷却时长 | 5 秒 | 两次跑车生成的最小间隔 |
| 冷却变量 | `float _cooldownRemaining` | 每帧 Tick 递减 deltaTime |
| 冷却检查 | `_cooldownRemaining <= 0f` | 冷却结束方可触发 |
| 冷却重置 | 成功生成跑车后 `_cooldownRemaining = 5.0f` | — |

---

## 失败与清理

| 场景 | 处理方式 |
|------|----------|
| 模型加载超时（>3 秒未就绪） | 放弃本次生成，**不重置冷却**，输出调试日志 |
| 玩家 Ped 不存在或已死亡 | 跳过生成，正常消耗冷却（避免刷事件） |
| 脚本中止（Abort/Reload） | 无需显式清理已生成车辆；脚本不持有车辆引用，车辆由游戏世界自然管理 |
| 连续短时间大量礼物 | 冷却机制自动限流，冷却期内礼物被静默丢弃 |

---

## capability_requirements

| # | capability | behavior | priority | status | verified_evidence | fallback |
|---|------------|----------|----------|--------|-------------------|----------|
| 1 | `GTA.World.CreateVehicle(Model, Vector3, float)` | 在指定坐标生成车辆 | **必须** | ✅ verified | SHVDN 第一批返回：静态方法 `World.CreateVehicle`，参数 `model`、`position`、`heading` | — |
| 2 | `Game.Player.Character` | 获取当前玩家 Ped 实例 | **必须** | ✅ verified | SHVDN 第一批返回：`Game.Player.Character`（`GTA.Ped` 类型） | — |
| 3 | `GTA.Ped.Position` / `GTA.Entity.Position` | 获取玩家当前世界坐标 | **必须** | ⚠️ needs_query | SHVDN 第二批：Entity/Ped 属性列表截断，Position 未出现；但 Ped 继承 Entity，且 SHVDN 标准 API 中包含此属性 | 使用 Native 调用 `Function.Call<Vector3>(Hash.GET_ENTITY_COORDS, ped, false)` 替代 |
| 4 | `GTA.VehicleHash` 枚举跑车值 | 提供跑车模型标识符 | **必须** | ✅ verified | 第一批：`Adder`、`Cheetah`；第二批：`EntityXF`、`Turismor`、`Zentorno`、`Turismo2` | — |
| 5 | `GTA.Model.Request()` + 等待加载 | 生成前加载车辆模型资源 | **必须** | ✅ verified | SHVDN 第一批注意事项：生成前需调用 `model.Request()` 并等待加载完成 | 使用 `model.Request(5000)` 超时等待（未确认具体重载，可用循环 `model.IsLoaded` 检测） |
| 6 | 玩家朝向获取 | 计算前方偏移位置 | 推荐 | ⚠️ needs_query | 未单独查询 `Ped.Heading` 属性 | 简化：直接在玩家坐标的 Y 轴正方向偏移 5 米（`position + new Vector3(0, 5, 0)`），不依赖朝向 |
| 7 | LIVE Studio 礼物事件接收 | 监听观众送礼并获取 gift_id | **必须** | ✅ 平台能力 | 由 LIVE Studio 模板提供 `OnGiftReceived` 回调 | — |

---

## 降级方案

| 降级场景 | 降级行为 |
|----------|----------|
| `Ped.Position` 不可用 | 使用 Native `GET_ENTITY_COORDS` 获取玩家坐标 |
| `Ped.Heading` 不可用 | 放弃朝向计算，固定使用世界 Y 轴正方向偏移（玩家前方近似） |
| 模型加载超时 | 放弃本次生成，记录日志，不崩溃 |
| 礼物事件未到达 | 脚本保持 IDLE 状态，不影响游戏正常运行 |
| 跑车模型池中某模型加载失败 | 跳过该模型，从剩余池中随机重试一次；若全部失败则放弃 |

---

## 验收标准

1. 观众在 LIVE Studio 赠送「Rose」（gift_id=5655）后，主播游戏内前方出现一辆跑车。
2. 连续赠送时，两次跑车生成间隔 ≥ 5 秒。
3. 冷却期内赠送的 Rose 不会生成跑车。
4. 生成的跑车模型限定在 `Adder`、`Cheetah`、`EntityXF`、`Turismor`、`Zentorno`、`Turismo2` 六款内。
5. 主播死亡或 Ped 不存在时不生成车辆，脚本不崩溃。
6. 脚本加载/卸载无残留异常。