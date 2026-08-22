# CLAUDE.md — working on Form Lock

How to work in **this** mod repo. It is a standalone git repo nested in the Moonlight Peaks
workspace; treat it as the active project (honor **its** gate/baseline, not the root's). Orientation
lives in the doc set — read those, don't duplicate them here.

- **[README.md](README.md)** — human quick-start + what the mod fixes.
- **[STRUCTURE.md](STRUCTURE.md)** — where things live (4 source files, one class each).
- **[docs/ARCHITECTURE.md](docs/ARCHITECTURE.md)** — how the patches work at runtime.
- **[docs/DECISIONS.md](docs/DECISIONS.md) · [FEATURES.md](docs/FEATURES.md) ·
  [ROADMAP.md](docs/ROADMAP.md) · [BACKLOG.md](docs/BACKLOG.md) · [GOTCHAS.md](docs/GOTCHAS.md)**
- **[RELEASING.md](RELEASING.md) · [TESTING.md](TESTING.md) · [CHANGELOG.md](CHANGELOG.md)** —
  release checklist, manual test log, release notes.

## Build

```bash
dotnet build src/FormLock.csproj -c Release -p:SkipDeploy=true
```

`SkipDeploy=true` skips copying the DLL into the live plugin dir (use it for a verify-only build so
you never overwrite the copy under test). Needs the game's `Managed/` assemblies at the Steam path
in `Directory.Build.props`. Do **not** launch the game to test — verification is manual and in-game
by the maintainer (see TESTING.md).

## Conventions (workspace)

- **Commit identity:** `dirtyredz <dirtyredz@live.com>`. Never the work email.
- **Versioning:** bump `<Version>` in `src/FormLock.csproj` only, only when publishing. Never
  hardcode a version in `Plugin.cs` (it derives `ModBuildInfo.Version`).
- **Layout:** plugin `.cs` flat in `src/` (no `src/FormLock/`); docs + `pack.ps1` at repo root.
- **`Directory.Build.props` + `pack.ps1` are workspace-synced canonicals** — edit the workspace
  source (`../../tools/sync-mod-files.ps1`), not the copies here.
- **Never commit** decompiled game code, `dist/`, `bin/`, `obj/`.

## Structure-review gate

This repo is gated (pre-push hook installed 2026-08-22). Edit/debug freely; the review fires once at
**push** on the accumulated change, not per edit or commit. Commit freely at logical boundaries;
Claude runs the review and pushes (asking first) when work is ready. `/gate status` shows what's
pending. `Last full review:` is stamped in [STRUCTURE.md](STRUCTURE.md).

## Release

Pack with `./pack.ps1` → `dist/FormLock-<version>.zip` (Nexus layout). Publish via the workspace
**nexus-publish** skill. Full chain: [../../docs/ARCHITECTURE.md](../../docs/ARCHITECTURE.md).
