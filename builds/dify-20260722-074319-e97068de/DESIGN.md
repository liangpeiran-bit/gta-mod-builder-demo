<think>
用户要求设计一个"玫瑰生存"玩法。让我分析：

1. 玩法目标：收到礼物"玫瑰"后触发生存挑战，5星通缉，所有NPC攻击主角，给主角一把冲锋枪和1000发子弹
2. 需要调用两个工具：礼物检索（玫瑰）和SHVDN API能力检索（设置通缉等级、给予武器、给予弹药等）

首先我需要确认"玫瑰"这个礼物在LIVE Studio中的gift_id，以及需要的GTA5 SHVDN API能力。

让我并行调用这两个工具。

注意：用户说"所有玩家攻击主角" - 在GTA5单人模式中，这应该是所有NPC/警察攻击主角。通过设置5星通缉等级，警察会自然攻击主角。

还需要查询：设置通缉等级、给予武器、给予弹药、以及可能需要的其他能力。
</think>让我先同时检索礼物"玫瑰"和必要的 SHVDN 能力。
<think>
让我整理检索结果：

**礼物检索**：找到 "Rose" (gift_id: 5655, diamondCount: 1)， exact_name 匹配。这是最可能的匹配。还有 Rose Hand、Rose Bear、Rosie the Rose Bean、Rose Soundwave 等候选。但用户说"玫瑰"，最直接的就是 "Rose" (id: 5655)。确认为 confirmed。

**SHVDN 能力检索**：
- `GTA.Player.WantedLevel` - 可设置通缉等级
- `GTA.Game.MaxWantedLevel` - 最大通缉等级
- `GTA.WeaponHash.SMG` - SMG 武器 hash
- `GTA.WeaponAsset` - 用于给予武器

现在还需要确认一些关键 API：如何给玩家武器和弹药。我需要进一步查询 `GTA.Player.Weapons` 或者 `GTA.Ped.Weapons`，以及 `Give`、`Ammo` 等相关 API。

让我查询更具体的武器给予和弹药设置 API。
</think>需要进一步确认武器给予和弹药设置的具体 API。
<think>
现在我有足够的信息来生成 DESIGN.md。总结关键发现：

**礼物**：
- "玫瑰" → "Rose" (gift_id: "5655", diamondCount: 1)，exact match

**SHVDN 能力**：
- `GTA.Player.WantedLevel` (get/set) - 设置通缉等级
- `GTA.Game.MaxWantedLevel` - 最大通缉等级为5
- `GTA.Ped.Weapons` → `GTA.WeaponCollection`
- `GTA.WeaponCollection.Give(WeaponHash weaponHash, int ammoCount, bool equipNow, bool isAmmoLoaded)` - 给予武器和弹药
- `GTA.WeaponHash.SMG` - SMG 冲锋枪
- `GTA.Weapon.Ammo` (get/set) - 弹药数量

用户说"冲锋枪"，最接近的是 SMG (GTA.WeaponHash.SMG)。用户要 1000 发子弹。

现在设计玩法：

**玩法名称**：玫瑰生存

**核心流程**：
1. 收到礼物"Rose" (gift_id: "5655") 后触发
2. 将通缉等级设置为 5 星
3. 给主角一把 SMG 冲锋枪和 1000 发子弹
4. 5 星通缉下，警察会自然攻击主角（"所有玩家攻击主角"在单人模式中解读为警察/NPC攻击）
5. 主角死亡 = 失败

**状态机**：
- IDLE：等待触发
- SURVIVAL：生存模式激活中
- ENDED：主角死亡，生存模式结束

**设计考量**：
- 用户没有指定胜利条件，只说了失败条件是主角死亡。所以这本质上是一个"存活尽可能久"的挑战。
- 用户没有指定时长限制，就不添加时间限制。
- 需要冷却机制：送礼后进入冷却，防止重复触发。
- 玩家死亡时清理状态。

**触发映射**：
- trigger: GiftEvent
- matcher: gift_id "5655" (Rose)
- gift_strategy: once（一次性触发，不是每次事件）
- cooldown: 需要设置一个合理的冷却时间。因为生存模式可能持续很久，冷却应该足够长。比如 60 秒或 120 秒。实际上考虑到生存模式可能持续很久甚至跨多个游戏会话，冷却应该设置在触发后，而且如果在 SURVIVAL 状态下收到礼物应该忽略。

让我仔细考虑：如果玩家已经在生存模式中，再次收到玫瑰怎么办？应该忽略（防刷）。冷却应该在生存模式结束后才开始计算。

实际上更简单的设计：
- 只在 IDLE 状态时接受触发
- 触发后进入 SURVIVAL 状态
- 玩家死亡后进入 DEAD 状态，短暂冷却后回到 IDLE

或者：
- 冷却只用于防止短时间内重复触发。设为 30 秒。
- 如果在 SURVIVAL 状态收到礼物，忽略。

让冷却 = 30 秒，玩家死亡时消耗冷却（即死亡后冷却30秒才能再次触发）。

现在整理输出。

关于"所有玩家攻击主角"——在 GTA5 单人模式中，"所有玩家"没有意义。5 星通缉已经会让警察、SWAT、直升机等攻击主角。这是最自然的解读。

关于 `Game.MaxWantedLevel` - 这确认了最大通缉等级是5，5星是最高。直接设置 `Game.Player.WantedLevel = 5` 即可。

现在生成 DESIGN.md。
</think>现在所有必要信息已齐备，开始生成 DESIGN.md。

# DESIGN.md

## 1. 需求摘要
- **玩法名称**：玫瑰生存
- **核心目标**：收到礼物"玫瑰"后自动开启生存模式，在 5 星通缉下存活尽可能久
- **触发条件**：直播间观众送出礼物"Rose"（gift_id: "5655"）
- **胜利条件**：无明确胜利条件（纯生存挑战，存活越久越好）
- **失败条件**：主角死亡
- **正向互动**：触发后通缉等级升至 5 星、给予一把 SMG 冲锋枪及 1000 发子弹
- **时长限制**：无（用户未指定）
- **礼物要求**：已指定 —— "Rose"（玫瑰）

## 2. change_summary 与 preserved_behaviors
- **change_summary**：首次生成 / 无
- **preserved_behaviors**：首次生成 / 无

## 3. 玩法流程
1. 玩法启动后处于 **IDLE** 状态，等待触发。
2. 直播间送出礼物 **Rose**（gift_id: "5655"）→ 检查当前状态：
   - 若 **IDLE**：进入 **SURVIVAL** 状态，执行步骤 3~5。
   - 若已在 **SURVIVAL** 或 **COOLDOWN**：忽略该礼物（防刷）。
3. 将主角通缉等级设为 **5 星**（`Game.Player.WantedLevel = 5`）。
4. 给予主角 **SMG** 冲锋枪一把，弹药 **1000** 发，并自动切换到该武器。
5. 5 星通缉下，警察、SWAT、直升机等 NPC 会持续攻击主角，主角用 SMG 反击求生。
6. 主角死亡 → 进入 **COOLDOWN** 状态，冷却结束后回到 **IDLE**。

## 4. 状态机
| 状态 | 说明 | 允许触发 |
|------|------|----------|
| **IDLE** | 等待礼物触发 | 是 |
| **SURVIVAL** | 生存模式进行中，5 星通缉 | 否（忽略礼物） |
| **COOLDOWN** | 主角死亡后的冷却期 | 否（忽略礼物） |

- 状态转换：
  - `IDLE → SURVIVAL`：收到 Rose 礼物
  - `SURVIVAL → COOLDOWN`：主角死亡
  - `COOLDOWN → IDLE`：冷却计时结束（30 秒）

## 5. LIVE Studio 触发映射

| 字段 | 值 |
|------|-----|
| **trigger** | GiftEvent |
| **event_type** | gift |
| **matcher** | gift_id = "5655"（Rose） |
| **gift_strategy** | once |
| **cooldown** | 30 秒（死亡后冷却） |
| **gameplay_behavior** | 仅在 IDLE 状态响应；进入 SURVIVAL：5 星通缉 + SMG + 1000 弹药 |
| **death_policy** | 玩家死亡 → 进入 COOLDOWN，冷却期间忽略新礼物，冷却结束回到 IDLE |

## 6. 游戏效果
- **通缉等级**：`Game.Player.WantedLevel = 5`（5 星，GTA5 最高通缉）
- **武器**：`Game.Player.Character.Weapons.Give(WeaponHash.SMG, 1000, true, true)`
  - `WeaponHash.SMG`：冲锋枪
  - `ammoCount = 1000`
  - `equipNow = true`：立即装备
  - `isAmmoLoaded = true`：弹药已装填
- **无额外实体生成**，仅依赖 GTA 内置通缉系统生成警察。

## 7. capability_requirements

| capability | behavior | priority | status | verified_evidence | fallback |
|------------|----------|----------|--------|-------------------|----------|
| Player.WantedLevel (set) | 设置通缉等级为 5 | must | verified | `GTA.Player.WantedLevel { get; set; }` — SHVDN v3 属性 | 无（核心能力，已核验） |
| Game.MaxWantedLevel | 确认最大通缉等级 ≥ 5 | must | verified | `GTA.Game.MaxWantedLevel { get; set; }` — 默认返回 5 | 无 |
| Ped.Weapons (WeaponCollection) | 访问玩家武器集合 | must | verified | `GTA.Ped.Weapons { get; }` → `GTA.WeaponCollection` | 无 |
| WeaponCollection.Give | 给予武器与弹药 | must | verified | `GTA.WeaponCollection.Give(WeaponHash, int, bool, bool)` | 无 |
| WeaponHash.SMG | SMG 冲锋枪枚举值 | must | verified | `GTA.WeaponHash.SMG` | 无 |
| Weapon.Ammo | 设置弹药数（Give 已内置） | optional | verified | `GTA.Weapon.Ammo { get; set; }` | 通过 Give 的 ammoCount 参数直接设置 |

## 8. gift_requirements

| 字段 | 值 |
|------|-----|
| **original_query** | 玫瑰 |
| **keywords** | rose, flower |
| **status** | matched |
| **confirmed_name** | Rose |
| **gift_id** | "5655" |
| **diamondCount** | 1 |
| **candidates** | Rose (5655, exact), Rose Hand (16896), Rose Bear (16300), Rosie the Rose Bean (12210), Rose Soundwave (17983) |
| **fallback** | 无（exact match，直接使用 gift_id "5655"） |

## 9. 冷却、限流与有界状态
- **触发冷却**：无触发冷却（进入 SURVIVAL 后自动忽略新礼物）。
- **死亡冷却**：30 秒（COOLDOWN 状态），死亡后开始计时，计时结束回到 IDLE。
- **实体上限**：无额外实体生成，警察由 GTA 通缉系统自然管理。
- **防刷**：SURVIVAL 和 COOLDOWN 状态下忽略所有 Rose 礼物。

## 10. 玩家死亡、失败与 OnGameplayAborted 清理
- **玩家死亡检测**：在 SURVIVAL 状态下每 Tick 检查 `Game.Player.Character.IsDead`。
- **死亡处理**：
  1. 状态切换至 COOLDOWN。
  2. 启动 30 秒冷却计时器。
  3. 不主动清除通缉等级（死亡后 GTA 自动重置通缉）。
  4. 不收回武器（GTA 死亡后武器自然处理）。
- **OnGameplayAborted 清理**：
  - 重置状态机为 IDLE。
  - 清除冷却计时器。
  - 无需清理实体（无额外生成实体）。

## 11. 降级方案
- 本玩法依赖的能力均已核验，无需降级。
- 若 SMG 武器因未知原因无法给予（极端情况），静默失败，不阻塞通缉等级设置——通缉效果仍生效。
- 若 `Game.Player.WantedLevel` 设置失败（极端情况），玩法静默失败并回到 IDLE。

## 12. 验收标准
1. 收到 Rose 礼物时，主角通缉等级立即变为 5 星。
2. 主角获得 SMG 冲锋枪一把，弹药为 1000 发，且自动切换到 SMG。
3. 5 星通缉下警察、SWAT 和直升机持续追击主角。
4. SURVIVAL 状态下再次收到 Rose 礼物，不产生任何效果。
5. 主角死亡后，进入 30 秒冷却，冷却期间收到 Rose 礼物不触发。
6. 冷却结束后，收到 Rose 礼物可重新触发生存模式。
7. 非 Rose 礼物不触发任何效果。
8. OnGameplayAborted 后状态干净重置，不影响下次启动。