# System Reboot — Game Design Document

**Document Type:** Living Game Design Document  
**Format:** Markdown  
**Target Engine:** Unity  
**Primary Language:** C#  
**Primary Platform:** Android mobile  
**Orientation:** Portrait  
**Current Status:** Design draft for vertical slice planning  
**Game Title:** *System Reboot*  
**Title Status:** Temporary working title  

---

## 1. High-Level Concept

### 1.1 Vision Statement

*System Reboot* is a portrait-first mobile idle RPG about an artificial network mind evolving into a machine god through cybernetic growth, strategic auto-combat, research, challenges, and meaningful prestige resets.

The game is inspired by the long-term idle progression and challenge structure of *Idling to Rule the Gods*, but uses its own sci-fi evolution / cybernetics theme and focuses on a more interesting combat system. Combat remains stat-driven, as expected from an idle game, but improves engagement through pre-battle loadouts, enemy mechanics, drones, modules, and meaningful build variety.

The player should feel like they are not merely leveling up, but rewriting themselves into something larger: a distributed intelligence expanding through bodies, drones, facilities, networks, planets, and eventually machine divinity.

### 1.2 Logline

A mobile idle RPG where a growing network mind gathers resources, upgrades cybernetic systems, configures combat loadouts, defeats hostile machine factions, performs System Reboots, and earns permanent power on the path to machine godhood.

### 1.3 Genre

- Idle / incremental RPG
- Mobile RPG
- Semi-idle progression game
- Strategic auto-combat
- Prestige-based long-term progression

### 1.4 Primary Player Fantasy

The player is an expanding **network mind / machine god**.

The player starts as a limited artificial intelligence with restricted capacity and gradually expands into a distributed cybernetic intelligence capable of controlling avatars, drones, facilities, combat protocols, research systems, and eventually vast machine infrastructure.

---

## 2. Design Pillars

### 2.1 Meaningful Idle Growth

The player should always feel that the network continues operating, even while offline. Offline progress supports mobile-friendly play, but it does not replace major combat decisions, boss progression, challenge completions, or milestone choices.

### 2.2 Strategic Auto-Combat

Combat should not be a pure stat comparison. Stats are still foundational, but loadouts, modules, drones, and enemy mechanics should matter. The player prepares before battle, watches the result, learns from failure, and adjusts their build.

### 2.3 Meaningful Prestige

The prestige system, called **System Reboot**, should reset enough short-term progress to feel significant while preserving enough long-term unlocks to prevent repeated runs from becoming stale.

### 2.4 Gradual Complexity

The game should begin with simple mobile-friendly screens and gradually reveal deeper systems such as drones, crafting, specializations, challenges, leaderboards, and advanced automation.

### 2.5 Fair Monetization

The game may include monetization, but it must not be pay-to-win. Optional rewarded ads and premium currency are allowed only when they provide capped convenience, cosmetics, or non-competitive value.

### 2.6 Android-First Practicality

The initial target is Android portrait mobile. The game should be designed with future iOS and PC support in mind, but Android usability, performance, and portrait UX are the priority.

---

## 3. Target Audience

### 3.1 Primary Audience

- Idle / incremental fans
- Players who enjoy prestige loops and long-term optimization
- RPG buildcrafting players
- Strategy/stat optimization players
- Sci-fi and cyberpunk fans

### 3.2 Secondary Audience

- Casual mobile players who enjoy short check-ins
- Players who prefer progression without high mechanical execution requirements

### 3.3 Audience Design Goal

The game should primarily satisfy idle/incremental players who enjoy long-term growth, prestige systems, optimization, and challenges. RPG and strategy players should find depth through combat loadouts, drone builds, modules, enemy counters, and progression planning. Casual mobile players should be able to make progress through short check-ins, clear UI, offline gains, and low-pressure systems.

---

## 4. Platform and Technical Form

### 4.1 Primary Platform

- Android mobile
- Portrait orientation
- One-handed friendly UI where practical

### 4.2 Future Platforms

Possible future platforms:

- iOS
- PC

Future platforms should not drive MVP decisions unless the cost is low.

### 4.3 Engine and Language

- Engine: Unity
- Programming language: C#
- MVP save model: local save
- Backend: planned later, not required for MVP

### 4.4 Visual Form

Recommended style:

- UI-heavy sci-fi idle game
- Mostly text, icons, panels, cards, progress bars, and animated combat panels
- Limited character animation requirements
- Free, purchased, or AI-generated assets may be used
- Occasional illustrated key art for major bosses, backgrounds, avatars, or milestones

---

## 5. Tone and Presentation

### 5.1 Tone

The tone is **epic machine ascension with a mysterious atmosphere**.

The game should feel powerful, futuristic, and atmospheric without becoming too dark, comedic, or oppressive. The player is becoming something vast and machine-like, but the experience should remain comfortable for repeated idle/mobile sessions.

### 5.2 Content Rating Direction

Target content level:

- Teen sci-fi action
- No graphic violence
- No gore
- No sexual content
- No extreme horror
- No mature themes that would restrict broad mobile appeal

### 5.3 Naming Style

The game uses a mixed naming style:

- Early game: clear, readable sci-fi names
- Mid game: thematic but understandable names
- Late game: more mysterious, epic, or cryptic names

Examples:

| Stage | Naming Style | Examples |
|---|---|---|
| Early | Clear sci-fi | Shield Drone, Output Module, Data Research |
| Mid | Thematic but readable | Barrier Swarm, Recursive Targeting, Neural Forge |
| Late | Mysterious / epic | Black Signal Lattice, Singularity Choir, Throne of Static |

---

## 6. Core Game Loop

### 6.1 Primary Loop

1. Generate Matter, Energy, and Data.
2. Spend resources on training, upgrades, research, facilities, drones, or equipment.
3. Configure avatar modules, drones, and combat protocols.
4. Enter combat while the app is open.
5. Defeat enemies and bosses to unlock zones and systems.
6. Reach a System Reboot requirement.
7. Perform a System Reboot.
8. Earn Core Fragments.
9. Spend Core Fragments on permanent upgrades.
10. Start the next run faster, broader, or strategically different.

### 6.2 Session Length

The game should support flexible session lengths:

| Session Type | Duration | Activities |
|---|---:|---|
| Quick check-in | 1–3 minutes | Collect Offline Cache, buy upgrades, start timers, review progress |
| Normal session | 3–7 minutes | Adjust upgrades, fight several battles, check research, manage drones |
| Longer session | 7–15 minutes | Test loadouts, attempt bosses, plan Reboot, run challenges |
| Deep optimization | Optional | Build planning, challenge routes, endless simulation testing |

### 6.3 First Major Goal

The first major player goal is:

> Perform the first System Reboot.

The early game should guide the player toward this goal through resource generation, stat training, first combat encounters, research unlocks, first boss progression, and a clear Reboot requirement.

### 6.4 First Reboot Timing

A new player should reach their first System Reboot after roughly:

> **1–2 hours of total play/check-in time**

This value is **TBD / tunable** after testing.

### 6.5 First Reboot Reward Feeling

The first System Reboot should:

- Unlock a new system
- Give a noticeable boost
- Introduce Core Fragments
- Open the Core Fragment shop
- Make the second run clearly faster
- Introduce basic challenges, automation, and specializations in a staged way

---

## 7. Core Stats

The game uses sci-fi themed stat names rather than traditional RPG labels.

| Stat | Gameplay Meaning |
|---|---|
| **Processing** | Calculation speed, combat action rate, automation efficiency, and certain hacking effects |
| **Integrity** | Health / structural stability of the avatar or core |
| **Output** | Offensive power for weapons, drones, and attack modules |
| **Hardening** | Damage reduction and resistance to hostile effects |
| **Efficiency** | Resource generation, offline gains, cooldown reduction, and upgrade scaling |
| **Bandwidth** | Limits how many modules, drones, protocols, or automated systems can run at once |

### 7.1 Stat Design Rule

Stats should remain important because this is an idle game. However, stats should not completely override loadout decisions. Difficult enemies, bosses, and challenges should encourage the player to adapt modules, drones, and protocols.

---

## 8. Resources and Currencies

### 8.1 Core Resources

| Resource | Purpose |
|---|---|
| **Matter** | Basic construction material for upgrades, facilities, and physical expansion |
| **Energy** | Powers training, avatar systems, drones, facilities, and combat preparation |
| **Data** | Research currency used for technologies, automation, systems, and long-term upgrades |

### 8.2 Prestige Resource

| Resource | Purpose |
|---|---|
| **Core Fragments** | Prestige currency earned from System Reboot and major challenge completions |

Core Fragments represent stable pieces of the network mind’s architecture that survive a System Reboot.

### 8.3 Upgrade and System Resources

| Resource | Purpose |
|---|---|
| **Nanites** | Crafting and upgrade material for modules, drones, repairs, and equipment |
| **Drone Parts** | Unlocking and upgrading support drones |
| **Facility Components** | Facility construction and infrastructure upgrades |
| **Challenge Tokens** | Challenge reward currency for challenge-specific upgrades and unlocks |
| **Offline Cache** | Stored idle gains accumulated while away |

### 8.4 Premium Currency

| Resource | Purpose |
|---|---|
| **Quantum Cores** | Premium currency for non-pay-to-win purchases |

Quantum Cores may be used for cosmetics, UI themes, profile customization, loadout slot convenience, optional ad-related systems, and other fair monetization features. They must not purchase exclusive power, permanent stat multipliers, challenge advantages, leaderboard advantages, or paid-only progression unlocks.

---

## 9. Offline Progression

### 9.1 Offline Progress Philosophy

Offline progress should make the player feel that their network continued operating while away, but it should not replace active combat, build testing, challenge attempts, or major milestone decisions.

### 9.2 Systems That Progress Offline

Offline progress should include:

- Training stats
- Matter generation
- Energy generation
- Data/research generation
- Facility production
- Drone maintenance/upgrades
- Crafting timers
- Non-combat quests/missions
- Offline Cache generation

### 9.3 Systems That Do Not Progress Offline

Offline progress should not include:

- Combat zone clears
- Boss kills
- Challenge completions
- Leaderboard score farming
- New enemy unlocks
- Rewards that depend on combat victory

### 9.4 Offline Cap

Suggested caps, all **TBD / tunable**:

| Game Stage | Offline Cap |
|---|---:|
| Early game | 2–4 hours |
| Mid game | 8–12 hours |
| Late game | 24 hours |

Premium or convenience features may slightly extend caps, but should never provide unlimited or unfair progression power.

### 9.5 Placeholder Offline Formula

All values are **TBD / tunable**.

```txt
OfflineDuration = min(TimeAway, OfflineCap)
OfflineEfficiency = BaseOfflineEfficiency + UpgradeBonus + AchievementBonus
OfflineGain = ActiveGainRatePerSecond * OfflineDuration * OfflineEfficiency
```

Possible early placeholder:

```txt
BaseOfflineEfficiency = 0.50
EarlyOfflineCap = 2 hours
```

---

## 10. Combat System

### 10.1 Combat Overview

Combat is **auto-combat with pre-battle loadouts**.

The player configures their avatar/core, modules, drones, and protocols before combat. Once combat begins, it resolves automatically. The player observes the result, reviews logs, and adjusts the loadout if needed.

### 10.2 Combat Session Rule

Combat requires the game to be open.

Offline progression does not clear combat zones, defeat bosses, or complete challenges.

### 10.3 Combat Units

The player fights using:

- Main cybernetic avatar/core
- Support drones

The avatar represents the player’s main combat body. Drones provide build variety through roles such as damage, defense, repair, hacking, disruption, resource support, and automation.

### 10.4 Combat Improvement Goals

Combat should improve on basic idle combat by emphasizing:

- Build variety
- Enemy mechanics
- Meaningful stat progression
- Pre-battle strategy
- Readable combat results
- Quick mobile-friendly encounter length

### 10.5 Combat Length

Standard encounters should last:

> **10–30 seconds**

Bosses and major challenge fights may last longer.

### 10.6 Combat Failure

Combat failure depends on mode.

| Mode | Failure Result |
|---|---|
| Normal combat | No major penalty; player does not progress and may adjust loadout |
| Boss combat | No major penalty, but may require recharge/waiting briefly |
| Challenge combat | Penalty depends on challenge rules |
| Event/special mode | Mode-specific |
| Offline | Combat does not occur offline |

### 10.7 Loadout Slots and Bandwidth

Combat loadouts should start small and expand through Bandwidth.

| Stage | Loadout Complexity |
|---|---|
| Early game | 3 active slots |
| After early research | 4–5 active slots |
| After first System Reboot | More flexible slot rules |
| Mid game | Separate module, drone, and protocol slots |
| Late game | Presets, specialized builds, challenge-specific loadouts |

Design rule:

> Bandwidth acts as the main limit on active combat systems, making it both a combat stat and a buildcrafting constraint.

### 10.8 Combat Visual Direction

Combat is shown in a compact portrait-friendly battle simulation panel.

Possible UI elements:

- Player core/avatar on one side
- Enemy system/entity on the other
- Drone slots around the player core
- Integrity/shield bars
- Module activation indicators
- Animated beams, pulses, glitches, shield impacts, EMP bursts
- Scrolling combat log
- Speed controls or skip after result is determined

### 10.9 Placeholder Combat Formula

All values are **TBD / tunable**.

```txt
EffectiveOutput = Output * ModuleMultiplier * DroneSupportMultiplier
DamageBeforeDefense = EffectiveOutput * AbilityPower
DamageAfterDefense = max(1, DamageBeforeDefense - EnemyHardening)
FinalDamage = DamageAfterDefense * TypeModifier * StatusModifier
```

Possible simple MVP model:

```txt
Damage = max(1, AttackerOutput * ModulePower - DefenderHardening)
```

### 10.10 Status and Enemy Mechanics

Possible statuses and counters:

| Status / Mechanic | Effect |
|---|---|
| Shielded | Absorbs damage before Integrity |
| Firewall | Reduces hacking or status effects |
| Armor Plating | High Hardening, weak to breach modules |
| Corruption | Damages Integrity or reduces Processing over time |
| EMP | Temporarily disables drones or modules |
| Overheat | Reduces Output or action rate |
| Bandwidth Jam | Temporarily reduces active slot capacity |
| Regeneration | Restores Integrity unless countered |
| Swarm Pressure | Many low-power attacks requiring sustain or area damage |

---

## 11. Enemy Types and World Structure

### 11.1 Enemy Categories

| Enemy Type | Role |
|---|---|
| Rogue AIs | Early/mid enemies that mirror the player’s evolution path |
| Corporate Security Systems | Structured enemies with shields, firewalls, drones, and defense protocols |
| Alien Machine Lifeforms | Strange enemies with unusual mechanics and non-human technology |
| Ancient Defense Networks | Relic systems guarding forgotten tech and major unlocks |
| Rival Machine Gods | Late-game bosses and challenge targets |
| Human Military Forces | Physical-world enemies using weapons, armor, EMP tools, and formations |
| Digital Viruses / Corruption Entities | Status-heavy enemies attacking Integrity, Processing, Bandwidth, or automation |

### 11.2 Enemy Design Rule

Enemy categories should not only change visuals and names. Each category should teach or test different counters, such as shield breaking, anti-drone defense, firewall penetration, corruption resistance, armor breach, burst damage, or sustain.

### 11.3 Progression Structure

Combat progression uses:

- Campaign zones
- Endless modes

### 11.4 Campaign Zones

Campaign zones provide:

- Main progression
- System unlocks
- Light lore delivery
- Boss milestones
- Enemy faction introductions
- Reboot targets

### 11.5 Endless Simulations

Endless modes provide:

- Scaling enemies
- Farming
- Build testing
- Long-term optimization
- Future leaderboard categories
- Challenge preparation

### 11.6 Boss Archives

Boss Archives may allow players to re-fight major bosses under harder conditions later.

---

## 12. System Reboot / Prestige

### 12.1 Prestige Name

The prestige/rebirth system is called:

> **System Reboot**

A System Reboot is a controlled reset where the network mind intentionally wipes unstable short-term growth to rebuild with stronger permanent architecture.

### 12.2 What Resets

Normal System Reboot resets:

- Main level
- Current combat stats
- Basic resources
- Current zone/enemy progress
- Temporary upgrades
- Avatar/module levels
- Drone levels

### 12.3 What Persists

Normal System Reboot preserves:

- Unlocked modules
- Unlocked drone types
- Research unlocks
- Automation unlocks
- Challenge completions
- Core Fragments
- Permanent stat bonuses
- Cosmetics/titles
- Account-wide upgrades
- System unlocks

### 12.4 Anti-Sameness Rule

Each System Reboot should change at least one of the following:

1. The player starts faster through automation.
2. The player has access to a new module, drone, or protocol.
3. The player can choose a different build path.
4. New challenges or enemy mechanics become viable.
5. Offline progression improves.
6. The next run has a different strategic goal.

### 12.5 First Reboot Unlocks

The first System Reboot should unlock the next layer of the game in a staged way:

1. Core Fragment shop
2. First permanent upgrade purchase
3. Expanded automation
4. Basic challenge unlock
5. Specialization preview / first choice
6. Second-run goal

### 12.6 Placeholder Core Fragment Formula

All values are **TBD / tunable**.

```txt
CoreFragmentsEarned = floor((HighestProgressScore / RebootThreshold) ^ RebootExponent)
```

Possible simple MVP version:

```txt
CoreFragmentsEarned = floor(HighestZoneReached / 5) + FirstBossBonus
```

---

## 13. Challenge System

### 13.1 Challenge Structure

The game uses a mixed challenge system:

- Full reset challenges
- Partial reset challenges
- Restriction-only challenges

### 13.2 Challenge Philosophy

Challenges should create run variety, long-term goals, and alternate progression routes without making every run equally disruptive.

### 13.3 Challenge Unlock Timing

Challenges unlock gradually.

| Stage | Challenge Access |
|---|---|
| Before first System Reboot | No formal challenges; player learns the core loop |
| After first System Reboot | First basic challenge unlocks |
| After 2–3 Reboots | Multiple challenge categories unlock |
| After research milestones | Specialized restrictions unlock |
| After major bosses | Boss/challenge variants unlock |
| Late game | Advanced, layered, and repeatable challenges unlock |

### 13.4 Challenge Rewards

Challenges may reward:

- Permanent stat multipliers
- Automation features
- Modules
- Drones
- Combat options
- Core Fragments
- Offline progress bonuses
- Account-wide unlocks
- Cosmetics
- Titles
- Challenge Tokens

### 13.5 First Vertical Slice Challenges

The vertical slice should include three early challenge types:

#### Speed Reboot Challenge

Goal: Reach a System Reboot requirement within a target time or efficiency score.

Purpose: Tests the core idle/prestige loop and rewards optimization.

#### Limited Bandwidth Challenge

Goal: Complete a combat milestone while using fewer modules, drones, or automation slots.

Purpose: Reinforces Bandwidth as a buildcrafting constraint.

#### Resource Starvation Challenge

Goal: Reach a milestone while Matter, Energy, and/or Data generation is reduced.

Purpose: Forces players to rethink upgrade priorities.

### 13.6 Saved for Later

Not recommended for the first vertical slice:

- No Drone Challenge
- Combat Lock Challenge
- Offline Disabled Challenge

These should be added later after more systems exist.

### 13.7 Placeholder Challenge Reward Formula

All values are **TBD / tunable**.

```txt
ChallengeReward = BaseReward * DifficultyMultiplier * CompletionTierMultiplier
```

Possible reward tiers:

| Completion | Multiplier |
|---|---:|
| Complete | 1.0 |
| Fast | 1.25 |
| Optimized | 1.5 |

---

## 14. Progression Systems

### 14.1 Included Systems

The game may include all of the following, unlocked gradually:

- Training stats
- Combat zones/enemies
- Research tree
- Drone collection/upgrades
- Avatar/core upgrades
- Facilities/base-building
- Equipment/items
- Crafting
- Achievements
- Quests/missions
- Pets/companions
- Leaderboards
- Classes/jobs/specializations

### 14.2 Unlock Philosophy

Progression systems should unlock through a mixed model:

- Combat zone/boss milestones
- Research milestones
- System Reboot count
- Challenge completions
- Player level/main progression

### 14.3 Suggested Unlock Structure

| Stage | Systems |
|---|---|
| Early game | Training, combat zones, research, avatar/core upgrades, achievements |
| After early progress | Drones, equipment, quests/missions, facilities |
| After first System Reboot | Crafting, specializations, advanced automation, challenges, Core Fragment shop |
| Mid/late game | Leaderboards, companions, advanced facilities, events, high-tier specializations |

---

## 15. Research Tree

### 15.1 Role

Research is a long-term progression system that unlocks:

- Automation
- Modules
- Drone types
- Facilities
- Combat options
- Resource efficiency
- Offline improvements
- Reboot-related upgrades
- Specializations
- Challenge modifiers

### 15.2 Reboot Behavior

Research unlocks usually persist through System Reboot.

Research may have temporary cycle upgrades that reset, but core unlocks should persist.

### 15.3 Research Design Rule

Research should change future runs by unlocking new tools, not by forcing players to repeat the same research every cycle.

---

## 16. Drones

### 16.1 Role

Drones are flexible support units for:

- Combat
- Resource generation
- Facility automation
- Offline progress
- Build specialization

### 16.2 Drone Categories

| Drone Type | Main Role |
|---|---|
| Assault Drones | Combat damage and pressure |
| Barrier Drones | Shields and mitigation |
| Repair Drones | Integrity recovery and sustain |
| Hacking Drones | Debuffs, firewall damage, enemy disruption |
| Harvest Drones | Matter/Nanite/resource generation |
| Research Drones | Data generation and research efficiency |
| Facility Drones | Automation and production support |
| Recon Drones | Offline Cache improvement, scouting, enemy info |
| Challenge Drones | Specialized drones unlocked from challenge rewards |

### 16.3 Drone Design Rule

Drone slots, drone types, and drone behavior should create meaningful loadout choices without requiring manual micromanagement during combat.

---

## 17. Equipment and Items

### 17.1 Equipment Role

Equipment provides:

- Stat bonuses
- Build-defining modifiers
- Crafting/upgrading goals
- Avatar specialization
- Drone synergy

Equipment should not replace core idle progression, but it should support build identity.

### 17.2 Equipment Categories

| Equipment Type | Purpose |
|---|---|
| Core Frames | Main avatar/core bonuses, Integrity, Hardening, Bandwidth |
| Output Arrays | Damage types and attack behavior |
| Shield Matrices | Defense, regeneration, resistance, anti-burst builds |
| Processing Cores | Speed, automation, cooldowns, hacking efficiency |
| Drone Interfaces | Drone count, drone scaling, support behavior |
| Utility Relays | Offline gains, resource efficiency, facility bonuses |
| Special Artifacts | Rare build-changing modifiers from bosses/challenges |

### 17.3 Equipment Design Rule

Early equipment should be easy to understand. Later equipment can introduce stronger synergies such as drone-focused builds, shield conversion, hacking setups, burst Output builds, or high-Efficiency farming builds.

---

## 18. Crafting

### 18.1 Crafting Role

Crafting starts simple and expands gradually.

Crafting can:

- Upgrade equipment
- Create modules/equipment
- Improve drones
- Convert resources/materials
- Create cosmetics
- Unlock special recipes from challenges

### 18.2 Crafting Design Rule

Crafting should support long-term goals and build customization, but it should not become so complex that it distracts from the idle/prestige loop.

---

## 19. Specializations

### 19.1 Role

Specializations represent long-term identity choices for the network mind.

They unlock after the player understands the core loop and has completed at least one System Reboot.

### 19.2 Example Specializations

| Specialization | Focus |
|---|---|
| War Protocol | Combat Output, assault drones, boss damage |
| Fortress Protocol | Integrity, Hardening, shields, sustain |
| Swarm Protocol | Drone slots, drone scaling, multi-role drone builds |
| Architect Protocol | Facilities, Matter, Energy, production efficiency |
| Oracle Protocol | Data, research speed, enemy analysis, automation |
| Void Protocol | Challenge modifiers, corruption resistance, risky high-reward builds |

### 19.3 Switching Rules

Specialization switching starts restrictive and becomes easier later.

| Stage | Switching Rule |
|---|---|
| First unlock | Choose one specialization after System Reboot |
| Early prestige | Switch only during System Reboot |
| Mid game | Switch with Core Fragment cost or cooldown |
| Later research | Unlock specialization presets |
| Late game | Reduce switching friction through account-wide upgrades |

### 19.4 Specialization Design Rule

Specializations should create identity and planning, but players should not feel forced to restart because of one bad choice.

---

## 20. Facilities / Base-Building

### 20.1 Role

Facilities represent the network mind’s growing infrastructure.

Possible facility roles:

- Matter generation
- Energy generation
- Data processing
- Drone assembly
- Nanite production
- Offline Cache storage
- Research speed
- Crafting support
- Automation control

### 20.2 Reboot Behavior

Facilities may have short-term levels that reset during System Reboot, while facility types, blueprints, automation, and permanent infrastructure upgrades persist.

### 20.3 Design Rule

Facilities should support idle planning and resource generation without becoming a separate city-builder that overwhelms the main game.

---

## 21. Quests and Missions

### 21.1 Role

Quests and missions provide:

- Tutorial guidance
- Milestone goals
- Research direction
- Combat experimentation
- Challenge introduction
- Light lore delivery
- Optional recurring objectives

### 21.2 Daily/Weekly Philosophy

Daily and weekly objectives may exist later, but they should not feel like mandatory chores.

### 21.3 Quest Design Rule

Quests should point players toward interesting decisions, not force repetitive login chores.

---

## 22. Achievements

### 22.1 Role

Achievements provide:

- Player guidance
- Small permanent bonuses
- Cosmetics
- Titles
- Long-term goals
- Recognition for experimentation

### 22.2 Achievement Categories

| Category | Purpose |
|---|---|
| Progression achievements | Zones, Reboots, research tiers, milestones |
| Combat achievements | Bosses, counters, build experimentation |
| Challenge achievements | Completion, speed, restriction mastery |
| Collection achievements | Modules, drones, cosmetics, lore |
| Efficiency achievements | Fast Reboots, optimized runs, resource milestones |
| Hidden achievements | Optional discoveries and late-game prestige goals |

### 22.3 Achievement Design Rule

Achievement bonuses should be small, readable, and additive. They should not become mandatory for normal progression.

---

## 23. Story and Lore

### 23.1 Story Importance

Story is light.

The game uses narrative flavor to support progression without interrupting idle gameplay.

### 23.2 Story Delivery

| Story Element | Use |
|---|---|
| System messages | Short atmospheric updates |
| Research logs | Optional lore attached to technology unlocks |
| Enemy descriptions | Explain factions and mechanics |
| Zone intros | Brief context for new campaign regions |
| Milestone messages | Mark major ascension moments |
| Challenge descriptions | Explain restrictions in-universe |

### 23.3 Story Design Rule

Story should create mystery and atmosphere around machine ascension, but it should not interrupt the core idle/combat/progression loop with long cutscenes or heavy dialogue.

---

## 24. User Interface

### 24.1 UI Complexity

The UI starts simple and gradually reveals depth.

| Stage | UI Depth |
|---|---|
| Early game | Core, Training, Combat, Research |
| After drones unlock | Loadout screen, drone slots, module cards |
| After System Reboot | Prestige screen, Core Fragment upgrades, Reboot preview |
| After challenges unlock | Challenge list, rules, rewards, restrictions |
| Mid/late game | Stat breakdowns, automation, build presets, logs |

### 24.2 Portrait Mobile UI Goals

- Readable on Android phones
- Clear resource display
- Large tap targets
- Bottom navigation or tab-based structure
- Minimal early screen clutter
- Tooltips for deep stats
- Combat log for readability
- Clear labels for icons
- One-handed friendly where practical

### 24.3 Suggested Main Screens

| Screen | Purpose |
|---|---|
| Core | Main avatar stats, current progression, key actions |
| Training | Stat growth and idle training |
| Resources | Matter, Energy, Data, Offline Cache, production rates |
| Combat | Zones, enemies, loadouts, battle simulation |
| Research | Tech tree and unlocks |
| Drones | Drone collection, upgrades, assignments |
| Facilities | Production and automation infrastructure |
| Equipment | Items, modules, crafting links |
| Reboot | System Reboot preview, Core Fragment gain, permanent upgrades |
| Challenges | Challenge rules, rewards, restrictions |
| Achievements | Milestones, small bonuses, cosmetics |
| Settings | Save, accessibility, audio, account later |

---

## 25. Accessibility

### 25.1 Planned Accessibility Features

| Feature | Purpose |
|---|---|
| Text scaling/readable fonts | Keeps stats and values readable |
| Colorblind-friendly indicators | Uses icons, labels, and shapes in addition to color |
| Reduced motion option | Reduces combat effects, screen shake, pulses, animated UI |
| Clear icon labels/tooltips | Helps with modules, drones, resources, research |
| Combat log | Lets players review auto-combat results |
| Readable portrait UI | Prevents early screen overload |

### 25.2 Accessibility Design Rule

Because the game is UI-heavy and stat-driven, readability is a core design requirement, not a polish-only feature.

---

## 26. Tutorial

### 26.1 Tutorial Style

The tutorial uses:

- Tutorial missions
- Contextual tips
- Optional info panels
- Combat result suggestions
- Progression nudges

### 26.2 Tutorial Design Rule

Tutorials should be helpful but not controlling. Players should be able to skip, dismiss, or revisit tutorial explanations.

### 26.3 Tutorial Mission Examples

| Mission | Teaches |
|---|---|
| Generate Matter | Resource production |
| Train Processing | Stat training |
| Enter first combat | Combat access |
| Equip first module | Loadout basics |
| Research first upgrade | Research tree |
| Collect Offline Cache | Offline progress |
| Defeat first boss | Milestone combat |
| Perform System Reboot | Prestige loop |

---

## 27. Audio

### 27.1 Audio Style

The game uses mixed audio:

- Ambient sci-fi UI audio for idle/progression screens
- Stronger electronic combat music for battles, bosses, and major milestones

### 27.2 Audio Categories

| Area | Audio Direction |
|---|---|
| Idle/progression screens | Ambient sci-fi, low drones, soft pulses |
| UI interactions | Clean clicks, data chirps, confirmation tones |
| Upgrades/research | Rising synths, unlock stingers, digital activation |
| Normal combat | Light electronic combat loop, laser/shield/glitch sounds |
| Boss combat | Stronger synth/electronic intensity |
| System Reboot | Deep machine shutdown/restart sequence |
| Challenges | Tense simulation audio |

### 27.3 Audio Design Rule

Audio should support long mobile sessions without becoming tiring.

---

## 28. Monetization

### 28.1 Monetization Philosophy

The game may monetize, but must not be pay-to-win.

### 28.2 Rules

- No forced ads
- Optional rewarded ads only
- Premium currency allowed
- No paid-only power upgrades
- No exclusive stat multipliers from payment
- No paid-only challenge advantages
- No paid leaderboard advantages
- No paid-only progression unlocks

### 28.3 Premium Currency

Premium currency:

> **Quantum Cores**

### 28.4 Acceptable Purchases

| Purchase Type | Allowed? | Notes |
|---|---:|---|
| Cosmetics | Yes | Preferred |
| Avatar/drone skins | Yes | No stat advantage |
| UI themes | Yes | Cosmetic |
| Profile customization | Yes | Cosmetic/status |
| Extra loadout slots | Yes | Convenience; must be carefully balanced |
| Remove ads | Yes | Convenience |
| Offline Cache extension | Maybe | Capped and non-P2W |
| Crafting timer reduction | Maybe | Small/capped |
| Resource bundles | Risky | Avoid or heavily cap |
| Combat stat boosts | No | Too close to P2W |
| Challenge boosts | No | Must preserve fairness |
| Leaderboard boosts | No | Not allowed |

### 28.5 Rewarded Ads

Rewarded ads may provide:

- Temporary resource boosts
- Offline Cache extension
- Cosmetic/ad token progress
- Small crafting timer reductions
- Very small daily Quantum Core drip, if balanced carefully

Rewarded ads must be capped, optional, and excluded from competitive scoring modes where relevant.

---

## 29. Leaderboards

### 29.1 Status

Leaderboards are a future backend-supported feature, not part of the local MVP.

### 29.2 Scope

Leaderboards apply only to:

- Challenge modes
- Endless simulations
- Speed Reboot records
- Boss Archive scores
- Efficiency score modes

### 29.3 Rewards

Leaderboard rewards must be cosmetic/status only:

- Titles
- Profile badges
- Seasonal frames
- Banners
- Cosmetic effects

No progression power.

### 29.4 Backend Requirement

Leaderboards should only be added once backend validation exists. Unity should not be trusted to submit final scores or progression results.

---

## 30. Social Features

### 30.1 Status

Social and guild features are not part of the MVP.

### 30.2 Future Possibilities

Possible future social-lite features:

- Friend/profile viewing
- Build comparison
- Sharing challenge results

No guilds/clans are planned for MVP.

### 30.3 Design Rule

The core experience should not depend on multiplayer, guilds, or social obligations.

---

## 31. Events

### 31.1 Status

Events are a later feature, not part of MVP.

### 31.2 Event Direction

Events may focus on:

- Special challenge rules
- Unusual modifiers
- Themed enemies
- Temporary goals
- Cosmetic/status rewards

### 31.3 FOMO Rule

Events should avoid harsh FOMO. They should not contain exclusive permanent power that permanently disadvantages players who miss them.

### 31.4 Replayability

Event content may return through archives or rotations.

---

## 32. Save System

### 32.1 MVP Save Direction

The MVP uses a local save system, designed to be cloud-ready later.

### 32.2 Save Principles

- Include save version number
- Structure save data by major system
- Keep progression data separate from UI-only state
- Prepare for future cloud sync
- Use moderate local save protection
- Avoid client-side design choices that prevent future backend authority

### 32.3 Suggested Save Modules

Possible save sections:

```txt
SaveData
  version
  playerProfile
  resources
  stats
  training
  combatProgress
  research
  drones
  facilities
  equipment
  crafting
  systemReboot
  challenges
  achievements
  quests
  settings
  monetizationLocalFlags
  timestamps
```

### 32.4 Moderate Local Anti-Cheat

MVP local save protection should include:

- Save version number
- Basic encryption or obfuscation
- Save checksum/hash
- Last saved timestamp
- Offline progress sanity checks
- Resource gain caps
- Duplicate claim protection for important rewards
- Migration path for future cloud saves

### 32.5 Save Protection Rule

The goal is not perfect anti-cheat during local MVP. The goal is to discourage casual save editing and prepare for future backend validation.

---

## 33. Backend Plan

### 33.1 Backend Status

Backend is planned later.

MVP is local save only.

### 33.2 Future Backend Priorities

Suggested priority order:

1. Cloud saves
2. Account login
3. Purchase validation
4. Remote config / balance values
5. Events and daily rewards
6. Leaderboards
7. Anti-cheat validation

### 33.3 Backend Authority Rule

Once backend systems exist, the Unity client should not be treated as the source of truth for:

- Purchases
- Rewards
- Currency balances
- Cooldowns
- Leaderboard scores
- Final progression state
- Reward claims
- Challenge results
- Anti-cheat-sensitive data

---

## 34. Vertical Slice Scope

### 34.1 MVP Type

The first playable version is a:

> **Vertical slice**

It should prove the core loop with enough polish and interconnected systems to test the actual game experience.

### 34.2 Vertical Slice Contents

The vertical slice should include:

- Core idle loop
- Offline Cache
- Basic stat training
- Matter, Energy, Data
- Early combat zones
- Auto-combat with pre-battle loadouts
- Main avatar/core upgrades
- Small drone system
- Basic research tree
- First System Reboot
- Core Fragments
- Core Fragment shop
- Basic challenge system
- Achievements
- Android portrait UI
- Local save system
- Placeholder/free/AI-generated assets
- Moderate local save protection

### 34.3 Not Required for Vertical Slice

Not required yet:

- Full monetization implementation
- Backend
- Cloud saves
- Leaderboards
- Events
- Social features
- Large content library
- Full crafting depth
- Full specialization depth
- Advanced companions/pets
- Full live-service structure

### 34.4 First Playable Loop

1. Generate Matter, Energy, and Data.
2. Spend resources on basic upgrades/training.
3. Configure a simple avatar/module loadout.
4. Fight 10–30 second combat encounters.
5. Defeat an early boss.
6. Unlock basic research.
7. Reach first System Reboot.
8. Earn Core Fragments.
9. Buy permanent upgrades.
10. Start a stronger second run.
11. Save/load locally.
12. Collect capped Offline Cache.

---

## 35. Development Priority

### 35.1 First Development Goal

Build a small complete playable loop rather than isolated systems.

### 35.2 Recommended Development Order

1. Local save foundation
2. Time/offline calculation foundation
3. Resource generation
4. Basic stat training
5. Basic upgrade spending
6. Combat loadout data model
7. Simple combat resolver
8. Combat UI panel
9. Early enemy/zone progression
10. Research unlocks
11. System Reboot
12. Core Fragment shop
13. First challenge types
14. Basic achievements
15. Accessibility/settings pass
16. Polish and balance iteration

### 35.3 Formula Complexity

MVP formulas should start simple and become more complex later only after the core loop is proven fun.

All formulas in this document are placeholders and **TBD / tunable**.

---

## 36. Technical Appendix: Unity / C# / Cursor Guidance

This section is not a full technical design document. It exists to guide implementation without over-engineering the MVP.

### 36.1 Unity MVP Approach

- Build local MVP first.
- Use Android portrait as the primary target.
- Keep systems modular and readable.
- Avoid giant manager classes.
- Keep game logic testable where practical.
- Use clear data models for resources, stats, upgrades, research, combat, drones, challenges, and saves.
- Keep formulas explicit and easy to rebalance.
- Separate UI state from progression state.

### 36.2 Future Backend Readiness

Although backend is later, save structures and game data should avoid assumptions that make backend migration difficult.

Future backend will likely handle:

- Cloud saves
- Account login
- Purchase validation
- Remote config
- Events
- Leaderboards
- Anti-cheat validation

### 36.3 Client Trust Boundary

During MVP, the game is local. Later, once backend exists, the Unity client should not be considered authoritative for sensitive data.

Backend should eventually validate:

- Purchases
- Rewards
- Currency changes
- Leaderboard scores
- Challenge results
- Cooldowns
- Reward claims
- Progression-sensitive actions

### 36.4 Data Contract Readiness

If backend APIs are added later, Unity-facing data should use stable, explicit structures. Avoid relying on anonymous, inconsistent, or deeply complex data shapes that are hard to serialize or migrate.

### 36.5 Suggested Future Backend Modules

If a C# backend is added later, likely modules include:

```txt
Players
Auth
CloudSaves
Economy
Purchases
Inventory
Research
Challenges
Leaderboards
Events
RemoteConfig
```

### 36.6 Technical Rule

Do not introduce backend complexity, multiplayer infrastructure, real-time services, queues, Redis, SignalR, or other large architecture pieces unless a clear gameplay requirement exists.

---

## 37. Open Balance Values

All values below are placeholders and should be tuned after playtesting.

| Value | Placeholder | Status |
|---|---:|---|
| First System Reboot timing | 1–2 hours | TBD |
| Normal combat length | 10–30 seconds | TBD |
| Early offline cap | 2–4 hours | TBD |
| Mid offline cap | 8–12 hours | TBD |
| Late offline cap | 24 hours | TBD |
| Early loadout slots | 3 | TBD |
| First challenge unlock | After first Reboot | TBD |
| First vertical slice challenge count | 3 | TBD |

---

## 38. Confirmed Decisions Summary

| Category | Decision |
|---|---|
| Theme | Sci-fi evolution / cybernetics |
| Player fantasy | Expanding network mind / machine god |
| Platform | Android first |
| Orientation | Portrait |
| Engine/language | Unity / C# |
| Mobile style | Semi-idle |
| Offline progress | All non-combat systems, capped |
| Combat | App open only |
| Combat control | Auto-combat with pre-battle loadouts |
| Combat units | Avatar/core + support drones |
| Combat focus | Build variety, enemy mechanics, meaningful stats |
| Challenge system | Mixed full reset, partial reset, restriction-only |
| Challenge rewards | Stats, automation, combat unlocks, prestige currency, offline bonuses, account unlocks, cosmetics/titles |
| Prestige name | System Reboot |
| Prestige currency | Core Fragments |
| Premium currency | Quantum Cores |
| Monetization | Optional ads and premium currency, no P2W |
| Art style | UI-heavy sci-fi, icons, animated panels |
| Tone | Epic machine ascension + mysterious atmosphere |
| Story | Light lore and atmospheric text |
| UI complexity | Starts simple, reveals depth |
| Save system | Cloud-ready local save |
| Backend | Later, cloud saves/purchases first |
| MVP scope | Vertical slice |
| GDD style | Detailed living design document |
| Formula style | Simple for MVP, expandable later |
| Local anti-cheat | Moderate |
| Content rating | Teen sci-fi action |
| Accessibility | Text scaling, colorblind indicators, reduced motion, labels, combat log |
| Tutorial | Tutorial missions + contextual tips |

---

## 39. Key Risks

### 39.1 Scope Creep

The design includes many systems. The vertical slice must stay focused on the core loop.

Mitigation:

- Build only the smallest version of each necessary system.
- Delay leaderboards, events, social features, advanced crafting, and full monetization.

### 39.2 Repetitive Reboot Runs

System Reboot may become repetitive if every run follows the same route.

Mitigation:

- Preserve unlocks.
- Add new automation.
- Unlock build options.
- Introduce challenges gradually.
- Use research and specializations to change run strategy.

### 39.3 Combat Complexity

Combat could become too complex for portrait mobile.

Mitigation:

- Start with 3 slots.
- Use Bandwidth to gradually increase complexity.
- Provide combat logs and result tips.
- Keep normal encounters short.

### 39.4 Pay-to-Win Pressure

Premium currency and rewarded ads can damage trust if they affect power too much.

Mitigation:

- Focus on cosmetics and convenience.
- Cap rewarded ads.
- Exclude ad/premium boosts from competitive modes.
- Avoid paid-only power.

### 39.5 Backend Migration

A local MVP can become hard to migrate if save data is messy.

Mitigation:

- Use versioned save data.
- Separate systems clearly.
- Avoid client-only assumptions for future leaderboard/purchase systems.

---

## 40. Next Design Tasks

Before implementation, the next useful design tasks are:

1. Define the first 30–60 minutes of progression step-by-step.
2. Design the first resource generators and costs.
3. Design the first 5–10 enemies.
4. Design the first boss.
5. Define the first 10 modules.
6. Define the first 4 drone types.
7. Draft the first research tree tier.
8. Draft the first System Reboot upgrade shop.
9. Define the first 3 challenges in detail.
10. Define the local save data structure.
11. Create Unity UI wireframes for portrait screens.
12. Prototype the combat resolver in simple C#.

---

# End of Document
