using System;
using System.Linq;
using Controller.PhaseControllers;
using UnityEngine.InputSystem;

namespace Controller.Input
{
    public class KeyboardInputBehavior : InputBehavior
    {
        private HelmPhaseController _helmPhaseController;

        void Awake()
        {
            _helmPhaseController = FindObjectOfType<HelmPhaseController>();
            _input = GetComponent<PlayerInput>();
            _shipsUI = FindObjectOfType<ShipUIManager>();
            _phaseManager = FindObjectOfType<PhaseManager>();
        }

        private void OnEnable()
        {
            getAdvanceAction().performed += tryAdvance;
            getPortTurnAction().performed += tryPortTurn;
            getStarboardTurnAction().performed += tryStarboardTurn;
            getToggleArcsAction().performed += toggleArcs;
            getResetActionAction().performed += resetAction;
            getToggleBandsAction().performed += toggleRangeBands;
            getEndShipPhaseAction().performed += endShipPhase;
            for (int i = 1; i <= 12; i++)
            {
                getSelectWeaponAction(i).performed += trySelectWeapon;
            }
        }

        private void trySelectWeapon(InputAction.CallbackContext obj)
        {
            if (ShouldTargetingControlsEnable())
            {
                int weaponOrdinal = getWeaponNumberFromActionName(obj.action);
                _shipsUI.TrySelectWeapon(weaponOrdinal);
            }
        }

        private static int getWeaponNumberFromActionName(InputAction action)
        {
            return Int32.Parse(action.name.Split(' ').Last());
        }

        private void tryStarboardTurn(InputAction.CallbackContext obj)
        {
            if (ShouldMovementControlsEnable())
            {
                _helmPhaseController.TryStarboardTurn(_shipsUI.GetSelectedShip());
            }
        }

        private void tryPortTurn(InputAction.CallbackContext obj)
        {
            if (ShouldMovementControlsEnable())
            {
                _helmPhaseController.TryPortTurn(_shipsUI.GetSelectedShip());
            }
        }

        private void tryAdvance(InputAction.CallbackContext obj)
        {
            if (ShouldMovementControlsEnable())
            {
                _helmPhaseController.TryAdvance(_shipsUI.GetSelectedShip());
            }
        }

        private void toggleArcs(InputAction.CallbackContext obj)
        {
            _shipsUI.SetShowingArcs(!_shipsUI.GetShowingArcs());
        }

        private void toggleRangeBands(InputAction.CallbackContext obj)
        {
            _shipsUI.SetShowingRange(_shipsUI.GetShowingRange() > 0 ? 0 : 5);
        }

        private void resetAction(InputAction.CallbackContext obj)
        {
            if (_shipsUI.GetSelectedShip() != null &&
                _phaseManager.HasShipChosenActionThisPhase(_shipsUI.GetSelectedShip()))
            {
                _phaseManager.ResetAction(_shipsUI.GetSelectedShip());
            }
        }

        private void endShipPhase(InputAction.CallbackContext obj)
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
            return _input.actions["Select Weapon " + weapon];
        }

        private void OnDisable()
        {
            getAdvanceAction().performed -= tryAdvance;
            getPortTurnAction().performed -= tryPortTurn;
            getStarboardTurnAction().performed -= tryStarboardTurn;
            getToggleArcsAction().performed -= toggleArcs;
            getResetActionAction().performed -= resetAction;
            getEndShipPhaseAction().performed -= endShipPhase;
            for (int i = 1; i <= 12; i++)
            {
                getSelectWeaponAction(i).performed -= trySelectWeapon;
            }
        }
    }
}