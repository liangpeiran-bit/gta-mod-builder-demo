# Final Review Checklist

Review only alignment between `DESIGN.md` and the C# implementation. Do not review style or suggest speculative improvements.

## Check DESIGN -> Code

- Every row in `Interaction Map` has a matching code path.
- Every declared chat trigger checks the right keyword or phrase.
- Every declared gift trigger checks the exact confirmed `gift_id`.
- Every rule in `Gameplay Rules` is implemented: state, timers, scoring, rounds, win/loss, voting, wave spawning, randomization, or per-tick behavior.
- Every `Game Effects` bullet has a visible in-game effect in code.
- Every `Edge Cases & Boundaries` item is honored.

## Check Code -> DESIGN

- Code does not listen for extra triggers that are absent from `DESIGN.md`.
- Code does not implement major effects absent from `DESIGN.md`.
- Code does not add gameplay rules, score changes, round transitions, or persistent state that are absent from `DESIGN.md`.
- Comments / variable names next to gift ids match the gift name in `DESIGN.md`.

## Acceptable Output

If aligned:

```text
alignment OK
```

If not aligned:

```text
misalignment:
1. DESIGN.md <section> <-> ModProject/<file>:<line>: <what DESIGN says, what code does, why they differ>
```

Report at most 5 findings. Omit anything that is uncertain, stylistic, or not grounded in both DESIGN and code.
