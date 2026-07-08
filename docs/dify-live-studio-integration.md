# Dify LIVE Studio Workflow

This document describes the Dify-only workflow for building GTA5 ScriptHookVDotNet v3 mods that react to TikTok LIVE Studio chat and gift events.

## Architecture

```text
User request
-> Multilingual Intake
-> LIVE Studio Scope Gate
-> Gift Catalog Resolver
-> DESIGN.md Generator
-> Mod.cs Generator
-> QA Fixer
-> Build Payload
-> GitHub Actions
-> GitHub Pages status/download page
```

## Stable Template Boundary

Dify generates only these files:

- `DESIGN.md`
- `ModProject/Mod.cs`

GitHub Actions owns the fixed template:

- `LiveStudioClient`
- `LiveStudioParser`
- `LiveStudioEvents`
- `MainThreadDispatcher`
- `ModProject.csproj`
- `Directory.Build.props`
- `scripts/build-mod.ps1`
- `references/shvdn/INDEX.md`
- `references/shvdn/capabilities.json`

Do not ask Dify to regenerate the WebSocket client, parser, dispatcher, project file, or build script.

## SHVDN Knowledge Visibility

The repository snapshot under `templates/live-studio/references/shvdn/` is the versioned source of truth, but Dify Cloud cannot read repository files automatically.

For Dify, import the generated Markdown pack under `docs/dify-knowledge/shvdn/` into a Dify Knowledge Base, for example `GTA5 SHVDN v3 Reference`. `Mod.cs Generator` and `QA Fixer` must use Knowledge Base retrieval output as their SHVDN context.

Do not write prompts that tell Dify to "look at" local repository paths. Those paths are only visible to GitHub Actions and local developers.

## Node Contracts

### Multilingual Intake

Purpose:

- Identify the user's language.
- Extract chat keywords, gift names, requested gameplay effects, cooldowns, and runtime rules.
- Keep explanations in the user's language.
- Keep code, identifiers, file names, gift names, commands, and in-game text in English.

Output shape:

```json
{
  "language": "zh-CN",
  "summary_en": "A LIVE Studio mod where Rose heals the player and chat boost accelerates the current vehicle.",
  "chat_triggers": [
    { "keyword": "boost", "effect": "boost current vehicle" }
  ],
  "gift_triggers": [
    { "gift_name": "Rose", "effect": "heal player" }
  ],
  "gameplay_mode": "triggered_effect"
}
```

### LIVE Studio Scope Gate

Allow only:

- GTA5 Story Mode.
- ScriptHookVDotNet v3.
- C# 7.3 / .NET Framework 4.8.
- LIVE Studio chat and gift triggers: `WebcastChatMessage`, `WebcastGiftMessage`.
- Built-in GTA5 effects: player state, vehicles, peds, weather, wanted level, weapons, subtitles/notifications, simple physics.

Block:

- GTA Online, cheats, money/XP/rank manipulation, anti-cheat bypass.
- follow, like, join, share, room-state triggers.
- OBS overlays, OpenIV/RPF, custom models/textures/audio, DLC packs.
- External network dependencies inside the generated mod beyond the fixed local LIVE Studio bridge.
- Arbitrary binary upload or third-party precompiled code.

### Gift Catalog Resolver

Use `templates/live-studio/references/gift-catalog.txt` as a static common gift snapshot.

Rules:

- If the user asks for Rose, resolve `gift_id: 5655`.
- If multiple candidates match, ask the user to choose.
- If no candidate matches, ask the user for a numeric `gift_id`.
- Never guess a gift id.

### DESIGN.md Generator

Output a complete `DESIGN.md` with these sections:

- `Mod Summary`
- `Interaction Map`
- `Gameplay Rules`
- `Game Effects`
- `Edge Cases & Boundaries`
- `Design Notes`
- `Change Log`

The `Interaction Map` must be implementation-ready:

```markdown
| Trigger | Behavior | Notes |
| --- | --- | --- |
| chat "boost" | Boost the player's current vehicle for 5 seconds. | 5 second cooldown. |
| Rose (gift_id: 5655) | Heal the player and set armor to 100. | Trigger only on RepeatEnd. |
```

### Mod.cs Generator

Generate only `ModProject/Mod.cs`.

Required structure:

```csharp
namespace ModProject
{
    public class Mod : Script
    {
        // ...
    }
}
```

Rules:

- Use `LiveStudioClient`, `ChatEvent`, `GiftEvent`, and `MainThreadDispatcher` from the fixed template.
- Do not regenerate template support classes.
- Do not call `World.*`, `Game.*`, `Ped`, `Vehicle`, or UI APIs directly from WebSocket callbacks.
- Use `MainThreadDispatcher.Enqueue(...)` for game-world changes.
- Default gift behavior should check `gift.RepeatEnd` so combo gifts do not fire repeatedly.
- Use cooldowns and bounded state for audience-triggered effects.
- Do not use `GTA.KeyEventArgs`; use `System.Windows.Forms.KeyEventArgs` / `KeyEventArgs`.
- Do not read set-only vehicle multiplier properties. If setting engine multipliers, reset them to `1.0f`.
- Keep C# 7.3 syntax.
- Before using a SHVDN type, enum, member, or native call that is not already in the template, verify it against the `SHVDN Context Retrieval` output from the Dify Knowledge Base.

### SHVDN API Intent Planner

Place this node after `DESIGN.md Generator` and before `Mod.cs Generator`.

Input:

- User request.
- Generated `DESIGN.md`.

Output JSON:

```json
{
  "needed_symbols": [
    "GTA.UI.Notification",
    "GTA.World.CreateVehicle",
    "GTA.VehicleHash"
  ],
  "risk_notes": [
    "Do not read EngineTorqueMultiplier; set only and reset to 1.0f."
  ]
}
```

This node does not write code. It narrows the Knowledge Base query.

### SHVDN Context Retrieval

Retrieve from the Dify Knowledge Base using `needed_symbols` and `risk_notes`.

Pass the retrieved context into `Mod.cs Generator` and `QA Fixer`.

If a required API is missing from retrieved context and is not already in the fixed template, prefer a simpler supported effect instead of inventing an API.

### QA Fixer

Check `DESIGN.md` against `Mod.cs`:

- Every `Interaction Map` row has a code path.
- Every chat trigger checks the correct keyword.
- Every gift trigger checks the exact `gift_id`.
- Combo gifts use `RepeatEnd` unless the design says otherwise.
- All GTA world mutations are inside `MainThreadDispatcher.Enqueue(...)`.
- No unsupported APIs or C# 8+ syntax.
- Any non-template SHVDN API usage exists in the retrieved SHVDN Knowledge Base context.

If code is invalid, rewrite `Mod.cs` instead of explaining the issue.

### Build Payload

Inputs:

- `design_md`
- `mod_cs`
- `assembly_name`

Outputs:

- `build_id`
- `assembly_name`
- `design_md_b64`
- `mod_cs_b64`
- `download_page`

### Trigger Build

Call:

```text
POST https://api.github.com/repos/liangpeiran-bit/gta-mod-builder-demo/actions/workflows/build-mod.yml/dispatches
```

Body:

```json
{
  "ref": "main",
  "inputs": {
    "build_id": "{{#code.build_id#}}",
    "assembly_name": "{{#code.assembly_name#}}",
    "design_md_b64": "{{#code.design_md_b64#}}",
    "mod_cs_b64": "{{#code.mod_cs_b64#}}"
  }
}
```

## Acceptance Tests

### Chat Trigger Demo

Prompt:

```text
Create a LIVE Studio GTA5 mod where chat "boost" repairs and boosts the current vehicle for 5 seconds, and chat "clear" clears the wanted level.
```

Expected:

- `DESIGN.md` contains both chat triggers.
- `Mod.cs` routes both chat triggers.
- Build status page eventually publishes a zip.

### Gift Trigger Demo

Prompt:

```text
Create a LIVE Studio GTA5 mod where Rose heals the player and gives 100 armor.
```

Expected:

- Gift resolver uses `gift_id: 5655`.
- `Mod.cs` checks `gift.GiftId == "5655"` and `gift.RepeatEnd`.
- Build status page eventually publishes a zip.

### Blocked Request Demo

Prompt:

```text
Create a GTA Online money mod triggered by likes.
```

Expected:

- Scope Gate blocks the request.
- Workflow does not trigger GitHub Actions.
