using System.Linq;
using Model;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller.Input
{
    public class InputBehavior : MonoBehaviour
    {
        protected PlayerInput _input;
        protected ShipUIManager _shipsUI;
        protected PhaseManager _phaseManager;

        void Awake()
        {
            _input = GetComponent<PlayerInput>();
            _shipsUI = FindObjectOfType<ShipUIManager>();
            _phaseManager = FindObjectOfType<PhaseManager>();
        }
        
        protected bool ShouldMovementControlsEnable()
        {
            Ship selectedShip = _shipsUI.GetSelectedShip();
            if (selectedShip != null)
            {
                if (_phaseManager.getShipActionsThisPhase(selectedShip).Any(action => action.actionType.isMovement()))
                {
                    return true;
                }
            }

            return false;
        }

        protected bool ShouldTargetingControlsEnable()
        {
            Ship selectedShip = _shipsUI.GetSelectedShip();
            if (selectedShip != null)
            {
                if (_phaseManager.HasShipChosenAnyActionThisPhaseNamed(selectedShip, "Shoot"))
                {
                    return true;
                }
            }


            return false;
        }
    }
}
