# ARCHITECTURE — Form Lock

How the mod works at runtime. Code-shape map is in [../STRUCTURE.md](../STRUCTURE.md).

## What it is

A BepInEx 5 plugin that HarmonyX-patches the shipped game (`Moonlight Peaks.exe`). It is **read-only
with respect to saves** — every patch either skips a vanilla call or restores a field it temporarily
changed; nothing is written to `GameData.json`.

## The problem, and the two features

### Feature 1 — Form retention (shipped, 1.0.0)

Vanilla drops you out of Cat/Bat/Aqua form on every pickup or bare-hand harvest:

1. `PlayerPickupState.OnActivate` / `PlayerHarvestState.OnActivate` call `GameInventory.TryGrabNone()`
   to free the hands for the harvest animation.
2. On deactivate, `GameInventory.TryGrabPreviousGrabbedItem()` restores what you held — but it
   **explicitly refuses to re-equip a Form-type tool** (`UseType == ToolUseType.Form → return false`).

So the form comes off and never goes back on. The fix is one **prefix on `GameInventory.TryGrabNone`**:
if the grabbed item is a protected form *and* the active state is `PlayerPickupState` or
`PlayerHarvestState`, skip the unequip entirely. Because the form was never dropped,
`TryGrabPreviousGrabbedItem`'s own "already holding something" guard makes the later restore a no-op.

### Feature 2 — Pickup-stutter suppression (WIP, unreleased)

Even with the form kept on, picking up a loose item in form produces a movement hitch, from two
interacting causes:

1. `BasePlayerState.OnActivate` adds `"BasePlayerState"` to the input blocker → `mover.StopMove()`
   zeros RVO velocity → the character snaps to a halt.
2. The blocker lingers until `OnDeactivate`, so `ProcessInput()` is skipped for the rest of the frame.

The fix layers three patches, all gated on `FormProtection.TryGetProtectedForm` so vanilla pickups
are untouched:

- **Prefix on `CharacterMover.StopMove`** — skip while in a form-pickup combo (velocity never zeroed).
- **Postfix on `PlayerPickupState.OnActivate`** — remove the `"BasePlayerState"` blocker key that
  `base.OnActivate` just added, so `ProcessInput()` runs next `Update()`.
- **Prefix/postfix on `PlayerPickupState.OnActivate`/`OnDeactivate`** — zero the pickup wait timers
  (saved + restored via the `_savedPickup*` sentinel) so the coroutine exits in two frames instead
  of ~0.46 s, killing the `HarvestTrigger` animation pop.

## Control flow

```
pickup/harvest starts
  → PlayerPickupState/PlayerHarvestState.OnActivate
      → GameInventory.TryGrabNone()   [Feature 1 prefix: skip if protected form]
      → (WIP) OnActivate prefix collapses timers; postfix clears input blocker
      → (WIP) CharacterMover.StopMove  [prefix: skip while form-pickup combo active]
  → OnDeactivate  → TryGrabPreviousGrabbedItem() → no-op (form never left)
                  → (WIP) OnDeactivate postfix restores saved timers
```

## Safety model

Every patch body is wrapped in try/catch that falls back to vanilla behaviour on any exception —
a broken check must never strip a form or wedge the pickup/harvest flow. `FormProtection` returns
`false` for unrecognised (future/modded) form subtypes, erring toward vanilla.

## External interfaces

- **Config** — 7 BepInEx `ConfigEntry`s (see [FEATURES.md](FEATURES.md)), editable in-game via Mod
  Menu (section/label titles carried in `ConfigDescription.Tags`) or the `.cfg` file.
- **Game types** — `GameInventory`, `PlayerView`/`PlayerPickupState`/`PlayerHarvestState`,
  `CharacterMover`, `PlayerInput`, `FormToolAsset` + `Cat/Bat/AquaToolAsset` (from
  `Vampire.Runtime.dll` / `Chicken.Utilities`). Referenced, never redistributed.
