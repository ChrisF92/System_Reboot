# Backend Start Notes

This backend starts from the priorities in `system_reboot_gdd.md` and `cursor_csharp_game_backend_instructions.md`.

## Initial scope

- `Players` owns server-created player identity and starting profile shape.
- `CloudSaves` owns versioned save snapshots for the cloud-ready local save path.
- `GameConfig` exposes safe, read-only balance/config values needed by Unity.

## Current storage

Repositories are in memory for the first scaffold. They keep the HTTP contracts and service boundaries testable before a database provider is selected.

## Unity-facing contract rules

- Routes are under `/api/v1`.
- JSON uses camelCase.
- Controllers return explicit request/response DTOs.
- Errors use `{ "error": { "code": "...", "message": "..." } }`.
- The server assigns player IDs and cloud save timestamps.

## Next backend steps

1. Choose the production persistence provider and add migrations.
2. Add authentication before exposing player or save APIs publicly.
3. Replace opaque save snapshots with module-owned server state where backend authority is required.
4. Add purchase validation before accepting premium-currency or store-related state.
