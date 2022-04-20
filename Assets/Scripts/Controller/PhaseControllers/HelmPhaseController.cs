using System;
using System.Collections.Generic;
using UnityEngine;

namespace Controller.PhaseControllers
{
    public class HelmPhaseController : MonoBehaviour, IPhaseController
    {
        private List<Vector3Int> _turnsSoFar;
        private List<Vector3Int> _destinationsSoFar;
        private Vector3Int _initialPosition;
        private Facing _initialFacing;
        private InitiativeController _initiativeUIController;

        void Awake()
        {
            _initiativeUIController = FindObjectOfType<InitiativeController>();
            _turnsSoFar = new List<Vector3Int>();
            _destinationsSoFar = new List<Vector3Int>();
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
            if (action.name == "Maneuver")
            {
                this._initialPosition = ship.gridPosition;
                this._initialFacing = ship.facing;
                _destinationsSoFar.Clear();
                _turnsSoFar.Clear();
            }
        }

        public void OnActionEnd(CrewAction action, Ship ship)
        {

        }

        public void OnActionCancel(CrewAction action, Ship ship)
        {
            if (action.name == "Maneuver")
            {
                _turnsSoFar.Clear();
                _destinationsSoFar.Clear();
                ship.gridPosition = _initialPosition;
                ship.facing = _initialFacing;
            }
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
                       _turnsSoFar[_turnsSoFar.Count - 1] != ship.gridPosition
                    ? 0
                    : 1;
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
}