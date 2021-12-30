using System;
using System.Collections;
using System.Collections.Generic;
using Model;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.InputSystem;

public class HelmPhaseController : MonoBehaviour
{
    private Dictionary<Ship, CrewAction> actionsThisPhase;
    private ShipUIManager _shipUiManager;
    private List<Vector3Int> _turnsSoFar;
    private List<Vector3Int> _destinationsSoFar;
    private Vector3Int _initialPosition;
    private Facing _initialFacing;
    private InitiativeController _initiativeController;

    void Awake()
    {
        _initiativeController = FindObjectOfType<InitiativeController>();
        _shipUiManager = FindObjectOfType<ShipUIManager>();
        actionsThisPhase = new Dictionary<Ship, CrewAction>();
        _turnsSoFar = new List<Vector3Int>();
        _destinationsSoFar = new List<Vector3Int>();
    }


    public void OnPhaseBegin()
    {
        this._initiativeController.GatherInitiatives();
    }

    public void SetInitiatives()
    {
        //guess we'll see here
    }

    public bool IsShipCurrentlyActing(Ship ship)
    {
        return actionsThisPhase.ContainsKey(ship);
    }

    public List<string> GetPossibleActionNamesForPhase()
    {
        return new List<string>() {"Maneuver"};
    }

    public void ToggleShipAction(Ship actor, string actionName)
    {
        if (IsShipCurrentlyActing(actor) && actionsThisPhase[actor].name == actionName)
        {
            EndActionInProgressForShip(actor);
        }
        else
        {
            this._initialPosition = actor.gridPosition;
            this._initialFacing = actor.facing;
            _destinationsSoFar.Clear();
            _turnsSoFar.Clear();
            actionsThisPhase.Add(actor, new CrewAction(actionName));
        }
    }

    public void EndActionInProgressForShip(Ship actor)
    {
        actionsThisPhase.Remove(actor);
    }

    public CrewAction getShipAction(Ship ship)
    {
        return this.actionsThisPhase.ContainsKey(ship) ? this.actionsThisPhase[ship] : null;
    }

    public bool TryStarboardTurn(Ship ship)
    {
        if (MayTurn(ship))
        {
            this._turnsSoFar.Add(ship.gridPosition);
            ship.TurnToStarboard();
            return true;
        }

        return false;
    }

    public bool TryPortTurn(Ship ship)
    {
        if (MayTurn(ship))
        {
            this._turnsSoFar.Add(ship.gridPosition);
            ship.TurnToPort();
            return true;
        }

        return false;
    }

    public bool TryAdvance(Ship ship)
    {
        if (MayAdvance(ship))
        {
            ship.Advance();
            this._destinationsSoFar.Add(ship.gridPosition);
            return true;
        }

        return false;
    }

    public void ResetAction(Ship ship)
    {
        actionsThisPhase.Remove(ship);
        _turnsSoFar.Clear();
        _destinationsSoFar.Clear();
        ship.gridPosition = _initialPosition;
        ship.facing = _initialFacing;
    }

    public bool MayAdvance(Ship ship)
    {
        return MovesRemaining(ship) > 0;
    }

    public int MovesRemaining(Ship ship)
    {
        return Math.Max(0, ship.speed - this._destinationsSoFar.Count);
    }

    public int MovesUntilNextTurn(Ship ship)
    {
        Maneuverability maneuverability = ship.maneuverability;
        if (maneuverability == Maneuverability.Perfect)
        {
            return _turnsSoFar.Count < 2 ||
                   _turnsSoFar[_turnsSoFar.Count - 2] != ship.gridPosition ||
                   _turnsSoFar[_turnsSoFar.Count - 1] != ship.gridPosition ? 0 : 1;
        }
        else
        {
            return Math.Max(0, (_turnsSoFar.Count + 1) * (int) maneuverability - _destinationsSoFar.Count);
        }
    }

    public bool MayTurn(Ship ship)
    {
        return MovesUntilNextTurn(ship) == 0;
    }
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