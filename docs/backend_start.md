# Backend Start Notes

This backend starts from the priorities in `system_reboot_gdd.md` and `cursor_csharp_game_backend_instructions.md`.

## Initial scope

- `Players` owns server-created player identity and starting profile shape.
- `CloudSaves` owns versioned save snapshots for the cloud-ready local save path.
- `GameConfig` exposes safe, read-only balance/config values needed by Unity from active persisted config versions.
- `Auth` owns account registration, login, and bearer sessions.
- `Resources` owns server-authoritative Matter, Energy, Data balances and offline claims.
- `Purchases` owns receipt validation and non-power entitlement grants.

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
- Resource balances and offline claims are calculated by the backend from active game config, not accepted from cloud-save JSON.
- Purchases require an owned player, validated receipt, and non-power product id.
- Offline resource claims and purchase validation require `requestId` for idempotent retries.

## Next backend steps

1. Replace the local mock purchase validator with real Android purchase validation.
2. Add admin-only endpoints or tooling for uploading and activating game config versions.
3. Add rate limits for auth, save, resource, and purchase endpoints.
4. Move additional progression state out of opaque cloud-save JSON as gameplay systems come online.
