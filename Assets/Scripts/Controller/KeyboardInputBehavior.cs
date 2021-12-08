using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class KeyboardInputBehavior : MonoBehaviour
{
    private PlayerInput _input;
    private ShipUIManager _shipsUI;
    private HelmPhaseController _helmPhaseController;


    void Awake()
    {
        _helmPhaseController = FindObjectOfType<HelmPhaseController>();
        _input = GetComponent<PlayerInput>();
        _shipsUI = FindObjectOfType<ShipUIManager>();
    }

    private void OnEnable()
    {
        getAdvanceAction().performed += TryAdvance;
        getPortTurnAction().performed += TryPortTurn;
        getStarboardTurnAction().performed += TryStarboardTurn;
    }

    private bool ShouldMovementControlsEnable()
    {
        Ship selectedShip = _shipsUI.GetSelectedShip();
        if (selectedShip != null)
        {
            CrewAction currentAction = _helmPhaseController.getShipAction(selectedShip);
            if (currentAction != null && currentAction.name == "Maneuver")
            {
                return true;
            }
        }
        return false;
    }

    private void TryStarboardTurn(InputAction.CallbackContext obj)
    {
        if (ShouldMovementControlsEnable())
        {
            _shipsUI.GetSelectedShip().TurnToStarboard();
        }
    }

    private void TryPortTurn(InputAction.CallbackContext obj)
    {
        if (ShouldMovementControlsEnable())
        {
            _shipsUI.GetSelectedShip().TurnToPort();
        }
    }

    private void TryAdvance(InputAction.CallbackContext obj)
    {
        if (ShouldMovementControlsEnable())
        {
            _shipsUI.GetSelectedShip().Advance();
        }
    }

    private InputAction getAdvanceAction()
    {
        return _input.actions["Advance"];
    }
    
    private InputAction getPortTurnAction()
    {
        return _input.actions["Port Turn"];
    }

    private InputAction getStarboardTurnAction()
    {
        return _input.actions["Starboard Turn"];
    }

    private void OnDisable()
    {
        getAdvanceAction().performed -= TryAdvance;
        getPortTurnAction().performed -= TryPortTurn;
        getStarboardTurnAction().performed -= TryStarboardTurn;
    }
}
