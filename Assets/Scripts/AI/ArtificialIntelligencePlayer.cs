using System;
using System.Collections.Generic;
using System.Linq;
using Controller;
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
        private HelmPhaseController helmController;

        void Awake()
        {
            this.phaseManager = FindObjectOfType<PhaseManager>();
            this.gunneryController = FindObjectOfType<GunneryPhaseController>();
            this.helmController = FindObjectOfType<HelmPhaseController>();
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

        private bool isInIdealRangeForArc(Ship target, WeaponFiringArc arc)
        {
            Weapon shortestRangeWeaponInArc = controlledShip.getShortestRangeWeaponInArc(arc);
            return gunneryController.MayTarget(controlledShip, target,
                       shortestRangeWeaponInArc) &&
                   Util.DistanceBetween(controlledShip.gridPosition, target.gridPosition) <=
                   (int) shortestRangeWeaponInArc.range;
        }

        private void moveTowardsOpponentNearestFrontArc()
        {
            Ship[] followCandidates = Ship.getAllShips()
                .Where(ship => ship.affiliation != controlledShip.affiliation)
                .OrderBy(ship => Util.DistanceBetween(controlledShip.gridPosition, ship.gridPosition)).ToArray();
            if (followCandidates.Any(candidate =>
                isInIdealRangeForArc(candidate, WeaponFiringArc.Fore)))
            {
                return;
            }

            foreach (var candidate in followCandidates)
            {
                if (pathTowards(candidate)) return;
                else phaseManager.ResetAction(controlledShip);
            }

            Util.logIfDebugging("Ship " + controlledShip.displayName +
                                " cannot find a way to put any opponents in its Front arc at shortest range!");
        }

        private bool pathTowards(Ship destination)
        {
            phaseManager.ToggleShipAction(controlledShip, "Maneuver");
            while (helmController.MayAdvance(controlledShip))
            {
                while (Mathf.Abs(Util.getAngleBetweenShips(controlledShip, destination)) >= 30 &&
                       helmController.MayTurn(controlledShip))
                {
                    if (Util.getAngleBetweenShips(controlledShip, destination) < 0)
                    {
                        helmController.TryStarboardTurn(controlledShip);
                    }
                    else
                    {
                        helmController.TryPortTurn(controlledShip);
                    }
                }

                if (isInIdealRangeForArc(destination, WeaponFiringArc.Fore))
                {
                    return true;
                }
                else
                {
                    helmController.TryAdvance(controlledShip);
                }
            }
            return isInIdealRangeForArc(destination, WeaponFiringArc.Fore);
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