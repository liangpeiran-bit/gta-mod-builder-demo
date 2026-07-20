<think>
用户要求基于上一版 Mod.cs 进行增量修改：
1. 通缉等级从3星改为4星
2. 冷却时间从30秒改为15秒
3. 其他保持不变：Rose gift_id 5655、50米路人攻击、SMG与500发弹药、真实 LIVE Studio WebSocket serviceSignalSub / IM_MESSAGE_TRANSPORT、重连和完整 Aborted 清理

这是 REVISE 模式。上一版 DESIGN.md 已经存在，我需要基于它修改这两个参数，然后输出修订版。

变化：
- `Game.Player.WantedLevel = 3` → `Game.Player.WantedLevel = 4`
- 冷却 30 秒 → 15 秒
- DESIGN.md 中相应位置也需要更新

不需要调用任何工具，因为：
- 礼物映射没有变化（Rose gift_id=5655，已确认）
- 所有 SHVDN API 能力已经在上一版中确认，不需要重新查询
- 只是参数变化

我需要输出修订版 DESIGN.md，包含 change_summary 和 preserved_behaviors。
</think># DESIGN.md

## 需求摘要
- **玩法名称**：玫瑰通缉街头混战 (Rose Wanted Street Brawl)
- **核心目标**：观众送出「玫瑰」礼物后，立即将主角通缉等级设为 4 星，给予 SMG 冲锋枪 + 500 发弹药，并驱使周围 50 米内所有路人攻击主角，制造街头混战。
- **胜利条件**：无（事件持续至自然消退或脚本终止）
- **失败条件**：主角死亡
- **时长/节奏**：单次事件无固定时长；同一礼物触发冷却 15 秒
- **正向互动**：无
- **负向互动**：周围 50 米路人攻击主角；主角被 4 星通缉
- **礼物触发**：Rose (gift_id=5655)

## change_summary
| 变更项 | 上一版 | 本轮值 | 原因 |
|--------|--------|--------|------|
| 通缉等级 | 3 星 | **4 星** | 用户要求提高难度 |
| 冷却时间 | 30 秒 | **15 秒** | 用户要求缩短冷却 |

## preserved_behaviors
- Rose gift_id=5655 礼物映射不变
- 50 米半径路人攻击不变
- SMG + 500 发弹药不变
- 真实 LIVE Studio WebSocket `serviceSignalSub` / `IM_MESSAGE_TRANSPORT` 不变
- WebSocket 断线重连机制不变
- 完整 Aborted 清理（关系组恢复、冷却重置）不变
- 主角死亡触发清理不变
- 脚本卸载 (Script.Aborted) 清理不变
- 自定义敌对 RelationshipGroup 创建与销毁不变
- 冷却期间相同礼物静默忽略不变
- 所有未提及行为沿用上一版完整 Mod.cs

## 玩法流程
1. 脚本启动后进入 **Idle** 状态，监听 LIVE Studio WebSocket 礼物事件。
2. 收到 `Rose`（gift_id=5655，1 钻石）→ 检查冷却是否结束。
3. 若冷却未结束 → 忽略本次触发。
4. 若冷却已结束 → 进入 **Active** 状态：
   - 设置 `Game.Player.WantedLevel = 4`
   - 给予玩家 `WeaponHash.SMG` + 500 发弹药
   - 以主角位置为圆心、50 米半径获取所有路人 Ped
   - 创建临时敌对 RelationshipGroup，设为与玩家组 `Hate`
   - 将范围内所有路人归入该敌对组，并下达 `FightAgainst(玩家)` 任务
5. 进入 **Cooldown** 状态，启动 15 秒冷却计时器。
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
           │         冷却15秒结束        │ Cooldown │
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
| 1 | 通缉 4 星 | `Game.Player.WantedLevel = 4` | verified |
| 2 | 给予 SMG + 500 弹药 | `Ped.Weapons.Give(WeaponHash.SMG, 500, true, true)` | WeaponHash.SMG needs_query; WeaponCollection.Give needs_query |
| 3 | 获取 50m 内路人 | `World.GetNearbyPeds(playerPed, 50f, null)` | verified |
| 4 | 创建敌对关系组 | `new RelationshipGroup(hash)` → `SetRelationshipBetweenGroups(playerGroup, Relationship.Hate, true)` | verified |
| 5 | 路人攻击主角 | 每个路人 `ped.RelationshipGroup = enemyGroup` → `ped.Task.FightAgainst(playerPed)` | verified |
| 6 | 15 秒冷却 | `DateTime.UtcNow` 计时 | 标准 .NET |

## capability_requirements
| capability | behavior | priority | status | verified_evidence | fallback |
|------------|----------|----------|--------|-------------------|----------|
| Game.Player | 获取本地玩家对象 | critical | verified | `GTA.Game.Player` static property (score 88) | — |
| Player.WantedLevel | 设置通缉等级为 4 | critical | verified | `System.Int32 WantedLevel { get; set; }` (score 88) | — |
| World.GetNearbyPeds | 获取主角 50m 半径内所有路人 | critical | verified | `static GTA.Ped[] GetNearbyPeds(GTA.Ped ped, System.Single radius, GTA.Model[] models)` (score 88) | — |
| Ped.RelationshipGroup | 读取玩家关系组 / 设置路人关系组 | critical | verified | `GTA.RelationshipGroup RelationshipGroup { get; set; }` (score 88) | — |
| RelationshipGroup.SetRelationshipBetweenGroups | 设置两组为 Hate | critical | verified | `System.Void SetRelationshipBetweenGroups(GTA.RelationshipGroup targetGroup, GTA.Relationship relationship, System.Boolean bidirectionally = )` (score 88) | — |
| Relationship.Hate | 仇恨关系枚举值 | critical | verified | `GTA.Relationship.Hate` enum literal (score 88) | — |
| Ped.Task.FightAgainst | 让路人攻击指定目标 | critical | verified | `System.Void FightAgainst(GTA.Ped target)` (score 88) | `FightAgainst(target, -1)` 也可 |
| WeaponHash 枚举 | SMG 武器哈希 | critical | needs_query | `GTA.WeaponHash` enum 存在 (score 88)，具体成员名未确认 | 使用 `WeaponHash.SMG`；若编译失败则尝试 `(WeaponHash)0x73647f98` |
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
- **冷却时长**：15 秒（从效果执行完毕起算）
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
1. ✅ 观众送出 `Rose`（gift_id=5655）时，主角通缉变为 4 星
2. ✅ 主角获得 SMG 冲锋枪，弹药 ≥ 500 发
3. ✅ 主角周围 50 米内路人立即转为敌对并主动攻击主角
4. ✅ 15 秒内重复送 `Rose` 不触发重复效果
5. ✅ 冷却结束后可再次触发
6. ✅ Aborted 事件后关系组恢复，脚本回到 Idle
7. ✅ 主角死亡时触发清理，不残留自定义关系组
8. ✅ 脚本卸载（Script.Aborted）后游戏世界恢复干净