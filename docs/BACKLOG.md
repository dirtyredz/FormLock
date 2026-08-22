# BACKLOG — Form Lock

Prioritized trough. P0 = blocker, P1 = should-fix, P2 = nice-to-have. Structural items feed
[../STRUCTURE.md](../STRUCTURE.md); testing items track [../TESTING.md](../TESTING.md) /
[../RELEASING.md](../RELEASING.md).

## P0 — blockers
_None._

## P1 — should-fix

- **Finish or shelve the WIP pickup-stutter feature (Feature 2).** It's committed but unreleased
  (`PickupStutterPatches`). Before it ships it needs: a real in-game confirmation the stutter is
  gone, and its own CHANGELOG + version bump. Until then it rides along in the DLL, on no config
  path unless `ApplyToPickup`.
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
- **Hand-test Bat & Aqua forms** — pickup + harvest, both untested in a real session. Same code
  path as Cat, but unverified. _(From [../TESTING.md](../TESTING.md).)_
- **Verify each config toggle gates its own behaviour**; fresh-install defaults + Mod Menu render;
  multiple pickups/harvests in a row; save-diff after a normal session. _(From TESTING.md.)_

## Known cosmetic (won't-fix, documented)

- The pickup/harvest animation trigger still fires on the form body's Animator, which has no clip
  wired for it, so Unity ignores it — no animation, no error. Item still lands in inventory.
