# Cursor Agent Instructions for a C# Game Backend with a Unity Frontend

## Prime directive

You are building a backend for a game in **C#**, with a **Unity frontend** planned or already in development.

Your priorities are:

1. **Clear C# over clever C#**
2. **Simple ASP.NET Core patterns over custom frameworks**
3. **Explicit game logic over hidden magic**
4. **Small services over giant manager classes**
5. **Stable API contracts for Unity**
6. **Testable domain logic over tightly coupled code**
7. **Backend authority over client-submitted game state**

Do not generate over-engineered enterprise architecture unless it is clearly needed.

When unsure, write less code. Ask for clarification or implement the smallest safe version. Do not invent architecture.

---

# Important Unity-specific assumptions

The Unity client is not trusted.

The Unity client may request actions, display state, and submit player intent, but the backend decides what is true.

The backend must be authoritative for:

- authentication
- player identity
- inventory ownership
- currency balances
- XP and leveling
- quest progress
- achievement unlocks
- shop purchases
- reward claims
- matchmaking
- leaderboard scores
- cooldowns
- anti-cheat validation

The Unity client must never be the source of truth for:

- final match results
- XP gained
- currency gained
- items earned
- damage dealt
- reward eligibility
- achievement completion
- quest completion
- leaderboard score

---

# Preferred project structure

Prefer a boring, predictable structure:

```txt
Game.Backend/
  Program.cs
  appsettings.json

  Config/
  Database/
  Middleware/
  Shared/
    Errors/
    Validation/
    Time/
    Randomness/
    Contracts/

  Modules/
    Players/
      PlayersController.cs
      PlayerService.cs
      PlayerRepository.cs
      PlayerModels.cs
      PlayerDtos.cs
      PlayerValidators.cs

    Matches/
      MatchesController.cs
      MatchService.cs
      MatchRepository.cs
      MatchModels.cs
      MatchDtos.cs
      MatchRules.cs

    Inventory/
      InventoryController.cs
      InventoryService.cs
      InventoryRepository.cs
      InventoryModels.cs
      InventoryDtos.cs

    Economy/
    Auth/
    Quests/
    Achievements/
    Leaderboards/

Game.Backend.Tests/
  Modules/
    Matches/
      MatchRulesTests.cs
      MatchServiceTests.cs
```

Use one module per major game concept:

```txt
Players
Auth
Sessions
Matches
Inventory
Economy
Quests
Achievements
Leaderboards
```

Avoid vague folders/classes like:

```txt
Managers
Processors
Handlers
Helpers
Utils
Core
Engine
Common
```

unless the purpose is extremely clear.

---

# C# architecture rules

Use simple layering:

```txt
Controller → Service → Repository → Database
```

## Controllers

Controllers should only:

- receive HTTP requests
- validate/bind input
- call services
- return HTTP responses

Controllers must not contain game rules or database queries.

Bad:

```csharp
[HttpPost("claim")]
public async Task<IActionResult> ClaimReward(...)
{
    var player = await _db.Players.FindAsync(playerId);
    player.Xp += 100;
    await _db.SaveChangesAsync();
    return Ok(player);
}
```

Better:

```csharp
[HttpPost("claim")]
public async Task<ActionResult<ClaimRewardResponse>> ClaimReward(
    ClaimRewardRequest request,
    CancellationToken cancellationToken)
{
    var result = await _questService.ClaimRewardAsync(request, cancellationToken);
    return Ok(result);
}
```

## Services

Services contain business logic.

Services should:

- enforce game rules
- check permissions
- coordinate repositories
- calculate rewards
- update domain state
- return explicit DTOs or service result objects

## Repositories

Repositories should only:

- query the database
- save data
- map database entities if needed

Repositories must not contain game rules.

---

# Unity API contract rules

Because Unity will consume this backend, API contracts must be stable, explicit, and easy to deserialize.

Use DTOs for every request and response.

Do not expose EF Core entities directly to Unity.

Good:

```csharp
public sealed record PlayerProfileResponse(
    Guid PlayerId,
    string DisplayName,
    int Level,
    int Xp,
    int SoftCurrency);
```

Avoid:

```csharp
return Ok(playerEntity);
```

API responses should be shaped for Unity consumption, not database convenience.

Prefer predictable JSON:

```json
{
  "playerId": "7e8f8df3-8b92-41ef-8f32-783e5d6f100a",
  "displayName": "PlayerOne",
  "level": 4,
  "xp": 1200,
  "softCurrency": 500
}
```

Avoid inconsistent response shapes such as sometimes returning a raw object, sometimes a string, and sometimes a wrapper.

---

# JSON serialization rules

Unity clients can be sensitive to serialization choices, especially if using Unity's built-in `JsonUtility`.

Prefer simple JSON structures:

- primitives
- strings
- numbers
- booleans
- arrays/lists
- simple nested objects

Avoid unnecessary complexity in public API contracts:

- deeply nested polymorphic objects
- dictionaries as primary response structures
- dynamic objects
- anonymous objects as long-term contracts
- inheritance-heavy DTOs
- circular references

Do not rely on Unity understanding backend-specific C# constructs.

Use explicit DTOs with stable property names.

Keep property naming consistent. Prefer camelCase JSON for HTTP APIs unless the project already uses another convention.

Example:

```csharp
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});
```

Do not change JSON naming conventions casually after the Unity client depends on them.

---

# API versioning rules

Treat backend endpoints as contracts with the Unity client.

Do not casually rename, remove, or reshape API fields once used by Unity.

Prefer additive changes:

Good:

```json
{
  "playerId": "...",
  "level": 4,
  "xp": 1200,
  "softCurrency": 500,
  "premiumCurrency": 25
}
```

Risky:

```json
{
  "id": "...",
  "progression": {
    "level": 4,
    "experience": 1200
  }
}
```

If a breaking change is needed, explain:

- what changes
- why it is breaking
- which Unity code may be affected
- migration plan
- whether API versioning is needed

Consider versioned routes if the client may have multiple live versions:

```txt
/api/v1/players/profile
/api/v1/inventory
/api/v1/matches/{matchId}/join
```

Do not introduce full API versioning infrastructure unless needed, but design routes so versioning can be added cleanly.

---

# Unity-friendly endpoint design

Design endpoints around player actions, not database tables.

Good:

```txt
POST /api/v1/matches/{matchId}/join
POST /api/v1/matches/{matchId}/actions
POST /api/v1/inventory/items/{itemId}/equip
POST /api/v1/quests/{questId}/claim-reward
POST /api/v1/shop/purchases
GET  /api/v1/players/me
GET  /api/v1/inventory
GET  /api/v1/game-config
```

Less good:

```txt
POST /matchParticipants
PATCH /playerInventoryRows/{id}
POST /questRewardClaims
```

For Unity, prefer endpoints that map clearly to gameplay flows:

- load player profile
- load inventory
- start match
- submit match action
- complete match
- claim reward
- buy item
- equip item
- load remote config

---

# Client/server boundary rules

The Unity frontend may contain presentation logic and local prediction, but core progression and economy logic belongs on the backend.

Allowed on Unity client:

- UI state
- animations
- input handling
- local VFX/SFX
- local previews
- non-authoritative prediction
- cached display data
- offline-friendly cosmetic display where safe

Belongs on backend:

- ownership checks
- reward eligibility
- XP grants
- currency grants/spends
- inventory mutations
- leaderboard submission validation
- shop transaction validation
- quest completion validation
- cooldown enforcement
- anti-cheat checks

If logic is duplicated between Unity and backend, make the backend authoritative.

---

# Shared contracts guidance

Do not prematurely create a shared C# package between Unity and backend.

Shared contracts can be useful later, but only if they reduce real duplication without coupling Unity to backend internals.

Before creating a shared contracts project/package, explain:

- what types need to be shared
- how Unity will consume them
- whether they depend on ASP.NET Core or EF Core
- how versioning will work
- whether plain OpenAPI-generated clients would be simpler

Shared contract types must not reference:

- EF Core
- ASP.NET Core
- backend-only services
- database entities
- server-only configuration

If contracts are shared, keep them as simple DTOs only.

---

# OpenAPI / client generation rules

If the project uses OpenAPI/Swagger, keep it accurate.

OpenAPI is useful because Unity may eventually use generated client code or strongly typed request/response models.

When adding endpoints:

- use explicit request DTOs
- use explicit response DTOs
- document error responses where practical
- avoid anonymous response objects
- avoid returning untyped `object`

Do not design APIs that are hard to represent in OpenAPI.

---

# Game config / remote config rules

Unity often needs game configuration values such as item prices, XP curves, reward tables, cooldowns, and balance values.

Do not hardcode long-term balance data in many scattered backend files.

Prefer a clear source of truth for game config:

- database tables
- JSON config files loaded by backend
- admin-managed config later if needed

Keep config access explicit.

Good:

```csharp
public sealed record ItemDefinition(
    string ItemId,
    string DisplayName,
    int Price,
    string CurrencyType);
```

Risky:

```csharp
if (itemId == "sword_01") price = 100;
if (itemId == "shield_01") price = 150;
```

Expose safe config to Unity through a read-only endpoint if needed:

```txt
GET /api/v1/game-config
```

Never expose secret server-only config to Unity.

---

# C# style rules

Use clear, idiomatic C#.

Prefer:

- explicit types for public APIs
- records for immutable DTOs
- small classes
- small methods
- async/await for I/O
- CancellationToken on async operations
- nullable reference types enabled
- dependency injection through constructors

Avoid:

- giant service classes
- static mutable state
- service locator patterns
- unnecessary base classes
- reflection-heavy designs
- dynamic
- magic strings
- global singletons
- premature interfaces for every class

Do not create interfaces automatically for every service or repository.

Only introduce an interface when:

- it is needed for testing
- there are multiple implementations
- the abstraction already exists in the project

---

# Naming rules

Use names that describe game concepts.

Good:

```csharp
CreateMatchAsync
JoinMatchAsync
GrantItemToPlayerAsync
SpendCurrencyAsync
CompleteQuestAsync
ClaimDailyRewardAsync
CalculateXpReward
CanPurchaseItem
```

Bad:

```csharp
ProcessData
HandleStuff
ExecuteAction
DoUpdate
GameManager
PlayerProcessor
BaseService
GenericHandler
```

Avoid vague suffixes unless they are meaningful:

```txt
Manager
Processor
Handler
Helper
Utility
Base
Core
```

---

# Game logic rules

Game logic should be deterministic and easy to test.

Prefer pure static methods or focused rule classes for core calculations:

```csharp
public static class MatchRules
{
    public static MatchOutcome CalculateOutcome(MatchState state)
    {
        // deterministic logic
    }
}
```

Good examples:

```csharp
CalculateXpReward(...)
CalculateMatchOutcome(...)
CanPurchaseItem(...)
ApplyDamage(...)
GetLevelFromXp(...)
CalculateQuestProgress(...)
```

These methods should not directly use:

- DbContext
- HttpContext
- DateTime.UtcNow
- Random.Shared
- environment variables
- configuration

Pass time and randomness in explicitly.

Bad:

```csharp
public int CalculateDailyReward(Player player)
{
    if (DateTime.UtcNow.DayOfWeek == DayOfWeek.Sunday)
        return 200;

    return Random.Shared.Next(50, 100);
}
```

Better:

```csharp
public int CalculateDailyReward(
    Player player,
    DateTimeOffset now,
    IRandomNumberGenerator rng)
{
    if (now.DayOfWeek == DayOfWeek.Sunday)
        return 200;

    return rng.Next(50, 100);
}
```

---

# Entity Framework rules

Use EF Core simply and predictably.

Prefer:

- clear entity classes
- explicit DbSet names
- explicit relationships
- migrations for schema changes
- AsNoTracking for read-only queries
- cancellation tokens

Avoid:

- lazy loading unless already used
- huge Include chains
- generic repositories
- exposing IQueryable from repositories
- putting business logic in entity setters
- storing critical game state in unstructured JSON blobs

Repository methods should return specific results:

```csharp
Task<Player?> GetByIdAsync(Guid playerId, CancellationToken cancellationToken);

Task<IReadOnlyList<InventoryItem>> GetInventoryAsync(
    Guid playerId,
    CancellationToken cancellationToken);
```

Avoid:

```csharp
IQueryable<Player> GetPlayers();
```

---

# DTO and model rules

Separate:

```txt
Entity models: database shape
Domain/rule models: game logic shape
DTOs: API request/response shape
```

Do not let database models leak everywhere.

Do not expose EF entities directly from API endpoints.

Use DTOs.

---

# Validation rules

Validate all client input at the boundary.

Validate:

- route params
- request bodies
- query strings
- player actions
- inventory item IDs
- match IDs
- shop purchases

Never trust Unity-submitted values for rewards, progression, or ownership.

The backend must validate action intent, not accept final outcomes blindly.

---

# Error handling rules

Use explicit application errors.

Example:

```csharp
public sealed class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

public sealed class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message) { }
}

public sealed class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
}
```

Then map them in middleware:

```txt
NotFoundException → 404
ForbiddenException → 403
ValidationException → 400
Unhandled exception → 500
```

Return Unity-friendly error responses.

Prefer a consistent shape:

```json
{
  "error": {
    "code": "insufficient_currency",
    "message": "Not enough soft currency to buy this item."
  }
}
```

Do not expose stack traces, database errors, or internal exception details to Unity.

---

# Async rules

Use async only for I/O.

Use:

```csharp
Task<T>
Task
CancellationToken
```

Do not make pure game logic async.

Bad:

```csharp
public async Task<int> CalculateXpAsync(Player player)
{
    return player.Level * 100;
}
```

Better:

```csharp
public int CalculateXp(Player player)
{
    return player.Level * 100;
}
```

Every async service/repository method should accept a `CancellationToken`.

---

# Dependency injection rules

Use constructor injection.

Good:

```csharp
public sealed class InventoryService
{
    private readonly InventoryRepository _inventoryRepository;
    private readonly PlayerRepository _playerRepository;

    public InventoryService(
        InventoryRepository inventoryRepository,
        PlayerRepository playerRepository)
    {
        _inventoryRepository = inventoryRepository;
        _playerRepository = playerRepository;
    }
}
```

Avoid:

- service locator
- static access to services
- resolving services manually from IServiceProvider
- hidden dependencies

Do not create a complex DI setup unless required.

---

# Testing rules

Use tests for important backend behavior.

Prioritize tests for:

- XP and leveling
- inventory grants/removals
- currency spending
- shop purchases
- quest rewards
- match results
- cooldowns
- auth permissions
- leaderboard updates
- anti-cheat validation
- API contract stability for Unity-facing endpoints

Prefer unit tests for pure game rules:

```csharp
[Fact]
public void CanPurchaseItem_ReturnsFalse_WhenPlayerHasInsufficientCurrency()
{
    var result = EconomyRules.CanPurchaseItem(playerCurrency: 50, itemCost: 100);

    result.Should().BeFalse();
}
```

Use integration tests for:

- controller endpoints
- database persistence
- auth behavior
- full service flows
- JSON response shape

Do not skip tests for economy, inventory, rewards, or Unity-facing API contracts.

Those are high-risk game systems.

---

# Anti-cheat backend rules

The backend decides what is true.

The Unity client can request:

- start match
- submit action
- claim reward
- buy item
- equip item

But the backend must verify:

- player identity
- player ownership
- current game state
- cooldowns
- resource costs
- inventory capacity
- reward eligibility
- score validity

Never accept these directly from Unity:

- final score
- XP gained
- currency gained
- damage dealt
- items earned
- quest completed
- achievement unlocked

---

# Multiplayer and real-time rules

Do not introduce websockets, SignalR, queues, or real-time infrastructure by default.

Before adding real-time multiplayer features, explain:

- whether the game needs real-time sync or turn-based/request-response flow
- what state must be authoritative on the server
- expected update frequency
- cheating risks
- persistence requirements
- fallback behavior when clients disconnect

For many games, start with simple HTTP endpoints first.

Only introduce SignalR/websockets when there is a clear gameplay requirement.

---

# Offline and reconnect rules

Unity clients may disconnect, retry, or send duplicate requests.

For important actions, design backend behavior to be safe when retried.

Important examples:

- purchase item
- claim daily reward
- claim quest reward
- complete match
- grant inventory item
- spend currency

Prefer idempotency or duplicate protection where needed.

For high-risk actions, consider request IDs:

```json
{
  "requestId": "client-generated-guid",
  "itemId": "sword_01"
}
```

Do not add idempotency infrastructure everywhere by default, but consider it for rewards, purchases, and currency changes.

---

# File size and complexity rules

Avoid large files.

Warning signs:

- 200+ lines: consider splitting
- 400+ lines: usually too large
- 700+ lines: probably wrong

Avoid methods with:

- more than 40 lines
- more than 3 levels of nesting
- many unrelated responsibilities
- mixed validation, database access, business logic, and response formatting

Prefer extracting small private methods or pure rule functions.

---

# Refactoring rules

Refactor only when it helps the current task.

Good refactors:

- rename unclear methods
- extract pure game rules
- split large methods
- remove duplicated logic
- add missing DTOs
- move DB access out of controllers
- stabilize Unity-facing contracts

Avoid:

- rewriting whole modules
- changing public APIs unnecessarily
- introducing new architectural patterns
- changing database schema casually
- replacing working code without need

Before large refactors, explain the problem and proposed change.

---

# Documentation rules

Comment why, not what.

Good:

```csharp
// Use server time because client clocks can be manipulated.
```

Bad:

```csharp
// Add XP to player.
player.Xp += xpReward;
```

Each important module should have a short note explaining:

- what the module owns
- what it does not own
- important invariants
- whether Unity depends on its API contracts

Example:

```txt
Inventory owns item grants, removals, and ownership checks.
It does not decide quest reward contents or shop prices.
Unity consumes InventoryDtos, so response fields should not be renamed casually.
```

---

# Logging rules

Log important game/backend events:

- failed login
- suspicious action
- currency spent
- currency granted
- item granted
- item removed
- match completed
- reward claimed
- purchase attempted
- duplicate reward claim attempt

Do not log:

- passwords
- tokens
- secrets
- full auth headers
- private user data

Use structured logging:

```csharp
_logger.LogInformation(
    "Player {PlayerId} claimed quest reward {QuestId}",
    playerId,
    questId);
```

---

# Cursor agent workflow

Do not generate a massive implementation in one pass.

Work in this order:

1. Understand the task
2. Identify the smallest useful change
3. List files to change
4. Add/update DTOs
5. Add validation
6. Add pure game rules
7. Add repository methods
8. Add service logic
9. Add controller endpoint
10. Add tests
11. Run build/tests/lint
12. Summarize changes

After coding, always report:

- files changed
- behavior added
- tests added
- commands run
- assumptions
- risks
- whether any Unity-facing API contract changed

Never claim tests passed unless they were actually run.

---

# Hard rules

Never do these without explicit approval:

- add a new framework
- change the database provider
- change authentication strategy
- change project structure
- introduce MediatR
- introduce CQRS
- introduce microservices
- introduce event sourcing
- introduce Redis
- introduce queues
- introduce websockets
- introduce SignalR
- add a paid third-party service
- change save data format
- broadly rebalance economy values
- make breaking API contract changes for Unity

MediatR, CQRS, event sourcing, generic repositories, SignalR, Redis, and queues are not default choices.

Use them only if the project already uses them or the user explicitly requests them.

---

# Final review checklist

Before finishing any task, check:

- Is this understandable to a human C# developer?
- Are controllers thin?
- Is business logic in services or rule classes?
- Is database access isolated?
- Are DTOs separate from EF entities?
- Is client input validated?
- Is the backend authoritative?
- Are Unity-facing response shapes stable and explicit?
- Are important game rules tested?
- Are names clear?
- Is there unnecessary abstraction?
- Did we avoid unrelated rewrites?

If any answer is “no,” fix it before presenting the result.

---

# Required Cursor response format

For each task, respond like this:

```md
## Understanding

Brief task summary.

## Plan

Small numbered plan.

## Files to change

Expected files.

## Implementation notes

Important design decisions, including Unity/API contract impact if relevant.

## After coding

- Files changed
- Tests added
- Commands run
- Assumptions
- Risks
- Unity-facing API changes
```

Keep explanations direct and practical.
