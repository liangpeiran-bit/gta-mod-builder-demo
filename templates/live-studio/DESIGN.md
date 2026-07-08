# DESIGN

This file is the source of truth for what the mod does. Fill it before writing code and keep it current as the design evolves.

## Mod Summary

<!-- A short description of the mod and the core viewer-facing fantasy. -->

## Interaction Map

<!--
Trigger format:
- Gift trigger: `<name> (gift_id: <id>)`, e.g. `rose (gift_id: 5655)`.
- Multi-id gift trigger: `<name> (gift_id: <id1>, <id2>)`; any listed id fires the behavior.
- Chat trigger: `chat "<keyword>"`.
- Unresolved gift: `<name> (gift_id: unresolved - <reason>)`.
-->

| Trigger | Behavior | Notes |
| --- | --- | --- |
| <!-- e.g. rose (gift_id: 5655) --> | <!-- one-line plain description --> | <!-- cooldown, scope, limits if any --> |

## Gameplay Rules

<!--
Use this section when the mod is more than a one-shot effect mapping.
Describe the rules of the custom gameplay system: state, timers, score, rounds,
win/loss conditions, voting, randomization, spawning waves, progression, or
ongoing Tick behavior. Write "None - this is a simple triggered-effect mod" if
there is no persistent gameplay system.
-->

-

## Game Effects

<!-- One bullet per Interaction Map row. Describe what visibly changes in game. Do not name SHVDN classes or methods here. -->

-

## Edge Cases & Boundaries

<!-- Real decisions the implementation will hit: cooldowns, dead player, combo gifts, queue pressure, repeated chat spam. -->

-

## Design Notes

<!-- Optional context, constraints, or known limitations. Leave empty if none. -->

-

## Change Log

<!-- Dated entries, newest at the bottom. Format: YYYY-MM-DD - one-line summary. -->

-
