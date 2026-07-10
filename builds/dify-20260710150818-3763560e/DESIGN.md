<think>
用户需求很清楚。我需要设计一个GTA5的LIVE Studio玩法。让我先分析需要的核心GTA5能力：

1. 车辆操作：获取车辆、检查车辆生命值、修复车辆、检查轮胎状态、设置轮胎防爆、引擎控制、加速设置
2. NPC/追兵：生成追击车辆、清除追兵、生成护航NPC
3. 特效：附近爆炸（但保留最低血量）、降低抓地力
4. 检查点系统：强制绕路检查点
5. 计时器：10分钟倒计时
6. 主播换车检测
7. 爆炸检测（车辆爆炸）
8. 胜利/失败条件判断

我需要查询SHVDN API能力。先汇总核心能力：

第一批核心能力（决定玩法可行性）：
- 获取玩家当前车辆
- 车辆生命值（健康值）获取与修复
- 车辆轮胎状态（是否爆胎）及防爆轮胎设置
- 车辆引擎状态控制（熄火）
- 生成追击车辆NPC
- 清除附近敌对NPC
- 生成护航NPC
- 车辆附近爆炸（但要保留最低血量）
- 车辆抓地力修改
- 检查点/导航点设置
- 计时器
- 玩家换车检测
- 车辆爆炸检测

这些太多了。我需要精简到256字符以内。分成两批：

第一批：核心能力，决定玩法可行性：
- 玩家车辆获取、车辆生命值修复、轮胎防爆、引擎控制、车辆爆炸检测、换车检测、计时器

第二批：其他能力：
- 生成追击NPC、清除追兵、护航NPC、附近爆炸但不致死、抓地力修改、检查点设置

先查询第一批。

capability_request: "获取玩家当前车辆、获取车辆健康值并修复、设置轮胎防爆、控制引擎熄火、检测车辆爆炸、检测玩家换车、计时器实现"

这应该不超过256字符。
</think>好的，我先分析核心能力需求，分两批查询 SHVDN API。
<think>
第一批查询结果：
- 获取玩家当前车辆：✅ Ped.CurrentVehicle
- 修复车辆：✅ Vehicle.BodyHealth, Vehicle.EngineHealth
- 轮胎防爆：✅ Vehicle.CanTiresBurst
- 引擎熄火：❌ 未确认
- 检测车辆爆炸：❌ 未确认
- 检测换车：✅ Tick + 缓存比较
- 计时器：✅ Game.GameTime

现在第二批：生成追击NPC、清除追兵、护航NPC、附近爆炸但不致死、抓地力修改、检查点设置、短暂加速

capability_request: "生成敌对NPC车辆追击玩家、清除附近敌对NPC、生成友方NPC车辆护航、在指定位置附近生成爆炸但不致死、修改车辆抓地力、设置导航检查点、给予车辆短暂加速效果"
</think>第一批部分能力未确认（引擎熄火、车辆爆炸检测），现在查询第二批独立能力。
<think>
现在我已经有两批查询结果。让我总结所有能力的状态：

第一批（核心能力）：
- 获取玩家当前车辆：✅ verified - Ped.CurrentVehicle, Game.Player, Player.Character
- 车辆健康值修复：✅ verified - Vehicle.BodyHealth, Vehicle.EngineHealth
- 轮胎防爆：✅ verified - Vehicle.CanTiresBurst
- 引擎熄火：❌ needs_query - 未确认
- 检测车辆爆炸：❌ needs_query - 未确认
- 检测玩家换车：✅ verified - Tick + 缓存比较
- 计时器：✅ verified - Game.GameTime

第二批（扩展能力）：
- 生成敌对NPC车辆追击：⚠️ partial - 可生成车辆和驾驶员，设置DrivingStyle/DrivingSpeed，但缺少TaskVehicleChase
- 清除附近敌对NPC：❌ unsupported - 缺少获取附近NPC的API
- 生成友方NPC车辆护航：⚠️ partial - 同上，可生成但缺护航任务API
- 附近爆炸不致死：⚠️ partial - World.AddExplosion可用，但无法精确控制不致死
- 修改车辆抓地力：❌ unsupported - 无直接API
- 设置导航检查点：✅ verified - World.CreateCheckpoint
- 短暂加速：✅ verified - Vehicle.EnginePowerMultiplier

现在需要设计 fallback：

引擎熄火 fallback：使用 EngineHealth 设为极低值（如0），强制引擎损坏导致熄火。或者设置 EnginePowerMultiplier 为0来模拟熄火效果。

车辆爆炸检测 fallback：在Tick中检查任务车辆的 IsAlive 属性或 Health < 某个阈值。但 Vehicle.IsAlive 或 Vehicle.Health 是否在查询结果中？第一批结果中没有列出这些。第一批只显示了Vehicle的BodyHealth和EngineHealth。但Vehicle可能有IsDead或IsAlive？没有确认。我们可以用BodyHealth <= 0 来判断，或者用Vehicle.IsDead（但未确认）。

等等，Vehicle.IsDead 没有被确认。可以用 BodyHealth 和 EngineHealth 都接近0来判断。或者检查 Vehicle 对象是否为 null（被销毁后CurrentVehicle可能返回null或不同的引用）。

更好的fallback：在 Tick 中检查 CurrentVehicle，如果任务车辆引用突然变成null或者CurrentVehicle不等同于任务车辆（因为被销毁），则判定爆炸/摧毁。

对于爆炸检测，还有一个办法：检查任务车辆的 BodyHealth 是否突然大幅下降。但这不是精确的。

我需要用已确认的API来设计fallback。

清除附近追兵：虽然无法获取附近NPC，但可以在生成时记录追兵Ped引用，然后对记录的引用调用Kill()或Delete()。这是一个可行的方案。

抓地力降低：无法通过API直接实现，但可以用其他方式模拟"降低操控性"：降低 BrakePower 或 EnginePowerMultiplier，或者让车辆短暂失去动力。但这不是抓地力。在没有直接API的情况下，fallback可以是用 EnginePowerMultiplier 设为低于1的值来模拟"车辆变难开"。

附近爆炸但不致死：使用 World.AddExplosion，设置较小的radius和合适的ExplosionType，并在爆炸后检查车辆血量，如果过低则修复到安全值。

现在用户没有指定礼物，所以不需要调用 query_live_gifts_tool。

现在汇总所有信息，输出 DESIGN.md。

关键设计：
- 脚本入口：Script.Tick
- 状态机：Idle -> Active -> Victory/Failure
- 任务车辆：主播当前驾驶的车辆
- 触发方式：观众通过礼物或指令触发（具体实现由LIVE Studio事件映射）
- 礼物未指定，无需具体映射

能力需求表需要详细列出。

关于"强制绕路检查点"：使用 World.CreateCheckpoint 创建额外检查点，主播必须经过该检查点才能继续。

关于"爆胎"：可以使用 Vehicle.CanTiresBurst = true 然后通过某种方式让轮胎爆裂。但直接"使一个轮胎爆炸"可能需要原生函数。这里我们fallback：如果CanTiresBurst为true，可以通过降低轮胎健康度来实现。但如果没有直接API，可以用 World.AddExplosion 小爆炸在轮胎附近来模拟爆胎？不太精确。

实际上，对于"爆胎"这个负面互动，更直接的做法是：设置 CanTiresBurst = true（如果之前被防爆保护设为false），然后通过某种伤害让轮胎爆裂。如果没有直接API，可以用 Vehicle.Wheels 相关属性？查询结果没有Wheels。

等等，Vehicle 类在查询中被截断了，只显示了前30个属性。Wheels 可能在后面。但我们不能假设。

对于爆胎，fallback：如果之前有防爆轮胎buff（CanTiresBurst=false），则将其恢复为true；如果本来就是true，则可以用一个小爆炸或直接降低轮胎健康...但无API。保守做法：爆胎效果可以用"瞬间降低 BodyHealth 一定量"来模拟"车辆受损"，虽然不是真正的爆胎。

类似地，引擎熄火用 EngineHealth 设为0来模拟。

让我们确定最终的fallback策略。

现在构建 DESIGN.md。
</think>两批查询完成。能力汇总完毕，开始输出 DESIGN.md。

# DESIGN.md

## 一、需求摘要

- **玩法名称**：易碎货物送达
- **核心目标**：主播驾驶指定任务车辆（载有易碎货物），在观众互动（帮助/干扰）下，于 10 分钟内抵达终点检查点。
- **胜利条件**：在限时内驾驶任务车辆到达终点，且车辆未爆炸、主播未主动换车。
- **失败条件**：
  1. 任务车辆爆炸/被摧毁
  2. 主播主动离开任务车辆并换乘其他车辆
  3. 超过 10 分钟未到达终点
- **时长/节奏**：单局上限 10 分钟；提前到达终点立即判定胜利。
- **观众互动触发**：观众通过 LIVE Studio 礼物或评论指令触发正/负面效果。
- **礼物要求**：未指定（不绑定具体 gift_id；由 LIVE Studio 事件层统一路由，本玩法接收已解析的效果指令）。

---

## 二、玩法流程

1. **准备阶段**：脚本加载后进入 `Idle` 状态，等待开始指令。
2. **开始指令**：主播/系统触发开始事件 → 记录当前车辆为任务车辆、记录起点、在终点创建检查点、启动 10 分钟倒计时 → 进入 `Active` 状态。
3. **进行阶段（Active）**：
   - 每 Tick 检测：任务车辆是否存在/存活、主播是否换车、是否到达终点、是否超时。
   - 接收 LIVE Studio 传入的正/负效果指令 → 执行对应效果（含冷却与防刷）。
4. **结束阶段**：
   - 胜利：主播驾驶任务车辆触碰终点检查点 → 提示胜利 → 进入 `Cleanup`。
   - 失败：车辆爆炸/被摧毁、主播换车、超时 → 提示失败原因 → 进入 `Cleanup`。
5. **清理阶段（Cleanup）**：移除所有检查点、清除脚本生成的 NPC 与车辆、重置车辆状态修改、释放引用 → 回到 `Idle`。

---

## 三、状态机

| 状态 | 说明 | 可转移至 |
|------|------|----------|
| `Idle` | 等待开始，无任务进行 | `Active` |
| `Active` | 任务进行中，持续检测 | `Victory`, `Failure`, `Cleanup` |
| `Victory` | 主播到达终点 | `Cleanup` |
| `Failure` | 车辆爆炸/换车/超时 | `Cleanup` |
| `Cleanup` | 清理资源和状态 | `Idle` |

---

## 四、触发条件与效果表

### 4.1 正面效果

| 效果名称 | 触发方式 | 行为描述 | 冷却 | 备注 |
|----------|----------|----------|------|------|
| 修复车辆 25% | LIVE Studio 指令 | 将任务车辆 `BodyHealth` 和 `EngineHealth` 各提升当前缺失值的 25%（即 `current + (1000 - current) * 0.25`） | 15 秒 | 不超出上限 1000.0 |
| 防爆轮胎 30 秒 | LIVE Studio 指令 | 设置任务车辆 `CanTiresBurst = false`，30 秒后恢复为 `true` | 45 秒 | 可刷新持续时间 |
| 清空追兵 | LIVE Studio 指令 | 遍历脚本记录的追兵 Ped 列表，对每个调用 `Kill()` 并移除其车辆 | 30 秒 | 只清理脚本生成的追兵 |
| 短暂加速 5 秒 | LIVE Studio 指令 | 设置任务车辆 `EnginePowerMultiplier *= 2.0`，5 秒后恢复原值 | 20 秒 | 可刷新持续时间 |
| 生成护航 NPC | LIVE Studio 指令 | 在任务车辆后方 10m 生成一辆友方车辆（驾驶员 `DrivingStyle = Normal`, `DrivingSpeed` 匹配当前车速），记录引用 | 60 秒（同时最多 2 辆） | 护航 NPC 随任务车辆移动跟踪 |

### 4.2 负面效果

| 效果名称 | 触发方式 | 行为描述 | 冷却 | 备注 |
|----------|----------|----------|------|------|
| 爆胎 | LIVE Studio 指令 | 若 `CanTiresBurst == false`（防爆中），则提前恢复为 `true`；若已为 `true`，则降低 `BodyHealth` 50 点模拟轮胎损伤 | 15 秒 | 无直接爆胎 API 的 fallback |
| 引擎熄火 | LIVE Studio 指令 | 将任务车辆 `EngineHealth` 设为 0（立即熄火停转），3 秒后恢复至熄火前值的 50% | 20 秒 | 通过 EngineHealth=0 实现熄火效果 |
| 生成追击车辆 | LIVE Studio 指令 | 在主播后方 30~50m 生成 1 辆敌对车辆（`DrivingStyle = Rushed`, `DrivingSpeed = 35+`），记录到追兵列表 | 25 秒（同时最多 3 辆） | 使用 `World.CreateVehicle` + `CreatePedOnSeat` |
| 降低抓地力 | LIVE Studio 指令 | 设置任务车辆 `EnginePowerMultiplier *= 0.5` 持续 6 秒（模拟操控变迟钝） | 18 秒 | 无直接抓地力 API 的 fallback |
| 附近爆炸 | LIVE Studio 指令 | 在任务车辆前方 5~8m 处调用 `World.AddExplosion`（`ExplosionType = Flame`, `radius = 2.0`），爆炸后检测 `BodyHealth`，若低于 150 则修复至 200 | 25 秒 | 保证不致死的最低血量安全网 |
| 强制绕路检查点 | LIVE Studio 指令 | 在任务车辆当前位置与终点之间的偏移位置创建额外检查点，主播触碰后移除；同一时间最多 1 个绕路点 | 40 秒 | 使用 `World.CreateCheckpoint` |

---

## 五、检测与判定逻辑

### 5.1 车辆爆炸/被摧毁检测
- **主策略（fallback）**：每 Tick 检查 `Ped.CurrentVehicle`，若此前缓存的任务车辆引用变为 `null` 或 `CurrentVehicle` 不等于任务车辆引用 → 判定车辆丢失/被摧毁。
- **辅助策略**：若任务车辆引用仍有效，检测 `BodyHealth <= 0` → 判定被摧毁。

### 5.2 主播换车检测
- 每 Tick 比较 `Ped.CurrentVehicle` 与缓存的任务车辆引用。若不同（包括变为 `null` 后重新进入另一车辆），判定为换车 → 失败。

### 5.3 到达终点检测
- 每 Tick 计算主播位置与终点检查点位置的距离，若 ≤ 检查点半径（默认 3.0m），判定到达 → 胜利。

### 5.4 超时检测
- 记录开始时间戳（`Game.GameTime`），每 Tick 计算差值 ≥ 600,000ms（10分钟）→ 超时失败。

---

## 六、capability_requirements

| capability | behavior | priority | status | verified_evidence | fallback |
|------------|----------|----------|--------|-------------------|----------|
| 获取玩家当前车辆 | 通过 `Game.Player.Character.CurrentVehicle` 获取 | 必须 | verified | `Ped.CurrentVehicle` 返回 `GTA.Vehicle` | — |
| 读取/修复车辆健康值 | `Vehicle.BodyHealth` / `EngineHealth` 读写 | 必须 | verified | 两个 float 属性，范围 0~1000 | — |
| 轮胎防爆开关 | `Vehicle.CanTiresBurst` 设为 false 防爆 | 正面 | verified | bool 属性可读写 | — |
| 计时器 | `Game.GameTime` 毫秒计时 | 必须 | verified | 静态 int，返回游戏运行毫秒 | — |
| 检测玩家换车 | Tick 中比较 `CurrentVehicle` 前后变化 | 必须 | verified | 无专用事件，通过缓存比较实现 | — |
| 短暂加速效果 | `Vehicle.EnginePowerMultiplier` 临时提升 | 正面 | verified | float 属性可读写，默认 1.0 | — |
| 导航检查点 | `World.CreateCheckpoint` 创建检查点 | 必须 | verified | 静态方法，接受位置/半径/颜色等参数 | — |
| 生成 NPC 车辆 | `World.CreateVehicle` + `Vehicle.CreatePedOnSeat` | 正/负面 | verified | 可指定 VehicleHash、位置、朝向和 PedHash | — |
| NPC 驾驶行为 | `Ped.DrivingStyle` 和 `Ped.DrivingSpeed` | 正/负面 | verified | 枚举 + float，可设置跟车行为 | — |
| 附近生成爆炸 | `World.AddExplosion` | 负面 | verified | 静态方法，支持位置/类型/半径/拥有者 | — |
| 引擎熄火控制 | 直接开关引擎 | 负面 | needs_query | 未在查询结果中找到 `IsEngineRunning` 或等效方法 | 将 `EngineHealth` 设为 0 强制熄火，3 秒后恢复 |
| 车辆爆炸检测 | 检测任务车辆是否被摧毁 | 必须 | needs_query | 无 `OnExplosion` 事件或 `IsDead` 属性 | 检测 CurrentVehicle 引用丢失或 BodyHealth ≤ 0 |
| 精确追击任务 | `TaskVehicleChase` 等任务 API | 负面 | needs_query | 仅有 `DrivingStyle`/`DrivingSpeed`，无 Task 系 API | 通过 DrivingStyle=Rushed + DrivingSpeed 模拟追击 |
| 清除附近 NPC | 遍历并删除附近敌对 NPC | 正面 | needs_query | 无 `World.GetNearbyPeds` 或遍历 API | 仅清除脚本记录的追兵列表中的 Ped 引用 |
| 修改车辆抓地力 | `Traction` / `Grip` 属性 | 负面 | needs_query | Vehicle 属性列表中无抓地力相关成员 | 降低 `EnginePowerMultiplier` 模拟操控迟缓 |
| 精确爆胎 | 使单个轮胎爆裂 | 负面 | needs_query | 无 `Wheels` 或 `BurstTire` 相关 API | 若防爆中则取消防爆；否则降低 BodyHealth 模拟 |

---

## 七、LIVE Studio 事件与礼物映射

- **礼物要求**：用户未指定具体礼物名称、gift_id 或钻石价格。本设计不绑定任何具体礼物。
- **事件接口**：LIVE Studio 将观众互动（礼物/评论指令）解析为效果标识符（如 `repair_25`, `anti_pop_30s`, `clear_pursuers`, `boost_5s`, `spawn_escort`, `pop_tire`, `stall_engine`, `spawn_pursuer`, `low_grip`, `near_explosion`, `detour_checkpoint`），脚本接收后路由至对应效果处理器。
- **扩展性**：后续可通过 `query_live_gifts_tool` 绑定具体 gift_id → 效果映射，无需修改玩法核心逻辑。

---

## 八、计时与冷却

| 项目 | 值 |
|------|-----|
| 单局总时长上限 | 10 分钟（600,000ms，基于 `Game.GameTime`） |
| 正面效果冷却（全局） | 各效果独立冷却，见 §4.1 |
| 负面效果冷却（全局） | 各效果独立冷却，见 §4.2 |
| 防爆轮胎持续时间 | 30 秒（可刷新） |
| 短暂加速持续时间 | 5 秒（可刷新） |
| 引擎熄火持续时间 | 3 秒 |
| 降低抓地力持续时间 | 6 秒 |

---

## 九、失败与清理

### 9.1 失败触发
1. **车辆爆炸/被摧毁**：检测到任务车辆引用丢失或 `BodyHealth ≤ 0`。
2. **主播换车**：`CurrentVehicle` 与任务车辆引用不一致。
3. **超时**：`Game.GameTime` 差值 ≥ 600,000ms。

### 9.2 清理操作（Cleanup 状态）
- 删除所有脚本创建的 `Checkpoint` 对象（终点 + 绕路点）。
- 遍历追兵列表：Kill 所有 Ped → Delete 所有 Vehicle。
- 遍历护航列表：Delete 所有 Ped → Delete 所有 Vehicle。
- 重置任务车辆的 `EnginePowerMultiplier` 为 1.0。
- 重置任务车辆的 `CanTiresBurst` 为 `true`。
- 清空所有内部列表和缓存引用。
- 状态置为 `Idle`。

### 9.3 脚本中止（Abort）
- 监听 `Script.Aborted` 事件（若可用）或在 `Tick` 中检测脚本终止 → 执行与 Cleanup 相同的清理逻辑。

---

## 十、降级方案

当能力标记为 `needs_query` 且 fallback 仍不足以支撑核心玩法时：

1. **引擎熄火**：若 `EngineHealth = 0` 无法可靠触发熄火效果，降级为"瞬间将 `EnginePowerMultiplier` 设为 0.01 持续 3 秒"。
2. **车辆爆炸检测**：若引用丢失判断不准确，额外降级为"检测 `BodyHealth` 单帧下降超过 300 点"作为爆炸信号。
3. **追击车辆行为**：若 `DrivingStyle` 无法驱动 NPC 主动追击，降级为"在主播前方生成静止路障车辆"（阻塞效果替代追击）。
4. **清空追兵**：若追兵列表管理失效，降级为"不对追兵做任何清理，仅依赖自然消失"。
5. **降低抓地力**：若 `EnginePowerMultiplier` 降低效果不明显，降级为"频繁脉冲式点刹"（短间隔降低 `EnginePowerMultiplier` 到 0.3 再恢复）。
6. **附近爆炸不致死**：若安全网仍导致偶尔死亡，降级为"爆炸后无条件将 `BodyHealth` 设为 max(当前值, 300)"。

---

## 十一、验收标准

| 编号 | 验收条件 | 验证方式 |
|------|----------|----------|
| AC-1 | 脚本加载后处于 Idle 状态，不产生任何游戏内效果 | 进入游戏，无检查点、无NPC生成 |
| AC-2 | 收到开始指令后，正确记录当前车辆并在终点生成检查点 | 开始后地图可见检查点标记 |
| AC-3 | 10 分钟内到达终点检查点触发胜利 | 驶入检查点区域，UI提示胜利，进入清理 |
| AC-4 | 任务车辆爆炸后触发失败 | 引爆车辆，UI提示失败 |
| AC-5 | 主播换车后触发失败 | 下车并进入另一车辆，UI提示失败 |
| AC-6 | 超时触发失败 | 等待 10 分钟未到终点，UI提示超时失败 |
| AC-7 | 修复 25% 正确恢复健康值 | 受损车辆触发修复，BodyHealth 和 EngineHealth 均提升 |
| AC-8 | 防爆轮胎 30 秒内轮胎不爆 | 期间射击轮胎，无爆胎；30秒后可爆 |
| AC-9 | 清空追兵删除脚本生成的追击NPC | 触发后追兵车辆和Ped消失 |
| AC-10 | 短暂加速 5 秒后恢复 | 触发后车辆明显提速，5秒后恢复 |
| AC-11 | 生成护航NPC跟随 | 触发后后方出现友方车辆跟随 |
| AC-12 | 爆胎效果生效 | 触发后防爆状态取消或车辆受损 |
| AC-13 | 引擎熄火 3 秒后恢复 | 触发后车辆停转，3秒后恢复 |
| AC-14 | 生成追击车辆 | 触发后出现敌对车辆追赶 |
| AC-15 | 降低抓地力 6 秒 | 触发后加速能力明显减弱，6秒后恢复 |
| AC-16 | 附近爆炸不致死 | 触发后爆炸发生但车辆血量不低于安全值 |
| AC-17 | 强制绕路检查点出现并被要求经过 | 触发后地图出现额外检查点 |
| AC-18 | 清理后无残留实体和效果 | 胜利/失败后检查点消失、NPC消失、车辆状态恢复 |
| AC-19 | 各效果冷却独立且正确 | 短时间内重复触发同一效果被拒绝 |
| AC-20 | 脚本中止后资源释放 | 卸载脚本后无残留NPC/检查点/状态修改 |