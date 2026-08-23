# Multiplayer Stress Report

## Environment
- Unity baseline: 2022.3.62f2
- Networking target: Photon PUN 2
- Real Photon App ID: not configured yet
- Real multi-instance Photon run: not available in this environment

## Static stress audit

| Area | Result | Notes |
|---|---|---|
| Room create/join | PASS (code audit) | Private friend-room flow and max-player configuration are present. |
| Player root replication | PASS (code audit) | PhotonPlayer serializes position + rotation. |
| VR head replication | PASS (code audit) | PhotonVRRigSync now serializes head target. |
| VR hand replication | PASS (code audit) | PhotonVRRigSync now serializes both hand targets. |
| Physics item ownership | PASS (code audit) | Grabs refuse local simulation until requested ownership is confirmed. |
| Physics item replication | PASS (code audit) | Position, rotation, velocity, and angular velocity are replicated. |
| Mode switching | PASS (code audit) | Shared mode architecture; player modifiers restore base movement values. |
| Extraction no-timer rule | PASS | Failure is driven by squad defeat/overwhelm, not elapsed time. |
| AI collision/pathing | PASS (code audit) | Hunter uses navigation when available with a movement fallback. |
| Local physics stress | AVAILABLE | Use MultiplayerStressHarness: 8 simulated peers + 120 physics items by default. |

## Real-network tests still required after Photon ID is configured

1. 2 players: connect, create/join, move, disconnect/reconnect.
2. 4 players: simultaneous movement and mode switching.
3. 8 players: continuous movement + VR head/hand updates for 10+ minutes.
4. 8 players: two players contest the same physics item repeatedly.
5. 8 players: throw weapons/crowbars/medical items through doors and around corners.
6. 8 players: Extraction with hunter AI, healing, downing, revives, and escape.
7. 8 players: NULL chase with all players receiving speed modifiers.
8. Owner disconnect: verify dropped physics items recover cleanly instead of freezing.
9. Late join: verify new players receive current mode, item transforms, and open/broken door state.
10. Network interruption: verify the room and player state recover without duplicated items.

## Known limitation
This report is a code/static stress audit plus a local physics harness. It is **not** a claim that a live Photon 8-player session has already been executed.
