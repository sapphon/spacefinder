using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AI;
using Controller.PhaseControllers;
using JetBrains.Annotations;
using Model;
using Model.Crew;
using NUnit.Framework.Constraints;
using UnityEngine;
using View;

namespace Controller
{
    public class PhaseManager : MonoBehaviour
    {
        protected Phase currentPhase = Phase.Engineering;
        protected List<Ship> shipsSignalingComplete = new List<Ship>();
        private HelmPhaseController _helmPhaseController;
        private GunneryPhaseController _gunneryPhaseController;
        private Queue<Ship> _shipsYetToActInOrder = new Queue<Ship>();
        private ShipUIManager _shipsUI;
        private Dictionary<CrewAction, Ship> actionsThisPhase;
        private IPhaseController _engineeringPhaseController;
        private CrewPanelUI _crewUI;


        void Awake()
        {
            _helmPhaseController = FindObjectOfType<HelmPhaseController>();
            _gunneryPhaseController = FindObjectOfType<GunneryPhaseController>();
            _engineeringPhaseController = FindObjectOfType<EngineeringPhaseController>();
            _shipsUI = FindObjectOfType<ShipUIManager>();
            _crewUI = FindObjectOfType <CrewPanelUI>();
            actionsThisPhase = new Dictionary<CrewAction, Ship>();
        }

        void Start()
        {
            this.NotifyBeginPhase();
        }

        public Phase GetCurrentPhase()
        {
            return this.currentPhase;
        }

        public List<Crew.Role> GetActivePhaseRoles()
        {
            switch (this.currentPhase)
            {
                case Phase.Engineering:
                    return new List<Crew.Role>() {Crew.Role.Engineer, Crew.Role.Captain};
                case Phase.Helm:
                    return new List<Crew.Role>() {Crew.Role.Pilot, Crew.Role.Scientist, Crew.Role.Captain};
                case Phase.Gunnery:
                    return new List<Crew.Role>() {Crew.Role.Gunner, Crew.Role.Captain};
                default:
                    return new List<Crew.Role>();
            }
        }

        public List<Action> GetPossibleActionsForCurrentPhase()
        {
            return Action.AllActions.Where(action => action.phase == currentPhase).ToList();
        }

        public List<Action> getPossibleActionsForSelectedCrewpersonThisPhase()
        {
            return getPossibleActionsForCrewpersonAndPhase(_crewUI.getSelectedCrewmemberForShip(_shipsUI.GetSelectedShip()),
                this.GetCurrentPhase());
        }

        protected List<Action> getPossibleActionsForCrewpersonAndPhase(CrewMember crew, Phase phase)
        {
            return GetPossibleActionsForCurrentPhase().Where(action => action.phase == phase)
                .Where(action => action.requiredRole == crew.role).ToList();
        }

        public bool TryAdvancePhase()
        {
            bool mayAdvance = CanPhaseEnd();
            if (mayAdvance)
            {
                EndPhase();
            }

            return mayAdvance;
        }

        public bool CanPhaseEnd()
        {
            Ship[] allShips = Ship.getAllShips();
            return allShips.Length == shipsSignalingComplete.Count && new HashSet<Ship>(allShips).SetEquals(allShips);
        }

        protected void EndPhase()
        {
            NotifyEndPhase();
            DoEndPhase();
            this.shipsSignalingComplete.Clear();
            NotifyBeginPhase();
        }

        private void NotifyBeginPhase()
        {
            getCurrentController().OnPhaseBegin();
        }

        private void NotifyEndPhase()
        {
            getCurrentController().OnPhaseEnd();
        }

        private void DoEndPhase()
        {
            if (FindObjectOfType<RoundManager>().TryAdvanceRound(this))
            {
                this.currentPhase = 0;
            }
            else
            {
                this.currentPhase++;
            }
        }

        public void SignalComplete(Ship ship)
        {
            if (!DoesCurrentPhaseUseInitiative())
            {
                EndActionIfInProgress(ship);
                shipsSignalingComplete.Add(ship);
            }
            else if (ShipHasInitiative(ship))
            {
                EndActionIfInProgress(ship);
                _shipsYetToActInOrder.Dequeue();
                shipsSignalingComplete.Add(ship);
                selectNextShipOrNone();
            }
        }

        private void selectNextShipOrNone()
        {
            if (_shipsYetToActInOrder.Count > 0)
            {
                Ship next = _shipsYetToActInOrder.Peek();
                if (next.isArtificiallyIntelligentlyControlled &&
                    next.GetComponent<ArtificialIntelligencePlayer>() != null)
                {
                    next.GetComponent<ArtificialIntelligencePlayer>().YourTurn(this.currentPhase);
                }

                _shipsUI.TrySelectShip(next.gridPosition);
            }
            else
            {
                _shipsUI.DeselectShip();
            }
        }

        private void EndActionIfInProgress(Ship ship)
        {
            if (this.HasShipChosenAnyActionThisPhase(ship))
            {
                this.EndAllActionsInProgressForShip(ship);
            }
        }

        public void SignalStillWorking(Ship ship)
        {
            shipsSignalingComplete.Remove(ship);
        }

        public bool isShipDone(Ship ship)
        {
            return shipsSignalingComplete.Contains(ship);
        }

        public bool IsLastPhase()
        {
            return Convert.ToInt32(this.currentPhase).Equals(Enum.GetValues(typeof(Phase)).Length - 1);
        }

        public void SetShipInitiativeOrder(Queue<Ship> shipsByInitiativeAscending)
        {
            this._shipsYetToActInOrder = shipsByInitiativeAscending;
            selectNextShipOrNone();
        }

        public Ship GetShipWithInitiative()
        {
            return this._shipsYetToActInOrder.Count < 1 ? null : this._shipsYetToActInOrder.Peek();
        }

        public bool ShipHasInitiative(Ship ship)
        {
            return GetShipWithInitiative() == ship;
        }

        public bool DoesCurrentPhaseUseInitiative()
        {
            return DoesPhaseUseInitiative(currentPhase);
        }

        public bool DoesPhaseUseInitiative(Phase phase)
        {
            return phase == Phase.Helm || currentPhase == Phase.Gunnery;
        }

        public bool HasShipChosenAnyActionThisPhase(Ship ship)
        {
            return actionsThisPhase.ContainsValue(ship);
        }
        
        public bool HasShipChosenAnyActionThisPhaseNamed(Ship ship, string actionName)
        {
            return actionsThisPhase.Any(entry => { return entry.Value == ship && entry.Key.actionType.name == actionName; });
        }

        public void ToggleShipAction(Ship shipBeingActedOn, CrewMember crewpersonActing, string actionName)
        {
            CrewAction actionCandidate = new CrewAction(Action.findByName(actionName), crewpersonActing, shipBeingActedOn);
            if(actionCandidate.isValid() != "")
            {
                ErrorUI.Get().ShowError(actionCandidate.isValid());
            }
            else
            {
                if (HasShipChosenAnyActionThisPhaseNamed(shipBeingActedOn, actionName))
                {
                    Util.logIfDebugging("Phase manager canceling action " + actionName + " for ship " +
                                        shipBeingActedOn.displayName);
                    EndAllActionsInProgressForShip(shipBeingActedOn);
                }
                else
                {
                    Util.logIfDebugging("Phase manager starting action " + actionName + " for ship " +
                                        shipBeingActedOn.displayName);
                    CrewAction actionToAdd =
                        new CrewAction(Action.findByName(actionName), crewpersonActing, shipBeingActedOn);
                    actionsThisPhase.Add(actionToAdd, shipBeingActedOn);
                    getCurrentController().OnActionBegin(actionToAdd, shipBeingActedOn);
                }
            }
        }

        public void EndAllActionsInProgressForShip(Ship actor)
        {
            foreach (CrewAction action in getShipActionsThisPhase(actor))
            {
                getCurrentController().OnActionEnd(action, actor);
                actionsThisPhase.Remove(action);
            }
        }

        public List<CrewAction> getShipActionsThisPhase(Ship ship)
        {
            return this.actionsThisPhase.Where(tuple => tuple.Value == ship).Select(tuple => tuple.Key).ToList();
        }
        
        public List<CrewAction> getCrewpersonActionsThisPhase(CrewMember person)
        {
            return this.actionsThisPhase.Where(tuple => tuple.Key.actor == person).Select(tuple => tuple.Key).ToList();
        }

        public void ResetAction(Ship ship)
        {
            CrewAction mostRecent = getShipActionsThisPhase(ship).Last();
            Util.logIfDebugging("Phase manager canceling action " + mostRecent.actionType.name + " for ship " +
                                ship.displayName);
            getCurrentController().OnActionCancel(mostRecent, ship);
            actionsThisPhase.Remove(mostRecent);
        }

        private IPhaseController getCurrentController()
        {
            switch (currentPhase)
            {
                case Phase.Helm:
                    return _helmPhaseController;
                case Phase.Gunnery:
                    return _gunneryPhaseController;
                case Phase.Engineering:
                    return _engineeringPhaseController;
                default:
                    return null;
            }
        }
    }

    public enum Phase
    {
        Engineering = 0,
        Helm = 1,
        Gunnery = 2
    }

    public class Action
    {
        public static List<Action> AllActions = new List<Action>(){new Action("Shoot", Phase.Gunnery, Crew.Role.Gunner), new Action("Maneuver", Phase.Helm, Crew.Role.Pilot), new Action("Hold It Together", Phase.Engineering, Crew.Role.Engineer)};

        public static Action findByName(String name)
        {
            return AllActions.Find(action => action.name == name);
        } 
        public string name { get; }
        public Phase phase { get; }
        public Crew.Role requiredRole { get;}
        public bool isMinor { get; }

        public bool isPush { get; }

        public Action(String name, Phase phase, Crew.Role requiredRole)
        {
            this.name = name;
            this.phase = phase;
            this.requiredRole = requiredRole;
            this.isMinor = false;
            this.isPush = false;
        }
    }

    public class CrewAction
    {
        public Action actionType { get; }

        public CrewMember actor { get; }
        
        public Ship ship { get; }

        public Phase phase { get; }


        public CrewAction(Action action, CrewMember actingCrew, Ship actingShip, Phase phaseActedDuring = Phase.Helm)
        {
            this.actionType = action;
            this.actor = actingCrew;
            this.ship = actingShip;
            this.phase = phaseActedDuring;
        }

        public string isValid()
        {
            //return value of empty string is "true"
            //any other string is an error message
            if (actor == null || actor.role != this.actionType.requiredRole)
            {
                return (actionType.name + " requires a " + this.actionType.requiredRole);

            }

            return "";
        }
    }
}