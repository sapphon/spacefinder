using System;
using System.Collections.Generic;
using System.Linq;
using Model;
using UnityEngine;

namespace Controller.PhaseControllers
{
    public class HelmPhaseController : MonoBehaviour, IPhaseController
    {
        private InitiativeController _initiativeUIController;
        private string[] _movementActions = new[]{"Maneuver", "Fly"};
        private ShipMovementState _currentMovementState;

        void Awake()
        {
            _initiativeUIController = FindObjectOfType<InitiativeController>();
            _currentMovementState = null;
        }


        public void OnPhaseBegin()
        {
            this._initiativeUIController.GatherInitiatives();
        }

        public void OnPhaseEnd()
        {
        }

        public void OnActionBegin(CrewAction action, Ship ship)
        {
            if (isAMovementAction(action.actionType))
            {
                this._currentMovementState = new ShipMovementState(ship);
            }
        }

        public void OnActionEnd(CrewAction action, Ship ship)
        {
            if (isAMovementAction(action.actionType))
            {
                _currentMovementState = null;
            }
        }

        public void OnActionCancel(CrewAction action, Ship ship)
        {
            if (isAMovementAction(action.actionType))
            {
                _currentMovementState.Reset();
                _currentMovementState = null;
            }
        }

        public bool isAMovementAction(Action actionType)
        {
            return _movementActions.Contains(actionType.name);
        }

        public bool TryStarboardTurn(Ship ship)
        {
            if (_currentMovementState.MayTurn())
            {
                this._currentMovementState.Turn(WeaponFiringArc.Starboard);
                ship.TurnToStarboard();
                return true;
            }

            return false;
        }

        public bool TryPortTurn(Ship ship)
        {
            if (_currentMovementState.MayTurn())
            {
                this._currentMovementState.Turn(WeaponFiringArc.Port);
                ship.TurnToPort();
                return true;
            }

            return false;
        }

        public bool TryAdvance(Ship ship)
        {
            if (_currentMovementState.MayAdvance())
            {
                this._currentMovementState.Advance();
                ship.Advance();
                return true;
            }

            return false;
        }

        public ShipMovementState getMovementState()
        {
            return this._currentMovementState;
        }
    }
}