<think>
用户想要一个基于礼物触发的GTA5效果：收到玫瑰礼物后，改变天气为下雨，设置通缉等级为4星，发放一把冲锋枪武器。

我需要设计这个玩法，首先需要查询SHVDN API能力，看看能否实现天气改变、通缉等级设置、武器发放。同时需要查询礼物系统中的玫瑰礼物信息。

让我先同时查询这些信息。
</think>
<think>
礼物查询成功，找到了"Rose"礼物，gift_id=5655，diamondCount=1，精确匹配。很好。

现在SHVDN API查询失败了，因为capability_request太长了。我需要缩短它，或者分多次查询。

让我重新查询SHVDN API，分批次进行。先查询天气修改相关能力。
</think>
<think>
第一个查询返回了错误（HTML页面），但第二个查询成功返回了API上下文。第三个查询也失败了。

根据第二次查询的结果，我得到了：

1. 天气：World.Weather 属性，但 Weather 枚举未确认 Rain 值。
2. 通缉等级：Player.WantedLevel 属性。
3. 发放武器：未确认 GiveWeapon 方法，但有 WeaponHash.SMG。

现在我需要进一步查询：
- 天气枚举的 Rain 值
- 武器给予方法
- 可能还需要确认是否有 Ped.Weapons.Give 或类似方法。

让我再尝试查询这些。但要注意 capability_request 限制256字符。我第二次查询成功了，因为请求很短。让我再试。
</think>
<think>
现在我已经获得了所有需要的信息。让我总结一下：

**礼物信息：**
- 精确匹配到"Rose"，gift_id=5655，diamondCount=1，状态verified。

**SHVDN API能力：**
- 设置天气为下雨：`GTA.World.Weather`属性存在，类型`GTA.Weather`枚举。但枚举值中未找到`Rain`，需要进一步确认。不过根据之前的查询（第二次查询），提到了`World.Weather`。所以天气设置能力是存在的（World.Weather属性），但具体的Rain枚举值未确认。需要标记为needs_query或fallback。可以使用原生函数`SET_WEATHER_TYPE_NOW_PERSIST`作为降级方案。
- 通缉等级4星：`GTA.Player.WantedLevel`属性存在，可读可写，类型int。已确认。
- 发冲锋枪：`GTA.WeaponHash.SMG`枚举存在。给予武器的方法未确认（查询*GiveWeapon*无结果）。可能需要使用`Ped.Weapons.Give`或原生函数`GIVE_WEAPON_TO_PED`。标记为needs_query或fallback。

现在我需要生成DESIGN.md。根据要求，需要包含以下章节：

- 玩法概述
- 输入与触发条件
- 状态机
- 核心玩法流程
- 胜利与失败条件
- 计时、冷却与防重复
- LIVE Studio 互动映射
- gift_requirements
- SHVDN 能力映射
- capability_requirements
- 资源清理与异常恢复
- 实现边界与降级方案

现在开始构建。

**玩法概述：** 当观众赠送"Rose"礼物（gift_id=5655，1钻石）时，立即触发以下效果：将游戏天气切换为下雨，玩家通缉等级设为4星，并发放一把冲锋枪（SMG）。

**输入与触发条件：** 仅接受LIVE Studio的礼物事件，具体为礼物名称"Rose"，gift_id 5655。无需评论触发。

**状态机：** 简单状态机：Idle -> Triggered -> Cooldown -> Idle。

**核心玩法流程：**
1. 脚本初始化，注册LIVE Studio礼物接收事件。
2. 收到礼物事件，检查是否为Rose（gift_id=5655）。
3. 若是，执行三个动作：
   - 设置天气为下雨（通过World.Weather属性或原生函数）
   - 设置通缉等级为4（通过Game.Player.WantedLevel = 4）
   - 发放冲锋枪（通过原生函数GIVE_WEAPON_TO_PED或Weapons.Give）
4. 进入冷却状态，防止短时间内重复触发。

**胜利与失败条件：** 无。这是单次触发效果，没有胜负判定。

**计时、冷却与防重复：** 建议冷却时间例如10-30秒，防止连续刷礼物导致频繁天气切换和武器堆积。可配置冷却时间。

**LIVE Studio 互动映射：** 表格。

**gift_requirements：** YAML列表，包含Rose礼物信息。

**SHVDN 能力映射：** 表格。

**capability_requirements：** YAML列表。

**资源清理与异常恢复：** 在脚本终止时（OnAbort）清理资源，注销事件处理器。

**实现边界与降级方案：** 
- 天气：若World.Weather枚举不可用，使用原生函数SET_WEATHER_TYPE_NOW_PERSIST，传入"RAIN"字符串。
- 武器：若Weapons.Give不可用，使用GIVE_WEAPON_TO_PED原生函数。
- 通缉等级：直接使用Player.WantedLevel属性。

现在输出DESIGN.md。
</think># DESIGN.md

## 玩法概述

玩法名称：**玫瑰风暴**  
当观众在 LIVE Studio 中赠送 **Rose**（玫瑰）礼物时，脚本立即在游戏中执行三项效果：  

1. 将天气切换为 **下雨**（Rain）。  
2. 将玩家通缉等级设置为 **4 星**。  
3. 向玩家发放一把 **微型冲锋枪（SMG）**，并附带一定弹药。  

该玩法为单次触发效果，无胜负判定，旨在通过礼物互动制造突发游戏氛围变化，增加直播观赏性与互动趣味。

---

## 输入与触发条件

- **触发来源**：LIVE Studio 礼物事件。
- **触发礼物**：`Rose`（gift_id = **5655**，钻石价格 = **1**）。
- **触发逻辑**：仅当收到的礼物名称与 gift_id 精确匹配时才执行效果；其他礼物被静默忽略。
- **无需评论触发**：本玩法不依赖任何聊天或弹幕指令。

---

## 状态机

| 状态 | 说明 |
|------|------|
| `Idle` | 等待礼物事件。冷却计时器为 0。 |
| `Triggered` | 收到 Rose 礼物，执行三项效果。进入 `Cooldown`。 |
| `Cooldown` | 冷却期内忽略同名礼物，防止重复堆叠。冷却结束后回到 `Idle`。 |

状态迁移：

```
Idle --[Rose 礼物 && 冷却结束]--> Triggered
Triggered --[效果执行完毕]--> Cooldown
Cooldown --[冷却计时结束]--> Idle
```

---

## 核心玩法流程

1. **脚本初始化（OnStart）**  
   - 订阅 LIVE Studio 礼物接收事件。  
   - 初始化冷却变量（`cooldownRemaining` 为 0）。  

2. **主循环（OnTick / 事件驱动）**  
   - 每帧递减冷却计时器（若大于 0）。  
   - 冷却期间不响应任何礼物。

3. **礼物事件回调**  
   - 解析事件中的 `gift_id` 和 `gift_name`。  
   - 若 `gift_id == 5655` 且 `gift_name == "Rose"`，且当前处于 `Idle` 状态：  
     a. **设置天气为下雨**：通过 `World.Weather = GTA.Weather.Rain`（或原生函数降级）。  
     b. **设置通缉等级**：`Game.Player.WantedLevel = 4`。  
     c. **发放冲锋枪**：向 `Game.Player.Character` 发放 `WeaponHash.SMG` 并设定弹药量（例如 500 发）。  
     d. 启动冷却计时器（建议默认 15 秒，可配置）。  

4. **冷却结束**  
   - 冷却计时归零后，允许再次触发。

---

## 胜利与失败条件

- **无胜负判定**。本玩法为纯触发式视觉效果与难度调整，不设胜利目标或失败惩罚。  
- 通缉等级升高后，玩家可自行选择逃脱警察或继续游戏；脚本不会自动清除通缉。

---

## 计时、冷却与防重复

| 项目 | 设计 |
|------|------|
| 冷却时间 | **15 秒**（默认值，可在常量中修改）。 |
| 计时方式 | 在 `OnTick` 中基于 `Game.LastFrameTime` 递减，避免高帧率下计时不稳。 |
| 防重复 | 冷却期内直接丢弃同一礼物事件；不执行任何效果，也不打印重复日志（避免刷屏）。 |
| 并发保护 | 使用 `bool isExecuting` 锁变量，确保同一帧内不会因事件重复派发而多次执行。 |

---

## LIVE Studio 互动映射

| 触发类型 | 礼物名称 | gift_id | 钻石数 | 验证状态 | 玩法效果 | 冷却 |
|----------|----------|---------|--------|----------|----------|------|
| 礼物 | Rose | 5655 | 1 | verified | 天气→下雨；通缉→4星；发放SMG冲锋枪 | 15秒 |

> 非礼物触发（如评论、计时）不在本设计中，对应字段为 null。

---

## gift_requirements

```yaml
- trigger_type: gift
  gift_name: Rose
  gift_id: 5655
  diamond_count: 1
  status: verified
  effect: "设置天气为下雨，通缉4星，发放SMG冲锋枪"
  evidence: "query_live_gifts_tool 精确匹配：name=Rose, id=5655, diamondCount=1, matchType=exact_name"
```

---

## SHVDN 能力映射

| 玩法效果 | 已验证能力/API | 检索结论 | 降级方案 |
|----------|---------------|---------|---------|
| 改变天气为下雨 | `GTA.World.Weather` 属性（类型 `GTA.Weather`） | 属性存在；枚举值 `Rain` 未在查询中直接命中，标记 **needs_query**。 | 使用原生函数 `Function.Call(Hash.SET_WEATHER_TYPE_NOW_PERSIST, "RAIN")` 作为降级。 |
| 设置通缉等级为4星 | `GTA.Player.WantedLevel` 属性（`System.Int32`，可读写） | 已验证。直接赋值 `4` 即可。 | 无需降级；属性本身为受管封装。 |
| 发放冲锋枪（SMG） | `GTA.WeaponHash.SMG` 枚举已验证；给予武器的方法 `*GiveWeapon*` 未命中 | 武器枚举存在；授予方法 **needs_query**。 | 降级使用原生函数 `Function.Call(Hash.GIVE_WEAPON_TO_PED, ped, (uint)WeaponHash.SMG, ammo, false, false)`。 |
| 获取玩家 Ped | `Game.Player.Character`（类型 `GTA.Ped`） | 标准 SHVDN v3 属性，无需额外查询。 | 无需降级。 |

---

## capability_requirements

```yaml
- effect: "设置天气为下雨"
  query: "Weather enum Rain value OR World.Weather setter"
  status: needs_query
  evidence: "World.Weather 属性已出现于查询，但 GTA.Weather 枚举未命中 Rain；需补查枚举全部成员或确认原生函数 SET_WEATHER_TYPE_NOW_PERSIST 可用。"

- effect: "给予玩家 SMG 冲锋枪"
  query: "Ped.Weapons.Give method or GIVE_WEAPON_TO_PED native"
  status: needs_query
  evidence: "WeaponHash.SMG 已确认；*GiveWeapon* 查询无结果。推测 Give 方法存在于 Weapons 集合，需补查 memberMatch: '*Weapons.Give*' 或直接验证原生函数 GIVE_WEAPON_TO_PED。"

- effect: "设置通缉等级为4星"
  query: "Player.WantedLevel"
  status: verified
  evidence: "查询命中 GTA.Player.WantedLevel 属性（get/set System.Int32）。"
```

---

## 资源清理与异常恢复

- **事件注销**：在脚本 `OnAbort` / `Dispose` 中注销 LIVE Studio 礼物回调，防止内存泄漏或空引用。
- **冷却重置**：脚本终止时不保留状态，下次启动总是从 `Idle` 开始。
- **异常保护**：三个效果分别用 try-catch 包裹；单个效果失败不影响其他效果执行，且错误记录到日志但不会崩溃脚本。
- **通缉等级不自动恢复**：脚本不负责降星或还原天气；这是设计意图，若需要可后续添加可选恢复逻辑（例如玩家死亡后复位）。

---

## 实现边界与降级方案

1. **天气枚举未确认**  
   - 优先尝试 `World.Weather = GTA.Weather.Rain`。  
   - 若编译失败或枚举不存在，降级为原生调用：`Function.Call(Hash.SET_WEATHER_TYPE_NOW_PERSIST, "RAIN")`。  
   - 注意：原生函数可能需要额外的 Weather 过渡时间参数；默认写 0 即可。

2. **武器给予方法未确认**  
   - 优先尝试受管方法：`Game.Player.Character.Weapons.Give(WeaponHash.SMG, 500, true, false)`。  
   - 若 `Weapons.Give` 不存在，降级为原生：  
     `Function.Call(Hash.GIVE_WEAPON_TO_PED, Game.Player.Character, (uint)WeaponHash.SMG, 500, false, false)`。  
   - 准备将 `WeaponHash` 枚举值显式转换为 `uint`。

3. **通缉等级**  
   - 直接使用 `Game.Player.WantedLevel = 4`，无降级必要。

4. **礼物映射**  
   - 硬编码 gift_id=5655；若未来 LIVE Studio 配置变动，需手动更新常量。

5. **单文件脚本**  
   - 所有逻辑编写在一个 `.cs` 文件中，继承自 `Script` 类；不依赖外部程序集或自定义资源。

6. **无自定义资源**  
   - 不加载纹理、音效或模型；仅使用游戏原生能力。