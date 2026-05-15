# Photon Removal and Split-Screen Setup

## Changes Made

### Photon Removal
- ✅ Removed `Assets/Photon/` folder (entire Photon networking library)
- ✅ Deleted `Assets/Scripts/ConnectToServer.cs` (Photon connection script)
- ✅ Deleted `Assets/Scripts/CreateAndJoinRooms.cs` (Photon room management script)
- ✅ Updated `Assets/FightTeleport.cs` comment to remove Photon reference

### Split-Screen Camera System
- ✅ Created `Assets/Scripts/SplitScreenCameraManager.cs` - Manages multiple camera viewports
- ✅ Modified `Assets/Scripts/GameMode.cs` - Added split-screen camera integration
- ✅ Added public `IsFPSMode` property to GameMode for camera manager access

## How the Split-Screen System Works

### Camera Viewports
- **1 Player**: Full screen (no split)
- **2 Players**: Side-by-side (50/50 split)
- **3 Players**: Top row (2 players), bottom row (1 player full width)
- **4 Players**: Quad split (2x2 grid)

### Integration with Existing Systems
- Works with both FPS mode (`PlayerCam.cs`) and Board mode (`MainCam.cs`)
- Automatically detects players by "Player" tag
- Updates camera settings when switching game modes
- Maintains existing camera controls and UI toggling

## Setup Instructions

1. **Add SplitScreenCameraManager to Scene**:
   - Add the `SplitScreenCameraManager` component to a GameObject in your Game scene
   - Ensure it runs before other camera-related scripts

2. **Configure GameMode**:
   - In the `GameMode` component, assign the `SplitScreenCameraManager` reference
   - The system will automatically find and configure cameras

3. **Player Setup**:
   - Ensure all player GameObjects have the "Player" tag
   - Players are positioned manually in the scene (as before)
   - The `SpawnPlayers.cs` script assigns names automatically

4. **Testing**:
   - Add multiple player objects to test split-screen
   - Use the game mode switch button to test both FPS and Board modes
   - Cameras should automatically adjust viewports based on player count

## Key Features

- **Automatic Detection**: Finds players by tag, no manual configuration needed
- **Mode Integration**: Works seamlessly with existing FPS/Board mode switching
- **Flexible Layouts**: Supports 1-4 players with appropriate viewport arrangements
- **Performance**: Only creates additional cameras when needed
- **Backwards Compatible**: Single-player mode works exactly as before

## Troubleshooting

- **No split-screen**: Check that players have the "Player" tag
- **Camera issues**: Ensure `SplitScreenCameraManager` is in the scene and references are set
- **Mode switching problems**: Verify `GameMode` has the split-screen manager reference

The system maintains all your existing gameplay while adding robust split-screen support!