using System.Linq;
using AI;
using Model;
using UnityEngine;

namespace Controller.PhaseControllers
{
    public class EngineeringPhaseController : MonoBehaviour, IPhaseController
    {
        private PhaseManager phaseManager;

        void Awake()
        {
            this.phaseManager = FindObjectOfType<PhaseManager>();
        }

        public void OnPhaseBegin()
        {
            foreach (Ship ship in ArtificialIntelligencePlayer.getAllAIControlledShips())
            {
                phaseManager.ToggleShipAction(ship,
                    ship.gameObject.GetComponent<ArtificialIntelligencePlayer>().getEngineeringPhaseActions().First());
                phaseManager.SignalComplete(ship);
            }
        }

        public void OnPhaseEnd()
        {
        }

        public void OnActionBegin(CrewAction action, Ship ship)
        {
        }

        public void OnActionEnd(CrewAction action, Ship ship)
        {
        }

        public void OnActionCancel(CrewAction action, Ship ship)
        {
        }
    }
}