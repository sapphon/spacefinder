using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Controller.PhaseControllers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class KeyboardInputBehavior : MonoBehaviour
{
    private PlayerInput _input;
    private ShipUIManager _shipsUI;
    private HelmPhaseController _helmPhaseController;
    private GunneryPhaseController _gunneryPhaseController;
    private PhaseManager _phaseManager;


    void Awake()
    {
        _gunneryPhaseController = FindObjectOfType<GunneryPhaseController>();
        _helmPhaseController = FindObjectOfType<HelmPhaseController>();
        _input = GetComponent<PlayerInput>();
        _shipsUI = FindObjectOfType<ShipUIManager>();
        _phaseManager = FindObjectOfType<PhaseManager>();
    }

    private void OnEnable()
    {
        getAdvanceAction().performed += TryAdvance;
        getPortTurnAction().performed += TryPortTurn;
        getStarboardTurnAction().performed += TryStarboardTurn;
        getToggleArcsAction().performed += ToggleArcs;
        getResetActionAction().performed += ResetAction;
        getToggleBandsAction().performed += ToggleRangeBands;
        getEndShipPhaseAction().performed += EndShipPhase;
        for (int i = 1; i <= 12; i++)
        {
            getSelectWeaponAction(i).performed += TrySelectWeapon;
        }

    }

    private bool ShouldMovementControlsEnable()
    {
        Ship selectedShip = _shipsUI.GetSelectedShip();
        if (selectedShip != null)
        {
            CrewAction currentAction = _phaseManager.getShipAction(selectedShip);
            if (currentAction != null && currentAction.name == "Maneuver")
            {
                return true;
            }
        }
        return false;
    }

    private void TrySelectWeapon(InputAction.CallbackContext obj)
    {
        int weaponOrdinal = getWeaponNumberFromActionName(obj.action);
        _shipsUI.TrySelectWeapon(weaponOrdinal);

    }

    private static int getWeaponNumberFromActionName(InputAction action)
    {
        return Int32.Parse(action.name.Split(' ').Last());
    }

    private void TryStarboardTurn(InputAction.CallbackContext obj)
    {
        if (ShouldMovementControlsEnable())
        {
            _helmPhaseController.TryStarboardTurn(_shipsUI.GetSelectedShip());
        }
    }

    private void TryPortTurn(InputAction.CallbackContext obj)
    {
        if (ShouldMovementControlsEnable())
        {
            _helmPhaseController.TryPortTurn(_shipsUI.GetSelectedShip());
        }
    }

    private void TryAdvance(InputAction.CallbackContext obj)
    {
        if (ShouldMovementControlsEnable())
        {
            _helmPhaseController.TryAdvance(_shipsUI.GetSelectedShip());
        }
    }

    private void ToggleArcs(InputAction.CallbackContext obj)
    {
        _shipsUI.SetShowingArcs(!_shipsUI.GetShowingArcs());
    }
    
    private void ToggleRangeBands(InputAction.CallbackContext obj)
    {
        _shipsUI.SetShowingRange(_shipsUI.GetShowingRange() > 0 ? 0 : 5);
    }

    private void ResetAction(InputAction.CallbackContext obj)
    {
        if (_shipsUI.GetSelectedShip() != null &&
            _phaseManager.HasShipChosenActionThisPhase(_shipsUI.GetSelectedShip()))
        {
            _phaseManager.ResetAction(_shipsUI.GetSelectedShip());
        }
    }

    private void EndShipPhase(InputAction.CallbackContext obj)
    {
        Ship selectedShip = _shipsUI.GetSelectedShip();
        if (selectedShip != null)
        {
            _phaseManager.SignalComplete(selectedShip);
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
    
    private InputAction getToggleArcsAction()
    {
        return _input.actions["Toggle Firing Arcs"];
    }
    
    private InputAction getEndShipPhaseAction()
    {
        return _input.actions["End Phase for Ship"];
    }
    
    private InputAction getToggleBandsAction()
    {
        return _input.actions["Toggle Range Bands"];
    }
    
    private InputAction getResetActionAction()
    {
        return _input.actions["Reset Action"];
    }
    
    private InputAction getSelectWeaponAction(int weapon)
    {
        return _input.actions["Select Weapon "+weapon];
    }

    private void OnDisable()
    {
        getAdvanceAction().performed -= TryAdvance;
        getPortTurnAction().performed -= TryPortTurn;
        getStarboardTurnAction().performed -= TryStarboardTurn;
        getToggleArcsAction().performed -= ToggleArcs;
        getResetActionAction().performed -= ResetAction;
        getEndShipPhaseAction().performed -= EndShipPhase;
        for (int i = 1; i <= 12; i++)
        {
            getSelectWeaponAction(i).performed -= TrySelectWeapon;
        }
    }
}
