$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot

function Assert-FileExists {
  param([string]$Path)
  if (-not (Test-Path -LiteralPath (Join-Path $repoRoot $Path) -PathType Leaf)) {
    throw "Missing required file: $Path"
  }
}

function Assert-DirectoryExists {
  param([string]$Path)
  if (-not (Test-Path -LiteralPath (Join-Path $repoRoot $Path) -PathType Container)) {
    throw "Missing required directory: $Path"
  }
}

function Assert-Contains {
  param(
    [string]$Path,
    [string]$Pattern,
    [string]$Message
  )
  $fullPath = Join-Path $repoRoot $Path
  $text = Get-Content -LiteralPath $fullPath -Raw
  if ($text -notmatch $Pattern) {
    throw $Message
  }
}

function Assert-NotContains {
  param(
    [string]$Path,
    [string]$Pattern,
    [string]$Message
  )
  $fullPath = Join-Path $repoRoot $Path
  $text = Get-Content -LiteralPath $fullPath -Raw
  if ($text -match $Pattern) {
    throw $Message
  }
}

Assert-DirectoryExists "templates/live-studio"
Assert-FileExists "templates/live-studio/DESIGN.md"
Assert-FileExists "templates/live-studio/Directory.Build.props"
Assert-FileExists "templates/live-studio/scripts/build-mod.ps1"
Assert-FileExists "templates/live-studio/ModProject/ModProject.csproj"
Assert-FileExists "templates/live-studio/ModProject/Mod.cs"
Assert-FileExists "templates/live-studio/ModProject/LiveStudio/LiveStudioClient.cs"
Assert-FileExists "templates/live-studio/ModProject/LiveStudio/LiveStudioParser.cs"
Assert-FileExists "templates/live-studio/ModProject/LiveStudio/LiveStudioEvents.cs"
Assert-FileExists "templates/live-studio/ModProject/LiveStudio/MainThreadDispatcher.cs"
Assert-FileExists "templates/live-studio/references/shvdn/INDEX.md"
Assert-FileExists "templates/live-studio/references/shvdn/README.md"
Assert-FileExists "templates/live-studio/references/shvdn/capabilities.json"
Assert-FileExists "scripts/prepare-dify-shvdn-knowledge.ps1"
Assert-DirectoryExists "docs/dify-knowledge/shvdn"
Assert-FileExists "docs/dify-knowledge/shvdn/README.md"
Assert-FileExists "docs/dify-knowledge/shvdn/shvdn-core-rules.md"
Assert-FileExists "docs/dify-knowledge/shvdn/shvdn-domain-index.md"
Assert-FileExists "docs/dify-knowledge/shvdn/shvdn-vehicles.md"
Assert-FileExists "docs/dify-knowledge/shvdn/shvdn-peds.md"
Assert-FileExists "docs/dify-knowledge/shvdn/shvdn-world-ui.md"
Assert-FileExists "docs/dify-knowledge/shvdn/shvdn-dify-knowledge.md"

Assert-Contains ".github/workflows/build-mod.yml" "assembly_name:" "workflow_dispatch must accept assembly_name."
Assert-Contains ".github/workflows/build-mod.yml" "design_md_b64:" "workflow_dispatch must accept design_md_b64."
Assert-Contains ".github/workflows/build-mod.yml" "mod_cs_b64:" "workflow_dispatch must accept mod_cs_b64."
Assert-Contains ".github/workflows/build-mod.yml" "templates/live-studio" "Workflow must copy the live-studio template."
Assert-Contains ".github/workflows/build-mod.yml" "ModProject/Mod.cs" "Workflow must write generated Mod.cs into the template."
Assert-Contains ".github/workflows/build-mod.yml" "\^\[A-Za-z0-9\]\[A-Za-z0-9_-\]" "build_id validation must reject path-like dot segments."
Assert-NotContains ".github/workflows/build-mod.yml" "source_b64:" "Legacy single-file source_b64 input must be removed."
Assert-NotContains ".github/workflows/build-mod.yml" "file_name:" "Legacy single-file file_name input must be removed."

Assert-Contains "templates/live-studio/ModProject/Mod.cs" "onLog:\s*msg\s*=>\s*MainThreadDispatcher\.Enqueue" "LIVE Studio log callbacks must be marshalled to the main thread."
Assert-Contains "templates/live-studio/ModProject/LiveStudio/LiveStudioClient.cs" "Semaphore" "LiveStudioClient process guard should use Semaphore instead of thread-affine Mutex."
Assert-NotContains "templates/live-studio/ModProject/LiveStudio/LiveStudioClient.cs" "Mutex" "LiveStudioClient must not use thread-affine Mutex for async process guard."

Assert-Contains "pages/status.html" "design_md" "Status page should display DESIGN.md content or metadata."
Assert-Contains "pages/status.html" "mod_cs" "Status page should expose generated Mod.cs content or metadata."
Assert-Contains "docs/dify-integration.md" "design_md_b64" "Dify integration docs must describe design_md_b64."
Assert-Contains "docs/dify-integration.md" "mod_cs_b64" "Dify integration docs must describe mod_cs_b64."
Assert-Contains "docs/dify-live-studio-integration.md" "Dify Knowledge Base" "Dify docs must say SHVDN context comes from Dify Knowledge Base."
Assert-NotContains "docs/dify-live-studio-integration.md" "verify it against `templates/live-studio/references/shvdn" "Dify docs must not imply LLM nodes can directly read repository paths."
Assert-Contains "README.md" "LIVE Studio" "README must describe the LIVE Studio builder."
Assert-Contains "README.md" "references/shvdn" "README must mention the bundled SHVDN reference snapshot."
Assert-Contains "README.md" "docs/dify-knowledge/shvdn" "README must mention the Dify-importable SHVDN knowledge pack."
Assert-FileExists ".gitignore"
Assert-Contains ".gitignore" "\*\*/bin/" ".gitignore must ignore .NET bin directories."
Assert-Contains ".gitignore" "\*\*/obj/" ".gitignore must ignore .NET obj directories."
Assert-Contains ".gitignore" "\*\*/dist/" ".gitignore must ignore local template dist outputs."

Write-Host "LIVE Studio builder validation passed."
