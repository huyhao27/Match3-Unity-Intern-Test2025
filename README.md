# Match3-Unity-Intern-Test2025
Test for Winter Wolf - IEC Games


## Task 1: Item Re-skin

This task involved updating the visual appearance of all in-game items. The original item sprites were replaced with new fish-themed sprites.

### Process Overview

The re-skin was accomplished by updating the source image used in the item prefabs.

1.  **Locate New Assets:** All new fish sprites are available in the project directory: `Assets/Textures/Fish`.

2.  **Identify Target Prefabs:** The game items that needed to be changed are prefabs located in `Assets/Resources/Prefabs`. This includes all `itemNormal...` prefabs.

3.  **Execute Re-skin:**
    * For each item prefab in the `Assets/Resources/Prefabs` folder, the **Sprite Renderer** component was modified.
    * The **Sprite** field in the `Sprite Renderer` component was updated by assigning the new, corresponding fish sprite from the `Assets/Textures/Fish` directory.
    * This change replaced the old item visual with the new fish visual while keeping all existing game logic and components (like colliders and scripts) intact.

### Result

All item prefabs in the `Resources` folder now use the fish sprites, and the game board correctly populates with the new "Fish" theme instead of the original "Food" theme.

