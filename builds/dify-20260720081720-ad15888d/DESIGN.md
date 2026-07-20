<think>
用户要求设计一个"玫瑰生存"玩法。核心需求：

1. 收到礼物"玫瑰"后触发
2. 通缉等级改为5星
3. 所有玩家攻击主角（这里指的应该是NPC警察，因为GTA5中通缉5星会让警察攻击玩家）
4. 给主角发一把冲锋枪（SMG）并设置1000发子弹

需要查询的内容：
1. 礼物"玫瑰" - 需要调用 query_live_gifts
2. SHVDN API - 需要查询：设置通缉等级、给武器、设置弹药、让NPC攻击玩家（可能需要Ped关系组）、生成敌对NPC等

让我先整理需要查询的SHVDN能力：
- 设置通缉等级：Game.Player.WantedLevel 或类似
- 给武器：Game.Player.Character.Weapons.Give 或类似
- 设置弹药：WeaponAsset, Ammo
- 让所有NPC攻击玩家：可能需要 RelationshipGroup 相关
- 冲锋枪：WeaponHash.SMG 或类似

还需要查询 VehicleHash 相关吗？不需要，这里没有载具需求。

礼物查询：query="玫瑰"，keywords="rose,flower"

SHVDN查询第一轮：核心能力 - 通缉等级、武器、弹药

让我同时发起礼物和SHVDN查询。

对于SHVDN查询，我需要查询：
- WantedLevel, set wanted level
- Give weapon, WeaponHash SMG
- Set ammo, weapon ammo
- 可能还需要 RelationshipGroup 让NPC攻击

第一轮查询核心能力：WantedLevel、GiveWeapon、WeaponHash、Ammo

第二轮查询：RelationshipGroup 如果需要的话。但先看第一轮结果。

实际上用户说"所有玩家攻击主角"，在GTA5中"玩家"可能指其他ped。但更合理的解释是：通缉5星后，警察会攻击主角。如果是"所有NPC攻击主角"，那可能需要设置relationship group。

让我先查询核心能力，同时查礼物。

SHVDN query 1: "Game.Player.WantedLevel set wanted level, Weapon.Give, WeaponHash.SMG, Ammo, Game.Player.Character.Weapons"

这可能需要更精确。让我分成两个查询。

第一轮 SHVDN: "WantedLevel, Game.Player.WantedLevel, set wanted stars, GiveWeapon, WeaponHash SMG, Ammo set"

第二轮（如果需要）: "RelationshipGroup, set relationship hostile, ped attack player"

同时查礼物。

让我开始。
</think>
<think>
好的，我得到了两个结果。让我分析：

**礼物结果**：
- 状态：matched
- 精确匹配：Rose (id: 5655, diamondCount: 1)
- 还有 Rose Hand, Rose Bear, Rosie the Rose Bean, Rose Soundwave

用户说的"玫瑰"，最佳匹配就是 "Rose" (id: 5655, diamondCount: 1)。

**SHVDN 第一轮**：
- Game.Player (property) - static GTA.Player Player { get; }
- Player.WantedLevel - System.Int32 WantedLevel { get; set; }
- Ped.Weapons - GTA.WeaponCollection Weapons { get; }
- WeaponHash enum - GTA.WeaponHash
- WeaponHash.SMG - enum value
- Weapon.Hash - GTA.WeaponHash Hash { get; }

但我还需要确认：
1. WeaponCollection 有什么方法？比如 Give()？
2. 如何设置弹药？

需要第二轮查询来获取 WeaponCollection.Give 和 ammo 相关。

让我查询："WeaponCollection Give, WeaponAsset, Ammo, set ammo count"
</think>
<think>
好的，现在我有足够的API信息了。让我总结关键发现：

**礼物**：
- Rose (id: 5655, diamondCount: 1) - exact match。用户说"玫瑰"，这就是 Rose。

**SHVDN API 确认的能力**：

1. **通缉等级**：`Game.Player.WantedLevel` - `System.Int32 WantedLevel { get; set; }` - 可以直接设置为5

2. **武器给予**：`Ped.Weapons.Give(GTA.WeaponHash weaponHash, System.Int32 ammoCount, System.Boolean equipNow, System.Boolean isAmmoLoaded)` - 返回 `GTA.Weapon`

3. **冲锋枪**：`GTA.WeaponHash.SMG` - enum value

4. **弹药**：`Give` 方法的第二个参数 `ammoCount` 直接设置弹药数量。所以可以用 `Game.Player.Character.Weapons.Give(WeaponHash.SMG, 1000, true, true)`

现在我还缺少什么？

关于"所有玩家攻击主角"：用户说"所有玩家攻击主角"。在GTA5单机模式中，这可能意味着：
- 设置通缉5星后，警察会攻击主角 - 这已经通过 WantedLevel 实现
- 或者让所有附近NPC变成敌对

用户原始表述是"所有玩家攻击主角"，但结合前文"通缉等级改为5星"，这很可能就是说通缉5星后警察攻击。但"所有玩家"这个表述有点奇怪，因为GTA5单机只有玩家自己。不过可以解释为"所有NPC（警察）攻击主角"。

通缉5星已经会让NPC警察攻击玩家，所以基本需求已满足。

不过，我还可以考虑是否需要额外让普通ped也攻击玩家。但用户说的是"所有玩家攻击主角"，可能只是指通缉系统让警察攻击。我就不额外添加复杂的relationship group逻辑了。

现在我需要构建 DESIGN.md。

关于"胜利条件"：用户没有指定，只说坚持存活。失败条件是主角死亡。

关于"通缉等级改为5星后所有玩家攻击主角"：GTA5中5星通缉自然会让警察和军队攻击玩家，这已经满足需求。

关于给冲锋枪和1000子弹：用 `Weapons.Give(WeaponHash.SMG, 1000, true, true)`。

现在构建完整的 DESIGN.md。

我需要遵循 LIVE_STUDIO_CONTRACT_V1 的所有约束。

让我设计：

**状态机**：
- IDLE: 等待礼物触发
- SURVIVAL: 生存模式激活中
- COOLDOWN: 冷却中（防止重复触发）

**触发条件**：收到礼物 Rose (id: 5655)，repeatEnd=true

**效果**：
1. 设置 WantedLevel = 5
2. 给予 SMG + 1000 子弹
3. 通知玩家

**冷却**：例如60秒。在生存模式中收到重复礼物时，根据 busy_state_policy 处理。

由于用户没有说需要冷却，但为了防止刷礼物滥用，我应该设置一个合理的冷却。默认60秒。

关于"所有玩家攻击主角"——用户可能还想让路人NPC也攻击。但在GTA5中，5星通缉已经让警察（和军队）攻击。如果用户确实想要所有ped攻击，那需要更复杂的实现。我先按照5星通缉来设计，因为这是用户明确说的。

另外，我考虑是否需要让普通ped也对玩家敌对。用户说"然后所有玩家攻击主角"，可能意味着除了警察外，路人也会攻击。但GTA5中通常只有通缉系统控制的执法NPC会攻击。如果用户想要"所有玩家"（实际上GTA5单机没有其他人类玩家），这可能指的是NPC。但是5星通缉本身就会让很多执法NPC出现并攻击。

我觉得按照字面理解：设置通缉5星 → 警察攻击。这就是"所有玩家攻击主角"在GTA5上下文中的实现。

好的，现在开始编写 DESIGN.md。

关于 capability_requirements 表：

| capability | behavior | priority | status | verified_evidence | fallback |
|---|---|---|---|---|---|
| Game.Player.WantedLevel | 设置通缉等级为5 | critical | verified | `System.Int32 WantedLevel { get; set; }` | - |
| Ped.Weapons.Give | 给予SMG+1000子弹 | critical | verified | `GTA.Weapon Give(GTA.WeaponHash weaponHash, System.Int32 ammoCount, System.Boolean equipNow, System.Boolean isAmmoLoaded)` | - |
| WeaponHash.SMG | 冲锋枪枚举值 | critical | verified | `GTA.WeaponHash.SMG` enum value | - |
| Game.Player | 获取本地玩家 | critical | verified | `static GTA.Player Player { get; }` | - |

关于 gift_requirements：

| field | value |
|---|---|
| original_query | 玫瑰 |
| keywords | rose,flower |
| status | matched |
| confirmed_name | Rose |
| gift_id | 5655 |
| diamondCount | 1 |
| candidates | Rose (5655, exact), Rose Hand (16896), Rose Bear (16300), Rosie the Rose Bean (12210), Rose Soundwave (17983) |
| fallback | - |

现在输出 DESIGN.md。

关于 LIVE Studio 映射：
- event_method: WebcastGiftMessage
- wire_field_paths: data.gift.id → 5655
- repeat_policy: 仅在 repeatEnd=true 时触发
- dedupe_policy: 按 msgId 去重
- cooldown: 60秒
- queue_cap: 3
- busy_state_policy: 生存模式运行中收到重复礼物 → 忽略（防止叠加效果）
- abort_cleanup: Script.Aborted 时清除通缉等级（恢复到触发前状态？或者清除通缉），移除武器

等等，清理时：
- 通缉等级：应该恢复到触发前的等级？还是清除？根据 LIVE_STUDIO_CONTRACT_V1 第12条："清理必须恢复或只撤销本 Mod 引入的状态，不得无条件删除玩家原有武器或把原通缉状态覆盖为固定值。"
- 所以需要在触发前记录原始通缉等级，清理时恢复。但 Script.Aborted 时可能无法恢复。我设计为：记录原始通缉等级，清理时尝试恢复。如果无法获取原始值（如脚本启动时就在生存模式），则设为0。
- 武器：不能删除玩家原有武器。但 Give 方法是添加武器，所以清理时不需要移除。但是否需要移除 SMG？根据第12条，不得无条件删除玩家原有武器。所以我们不应该在清理时移除SMG。可以在生存模式结束时保留武器。

好的，现在编写 DESIGN.md。

还有一个问题：用户说"所有玩家攻击主角"。在单机GTA5中，这可能意味着让所有NPC（不仅仅是警察）对玩家敌对。但是通缉5星已经让执法NPC攻击。

我需要决定是否添加额外的ped敌对逻辑。让我重新读用户输入："设计一个生存玩法：收到礼物"玫瑰"后，开启生存模式，把通缉等级改为5星，然后所有玩家攻击主角，然后给主角发一把冲锋枪并设置1000子弹"

"所有玩家攻击主角"——这里"玩家"可能是指NPC角色。在GTA5中，有时路人（ped）也会因为通缉而逃跑或报警，但不一定会攻击。不过通缉5星后，警察和SWAT/军队会主动攻击。这可能已经满足需求。

我决定不添加额外的RelationshipGroup逻辑，因为这可能导致过度设计。5星通缉足以让大量执法NPC攻击玩家。

但为了更准确满足"所有玩家攻击主角"，我可以在设计中加入一个选项：设置玩家与所有ped的关系为敌对。但这需要额外的API查询。当前工具预算已经用完（2次SHVDN调用），但能力已经足够核心实现。

我可以在 capabilities 表中标记 "all peds attack player" 为 needs_query 或 fallback，并说明通过5星通缉自然实现。

实际上，让我再想想。用户说的是"所有玩家攻击主角"，可能真的是指让所有ped敌对。但我已用完SHVDN查询预算。我可以在capability表中记录为 fallback：使用5星通缉系统间接实现。

好的，开始编写 DESIGN.md。

需要注意的几点：
- 使用 JavaScriptSerializer
- WebSocket 地址 ws://127.0.0.1:60080
- 订阅结构必须精确
- 后台线程解析，主线程执行GTA操作
- 冷却、队列、去重

让我也想想是否有其他需要考虑的。用户需求相对简单：礼物触发 → 5星通缉 → SMG+子弹。

还有一个问题：在生存模式中，玩家死亡后怎么办？用户说失败条件是主角死亡。死亡后应该清除状态，回到IDLE。

另外，如果玩家在生存模式中成功清除通缉（比如通过作弊或躲藏），是否算胜利？用户没有指定胜利条件。我设计为：生存模式持续直到玩家死亡或手动结束。没有明确的胜利条件，只有失败条件（死亡）。

实际上，GTA5中玩家死亡后通缉等级会自动清除。所以死亡后生存模式自然结束。但我们可能需要在死亡时做一些清理。

好的，现在开始写 DESIGN.md。

我还需要考虑：如果用户连续收到多个 Rose 礼物，在生存模式中应该怎么处理？根据 busy_state_policy，应该忽略（不叠加新的效果）。但可以显示一个提示。

关于通知：收到礼物触发时显示通知，告知玩家生存模式开始。

现在整理所有信息，准备输出。
</think># DESIGN.md

## 需求摘要
- **玩法名称**：玫瑰生存（Rose Survival）
- **核心目标**：收到礼物「Rose」后自动开启生存模式，主角获得冲锋枪与弹药，通缉等级升至5星，在执法力量围攻下坚持存活。
- **胜利条件**：无明确胜利条件（存活即挑战；死亡触发失败）。
- **失败条件**：主角死亡。
- **触发方式**：LIVE Studio 礼物消息 `WebcastGiftMessage`，礼物名称 `Rose`（gift_id=5655）。
- **正向效果**：给予冲锋枪（SMG）+ 1000 发子弹，并自动切换装备。
- **负向效果**：通缉等级设为 5 星，警察/军队等执法 NPC 持续攻击主角。
- **终止条件**：主角死亡；或脚本 Abort 时清理。

## 玩法流程
1. 脚本启动，建立 WebSocket 连接，订阅 LIVE Studio 消息。
2. 处于 **IDLE** 状态，等待礼物消息。
3. 收到 `WebcastGiftMessage`，`data.gift.id == 5655` 且 `data.repeatEnd == true` 时：
   - 校验冷却与去重（msgId）。
   - 通过 MainThreadDispatcher 排队执行：
     a. 记录当前通缉等级（用于 Abort 恢复）。
     b. 设置 `Game.Player.WantedLevel = 5`。
     c. 调用 `Game.Player.Character.Weapons.Give(WeaponHash.SMG, 1000, true, true)`。
     d. 显示通知："玫瑰生存模式开启！5星通缉 + SMG/1000发子弹"。
   - 状态切换为 **SURVIVAL**，启动冷却计时器。
4. **SURVIVAL** 状态下：
   - 持续检测 `Game.Player.Character.IsDead`，死亡则进入清理。
   - 新到的 Rose 礼物（同 ID）在冷却期内忽略；冷却期外且存活时，刷新弹药但不叠加通缉。
5. 脚本 Abort 时：
   - 恢复通缉等级至触发前记录值（若无法获取则清除通缉）。
   - 不删除任何武器。
   - 关闭 WebSocket。

## 状态机
```
IDLE  ──(收到 Rose 礼物)──▶ SURVIVAL
SURVIVAL ──(玩家死亡)──▶ IDLE（清理并重置冷却）
SURVIVAL ──(Script.Aborted)──▶ 清理 → 终止
```

## 触发条件表
| 条件 | 字段/值 | 说明 |
|---|---|---|
| 事件类型 | `data.common.method == "WebcastGiftMessage"` | 礼物消息 |
| 礼物 ID | `data.gift.id == 5655` | Rose |
| 连击结束 | `data.repeatEnd == true` | 仅在连击结束时触发一次 |
| 去重 | `data.common.msgId` | 已处理 msgId 集合，避免重复 |
| 冷却 | 60 秒 | 冷却内同礼物忽略 |
| 队列上限 | 3 | 超出丢弃最早 |

## 效果表
| 效果 | 实现 | 参数 |
|---|---|---|
| 通缉 5 星 | `Game.Player.WantedLevel = 5` | — |
| 给予冲锋枪 | `Game.Player.Character.Weapons.Give(WeaponHash.SMG, 1000, true, true)` | 武器=SMG，弹药=1000，立即装备，已装弹 |
| 通知 | `Notification.Show()` 或 `GTA.UI.Notification.PostTicker` | 中文提示 |

## capability_requirements

| capability | behavior | priority | status | verified_evidence | fallback |
|---|---|---|---|---|---|
| `Game.Player` | 获取本地玩家实例 | critical | verified | `static GTA.Player Player { get; }` | — |
| `Player.WantedLevel` | 读取/设置通缉等级 | critical | verified | `System.Int32 WantedLevel { get; set; }` | — |
| `Ped.Weapons` | 访问玩家武器集合 | critical | verified | `GTA.WeaponCollection Weapons { get; }` | — |
| `WeaponCollection.Give` | 给予武器并设定弹药 | critical | verified | `GTA.Weapon Give(GTA.WeaponHash weaponHash, System.Int32 ammoCount, System.Boolean equipNow, System.Boolean isAmmoLoaded)` | — |
| `WeaponHash.SMG` | 冲锋枪枚举值 | critical | verified | `GTA.WeaponHash.SMG` enum value | — |
| `Ped.IsDead` | 检测玩家死亡 | critical | needs_query | 未在本次查询中明确返回；基于 SHVDN 常识 `GTA.Ped.IsDead` 属性存在 | 使用 `Game.Player.Character.IsDead` 若不可用则轮询 `Game.Player.Character.Health <= 0` |
| 所有 NPC 攻击主角 | 5 星通缉使执法 NPC 攻击 | high | fallback | 通过 `WantedLevel = 5` 间接实现；GTA5 原生通缉系统自动生成敌对警察/军队 | 若需所有行人敌对，后续可通过 `Ped.RelationshipGroup` 扩展 |
| `Notification` / HUD 通知 | 显示玩法开始提示 | medium | needs_query | 未明确查询；SHVDN 常用 `GTA.UI.Notification.Show()` 或 `GTA.UI.Screen.ShowSubtitle()` | 使用 `GTA.UI.Screen.ShowSubtitle()` 作为降级 |

## LIVE Studio 事件与礼物映射

### WebSocket 连接
- **地址**：`ws://127.0.0.1:60080`
- **订阅结构**：
  ```json
  {"type":"subscribe","id":"gta-mod","data":{"type":"serviceSignalSub","name":"IM_MESSAGE_TRANSPORT"}}
  ```

### 事件映射
| 字段 | 路径 | 说明 |
|---|---|---|
| event_method | `data.common.method` | 取值 `"WebcastGiftMessage"` |
| msgId | `data.common.msgId` | 去重键 |
| createTime | `data.common.createTime` | 时间戳 |
| userId | `data.user.id` | 送礼用户 |
| nickname | `data.user.nickname` | 送礼昵称 |
| giftId | `data.gift.id` | 礼物 ID，匹配 5655 |
| giftName | `data.gift.name` | 礼物名称 "Rose" |
| diamondCount | `data.gift.diamondCount` | 钻石数（1） |
| repeatCount | `data.repeatCount` | 连击计数 |
| repeatEnd | `data.repeatEnd` | 连击是否结束 |

### 触发策略
| 策略 | 值 |
|---|---|
| repeat_policy | 仅在 `repeatEnd == true` 时触发 |
| dedupe_policy | 按 `msgId` HashSet 去重，集合上限 200 条 |
| cooldown | 60 秒（同礼物 ID） |
| queue_cap | 3（MainThreadDispatcher 队列上限） |
| busy_state_policy | SURVIVAL 模式下收到同礼物：冷却内忽略；冷却外且存活时，仅补满 SMG 弹药至 1000，不重复设通缉 |
| abort_cleanup | 恢复通缉等级至触发前记录值；若无法获取原始值则清除通缉；不删除任何武器；关闭 WebSocket |

## gift_requirements

| 字段 | 值 |
|---|---|
| original_query | 玫瑰 |
| keywords | rose,flower |
| status | matched |
| confirmed_name | Rose |
| gift_id | 5655 |
| diamondCount | 1 |
| candidates | [Rose (id:5655, score:100, exact_name), Rose Hand (id:16896, score:90), Rose Bear (id:16300, score:90), Rosie the Rose Bean (id:12210, score:90), Rose Soundwave (id:17983, score:90)] |
| fallback | — |

## 计时与冷却
| 项目 | 值 |
|---|---|
| 礼物触发冷却 | 60 秒（自上次效果生效起算） |
| 去重集合上限 | 200 条 msgId（FIFO 淘汰） |
| 主线程队列轮询 | 每 Tick 消费一条 |
| 死亡检测频率 | 每 Tick（约每秒 60 次） |

## 失败与清理
| 场景 | 行为 |
|---|---|
| 玩家死亡 | SURVIVAL → IDLE；通缉等级由 GTA 原生机制自动清除；保留武器；清除冷却状态 |
| Script.Aborted | 恢复通缉至触发前记录值（无记录则 `WantedLevel = 0`）；不删除 SMG 或其他武器；取消 WebSocket 接收、Abort/Dispose |
| WebSocket 断连 | 每 10 秒尝试重连一次；重连后重新订阅；期间保持当前状态（IDLE 或 SURVIVAL 继续检测死亡） |

## 降级方案
1. **IsDead 不可用**：改用 `Game.Player.Character.Health <= 0` 判定死亡。
2. **Notification.Show 不可用**：降级为 `GTA.UI.Screen.ShowSubtitle("SUBtitle text", 5000)`。
3. **WantedLevel 设置失败**：记录日志，仍给予武器，生存模式以无通缉状态运行（仅武器+弹药生效）。
4. **礼物 ID 不匹配**：已通过 gift_requirements 锁定 5655；若 LIVE Studio 推送不同 ID 的 Rose 变体，不会触发。
5. **WebSocket 始终无法连接**：脚本保持 IDLE 状态，每 10 秒重试，不阻塞游戏。

## 验收标准
- [ ] 收到 `gift.id=5655` 且 `repeatEnd=true` 时，通缉等级立即变为 5 星。
- [ ] 玩家获得 SMG 并装备，弹药为 1000 发。
- [ ] 60 秒内重复收到 Rose 礼物不叠加通缉、不重复给枪。
- [ ] 60 秒冷却后、玩家仍存活时，重复 Rose 仅补满 SMG 弹药至 1000。
- [ ] 玩家死亡后状态回归 IDLE，冷却重置。
- [ ] 脚本退出（Abort）时通缉等级恢复至触发前（或清除），武器保留。
- [ ] 非 Rose 礼物（如 Rose Hand id=16896）不触发玩法。
- [ ] 同一 msgId 不重复处理。
- [ ] WebSocket 断连后自动重连并重新订阅。