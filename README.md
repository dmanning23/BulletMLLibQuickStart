# BulletMLLib Quick Start

The smallest working example of [BulletMLLib](https://www.nuget.org/packages/BulletMLLib) in a MonoGame project.

[BulletML](http://www.asahi-net.or.jp/~cs8k-cyu/bulletml/index_e.html) is an XML language for describing bullet patterns in shoot 'em ups. You write the pattern in XML, and BulletMLLib runs it. It creates the bullets, moves them, and fires new ones on schedule. Your game only has to draw them and decide when they die.

This project shows the minimum code needed to do that. It loads every pattern in the [BulletMLExamples](https://github.com/dmanning23/BulletMLExamples) repo and lets you flip through them while you steer a "ship" that the bullets aim at.

## Running the sample

### Requirements

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- Git

### Get the code

The example patterns come from a git submodule, so clone with submodules:

```sh
git clone --recurse-submodules <this repo>
```

If you already cloned without them:

```sh
git submodule update --init
```

### Build and run

```sh
cd BulletMLQuickStart/BulletMLQuickStart
dotnet build
cd bin/Debug/net9.0
dotnet BulletMLQuickStart.dll
```

The patterns are loaded from `../../../../../externals/BulletMLExamples`, **relative to the working directory**. That path resolves correctly when the app runs from its build output folder, which is where the VS Code / Visual Studio debugger launches it. If the app starts and immediately crashes, the working directory is the likely problem.

### Controls

| Key (keyboard) | Action |
|---|---|
| Arrow keys | Move the ship (bullets that aim will follow it) |
| D / F | Previous / next pattern |
| A | Restart the current pattern |
| C / V | Decrease / increase the bullet scale |
| Y | Slow down time |
| Esc | Quit |

A gamepad works too: the shoulder buttons switch patterns, **A** restarts, and the triggers change the scale. Keyboard bindings come from [HadoukInput](https://www.nuget.org/packages/HadoukInput)'s default player-one keymap.

The top-left of the screen shows the current pattern file, the number of live bullets, and the current time speed and scale.

## How it works

Adding BulletMLLib to a game takes four pieces. Each one maps to a file in this project.

### 1. A bullet class: `Mover.cs`

Inherit from `BulletMLLib.Bullet`. You must provide:

- `X` and `Y`: where the bullet is. BulletMLLib reads and writes these every update, so back them with whatever your game uses for position (here, a `Vector2`).
- `PostUpdate()`: called after BulletMLLib moves the bullet. Use it for your own per-bullet logic. The sample marks bullets as unused once they leave the screen.

```csharp
public class Mover : Bullet
{
    public Vector2 pos;

    public override float X { get => pos.X; set => pos.X = value; }
    public override float Y { get => pos.Y; set => pos.Y = value; }

    public bool Used { get; set; }

    public Mover(IBulletManager manager) : base(manager) { }

    public override void PostUpdate()
    {
        if (X < 0 || X > screenWidth || Y < 0 || Y > screenHeight)
            Used = false;
    }
}
```

### 2. A bullet manager: `MoverManager.cs`

Implement `BulletMLLib.IBulletManager`. BulletMLLib calls it whenever a pattern needs to create or remove a bullet, or needs to know where the player is.

| Member | What it's for |
|---|---|
| `CreateBullet()` | A pattern fired a new bullet. Create one, add it to your list, return it. |
| `CreateTopBullet()` | Create the invisible "emitter" bullet that runs a pattern's top-level action. Keep these in a separate list: you usually don't draw them, and you discard them when `TasksFinished()` is true. |
| `RemoveBullet(IBullet)` | A pattern says this bullet is done (`<vanish/>`). Mark it for removal. |
| `PlayerPosition(IBullet)` | Where aimed bullets (`type="aim"`) should point. |
| `Rand` | The random number source for `$rand` in patterns. |
| `GameDifficulty` | Returns 0–1. This is the value of `$rank` in patterns, so harder difficulties can fire faster or denser bullets. |
| `CallbackFunctions` | Extra `$name` values your patterns can use, for example `$tier`. |

The manager also owns the update loop. Each frame it calls `Update()` on every bullet, then clears out the dead ones:

```csharp
public void Update()
{
    foreach (var m in movers) m.Update();
    foreach (var m in topLevelMovers) m.Update();

    movers.RemoveAll(m => !m.Used);
    topLevelMovers.RemoveAll(m => m.TasksFinished());
}
```

`MoverManager` also has `TimeSpeed` (for slowdown and speedup) and `Scale` (to resize a pattern that was written for a different screen size). They aren't required, but most games want them.

### 3. Loading patterns: `Game1.LoadContent`

A `BulletPattern` is a parsed BulletML file. Load each file once and reuse it:

```csharp
var pattern = new BulletPattern(moverManager);
pattern.ParseXML("path/to/pattern.xml");
```

`ParseXML` also accepts a MonoGame `ContentManager` as its second argument, if you'd rather ship patterns through the content pipeline than as loose files.

### 4. Firing a pattern: `Game1.AddBullet`

To start a pattern, create a top-level bullet where the enemy is and point it at the pattern's root node:

```csharp
var emitter = (Mover)moverManager.CreateTopBullet();
emitter.pos = enemyPosition;
emitter.InitTopNode(pattern.RootNode);
```

After that, call `moverManager.Update()` once per frame in `Update()`, and draw everything in `moverManager.movers` in `Draw()`. That's the whole integration.

## Taking it into your own game

This sample keeps things as simple as possible. In a real game you'll probably want to:

- **Pool bullets.** `CreateBullet` allocates a new `Mover` every time. Bullet-heavy patterns create thousands, so reuse objects from a pool instead.
- **Add collision.** Check `movers` against the player's hitbox each frame.
- **Move the emitter.** Update the top-level bullet's position every frame to make patterns follow a moving enemy.
- **Hook up difficulty.** Point `GameDifficulty` at your game's difficulty setting so `$rank` means something.
- **Remove the static references.** `Mover` and `Myship` read the screen size from `Game1.graphics` to keep the sample short. Pass the bounds in instead.

## Writing patterns

All the patterns loaded here live in [BulletMLExamples](https://github.com/dmanning23/BulletMLExamples). They're a good starting point for your own. The [BulletML reference](http://www.asahi-net.or.jp/~cs8k-cyu/bulletml/bulletml_ref_e.html) documents every element.

## Dependencies

All from NuGet:

- [BulletMLLib](https://www.nuget.org/packages/BulletMLLib): the BulletML runtime
- [MonoGame.Framework.DesktopGL](https://www.nuget.org/packages/MonoGame.Framework.DesktopGL)
- [HadoukInput](https://www.nuget.org/packages/HadoukInput): keyboard and gamepad input
- [FontBuddy](https://www.nuget.org/packages/FontBuddy): on-screen text
- [GameTimer](https://www.nuget.org/packages/GameTimer): game clock

Only BulletMLLib and MonoGame are needed for your own integration. The others just run this sample's UI.

## License

See [LICENSE.txt](LICENSE.txt).
