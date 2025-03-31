using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameSaveData
{
    public int currentDay;
    public GameStateManager.GamePhase currentPhase;
    public Dictionary<GameStateManager.ResourceType, int> resources;
    public Dictionary<string, int> relationships;
    public Dictionary<string, bool> flags;
}