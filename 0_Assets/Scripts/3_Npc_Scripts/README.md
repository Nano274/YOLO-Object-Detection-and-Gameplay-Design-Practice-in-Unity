# NPC Generation and Movement Module

## Overview
This module manages the dynamic generation of NPCs and their random wandering behavior in the game. It includes two main scripts: **RandomMovement** for NPC wandering and **AnimalFarm** for NPC spawning.

---

## Scripts

### 1. RandomMovement Script

#### Description
The `RandomMovement` script allows NPCs to wander randomly within a specified radius. It includes idle states and animations, making the movement more realistic.

#### Features
- NPCs periodically sit idle (`waitTime`) before moving to a new random position.
- Target positions are generated within a circular area (`wanderRadius`).
- Animations switch between sitting (`isSit`) and walking (`isWalk`) states during movement.

#### Parameters
- **`wanderRadius`**: Defines the radius within which the NPC can move.
- **`waitTime`**: Specifies how long the NPC remains idle before moving.
- **Animations**: Controlled by the Animator component with `isWalk` and `isSit` boolean parameters.

#### Usage
1. Attach the `RandomMovement` script to an NPC prefab.
2. Ensure the NPC has:
   - A `NavMeshAgent` component for navigation.
   - An `Animator` component with `isWalk` and `isSit` parameters.
3. Bake the NavMesh in Unity to enable NPC movement.

---

### 2. AnimalFarm Script

#### Description
The `AnimalFarm` script is responsible for dynamically spawning NPCs at predefined spawn points within the game.

#### Features
- NPCs are spawned at random locations near the defined spawn points.
- Raycasting ensures NPCs are placed on valid terrain (`groundLayer`).
- Constraints can be dynamically added or removed from NPCs' Rigidbody components.

#### Parameters
- **`animalPrefabList`**: A list of NPC prefabs available for spawning.
- **`spawnPoints`**: An array of predefined spawn points (`Transform` objects).
- **`spawnCount`**: Number of NPCs to spawn at each spawn point.
- **`spawnRadius`**: Radius around each spawn point within which NPCs can appear.
- **`raycastHeight`**: Height from which the raycast begins to find valid terrain.
- **`groundLayer`**: Layer used for terrain validation.

#### Usage
1. Attach the `AnimalFarm` script to an empty GameObject in the scene.
2. Assign:
   - `animalPrefabList` with NPC prefabs.
   - `spawnPoints` with spawn locations.
   - `groundLayer` for terrain detection.
3. Customize `spawnCount`, `spawnRadius`, and `raycastHeight` as needed.

---

## Example Setup
1. Create a NavMesh for the scene:
   - Bake the NavMesh using Unity's Navigation system.
2. Prepare NPC prefabs:
   - Add `NavMeshAgent` and `Animator` components.
   - Assign appropriate animations for walking and sitting states.
3. Configure the scripts:
   - Attach `RandomMovement` to NPC prefabs.
   - Attach `AnimalFarm` to a GameObject and configure parameters in the Inspector.

---

## Notes
- Ensure `NavMesh` is properly baked; otherwise, NPCs cannot move.
- All NPC prefabs should have compatible Rigidbody and Animator components.
- Customize the scripts to add additional behaviors or interactions as needed.
