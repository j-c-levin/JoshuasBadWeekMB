# Joshua's Bad Week - Survival Game

A simple 2D survival game built with MonoGame where the player must survive for 2 minutes to win.

## Game Overview

**Objective**: Survive for 2 minutes (120 seconds) to win the game.

**Controls**:
- Use arrow keys to move the yellow square in all 8 directions
- The square rotates to face the direction it's moving
- The square cannot move past the screen boundaries

**UI Elements**:
- **Health**: Displayed in the top-left corner (starts at 10)
- **Timer**: Displayed in the top-right corner (counts down from 120 seconds)
- **Win Message**: "You Win!" appears in the center when the timer reaches 0

## How to Run

1. Make sure you have .NET 8.0 SDK installed
2. Navigate to the `joshuas_bad_week` directory
3. Run the following commands:
   ```bash
   dotnet build
   dotnet run
   ```

## Project Structure

The game is organized into a clean, modular architecture:

```
joshuas_bad_week/
├── Config/
│   └── GameConfig.cs          # Centralized configuration settings
├── Entities/
│   └── Player.cs              # Player entity with movement and rendering
├── Managers/
│   ├── GameStateManager.cs    # Game state, timer, and win condition logic
│   └── UIManager.cs           # UI rendering (health, timer, win text)
├── Content/
│   ├── Content.mgcb           # MonoGame content pipeline configuration
│   └── DefaultFont.spritefont # Font definition for UI text
├── Game1.cs                   # Main game class
└── Program.cs                 # Entry point
```

## Configuration

All game settings are easily configurable in `Config/GameConfig.cs`:

### Player Settings
- `PlayerSize`: Size of the player square (default: 20 pixels)
- `PlayerSpeed`: Movement speed (default: 200 pixels/second)
- `PlayerColor`: Color of the player square (default: Yellow)

### Game Settings
- `GameDurationSeconds`: How long to survive (default: 120 seconds)
- `InitialHealth`: Starting health (default: 10)

### Screen Settings
- `ScreenWidth`: Game window width (default: 800 pixels)
- `ScreenHeight`: Game window height (default: 600 pixels)

### UI Settings
- `UITextColor`: Color of UI text (default: White)
- `HealthTextPosition`: Position of health display
- Font size and other UI properties can be modified in `DefaultFont.spritefont`

## Technical Features

### Player Movement
- **8-directional movement**: Supports all arrow key combinations
- **Normalized diagonal movement**: Diagonal movement speed is normalized to prevent faster movement
- **Rotation**: Player square rotates to face movement direction using `Math.Atan2`
- **Boundary collision**: Player cannot move outside screen boundaries

### Game State Management
- **Timer system**: Precise countdown from 120 seconds
- **Win condition**: Game pauses and displays win message when timer reaches 0
- **State tracking**: Manages Playing, Won, and Paused states

### Rendering System
- **Procedural textures**: Player square is rendered using a 1x1 white pixel texture with color tinting
- **UI rendering**: Health and timer text with proper positioning
- **Centered win message**: Win text is automatically centered on screen

## Architecture Benefits

1. **Modular Design**: Each class has a single responsibility
2. **Easy Configuration**: All settings centralized in `GameConfig.cs`
3. **Extensible**: Easy to add new features like enemies, power-ups, etc.
4. **Clean Separation**: Game logic, rendering, and configuration are separated
5. **MonoGame Best Practices**: Follows standard MonoGame patterns and conventions

## Future Enhancements

The current architecture makes it easy to add:
- Enemies that reduce health when touched
- Power-ups and collectibles
- Different difficulty levels
- Sound effects and music
- Particle effects
- Multiple levels or game modes
- High score system
