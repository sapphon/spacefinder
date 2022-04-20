using System;
using System.Collections.Generic;
using System.Linq;
using Model;
using UnityEditor;
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
            FiringSolution potentialSolution = new FiringSolution(attacker, target, weapon);
            if (isInRange(potentialSolution) && isInArc(potentialSolution))
            {
                return true;
                //Future: Attacker has available gunner: https://trello.com/c/lCGGu5gH
            }

            return false;
        }

        public bool TryTarget(Ship attacker, Ship target, Weapon weapon)
        {
            Debug.Log("Attempting targeting of " + target.name + " by " + attacker.name + " with " + weapon.name);
            if (MayTarget(attacker, target, weapon))
            {
                ClearPreviousTargetForWeapon(attacker, weapon);
                DoTarget(attacker, target, weapon);
                Debug.Log("Targeting successful");
                return true;
            }
            Debug.Log("Targeting unsuccessful");
            return false;
        }
        
        private bool isInArc(FiringSolution solution)
        {
            if (solution.weapon.arc == WeaponFiringArc.Turret)
            {
                return true;
            }
            else
            {
                float angleBetween = Vector3.SignedAngle(solution.attacker.getForwardVectorInWorld(),
                    solution.target.getWorldSpacePosition()- solution.attacker.getWorldSpacePosition(),
                    Vector3.forward);
                Debug.Log("Firing angle " + angleBetween);
                if (solution.weapon.arc == WeaponFiringArc.Fore)
                {
                    if (Mathf.Abs(angleBetween) < 45f) return true;
                }

                return false;

            }
        }
        
        private bool isInRange(FiringSolution solution)
        {
            var distanceBetween = Util.DistanceBetween(solution.attacker.gridPosition, solution.target.gridPosition);
            var maxRange = (int) solution.weapon.range * 10;
            Debug.Log("Range calculation: maximum " + maxRange + ", actual " + distanceBetween);
            return distanceBetween <= maxRange;
        }

        private void DoTarget(Ship attacker, Ship target, Weapon weapon)
        {
            this.firingSolutions.Add(new FiringSolution(attacker, target, weapon));
            this._shipUiManager.UpdateAttackMarkers(this.firingSolutions);
        }

        private void ClearPreviousTargetForWeapon(Ship attacker, Weapon weapon)
        {
            this.firingSolutions.RemoveWhere(solution =>
                solution.weapon == weapon && solution.attacker == attacker);
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
        
        public FiringSolution(Ship attacker, Ship target, Weapon weapon)
        {
            this.attacker = attacker;
            this.target = target;
            this.weapon = weapon;
            this.gunner = null;
        }
        public FiringSolution(Ship attacker, Ship target, Weapon weapon, CrewMember gunner)
        {
            this.attacker = attacker;
            this.target = target;
            this.weapon = weapon;
            this.gunner = gunner;
        }                
    }
}