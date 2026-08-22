# BACKLOG — Form Lock

Prioritized trough. P0 = blocker, P1 = should-fix, P2 = nice-to-have. Structural items feed
[../STRUCTURE.md](../STRUCTURE.md); testing items track [../TESTING.md](../TESTING.md) /
[../RELEASING.md](../RELEASING.md).

## P0 — blockers
_None._

## P1 — should-fix

- **Ship the pickup-stutter feature (Feature 2).** ✅ **Confirmed working in-game 2026-08-22** (Cat
  Form smooth, log lines present; see [../TESTING.md](../TESTING.md)). Remaining before release:
  a CHANGELOG entry + `<Version>` bump in `src/FormLock.csproj` (e.g. 1.1.0) — **not yet authorised**;
  the original ask was explicitly "do not publish it." Until released it rides along in the DLL, on
  no config path unless `ApplyToPickup`.
  - ✅ **Resolved (2026-08-22, decompile analysis):** harvest does **not** want the same treatment.
    The activation-time halt comes from `BasePlayerState.OnActivate`'s
    `if (!isInputAllowed) InputBlocker.Add("BasePlayerState")`; `PlayerHarvestState` overrides
    `isInputAllowed => true` so that Add never runs and there is no walk-through halt to suppress.
    Pickup-only scope is correct. See the `PickupStutterPatches` class doc and [DECISIONS.md](DECISIONS.md).

## P2 — nice-to-have

- **`PlayerView` current-state fetch appears ~2×** with slightly different shapes
  (`FormRetentionPatches` builds `CurrentState`; `PickupStutterPatches.StopMove` re-tests it).
  Reviewers split on whether a helper earns its keep at 2 uses; deferred. Extract only if a third
  same-shape use appears. _(Source: 2026-08-22 structural review.)_
- **Config read as `FormLockPlugin` statics from the patches.** Fine at 7 entries; if the config
  set grows materially, extract a `Config` holder instead of widening the coupling. _(2026-08-22.)_
- ✅ **Hand-test Bat & Aqua forms** — pickup + harvest — done 2026-08-22 (owner test round).
- ✅ **Config toggles gate their own behaviour** — verified 2026-08-22. Still open from the same
  list: **fresh-install defaults + Mod Menu render**, and a **save-diff after a normal session**.
  _(From [../TESTING.md](../TESTING.md).)_

## Known cosmetic (won't-fix, documented)

- The pickup/harvest animation trigger still fires on the form body's Animator, which has no clip
  wired for it, so Unity ignores it — no animation, no error. Item still lands in inventory.
