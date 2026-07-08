# Dify Integration

Use this repository as a no-backend LIVE Studio mod compiler:

```text
Dify -> DESIGN.md + Mod.cs -> GitHub workflow_dispatch -> GitHub Actions -> gh-pages -> public download link
```

This builder only supports the LIVE Studio template. Dify must not send a standalone `.cs` script; it must send:

- `DESIGN.md`: the binding product/gameplay spec.
- `Mod.cs`: only `ModProject/Mod.cs` business logic that uses the fixed LIVE Studio template.

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

def _extract_mod_cs(text: str) -> str:
    match = re.search(r"```(?:csharp|cs)?\\s*([\\s\\S]*?)```", text or "", re.IGNORECASE)
    return (match.group(1) if match else text or "").strip()

def main(design_md: str, mod_cs: str, assembly_name: str = "LiveStudioMod") -> dict:
    build_id = f"dify-{time.strftime('%Y%m%d%H%M%S')}-{uuid.uuid4().hex[:8]}"
    clean_name = _clean_assembly_name(assembly_name)
    design = (design_md or "").strip()
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
    "mod_cs_b64": "{{#code.mod_cs_b64#}}"
  }
}
```

## Dify Answer Node

Return the source summary and download page immediately:

```text
LIVE Studio DLL build submitted.

Download page:
{{#code.download_page#}}

The build usually takes 1-3 minutes. Keep the page open until the download button appears. If the build fails, the page shows the compiler log and the generated DESIGN.md / Mod.cs.
```

## GitHub Token

For the Dify HTTP node, create a GitHub fine-grained token or classic PAT with access to this repository and permission to run Actions workflows. Store it as a Dify environment variable named `GITHUB_TOKEN`.

Do not expose this token in the final answer.
