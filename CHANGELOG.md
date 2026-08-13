# Changelog

## 1.0.0 — 2026-08-12

First release. Confirmed working in-game: staying in Cat Form through both a loose-item pickup
and a bare-hand harvest, on a dedicated test save.

Keeps Cat/Bat/Aqua form equipped through item pickups and bare-hand harvests, instead of the
game silently kicking you back to human form to do either.

- Root cause: `PlayerPickupState.OnActivate()` and `PlayerHarvestState.OnActivate()` both
  unconditionally call `GameInventory.TryGrabNone()` to free your hands for the animation, and
  `GameInventory.TryGrabPreviousGrabbedItem()` explicitly refuses to re-equip a Form-type tool
  afterwards — so every pickup or harvest permanently dropped you back to human form, with no
  prompt and no way back in except the tool wheel.
- Fix is a single Harmony Prefix on `GameInventory.TryGrabNone`: skip the unequip entirely when
  the grabbed item is a form tool and the active state is `PlayerPickupState` or
  `PlayerHarvestState`. `TryGrabPreviousGrabbedItem`'s own "already holding something" check
  then makes the later restore call on deactivate a no-op, so nothing else had to change.
- Covers loose ground items and bare-hand harvest nodes (bushes, produce). Tool-gated actions —
  chopping a tree, mining a rock — are unaffected either way: they already can't be targeted
  while a form tool is grabbed instead of the required tool, so there was never an unequip on
  that path to intervene on.
- Configurable per-flow (`ApplyToPickup`, `ApplyToHarvest`) and per-form (`KeepCatForm`,
  `KeepBatForm`, `KeepAquaForm`), all on by default. Master `Enabled` switch and
  `VerboseLogging`.
- Read-only. No Harmony patch writes anything; nothing is added to your save.

Bat Form and Aqua Form share the exact same code path as Cat Form and are on by default, but
only Cat Form has been confirmed in a real play session so far — see
[TESTING.md](TESTING.md).

While diagnosing this, a temporary in-game "give me an unowned form" debug hotkey was built and
used to test on a save with no forms unlocked yet, then removed before release — it was a
testing aid, not part of the fix. Its first attempt used `ToolTypeAsset.BorrowTool`, which
turned out to be unset for all three forms in this game's data; the working version used
`Asset.GetAll<ItemAsset>()`, the same item lookup the game's own (seemingly non-functional in
this build) debug item browser uses.
