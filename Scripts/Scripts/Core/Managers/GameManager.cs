using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Docks docks;
    public GameStateManager gameStateManager;

    public void EndOfDay()
    {
        docks.ProcessShades();

        //gameStateManager.AdvancePhase();
    }
}
