# How to Run

This is source-only C#.

## Console quick test

Create a console app and call:

```csharp
Game.Slice.FoundationVerticalSlice.RunToConsole();
```

## Unity

Copy `src/Game` into:

```text
Assets/Scripts/Game
```

Then create a MonoBehaviour adapter that calls domain/application services.

Do not move gameplay rules into MonoBehaviours.
