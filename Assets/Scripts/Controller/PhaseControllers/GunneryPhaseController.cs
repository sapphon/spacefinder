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
        private HashSet<FiringSolutionStruct> firingSolutions;

        void Awake()
        {
            _initiativeUIController = FindObjectOfType<InitiativeController>();
            _shipUiManager = FindObjectOfType<ShipUIManager>();
            this.firingSolutions = new HashSet<FiringSolutionStruct>();
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
            foreach (FiringSolutionStruct solution in this.firingSolutions)
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

        public bool TryTarget(Ship attacker, Ship target, Weapon weapon, CrewMember gunner)
        {
            Util.logIfDebugging("Attempting targeting of " + target.name + " by " + attacker.name + " with " + weapon.name);
            FiringSolution potentialSolution = new FiringSolution(attacker.gridPosition, attacker.facing, target, weapon, gunner);
            if (potentialSolution.isInArc() && potentialSolution.isInRange())
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

        private void DoTarget(Ship attacker, Ship target, Weapon weapon, CrewMember gunner)
        {
            this.firingSolutions.Add(new FiringSolutionStruct(attacker, target, weapon, gunner));
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
    

    public struct FiringSolutionStruct
    {
        public Ship attacker;
        public Ship target;
        public Weapon weapon;
        public CrewMember gunner;

        public FiringSolutionStruct(Ship attacker, Ship target, Weapon weapon, CrewMember gunner)
        {
            this.attacker = attacker;
            this.target = target;
            this.weapon = weapon;
            this.gunner = gunner;
        }

        public FiringSolutionStruct(Ship attacker, Ship target, Weapon weapon)
        {
            this.attacker = attacker;
            this.target = target;
            this.weapon = weapon;
            this.gunner = null;
        }
    }
}