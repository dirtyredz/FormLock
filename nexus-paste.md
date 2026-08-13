# Form Lock — Nexus page source

**Nexus page:** not yet published

The description field is **SCEditor with a BBCode source**, so the block below is the literal
value to set. Every paragraph is a single unbroken line — see
[15-page-style.md](../../15-page-style.md) on why: the editor turns every wrap in a pasted
paragraph into a `<br>`, which is what happened to Last Swing and Transplant.

Style reference: [15-page-style.md](../../15-page-style.md). Mechanics:
[13-nexus-page-standard.md](../../13-nexus-page-standard.md).

## Other fields

| Field | Value |
|---|---|
| Name | `Form Lock` — no subtitle, per the standing decision in 13-nexus-page-standard.md |
| Category | Gameplay |
| Tags | Quality of Life, Gameplay — confirm both exist in the fixed per-game tag list before saving |
| Short description | Pick something up in Cat, Bat or Aqua form and the game turns you human again to do it. Form Lock stops that — you stay in form. |

## Description source

```bbcode
[size=6][color=#F7D994]🔒  Form Lock[/color][/size]
[color=#C7A25B][i]Pick something up in Cat, Bat or Aqua form and the game turns you human again to do it. Form Lock stops that — you stay in form.[/i][/color]
[color=#C7A25B]💾 Save-safe  ·  🔧 One Harmony patch  ·  🐾 Cat · Bat · Aqua  ·  🎨 Configurable[/color]
[color=#7A6A9B]────────────────────────────────────────[/color]
[quote]🔒  [color=#F7D994][b]The promise.[/b][/color] Your form is a choice you made, not something the game quietly undoes every time you bend down.[/quote]

[size=5][color=#F7D994]🐾  What it does[/color][/size]
[color=#D4D4D8]Turn into a cat to slip somewhere, and the moment you pick something up off the ground, you're human again — no warning, no prompt, just back on two legs holding an item. Same story for Bat Form and Aqua Form: one loose item, and the form comes off.

That's the game freeing your hands for the pickup animation, and then simply declining to put the form back on afterwards — by design, not a bug, but not a choice you get to make either.

Form Lock keeps whatever form you're wearing through both a loose-item pickup and a bare-hand harvest — bushes, produce, anything that doesn't need a tool. The item still lands in your inventory exactly like normal. You just don't get bounced out of form to receive it.

Tool-gated harvesting — chopping a tree, mining a rock — is untouched, and deliberately so: you already can't target those while a form is grabbed instead of the right tool, so there was never an unwanted unequip on that path to begin with.[/color]

[size=5][color=#F7D994]✨  Main features[/color][/size]
[list]
[*][b]Cat, Bat and Aqua form[/b] all stay equipped through a pickup
[*][b]...and through a bare-hand harvest[/b] — bushes, produce, anything tool-free
[*][b]Tool-gated harvesting is untouched[/b] — chopping and mining already need the right tool equipped, so nothing changes there
[*][b]Per-form toggles[/b] — turn Cat, Bat or Aqua off individually if one should keep the game's own behaviour
[*][b]Per-flow toggles[/b] — apply to pickups, harvests, or both
[*][b]One master switch[/b] restores stock behaviour instantly
[*][b]Read-only[/b] — one Harmony patch, no save writes, nothing left behind if you uninstall
[/list]

[size=5][color=#F7D994]📋  Requirements[/color][/size]
[list]
[*][b]BepInEx 5 (win_x64)[/b], version 5.4.23.5 or newer
[/list]
[color=#D4D4D8]PC/Steam only. The Switch and mobile builds can't load BepInEx.[/color]

[size=5][color=#F7D994]📥  Installation[/color][/size]
[b]🟢 With Vortex[/b]
[color=#D4D4D8]Open the Files tab, click the Vortex button, and enable the mod. Done.[/color]

[b]🔧 Manually[/b]
[list=1]
[*]Install [b]BepInEx 5 (win_x64)[/b] into your Moonlight Peaks folder, if you do not have it already. The BepInEx folder sits beside Moonlight Peaks.exe.
[*]Launch the game once, then quit. This creates the BepInEx/plugins folder.
[*]Download Form Lock from the Files tab and extract the archive over your Moonlight Peaks folder, so the file ends up at BepInEx/plugins/FormLock/FormLock.dll
[*]Launch the game.
[/list]
[color=#D4D4D8]To uninstall, delete BepInEx/plugins/FormLock. Nothing is written to your save, so there is nothing else to clean up.[/color]

[size=5][color=#F7D994]🎛️  Configuration[/color][/size]
[quote]📝  [color=#F7D994][b]Every setting is a switch.[/b][/color] Which form, which flow, on or off — install Mod Nook and flip them mid-session instead of editing a text file.[/quote]
[color=#D4D4D8]Settings are written to BepInEx/config/com.dirtyredz.moonlightpeaks.formlock.cfg on first launch. The defaults — everything on — are meant to be left alone unless one form or one flow should behave like vanilla for you.

Install [url=https://www.nexusmods.com/moonlightpeaks/mods/127][b]Mod Nook[/b][/url] and change them in game instead. Form Lock shows up in it on its own, and every setting is a toggle switch you can flip mid-session and see take effect immediately. Nothing here needs it — it just makes this mod easier to live with.[/color]

[size=5][color=#F7D994]🤝  Compatibility[/color][/size]
[color=#D4D4D8]A single Harmony Prefix on one method ([i]GameInventory.TryGrabNone[/i]) that only ever changes whether that one call runs — it doesn't touch UI, inventory layout or anything another mod is likely to also be patching. No known conflicts.[/color]

[size=5][color=#F7D994]💜  Shout outs[/color][/size]
[list]
[*][b]Little Chicken Game Company[/b], for a form-switching system clean enough that the actual bug was three lines deep in a shared "free your hands" helper, not buried in special-case code.
[*]The [b]BepInEx[/b] and [b]HarmonyX[/b] teams, without whom none of this scene exists.
[*][b]Elsiabeth[/b], for [b]Mod Nook[/b] — settings in-game instead of a text file is the difference between a config being used and being ignored.
[*][b]My Mate[/b], for being my inspiration.
[/list]
```
