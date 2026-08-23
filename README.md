# Kaiju Paradise Game

Unity multiplayer game prototype built around friend-group sessions and multiple modes.

## Planned modes
- Story Mode
- PvE
- Extraction
- Survival
- Infection
- Team Battle
- Free Roam / Hangout
- NULL-style meta boss fight

## Main map
**Catacombs** is the primary map and is intended to support **all game modes**. Mode systems reuse the same map while changing objectives, AI behavior, encounters, and rules.

The supplied **Cave** map is an additional environment for future modes, alternate sections, events, or testing.

## Extraction
Extraction takes place in the Catacombs. Players receive a health and speed boost and must find the escape door while extremely fast hunter AI actively chases and attacks them. **There is no time limit**: the danger comes from the AI pressure and the need to find and reach the door before the squad gets overwhelmed.

## NULL boss
The NULL-style boss fight uses the supplied monster player/boss model. Players receive a major temporary speed boost while the boss is significantly faster, turning the encounter into a chaotic chase with room for future meta/glitch mechanics, reality shifts, and scripted arena events.

## Tech
- Unity 2022.3 LTS baseline
- Photon networking (PUN 2 adapter included; networking code is isolated so the backend can be swapped later)
- Modular game-mode architecture
- Gorilla-style locomotion integration planned as the VR movement foundation

## Important
The repository intentionally does **not** include copyrighted game assets or copied game code. The supplied Catacombs map, Cave map, and player/boss model can be integrated as project assets.

## Photon setup
1. Create a Photon App ID for the Unity/PUN 2 application.
2. Import Photon PUN 2 into the Unity project.
3. Open `Assets/Resources/PhotonServerSettings.asset` after import and configure the App ID.
4. Start from `MainMenu` and create/join a friend room.

The starter networking layer is disabled automatically when Photon PUN 2 is not installed, so the project remains editable before the SDK is imported.
