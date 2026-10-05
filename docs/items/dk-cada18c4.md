---
id: dk-cada18c4
type: feature
created: 2026-10-05
status: todo
since: 2026-10-05
area:
priority: P1
rank: n
parent:
fixes: []
blocked_by: []
relates: []
---
# Ship pickup-stutter feature: CHANGELOG entry and version bump

_Moved from docs/BACKLOG.md (P1)._

**Ship the pickup-stutter feature (Feature 2).** ✅ **Confirmed working in-game 2026-08-22** (Cat
Form smooth, log lines present; see ../TESTING.md). Remaining before release:
a CHANGELOG entry + `<Version>` bump in `src/FormLock.csproj` (e.g. 1.1.0) — **not yet authorised**;
the original ask was explicitly "do not publish it." Until released it rides along in the DLL, on
no config path unless `ApplyToPickup`.

- ✅ **Resolved (2026-08-22, decompile analysis):** harvest does **not** want the same treatment.
  The activation-time halt comes from `BasePlayerState.OnActivate`'s
  `if (!isInputAllowed) InputBlocker.Add("BasePlayerState")`; `PlayerHarvestState` overrides
  `isInputAllowed => true` so that Add never runs and there is no walk-through halt to suppress.
  Pickup-only scope is correct. See the `PickupStutterPatches` class doc and DECISIONS.md.
