param(
  [Parameter(Mandatory = $true)]
  [string]$Path
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) {
  throw "Generated gameplay file is missing: $Path"
}

$source = Get-Content -LiteralPath $Path -Raw
if ([string]::IsNullOrWhiteSpace($source)) {
  throw "Generated gameplay source is empty."
}
if ([System.Text.Encoding]::UTF8.GetByteCount($source) -gt 120000) {
  throw "Generated gameplay source exceeds the 120 KB limit."
}

$required = [ordered]@{
  'using\s+ModProject\.LiveStudio\s*;' = 'Generated gameplay must import ModProject.LiveStudio.'
  'namespace\s+ModProject\b' = 'Generated gameplay must use namespace ModProject.'
  'public\s+partial\s+class\s+Mod\b' = 'Generated gameplay must declare public partial class Mod.'
  'partial\s+void\s+On(Chat|Gift)\s*\(' = 'Generated gameplay must implement at least one LIVE Studio gameplay hook.'
}

foreach ($entry in $required.GetEnumerator()) {
  if ($source -notmatch $entry.Key) {
    throw $entry.Value
  }
}

$forbidden = [ordered]@{
  '<think>|```' = 'Generated gameplay contains reasoning or Markdown fences.'
  'class\s+(LiveStudioClient|LiveStudioParser|LiveStudioEvent|MainThreadDispatcher)\b' = 'Generated gameplay must not redeclare fixed LIVE Studio runtime types.'
  '\b(ClientWebSocket|JavaScriptSerializer)\b' = 'Generated gameplay must not implement transport or JSON parsing.'
  'ws://127\.0\.0\.1:60080|serviceSignalSub|IM_MESSAGE_TRANSPORT' = 'Generated gameplay must not contain LIVE Studio wire protocol details.'
  'public\s+Mod\s*\(' = 'Generated gameplay must not replace the fixed Mod constructor.'
  'class\s+Mod\s*:\s*Script' = 'Generated gameplay must not replace the fixed Script lifecycle shell.'
  '\bHandleEvent\s*\(' = 'Generated gameplay must not replace fixed event routing.'
  'Newtonsoft\.Json|System\.Text\.Json' = 'Generated gameplay must not add a JSON library.'
}

foreach ($entry in $forbidden.GetEnumerator()) {
  if ($source -match $entry.Key) {
    throw $entry.Value
  }
}

Write-Host "Generated gameplay boundary validation passed."
