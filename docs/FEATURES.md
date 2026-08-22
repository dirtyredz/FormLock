# FEATURES — Form Lock

What the mod does. The why is in [DECISIONS.md](DECISIONS.md); how, in [ARCHITECTURE.md](ARCHITECTURE.md).

## Capabilities

| Feature | Status | Notes |
|---------|--------|-------|
| Keep Cat/Bat/Aqua form through **loose-item pickups** | ✅ Shipped 1.0.0 | Cat confirmed in-game; Bat/Aqua share the code path, hand-test pending |
| Keep form through **bare-hand harvests** (bushes, produce, no-tool nodes) | ✅ Shipped 1.0.0 | Cat confirmed in-game |
| Suppress the **movement stutter** when picking up in form | 🚧 WIP (committed, unreleased) | Feature 2; not part of the 1.0.0 release |
| Per-flow + per-form toggles, master switch, verbose logging | ✅ Shipped 1.0.0 | 7 config entries below |

## Explicitly out of scope

- **Tool-gated actions** (chopping trees, mining rocks). They already can't be targeted while a form
  tool is grabbed instead of the required tool — `PlayerInteractor.HasCorrectToolForInteractable`
  filters them out — so there is no unequip for this mod to intervene on. Unchanged either way.

## Config entries (`.cfg` sections → keys)

| Section | Key | Default | Effect |
|---------|-----|---------|--------|
| General | `Enabled` | `true` | Master switch; off = full vanilla behaviour |
| General | `ApplyToPickup` | `true` | Keep form while picking loose items off the ground |
| General | `ApplyToHarvest` | `true` | Keep form while harvesting bare-hand nodes |
| Forms | `KeepCatForm` | `true` | Apply to Cat form |
| Forms | `KeepBatForm` | `true` | Apply to Bat form (skips its unequip-time land/water repositioning) |
| Forms | `KeepAquaForm` | `true` | Apply to Aqua form (skips its unequip-time water repositioning) |
| Diagnostics | `VerboseLogging` | `false` | Log each time a form is kept on |

All editable in-game via Mod Menu (display titles carried in `ConfigDescription.Tags`) or by hand
in `com.dirtyredz.moonlightpeaks.formlock.cfg`.
