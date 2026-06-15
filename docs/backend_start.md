# Backend Start Notes

This backend starts from the priorities in `system_reboot_gdd.md` and `cursor_csharp_game_backend_instructions.md`.

## Initial scope

- `Players` owns server-created player identity and starting profile shape.
- `CloudSaves` owns versioned save snapshots for the cloud-ready local save path.
- `GameConfig` exposes safe, read-only balance/config values needed by Unity.

## Current storage

Repositories use EF Core with SQLite. The default local database is `system-reboot.db`, and schema changes are tracked through migrations.

## Unity-facing contract rules

- Routes are under `/api/v1`.
- JSON uses camelCase.
- Controllers return explicit request/response DTOs.
- Errors use `{ "error": { "code": "...", "message": "..." } }`.
- The server assigns player IDs and cloud save timestamps.

## Next backend steps

1. Add authentication before exposing player or save APIs publicly.
2. Replace opaque save snapshots with module-owned server state where backend authority is required.
3. Add purchase validation before accepting premium-currency or store-related state.
4. Add request idempotency for high-risk reward, purchase, and currency actions.
