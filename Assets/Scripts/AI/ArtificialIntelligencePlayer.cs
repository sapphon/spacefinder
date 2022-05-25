using System.Linq;
using Model;
using UnityEngine;

namespace AI
{
    public class ArtificialIntelligencePlayer : MonoBehaviour
    {
        private PhaseManager phaseManager;
        private Ship controlledShip;

        void Awake()
        {
            this.phaseManager = FindObjectOfType<PhaseManager>();
            this.controlledShip = GetComponent<Ship>();
        }

        public static Ship[] getAllAIControlledShips()
        {
            return Ship.getAllShips().Where(ship => ship.isArtificiallyIntelligentlyControlled).ToArray();
        }

        private string[] getEngineeringPhaseActions()
        {
            return  new string[]{"Hold It Together"};
        }

        public void YourTurn(Phase phase)
        {
            if (phase == Phase.Engineering)
            {
                phaseManager.ToggleShipAction(controlledShip,
                    this.getEngineeringPhaseActions().First());
                phaseManager.SignalComplete(controlledShip);
            }
        }
    }
}