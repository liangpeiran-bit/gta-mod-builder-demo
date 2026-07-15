<think>
用户需求非常明确：观众赠送"玫瑰"礼物时，在游戏内刷新一辆跑车。这是一个简单的触发机制。

我需要：
1. 调用 query_live_gifts 查询"玫瑰"礼物
2. 调用 query_shvdn_api 查询 GTA5 中刷新车辆的能力

这两个调用可以并行进行。

先检查礼物工具参数：
- query: "玫瑰" - 用户原始称呼，简洁，不超过128字符
- keywords: "rose" - 英文关键词辅助模糊检索

SHVDN 能力查询：我需要查询：
- 创建车辆/刷新车辆的能力（Vehicle creation/spawning）
- 获取玩家位置/坐标的能力
- Ped 相关能力

第一批先查核心能力：车辆创建、玩家位置获取。

capability_request 必须简洁，不超过256字符。
</think>
<think>
礼物查询结果：status=matched，精确命中 "Rose"（gift_id=5655，diamondCount=1）。

SHVDN查询结果：partial。确认了 World.CreateVehicle、VehicleHash.Infernus、Game.Player。Player.Character 和 Ped.Position 未直接确认但在常规API中可用。Turismo 未找到。

现在我需要继续查询第二批能力：需要确认 Player.Character 和 Ped.Position 以及更多跑车模型。另外还需要 LIVE Studio 的礼物事件接收能力。

第二批查询：
- 确认 Player 类的 Character 属性和 Entity 的 Position 属性
- 查询更多跑车模型：TurismoR、Cheetah、EntityXF 等
- LIVE Studio 事件订阅机制

但是 SHVDN 工具最多调用2次，我已经用了1次，还有1次。需要合并能力查询。

还需要查 LIVE Studio 相关：如何订阅礼物事件？这可能需要查 SHVDN 中 LIVE Studio 相关的 API。

让我合并第二批：
- Player.Character、Entity.Position、Ped 继承关系
- 跑车模型：TurismoR、Cheetah、EntityXF、Adder 等
- LIVE Studio 事件：礼物接收事件、OnGiftReceived 等

capability_request 必须不超过256字符。

让我精简：

"Player类的Character属性返回Ped类型；Entity类的Position属性；VehicleHash枚举中的跑车模型：TurismoR、EntityXF、Cheetah、Adder、Zentorno；LIVE Studio SDK中礼物接收事件OnGiftReceived的订阅方式与Gift类结构"

但 LIVE Studio 可能不在 SHVDN 能力库中。我需要尝试查询。如果查不到就做 fallback。

实际上 256 字符比较紧张，我只查最关键的：
- Player.Character、Entity.Position
- TurismoR、EntityXF 等跑车 VehicleHash
- LIVE Studio 礼物事件订阅
</think>
<think>
工具调用失败了，返回了错误。根据规则：SHVDN 返回 partial、unsupported、截断或错误时记录 needs_query 或 fallback。但我已经用完了2次 SHVDN 调用（第二次失败了）。规则说最多调用2次，不得重试。

现在我需要基于已有的信息输出 DESIGN.md。

已有信息：
- 礼物：Rose，gift_id=5655，diamondCount=1，status=matched
- SHVDN 第一批：World.CreateVehicle 确认、VehicleHash.Infernus 确认、Game.Player 确认、Player.Character 和 Ped.Position 部分确认（推测可用）、Turismo 未找到
- 第二批失败

现在输出 DESIGN.md。

需求摘要：
- 观众发送"玫瑰"礼物（gift_id=5655），触发在玩家位置刷新一辆跑车

流程：
1. 脚本启动，订阅 LIVE Studio 礼物事件
2. 收到礼物事件，检查 gift_id == 5655（Rose）
3. 获取玩家当前位置
4. 在玩家附近生成一辆跑车（默认使用 Infernus，因为已确认）
5. 完成

对于 LIVE Studio SDK 事件订阅方式，第二批查询失败，标记为 needs_query。但我们可以用 fallback 方式描述：假设使用 LIVE Studio 的标准事件机制。根据模板，通常是通过 LiveStudio 类的某个事件订阅。

关于跑车模型：用户说"跑车"，Infernus 已确认可用，作为默认。Turismo 等未确认。

Player.Character 和 Ped.Position 标记为 needs_query（第二批失败），但实际在 SHVDN 中这些是标准 API，第一批已说明"基于通用 API 可用"，标记为 verified_with_caveat。

现在构建 DESIGN.md。

capability_requirements 表：
- World.CreateVehicle：verified，证据来自第一批
- Game.Player：verified
- Player.Character：needs_query（第二批失败，第一批推测可用）
- Ped.Position（Entity.Position）：needs_query
- VehicleHash.Infernus：verified
- VehicleHash 其他跑车：needs_query
- LIVE Studio 礼物事件订阅：needs_query（第二批失败）

gift_requirements：
- status: matched
- confirmed_name: Rose
- gift_id: 5655
- diamondCount: 1

现在输出。
</think># DESIGN.md

## 1. 需求摘要

| 项目 | 内容 |
|---|---|
| 玩法名称 | 礼物触发刷车 (Gift-Triggered Vehicle Spawn) |
| 核心目标 | 观众在 LIVE Studio 中赠送「玫瑰」礼物后，游戏内立即在玩家角色身旁刷新一辆跑车 |
| 胜利条件 | 不适用（无对抗/竞速目标） |
| 失败条件 | 不适用 |
| 时长/节奏 | 瞬时触发，无持续计时 |
| 正向互动 | 观众赠送「Rose」礼物（gift_id=5655） |
| 负向互动 | 无 |
| 触发方式 | LIVE Studio 礼物事件驱动 |
| 复杂度约束 | 单次触发、单车辆、无冷却、无队列 |

---

## 2. 玩法流程

```
[脚本启动] → [订阅 LIVE Studio 礼物事件]
     ↓
[观众赠送礼物] → [LIVE Studio 推送事件]
     ↓
[脚本检查 gift_id == 5655 (Rose)]
     ↓ (匹配)
[获取玩家角色当前位置 + 朝向]
     ↓
[在玩家前方偏移位置调用 World.CreateVehicle(跑车模型, 位置, 朝向)]
     ↓
[车辆刷新完成] → [等待下一次礼物事件]
```

---

## 3. 状态机

```
┌──────────┐
│   IDLE   │ ← 初始状态 / 每次刷车后回到此状态
└────┬─────┘
     │ 收到 LIVE Studio 礼物事件
     ▼
┌──────────────┐
│ CHECK_GIFT   │
└────┬─────────┘
     │ gift_id == 5655 (Rose)
     ▼
┌──────────────┐
│ SPAWN_CAR    │ → 获取位置 → CreateVehicle → 回到 IDLE
└──────────────┘
     │ gift_id != 5655
     ▼
   (忽略) → 回到 IDLE
```

---

## 4. 触发条件

| 条件 | 说明 |
|---|---|
| 事件源 | LIVE Studio `OnGiftReceived` 事件 |
| 匹配字段 | `Gift.Id == 5655`（精确匹配） |
| 触发动作 | 刷新一辆跑车 |
| 未匹配行为 | 静默忽略，不产生任何游戏内效果 |

---

## 5. 效果表

| 效果 | 描述 | 参数 |
|---|---|---|
| 车辆生成 | 在玩家角色附近刷新一辆跑车 | 模型: Infernus（默认），位置: 玩家前方约3米，朝向: 与玩家一致 |

**刷车位置计算**：
- 基准坐标 = `Game.Player.Character.Position`
- 偏移 = 玩家前方向量 × 3.0 米（使用 `Game.Player.Character.ForwardVector`）
- 朝向 = `Game.Player.Character.Heading`

---

## 6. capability_requirements

| capability | behavior | priority | status | verified_evidence | fallback |
|---|---|---|---|---|---|
| `World.CreateVehicle(Model, Vector3, float)` | 在指定坐标生成车辆 | critical | verified | SHVDN API 确认：`static Vehicle CreateVehicle(Model model, Vector3 position, float heading = 0f)` | 无 |
| `Game.Player` | 获取本地玩家 Player 对象 | critical | verified | SHVDN API 确认：`Game.Player` 静态属性返回 `GTA.Player` | 无 |
| `Player.Character` | 从 Player 获取玩家 Ped | critical | needs_query | 第一批推测可用（SHVDN 常规链式调用）；第二批查询失败 | 使用 `Game.Player.Character`，若不可用则降级为 `World.GetNearbyPeds` 筛选玩家 |
| `Entity.Position` (Ped.Position) | 获取实体三维坐标 | critical | needs_query | 第一批推测继承自 Entity 可用；第二批查询失败 | 使用 `Ped.Position`，若不可用则降级为 `Function.Call<Vector3>(Hash.GET_ENTITY_COORDS, ped)` |
| `VehicleHash.Infernus` | 跑车模型枚举值 | high | verified | SHVDN 枚举确认 `GTA.VehicleHash.Infernus` | 无 |
| `VehicleHash` 其他跑车（TurismoR、EntityXF、Cheetah 等） | 更多跑车模型选项 | low | needs_query | 第一批 Turismo 未命中；第二批查询失败 | 仅使用已确认的 `Infernus`；或通过 `Game.GenerateHash("turismor")` 动态生成模型 Hash |
| LIVE Studio `OnGiftReceived` 事件 | 订阅观众礼物推送 | critical | needs_query | 第二批查询失败；LIVE Studio SDK 标准事件 | 假设存在 `LIVEStudio.OnGiftReceived += handler`，签名待代码生成节点确认 |
| `Gift` 类型结构（Id 字段） | 读取礼物 ID | critical | needs_query | 第二批查询失败 | 假设 `Gift.Id` 为 int 类型字段 |

---

## 7. LIVE Studio 事件与礼物映射

### 7.1 gift_requirements

| 字段 | 值 |
|---|---|
| original_query | 玫瑰 |
| keywords | rose, flower |
| status | **matched** |
| confirmed_name | **Rose** |
| gift_id | **5655** |
| diamondCount | **1** |
| candidates | [{"name":"Rose","id":5655,"diamondCount":1,"matchType":"exact_name","score":100}] |
| fallback | 无（精确命中） |

### 7.2 事件映射

| LIVE Studio 事件 | 触发条件 | 脚本动作 |
|---|---|---|
| `OnGiftReceived` | `gift.Id == 5655` | 执行刷车流程 |
| `OnGiftReceived` | `gift.Id != 5655` | 忽略 |

---

## 8. 计时与冷却

| 项目 | 策略 |
|---|---|
| 冷却时间 | 无冷却（每次 Rose 礼物均触发） |
| 车辆清理 | 不自动清理；由 GTA5 原生 despawning 机制或玩家自行处理 |
| 并发保护 | 单线程事件处理，无需额外锁；事件回调中同步完成刷车 |

---

## 9. 失败与清理

| 场景 | 处理 |
|---|---|
| 玩家不在游戏内 / 角色为 null | 检查 `Game.Player.Character` 非 null，否则跳过并记录日志 |
| 模型加载失败 | 调用 `Model.Request(500)` 等待最多 500ms，超时则跳过本次触发 |
| 脚本中止 (Abort/Unload) | `OnDispose` 或 `Script.Aborted` 中取消 LIVE Studio 事件订阅 |
| 位置无效（室内/水中） | 不做额外校验，直接在地面高度生成（依赖 GTA5 物理将车辆放置到最近地面） |

---

## 10. 降级方案

| 场景 | 降级措施 |
|---|---|
| LIVE Studio SDK 不可用 | 脚本加载失败，记录错误日志，不执行任何逻辑 |
| `Infernus` 模型不可用 | 回退到 `VehicleHash.Adder`（若可用）或最基础的 `VehicleHash.Blista` |
| `Player.Character` 为 null | 静默跳过，不刷车 |
| 所有跑车模型均不可用 | 使用 `VehicleHash.Blista` 作为最终兜底 |

---

## 11. 验收标准

1. **触发验证**：观众在 LIVE Studio 中赠送「Rose」礼物（gift_id=5655），游戏内玩家身旁刷新一辆 Infernus 跑车
2. **非触发验证**：赠送非 Rose 礼物（任意其他礼物），游戏内无任何车辆刷新
3. **位置验证**：刷新的车辆出现在玩家角色前方约 3 米处，朝向与玩家一致
4. **重复触发**：连续赠送多个 Rose 礼物，每次均刷新一辆新车
5. **脚本启停**：脚本卸载后不再响应礼物事件；重新加载后恢复正常
6. **异常容错**：玩家角色不存在时脚本不崩溃，仅静默跳过