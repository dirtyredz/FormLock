# Releasing Form Lock

The shared rules live in [12-versioning-and-release.md](../../12-versioning-and-release.md).
This file is what is specific to this mod, and where it currently stands.

## Packaging

```powershell
.\mods\FormLock\pack.ps1
```

Produces `dist/FormLock-<version>.zip` laid out as Nexus and Vortex expect:

```
BepInEx/plugins/FormLock/FormLock.dll
```

Note that is **not** the dev deploy path. `dotnet build` deploys to
`plugins/MoonlightPeaksMods/FormLock/` to keep hand-built DLLs clear of Vortex; players get the
plain `plugins/FormLock/` layout. `pack.ps1` builds with `SkipDeploy=true`, so packaging never
overwrites the copy under test.

The script reads the version from the csproj; `Plugin.cs` derives the same value at build time via
`ModBuildInfo.Version`, so the archive name and the version the DLL reports can never disagree.

## No test project, on purpose

The entire mod is the shipped form-retention prefix (`FormRetentionPatches.cs`), the WIP
pickup-stutter patches (`PickupStutterPatches.cs`), a shared `FormProtection` policy, and its
BepInEx config (`Plugin.cs`). Every code path reads Unity and game types — `GameInventory`,
`PlayerPickupState`, `PlayerHarvestState`, `FormToolAsset` — so a console runner outside the game
could not exercise anything real. Verification is manual — see [TESTING.md](TESTING.md).

## Pre-release checklist

Automated/self-evident checks, verified for 1.0.0:

- [x] **Version set in the csproj** — `Plugin.cs` derives it via `ModBuildInfo.Version`
- [x] **CHANGELOG** has exactly one entry for this version
- [x] **Diagnostics off** — `VerboseLogging` defaults to `false`
- [x] **Save-safe** — no writes to `GameData.json`. The patch only skips a call
      (`GameInventory.TryGrabNone`); it never adds, removes or renames a save field. Confirmed
      by reading the field the fix touches, not just by reading the patch.
- [x] **No stray debug code shipped** — the testing-only `GiveFormsKey` hotkey and its
      `GiveTestForms`/`GiveForm<T>` methods, added to grant unowned forms on a test save, were
      removed from `Plugin.cs` before this release. `git diff`/file read confirms `Plugin.cs`
      only defines the seven real settings.

Confirmed by hand so far:

- [x] **Cat Form** — confirmed in-game: an item picked up while in Cat Form stays in Cat Form,
      for both a loose-ground pickup and a bare-hand harvest.

Still to do by hand before publishing:

- [ ] **Bat Form / Aqua Form** — same code path as Cat Form (`FormToolAsset` is matched
      generically, not per-subtype, in `FormProtection`), but neither has actually been tried in a
      real session yet. Worth at least one pickup and one harvest per form before claiming it on
      the page.
- [ ] **Fresh install** — delete `com.dirtyredz.moonlightpeaks.formlock.cfg`, launch, confirm
      the defaults are sensible and Mod Menu renders every description without overflowing.
- [ ] **Toggles actually gate the behaviour** — flip `ApplyToPickup`, `ApplyToHarvest`, and each
      `Keep*Form` off one at a time and confirm vanilla unequip-on-pickup/harvest comes back for
      exactly the thing that was turned off, not the others.
- [ ] **`Enabled = false`** restores stock behaviour with the mod otherwise still loaded.
- [ ] **Screenshots** — none exist yet. See the shot list in [NEXUS.md](NEXUS.md).
- [ ] **Save verified untouched** — back up a save, play a session picking things up and
      harvesting in form, confirm no diff beyond normal play (per
      [11-mod-data-and-saves.md](../../11-mod-data-and-saves.md)).
- [ ] Install the packed zip on a clean BepInEx and confirm it loads from `plugins/FormLock/`,
      not just the dev path.

## Open decisions

- **Version 1.0.0.** The mod is feature-complete for its stated scope (pickup + bare-hand
  harvest, all three forms) and the one behaviour that has been hand-tested works, so this is a
  first release rather than a preview.
- **Scope.** Deliberately does not touch tool-gated harvesting (chopping, mining) — those
  interactions are already unreachable while a form tool is grabbed, so there is nothing for
  this patch to change there. See the README for why.
