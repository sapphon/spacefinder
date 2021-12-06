using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    protected int currentRound = 1;
    protected List<Ship> shipsSignalingComplete = new List<Ship>();

    protected bool CanRoundEnd()
    {
        Ship[] allShips = Ship.getAllShips();
        return allShips.Length == shipsSignalingComplete.Count && new HashSet<Ship>(allShips).SetEquals(allShips);
    }

    public int GetCurrentRound()
    {
        return currentRound;
    }

    public bool TryAdvanceRound()
    {
        bool mayAdvance = CanRoundEnd();
        if (mayAdvance)
        {
            EndRound();
        }

        return mayAdvance;
    }

    protected void EndRound()
    {
        currentRound++;
    }
}
