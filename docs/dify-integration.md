# Dify Integration

Use this repository as a no-backend demo compiler:

```text
Dify -> GitHub workflow_dispatch -> GitHub Actions -> gh-pages -> public download link
```

## Dify Code Node

Input variable:

- `text`: final LLM answer that contains one fenced C# code block.

Python code:

```python
import base64
import re
import time
import uuid

def main(text: str) -> dict:
    match = re.search(r"```(?:csharp|cs)?\\s*([\\s\\S]*?)```", text or "", re.IGNORECASE)
    source = (match.group(1) if match else text or "").strip()

    class_match = re.search(r"public\\s+class\\s+([A-Za-z_][A-Za-z0-9_]*)", source)
    mod_name = class_match.group(1) if class_match else "GeneratedMod"
    file_name = f"{mod_name}.cs"

    build_id = f"{int(time.time())}-{uuid.uuid4().hex[:8]}"
    source_b64 = base64.b64encode(source.encode("utf-8")).decode("ascii")

    return {
        "build_id": build_id,
        "file_name": file_name,
        "source_b64": source_b64,
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
    "file_name": "{{#code.file_name#}}",
    "source_b64": "{{#code.source_b64#}}"
  }
}
```

## Dify Answer Node

Return the download page immediately:

```text
DLL build submitted.

Download page:
{{#code.download_page#}}

The build usually takes 1-3 minutes. Keep the page open until the download button appears.
```

## GitHub Token

For the Dify HTTP node, create a GitHub fine-grained token or classic PAT with access to this repository and permission to run Actions workflows. Store it as a Dify environment variable named `GITHUB_TOKEN`.

Do not expose this token in the final answer.
