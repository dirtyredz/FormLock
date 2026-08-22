# DECISIONS — Form Lock

Why the mod is shaped the way it is. Newest first. Workspace-wide conventions live in
[../../../docs/DECISIONS.md](../../docs/DECISIONS.md).

## 2026-08-22 — Scope the pickup-stutter fix to pickup only, not harvest

**Decision.** `PickupStutterPatches` gates on `PlayerPickupState` alone; harvest is intentionally
left untouched.
**Why.** Decompiling `BasePlayerState`/`PlayerHarvestState` showed the stutter's root cause — the
activation-time `input.InputBlocker.Add("BasePlayerState")` → `StopMove()` velocity-zero — only runs
`if (!isInputAllowed)`. `PlayerHarvestState` overrides `isInputAllowed => true`, so that Add never
fires on activation; harvest has no walk-through halt to suppress.
**Rejected.** Mirroring the patches onto `PlayerHarvestState` — it would suppress a halt harvest
doesn't have, and harvest's timer fields (`harvestAfter`/`harvestLength`) differ from pickup's anyway.

## 2026-08-22 — Split patches by feature; extract a shared form-protection policy

**Decision.** Move from one `FormPatches.cs` to `FormRetentionPatches.cs` + `PickupStutterPatches.cs`,
with a `FormProtection` policy class holding the shared "is this a protected grabbed form?" predicate
and the form-subtype→config map.
**Why.** The full structural review (componentization + abstraction + Codex) found the file had
accreted a second, independently-evolving feature and copy-pasted its guard four times.
**Rejected.** (a) One file per Harmony target — needless fragmentation for a mod this small.
(b) A config/DI abstraction over `FormLockPlugin` statics — over-engineering at 7 entries.
(c) Extracting the `PlayerView` state-fetch helper — only ~2 uses, kept inline.

## 2026-08-12 — Ship 1.0.0 with form retention only

**Decision.** First release is `1.0.0` (not a preview), scoped to pickup + bare-hand harvest across
all three forms.
**Why.** Feature-complete for its stated scope; the one hand-tested behaviour (Cat form) works.
Bat/Aqua share the exact code path (`FormToolAsset` matched generically, not per-subtype).

## Single-source the version from the csproj

**Decision.** `[BepInPlugin]` version = `ModBuildInfo.Version`, a compile-time constant generated
from `<Version>` in `FormLock.csproj`; never a hardcoded string in `Plugin.cs`.
**Why.** The archive name (`pack.ps1`) and the version BepInEx reports can never drift apart.
See workspace `12-versioning-and-release.md`.

## Read-only, fail-safe patching

**Decision.** Never write to saves; wrap every patch body in try/catch that falls back to vanilla on
any exception; return vanilla behaviour for unrecognised form subtypes.
**Why.** A form-keeping mod must never strip a form unexpectedly or wedge the pickup/harvest flow,
and must be safe to add to or remove from an existing save.

## No test project

**Decision.** Verification is manual (see [../TESTING.md](../TESTING.md)); no unit-test project.
**Why.** Every code path reads live Unity/game types (`GameInventory`, `PlayerPickupState`,
`FormToolAsset`), so a console runner outside the game could not exercise anything real.
