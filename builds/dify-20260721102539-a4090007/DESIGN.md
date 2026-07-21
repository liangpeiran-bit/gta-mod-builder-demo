<think>
用户想要设计一个GTA5 LIVE Studio生存玩法：

1. 收到礼物"玫瑰"触发
2. 通缉等级改为5星
3. 所有玩家（NPC? 还是Ped?）攻击主角
4. 给主角一把冲锋枪和1000子弹

我需要：
- 查询SHVDN API：需要了解通缉等级设置、给武器/子弹、让Ped攻击主角、以及可能的车辆/Ped相关API
- 查询礼物"玫瑰"：query_live_gifts

先并行调用这两个工具。

SHVDN查询：我需要查询：
- WantedLevel / Player.WantedLevel 设置通缉等级
- GiveWeapon / WeaponHash 给武器
- Ped攻击玩家：可能需要Ped.Task攻击玩家，或者World.GetAllPeds让它们攻击主角
- 冲锋枪：SMG或MicroSMG？在GTA5中，冲锋枪可能是"SMG"（WeaponHash.SMG）或"MicroSMG"

礼物查询：query="玫瑰"，keywords="rose, flower"

先查询SHVDN：
- WantedLevel set
- GiveWeapon
- WeaponHash SMG
- Task attack player
- Ped relationship

把这些合并到一个query中。

SHVDN query: "Player.WantedLevel set GiveWeapon WeaponHash SMG Ped task attack player relationship hostile"

第二个SHVDN query可能需要更具体的API。

我计划：
1. 第一轮：SHVDN核心能力 + 礼物查询
2. 如果需要，第二轮SHVDN补充查询

先发起这两个调用。
</think>收到需求，开始工具查询。先并行调用SHVDN API和礼物查询。
<think>
好的，现在分析结果：

**礼物查询结果**：`Rose` 精确匹配，gift_id=5655，diamondCount=1。用户说的"玫瑰"就是 Rose。状态: matched，确认名称 Rose，gift_id 5655。

**SHVDN 结果**：
已确认的能力：
- `GTA.Player.WantedLevel` - 可读写 int 属性 ✓
- `GTA.WeaponHash.SMG` - 枚举值 ✓
- `GTA.Ped.RelationshipGroup` - 属性 ✓
- `GTA.RelationshipGroup` - 结构体 ✓
- `GTA.Ped.Task` - 返回 TaskInvoker ✓

还缺少：
- `GiveWeapon` 方法 - 需要确认 Ped.Weapons.Give(...) 或类似方法
- `TaskInvoker` 中攻击相关方法 - 如 `.FightAgainst()` 或 `.Attack()`
- `World.GetAllPeds()` 或 `World.GetNearbyPeds()`
- `RelationshipGroup` 的关系设置方法 - 如 `SetRelationshipBetweenGroups`

需要第二轮SHVDN查询。这些是独立的：
- Weapon give: "Ped Weapons Give weapon ammo"
- Task attack: "TaskInvoker FightAgainst AttackPed"
- Relationship: "RelationshipGroup SetRelationshipBetweenGroups"
- World: "World GetAllPeds GetNearbyPeds"

合并到一个query里：

"Ped.Weapons.Give WeaponAsset TaskInvoker FightAgainst AttackPed RelationshipGroup SetRelationship World GetAllPeds GetNearbyPeds"
</think>礼物明确匹配 Rose (ID: 5655)。SHVDN 第一轮已确认核心类型，但还缺武器给予、攻击任务和关系设置的具体方法，发起第二轮补充查询。
<think>
第二轮SHVDN结果分析：

新增确认的能力：
- `GTA.TaskInvoker.FightAgainst(GTA.Ped target)` 和 `FightAgainst(GTA.Ped target, int duration)` ✓
- `GTA.World.GetAllPeds(GTA.Model[] models)` ✓
- `GTA.World.GetNearbyPeds(...)` - 两个重载 ✓
- `GTA.RelationshipGroup.SetRelationshipBetweenGroups(GTA.RelationshipGroup targetGroup, GTA.Relationship relationship, bool bidirectionally)` ✓
- `GTA.WeaponAsset(GTA.WeaponHash hash)` 构造函数 ✓

但仍缺少：
- `Ped.Weapons.Give(...)` 具体方法 - 没有找到。可能需要 `WeaponAsset.Give()` 或者通过 Native 方式给予武器。

我需要确认给予武器的方法。可能的方法是 `GTA.Ped.Weapons.Give(WeaponHash, int, bool, bool)`。但SHVDN没有返回。让我想想...

在SHVDN v3中，给Ped武器通常通过：
- `ped.Weapons.Give(WeaponHash weaponHash, int ammo, bool equipNow, bool isAmmoLoaded)` 

可能没有在结果中，因为截断了(totalCandidates=21, truncated=true)。也可能不在索引中。

但我们可以用另一种方式：`WeaponAsset` 可能有一个 `Give` 方法，或者需要查找 `PedWeapons` 或 `PedWeaponCollection`。

实际上在 SHVDN 中：
- `GTA.Ped.Weapons` 返回 `GTA.PedWeaponCollection`
- `GTA.PedWeaponCollection.Give(WeaponHash, int, bool, bool)` 

让我看看是否需要再查，但已经超过2次查询限制。不过第二次返回的截断可能包含但没显示 Give 方法。

实际上我已经使用了2次SHVDN查询（配额已用完），不能再查询了。

没关系，我可以在capability_requirements中标记 `Ped.Weapons.Give` 为 needs_query（因为API返回截断），并给出 fallback 方案。

但等等，我记得在SHVDN v3中，`Ped.Weapons.Give` 确实存在。不过根据工具约束，我不能凭记忆编造。让我检查一下第一轮和第二轮的结果...

第一轮有 `GTA.Player.WantedLevel`、`GTA.WeaponHash.SMG`、`GTA.Ped.RelationshipGroup`、`GTA.RelationshipGroup`、`GTA.Ped.Task`。

第二轮有 `FightAgainst`、`GetAllPeds`、`GetNearbyPeds`、`SetRelationshipBetweenGroups`、`WeaponAsset`。

缺少的：
1. `Ped.Weapons.Give` 或 `PedWeaponCollection.Give` - 未确认
2. `GTA.Relationship` 枚举 - 需要确认值如 `Relationship.Hate`

现在我可以标记 needs_query 并给出 fallback。

实际上对于武器给予，fallback 可以是 Native 调用 `GIVE_WEAPON_TO_PED`。SHVDN 支持通过 `Function.Call` 调用 native。

另外对于 `GTA.Relationship` 枚举，我需要知道是否有 `Hate` 值。这也是一个缺失。

现在汇总所有信息，开始编写 DESIGN.md。

关键设计要素：
- 触发：礼物 Rose (ID 5655, 1钻石)
- 效果：通缉5星、所有Ped攻击主角、给冲锋枪+1000子弹
- 生存模式：需要定义持续时间、胜利/失败条件
- 由于用户未指定时长和胜利条件，我将设定合理的默认值

设计：
- 状态机：IDLE → SURVIVAL（收到Rose后进入）→ 持续300秒（5分钟）→ 如果玩家存活则SURVIVAL_SUCCESS → 回到IDLE；如果玩家死亡则SURVIVAL_FAILED → 回到IDLE
- 冷却：比如120秒冷却，防止重复触发
- 防刷：msgId去重，repeatEnd=true才触发

现在看看关系设置。让"所有玩家攻击主角"——这里"所有玩家"在GTA5语境中应该理解为所有NPC Ped。通过设置主角所在的关系组与所有其他Ped的关系组为敌对来实现。但更简单的方法：给所有附近Ped下达FightAgainst主角的任务。

实际上，更好的做法可能是：
1. 获取附近所有Ped
2. 对每个Ped（非玩家），执行 `ped.Task.FightAgainst(Game.Player.Character)`
3. 这是让所有NPC攻击主角的最直接方式

对于 `RelationshipGroup` 方式，可能需要设置全局关系，这可能更复杂。

考虑到简单性和已验证的API，我选择 `FightAgainst` 方式。同时也可以设置关系组作为备份。

对 `Ped.Weapons.Give` 的处理：标记为 needs_query，fallback 使用 `Function.Call(Hash.GIVE_WEAPON_TO_PED, ...)`。

`GTA.Relationship` 枚举也标记为 needs_query，但 `FightAgainst` 已足够，不需要依赖它。

现在编写 DESIGN.md。

还需要处理：World.GetAllPeds 需要一个 Model[] 参数，如果传 null 或空数组会怎样？可能需要传空数组或 null。或者用 GetNearbyPeds 更好，因为 GetAllPeds 会返回所有Peds（可能很多），而且需要 Model 过滤。

对于"所有玩家攻击主角"，使用 `GetNearbyPeds(playerPosition, largeRadius, null)` 更合适。半径可以设置大一些比如200米。

关于 `GetNearbyPeds` 的 Model[] 参数：可以传 null 表示不过滤。

设计要点：
- 触发后获取玩家位置，获取附近200米内所有Ped
- 对每个非玩家Ped执行 FightAgainst
- 还需要在持续时间内持续让新刷出的Ped也攻击主角——可以用定时器每10秒重新执行一次

对于通缉等级：直接 `Game.Player.WantedLevel = 5`。

对于武器：需要确认给予方法。fallback 使用 Native。

关于胜利条件：用户没有明确指定，我设定为存活300秒（5分钟）。失败条件是玩家死亡。

关于"所有玩家"：如果理解为多人模式中的所有其他玩家，那这是GTA Online概念，但在单人模式mod中，`World.GetAllPeds()` 返回NPC。在单人模式中，"所有玩家"应理解为所有NPC。我就按这个理解。

现在编写 DESIGN.md。

Gift_requirements:
- original_query: "玫瑰"
- keywords: "rose"
- status: matched
- confirmed_name: "Rose"
- gift_id: 5655
- diamondCount: 1
- candidates: 已有列表
- fallback: N/A

Capability_requirements:
- WantedLevel set: verified, GTA.Player.WantedLevel { get; set; }
- WeaponHash.SMG: verified, GTA.WeaponHash.SMG
- TaskInvoker.FightAgainst: verified, two overloads
- World.GetNearbyPeds: verified
- World.GetAllPeds: verified
- RelationshipGroup.SetRelationshipBetweenGroups: verified
- WeaponAsset constructor: verified
- Ped.Weapons.Give: needs_query, fallback: Function.Call with GIVE_WEAPON_TO_PED native
- GTA.Relationship enum: needs_query (but not essential), fallback: use FightAgainst

现在组织DESIGN.md。
</think>工具调用完成。汇总所有证据，生成 DESIGN.md。

# DESIGN.md

## 需求摘要

| 字段 | 内容 |
|------|------|
| 玩法名称 | 玫瑰生存（Rose Survival） |
| 触发方式 | LIVE Studio 礼物触发：收到礼物「玫瑰」(Rose, gift_id=5655) |
| 核心目标 | 在 5 星通缉下抵抗所有 NPC 的攻击，存活指定时长 |
| 胜利条件 | 存活 300 秒（5 分钟）后自动结束，视为成功 |
| 失败条件 | 主角死亡 |
| 语言 | 中文（Notification 提示使用中文） |
| 原始输入 | 收到礼物"玫瑰"后，开启生存模式，通缉5星，所有玩家攻击主角，给主角冲锋枪+1000子弹 |

---

## change_summary

首次生成，无变更。

## preserved_behaviors

首次生成，无保留行为。

---

## 玩法流程

```
[IDLE] ──(收到 Rose 礼物, repeatEnd=true)──▶ [SURVIVAL_START]
                                                   │
                                          ┌────────┘
                                          ▼
                                   设置 WantedLevel=5
                                   给予 WeaponHash.SMG + 1000 ammo
                                   扫描附近 NPC，执行 FightAgainst(主角)
                                          │
                                          ▼
                                   [SURVIVAL_ACTIVE]
                                   每 10 秒重新扫描附近 NPC
                                   持续攻击主角
                                          │
                              ┌───────────┴───────────┐
                              ▼                       ▼
                    玩家存活 ≥ 300 秒           玩家死亡
                              │                       │
                              ▼                       ▼
                    [SURVIVAL_SUCCESS]       [SURVIVAL_FAILED]
                     Notification 通知        Notification 通知
                     清理仇恨关系               清理
                              │                       │
                              └───────────┬───────────┘
                                          ▼
                                       [COOLDOWN]
                                      120 秒冷却
                                          │
                                          ▼
                                       [IDLE]
```

---

## 状态机

| 状态 | 描述 | 允许触发 |
|------|------|----------|
| `IDLE` | 等待礼物触发 | Rose 礼物 → `SURVIVAL_START` |
| `SURVIVAL_START` | 瞬时状态，执行初始化 | 自动过渡到 `SURVIVAL_ACTIVE` |
| `SURVIVAL_ACTIVE` | 生存模式运行中，定时刷新 NPC 仇恨 | 玩家死亡 → `SURVIVAL_FAILED`；计时到期 → `SURVIVAL_SUCCESS` |
| `SURVIVAL_SUCCESS` | 瞬时状态，通知并清理 | 自动过渡到 `COOLDOWN` |
| `SURVIVAL_FAILED` | 瞬时状态，通知并清理 | 自动过渡到 `COOLDOWN` |
| `COOLDOWN` | 120 秒冷却，无视礼物 | 冷却到期 → `IDLE` |

---

## 触发条件

| 条件 | 说明 |
|------|------|
| 事件类型 | `WebcastGiftMessage` |
| 匹配字段 | `data.gift.id == 5655` 且 `data.gift.name == "Rose"` |
| 触发策略 | `repeatEnd == true` 时触发（一次性） |
| 去重 | 按 `data.common.msgId` 去重 |
| 冷却 | 生存模式结束后 120 秒内不再触发 |
| 忙时策略 | 若已在 SURVIVAL_ACTIVE 或 COOLDOWN，忽略新礼物 |

---

## 效果表

| 效果 | 时机 | 方法 |
|------|------|------|
| 通缉等级设为 5 星 | SURVIVAL_START | `Game.Player.WantedLevel = 5` |
| 给予冲锋枪 | SURVIVAL_START | `WeaponAsset(GTA.WeaponHash.SMG)` + ammo 1000（见 fallback） |
| NPC 攻击主角 | SURVIVAL_START + 每 10 秒刷新 | `ped.Task.FightAgainst(Game.Player.Character)` |
| 生存成功通知 | SURVIVAL_SUCCESS | `Notification.Show("玫瑰生存：你成功存活了 5 分钟！")` |
| 生存失败通知 | SURVIVAL_FAILED | `Notification.Show("玫瑰生存：你被击杀了...")` |
| 清理通缉 | SURVIVAL_SUCCESS / FAILED | `Game.Player.WantedLevel = 0` |
| 清理 NPC 任务 | SURVIVAL_SUCCESS / FAILED | 对已记录 Ped 调用 `Task.ClearAll()` |

---

## capability_requirements

| capability | behavior | priority | status | verified_evidence | fallback |
|------------|----------|----------|--------|-------------------|----------|
| Player.WantedLevel set | 设置/清除通缉等级 | critical | verified | `GTA.Player.WantedLevel { get; set; }` (System.Int32) | N/A |
| WeaponHash.SMG | 冲锋枪枚举值 | critical | verified | `GTA.WeaponHash.SMG` enum literal | N/A |
| WeaponAsset constructor | 构造武器资源 | critical | verified | `GTA.WeaponAsset(GTA.WeaponHash hash)` | N/A |
| TaskInvoker.FightAgainst | 让 NPC 攻击目标 | critical | verified | `GTA.TaskInvoker.FightAgainst(GTA.Ped target)` 与 `FightAgainst(GTA.Ped target, int duration)` | N/A |
| World.GetNearbyPeds | 获取附近所有 NPC | critical | verified | `static GTA.Ped[] GetNearbyPeds(GTA.Ped ped, float radius, GTA.Model[] models)` | N/A |
| World.GetAllPeds | 获取全局 NPC（备用） | low | verified | `static GTA.Ped[] GetAllPeds(GTA.Model[] models)` | N/A |
| Ped.RelationshipGroup | 读取/设置关系组 | medium | verified | `GTA.RelationshipGroup RelationshipGroup { get; set; }` | N/A |
| RelationshipGroup.SetRelationshipBetweenGroups | 设置组间关系 | medium | verified | `void SetRelationshipBetweenGroups(GTA.RelationshipGroup targetGroup, GTA.Relationship relationship, bool bidirectionally)` | N/A |
| GTA.Relationship enum | 关系枚举值（Hate 等） | medium | needs_query | 未在搜索结果中直接返回枚举字面量 | 使用 `FightAgainst` 替代关系组方案；或通过 `Function.Call` 调用 native |
| Ped.Weapons.Give | 给予武器与弹药 | critical | needs_query | 第二轮搜索截断 (truncated=true)，未返回 `PedWeaponCollection.Give` 签名 | `Function.Call(Hash.GIVE_WEAPON_TO_PED, ped, (int)WeaponHash.SMG, 1000, false, true)` |
| Ped.Task.ClearAll | 清理 NPC 任务 | medium | needs_query | 未在搜索结果中出现 | `Function.Call(Hash.CLEAR_PED_TASKS, ped)` 或 `ped.Task.ClearAll()` |
| Game.Player.Character | 获取主角 Ped | critical | needs_query | 未在搜索结果中出现（SHVDN 基础类型，通常可用） | 通用 SHVDN 惯用法，若不可用则 `Function.Call(Hash.GET_PLAYER_PED, Game.Player.Handle)` |

---

## LIVE Studio 事件与礼物映射

### 礼物映射

| 字段 | 值 |
|------|-----|
| event_method | `WebcastGiftMessage` |
| wire_field_paths | `data.common.msgId`, `data.common.createTime`, `data.user.id`, `data.user.nickname`, `data.gift.id`, `data.gift.name`, `data.gift.diamondCount`, `data.repeatCount`, `data.repeatEnd`, `data.comboCount` |
| match_condition | `data.gift.id == 5655 && data.gift.name == "Rose"` |
| repeat_policy | 仅 `data.repeatEnd == true` 时触发 |
| dedupe_policy | 按 `data.common.msgId` 哈希去重，已处理 msgId 忽略 |
| cooldown | 模式结束后 120 秒；模式运行中不接受新触发 |
| queue_cap | 1（不排队，忙时丢弃） |
| busy_state_policy | 若当前状态为 SURVIVAL_ACTIVE 或 COOLDOWN，忽略新 Rose 礼物 |
| abort_cleanup | `Script.Aborted` 时：清除通缉等级恢复原值、清理所有记录的 Ped 任务、取消定时器、Abort WebSocket |

---

## gift_requirements

| 字段 | 值 |
|------|-----|
| original_query | 玫瑰 |
| keywords | rose,flower |
| status | matched |
| confirmed_name | Rose |
| gift_id | 5655 |
| diamondCount | 1 |
| candidates | Rose (5655, score=100, exact), Rose Hand (16896, 199钻), Rose Bear (16300, 214钻), Rosie the Rose Bean (12210, 399钻), Rose Soundwave (17983, 499钻) |
| fallback | N/A（已精确匹配） |

---

## 计时与冷却

| 参数 | 值 | 说明 |
|------|-----|------|
| 生存时长 | 300 秒 (5 分钟) | SURVIVAL_ACTIVE 最大持续时间 |
| NPC 刷新间隔 | 10 秒 | 重新扫描附近 NPC 并下达 FightAgainst |
| 模式冷却 | 120 秒 | 成功或失败后进入 COOLDOWN |
| 扫描半径 | 200 米 | GetNearbyPeds 搜索半径 |
| 每帧 NPC 上限 | 50 | 单次 FightAgainst 的 Ped 数量上限，防止卡顿 |

---

## 失败与清理

| 场景 | 清理动作 |
|------|----------|
| SURVIVAL_SUCCESS | `Game.Player.WantedLevel = 0`；对所有 tracked_peds 调用 `ClearAll()`；进入 COOLDOWN |
| SURVIVAL_FAILED（玩家死亡） | 同上；额外通知失败消息 |
| Script.Aborted | 保存并恢复原始 WantedLevel；清除所有 tracked_peds 任务；取消定时器；释放 WebSocket |
| 模式中再次收到 Rose | 忽略，不排队 |
| 玩家死亡检测 | 每帧检查 `Game.Player.Character.IsDead` |

---

## 降级方案

| 风险 | 降级 |
|------|------|
| `Ped.Weapons.Give` 不可用 | 使用 `Function.Call(Hash.GIVE_WEAPON_TO_PED, ...)` native 给予武器 |
| `GTA.Relationship` 枚举未知 | 不依赖关系组方案；完全使用 `FightAgainst` 逐 Ped 下达攻击任务 |
| `GetNearbyPeds` 返回空 | 降级使用 `World.GetAllPeds(null)` 获取全局 Ped，但限制前 50 个 |
| 通缉等级设置失败 | 使用 `Function.Call(Hash.SET_PLAYER_WANTED_LEVEL, Game.Player, 5, false)` 作为 native fallback |
| 玩家不在世界内 | 跳过本轮 NPC 扫描，等待下一次刷新 |

---

## 验收标准

1. 在 LIVE Studio 中发送 Rose 礼物（gift_id=5655），主角通缉等级立即变为 5 星
2. 主角获得 SMG（冲锋枪）且弹药为 1000 发
3. 附近 NPC 主动攻击主角（近战或使用武器）
4. 每 10 秒新刷出的 NPC 也加入攻击
5. 存活 5 分钟后收到成功通知，通缉清零，NPC 停止攻击
6. 玩家在模式中死亡，收到失败通知，通缉清零，NPC 停止攻击
7. 模式结束后 120 秒内再次发送 Rose 礼物，不会触发
8. 模式运行中重复发送 Rose，不重复触发
9. 脚本卸载时恢复通缉等级、清理 NPC 任务、关闭 WebSocket
10. 不删除玩家原有武器，不无条件覆盖通缉状态（仅设置/恢复）