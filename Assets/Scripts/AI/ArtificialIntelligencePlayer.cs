using System;
using System.Collections.Generic;
using System.Linq;
using Controller.PhaseControllers;
using Model;
using NUnit.Framework.Constraints;
using UnityEngine;

namespace AI
{
    public class ArtificialIntelligencePlayer : MonoBehaviour
    {
        private PhaseManager phaseManager;
        private Ship controlledShip;
        private GunneryPhaseController gunneryController;

        void Awake()
        {
            this.phaseManager = FindObjectOfType<PhaseManager>();
            this.gunneryController = FindObjectOfType<GunneryPhaseController>();
            this.controlledShip = GetComponent<Ship>();
        }

        public static Ship[] getAllAIControlledShips()
        {
            return Ship.getAllShips().Where(ship => ship.isArtificiallyIntelligentlyControlled).ToArray();
        }

        public void YourTurn(Phase phase)
        {
            if (phase == Phase.Engineering)
            {
                phaseManager.ToggleShipAction(controlledShip,
                    "Hold It Together");
            }
            else if (phase == Phase.Gunnery)
            {
                fireAllWeaponsAtRandomOpponents();
            }
            else if (phase == Phase.Helm)
            {
                moveTowardsOpponentNearestFrontArc();
            }

            phaseManager.SignalComplete(controlledShip);
        }

        private void moveTowardsOpponentNearestFrontArc()
        {
            phaseManager.ToggleShipAction(controlledShip, "Maneuver");
            //sort by angle from front, then first by existing ability to target
            Ship[] followCandidates = Ship.getAllShips()
                .Where(ship => ship.affiliation != controlledShip.affiliation)
                .OrderBy(ship => Util.DistanceBetween(controlledShip.gridPosition, ship.gridPosition)).ToArray();
            foreach (var candidate in followCandidates)
            {
                if (gunneryController.MayTarget(controlledShip, candidate,
                    controlledShip.getShortestRangeWeaponInArc(WeaponFiringArc.Fore)))
                    return;
            }
        }

        private void fireAllWeaponsAtRandomOpponents()
        {
            phaseManager.ToggleShipAction(controlledShip, "Shoot");
            foreach (Weapon toShoot in this.controlledShip.weapons)
            {
                Ship toTarget = Ship.getAllShips().Where(ship => ship.affiliation != controlledShip.affiliation)
                    .FirstOrDefault(ship => gunneryController.MayTarget(this.controlledShip, ship, toShoot));
                if (toTarget != null)
                {
                    Util.logIfDebugging("AI player controlling ship " + this.controlledShip.displayName +
                                        " has chosen ship " + toTarget.displayName + " as a target for " +
                                        toShoot.name);
                    gunneryController.TryTarget(this.controlledShip, toTarget, toShoot);
                }
            }
        }
    }
}