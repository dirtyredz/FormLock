# Form Lock

Keeps Cat/Bat/Aqua form equipped through item pickups and bare-hand harvests, instead of the
game silently kicking you back to human form.

**Status:** 🚀 **Published** — v1.0.0 live on Nexus as
[mod 141](https://www.nexusmods.com/moonlightpeaks/mods/141).

## The bug this fixes

Reported behaviour: picking up an item while in Cat Form reverts you to human form to do it.

Root cause, found by decompiling `Vampire.Runtime.dll`:

- `PlayerPickupState.OnActivate()` (loose items on the ground) and `PlayerHarvestState.OnActivate()`
  (bare-hand harvest nodes - bushes, produce) both unconditionally call `GameInventory.TryGrabNone()`
  before playing their harvest animation, to free your hands.
- `GameInventory.TryGrabPreviousGrabbedItem()`, called afterwards to restore whatever you were
  holding, explicitly refuses to re-equip a Form-type tool:

  ```csharp
  if (itemAsset.ToolAddon != null && itemAsset.ToolAddon.ToolType.UseType == ToolUseType.Form)
      return false;
  ```

So every pickup or harvest permanently drops you out of form, with no prompt and no way back in
except manually re-equipping from the tool wheel.

## What the patch does

A single Harmony Prefix on `GameInventory.TryGrabNone`: if the currently grabbed item is a form
tool, and the active player state is `PlayerPickupState` or `PlayerHarvestState`, skip the
unequip entirely. `TryGrabPreviousGrabbedItem`'s own "already holding something" check then makes
the later restore call on deactivate a no-op, so nothing else needs to change.

## What this covers, and what it doesn't

- **Covered:** walking up to a loose dropped item and having it fly into your inventory, and
  bare-hand harvesting (bushes, produce, and similar nodes with no `ToolRequirements`) - both
  work while staying in Cat/Bat/Aqua form.
- **Not covered, and not changed:** tool-gated actions - chopping a tree, mining a rock, anything
  that requires a specific tool type. Those already can't be targeted while a form tool is
  grabbed instead of the required tool (`PlayerInteractor.HasCorrectToolForInteractable` filters
  them out before you ever reach an interaction), so there's no unequip for this patch to
  intervene on in the first place. You still need to swap to the actual tool for those.
- **Cosmetic:** the pickup/harvest animation trigger still fires on the player's Animator. The
  Cat/Bat/Aqua body's Animator Controller has no clip wired to it, so Unity just ignores the
  trigger - no error, but no animation plays either. Purely visual; the item still lands in your
  inventory.

## Configuration

- `Enabled` - master switch.
- `ApplyToPickup` / `ApplyToHarvest` - which of the two flows this applies to.
- `KeepCatForm` / `KeepBatForm` / `KeepAquaForm` - which forms this applies to. Bat and Aqua form
  each run extra land/water positioning logic on unequip (`FormHelper.IsFormItemEquippable`,
  teleport-to-safe-position); staying in form skips that entirely since you never leave it. Off
  by default risk-wise these are no different from Cat form's case, so all three default on -
  turn one off if a particular form ever behaves oddly picking things up.
- `VerboseLogging` - logs every time a form is kept on through a pickup/harvest.

Settings are configurable in-game via Mod Menu, or by editing the `.cfg` file directly.

## Status

**v1.0.0 is published** — live on Nexus as [mod 141](https://www.nexusmods.com/moonlightpeaks/mods/141).
Confirmed working in-game for Cat Form (staying in form through both a pickup and a bare-hand
harvest); Bat Form and Aqua Form share the same code path but haven't been hand-tested yet.

A follow-up **pickup-stutter suppression** feature is committed but **not yet released** — see
[docs/ROADMAP.md](docs/ROADMAP.md) and [docs/BACKLOG.md](docs/BACKLOG.md) for what it needs before
it ships. Release plumbing: [RELEASING.md](RELEASING.md) (checklist), [TESTING.md](TESTING.md)
(verified behaviour), [CHANGELOG.md](CHANGELOG.md) (release notes), and
[NEXUS.md](NEXUS.md) / [nexus-paste.md](nexus-paste.md) (page copy).

## Testing note

While verifying this, a temporary `GiveFormsKey` debug hotkey (Home) was added to grant unowned
forms for testing, since the game's own built-in debug item spawner (`F4` Creative Mode + `I`)
does not appear to be wired up in the shipped build, and `ToolTypeAsset.BorrowTool` - the
reference the game itself uses to hand you a tool from the wheel before you own it - turned out
to be unset for all three forms. It briefly used `Asset.GetAll<ItemAsset>()` (the same lookup
`ItemDebugScreen.LoadItemLibrary()` uses for the game's own item browser) to find each form item
instead. That hotkey and its config option have since been removed from the mod - it was
testing-only, not part of the fix.
