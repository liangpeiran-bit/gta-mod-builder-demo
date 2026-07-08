# SHVDN Full API Snapshot

This folder contains the complete ScriptHookVDotNet v3 API snapshot used by the workspace query script.

Files:

- `INDEX.md`: human-readable first-pass index.
- `capabilities.json`: full machine-readable API catalog.

Use the PowerShell query script instead of reading `capabilities.json` directly:

```powershell
.\scripts\query-shvdn-api.ps1 type Vehicle
.\scripts\query-shvdn-api.ps1 enumName VehicleHash -Filter Krieger
.\scripts\query-shvdn-api.ps1 memberMatch "CreateVehicle"
.\scripts\query-shvdn-api.ps1 namespace GTA.UI
```

The catalog is large. Read `INDEX.md` for orientation, then query targeted types, enum values, or members.
