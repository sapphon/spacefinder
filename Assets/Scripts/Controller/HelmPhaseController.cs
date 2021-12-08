using System.Collections;
using System.Collections.Generic;
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

    void Awake()
    {
        _shipUiManager = FindObjectOfType<ShipUIManager>();
        actionsThisPhase = new Dictionary<Ship, CrewAction>();
        _turnsSoFar = new List<Vector3Int>();
        _destinationsSoFar = new List<Vector3Int>();
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
            actionsThisPhase.Remove(actor);
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

    public bool MayAdvance(Ship ship)
    {
        return this._destinationsSoFar.Count < ship.speed;
    }


    public bool MayTurn(Ship shipToTrack)
    {
        return this._turnsSoFar.Count == 0 || !this._turnsSoFar[this._turnsSoFar.Count - 1].Equals(shipToTrack.gridPosition);
    }
}

public class CrewAction
{
    public string name {  get; }
    public Phase phase {  get; }

    public CrewAction(string name, Phase phase = Phase.Helm)
    {
        this.name = name;
        this.phase = phase;
    }

}
