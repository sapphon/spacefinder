using System;
using System.Collections.Generic;
using System.Linq;
using Model;
using UnityEngine;

namespace Controller
{
    public class ShipMovementState
    {
        private List<Tuple<Vector3Int, WeaponFiringArc>> _turnsSoFar;
        private List<Vector3Int> _destinationsSoFar;
        private Vector3Int _initialPosition;
        private Facing _initialFacing;
        private Ship mover;

        public ShipMovementState(Ship shipMoving)
        {
            mover = shipMoving;
            _turnsSoFar = new List<Tuple<Vector3Int, WeaponFiringArc>>();
            _destinationsSoFar = new List<Vector3Int>();
            _initialPosition = mover.gridPosition;
            _initialFacing = mover.facing;
        }

        public ShipMovementState(ShipMovementState other)
        {
            mover = other.mover;
            _turnsSoFar = new List<Tuple<Vector3Int, WeaponFiringArc>>(other._turnsSoFar);
            _destinationsSoFar = new List<Vector3Int>(other._destinationsSoFar);
            _initialFacing = other._initialFacing;
            _initialPosition = other._initialPosition;
        }

        public void Reset()
        {
            mover.gridPosition = _initialPosition;
            mover.facing = _initialFacing;
        }
        
        public bool MayAdvance()
        {
            return MovesRemaining() > 0;
        }

        public int MovesRemaining()
        {
            Util.logIfDebugging("MovementState movement remaining: " +
                                Math.Max(0, mover.speed - this._destinationsSoFar.Count));
            return Math.Max(0, mover.speed - this._destinationsSoFar.Count);
        }

        public int MovesUntilNextTurn()
        {
            Maneuverability maneuverability = mover.maneuverability;
            if (maneuverability == Maneuverability.Perfect)
            {
                return _turnsSoFar.Count < 2 ||
                       _turnsSoFar[_turnsSoFar.Count - 2].Item1 != GetCurrentPosition() ||
                       _turnsSoFar[_turnsSoFar.Count - 1].Item1 != GetCurrentPosition()
                    ? 0
                    : 1;
            }
            else
            {
                return Math.Max(0, (_turnsSoFar.Count + 1) * (int) maneuverability - _destinationsSoFar.Count);
            }
        }

        public bool MayTurn()
        {
            return MovesUntilNextTurn() == 0;
        }

        public bool Turn(WeaponFiringArc direction)
        {
            if (direction != WeaponFiringArc.Port && direction != WeaponFiringArc.Starboard)
            {
                Util.logIfDebugging("ShipMovementState cannot turn in the direction of " + direction);
                return false;
            }

            this._turnsSoFar.Add(new Tuple<Vector3Int, WeaponFiringArc>(GetCurrentPosition(), direction));
            return true;
        }

        public Facing GetCurrentFacing()
        {
            return this._initialFacing +
                   (60 * this._turnsSoFar.Count(turn => turn.Item2.Equals(WeaponFiringArc.Port)) +
                    300 * (this._turnsSoFar.Count(turn => turn.Item2.Equals(WeaponFiringArc.Starboard)))
                   ) % 360;
        }

        public Vector3Int GetCurrentPosition()
        {
            if (this._destinationsSoFar.Count < 1)
            {
                return this.mover.gridPosition;
            }
            else return _destinationsSoFar.Last();
        }

        public void Advance()
        {
            Facing currentFacing = GetCurrentFacing();
            Vector3Int gridPosition = GetCurrentPosition();
            if (currentFacing == Facing.N)
            {
                _destinationsSoFar.Add(new Vector3Int(gridPosition.x + 1, gridPosition.y, gridPosition.z));
            }
            else if (currentFacing == Facing.NW)
            {
                _destinationsSoFar.Add(new Vector3Int(gridPosition.x + (gridPosition.y % 2 == 0 ? 0 : 1), gridPosition.y - 1, gridPosition.z));
            }
            else if (currentFacing == Facing.SW)
            {
                _destinationsSoFar.Add(new Vector3Int(gridPosition.x - (gridPosition.y % 2 == 0 ? 1 : 0), gridPosition.y - 1, gridPosition.z));
            }
            else if (currentFacing == Facing.S)
            {
                _destinationsSoFar.Add(new Vector3Int(gridPosition.x - 1, gridPosition.y, gridPosition.z));
            }
            else if (currentFacing == Facing.SE)
            {
                _destinationsSoFar.Add(new Vector3Int(gridPosition.x - (gridPosition.y % 2 == 0 ? 1 : 0), gridPosition.y + 1, gridPosition.z));
            }
            else if (currentFacing == Facing.NE)
            {
                _destinationsSoFar.Add(new Vector3Int(gridPosition.x + (gridPosition.y % 2 == 0 ? 0 : 1), gridPosition.y + 1, gridPosition.z));
            }
        }
    }
}