<think>
用户需要实现一个玩法：当收到"玫瑰"礼物时，触发三个效果：
1. 天气改为下雨
2. 通缉等级改为4星
3. 发放一把冲锋枪武器

我需要：
1. 首先查询"玫瑰"礼物的详细信息（gift_id、钻石数等）
2. 然后查询相关的SHVDN API能力：天气修改、通缉等级修改、武器发放

让我先并行调用这两个工具。
</think>
<think>
礼物查询返回了结果：名称"Rose"精确匹配，gift_id: 5655，diamondCount: 1。

SHVDN API查询出错，因为design_md太长了。我需要缩短它。

现在我需要重新查询SHVDN API。
</think>
<think>
好的，我现在有了礼物查询结果和SHVDN能力查询结果。总结：

**礼物方面：**
- "玫瑰"精确匹配：gift_id=5655，name="Rose"，diamondCount=1，状态verified。
- 其他候选是模糊匹配，但用户说"玫瑰礼物"，应理解为Rose（精确匹配）。

**SHVDN能力：**
- World.Weather (属性) - 已确认
- Weather.Rain 枚举值 - 未确认（needs_query）
- Player.WantedLevel - 已确认
- WeaponHash.SMG - 已确认
- Weapons.Give() 方法 - 未确认（needs_query）
- Game.Player - 已确认

现在我需要设计DESIGN.md。需要涵盖：
- 玩法概述
- 输入与触发条件
- 状态机
- 核心玩法流程
- 胜利与失败条件（这里没有胜利失败，但需要说明）
- 计时、冷却与防重复
- LIVE Studio互动映射
- gift_requirements
- SHVDN能力映射
- capability_requirements
- 资源清理与异常恢复
- 实现边界与降级方案

由于用户没有指定胜负条件、时间限制，玩法是瞬时触发式。但需要设计冷却防止重复触发（短时间内大量玫瑰礼物）。通常冷却时间可设为5-10秒。

状态机：Idle -> Triggered -> CoolingDown -> Idle。或者更简单：Idle，收到礼物后执行效果，然后进入冷却。

LIVE Studio互动映射：触发类型为"礼物"，礼物名称Rose，gift_id=5655，钻石数=1，验证状态=verified，玩法效果=天气下雨+通缉4星+发放冲锋枪，冷却=例如10秒。

gift_requirements YAML：一个条目，status: verified。

SHVDN能力映射表格：列出每个玩法效果对应的能力、检索结论、降级方案。

capability_requirements YAML：每个效果需要的能力及其查询状态。

现在需要再查询一下Weapons.Give方法和Weather.Rain枚举值，因为之前标记为未确认。但根据规则，我可以在capability_requirements中标记为needs_query，让代码生成节点继续查询。不过为了更完整，我现在可以再查询一次。

但考虑到时间和工具限制，我可以把未确认的能力标记为needs_query，并提供降级方案。

对于天气设置：降级方案可以是使用已知的Weather枚举值（如Weather.Clear或其他已确认的），但Weather.Rain应该存在。我可以再查一次，用不同的查询方式。也许可以查询"Weather.Rain"作为memberMatch，或者查询整个Weather枚举。

对于武器发放：降级方案可以是使用Game.Player.Character.Weapons.Give()，但需要代码生成节点确认。或者可以使用Native函数GIVE_WEAPON_TO_PED。

现在让我再查询一次这两个能力。
</think>
<think>
第二次查询也返回空。这意味着Weather.Rain和武器发放方法都未确认。但这不代表GTA5中真的没有这些功能——只是这个工具没有索引到。实际上，SHVDN v3确实有Weather枚举（包括Rain）、Ped.Weapons.Give()方法、以及Native函数GIVE_WEAPON_TO_PED。但根据规则，我只能基于工具结果标记状态。

所以：
- Weather.Rain: needs_query（虽然World.Weather属性已确认，但Rain值未确认）
- 武器发放: needs_query（WeaponHash.SMG已确认，但发放方法未确认）

现在我要设计DESIGN.md。根据规则，我不能编造API，必须诚实标记未验证的能力。但即使这些能力未验证，我仍然应该设计玩法，并给出降级方案。

降级方案：
- 天气：如果Weather.Rain不可用，可以尝试使用Native调用SET_WEATHER_TYPE_NOW，或者使用其他已确认的天气类型（如果有）。但工具没有确认任何Weather枚举值。我可以建议使用(int)转换或者Native。降级方案可以写：使用Function.Call(Hash.SET_WEATHER_TYPE_NOW, "RAIN")，尽管这个Native也没被确认。或者用World.Weather = (Weather)1这样的硬编码。但更好的做法是在capability_requirements中标记needs_query，让代码生成节点去查询Native。

- 武器发放：降级方案可以使用Native: Function.Call(Hash.GIVE_WEAPON_TO_PED, ped, weaponHash, ammo, false, false)。但同样未确认。或者使用Game.Player.Character.Weapons.Give()（如果代码生成节点能确认的话）。

现在构建DESIGN.md。

关于状态机：由于是瞬时触发，不需要复杂状态机。但需要冷却机制防止刷礼物。状态可以简单：

- Idle：等待礼物事件
- 收到玫瑰礼物 → 进入Cooldown，执行效果
- Cooldown持续N秒（如10秒），期间忽略玫瑰礼物
- Cooldown结束 → 回到Idle

另外，关于"礼物事件监听"：这是LIVE Studio集成的一部分。在SHVDN脚本中，通常做法是暴露一个public static方法，由外部系统（LIVE Studio桥接）在收到礼物时调用。所以在设计中，需要定义一个public方法如OnGiftReceived(int giftId, string giftName, int diamondCount)，由外部调用。

关于清理：不需要特殊清理，因为效果是瞬时修改状态。但如果需要在脚本卸载时恢复天气，可以记录原始天气并恢复。但用户没有要求恢复，所以可以不做。

现在生成DESIGN.md。
</think># DESIGN.md

## 玩法概述
当主播收到观众赠送的**玫瑰（Rose）**礼物时，自动触发三项游戏内效果：
1. 天气改为**下雨**
2. 玩家通缉等级设为**4星**
3. 向玩家发放一把**冲锋枪（SMG）**

该玩法为瞬时触发式效果，无胜负判定。主要目的是通过礼物互动制造戏剧性的直播效果。

---

## 输入与触发条件
- **触发来源**：LIVE Studio 礼物事件
- **触发礼物**：Rose（gift_id: 5655，钻石数: 1）
- **触发方式**：外部桥接模块调用脚本暴露的公共方法，传入礼物 ID
- **无评论触发、无计时触发、无系统触发**

---

## 状态机
```
 ┌──────────┐    收到Rose礼物    ┌──────────────┐
 │          │  (cooldown=false)  │              │
 │   Idle   │ ─────────────────> │  Triggered   │
 │          │                    │  (执行效果)   │
 │          │ <───────────────── │              │
 └──────────┘   冷却计时结束      └──────┬───────┘
       ^                                │
       │           收到Rose礼物          │
       │         (cooldown=true)        │
       └────────────────────────────────┘
                   (忽略)
```

- **Idle**：默认状态，等待礼物事件
- **Triggered**：收到 Rose 礼物且不在冷却期，依次执行三项效果，然后启动冷却计时器
- **冷却中**：收到 Rose 礼物时直接忽略，不执行任何效果
- 冷却结束后自动回到 Idle

---

## 核心玩法流程
1. 脚本初始化：注册礼物回调入口（public static 方法，供 LIVE Studio 桥接调用）
2. 收到礼物事件时，检查 gift_id 是否为 5655（Rose）
3. 检查冷却状态：
   - 若处于冷却期：忽略本次事件，不做任何操作
   - 若不在冷却期：继续执行
4. 依次执行三项效果：
   - 设置天气为下雨：通过 World.Weather 属性赋值（需确认 Weather.Rain 枚举值）
   - 设置通缉等级为 4：通过 Game.Player.WantedLevel = 4
   - 发放冲锋枪：通过武器发放方法给予玩家 SMG（WeaponHash.SMG，弹药量 500 发，立即装备）
5. 进入冷却状态，启动冷却计时器（默认 10 秒）
6. 冷却计时结束后，回到 Idle 状态

---

## 胜利与失败条件
- **无胜利条件**：本玩法为纯触发式效果，不设胜负
- **无失败条件**：玩家不会因通缉等级提升而"失败"，警察追捕属于游戏正常反应

---

## 计时、冷却与防重复
- **冷却时间**：10 秒（可配置常量）
- **防重复机制**：冷却期内收到的所有 Rose 礼物事件均被忽略
- **计时实现**：在脚本 Tick 中累计 deltaTime，或在 Triggered 时记录触发时间戳，Tick 中比较当前时间与触发时间
- **无超时限制**：通缉等级和天气不会自动恢复，由玩家自行处理或后续脚本干预

---

## LIVE Studio 互动映射

| 触发类型 | 礼物名称 | gift_id | 钻石数 | 验证状态 | 玩法效果 | 冷却 |
|----------|----------|---------|--------|----------|----------|------|
| 礼物 | Rose | 5655 | 1 | verified | 天气→下雨 / 通缉→4星 / 发放SMG | 10秒 |

---

## gift_requirements

```yaml
- trigger_type: gift
  gift_name: Rose
  gift_id: 5655
  diamond_count: 1
  status: verified
  effect: 设置天气为下雨、通缉等级4星、发放冲锋枪
  evidence: "query_live_gifts 精确匹配：name=Rose, id=5655, diamondCount=1, matchType=exact_name"
```

---

## SHVDN 能力映射

| 玩法效果 | 已验证能力/API | 检索结论 | 降级方案 |
|----------|---------------|----------|----------|
| 设置天气为下雨 | World.Weather 属性（静态，GTA.World）— 已确认 | Weather.Rain 枚举值未在工具中命中，无法确认具体枚举成员名称 | 使用 Native `Function.Call(Hash.SET_WEATHER_TYPE_NOW, "RAIN")` 或硬编码枚举整数值 `(Weather)1`；需代码生成节点进一步查询确认 |
| 设置通缉等级为4星 | Player.WantedLevel（实例属性，int 读写）— 已确认 | 签名 `System.Int32 WantedLevel { get; set; }`，可直接赋值 | 无需降级 |
| 发放冲锋枪（SMG） | WeaponHash.SMG 枚举值 — 已确认 | 武器发放方法 Give 未命中，Native GIVE_WEAPON_TO_PED 也未命中 | 使用 `Game.Player.Character.Weapons.Give(WeaponHash.SMG, 500, true, true)` 或 Native `Function.Call(Hash.GIVE_WEAPON_TO_PED, ped, (int)WeaponHash.SMG, 500, false, false)`；需代码生成节点进一步查询确认签名 |
| 获取本地玩家 | Game.Player 静态属性 — 已确认 | 签名 `static GTA.Player Player { get; }` | 无需降级 |
| 礼物事件接收 | 无 SHVDN 原生能力，需外部桥接调用脚本公共方法 | 脚本定义 `public static void OnGiftReceived(int giftId, string giftName, int diamondCount)` 供 LIVE Studio 桥接模块调用 | 非 SHVDN 范畴，属于集成层 |

---

## capability_requirements

```yaml
- effect: 设置天气为下雨
  query: "Weather.Rain 枚举值；若不可用则查询 SET_WEATHER_TYPE_NOW Native 或 World.Weather 的 Weather 枚举完整成员列表"
  status: needs_query
  evidence: "World.Weather 属性已确认；Weather.Rain 两次查询均返回空"

- effect: 设置通缉等级为4星
  query: "Player.WantedLevel 属性 setter 行为确认"
  status: verified
  evidence: "memberMatch 返回签名 System.Int32 WantedLevel { get; set; }"

- effect: 发放冲锋枪
  query: "Ped.Weapons.Give 方法或 GIVE_WEAPON_TO_PED Native 签名确认"
  status: needs_query
  evidence: "WeaponHash.SMG 枚举已确认；但 Give 方法和 Native 均未命中"

- effect: 获取本地玩家
  query: "Game.Player 静态属性"
  status: verified
  evidence: "memberMatch 返回 static GTA.Player Player { get; }"

- effect: 礼物事件监听
  query: "LIVE Studio 桥接接口规范"
  status: fallback
  evidence: "非 SHVDN 能力范畴，由集成层提供；脚本暴露 public static void 方法作为回调入口"
```

---

## 资源清理与异常恢复
- **脚本卸载（OnAbort/OnStop）**：无需特殊清理，天气和通缉等级属于游戏世界持久状态
- **异常处理**：每次执行效果前检查 Game.Player 和 Game.Player.Character 是否有效（非 null、非 Dead）
- **冷却状态重置**：脚本重载时冷却状态重置为 Idle
- **无持续资源占用**：只有冷却计时器在 Tick 中做简单时间比较，无额外实体、粒子或音效需清理

---

## 实现边界与降级方案
1. **天气设置为下雨**：
   - 优先方案：`World.Weather = Weather.Rain;`
   - 若 Weather.Rain 枚举值不可用：使用 Native `Function.Call(Hash.SET_WEATHER_TYPE_NOW, "RAIN");` 或尝试 `(Weather)1` 硬编码（需验证 GTA5 中 Rain 的枚举整数值）
   - 最终降级：使用已确认的任意 Weather 枚举值（如 Clear），功能降级为"改变天气"但不保证为下雨

2. **武器发放**：
   - 优先方案：`Game.Player.Character.Weapons.Give(WeaponHash.SMG, 500, true, true);`
   - 若 Give 方法签名不匹配：使用 Native `Function.Call(Hash.GIVE_WEAPON_TO_PED, Game.Player.Character.Handle, (int)WeaponHash.SMG, 500, false, false);`
   - 需代码生成节点确认具体方法签名或 Native 名称

3. **礼物事件接收**：
   - 脚本定义公共静态方法作为回调入口
   - 由 LIVE Studio 桥接模块在收到礼物时调用，传入 gift_id
   - 该部分不属于 SHVDN 能力范畴，需集成层配合

4. **冷却实现**：
   - 使用 `DateTime` 或 `Game.GameTime` 记录上次触发时间
   - 在 `OnTick` 中比较时间差，冷却结束后允许再次触发
   - 不使用 `Script.Wait()` 阻塞方式，以保证脚本持续响应