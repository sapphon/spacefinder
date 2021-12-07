using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseManager : MonoBehaviour
{
    protected Phase currentPhase = Phase.Helm;
    protected List<Ship> shipsSignalingComplete = new List<Ship>();

    public Phase GetCurrentPhase()
    {
        return this.currentPhase;
    }

    public bool TryAdvancePhase()
    {
        bool mayAdvance = CanPhaseEnd();
        if (mayAdvance)
        {
            EndPhase();
        }

        return mayAdvance;
    }

    public bool CanPhaseEnd()
    {
        Ship[] allShips = Ship.getAllShips();
        return allShips.Length == shipsSignalingComplete.Count && new HashSet<Ship>(allShips).SetEquals(allShips);
    }

    protected void EndPhase()
    {
        if (FindObjectOfType<RoundManager>().TryAdvanceRound(this))
        {
            this.currentPhase = 0;
        }
        else
        {
            this.currentPhase++;
        }

        this.shipsSignalingComplete.Clear();
    }

    public void SignalComplete(Ship ship)
    {
        shipsSignalingComplete.Add(ship);
    }

    public void SignalStillWorking(Ship ship)
    {
        shipsSignalingComplete.Remove(ship);
    }

    public bool isShipDone(Ship ship)
    {
        return shipsSignalingComplete.Contains(ship);
    }

    public bool IsLastPhase()
    {
        return Convert.ToInt32(this.currentPhase).Equals(Enum.GetValues(typeof(Phase)).Length - 1);
    }
}

public enum Phase
{
    Engineering = 0,
    Helm = 1,
    Gunnery = 2
}