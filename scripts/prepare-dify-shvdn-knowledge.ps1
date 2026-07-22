$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$sourceDir = Join-Path $repoRoot "templates/live-studio/references/shvdn"
$capabilitiesPath = Join-Path $sourceDir "capabilities.json"
$indexPath = Join-Path $sourceDir "INDEX.md"
$outputDir = Join-Path $repoRoot "docs/dify-knowledge/shvdn"

if (-not (Test-Path -LiteralPath $capabilitiesPath)) {
  throw "Cannot find SHVDN capabilities snapshot: $capabilitiesPath"
}

New-Item -ItemType Directory -Force -Path $outputDir | Out-Null

$catalog = Get-Content -LiteralPath $capabilitiesPath -Raw | ConvertFrom-Json

function Write-Utf8NoBom {
  param(
    [string]$Path,
    [string]$Text
  )
  [System.IO.File]::WriteAllText($Path, $Text, [System.Text.UTF8Encoding]::new($false))
}

function Get-TypeDoc {
  param([string]$FullName)

  $prop = $catalog.types.PSObject.Properties[$FullName]
  if ($null -eq $prop) {
    throw "Type not found in SHVDN snapshot: $FullName"
  }
  return $prop.Value
}

function Append-MemberList {
  param(
    [System.Collections.Generic.List[string]]$Lines,
    [string]$Title,
    [object[]]$Items,
    [int]$Limit = 80
  )

  $Lines.Add("")
  $Lines.Add("### $Title")
  if ($null -eq $Items -or $Items.Count -eq 0) {
    $Lines.Add("")
    $Lines.Add("- None")
    return
  }

  $count = 0
  foreach ($item in $Items) {
    if ($null -eq $item) {
      continue
    }
    if ($count -ge $Limit) {
      $Lines.Add("- ... truncated; query the source snapshot for more members.")
      break
    }
    $summary = ""
    if ($item.PSObject.Properties["summary"] -and -not [string]::IsNullOrWhiteSpace($item.summary)) {
      $summary = " - " + (($item.summary -replace "\s+", " ").Trim())
    }
    $signature = $item.signature
    if ([string]::IsNullOrWhiteSpace($signature) -and $item.PSObject.Properties["literal"]) {
      $signature = $item.literal
    }
    if ([string]::IsNullOrWhiteSpace($signature) -and $item.PSObject.Properties["name"]) {
      $signature = $item.name
    }
    $Lines.Add(("- {0}{1}" -f $signature, $summary))
    $count++
  }
}

function New-TypeSection {
  param([string]$FullName)

  $type = Get-TypeDoc $FullName
  $lines = [System.Collections.Generic.List[string]]::new()
  $lines.Add(("## {0}" -f $type.fullName))
  $lines.Add("")
  $lines.Add(("- Kind: {0}" -f $type.kind))
  $lines.Add(("- Namespace: {0}" -f $type.PSObject.Properties["namespace"].Value))
  $lines.Add(("- Domain: {0}" -f $type.domain))
  if (-not [string]::IsNullOrWhiteSpace($type.summary)) {
    $lines.Add(("- Summary: {0}" -f (($type.summary -replace "\s+", " ").Trim())))
  }

  Append-MemberList -Lines $lines -Title "Constructors" -Items @($type.constructors) -Limit 20
  Append-MemberList -Lines $lines -Title "Properties" -Items @($type.properties) -Limit 120
  Append-MemberList -Lines $lines -Title "Methods" -Items @($type.methods) -Limit 120
  Append-MemberList -Lines $lines -Title "Enum Values" -Items @($type.enumValues) -Limit 220

  return ($lines -join [Environment]::NewLine)
}

function Write-KnowledgeFile {
  param(
    [string]$FileName,
    [string]$Title,
    [string]$Intro,
    [string[]]$TypeNames
  )

  $lines = [System.Collections.Generic.List[string]]::new()
  $lines.Add("# $Title")
  $lines.Add("")
  $lines.Add($Intro)
  $lines.Add("")
  $lines.Add("Snapshot: $($catalog.snapshotDate)")
  $lines.Add("SHVDN version: $($catalog.shvdnVersion)")
  $lines.Add("")
  $lines.Add("Use this as Dify Knowledge Base context. Do not paste the full source JSON into prompts.")

  foreach ($typeName in $TypeNames) {
    $lines.Add("")
    $lines.Add((New-TypeSection $typeName))
  }

  Write-Utf8NoBom -Path (Join-Path $outputDir $FileName) -Text ($lines -join [Environment]::NewLine)
}

$readme = @'
# Dify SHVDN Knowledge Pack

This folder is generated from the repository source snapshot in `templates/live-studio/references/shvdn/`.

Import these Markdown files into a Dify Knowledge Base such as `GTA5 SHVDN v3 Reference`.

Recommended imports:

- `shvdn-core-rules.md`
- `shvdn-domain-index.md`
- `shvdn-world-ui.md`
- `shvdn-vehicles.md`
- `shvdn-peds.md`

Do not assume Dify can read repository files directly. Dify LLM nodes should use the Knowledge Base retrieval output as their SHVDN context.
'@
$readme = $readme + [Environment]::NewLine + [Environment]::NewLine + "Snapshot: $($catalog.snapshotDate)" + [Environment]::NewLine + "SHVDN version: $($catalog.shvdnVersion)" + [Environment]::NewLine
Write-Utf8NoBom -Path (Join-Path $outputDir "README.md") -Text $readme

$coreRules = @'
# SHVDN Core Rules for Dify

Use these rules in `Mod.cs Generator` and `QA Fixer`.

## Runtime

- Target ScriptHookVDotNet v3.
- Target C# 7.3 and .NET Framework 4.8.
- Generate only `ModProject/GeneratedGameplay.cs`; do not regenerate the fixed LIVE Studio template.
- The fixed template v3 dispatches all generated hooks on the GTA main thread. Call SHVDN APIs directly inside hooks; do not create dispatchers, tasks, threads, or timers.

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
'@
Write-Utf8NoBom -Path (Join-Path $outputDir "shvdn-core-rules.md") -Text $coreRules

$domainLines = [System.Collections.Generic.List[string]]::new()
$domainLines.Add("# SHVDN Domain Index")
$domainLines.Add("")
$domainLines.Add("Snapshot: $($catalog.snapshotDate)")
$domainLines.Add("SHVDN version: $($catalog.shvdnVersion)")
$domainLines.Add("")
$domainLines.Add("Use this file to orient retrieval. For detailed members, retrieve the domain-specific files.")
foreach ($domain in $catalog.domains) {
  $domainLines.Add("")
  $domainLines.Add("## $($domain.name)")
  foreach ($typeName in $domain.typeNames) {
    $domainLines.Add(("- {0}" -f $typeName))
  }
}
Write-Utf8NoBom -Path (Join-Path $outputDir "shvdn-domain-index.md") -Text ($domainLines -join [Environment]::NewLine)

Write-KnowledgeFile `
  -FileName "shvdn-world-ui.md" `
  -Title "SHVDN World, UI, Native, and Model APIs" `
  -Intro "High-frequency APIs for notifications, subtitles, world state, spawning, native calls, and model loading." `
  -TypeNames @(
    "GTA.UI.Notification",
    "GTA.UI.Screen",
    "GTA.World",
    "GTA.Weather",
    "GTA.Model",
    "GTA.Native.Function",
    "GTA.Native.Hash",
    "GTA.ExplosionType"
  )

Write-KnowledgeFile `
  -FileName "shvdn-vehicles.md" `
  -Title "SHVDN Vehicle APIs" `
  -Intro "Vehicle APIs and common enums for vehicle-based LIVE Studio effects." `
  -TypeNames @(
    "GTA.Vehicle",
    "GTA.VehicleHash",
    "GTA.VehicleSeat",
    "GTA.VehicleDoorIndex",
    "GTA.VehicleClass"
  )

Write-KnowledgeFile `
  -FileName "shvdn-peds.md" `
  -Title "SHVDN Player, Ped, Weapon, and Task APIs" `
  -Intro "Player, ped, weapon, model hash, and task APIs for NPC spawning, healing, combat, and follow behavior." `
  -TypeNames @(
    "GTA.Player",
    "GTA.Ped",
    "GTA.PedHash",
    "GTA.WeaponHash",
    "GTA.WeaponCollection",
    "GTA.TaskInvoker",
    "GTA.RelationshipGroup",
    "GTA.Relationship"
  )

$bundleParts = @(
  "README.md",
  "shvdn-core-rules.md",
  "shvdn-domain-index.md",
  "shvdn-world-ui.md",
  "shvdn-vehicles.md",
  "shvdn-peds.md"
)

$bundleLines = [System.Collections.Generic.List[string]]::new()
$bundleLines.Add("# GTA5 SHVDN v3 Reference for Dify")
$bundleLines.Add("")
$bundleLines.Add("This is the single-file import bundle generated from the repository SHVDN snapshot.")
$bundleLines.Add("")
foreach ($part in $bundleParts) {
  $bundleLines.Add("")
  $bundleLines.Add("---")
  $bundleLines.Add("")
  $bundleLines.Add((Get-Content -LiteralPath (Join-Path $outputDir $part) -Raw))
}
Write-Utf8NoBom -Path (Join-Path $outputDir "shvdn-dify-knowledge.md") -Text ($bundleLines -join [Environment]::NewLine)

$requiredTerms = @(
  "GTA.UI.Notification",
  "GTA.UI.Screen",
  "GTA.World",
  "GTA.Vehicle",
  "GTA.Ped",
  "GTA.VehicleHash",
  "GTA.WeaponHash"
)

$combined = Get-ChildItem -LiteralPath $outputDir -Filter "*.md" | ForEach-Object {
  Get-Content -LiteralPath $_.FullName -Raw
} | Out-String

foreach ($term in $requiredTerms) {
  if ($combined -notmatch [regex]::Escape($term)) {
    throw "Generated Dify knowledge pack is missing required term: $term"
  }
}

Write-Host "Dify SHVDN knowledge pack generated at $outputDir"
