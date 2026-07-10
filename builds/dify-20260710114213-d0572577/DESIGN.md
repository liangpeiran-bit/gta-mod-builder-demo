<think>
用户需求很简单：收到玫瑰礼物，触发三个效果：天气改为下雨，通缉等级改为4星，发放一把冲锋枪武器。

我需要：
1. 查询玫瑰礼物的信息 - 使用 query_live_gifts
2. 查询 SHVDN API 能力：天气修改、通缉等级修改、武器发放 - 使用 query_shvdn_api

先同时调用这两个工具。
</think>
<think>
结果分析：

**礼物查询**：
- "玫瑰" 精确匹配到 "Rose"（gift_id: 5655, diamondCount: 1），状态 matched。还有其他候选但 Rose 是 exact_name match，score 100。用户说的"玫瑰礼物"应该就是 Rose，gift_id=5655，钻石数=1。

**SHVDN API**：
- 天气：`GTA.World.Weather` 属性存在，类型 `GTA.Weather` 枚举。但具体下雨枚举值（如 Rain）未被确认。需要进一步查询。
- 通缉等级：`GTA.Player.WantedLevel`（int）已确认，可读写。直接设置为4。
- 武器发放：`GTA.WeaponHash.SMG` 已确认。但发放方法未确认。需要进一步查询 `Give` 或 `WeaponCollection` 相关方法。

现在需要补充查询：天气枚举值和武器发放方法。再调用一次 query_shvdn_api。
</think>
<think>
现在汇总所有结果：

**礼物**：
- "Rose" (gift_id: 5655, diamondCount: 1) - verified (exact_name match)

**SHVDN API**：
- 天气：`GTA.Weather.Raining` 已确认，通过 `World.Weather` 设置 - verified
- 通缉等级：`Player.WantedLevel` (int) - verified
- 武器发放：未确认。需要再查一次，可能用 Native 函数 GIVE_WEAPON_TO_PED。或者换个查询方式。

让我再查一次武器发放方法，尝试不同的查询词。
</think>
<think>
现在汇总所有查询结果：

**礼物**：Rose (gift_id: 5655, diamondCount: 1) - verified

**SHVDN API**：
- 天气设为下雨：`GTA.Weather.Raining` + `World.Weather` - verified
- 通缉等级4星：`Player.WantedLevel` (int) - verified
- 发放冲锋枪武器：
  - 武器枚举：`GTA.WeaponHash.SMG` - verified
  - 授予方法：未确认 - needs_query / fallback

对于武器发放，由于三次查询都没找到直接的 SHVDN 封装方法，我将在设计中采用 fallback 方案：使用 Native 函数 `GIVE_WEAPON_TO_PED` 通过 `Function.Call`，但这在查询中未被确认，所以标记为 needs_query（代码生成节点需要进一步确认 Native 函数的可用性和签名）。

现在可以编写 DESIGN.md 了。

这是一个简单的礼物触发玩法，没有复杂的胜负条件、状态机等。核心流程：
1. 初始化：注册礼物事件监听
2. 收到 Rose 礼物时触发
3. 执行三个操作：设置天气为下雨、通缉等级4星、发放冲锋枪
4. 冷却：防止重复触发

状态机很简单：Idle -> Triggered -> Cooling -> Idle

现在编写完整的 DESIGN.md。
</think># DESIGN.md

## 玩法概述

当主播在 LIVE Studio 收到观众赠送的「Rose（玫瑰）」礼物时，GTA5 游戏内自动触发三项即时效果：

1. **天气变为下雨** — 当前世界天气立即切换为 `Raining`（下雨）。
2. **通缉等级设为 4 星** — 玩家角色通缉等级被强制设为 4 星。
3. **发放一把冲锋枪** — 玩家获得一把 SMG 冲锋枪武器。

该玩法为纯礼物驱动的单向触发，无胜负判定、无时间限制、无持续回合。每次收到玫瑰礼物即执行一次，设有冷却机制防止同一礼物短时间重复刷屏。

---

## 输入与触发条件

| 项目 | 说明 |
|------|------|
| 触发类型 | LIVE Studio 礼物事件 |
| 触发礼物 | **Rose**（玫瑰） |
| gift_id | `5655` |
| 钻石价格 | 1 钻石 |
| 其他触发方式 | 无（不支持评论触发、计时触发或系统触发） |
| 冷却时间 | 同一礼物单次触发后有 10 秒冷却，冷却期间忽略重复赠送 |

---

## 状态机

```
 ┌──────────┐    收到 Rose 礼物    ┌───────────┐
 │   Idle   │ ──────────────────→ │ Triggered │
 │ (待机)   │                     │ (已触发)   │
 └──────────┘                     └─────┬─────┘
       ↑                                │ 执行三个效果
       │                                │ 1. 天气→Raining
       │         冷却结束               │ 2. 通缉→4星
       │   (10秒后自动恢复)             │ 3. 发放SMG
       │                                │
       │         ┌──────────┐           │
       └─────────│ Cooling  │←──────────┘
                 │ (冷却中)  │
                 └──────────┘
```

- **Idle**：默认状态，等待礼物事件。
- **Triggered**：收到 Rose 礼物，立即执行三个效果，然后进入 Cooling。
- **Cooling**：冷却状态，忽略 Rose 礼物，10 秒后返回 Idle。

---

## 核心玩法流程

1. **初始化（OnStart / 脚本加载时）**
   - 订阅 LIVE Studio 礼物事件。
   - 将状态设为 `Idle`。
   - 冷却计时器归零。

2. **主循环（OnTick / 每帧检测）**
   - 无持续逻辑，所有行为由礼物事件驱动。
   - 若处于 Cooling 状态，递减冷却计时器，归零后切换至 Idle。

3. **礼物事件回调（OnGiftReceived）**
   - 检查当前状态：若为 Cooling，直接忽略并返回。
   - 检查礼物是否为 Rose（gift_id = 5655 或 名称匹配 "Rose"）。
   - 匹配成功：
     a. 设置世界天气为 `GTA.Weather.Raining`（`World.Weather = Weather.Raining`）。
     b. 设置玩家通缉等级为 4（`Game.Player.WantedLevel = 4`）。
     c. 给玩家角色发放冲锋枪武器（见降级方案）。
   - 将状态切换为 Cooling，冷却计时器设为 10 秒。

4. **异常处理**
   - 若玩家角色不存在（Ped 为 null），跳过通缉等级和武器发放，仅执行天气修改。
   - 若世界天气设置失败，静默忽略（不中断后续步骤）。

---

## 胜利与失败条件

- 无。本玩法为纯效果触发，不存在胜负判定。

---

## 计时、冷却与防重复

| 机制 | 说明 |
|------|------|
| 冷却时间 | 10 秒（从触发完成开始计时） |
| 冷却期间行为 | 忽略所有 Rose 礼物事件 |
| 防重复触发 | 通过状态机 Cooling 状态阻止 |
| 无超时 | 玩法永不主动结束，持续监听 |

---

## LIVE Studio 互动映射

| 触发类型 | 礼物名称 | gift_id | 钻石数 | 验证状态 | 玩法效果 | 冷却 |
|----------|----------|---------|--------|----------|----------|------|
| 礼物 | Rose | 5655 | 1 | verified | 天气→下雨 + 通缉4星 + 发放SMG | 10秒 |

---

## gift_requirements

```yaml
- trigger_type: gift
  gift_name: Rose
  gift_id: 5655
  diamond_count: 1
  status: verified
  effect: 设置天气为Raining、通缉等级4星、发放SMG冲锋枪
  evidence: "query_live_gifts 精确匹配：exact_name match, score 100, name='Rose', id=5655, diamondCount=1"
```

---

## SHVDN 能力映射

| 玩法效果 | 已验证能力/API | 检索结论 | 降级方案 |
|----------|---------------|----------|----------|
| 天气改为下雨 | `GTA.World.Weather` 属性 + `GTA.Weather.Raining` 枚举值 | verified：`World.Weather` 可读写，`Weather.Raining` 存在 | 无需降级，直接使用 |
| 通缉等级设为4星 | `GTA.Player.WantedLevel` 属性（int，可读写） | verified：直接赋值 `Game.Player.WantedLevel = 4` | 无需降级，直接使用 |
| 发放冲锋枪武器 | `GTA.WeaponHash.SMG` 枚举值 | verified：SMG 枚举存在于 WeaponHash | — |
| 发放冲锋枪武器（授予方法） | 未确认：`Player.Weapons.Give`、`WeaponCollection.Give`、`GiveWeapon` 等均未命中 | needs_query：三次检索均未发现 SHVDN 封装的武器授予方法 | **fallback**：使用 `Function.Call(Hash.GIVE_WEAPON_TO_PED, ped, (uint)WeaponHash.SMG, ammo, false, false)` 通过 Native 函数发放；若 Native 也失败，则跳过武器发放并记录日志，不影响其他效果执行 |

---

## capability_requirements

```yaml
- effect: 设置世界天气为下雨
  query: "GTA.World.Weather 属性 + GTA.Weather 枚举中下雨相关值"
  status: verified
  evidence: "World.Weather 可读写；Weather.Raining 枚举值已确认"

- effect: 设置玩家通缉等级为4星
  query: "GTA.Player.WantedLevel 属性"
  status: verified
  evidence: "Player.WantedLevel 为 int 类型，可读写，直接赋值即可"

- effect: 获取冲锋枪武器枚举值
  query: "GTA.WeaponHash 枚举中 SMG 相关值"
  status: verified
  evidence: "WeaponHash.SMG 存在，代表冲锋枪"

- effect: 向玩家发放武器
  query: "Player.Weapons.Give / WeaponCollection.Give / GiveWeapon 方法"
  status: needs_query
  evidence: "三次检索均未命中 SHVDN 封装的武器授予方法；代码生成节点需进一步确认 Native GIVE_WEAPON_TO_PED 签名或替代 API"
```

---

## 资源清理与异常恢复

| 场景 | 处理方式 |
|------|----------|
| 脚本卸载 (OnStop / Abort) | 取消礼物事件订阅，重置冷却状态 |
| 玩家角色不存在 | 跳过通缉等级设置和武器发放，仅执行天气修改 |
| 礼物事件传入空数据 | 检查 gift_id 是否为 0 或 null，直接返回 |
| 天气设置异常 | try-catch 包裹，失败时静默继续 |
| 连续快速触发 | 冷却机制保证 10 秒内仅触发一次 |
| 玩家死亡/重生 | 不主动恢复天气和通缉；下次礼物触发时重新设置 |

---

## 实现边界与降级方案

1. **武器发放为首要降级点**：SHVDN v3 封装的武器授予 API 未在检索中确认，优先使用 Native `GIVE_WEAPON_TO_PED`（需代码生成节点进一步校验 Hash 值和签名）。若 Native 同样不可用，武器发放效果降级为「跳过」，其他两个效果正常执行。

2. **天气枚举仅使用 Raining**：检索仅确认 `GTA.Weather.Raining`，不使用 `Rain`、`ThunderStorm` 等未验证值。

3. **通缉等级直接赋值**：`Player.WantedLevel` 赋值即可，不调用 `Function.Call` 设置通缉。

4. **单文件脚本**：所有逻辑在单个 `*.cs` 文件中实现，继承 `Script` 基类，无需额外程序集或资源文件。

5. **无其他 NPC/实体操作**：不生成车辆、不创建 NPC、不播放特效，保持最小依赖。

6. **礼物匹配采用双重校验**：同时匹配 gift_id（数字 5655）和礼物名称（字符串 "Rose"），任一匹配即视为命中，提高兼容性。