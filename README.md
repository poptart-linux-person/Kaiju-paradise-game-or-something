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

## Tech
- Unity 2022.3 LTS baseline
- Photon networking (PUN 2 adapter included; networking code is isolated so the backend can be swapped later)
- Modular game-mode architecture

## Important
The repository intentionally does **not** include copyrighted game assets or copied game code. The supplied map and player model can be integrated later.

## Photon setup
1. Create a Photon App ID for the Unity/PUN 2 application.
2. Import Photon PUN 2 into the Unity project.
3. Open `Assets/Resources/PhotonServerSettings.asset` after import and configure the App ID, or use the `PhotonLauncher` inspector field if the package exposes it.
4. Start from `MainMenu` and create/join a friend room.

The starter networking layer is disabled automatically when Photon PUN 2 is not installed, so the project remains editable before the SDK is imported.
