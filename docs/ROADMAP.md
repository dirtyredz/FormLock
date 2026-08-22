# ROADMAP — Form Lock

Small mod, short plan. Task detail lives in [BACKLOG.md](BACKLOG.md).

- **✅ 1.0.0 (shipped, 2026-08-12)** — form retention through pickups + bare-hand harvests, all three
  forms, full config. Live on Nexus ([mod 141](https://www.nexusmods.com/moonlightpeaks/mods/141)).
- **🚧 Next (1.1.0 candidate)** — the WIP pickup-stutter suppression (Feature 2). Ship criteria:
  in-game confirmation the hitch is gone, decide whether `PlayerHarvestState` needs the same
  treatment, CHANGELOG entry + version bump. Currently committed but unreleased.
- **Backlog before either grows** — hand-test Bat/Aqua, confirm each toggle gates its own behaviour,
  save-diff verification (see [BACKLOG.md](BACKLOG.md) / [../TESTING.md](../TESTING.md)).

No further scope planned; the mod is deliberately narrow (tool-gated actions stay out — see
[FEATURES.md](FEATURES.md)).
