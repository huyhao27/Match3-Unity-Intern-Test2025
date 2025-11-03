using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : ScriptableObject
{
    public int BoardSizeX = 5;

    public int BoardSizeY = 5;

    public int MatchesMin = 3;
    
    public int BottomCellCount = 5;
    public float AutoplayDelay = 0.5f;

    public float TimeForHint = 5f;
}
