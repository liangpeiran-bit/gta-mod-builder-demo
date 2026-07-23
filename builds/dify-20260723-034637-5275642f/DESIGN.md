<think>
用户需求很清晰：设计一个生存玩法，收到礼物"玫瑰"后触发。需要做以下事情：

1. 开启生存模式
2. 把通缉等级改为5星
3. 所有玩家攻击主角（这个有意思 - 可能需要让NPC敌对，或者如果有多人模式的话...但这是GTA5单人模式，所以"所有玩家"可能指的是NPC？不，GTA5是单人游戏，没有其他玩家。所以"所有玩家攻击主角"可能意味着让所有NPC（行人）攻击主角？或者可能指的是让警察攻击？考虑到通缉等级已经是5星，警察自然会攻击。但用户明确说"所有玩家攻击主角"，在GTA5上下文中这可能意味着让周围的行人/peds攻击主角。）我需要用SHVDN API查询来确认。

4. 给主角发一把冲锋枪并设置1000子弹
5. 以及"认为需要进行的操作" - 比如可能需要无敌状态、护甲、或者一些额外的东西来让生存模式更有趣。

首先，我需要：
1. 查询礼物"玫瑰"的信息
2. 查询SHVDN API能力：设置通缉等级、给武器、设置子弹、让peds攻击玩家、可能需要的其他能力

让我同时进行这些查询。
</think>好的，我先同时查询礼物"玫瑰"的信息和必要的SHVDN API能力。
<think>
好的，现在我有了一些信息。让我总结一下：

礼物方面：找到了"Rose"（玫瑰），gift_id=5655，diamondCount=1，精确匹配。

SHVDN API方面：部分匹配到了：
- `GTA.Player.WantedLevel` - 设置通缉等级
- `GTA.Ped.RelationshipGroup` - 关系组
- `GTA.RelationshipGroup` - 关系组结构体
- `GTA.PickupType.WeaponSMG` - SMG武器拾取类型

但我还需要更多信息：
- 如何给玩家武器和弹药：`GTA.Ped.Weapons` 集合，`WeaponAsset`，`GiveWeapon` 之类
- 如何让peds攻击玩家：需要 `RelationshipGroup` 设置敌对关系
- 玩家无敌或其他保护

让我再查询一次，获取更具体的API信息。

我需要查询：
- 给武器：`WeaponAsset`, `GiveWeapon`, `WeaponHash`, `WeaponGroup` 
- 设置弹药：`Ammo`, `SetAmmo`
- 设置关系让NPC攻击玩家：`SetRelationship`, `Relationship`, `Hate`, `ped attack`
- 无敌：`IsInvincible`, `Invincible`
- 什么冲锋枪可用：SMG, MicroSMG, CombatPDW等

让我精确查询这些。
</think>礼物"玫瑰"已确认（gift_id=5655）。还需要更多 SHVDN API 细节，让我继续查询武器和关系系统。
<think>
好的，我现在已经收集到足够信息了。让我整理一下已确认的API：

已确认的API：
1. `GTA.Player.WantedLevel` - 设置通缉等级，`System.Int32 WantedLevel { get; set; }`
2. `GTA.Ped.RelationshipGroup` - `GTA.RelationshipGroup RelationshipGroup { get; set; }`
3. `GTA.RelationshipGroup` - 结构体，有构造函数 `RelationshipGroup(int hash)` 和 `RelationshipGroup(uint hash)`
4. `GTA.RelationshipGroup.SetRelationshipBetweenGroups(GTA.RelationshipGroup targetGroup, GTA.Relationship relationship, bool bidirectionally = )` - 设置关系
5. `GTA.WeaponAsset` - 构造函数 `WeaponAsset(WeaponHash hash)` 等
6. `GTA.Ped.Weapons` - `GTA.WeaponCollection Weapons { get; }` - 武器集合
7. `GTA.Weapon.Hash` - `GTA.WeaponHash Hash { get; }`
8. `GTA.PickupType.WeaponSMG` - SMG武器拾取类型

我还需要知道：
- `GTA.WeaponCollection` 有哪些方法？应该有 `Give` 或类似方法
- `GTA.Relationship` 枚举有哪些值？应该有 `Hate`、`Neutral`、`Respect` 等
- `GTA.WeaponHash` - SMG的hash是什么？可能是 `WeaponHash.SMG`、`WeaponHash.MicroSMG` 等

这些是SHVDN v3的常用API，我可以合理推断。但我需要确保这些存在。我得用我已经确认的信息来设计。

实际上我已经有了足够信息来构建设计文档。关键API都已经确认：

1. `Game.Player.WantedLevel = 5` - 设置5星通缉
2. 给武器：通过 `Game.Player.Character.Weapons.Give(...)` 或类似的API。WeaponCollection 在 SHVDN v3 中通常有 `Give(WeaponHash weaponHash, int ammoCount, bool equipNow, bool isAmmoLoaded)` 方法。让我假设这个存在（这是标准SHVDN API）。
3. `Game.Player.Character.Weapons[WeaponHash.SMG].Ammo = 1000` - 设置弹药
4. `RelationshipGroup.SetRelationshipBetweenGroups` - 设置所有NPC对玩家的关系为Hate
5. 可能还需要 `Game.Player.Character.IsInvincible = true` 来增加生存能力

对于"WeaponCollection.Give"方法我还没有显式确认，但这在SHVDN中是非常标准的方法。我可以将其标记为需要fallback。

对于"所有玩家攻击主角"——由于GTA5是单人游戏，这里的"玩家"可能是指NPC（行人）。用户可能不太熟悉游戏术语。在5星通缉下，警察自然会攻击主角。但用户可能还想让普通行人也攻击主角。这可以通过RelationshipGroup来实现。

让我设计这个玩法：

触发：收到"玫瑰"礼物（gift_id=5655）
效果：
1. 设置通缉等级为5星
2. 设置所有NPC（行人/路人关系组）对玩家的关系为Hate
3. 给玩家SMG（冲锋枪）并设置1000发子弹
4. 给玩家满护甲
5. 可能给玩家一些额外血量加成或无敌几秒
6. 设置一个定时器，持续X秒（比如120秒），存活即为胜利

关于"我认为需要进行的操作"：
- 满护甲
- 清除当前通缉星再重新设置（避免叠加问题）
- 可能给玩家一把好用的冲锋枪，比如SMG或CombatPDW
- 可以给一个短暂的初始无敌时间来准备

现在让我整理 DESIGN.md。根据模板v3的要求，我需要明确：
- 玩法目标
- 状态机
- 触发映射
- 游戏效果
- 能力需求
- 礼物需求
- 冷却、限流
- 死亡处理
- 降级方案
- 验收标准

关于"所有玩家攻击主角"——在GTA5单人模式中，我理解这应该是指让NPC行人也参与攻击（而不仅仅是警察）。实现方式是通过RelationshipGroup让平民关系组对玩家敌对。

对于玩家死亡，应该标记为失败，并清理状态。

冷却方面：由于是一次性礼物触发，应该设置合理的冷却时间，比如300秒（5分钟），防止重复触发。

实体上限：这个玩法不涉及生成实体，主要是状态修改。

现在让我输出 DESIGN.md。

等一下，关于WeaponCollection.Give方法，我需要一个fallback。如果Give不存在，可以用其他方式。但实际上在SHVDN v3中，`Ped.Weapons.Give(WeaponHash, int, bool, bool)` 是标准方法。我已经确认了Weapons属性存在，而WeaponCollection类通常包含Give方法。我可以标记为"基于已知WeaponCollection类型推断"，如果没有则fallback到使用Native函数。

还有关于"所有玩家攻击主角"，在GTA5单人模式中不存在其他玩家，所以我将其解释为"使周围NPC/行人对玩家产生敌意"。这通过设置RelationshipGroup来实现。

关于Relationship枚举：应该有`GTA.Relationship.Hate`。这个在SHVDN中通常是存在的。我已经确认了`SetRelationshipBetweenGroups`方法签名中使用了`GTA.Relationship relationship`参数，所以Relationship枚举一定存在。

现在可以输出 DESIGN.md 了。
</think>已收集足够的 API 证据。现在汇总并输出 DESIGN.md。

# DESIGN.md

## 1. 需求摘要

**玩法名称**：玫瑰生存（Rose Survival）

**核心目标**：观众赠送"玫瑰"礼物后，主角进入生存模式——通缉等级拉满至5星，周围所有 NPC 行人对主角产生敌意，主角获得一把冲锋枪（SMG）及 1000 发子弹，在围剿中尽力存活。

**胜利条件**：存活指定时长（默认 120 秒），计时结束后通缉等级清零、敌对关系恢复、标记胜利。
**失败条件**：主角在计时期间死亡。
**时长**：120 秒（可配置）。
**正向互动**：观众赠送"玫瑰"触发玩法启动。
**负向互动**：所有 NPC 行人攻击主角 + 5 星警察围剿。

---

## 2. change_summary 与 preserved_behaviors

- **change_summary**：首次生成，无历史版本。
- **preserved_behaviors**：首次生成/无。

---

## 3. 玩法流程

```
[观众赠送"玫瑰"礼物]
        │
        ▼
  ┌─────────────────────────┐
  │ 检查冷却 & 状态         │
  │ (不在冷却 & 未激活)     │
  └────────┬────────────────┘
           │ 通过
           ▼
  ┌─────────────────────────┐
  │ 1. 记录激活状态         │
  │ 2. 进入冷却             │
  │ 3. 保存玩家当前状态快照 │
  │    (武器/护甲/通缉)     │
  └────────┬────────────────┘
           ▼
  ┌─────────────────────────┐
  │ 4. 设置通缉等级 = 5     │
  │ 5. 设置平民关系组       │
  │    对玩家 = Hate        │
  │ 6. 给予 SMG + 1000 弹药 │
  │ 7. 补满护甲             │
  │ 8. 启动 120s 生存计时   │
  └────────┬────────────────┘
           ▼
  ┌─────────────────────────┐
  │      生存阶段           │
  │  玩家抵御警察+NPC围攻   │
  │  每 Tick:               │
  │    - 检查是否存活       │
  │    - 检查计时是否到期   │
  └────────┬────────────────┘
           │
   ┌───────┴───────┐
   ▼               ▼
[玩家死亡]    [计时到期]
   │               │
   ▼               ▼
[失败]         [胜利]
   │               │
   └───────┬───────┘
           ▼
  ┌─────────────────────────┐
  │ 清理:                   │
  │ - 恢复平民关系为 Neutral│
  │ - 清除通缉等级 (仅胜利) │
  │ - 清除激活状态          │
  │ - 记录本次结果          │
  └─────────────────────────┘
```

---

## 4. 状态机

| 状态 | 描述 | 允许的转换 |
|------|------|-----------|
| `Idle` | 等待触发，无活动玩法 | → `Active`（收到玫瑰礼物且冷却完毕） |
| `Active` | 生存进行中，计时器运行，NPC 敌对 | → `Victory`（计时到期且存活）、→ `Failure`（玩家死亡） |
| `Victory` | 存活成功 | → `Idle`（清理后自动） |
| `Failure` | 玩家死亡 | → `Idle`（清理后自动） |

- `Active` 状态下忽略新的玫瑰礼物（冷却保护 + 状态检查双重保险）。
- `Victory`/`Failure` 为瞬时状态，清理后立即回到 `Idle`。

---

## 5. LIVE Studio 触发映射

| trigger | event_type | matcher | gift_strategy | cooldown | gameplay_behavior | death_policy |
|---------|-----------|---------|--------------|----------|-------------------|-------------|
| 收到玫瑰礼物 | GiftEvent | `"5655"` (Rose) | `once` | 300s（全局，与状态无关） | 进入生存模式：5星通缉 + NPC敌对 + SMG+弹药 + 满护甲 + 120s计时 | 死亡消耗冷却（已进入冷却），标记失败并清理 |

- **matcher 说明**：精确匹配 gift_id `"5655"`，对应礼物名 `Rose`（玫瑰），diamondCount=1。
- **strategy=once**：该礼物为一次性效果，由模板固定 `TriggerGiftOnce` 处理，自动去重和 1200ms fallback 兜底。

---

## 6. 游戏效果

### 6.1 通缉等级

- 设置 `Game.Player.WantedLevel = 5`。
- 这会立刻触发 GTA 原生 5 星警察响应（直升机、NOOSE、路障等）。

### 6.2 NPC 行人敌对

- 获取平民关系组（`PedGroup.Civillian` 或通过 hash `"CIVMALE"`/`"CIVFEMALE"` 群组对应的 RelationshipGroup）。
- 调用 `RelationshipGroup.SetRelationshipBetweenGroups(civGroup, playerGroup, Relationship.Hate, true)`，使平民与玩家互相敌对。
- 效果：路过的行人会攻击/逃跑并可能报警，加剧生存压力。

### 6.3 武器与弹药

- 通过 `Game.Player.Character.Weapons.Give(WeaponHash.SMG, 1000, true, true)` 给予冲锋枪。
- 参数：SMG 武器哈希、1000 发弹药、立即装备、弹药已装填。
- 如 `WeaponCollection.Give` 未确认存在，fallback 使用 `Game.Player.Character.Weapons[WeaponHash.SMG].Ammo = 1000` 分步设置。

### 6.4 护甲

- 设置 `Game.Player.Character.Armor = Game.Player.Character.MaxArmor`，补满护甲。

### 6.5 生存计时

- 在主线程 Tick 中累积 deltaTime。
- 到达 120 秒且玩家存活 → 标记 Victory → 清理。

### 6.6 胜利清理

- 通缉等级清零：`Game.Player.WantedLevel = 0`。
- 平民关系恢复：`SetRelationshipBetweenGroups(civGroup, playerGroup, Relationship.Neutral, true)`。
- 状态回到 `Idle`。

### 6.7 失败清理

- 平民关系恢复（同上）。
- 通缉等级保留（死亡后 GTA 自然处理，也可显式清零）。
- 状态回到 `Idle`。

---

## 7. capability_requirements

| capability | behavior | priority | status | verified_evidence | fallback |
|-----------|----------|----------|--------|-------------------|----------|
| `Game.Player.WantedLevel` (set) | 设置为 5 触发 5 星通缉 | P0 | **verified** | `GTA.Player.WantedLevel { get; set; }` (System.Int32)，已确认 | 无 |
| `Game.Player.Character.Weapons` | 获取 WeaponCollection 给武器 | P0 | **partial** | `GTA.Ped.Weapons → GTA.WeaponCollection` 已确认；`Give(WeaponHash, int, bool, bool)` 为 SHVDN v3 标准方法但本次查询未显式列出 | 分步: `Weapons[hash].Ammo = 1000`，若武器不存在则先通过 WeaponAsset 创建 |
| `WeaponHash.SMG` | SMG 武器哈希 | P0 | **partial** | `GTA.PickupType.WeaponSMG` 枚举值已确认，推断 `GTA.WeaponHash.SMG` 存在 | 使用 `(WeaponHash)0x2DC677A0`（SMG 的已知 Joaat hash）或 `GTA.WeaponHash.MicroSMG` |
| `GTA.RelationshipGroup.SetRelationshipBetweenGroups` | 设置平民对玩家 Hate | P1 | **verified** | `System.Void SetRelationshipBetweenGroups(GTA.RelationshipGroup targetGroup, GTA.Relationship relationship, System.Boolean bidirectionally = )` 已确认 | 无 |
| `GTA.Relationship.Hate` | 敌对关系枚举值 | P1 | **inferred** | `GTA.Relationship` 作为参数类型已确认，Hate 为 GTA 标准关系枚举值 | 直接使用整数 5（GTA 原生 Relationship 枚举 Hate=5） |
| `Game.Player.Character.Armor` / `MaxArmor` | 补满护甲 | P2 | **inferred** | SHVDN v3 标准 Ped 属性 | 跳过护甲设置（非致命） |
| `Game.Player.Character.IsAlive` | 检测玩家死亡 | P0 | **inferred** | SHVDN v3 标准 Ped 属性 | 使用 `!Game.Player.Character.IsDead` |

---

## 8. gift_requirements

| 字段 | 值 |
|------|-----|
| original_query | 玫瑰 |
| keywords | rose, flower, 玫瑰 |
| status | **matched** |
| confirmed_name | Rose |
| gift_id | `"5655"` |
| diamondCount | 1 |
| candidates | Rose (5655, diamondCount=1, exact_name), Rose Hand (16896, 199), Rose Bear (16300, 214), Rosie the Rose Bean (12210, 399), Rose Soundwave (17983, 499) |
| fallback | 无（精确匹配已确认） |

---

## 9. 冷却、限流与有界状态

| 机制 | 策略 |
|------|------|
| **触发冷却** | 300 秒全局冷却（从 `Active` 状态开始时计时）。冷却期间忽略所有玫瑰礼物。死亡也消耗冷却（冷却已开始）。 |
| **状态互斥** | `Active` 状态下拒绝新触发，即使冷却意外通过也由状态检查兜底。 |
| **实体上限** | 本玩法不主动生成自定义实体（车辆/ Ped/Prop），仅修改现有游戏状态。无实体管理需求。 |
| **计时器上限** | 单一 120 秒计时器，`Active` 状态结束时释放。 |
| **关系恢复** | 仅在 `Victory`/`Failure` 清理时恢复一次，防止遗漏。 |

---

## 10. 玩家死亡、失败与 OnGameplayAborted 清理

| 场景 | 行为 |
|------|------|
| **玩家在 Active 状态死亡** | 标记 `Failure`，恢复平民关系为 Neutral，状态回 `Idle`。死亡时通缉等级由 GTA 自然处理（重生后清零），也可显式清零。 |
| **计时到期存活** | 标记 `Victory`，恢复平民关系，通缉清零。 |
| **OnGameplayAborted** | 当玩法被强制终止（如脚本重载）时，执行安全清理：恢复平民关系为 Neutral、清除通缉等级、重置状态为 `Idle`。不对固定 LIVE Studio 连接或调度器做任何操作。 |

---

## 11. 降级方案

| 场景 | 降级措施 |
|------|---------|
| `WeaponCollection.Give` 不存在 | 分步操作：先检查武器是否拥有，无则通过 `WeaponAsset` 创建，然后设置 `Ammo = 1000` |
| `WeaponHash.SMG` 不存在 | 降级使用 `WeaponHash.MicroSMG` 或硬编码 hash `0x2DC677A0` |
| `Relationship.Hate` 枚举不存在 | 使用整数 `5`（GTA 原生 Hate 值） |
| `Armor`/`MaxArmor` 不可用 | 跳过护甲设置，不影响核心玩法 |
| 平民关系组获取失败 | 使用 `Game.GenerateHash("CIVMALE")` 创建 RelationshipGroup 兜底 |
| 120s 内玩家断线/直播间关闭 | 模板 v3 固定运行时维持连接；本玩法不清除运行时状态，仅关系恢复和状态重置由 `OnGameplayAborted` 保障 |

---

## 12. 验收标准

1. **触发**：观众在直播间赠送"玫瑰"（gift_id=5655），本地 GTA 主角立即进入生存模式。
2. **通缉**：`Game.Player.WantedLevel` 变为 5，游戏中出现 5 星警察响应（直升机、NOOSE 等）。
3. **NPC 敌对**：街道上行人对主角表现出敌意（攻击、报警、逃跑后报警）。
4. **武器**：主角获得 SMG 冲锋枪，弹药为 1000 发，且自动切换为该武器。
5. **护甲**：主角护甲补满。
6. **冷却**：首次触发后 300 秒内再次赠送玫瑰不产生效果。
7. **胜利**：存活 120 秒后通缉清零、平民关系恢复、可正常游戏。
8. **失败**：在 120 秒内死亡，平民关系恢复。
9. **重入**：冷却结束后可再次触发。
10. **清理**：`OnGameplayAborted` 后平民关系恢复、通缉清除、状态重置，不影响后续游戏。