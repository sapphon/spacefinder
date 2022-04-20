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
                CrewAction currentAction = _phaseManager.getShipAction(selectedShip);
                if (currentAction != null && currentAction.name == "Maneuver")
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
                CrewAction currentAction = _phaseManager.getShipAction(selectedShip);
                if (currentAction != null && currentAction.name == "Shoot")
                {
                    return true;
                }
            }

            return false;
        }
    }
}
