using System;
using System.Collections;
using System.Collections.Generic;
using Model;
using UnityEngine;

public class PhaseManager : MonoBehaviour
{
    protected Phase currentPhase = Phase.Engineering;
    protected List<Ship> shipsSignalingComplete = new List<Ship>();
    private HelmPhaseController _helmPhaseController;

    void Awake()
    {
        _helmPhaseController = FindObjectOfType<HelmPhaseController>();
    }

    public Phase GetCurrentPhase()
    {
        return this.currentPhase;
    }

    public List<Crew.Role> GetActivePhaseRoles()
    {
        switch (this.currentPhase)
        {
            case Phase.Engineering:
                return new List<Crew.Role>(){Crew.Role.Engineer, Crew.Role.Captain};
            case Phase.Helm:
                return new List<Crew.Role>() {Crew.Role.Pilot, Crew.Role.Scientist, Crew.Role.Captain};
            case Phase.Gunnery:
                return new List<Crew.Role>() {Crew.Role.Gunner, Crew.Role.Captain};
            default:
                return new List<Crew.Role>(); 
        }
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
        DoEndPhase();
        if (this.currentPhase == Phase.Helm)
        {
            _helmPhaseController.OnPhaseBegin();
        }

        this.shipsSignalingComplete.Clear();
    }

    private void DoEndPhase()
    {
        if (FindObjectOfType<RoundManager>().TryAdvanceRound(this))
        {
            this.currentPhase = 0;
        }
        else
        {
            this.currentPhase++;
        }
    }

    public void SignalComplete(Ship ship)
    {
        EndActionIfInProgress(ship);
        shipsSignalingComplete.Add(ship);
    }

    private void EndActionIfInProgress(Ship ship)
    {
        if(this.currentPhase == Phase.Helm && _helmPhaseController.IsShipCurrentlyActing(ship)){
            _helmPhaseController.EndActionInProgressForShip(ship);
        }
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