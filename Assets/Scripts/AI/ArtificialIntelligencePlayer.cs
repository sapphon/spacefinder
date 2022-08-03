using System;
using System.Collections.Generic;
using System.Linq;
using Controller;
using Controller.PhaseControllers;
using Model;
using Model.Crew;
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
        private WeaponFiringArc faceEnemyWithArc = WeaponFiringArc.Fore;
        private WeaponFiringArc stayInEnemyArc = WeaponFiringArc.Aft;

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
                if (isAnyUnoccupied(Crew.Role.Engineer))
                {
                    phaseManager.ToggleShipAction(controlledShip,
                        getUnoccupied(Crew.Role.Engineer), "Hold It Together");
                }
            }
            else if (phase == Phase.Gunnery)
            {
                fireAllWeaponsAtRandomOpponents();
            }
            else if (phase == Phase.Helm)
            {
                if (isAnyUnoccupied(Crew.Role.Pilot))
                {
                    moveTowardsOpponentNearestFrontArc();
                }
            }

            phaseManager.SignalComplete(controlledShip);
        }

        private CrewMember getUnoccupied(Crew.Role role)
        {
            return controlledShip.crew.getMembers().Where(person => person.role == role)
                .FirstOrDefault(person => phaseManager.getCrewpersonActionsThisPhase(person).Count < 1);
        }

        private bool isAnyUnoccupied(Crew.Role role)
        {
            return getUnoccupied(role) != null;
        }

        private bool isInIdealRangeForArc(Ship target, WeaponFiringArc arc)
        {
            return isInIdealRangeForArc(getCurrentDisposition(), target, arc);
        }

        private Tuple<Vector3Int, Facing> getCurrentDisposition()
        {
            return new Tuple<Vector3Int, Facing>(controlledShip.gridPosition, controlledShip.facing);
        }

        private bool isInIdealRangeForArc(Tuple<Vector3Int, Facing> disposition, Ship target, WeaponFiringArc arc)
        {
            Weapon shortestRangeWeaponInArc = controlledShip.getShortestRangeWeaponInArc(arc);
            FiringSolution potentialSolution = new FiringSolution(disposition.Item1, disposition.Item2, target,
                shortestRangeWeaponInArc);
            return potentialSolution.isInArc() && potentialSolution.isWithinOneRangeBand();
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
                if (pathTowards(candidate))
                {
                    phaseManager.SignalComplete(controlledShip);
                    return;
                }

                else
                {
                    phaseManager.ResetAction(controlledShip);
                    Util.logIfDebugging("AI controlling ship " + controlledShip.displayName +
                                        "could not move optimally; you can still move it manually");
                }
            }
        }

        private HashSet<Tuple<Vector3Int, Facing>> findValidDispositions(ShipMovementState state,
            HashSet<Tuple<Vector3Int, Facing>> dispositions)
        {
            Tuple<Vector3Int, Facing> current =
                new Tuple<Vector3Int, Facing>(state.GetCurrentPosition(), state.GetCurrentFacing());
            if (!dispositions.Contains(current))
            {
                dispositions.Add(current);
                Util.logIfDebugging("Dispo count: " + dispositions.Count);
            }

            if (state.MayTurn())
            {
                ShipMovementState portTurnState = new ShipMovementState(state);
                portTurnState.Turn(WeaponFiringArc.Port);
                dispositions.UnionWith(findValidDispositions(portTurnState, dispositions));
                ShipMovementState starboardTurnState = new ShipMovementState(state);
                starboardTurnState.Turn(WeaponFiringArc.Starboard);
                dispositions.UnionWith(findValidDispositions(starboardTurnState, dispositions));
                Util.logIfDebugging("Dispo count: " + dispositions.Count);
            }

            if (state.MayAdvance())
            {
                ShipMovementState advanceState = new ShipMovementState(state);
                advanceState.Advance();
                dispositions.UnionWith(findValidDispositions(advanceState, dispositions));
                Util.logIfDebugging("Dispo count: " + dispositions.Count);
            }

            Util.logIfDebugging("Dispo count: " + dispositions.Count);
            return dispositions;
        }

        private bool pathTowards(Ship destination)
        {
            phaseManager.ToggleShipAction(controlledShip, getUnoccupied(Crew.Role.Pilot), "Maneuver");
            HashSet<Tuple<Vector3Int, Facing>> dispositions = this.findValidDispositions(
                new ShipMovementState(this.controlledShip),
                new HashSet<Tuple<Vector3Int, Facing>>());
            Tuple<Vector3Int, Facing> bestDisposition = dispositions.FirstOrDefault(disposition =>
                isInIdealRangeForArc(disposition, destination, this.faceEnemyWithArc));

            if (bestDisposition == null) return false;
            positionControlledShip(bestDisposition);
            return true;
        }

        private void positionControlledShip(Tuple<Vector3Int, Facing> bestDisposition)
        {
            controlledShip.gridPosition = bestDisposition.Item1;
            controlledShip.facing = bestDisposition.Item2;
        }


        private void fireAllWeaponsAtRandomOpponents()
        {
            foreach (Weapon toShoot in this.controlledShip.weapons)
            {
                if (!isAnyUnoccupied(Crew.Role.Gunner)) return;
                CrewMember crewpersonActing = getUnoccupied(Crew.Role.Gunner);
                phaseManager.ToggleShipAction(controlledShip, crewpersonActing, "Shoot");
                Ship toTarget = Ship.getAllShips().Where(ship => ship.affiliation != controlledShip.affiliation)
                    .FirstOrDefault(ship =>
                    {
                        FiringSolution solution = new FiringSolution(this.controlledShip, ship, toShoot);
                        return solution.isInArc() && solution.isInRange();
                    });
                if (toTarget != null)
                {
                    Util.logIfDebugging("AI player controlling ship " + this.controlledShip.displayName +
                                        " has chosen ship " + toTarget.displayName + " as a target for " +
                                        toShoot.name);
                    gunneryController.TryTarget(this.controlledShip, toTarget, toShoot, crewpersonActing);
                }
            }
        }
    }
}