# AGENTS.md

## Cursor Cloud specific instructions

### Repository status
This repository is currently **documentation-only / pre-implementation**. It contains two
design documents and **no source code, project files, or build system yet**:

- `system_reboot_gdd.md` — Game Design Document for "System Reboot", an idle/incremental
  Unity (C#) mobile RPG. The MVP is explicitly a **local-save, offline, single-player**
  vertical slice that requires **no backend**.
- `cursor_csharp_game_backend_instructions.md` — coding/architecture guidelines for a
  **future** C# **ASP.NET Core** backend (with EF Core, xUnit tests) consumed by the Unity
  client. This backend is explicitly post-MVP and not required for the MVP.

Because there is no code, there is currently **nothing to build, lint, test, or run** from
this repository. Any future agent that scaffolds the backend should follow the layering and
conventions in `cursor_csharp_game_backend_instructions.md`
(`Controller -> Service -> Repository -> Database`, DTOs for all Unity-facing contracts, etc.).

### Toolchain (already installed in the VM snapshot)
The anticipated backend stack is **.NET / ASP.NET Core**. The **.NET SDK is pre-installed**
in `~/.dotnet` and is on `PATH` for interactive shells (configured in `~/.bashrc`, which also
sets `DOTNET_ROOT`, `DOTNET_CLI_TELEMETRY_OPTOUT=1`, `DOTNET_NOLOGO=1`). Verify with
`dotnet --info`.

- If `dotnet` is not found in a non-interactive context, use the absolute path
  `~/.dotnet/dotnet` (or `source ~/.bashrc`).
- Unity is **not** installed and the Unity client cannot be built/run headless in this
  environment; client work must be validated elsewhere.

### Once the backend exists
Standard .NET commands apply once a `.sln`/`.csproj` is added (no need to duplicate them here):
- Restore: `dotnet restore`
- Build: `dotnet build`
- Run (dev): `dotnet run --project <BackendProject>` (ASP.NET Core dev server)
- Test: `dotnet test`

The startup update script already restores a solution automatically **if** one is present;
it is a safe no-op while the repo has no `.sln`.
