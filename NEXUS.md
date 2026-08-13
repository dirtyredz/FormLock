# Nexus Page Copy

> **Pasting into the upload form? Use [nexus-paste.md](nexus-paste.md), not this file.**
> The copy here is wrapped for reading, and the editor turns every wrap into a `<br>`.
> Style: [15-page-style.md](../../15-page-style.md). Structure/mechanics:
> [13-nexus-page-standard.md](../../13-nexus-page-standard.md).

Reference copy for the mod page, plus the shot list.

**Category:** Gameplay
**Tags:** Quality of Life, Gameplay *(confirm both exist in the fixed tag list at upload time —
see [13-nexus-page-standard.md](../../13-nexus-page-standard.md) on tags being a per-game
vocabulary, not free text)*
**Requirements:** BepInEx 5 (win_x64), 5.4.23.5 or newer
**Nexus page:** not yet published

---

## Summary (one line, shows in listings)

Pick something up in Cat, Bat or Aqua form and the game turns you human again to do it. Form
Lock stops that — you stay in form.

---

# Paste-ready page copy

## Title emoji

🔒 — matches the mod's name literally, and applies equally to all three forms rather than just
Cat Form.

## At-a-glance strip

💾 Save-safe  ·  🔧 One Harmony patch  ·  🐾 Cat · Bat · Aqua  ·  🎨 Configurable

## Promise panel

🔒 **The promise.** Your form is a choice you made, not something the game quietly undoes every
time you bend down.

## Field: What it does

Turn into a cat to slip somewhere, and the moment you pick something up off the ground, you're
human again — no warning, no prompt, just back on two legs holding an item. Same story for Bat
Form and Aqua Form: one loose item, and the form comes off.

That's the game freeing your hands for the pickup animation, and then simply declining to put
the form back on afterwards — by design, not a bug, but not a choice you get to make either.

Form Lock keeps whatever form you're wearing through both a loose-item pickup and a bare-hand
harvest — bushes, produce, anything that doesn't need a tool. The item still lands in your
inventory exactly like normal. You just don't get bounced out of form to receive it.

Tool-gated harvesting — chopping a tree, mining a rock — is untouched, and deliberately so:
you already can't target those while a form is grabbed instead of the right tool, so there was
never an unwanted unequip on that path to begin with.

## Field: Main features

- **Cat, Bat and Aqua form** all stay equipped through a pickup
- **...and through a bare-hand harvest** — bushes, produce, anything tool-free
- **Tool-gated harvesting is untouched** — chopping and mining already need the right tool
  equipped, so nothing changes there
- **Per-form toggles** — turn Cat, Bat or Aqua off individually if one should keep the game's
  own behaviour
- **Per-flow toggles** — apply to pickups, harvests, or both
- **One master switch** restores stock behaviour instantly
- **Read-only** — one Harmony patch, no save writes, nothing left behind if you uninstall

## Field: Requirements

**Required**

- **BepInEx 5 (win_x64)**, version 5.4.23.5 or newer

**Recommended companion**

- **Mod Nook** — my in-game settings menu. Form Lock's settings are all switches — which form,
  which flow, on or off — and Mod Nook turns that config file into toggles you can flip
  mid-session and see take effect immediately. Nothing here needs it; without it the settings
  live in a plain config file. https://www.nexusmods.com/moonlightpeaks/mods/127
- **Mod Menu** by Elsiabeth does the same job and is also supported. Mod Nook and Mod Menu can
  both be installed — each adds its own button and neither interferes with the other.

PC/Steam only. The Switch and mobile builds can't load BepInEx.

**Compatibility**

A single Harmony Prefix on one method (`GameInventory.TryGrabNone`) that only ever changes
whether that one call runs — it doesn't touch UI, inventory layout or anything another mod is
likely to also be patching. No known conflicts.

## Field: Installation instructions

**With Vortex**

Open the Files tab, click the Vortex button, and enable the mod. Done.

**Manually**

1. Install **BepInEx 5 (win_x64)** into your Moonlight Peaks folder, if you do not have it
   already. The BepInEx folder sits beside Moonlight Peaks.exe.
2. Launch the game once, then quit — this creates the `BepInEx/plugins` folder.
3. Download Form Lock from the Files tab and extract the archive **over your Moonlight Peaks
   folder**, so the file ends up at `BepInEx/plugins/FormLock/FormLock.dll`.
4. Launch the game.

Settings are written to `BepInEx/config/com.dirtyredz.moonlightpeaks.formlock.cfg` on first
launch. With **Mod Nook** installed you never need to open it — every setting appears under
**Pause > Mod Nook** and applies immediately, without a restart.

To uninstall, delete `BepInEx/plugins/FormLock`. Nothing is written to your save, so there is
nothing else to clean up.

## Field: Configuration

Settings are written to `BepInEx/config/com.dirtyredz.moonlightpeaks.formlock.cfg` on first
launch. The defaults — everything on — are meant to be left alone unless one form or one flow
should behave like vanilla for you.

Install **Mod Nook** and change them in game instead. Form Lock shows up in it on its own, and
every setting is a toggle switch you can flip mid-session.

## Field: Shout outs

- **Little Chicken Game Company**, for a form-switching system clean enough that the actual bug
  was three lines deep in a shared "free your hands" helper, not buried in special-case code.
- The **BepInEx** and **HarmonyX** teams, without whom none of this scene exists.
- **Elsiabeth**, for **Mod Nook** — settings in-game instead of a text file is the difference
  between a config being used and being ignored.
- **My Mate**, for being my inspiration.

---

## Long-form description (reference)

Turn into a cat to slip somewhere, and the moment you pick something up off the ground, you're
human again — no warning, no prompt, just back on two legs holding an item. Same story for Bat
Form and Aqua Form: one loose item, and the form comes off.

### 🐾 Why this happens

That's the game freeing your hands for the pickup animation, and then simply declining to put
the form back on afterwards — by design, not a bug, but not a choice you get to make either.

### 🔒 What Form Lock does

Keeps whatever form you're wearing through both a loose-item pickup and a bare-hand harvest —
bushes, produce, anything that doesn't need a tool. The item still lands in your inventory
exactly like normal. You just don't get bounced out of form to receive it.

Tool-gated harvesting — chopping a tree, mining a rock — is untouched, and deliberately so: you
already can't target those while a form is grabbed instead of the right tool, so there was never
an unwanted unequip on that path to begin with.

### 🎛️ Configuration

Every form (Cat, Bat, Aqua) and every flow (pickup, harvest) can be switched off independently
if you'd rather keep the game's own behaviour for one of them. One master switch turns the whole
mod off.

### 💾 Save-safe

**Read-only.** One Harmony patch that only ever skips a single method call — nothing is added,
removed or renamed in your save file.

### 📦 Installation

1. Install [BepInEx 5](https://github.com/BepInEx/BepInEx/releases) into your Moonlight Peaks folder
2. Launch the game once, then quit
3. Extract this archive over your Moonlight Peaks folder
4. Launch

---

## Screenshot shot list

None captured yet. Capture on the current build, on a save with at least one form unlocked.

| # | Shot | Must show |
|---|---|---|
| 1 | `01-cat-form-pickup.png` | Standing in Cat Form with an item mid-flight/just collected, form visibly still on |
| 2 | `02-bat-form-harvest.png` | Bat Form, right after a bare-hand harvest, form still on |
| 3 | `03-mod-nook-settings.png` *(optional)* | The Mod Nook panel showing Form Lock's toggles |
| - | Thumbnail, 16:9 | composed at 16:9 per the note in `CoffinBreak/nexus-paste.md` — stretched, not cropped, by the listing tile |
| - | Title banner | wide format, roughly matching the other five mods' banners |

Shot 1 is the whole pitch — a cat-eared/tailed character holding a freshly-picked item states the
feature with no caption needed. Shot 2 exists mainly to back up the "all three forms" claim
visually, since Cat Form alone would look like the only one that was ever tested.
