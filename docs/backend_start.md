# Backend Start Notes

This backend starts from the priorities in `system_reboot_gdd.md` and `cursor_csharp_game_backend_instructions.md`.

## Initial scope

- `Players` owns server-created player identity and starting profile shape.
- `CloudSaves` owns versioned save snapshots for the cloud-ready local save path.
- `GameConfig` exposes safe, read-only balance/config values needed by Unity.
- `Auth` owns account registration, login, and bearer sessions.

## Current storage

Repositories use EF Core with SQLite. The default local database is `system-reboot.db`, and schema changes are tracked through migrations.

## Unity-facing contract rules

- Routes are under `/api/v1`.
- JSON uses camelCase.
- Controllers return explicit request/response DTOs.
- Errors use `{ "error": { "code": "...", "message": "..." } }`.
- The server assigns player IDs and cloud save timestamps.
- Player and cloud-save routes require bearer authentication.
- Player and cloud-save access is limited to the owning account.

## Next backend steps

1. Replace opaque save snapshots with module-owned server state where backend authority is required.
2. Add purchase validation before accepting premium-currency or store-related state.
3. Add request idempotency for high-risk reward, purchase, and currency actions.
4. Add rate limits for auth, save, and purchase endpoints.
