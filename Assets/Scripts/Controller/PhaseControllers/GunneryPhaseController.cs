using System;
using System.Collections.Generic;
using System.Linq;
using Model;
using Model.Crew;
using UnityEngine;

namespace Controller.PhaseControllers
{
    public class GunneryPhaseController : MonoBehaviour, IPhaseController
    {
        private ShipUIManager _shipUiManager;
        private InitiativeController _initiativeUIController;
        private HashSet<FiringSolution> firingSolutions;

        void Awake()
        {
            _initiativeUIController = FindObjectOfType<InitiativeController>();
            _shipUiManager = FindObjectOfType<ShipUIManager>();
            this.firingSolutions = new HashSet<FiringSolution>();
        }


        public void OnPhaseBegin()
        {
            this._initiativeUIController.SendInitiativesToPhaseManager();
        }

        public void OnPhaseEnd()
        {
            FireOnAllSolutions();
            this.firingSolutions.Clear();
            this._shipUiManager.UpdateAttackMarkers(this.firingSolutions);
        }

        public void OnActionBegin(CrewAction action, Ship ship)
        {
        }

        public void OnActionEnd(CrewAction action, Ship ship)
        {
        }

        public void FireOnAllSolutions()
        {
            DiceRoller rnJesus = new DiceRoller();
            foreach (FiringSolution solution in this.firingSolutions)
            {
                int toHitRoll = rnJesus.rollAndTotal(1, Die.D20);
                int gunneryBonus = (solution.gunner != null ? solution.gunner.gunneryBonus : 0);
                Util.logIfDebugging(solution.attacker.displayName + " fired " + solution.weapon.name + " at " +
                          solution.target.displayName + ", rolled " + toHitRoll +
                          "+" + gunneryBonus + "; needed " + solution.target.armorClass + ".");
                bool hit = toHitRoll + gunneryBonus >= solution.target.armorClass;
                if (hit)
                {
                    new HitResolver(solution, rnJesus).Resolve();
                }
            }

            this.firingSolutions.Clear();
        }

        public void OnActionCancel(CrewAction action, Ship ship)
        {
            if (action.actionType == Action.findByName("Shoot"))
            {
                //for larger ships, taking into account which gunner is canceling will be necessary
                this.firingSolutions.RemoveWhere(solution => solution.attacker == ship);
                this._shipUiManager.UpdateAttackMarkers(this.firingSolutions);
            }
        }

        public bool IsInRangeAndArc(Ship attacker, Ship target, Weapon weapon)
        {
            FiringSolution potentialSolution = new FiringSolution(attacker, target, weapon);
            if (isInRange(potentialSolution) && isInArc(potentialSolution))
            {
                return true;
            }

            return false;
        }

        public bool TryTarget(Ship attacker, Ship target, Weapon weapon, CrewMember gunner)
        {
            Util.logIfDebugging("Attempting targeting of " + target.name + " by " + attacker.name + " with " + weapon.name);
            if (IsInRangeAndArc(attacker, target, weapon))
            {
                ClearPreviousTargetForWeapon(attacker, weapon);
                ClearPreviousSolutionsFor(gunner);
                DoTarget(attacker, target, weapon, gunner);
                Util.logIfDebugging("Targeting successful");
                return true;
            }

            Util.logIfDebugging("Targeting unsuccessful");
            return false;
        }


        public bool isInArc(FiringSolution solution)
        {
            return isInArc(solution.attacker.gridPosition, solution.attacker.facing, solution.target.gridPosition, solution.weapon.arc);
        }
        
        public bool isInArc(Vector3Int origin, Facing facing, Vector3Int target, WeaponFiringArc arc)
        {
            bool toReturn = false;
            if (arc == WeaponFiringArc.Turret)
            {
                toReturn = true;
                Util.logIfDebugging("Attack is in arc, because the weapon is turreted.");

            }
            else
            {
                var angleBetween = Util.getAngleBetween(origin, facing, target);

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

        public bool isInRange(FiringSolution solution)
        {
            var distanceBetween = Util.DistanceBetween(solution.attacker.gridPosition, solution.target.gridPosition);
            var maxRange = (int) solution.weapon.range * 10;
            Util.logIfDebugging("Range calculation: maximum " + maxRange + ", actual " + distanceBetween);
            return distanceBetween <= maxRange;
        }

        private void DoTarget(Ship attacker, Ship target, Weapon weapon, CrewMember gunner)
        {
            this.firingSolutions.Add(new FiringSolution(attacker, target, weapon, gunner));
            this._shipUiManager.UpdateAttackMarkers(this.firingSolutions);
        }

        private void ClearPreviousTargetForWeapon(Ship attacker, Weapon weapon)
        {
            this.firingSolutions.RemoveWhere(solution =>
                solution.weapon == weapon && solution.attacker == attacker);
        }
        private void ClearPreviousSolutionsFor(CrewMember gunner)
        {
            this.firingSolutions.RemoveWhere(solution => solution.gunner == gunner);
        }

        public bool IsTargeted(Ship ship)
        {
            return this.firingSolutions.Any(solution => solution.target == ship);
        }
    }
    

    public struct FiringSolution
    {
        public Ship attacker;
        public Ship target;
        public Weapon weapon;
        public CrewMember gunner;

        public FiringSolution(Ship attacker, Ship target, Weapon weapon, CrewMember gunner)
        {
            this.attacker = attacker;
            this.target = target;
            this.weapon = weapon;
            this.gunner = gunner;
        }

        public FiringSolution(Ship attacker, Ship target, Weapon weapon)
        {
            this.attacker = attacker;
            this.target = target;
            this.weapon = weapon;
            this.gunner = null;
        }
    }
}