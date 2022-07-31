using Model;
using Model.Crew;
using UnityEngine;

namespace Controller
{
    public class FiringSolution
    {
        private readonly Vector3Int _attackerPosition;
        private readonly Facing _attackerFacing;
        private readonly Vector3Int _targetPosition;
        private readonly Facing _targetFacing;
        private readonly Weapon _weapon;
        private readonly CrewMember _gunner;

        public FiringSolution(Vector3Int attackerPosition, Facing attackerFacing, Ship target, Weapon weapon)
        {
            _attackerPosition = attackerPosition;
            _attackerFacing = attackerFacing;
            _targetPosition = target.gridPosition;
            _targetFacing = target.facing;
            _weapon = weapon;
        }
        
        public FiringSolution(Vector3Int attackerPosition, Facing attackerFacing, Ship target, Weapon weapon, CrewMember gunner)
        {
            _attackerPosition = attackerPosition;
            _attackerFacing = attackerFacing;
            _targetPosition = target.gridPosition;
            _targetFacing = target.facing;
            _weapon = weapon;
            _gunner = gunner;
        }
        
        public FiringSolution(Ship attacker, Ship target, Weapon weapon)
        {
            _attackerPosition = attacker.gridPosition;
            _attackerFacing = attacker.facing;
            _targetPosition = target.gridPosition;
            _targetFacing = target.facing;
            _weapon = weapon;
        }

        public bool isInRange()
        {
            var distanceBetween = Util.DistanceBetween(_attackerPosition, _targetPosition);
            var maxRange = (int) _weapon.range * 10;
            Util.logIfDebugging("Range calculation: maximum " + maxRange + ", actual " + distanceBetween);
            return distanceBetween <= maxRange;
        }

        public bool isInArc()
        {
            WeaponFiringArc arc = _weapon.arc;
            bool toReturn = false;
            if (arc == WeaponFiringArc.Turret)
            {
                toReturn = true;
                Util.logIfDebugging("Attack is in arc, because the weapon is turreted.");

            }
            else
            {
                var angleBetween = Util.getAngleBetween(_attackerPosition, _attackerFacing, _targetPosition);

                if (arc == WeaponFiringArc.Fore)
                {
                    if (Mathf.Abs(angleBetween) <= 30f || Mathf.Approximately(Mathf.Abs(angleBetween), 30f))
                        toReturn = true;
                }
                else if (arc == WeaponFiringArc.Aft)
                {
                    if (Mathf.Abs(angleBetween) >= 150f || Mathf.Approximately(Mathf.Abs(angleBetween), 150f))
                        toReturn = true;
                }
                else if (arc == WeaponFiringArc.Port)
                {
                    if (angleBetween > 30f && angleBetween < 150f) toReturn = true;
                }
                else if (arc == WeaponFiringArc.Starboard)
                {
                    if (angleBetween < -30f && angleBetween > -150f) toReturn = true;
                }

                Util.logIfDebugging("Attack " + (toReturn
                                        ? " is "
                                        : " is not ") + " in the " + arc +
                                    " arc, according with an angle of incidence of " + angleBetween + ".");

            }
            return toReturn;
        }

        public bool isWithinOneRangeBand()
        {
            var distanceBetween = Util.DistanceBetween(_attackerPosition, _targetPosition);
            var shortestRange = (int) _weapon.range;
            Util.logIfDebugging("Short range calculation: shortest band " + shortestRange + ", actual " + distanceBetween);
            return distanceBetween <= shortestRange;

        }
    }
}