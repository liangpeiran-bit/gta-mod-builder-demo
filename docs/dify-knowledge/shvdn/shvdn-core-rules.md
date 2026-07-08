# SHVDN Core Rules for Dify

Use these rules in `Mod.cs Generator` and `QA Fixer`.

## Runtime

- Target ScriptHookVDotNet v3.
- Target C# 7.3 and .NET Framework 4.8.
- Generate only `ModProject/Mod.cs`; do not regenerate the fixed LIVE Studio template.
- All GTA world changes from LIVE Studio callbacks must run through `MainThreadDispatcher.Enqueue(...)`.

## Known Compile Pitfalls

- Do not use `GTA.KeyEventArgs`; use `System.Windows.Forms.KeyEventArgs` / `KeyEventArgs`.
- Do not use unqualified `Screen.ShowSubtitle`; use `GTA.UI.Screen.ShowSubtitle` or `GTA.UI.Notification.Show`.
- Do not read set-only vehicle multiplier properties. `Vehicle.EnginePowerMultiplier` and `Vehicle.EngineTorqueMultiplier` may be set; reset them to `1.0f` instead of storing original values.
- Do not use `CreatedAt` properties; track expiry with `DateTime` dictionaries or small tracked object classes.
- Do not use C# 8+ syntax: records, file-scoped namespaces, nullable reference types, target-typed `new`, top-level statements.

## Unsupported Scope

- GTA Online, online economy, anti-cheat bypass, money/rank/XP manipulation.
- OpenIV, RPF, DLC packs, custom models, custom textures, OBS overlays.
- Third-party precompiled binaries or arbitrary external assets.

## Generation Rule

If a requested SHVDN type/member/enum is not present in retrieved Knowledge Base context and not already used in the fixed template, do not invent it. Prefer a simpler supported effect.