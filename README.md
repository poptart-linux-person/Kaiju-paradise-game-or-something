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

## VR player setup
The project is being wired around the original **Another-Axiom GorillaLocomotion** `Player` implementation rather than a custom replacement. The upstream component uses a Rigidbody, head/body colliders, tracked left/right hand transforms, and configurable locomotion layers.

Use `Kaiju Game/Install Gorilla Locomotion` in the Unity Editor to import the upstream MIT-licensed `.unitypackage`. After your rigged player model is imported/unpacked, select its root and run `Kaiju Game/Configure Selected Model As VR Player`. That creates the VR tracking targets, colliders, Rigidbody, and GorillaLocomotion bindings. The model archive currently stored in `Assets/destiny-chimps-player-model.zip` and the supplied monster rig archive are kept separate so the actual authored assets remain untouched.

## Tech
- Unity 2022.3 LTS baseline
- Photon networking (PUN 2 adapter included; networking code is isolated)
- Modular game-mode architecture
- GorillaLocomotion VR movement integration

## Important
The repository intentionally does **not** include copied third-party game code or copyrighted game assets. GorillaLocomotion is imported through its upstream MIT-licensed package, while the supplied map/model archives are project-owned inputs.

## Photon setup
1. Create a Photon App ID for the Unity/PUN 2 application.
2. Import/configure Photon PUN 2.
3. Open `Assets/Resources/PhotonServerSettings.asset` after import and configure the App ID.
4. Start from `MainMenu` and create/join a friend room.
