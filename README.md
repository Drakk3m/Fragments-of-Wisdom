# Fragments of Wysdom

This repository contains the C# domain/application foundations of **Fragments of Wysdom**, a knowledge- and memory-driven RPG. It is **not yet a verified playable Unity vertical slice**.

## Single source of truth

**[Game Design, Creative Vision & Architecture Bible](docs/Fragments_of_Wysdom_Bible.md)** contains the approved and proposed design, architecture, code-grounded implementation matrix, executed audit, compatibility requirements, development workflow, backlog and archived technical source records.

Read the Bible before making changes. Treat historical sections as provenance; the active chapters and executed audit take precedence over superseded statements.

## Verification

```sh
dotnet restore Fragments-of-Wisdom.sln
dotnet build Fragments-of-Wisdom.sln --no-restore -v:minimal
dotnet run --project src/Game.SmokeTests/Game.SmokeTests.csproj --no-build
```

Do not confuse passing backend smoke tests with Unity Play Mode or playable gameplay verification.
