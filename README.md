# Match3-Unity-Intern-Test2025
Test project for Winter Wolf - IEC Games
(Keeping track of the tasks)

## Task 1: Item Re-skin (Done)
This was to change the item graphics from food to fish.

The re-skin was done by updating the `Sprite` in the **Sprite Renderer** component on all prefabs in `Assets/Resources/Prefabs`.

The new fish sprites from `Assets/Textures/Fish` were dragged onto the `Sprite` field for each `itemNormal...` prefab.

## Task 2: Change Gameplay (Done)
This was the big task: changing the game from a "swap-to-match-3" to a "tap-to-collect" style.

### New Gameplay Rules:
* Tapping an item on the board moves it to a bottom bar.
* The bottom bar has 5 cells (this is set in `GameSettings.cs` and built by `BottomArea.cs`).
* Items can't be moved back to the board.
* If exactly 3 identical items are in the bottom bar, they are cleared.
* **Win Condition:** Clear all items from the board. A `UIPanelWin` screen is shown.
* **Lose Condition:** The 5-cell bottom bar fills up completely. The `UIPanelGameOver` screen is shown.

### Other Requirements (Done):
* **Divisible by 3:** The `Board.cs` `Fill()` method was updated to make sure the starting board has a total count of each item type that is a multiple of 3.
* **Home Screen:** `UIPanelMain` was updated. The old "Moves/Timer" buttons were removed.
* **Autoplay Win:** Added an "Autoplay Win" button to the Home screen. This runs an AI (`AutoplayWinCoroutine`) that tries to find 2 matching items in the bar and taps the 3rd one on the board.
* **Auto Lose:** Added an "Auto Lose" button to the Home screen. This runs an AI (`AutoplayLoseCoroutine`) that tries to tap items *not* currently in the bar, to fill it up without matching.
* **Delay:** All autoplay actions use a 0.5s delay, loaded from `GameSettings.cs`.

### Code Cleanup (Done):
* Comment/Removed old logic for `LevelMoves.cs`, `LevelTime.cs`, and `LevelCondition.cs`.
* Comment/Removed old `swap`, `GetHorizontalMatches`, `GetPotentialMatches`, etc. functions from `Board.cs`.
* Cleaned up `GameManager.cs` to comment/remove all old level logic.