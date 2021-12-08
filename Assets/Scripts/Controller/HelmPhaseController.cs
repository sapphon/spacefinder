using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HelmPhaseController : MonoBehaviour
{
    private Dictionary<Ship, CrewAction> actionsThisPhase;
    private ShipUIManager _shipUiManager;

    void Awake()
    {
        _shipUiManager = FindObjectOfType<ShipUIManager>();
        actionsThisPhase = new Dictionary<Ship, CrewAction>();
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
            actionsThisPhase.Add(actor, new CrewAction(actionName));
        }
    }

    public CrewAction getShipAction(Ship ship)
    {
        return this.actionsThisPhase.ContainsKey(ship) ? this.actionsThisPhase[ship] : null;
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
