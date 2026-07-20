<think>
用户想要设计一个生存玩法：

1. 收到礼物"玫瑰"后触发
2. 开启生存模式
3. 通缉等级改为5星
4. 所有玩家攻击主角（可能是NPC或附近的行人攻击主角？"所有玩家"可能指所有NPC敌人，因为在GTA5中"所有玩家"通常指所有NPC。但在LIVE Studio上下文中，也可能是其他观众玩家。不过更合理的解释是让所有NPC（行人/警察）攻击主角）
5. 给主角发一把冲锋枪并设置1000子弹

首先我需要查询礼物"玫瑰"的信息，同时查询SHVDN API相关能力。

需要查询的SHVDN能力：
- 设置通缉等级 (WantedLevel, Player.WantedLevel)
- 给武器和弹药 (GiveWeapon, WEAPON_SMG 或类似)
- 让NPC攻击玩家 (Ped, Task, FightAgainst, RelationshipGroup)
- 创建敌对NPC或让附近NPC攻击

让我同时发起礼物查询和SHVDN查询。

第一轮：礼物查询 + SHVDN核心能力查询

礼物查询：query = "玫瑰"，keywords = "rose"

SHVDN查询：需要查询设置通缉等级、给武器、让NPC攻击玩家等能力。

第一次SHVDN查询应该覆盖核心能力：
- Player.WantedLevel, Game.Player.WantedLevel, set wanted level
- GiveWeapon, WeaponHash, SMG
- RelationshipGroup, set relationship, attack player, ped task combat

这些可以放在一个query里。

第二次SHVDN查询可以查询更具体的东西如果需要的话。

让我先发出第一轮查询。
</think>好的，我现在同时查询礼物"玫瑰"和核心 GTA5 能力。
<think>
好的，礼物查询结果：
- "Rose" 精确匹配，id=5655，diamondCount=1，score=100
- 用户说的是"玫瑰"，最匹配的是 "Rose"，score=100，exact_name。

礼物工具 status=matched，礼物 "Rose"（id=5655, diamondCount=1）。

SHVDN查询结果：
- Player.WantedLevel - get/set，confirmed
- GTA.WeaponHash.SMG - confirmed
- Ped.RelationshipGroup - get/set，confirmed
- GTA.RelationshipGroup struct - confirmed
- TaskInvoker.FightAgainst - confirmed（两个重载）

还需要查询的能力：
- GiveWeapon / Weapons.Give - 给玩家武器的方法
- Ped.Weapons 或类似
- World.GetNearbyPeds 或获取附近行人
- RelationshipGroup.SetRelationshipBetweenGroups 或类似

这些在第一次查询中没有完全覆盖。让我发起第二次查询。

需要查询：
- Weapons.Give, Ped.Weapons, give weapon to ped
- World.GetNearbyPeds, nearby peds
- Relationship.SetRelationshipBetweenGroups, set relationship group relation

让我把这些放进第二次查询。
</think>礼物"Rose"已精确匹配（id=5655）。SHVDN 第一轮已确认 WantedLevel、WeaponHash.SMG、RelationshipGroup、FightAgainst。现在查询剩余的独立能力：武器发放、附近行人获取、关系组设置。
<think>
第二轮SHVDN结果返回了：

已确认：
- Game.Player - static property
- Game.Player.Character - GTA.Ped Character { get; }
- Ped.Weapons - GTA.WeaponCollection Weapons { get; }
- World.GetNearbyPeds - 两个重载（Vector3位置和Ped为中心）
- RelationshipGroup.SetRelationshipBetweenGroups - 签名确认

关键缺失：WeaponCollection.Give 方法。第二轮被截断了（truncated: true），但WeaponCollection.Give没有被列出来。

不过我们知道Ped.Weapons返回WeaponCollection，WeaponCollection应该有一个Give方法。但根据SHVDN工具的返回，我们没有明确看到它。

让我基于现有信息推断。在SHVDN v3中，WeaponCollection通常有：
- Give(WeaponHash weaponHash, int ammoCount, bool equipNow, bool isAmmoLoaded)

但这需要确认。第二轮没有返回Give方法的具体签名。不过由于truncated=true，Give方法可能在其中但被截断了。

另外需要确认 GTA.Relationship 枚举（用于 SetRelationshipBetweenGroups）。

我还需要确认：Notification（发送通知）、Game.Player.WantedLevel（已在第一轮确认）。

现在汇总所有能力：

第一轮确认：
- Player.WantedLevel (get/set) ✓
- GTA.WeaponHash.SMG ✓
- Ped.RelationshipGroup ✓
- GTA.RelationshipGroup (struct, constructor) ✓
- TaskInvoker.FightAgainst ✓

第二轮确认：
- Game.Player ✓
- Game.Player.Character ✓
- Ped.Weapons → WeaponCollection ✓
- World.GetNearbyPeds ✓
- RelationshipGroup.SetRelationshipBetweenGroups ✓

还需要但未明确确认的：
- WeaponCollection.Give - 可能被截断，标记为 needs_query
- GTA.Relationship 枚举 - 未明确返回

对于WeaponCollection.Give，我可以标记为 needs_query，但作为fallback，我们可以使用Game.Player.Character.Weapons.Give(...) 这个常见模式。在SHVDN v3中这个方法是存在的。

对于GTA.Relationship枚举，通常有Hate、Dislike、Neutral、Like、Respect等值。

现在准备输出DESIGN.md。

玩法设计：

1. 触发：收到礼物"Rose"（gift_id=5655, diamondCount=1）
2. 效果：
   - 通缉等级设为5星：Game.Player.WantedLevel = 5
   - 所有附近NPC攻击主角：使用RelationshipGroup将附近Peds设为敌对
   - 给主角冲锋枪+1000子弹：Game.Player.Character.Weapons.Give(WeaponHash.SMG, 1000, true, true)

关于"所有玩家攻击主角"：在GTA5单机语境下，"玩家"更合理理解为附近的NPC/行人。我们将使用World.GetNearbyPeds获取附近行人，然后让他们攻击主角。

流程：
- 收到礼物Rose → 检查冷却 → 设置通缉5星 → 给武器 → 让附近NPC攻击 → 通知玩家

冷却机制：比如60秒，防止连续触发。

状态机：
- IDLE → 等待礼物Rose
- SURVIVAL_ACTIVE → 生存模式进行中
- 冷却中 → 等待冷却结束

失败条件：主角死亡 → 结束生存模式（但用户没有明确要求反复触发，所以死亡后不再自动开启）

清理：Script.Aborted时清理，重置关系组等。

让我设计完整的DESIGN.md。

关于"所有玩家攻击主角"的解读：原文是"所有玩家攻击主角"。在GTA5中，"玩家"可能指：
1. 所有NPC行人 - 范围太大，技术上可通过关系组实现
2. 附近一定范围内的NPC

我选择用RelationshipGroup方案：创建一个新的敌对关系组，将附近NPC加入该组并设为仇恨关系，让他们攻击主角。这样更可控。

另外，5星通缉本身就会让警察大量出现并攻击主角，这已经是很强的"所有警察攻击主角"。配合让附近行人也敌对，效果更激烈。

关于WeaponCollection.Give - 虽然第二轮被截断了，但这是SHVDN v3中非常标准的方法。我会在capability表中标记为 needs_query，并在fallback中说明常见签名。

现在关于GTA.Relationship枚举：这也是标准枚举。也标记为needs_query。

让我开始输出DESIGN.md。
</think>所有查询已完成。礼物 Rose (id=5655) 精确命中，SHVDN 两轮覆盖核心能力。唯一缺口是 `WeaponCollection.Give` 因截断未返回，`GTA.Relationship` 枚举亦未直接返回。现在输出完整 DESIGN.md。

# DESIGN.md

## 需求摘要

| 字段 | 内容 |
|---|---|
| 玩法名称 | 玫瑰生存 (Rose Survival) |
| 触发方式 | 收到 LIVE Studio 礼物 **Rose**（gift_id=5655，diamondCount=1）时自动触发 |
| 核心目标 | 在 5 星通缉 + 附近 NPC 围攻下存活 |
| 胜利条件 | 未明确指定；默认存活即胜利（生存模式持续运行直到玩家死亡或脚本终止） |
| 失败条件 | 主角被击杀 |
| 正向效果 | 主角获得一把冲锋枪（SMG）及 1000 发子弹 |
| 负向效果 | 通缉等级升至 5 星；附近所有 NPC 行人切换为敌对并攻击主角 |
| 语言 | 中文 |

## 玩法流程

```
[IDLE] ──收到 WebcastGiftMessage(gift.name="Rose", repeatEnd=true)──▶
    │
    ├─ 检查冷却：若在冷却中则忽略（重复送礼不叠加）
    ├─ 设置通缉等级为 5
    ├─ 给主角 SMG + 1000 子弹
    ├─ 将附近 NPC 关系组设为敌对（Hate），命令攻击主角
    ├─ 通知玩家："🌹 玫瑰生存模式启动！通缉5星，杀出重围！"
    └─▶ [SURVIVAL_ACTIVE]
              │
              ├─ 玩家死亡 ──▶ [DEAD] ── 通知失败 ──▶ [IDLE]（可重新触发）
              └─ 脚本 Abort ──▶ 清理
```

## 状态机

| 状态 | 说明 | 可触发的转换 |
|---|---|---|
| `IDLE` | 等待礼物触发，无生存模式运行 | 收到 Rose → `SURVIVAL_ACTIVE` |
| `COOLDOWN` | 冷却期（默认 60 秒），期间忽略重复 Rose 礼物 | 冷却结束 → `IDLE` |
| `SURVIVAL_ACTIVE` | 生存模式运行中，5 星通缉 + NPC 敌对 | 玩家死亡 → `DEAD`；Abort → 清理 |
| `DEAD` | 玩家已死亡，生存模式结束 | 自动 → `COOLDOWN` → `IDLE` |

## 触发条件

| 条件 | 规则 |
|---|---|
| 事件类型 | `WebcastGiftMessage`，`data.common.method = "WebcastGiftMessage"` |
| 礼物匹配 | `data.gift.id == 5655` 且 `data.gift.name == "Rose"` |
| 触发策略 | `repeatEnd == true` 时触发一次性效果（默认策略） |
| 去重 | 按 `data.common.msgId` 去重 |
| 冷却 | 60 秒；冷却期间收到 Rose 仅忽略，不排队 |
| 队列上限 | 1（busy 时丢弃新请求） |

## 效果表

| 序号 | 效果 | 实现方式 | 参数 |
|---|---|---|---|
| 1 | 通缉等级 5 星 | `Game.Player.WantedLevel = 5` | — |
| 2 | 发放冲锋枪 | `Game.Player.Character.Weapons.Give(WeaponHash.SMG, 1000, true, true)` | SMG, 1000 发, 装备, 已装弹 |
| 3 | 附近 NPC 敌对 | 创建临时 `RelationshipGroup`，设为与玩家 `Hate`；`World.GetNearbyPeds` 获取半径 50m 内行人，将其 `RelationshipGroup` 改为敌对组，并调用 `TaskInvoker.FightAgainst(playerPed)` | 半径 50m，双向仇恨 |
| 4 | 屏幕通知 | `Notification.Show("🌹 玫瑰生存模式启动！通缉5星，杀出重围！")` | — |
| 5 | 死亡通知 | `Notification.Show("💀 你倒在了玫瑰生存中...")` | — |

## Capability Requirements

| capability | behavior | priority | status | verified_evidence | fallback |
|---|---|---|---|---|---|
| `Game.Player.WantedLevel` (set) | 设置通缉等级为 5 | MUST | verified | `GTA.Player.WantedLevel { get; set; }`，score=88 | — |
| `GTA.WeaponHash.SMG` | 冲锋枪枚举值 | MUST | verified | `GTA.WeaponHash.SMG`，score=88 | — |
| `Ped.Weapons` → `WeaponCollection` | 获取玩家武器集合 | MUST | verified | `GTA.Ped.Weapons { get; } → GTA.WeaponCollection`，score=88 | — |
| `WeaponCollection.Give` | 发放武器与弹药 | MUST | needs_query | 第二轮截断未返回；SHVDN v3 标准签名：`Give(WeaponHash, int ammo, bool equip, bool loaded)` | 若不可用，改用 `Game.Player.Character.Weapons[WeaponHash.SMG].Ammo = 1000` 两步操作 |
| `World.GetNearbyPeds` | 获取附近行人 | MUST | verified | `static GTA.Ped[] GetNearbyPeds(GTA.Ped ped, System.Single radius, GTA.Model[] models)`，score=88 | — |
| `Ped.RelationshipGroup` (set) | 修改 NPC 关系组 | MUST | verified | `GTA.Ped.RelationshipGroup { get; set; }`，score=88 | — |
| `GTA.RelationshipGroup` (struct) | 创建自定义关系组 | MUST | verified | `GTA.RelationshipGroup(System.Int32 hash)` 构造函数，score=88 | — |
| `RelationshipGroup.SetRelationshipBetweenGroups` | 设置组间关系为 Hate | MUST | verified | `System.Void SetRelationshipBetweenGroups(GTA.RelationshipGroup targetGroup, GTA.Relationship relationship, System.Boolean bidirectionally)`，score=88 | — |
| `GTA.Relationship` 枚举 | Hate 值 | MUST | needs_query | 未直接返回；SHVDN v3 标准枚举值 `GTA.Relationship.Hate` | 若不可用，使用整数值 `5`（GTA5 原生 HATE 值） |
| `TaskInvoker.FightAgainst` | NPC 攻击玩家 | MUST | verified | `System.Void FightAgainst(GTA.Ped target)` 和带 duration 重载，score=88 | — |
| `Game.Player.Character` | 获取玩家 Ped | MUST | verified | `GTA.Player.Character { get; } → GTA.Ped`，score=88 | — |
| `Notification.Show` | 屏幕通知 | SHOULD | needs_query | 未查询；SHVDN v3 标准：`GTA.Notification.Show(string)` | 若不可用，降级为 `UI.Notify(string)` 或不显示 |
| `Game.Player` (static) | 获取本地玩家 | MUST | verified | `static GTA.Player Game.Player { get; }`，score=88 | — |

## LIVE Studio 事件与礼物映射

### gift_requirements

| 字段 | 值 |
|---|---|
| original_query | 玫瑰 |
| keywords | rose |
| status | matched |
| confirmed_name | Rose |
| gift_id | 5655 |
| diamondCount | 1 |
| candidates | Rose (5655, score=100, exact_name) |
| fallback | — |

### 事件映射

| event_method | wire_field_paths | repeat_policy | dedupe_policy | cooldown | queue_cap | busy_state_policy | abort_cleanup |
|---|---|---|---|---|---|---|---|
| `WebcastGiftMessage` | `data.gift.id`, `data.gift.name`, `data.repeatEnd`, `data.common.msgId` | 仅在 `repeatEnd=true` 时触发 | 按 `msgId` 去重（HashSet） | 60 秒 | 1 | 丢弃（不排队） | 重置关系组，清理 HashSet |

### WebSocket 连接细节

| 参数 | 值 |
|---|---|
| 地址 | `ws://127.0.0.1:60080` |
| 订阅结构 | `{"type":"subscribe","id":"gta-mod","data":{"type":"serviceSignalSub","name":"IM_MESSAGE_TRANSPORT"}}` |
| JSON 库 | `System.Web.Script.Serialization.JavaScriptSerializer` (.NET Framework 4.8) |
| 线程模型 | WebSocket 接收 → 后台线程解析 → `MainThreadDispatcher.Enqueue` → `Script.Tick` 消费 |
| 主线程操作 | 所有 `Game.*`、`World.*`、`Ped.*`、`Notification.*` 操作必须经 `MainThreadDispatcher` 排队 |

## 计时与冷却

| 参数 | 值 |
|---|---|
| 冷却时长 | 60 秒（从效果触发完成起算） |
| 冷却行为 | 冷却期间收到 Rose 礼物：仅忽略，不排队、不延迟触发 |
| 去重存储 | `HashSet<string>` 存储已处理的 `msgId`，容量上限 1000；超出时清空重置 |
| 生存模式最大时长 | 无上限（直到玩家死亡或脚本终止） |

## 失败与清理

| 场景 | 清理动作 |
|---|---|
| 玩家死亡 | 通知 "💀 你倒在了玫瑰生存中..."；进入 COOLDOWN（60 秒后回到 IDLE） |
| Script.Aborted | (1) 取消 WebSocket 接收回调 (2) Abort/Dispose WebSocket（非 GTA 主线程） (3) 若自定义关系组仍存在，恢复为默认 (4) 不恢复通缉等级（5星是游戏机制自然处理） (5) 不清除武器 |
| 正常退出 | 同上 |

### 关系组清理细节

- 创建自定义 `RelationshipGroup` 时使用固定 hash（如 `"ROSE_SURVIVAL".GetHashCode()` 或硬编码值 `0x52E01111`）
- 存活 NPC 的关系组在 `SetRelationshipBetweenGroups` 后不会自动恢复；Script.Aborted 时将该自定义组与玩家的关系设为 `Neutral`/`Respect`
- 不恢复 NPC 原有关系组（无法追踪），仅解除敌对关系

## 降级方案

| 场景 | 降级策略 |
|---|---|
| `WeaponCollection.Give` 不可用 | 分两步：先 `Game.Player.Character.Weapons[WeaponHash.SMG].Ammo = 1000`，若武器不存在则先通过别的方式获取（如 `Game.Player.Character.Weapons.Give` 的替代 Native 调用 `GIVE_WEAPON_TO_PED`） |
| `GTA.Relationship.Hate` 不可解析 | 使用原始整数 `5`（GTA5 原生 Relationship 值：Hate=5） |
| `Notification.Show` 不可用 | 降级为 `UI.Notify(string)` |
| 附近无 NPC | 仅设置通缉 5 星 + 给武器；不报错、不阻塞 |
| WebSocket 断线 | 静默重连（间隔 5 秒，最多 3 次），重连后重新订阅；期间已有生存模式继续运行不受影响 |

## 验收标准

1. **礼物触发**：直播中送出 "Rose"（id=5655）且 `repeatEnd=true` 时，一秒内触发生存模式
2. **通缉等级**：触发后 `Game.Player.WantedLevel` 立即变为 5
3. **武器发放**：主角获得 SMG，弹药为 1000 发（或原弹药+1000）
4. **NPC 敌对**：半径 50m 内至少一个行人切换为攻击主角（在市区测试应有明显效果）
5. **通知可见**：屏幕显示玫瑰生存启动通知
6. **去重**：同一 `msgId` 不触发第二次
7. **冷却**：60 秒内重复送出 Rose 不会叠加效果
8. **死亡处理**：玩家死亡后生存模式结束，通知显示，60 秒后可重新触发
9. **清理**：Script.Aborted 后自定义关系组不再仇恨玩家，WebSocket 正确关闭
10. **线程安全**：所有 GTA 操作经 MainThreadDispatcher 执行，无死锁或崩溃