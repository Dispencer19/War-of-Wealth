# Unified Multiplayer System - War of Wealth

## Overview
The project now uses a unified multiplayer system that properly supports split-screen gameplay with 1-4 players. All fragmented camera and player management systems have been consolidated into centralized managers.

## New Architecture

### Core Managers

#### 1. PlayerManager (Singleton)
**Location**: `Assets/Scripts/PlayerManager.cs`
**Purpose**: Central hub for all player references and operations

**Features**:
- Automatically finds players by "Player" tag
- Provides unified access to player components (Movement, Health, Bank)
- Handles player teleportation and movement control
- Supports dynamic player count detection

**Key Methods**:
```csharp
PlayerManager.Instance.GetPlayer(int index)
PlayerManager.Instance.TeleportPlayer(int index, Vector3 pos, Quaternion rot)
PlayerManager.Instance.EnablePlayerMovement(int index)
PlayerManager.Instance.ResetPlayerHealth(int index)
```

#### 2. CameraManager (Singleton)
**Location**: `Assets/Scripts/CameraManager.cs`
**Purpose**: Unified camera system for both board and FPS modes

**Features**:
- Automatic split-screen viewport setup (1-4 players)
- Seamless mode switching between Board and FPS
- Per-player camera configuration
- Supports both board overview and individual FPS cameras

**Supported Layouts**:
- **1 Player**: Full screen
- **2 Players**: Side-by-side (50/50)
- **3 Players**: Top row (2 players) + bottom (1 full-width)
- **4 Players**: Quad split (2x2 grid)

#### 3. Updated PlayerCam
**Location**: `Assets/Scripts/PlayerCam.cs`
**Updates**:
- Added `playerIndex` field for multi-player support
- Only player 1 locks cursor (avoids conflicts)
- Automatic orientation finding

### Refactored Systems

#### FightTeleport.cs
**Changes**:
- Now uses PlayerManager instead of hard-coded GameObject.Find()
- Configurable player indices instead of fixed "Player1"/"Player2"
- Added `EndFight()` method to restore player control
- Supports any two players fighting

#### ShootingRangeManager_SpencerHP.cs
**Changes**:
- Uses PlayerManager for player operations
- Configurable player index instead of hard-coded player1
- Added `ExitShootingRange()` method to return to board mode
- Proper cleanup of coroutines and UI

#### BoardTurns.cs
**Changes**:
- Removed manual player array management
- Uses PlayerManager for all player operations
- Cleaner initialization and error handling
- Maintains all existing board game logic

#### GameMode.cs
**Changes**:
- Integrated with new CameraManager
- Automatic camera setup based on player count
- Cleaner mode switching logic

## Setup Instructions

### 1. Scene Setup
1. **Add Managers to Scene**:
   - Add `PlayerManager` component to a GameObject (it will become a singleton)
   - Add `CameraManager` component to a GameObject (it will become a singleton)

2. **Player Objects**:
   - Ensure all player GameObjects have the "Player" tag
   - Each player should have:
     - `PlayerMovement_SpencerHP` component
     - `PlayerHealth` component
     - `PlayerBankAccounts` component
   - PlayerMovement components will automatically get correct `playerIndex` values

3. **Camera Configuration**:
   - Main Camera should be tagged as "MainCamera"
   - CameraManager will create additional cameras as needed
   - For FPS mode, cameras attach to player transforms

### 2. Component References
Update any scripts that reference the old systems:

**Old Way**:
```csharp
GameObject player1 = GameObject.Find("Player1");
player1.GetComponent<PlayerMovement_SpencerHP>().canMove = false;
```

**New Way**:
```csharp
PlayerManager.Instance.DisablePlayerMovement(0); // Player index 0
```

### 3. Fight System Setup
1. Configure `FightTeleport` component:
   - Set `player1Index` and `player2Index` (0-based)
   - Assign spawn point transforms
2. Call `StartFight()` to begin, `EndFight()` to restore control

### 4. Shooting Range Setup
1. Configure `ShootingRangeManager_SpencerHP`:
   - Set `playerIndex` for which player enters
   - Assign spawn point and UI references
2. Call `EnterShootingRange()` and `ExitShootingRange()`

## Input System

### Player Movement (PlayerMovement_SpencerHP.cs)
- **Player 1**: WASD + Space (jump)
- **Player 2**: Arrow Keys + Enter (jump)
- **Additional players**: Currently only 2 players supported due to input limitations

### Camera Control (PlayerCam.cs)
- All players use mouse for FPS camera look
- Only Player 1 locks cursor to avoid conflicts

## Backwards Compatibility

### Obsolete Scripts
- `SpawnPlayers.cs`: Functionality moved to PlayerManager
- `SplitScreenCameraManager.cs`: Replaced by CameraManager
- `MainCam.cs` & `PlayerCamera.cs`: Consolidated into CameraManager

### Preserved Functionality
- All board game mechanics work identically
- UI systems unchanged
- Dice rolling and property management unchanged
- Save/load functionality preserved

## Future Extensions

### Adding More Players
1. Extend input system in `PlayerMovement_SpencerHP.cs`
2. Add more key bindings for additional players
3. Update CameraManager viewport calculations if needed

### Network Support
The unified system provides a foundation for adding Photon networking:
1. Add PhotonView components to players
2. Implement OnPhotonSerializeView in player components
3. Use PlayerManager as central state authority

### Enhanced Features
- Per-player camera settings (FOV, sensitivity)
- Player customization UI
- Spectator camera modes
- Advanced split-screen layouts

## Troubleshooting

### Common Issues

**"PlayerManager not found"**
- Ensure PlayerManager component exists in scene
- Check that it initializes before other scripts

**Split-screen not working**
- Verify players have "Player" tag
- Check CameraManager is in scene and configured
- Ensure GameMode calls camera setup

**Player movement not working**
- Check playerIndex is set correctly on PlayerMovement components
- Verify PlayerManager found all players

**Camera issues**
- Ensure main camera is tagged "MainCamera"
- Check that CameraManager creates additional cameras
- Verify viewport rects are set correctly

### Debug Information
All managers log initialization status. Check console for:
- "PlayerManager initialized with X players"
- "BoardTurns initialized for X players"
- Camera setup confirmations

## Migration Guide

### From Old System
1. Remove references to old camera scripts
2. Replace GameObject.Find() calls with PlayerManager methods
3. Update fight/range systems to use new APIs
4. Test with 1 player first, then add more

### Testing Checklist
- [ ] Single player board mode
- [ ] Single player FPS mode
- [ ] Two player split-screen board
- [ ] Two player split-screen FPS
- [ ] Fight system with correct players
- [ ] Shooting range entry/exit
- [ ] Turn-based gameplay with multiple players
- [ ] UI updates for correct player

The unified system provides a solid foundation for local multiplayer with clear paths for future enhancements!