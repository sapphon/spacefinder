using System.Linq;
using Controller.PhaseControllers;
using Model;
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
            phaseManager.SignalComplete(controlledShip);
        }
    }
}