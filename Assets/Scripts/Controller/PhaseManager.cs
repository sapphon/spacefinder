using System;
using System.Collections;
using System.Collections.Generic;
using Controller.PhaseControllers;
using Model;
using UnityEngine;

public class PhaseManager : MonoBehaviour
{
    protected Phase currentPhase = Phase.Engineering;
    protected List<Ship> shipsSignalingComplete = new List<Ship>();
    private HelmPhaseController _helmPhaseController;
    private GunneryPhaseController _gunneryPhaseController;
    private Queue<Ship> _shipsYetToActInOrder = new Queue<Ship>();
    private ShipUIManager _shipsUI;
    private Dictionary<Ship, CrewAction> actionsThisPhase;


    void Awake()
    {
        _helmPhaseController = FindObjectOfType<HelmPhaseController>();
        _gunneryPhaseController = FindObjectOfType<GunneryPhaseController>();
        _shipsUI = FindObjectOfType<ShipUIManager>();
        actionsThisPhase = new Dictionary<Ship, CrewAction>();
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
    
    public List<string> GetPossibleActionNamesForPhase()
    {
        switch (currentPhase)
        {
            case Phase.Helm:
                return new List<string>() {"Maneuver"};
            case Phase.Gunnery:
                return new List<string>() {"Shoot"};
            default:
                return new List<string>(){"Hold It Together"};
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
        else if (this.currentPhase == Phase.Gunnery)
        {
            _gunneryPhaseController.OnPhaseBegin();
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
        if (!DoesCurrentPhaseUseInitiative())
        {
            EndActionIfInProgress(ship);
            shipsSignalingComplete.Add(ship);
        }
        else if(ShipHasInitiative(ship))
        {
            EndActionIfInProgress(ship);
            _shipsYetToActInOrder.Dequeue();
            shipsSignalingComplete.Add(ship);
            selectNextShipOrNone();
        }
    }

    private void selectNextShipOrNone()
    {
        if (_shipsYetToActInOrder.Count > 0)
        {
            _shipsUI.TrySelectShip(_shipsYetToActInOrder.Peek().gridPosition);
        }
        else
        {
            _shipsUI.DeselectShip();
        }

    }

    private void EndActionIfInProgress(Ship ship)
    {
        if (this.HasShipChosenActionThisPhase(ship)){
            this.EndActionInProgressForShip(ship);
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
    
    public void SetShipInitiativeOrder(Queue<Ship> shipsByInitiativeAscending)
    {
        this._shipsYetToActInOrder = shipsByInitiativeAscending;
        selectNextShipOrNone();
    }

    public Ship GetShipWithInitiative()
    {
        return this._shipsYetToActInOrder.Count < 1 ? null : this._shipsYetToActInOrder.Peek();
    }

    public bool ShipHasInitiative(Ship ship)
    {
        return GetShipWithInitiative() == ship;
    }

    public bool DoesCurrentPhaseUseInitiative()
    {
        return DoesPhaseUseInitiative(currentPhase);
    }

    public bool DoesPhaseUseInitiative(Phase phase)
    {
        return phase == Phase.Helm || currentPhase == Phase.Gunnery;
    }
    
    public bool HasShipChosenActionThisPhase(Ship ship)
    {
        return actionsThisPhase.ContainsKey(ship);
    }

    public void ToggleShipAction(Ship actor, string actionName)
    {
        if (HasShipChosenActionThisPhase(actor) && actionsThisPhase[actor].name == actionName)
        {
            EndActionInProgressForShip(actor);
        }
        else
        {
            actionsThisPhase.Add(actor, new CrewAction(actionName));
            if (this.currentPhase == Phase.Helm)
            {
                _helmPhaseController.OnActionBegin(this.actionsThisPhase[actor], actor);
            }
            else if (currentPhase == Phase.Gunnery)
            {
                _gunneryPhaseController.OnActionBegin(this.actionsThisPhase[actor], actor);
            }
        }
    }

    public void EndActionInProgressForShip(Ship actor)
    {
        if (this.currentPhase == Phase.Helm)
        {
            _helmPhaseController.OnActionEnd(this.actionsThisPhase[actor], actor);
        }
        else if (currentPhase == Phase.Gunnery)
        {
            _gunneryPhaseController.OnActionEnd(this.actionsThisPhase[actor], actor);
        }
        actionsThisPhase.Remove(actor);
    }

    public CrewAction getShipAction(Ship ship)
    {
        return this.actionsThisPhase.ContainsKey(ship) ? this.actionsThisPhase[ship] : null;
    }
    public void ResetAction(Ship ship)
    {
        if (this.currentPhase == Phase.Helm)
        {
            _helmPhaseController.OnActionCancel(this.actionsThisPhase[ship], ship);
        }
        else if (currentPhase == Phase.Gunnery)
        {
            _gunneryPhaseController.OnActionCancel(this.actionsThisPhase[ship], ship);
        }

        actionsThisPhase.Remove(ship);
    }
}

public enum Phase
{
    Engineering = 0,
    Helm = 1,
    Gunnery = 2
}

public class CrewAction
{
    public string name { get; }
    public Phase phase { get; }

    public CrewAction(string name, Phase phase = Phase.Helm)
    {
        this.name = name;
        this.phase = phase;
    }
}