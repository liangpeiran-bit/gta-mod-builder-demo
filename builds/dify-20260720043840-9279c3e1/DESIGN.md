<think>
我们来做需求分析。MODE: NEW，所以是首次生成。需要：收到玫瑰后，设置通缉3星，周围50米路人攻击主角，给SMG冲锋枪和500发弹药，冷却30秒。必须使用真实LIVE Studio WebSocket事件，完整Aborted清理。

首先需要查询礼物：玫瑰。然后查询SHVDN API能力：设置通缉等级、让路人攻击主角、给武器弹药、计算距离等。

让我们先同时查询礼物和SHVDN API。

礼物查询：query = "玫瑰"，keywords可以用 "rose, flower"。

SHVDN查询第一批：核心能力：设置通缉等级、让ped攻击玩家、给武器弹药。我们需要查：
- 设置通缉等级: Game.Player.WantedLevel 或类似的
- 让周围路人攻击主角: 可能需要设置ped关系，比如 Ped.RelationshipGroup, World.SetRelationshipBetweenGroups, 或者使用 Native functions
- 给武器: WeaponAsset, Game.Player.Character.Weapons.Give 等
- 获取周围peds: World.GetNearbyPeds 或类似

第一批查询：设置通缉等级、武器、附近peds。第二批：关系组和攻击行为。

让我们先发第一批查询和礼物查询。

对于SHVDN，我们可以查询："Game.Player.WantedLevel set wanted level, World.GetNearbyPeds, WeaponAsset give weapon SMG, Ped weapons"

礼物查询："玫瑰"。

一起发起。
</think>好的，这是首次生成，需要同时查询礼物和核心 GTA5 能力。先发起工具调用。
<think>
好的，礼物查询结果：matched，精确匹配到 "Rose"，gift_id=5655，diamondCount=1。

SHVDN API 结果：status=partial，但返回了很多有用的API。我们现在知道了：
- `Game.Player.WantedLevel` - 可读写，用于设置通缉等级
- `World.GetNearbyPeds` - 获取附近peds
- `Ped.RelationshipGroup` - 关系组
- `WeaponAsset` - 武器资源
- `RelationshipGroup` struct

但是我们还需要：
- 如何给武器和弹药：`Ped.Weapons.Give()` 或类似
- 如何设置关系组之间的仇恨：`World.SetRelationshipBetweenGroups` 或类似
- 如何让ped攻击玩家：可能需要 `Ped.Task.FightAgainst` 或类似

还需要 WeaponHash 枚举来找 SMG。

第二批查询：这些独立的能力。

查询：`World.SetRelationshipBetweenGroups, Ped.Weapons.Give, WeaponHash.SMG, Ped.Task.FightAgainst, RelationshipGroup.Hate, RelationshipGroup.Player`

让我发起第二批查询。
</think>礼物已精确匹配到 `Rose`（gift_id=5655，1钻石）。SHVDN 第一批已确认核心 API，现在查询剩余的独立能力：武器给予、关系组仇恨、Ped 攻击任务。
<think>
第二批查询也返回了结果。现在汇总所有已确认的API：

第一批：
- `Game.Player` (GTA.Game.Player) - static property
- `GTA.Player.WantedLevel` - int, get/set
- `World.GetNearbyPeds(Vector3 position, float radius, Model[] models)` 或 `World.GetNearbyPeds(Ped ped, float radius, Model[] models)`
- `Ped.RelationshipGroup` - get/set
- `GTA.RelationshipGroup` struct, constructors with int/uint hash
- `GTA.WeaponAsset` struct, constructors with WeaponHash/int/uint

第二批：
- `GTA.TaskInvoker.FightAgainst(Ped target)` 或 `FightAgainst(Ped target, int duration)`
- `GTA.Weapon.Hash` 属性
- `GTA.RelationshipGroup.SetRelationshipBetweenGroups(RelationshipGroup targetGroup, Relationship relationship, bool bidirectionally = )`
- `Ped.Task` 属性返回 TaskInvoker
- `Ped.Weapons` 属性返回 WeaponCollection
- `GTA.WeaponHash` enum
- `GTA.Relationship.Hate` enum value
- `GTA.Native.Hash.SET_RELATIONSHIP_BETWEEN_GROUPS` native hash

但还需要 WeaponCollection.Give 方法。这个没有直接返回。可能需要进一步查询，但我们已经有2次查询了。WeaponCollection 应该有 Give 方法，这是SHVDN中常见的。我可以标记为 needs_query。

还有，SMG 对应的 WeaponHash 值是什么？WeaponHash 是一个 enum，应该有 SMG 成员。但我们需要确认具体的 enum 值名称。可能是 `WeaponHash.SMG`。这个我们标记一下。

另外，关于 RelationshipGroup：我们需要为路人创建一个自定义的 RelationshipGroup，然后设置该组与玩家组的关系为 Hate。

还需要了解：
- `Game.Player.Character` 获取玩家 Ped
- `Game.Player.Character.RelationshipGroup` 获取玩家关系组

还有清理时需要恢复关系。

现在两个工具都已完成调用（SHVDN 2次，礼物1次），可以输出 DESIGN.md。

让我整理所有信息：

礼物：
- 名称：Rose，gift_id=5655，diamondCount=1
- status: matched (exact)

能力清单：

已确认：
1. `Game.Player` → Player 对象
2. `Player.WantedLevel` → set 为 3
3. `World.GetNearbyPeds(Ped ped, float radius, Model[] models)` → 获取主角周围50米的 peds
4. `Ped.RelationshipGroup` → 获取/设置关系组
5. `RelationshipGroup.SetRelationshipBetweenGroups(RelationshipGroup targetGroup, Relationship relationship, bool bidirectionally)` → 设置仇恨
6. `GTA.Relationship.Hate` → 仇恨枚举
7. `Ped.Task.FightAgainst(Ped target)` → 让路人攻击主角
8. `Ped.Weapons` → WeaponCollection
9. `WeaponHash` enum → SMG 的 hash
10. `WeaponAsset` → 用于给予武器

需要标记 needs_query 的：
- `WeaponCollection.Give(WeaponHash, int ammo, bool equipNow, bool isAmmoLoaded)` - 未直接返回但属于 SHVDN 标准 API，标记 needs_query，但给出合理的 fallback
- `WeaponHash.SMG` 具体的 enum 值名称

还有 LIVE Studio WebSocket 事件。这是模板提供的，不需要查询。

现在设计整体玩法：

状态机：
- Idle: 等待礼物触发
- Active: 事件进行中（通缉3星，路人攻击）
- Cooldown: 冷却30秒
- Aborted: 清理

触发：
- LIVE Studio 礼物事件 "Rose" (gift_id=5655)

效果：
1. 设置 WantedLevel = 3
2. 给予 SMG + 500发弹药
3. 获取周围50米所有路人 Ped
4. 创建自定义 RelationshipGroup，设置与玩家组为 Hate
5. 每个路人 Task.FightAgainst(玩家)
6. 30秒冷却

清理 (Aborted/事件结束):
- 恢复 RelationshipGroup
- 清理任何残留

但我们还需要考虑：事件何时结束？用户没有指定胜利条件。所以事件触发后一直持续，直到玩家死亡（失败条件）或手动终止。冷却只在触发之间生效，避免刷礼物滥用。

另外，清理时是否需要重置 WantedLevel？一般来说不需要，但 Aborted 时可以考虑。用户要求完整 Aborted 清理。

关于持续攻击：我们需要考虑每帧/定期刷新周围的 peds 吗？用户说"周围50米内的所有路人开始攻击主角"，可能只需要触发一次（当时在范围内的）。但为了更有趣，也许应该持续让进入50米范围的路人也攻击？不过这比较复杂。先按触发时一次性让范围内路人攻击来处理。

也可能需要持续检测，因为路人会死，新的路人会刷新。但这是玩法设计的复杂度。我们先按"触发时让当前50米内路人攻击"来实现，可以加上一个简单的持续效果（比如在 Active 状态中每几秒刷新一次）。

实际上用户说"周围50米内的所有路人开始攻击主角"，暗示是触发时的一次性行为。我们可以设计为：在 Active 状态下，每隔几秒重新扫描并让新的路人攻击。但为简单起见，先只做触发时的一次性。

现在开始写 DESIGN.md。

关于 WeapCollection.Give：虽然工具没直接返回，但这是 SHVDN 非常基础的 API。我可以标记 needs_query，fallback 使用 `Game.Player.Character.Weapons.Give(WeaponHash.SMG, 500, true, true)`。

关于 WeaponHash.SMG：这也是标准 enum。标记 needs_query 但 fallback 使用 `WeaponHash.SMG`。

还有一些细节：
- Model[] 参数在 GetNearbyPeds 中可以用来过滤，传 null 或空数组表示不过滤
- 自定义 RelationshipGroup 的 hash 可以用随机数或固定值
- 清理时要把自定义 RelationshipGroup 的关系重置回 Neutral/Respect

现在输出 DESIGN.md。
</think>已完成全部工具调用（SHVDN 2次，礼物1次）。现在汇总所有证据，输出 DESIGN.md。

# DESIGN.md

## 需求摘要
- **玩法名称**：玫瑰通缉街头混战 (Rose Wanted Street Brawl)
- **核心目标**：观众送出「玫瑰」礼物后，立即将主角通缉等级设为 3 星，给予 SMG 冲锋枪 + 500 发弹药，并驱使周围 50 米内所有路人攻击主角，制造街头混战。
- **胜利条件**：无（事件持续至自然消退或脚本终止）
- **失败条件**：主角死亡
- **时长/节奏**：单次事件无固定时长；同一礼物触发冷却 30 秒
- **正向互动**：无
- **负向互动**：周围 50 米路人攻击主角；主角被 3 星通缉
- **礼物触发**：Rose (gift_id=5655)

## 玩法流程
1. 脚本启动后进入 **Idle** 状态，监听 LIVE Studio WebSocket 礼物事件。
2. 收到 `Rose`（gift_id=5655，1 钻石）→ 检查冷却是否结束。
3. 若冷却未结束 → 忽略本次触发。
4. 若冷却已结束 → 进入 **Active** 状态：
   - 设置 `Game.Player.WantedLevel = 3`
   - 给予玩家 `WeaponHash.SMG` + 500 发弹药
   - 以主角位置为圆心、50 米半径获取所有路人 Ped
   - 创建临时敌对 RelationshipGroup，设为与玩家组 `Hate`
   - 将范围内所有路人归入该敌对组，并下达 `FightAgainst(玩家)` 任务
5. 进入 **Cooldown** 状态，启动 30 秒冷却计时器。
6. 冷却结束后回到 **Idle** 状态。
7. 收到 `Aborted` 事件 → 执行完整清理，回到 Idle。

## 状态机

```
           ┌──────────────────────────────────┐
           │                                  │
           ▼                                  │
        ┌──────┐   Rose礼物 & 冷却OK    ┌──────────┐
        │ Idle │ ──────────────────────▶│  Active  │
        └──────┘                        └──────────┘
           ▲                                  │
           │                         效果执行完成
           │                                  ▼
           │                            ┌──────────┐
           │         冷却30秒结束        │ Cooldown │
           └────────────────────────────└──────────┘
           
           任意状态 ── Aborted ──▶ Cleanup ──▶ Idle
```

## 触发条件
| 触发源 | 条件 | 行为 |
|--------|------|------|
| LIVE Studio 礼物事件 | 礼物名称匹配 `Rose`（gift_id=5655），且当前不在 Cooldown | 进入 Active |
| LIVE Studio 礼物事件 | 礼物名称匹配 `Rose`，但在 Cooldown 内 | 忽略，记录日志 |
| LIVE Studio Aborted | 任意时刻 | 执行完整清理，回到 Idle |

## 效果表
| 步骤 | 效果 | 实现方式 | 备注 |
|------|------|----------|------|
| 1 | 通缉 3 星 | `Game.Player.WantedLevel = 3` | verified |
| 2 | 给予 SMG + 500 弹药 | `Ped.Weapons.Give(WeaponHash.SMG, 500, true, true)` | WeaponHash.SMG needs_query; WeaponCollection.Give needs_query |
| 3 | 获取 50m 内路人 | `World.GetNearbyPeds(playerPed, 50f, null)` | verified |
| 4 | 创建敌对关系组 | `new RelationshipGroup(hash)` → `SetRelationshipBetweenGroups(playerGroup, Relationship.Hate, true)` | verified |
| 5 | 路人攻击主角 | 每个路人 `ped.RelationshipGroup = enemyGroup` → `ped.Task.FightAgainst(playerPed)` | verified |
| 6 | 30 秒冷却 | `DateTime` / `Game.GameTime` 计时 | 标准 .NET |

## capability_requirements
| capability | behavior | priority | status | verified_evidence | fallback |
|------------|----------|----------|--------|-------------------|----------|
| Game.Player | 获取本地玩家对象 | critical | verified | `GTA.Game.Player` static property (score 88) | — |
| Player.WantedLevel | 设置通缉等级为 3 | critical | verified | `System.Int32 WantedLevel { get; set; }` (score 88) | — |
| World.GetNearbyPeds | 获取主角 50m 半径内所有路人 | critical | verified | `static GTA.Ped[] GetNearbyPeds(GTA.Ped ped, System.Single radius, GTA.Model[] models)` (score 88) | — |
| Ped.RelationshipGroup | 读取玩家关系组 / 设置路人关系组 | critical | verified | `GTA.RelationshipGroup RelationshipGroup { get; set; }` (score 88) | — |
| RelationshipGroup.SetRelationshipBetweenGroups | 设置两组为 Hate | critical | verified | `System.Void SetRelationshipBetweenGroups(GTA.RelationshipGroup targetGroup, GTA.Relationship relationship, System.Boolean bidirectionally = )` (score 88) | — |
| Relationship.Hate | 仇恨关系枚举值 | critical | verified | `GTA.Relationship.Hate` enum literal (score 88) | — |
| Ped.Task.FightAgainst | 让路人攻击指定目标 | critical | verified | `System.Void FightAgainst(GTA.Ped target)` (score 88) | `FightAgainst(target, -1)` 也可 |
| WeaponHash 枚举 | SMG 武器哈希 | critical | needs_query | `GTA.WeaponHash` enum 存在 (score 88)，具体成员名未确认 | 使用 `WeaponHash.SMG`；若编译失败则尝试 `(WeaponHash)0x2` (hash: `WEAPON_SMG`) |
| WeaponCollection.Give | 给予武器和弹药 | critical | needs_query | `GTA.WeaponCollection` 类型存在，Give 方法为标准 SHVDN API | `ped.Weapons.Give(WeaponHash.SMG, 500, true, true)` |
| Ped.Weapons | 访问玩家武器集合 | critical | verified | `GTA.WeaponCollection Weapons { get; }` (score 88) | — |
| LIVE Studio WebSocket 事件 | 接收礼物和 Aborted | critical | verified | 模板提供 `OnGiftReceived` / `OnAborted` | — |

## LIVE Studio 事件与礼物映射
### gift_requirements
| field | value |
|-------|-------|
| original_query | 玫瑰 |
| keywords | rose, flower |
| status | **matched** |
| confirmed_name | Rose |
| gift_id | 5655 |
| diamondCount | 1 |
| candidates | Rose (score 100, exact_name), Rose Hand (score 90), Rose Bear (score 90), Rosie the Rose Bean (score 90), Rose Soundwave (score 90) |
| fallback | — |

### 事件绑定
| LIVE Studio 事件 | 处理逻辑 |
|------------------|----------|
| `OnGiftReceived(giftId, giftName, diamondCount, …)` | 若 `giftId == 5655` 且冷却结束 → 触发 Active；否则忽略 |
| `OnAborted()` | 执行 `Cleanup()` → 回到 Idle |

## 计时与冷却
- **冷却时长**：30 秒（从效果执行完毕起算）
- **计时方式**：`DateTime.UtcNow` 记录上次触发时间，与当前时间比较
- **冷却期间行为**：相同礼物触发被静默忽略
- **Aborted 期间**：冷却计时器重置，清理后直接回到 Idle

## 失败与清理
### 失败条件
- 主角死亡（`Game.Player.Character.IsDead`）→ 自动进入 Cleanup

### Cleanup 流程（Aborted 或主角死亡触发）
1. 将自定义敌对 RelationshipGroup 与玩家组的关系重置为 `Relationship.Neutral`（或 `Respect`）
2. 清除对范围内路人的持续任务引用（如有）
3. 重置冷却计时器
4. 状态回到 Idle
5. **不重置** WantedLevel（通缉由游戏自然消退或玩家自行处理）

### 脚本中止清理
- `Script.Aborted` 事件中执行与 Cleanup 相同的逻辑
- 确保自定义 RelationshipGroup 不会残留影响游戏世界

## 降级方案
1. **WeaponHash.SMG 编译失败** → 尝试 `(WeaponHash)0x73647f98`（即 `WEAPON_SMG` 的已知 hash）；若仍失败则尝试 `WeaponHash.MicroSMG`
2. **WeaponCollection.Give 不可用** → 使用 `Function.Call(Hash.GIVE_WEAPON_TO_PED, playerPed, weaponHash, 500, false, true)` Native 调用
3. **RelationshipGroup 设置未生效** → 改用 Native `Function.Call(Hash.SET_RELATIONSHIP_BETWEEN_GROUPS, …)` 作为 fallback
4. **FightAgainst 未生效** → 改用 `ped.Task.ClearAll()` 后 `ped.Task.Combat(playerPed)`
5. **GetNearbyPeds 返回空** → 仅执行通缉 + 武器给予，记录警告日志

## 验收标准
1. ✅ 观众送出 `Rose`（gift_id=5655）时，主角通缉变为 3 星
2. ✅ 主角获得 SMG 冲锋枪，弹药 ≥ 500 发
3. ✅ 主角周围 50 米内路人立即转为敌对并主动攻击主角
4. ✅ 30 秒内重复送 `Rose` 不触发重复效果
5. ✅ 冷却结束后可再次触发
6. ✅ Aborted 事件后关系组恢复，脚本回到 Idle
7. ✅ 主角死亡时触发清理，不残留自定义关系组
8. ✅ 脚本卸载（Script.Aborted）后游戏世界恢复干净