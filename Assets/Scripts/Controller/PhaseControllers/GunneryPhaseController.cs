using System;
using Model;
using UnityEngine;

namespace Controller.PhaseControllers
{
    public class GunneryPhaseController : MonoBehaviour, IPhaseController
    {
        private ShipUIManager _shipUiManager;
        private InitiativeController _initiativeUIController;

        void Awake()
        {
            _initiativeUIController = FindObjectOfType<InitiativeController>();
            _shipUiManager = FindObjectOfType<ShipUIManager>();
        }


        public void OnPhaseBegin()
        {
            this._initiativeUIController.SendInitiativesToPhaseManager();
        }

        public void OnActionBegin(CrewAction action, Ship ship)
        {
            if (action.name == "Shoot")
            {
                //do a shoot
            }
        }

        public void OnActionEnd(CrewAction action, Ship ship)
        {
        
        }

        public void OnActionCancel(CrewAction action, Ship ship)
        {
            if (action.name == "Shoot")
            {
                //cancel shoot
            }
        }

        public bool MayTarget(Ship attacker, Ship target, Weapon weapon)
        {
            //is attacker's init
            //weapon is not targeting a different target
            //weapon is in range
            //target is in weapon arc
            return true;
            //Future: Attacker has available gunner: https://trello.com/c/lCGGu5gH
        }

        public bool TryTarget(Ship attacker, Ship target, Weapon weapon)
        {
            if (MayTarget(attacker, target, weapon))
            {
                
                return true;
            }
            return false;
        }

    }
}