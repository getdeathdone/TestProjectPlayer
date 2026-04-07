# TestProjectPlayer

## Implementation Status (By Task List)

### 1) 3D Scene
- Scene `Assets/Scenes/Game.unity` is present with a player.
- Basic environment objects are present:
  - Ground (`Plane`) is created by `EnvironmentBootstrapper`.
  - Obstacles (`Cube`) are created in code.
  - Additional decorations are spawned from Addressables (`Decoration` label).
- Primitives and free assets are used.

### 2) Player Movement
- Movement uses `CharacterController` (`PlayerMovement`).
- Movement input is read from a virtual joystick via `InputService`.
- Virtual joystick control is implemented.

### 3) Camera
- A custom follow camera is implemented (`CameraController`) with rotation, smoothing, and aiming mode.
- Cinemachine is not used (custom logic is used instead).

### 4) Weapon Interaction
- One weapon is spawned in the scene via Addressables (`WeaponProvider`, `Weapons` label).
- Player can approach, pick up, and shoot (`WeaponService` + `Weapon`).
- Shot impact is detected (physics and bullet collision flow).
- Console logging on hit is implemented in `BulletProjectile`:
  - `GameObject: <name>, HitPoint: <x, y, z>`

### 5) UI
- `Pick Up Weapon` button:
  - Visible only when player is near the weapon.
  - On click, triggers pickup signal.
- `Shoot` button:
  - Visible only when weapon is equipped.
  - Triggers shooting logic.
- Additional hold-to-aim button is present.
- UI control layer is implemented for pickup/aim/fire interactions.

### 6) Animations
- Character animations are integrated (movement/combat state flow).
- Weapon-related animation integration is implemented.
- Animator setup is present in project assets (controller and animation support scripts).

## Addressables Usage
- Addressables are used for spawning:
  - Player (`Player`)
  - Weapon (`Weapons`)
  - Enemies (`Enemies`)
  - Decorations (`Decoration`)

## Compliance Note
- Covered: scene, `CharacterController` movement, follow camera, weapon interaction, hit logging, conditional button visibility.
