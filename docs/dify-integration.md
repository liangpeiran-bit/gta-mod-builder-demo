# Dify Integration

Use this repository as a no-backend LIVE Studio mod compiler:

```text
Dify -> DESIGN.md + GeneratedGameplay.cs -> GitHub workflow_dispatch -> GitHub Actions -> gh-pages -> public download link
```

This builder only supports the LIVE Studio template. Dify must not send a standalone `.cs` script; it must send:

- `DESIGN.md`: the binding product/gameplay spec.
- `GeneratedGameplay.cs`: only `ModProject/GeneratedGameplay.cs` partial gameplay logic that uses the fixed LIVE Studio template.

## Dify Code Node

Input variables:

- `design_md`: output from `DESIGN.md Generator`.
- `mod_cs`: output from `Mod.cs Generator` or `QA Fixer`.
- `assembly_name`: user-visible assembly name, such as `LiveStudioChaos`.

Python code:

```python
import base64
import re
import time
import uuid

def _clean_assembly_name(value: str) -> str:
    value = (value or "LiveStudioMod").strip()
    value = re.sub(r"[^A-Za-z0-9._-]", "_", value)
    value = value.strip("._-")
    return value or "LiveStudioMod"

def _strip_reasoning(text: str) -> str:
    text = text or ""
    text = re.sub(r"<think>[\s\S]*?</think>", "", text, flags=re.IGNORECASE)
    if "<think>" in text.lower():
        fence = re.search(r"```", text)
        if fence:
            text = text[fence.start():]
    return text.strip()

def _extract_design_md(text: str) -> str:
    text = _strip_reasoning(text)
    fences = re.findall(r"```(?:markdown|md)?\s*([\s\S]*?)```", text, flags=re.IGNORECASE)
    if fences:
        return fences[-1].strip()
    return text.strip()

def _extract_mod_cs(text: str) -> str:
    text = _strip_reasoning(text)
    csharp_fences = re.findall(r"```(?:csharp|cs|c#)\s*([\s\S]*?)```", text, flags=re.IGNORECASE)
    if csharp_fences:
        source = csharp_fences[-1]
    else:
        fences = re.findall(r"```(?:[A-Za-z0-9_-]+)?\s*([\s\S]*?)```", text)
        code_like = [f for f in fences if "public class" in f or "namespace ModProject" in f or "using GTA" in f]
        source = code_like[-1] if code_like else text

    source = source.replace("\r\n", "\n").strip()
    starts = [idx for idx in [
        source.find("using System"),
        source.find("using GTA"),
        source.find("namespace ModProject"),
        source.find("public partial class Mod"),
    ] if idx >= 0]
    if starts:
        source = source[min(starts):]
    source = re.split(r"\n#{2,}\s", source, maxsplit=1)[0].strip()
    return source

def main(design_md: str, mod_cs: str, assembly_name: str = "LiveStudioMod") -> dict:
    build_id = f"dify-{time.strftime('%Y%m%d%H%M%S')}-{uuid.uuid4().hex[:8]}"
    clean_name = _clean_assembly_name(assembly_name)
    design = _extract_design_md(design_md)
    source = _extract_mod_cs(mod_cs)

    return {
        "build_id": build_id,
        "assembly_name": clean_name,
        "design_md_b64": base64.b64encode(design.encode("utf-8")).decode("ascii"),
        "mod_cs_b64": base64.b64encode(source.encode("utf-8")).decode("ascii"),
        "download_page": f"https://liangpeiran-bit.github.io/gta-mod-builder-demo/status.html?id={build_id}",
    }
```

## Dify HTTP Request Node

Method:

```text
POST
```

URL:

```text
https://api.github.com/repos/liangpeiran-bit/gta-mod-builder-demo/actions/workflows/build-mod.yml/dispatches
```

Headers:

```text
Authorization: Bearer {{ GITHUB_TOKEN }}
Accept: application/vnd.github+json
X-GitHub-Api-Version: 2022-11-28
Content-Type: application/json
```

Body:

```json
{
  "ref": "main",
  "inputs": {
    "build_id": "{{#code.build_id#}}",
    "assembly_name": "{{#code.assembly_name#}}",
    "design_md_b64": "{{#code.design_md_b64#}}",
    "mod_cs_b64": "{{#code.mod_cs_b64#}}",
    "template_contract_version": "live-studio-template-3"
  }
}
```

## Dify Answer Node

Return the source summary and download page immediately:

```text
LIVE Studio DLL build submitted.

Download page:
{{#code.download_page#}}

The build usually takes 1-3 minutes. Keep the page open until the download button appears. If the build fails, the page shows the compiler log and the generated DESIGN.md / GeneratedGameplay.cs.
```

## GitHub Token

For the Dify HTTP node, create a GitHub fine-grained token or classic PAT with access to this repository and permission to run Actions workflows. Store it as a Dify environment variable named `GITHUB_TOKEN`.

Do not expose this token in the final answer.
