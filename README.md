# Kaiju Paradise Game

Unity multiplayer VR game prototype built around friend-group sessions and multiple modes.

## Modes
- Story Mode
- PvE
- Extraction
- Survival
- Infection
- Team Battle
- Free Roam / Hangout
- NULL-style meta boss fight

## Main map
**Catacombs** is the primary map for every mode. Mode systems reuse the same map while changing objectives, AI behavior, encounters, and rules.

The supplied **Cave** map is an additional environment for future modes, alternate sections, events, or testing.

## Extraction
Extraction takes place in the Catacombs. Players get a temporary health and speed boost and must find the escape door while extremely fast hunter AI chases and attacks them. **There is no time limit**; the pressure comes from relentless AI and eventually getting overwhelmed.

## Physical gameplay
Weapons, medkits, bandages, keycards, crowbars, and other objects are physics-based. Objects remain dynamic while held, so players can use them as physical supports to push against the floor/walls and move around. Weapons can damage through impact, medical items heal during an in-world use action, keycards unlock specific access levels, and crowbars can physically break security doors.

Security doors support Blue, Yellow, Red, and Black access levels. Doors can also be brute-forced with enough impact, including intentional high-speed/jump-through collisions.

## NULL boss
The NULL-style boss fight uses the supplied monster player/boss model. Players receive a major temporary speed boost while the boss is significantly faster, turning the encounter into a chaotic chase with room for future meta/glitch mechanics, reality shifts, and scripted arena events.

## VR player
The project uses the original **Another-Axiom GorillaLocomotion** `Player` implementation as the movement foundation instead of a custom locomotion replacement. The rig setup adds tracked head/hands, Rigidbody/colliders, physical VR hands, keycard inventory, and mode-specific player modifiers.

Use `Kaiju Game/Install Gorilla Locomotion` in the Unity Editor to import the upstream MIT-licensed package. After your rigged player model is imported/unpacked, select its root and run `Kaiju Game/Configure Selected Model As VR Player`.

## Multiplayer
Photon PUN 2 is the multiplayer backend for friend-group rooms. Physical items include an optional Photon sync/ownership component so dropped and held objects can replicate between players.

## Tech
- Unity 2022.3 LTS baseline
- Photon PUN 2
- GorillaLocomotion VR movement
- Modular game-mode architecture
- Physics-driven interactions and combat
