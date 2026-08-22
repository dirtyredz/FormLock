# Testing Log

Manual, because there is nothing here a console runner could exercise — see
[RELEASING.md](RELEASING.md).

## Confirmed working

| Behaviour | Notes |
|---|---|
| Cat Form survives a loose-item pickup | Walked up to a dropped item in Cat Form; item landed in inventory, form stayed on |
| Cat Form survives a bare-hand harvest | Harvested a no-tool node in Cat Form; form stayed on |
| `GameInventory.TryGrabPreviousGrabbedItem`'s no-op path | Confirmed indirectly — no second unequip/re-equip flicker was observed, consistent with the item never having been dropped in the first place |
| Pickup-stutter fix (Cat Form) | Confirmed 2026-08-22: walking through a loose item in Cat Form no longer snaps movement to a halt; `LogOutput.log` showed `Suppressed CharacterMover.StopMove during form pickup.` |

## 2026-08-22 — in-game test round (after the pickup-stutter deploy)

Fresh dev build deployed to `plugins/MoonlightPeaksMods/FormLock/`; owner ran the test script and
reported **all items good**: the pickup-stutter fix confirmed working (Cat Form, smooth, log lines
present), Bat/Aqua pickup + bare-hand harvest keep form, and the config toggles gate their own
behaviour. This confirms the WIP Feature 2 works in-game — remaining before a release is only the
CHANGELOG entry + version bump (not yet authorised).

Tested on a dedicated test save (`80f8b6b7-647c-42fc-8ca2-bf0411bd4d3f`, "Dirtyredz (FormLock
Test)") duplicated from the real save specifically for this, so nothing here risked the original.

## Found by testing, then fixed

| Symptom | Cause |
|---|---|
| Default `GiveFormsKey` (F10, debug-only, since removed) did nothing | Conflicted with MoonlightMinimap, which also binds F10 to show/hide the map. Both consumed the keypress. |
| Changed default didn't reach the running game | BepInEx keeps existing values in the `.cfg`; a new code default only reaches a fresh config file. Had to hand-edit the live `.cfg` as well. |
| `GiveTestForms` (debug-only, since removed) silently added nothing, every press | `ToolTypeAsset.BorrowTool` — the reference the game itself uses to hand you a tool from the wheel before you own it — was unset (`null`) for `Cat`, `Bat` and `Aqua` in this build's data. `VerboseLogging` surfaced this immediately once actually read: `LogOutput.log` showed "no BorrowTool reference" logged for all three, every single press, meaning the key input itself was working fine and the failure was entirely in the item lookup. Switched to `Asset.GetAll<ItemAsset>()`, the same enumeration `ItemDebugScreen.LoadItemLibrary()` uses for the game's own item browser, matched by which `FormToolAsset` subtype each item's `ToolAddon` is — that worked on the first try. |
| No visible confirmation the first time the (since-removed) debug hotkey worked | Success/failure only went to the BepInEx log, which isn't visibly open by default. Added an in-game `Shouter.Shout(...)` call so the debug tool gave immediate on-screen feedback instead of requiring a log check — moot now that the hotkey itself has been removed, but the lesson (a mod's only feedback channel should not be a log file the player has no reason to have open) is worth remembering for anything future. |

## Still to verify

- [x] **Bat Form** — pickup and harvest — confirmed 2026-08-22 (owner test round)
- [x] **Aqua Form** — pickup and harvest — confirmed 2026-08-22 (owner test round)
- [x] **Each config toggle actually gates its own behaviour** — `ApplyToPickup`,
      `ApplyToHarvest`, `KeepCatForm`, `KeepBatForm`, `KeepAquaForm`, `Enabled` — confirmed 2026-08-22
- [ ] **Fresh install** — delete the config, launch, check defaults and Mod Menu rendering
- [ ] **Save diff** — back up, play a session using the mod normally, confirm no unexpected
      change beyond ordinary play
- [ ] **Multiple pickups/harvests in a row while staying in form** — only single instances of
      each were tried so far
- [ ] **Interaction with other installed mods that also touch grabbed-item state** — nothing
      specific is suspected, just untried
