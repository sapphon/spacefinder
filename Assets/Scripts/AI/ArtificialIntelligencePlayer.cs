using System.Linq;
using Model;
using UnityEngine;

namespace AI
{
    public class ArtificialIntelligencePlayer : MonoBehaviour
    {
        public static Ship[] getAllAIControlledShips()
        {
            return Ship.getAllShips().Where(ship => ship.isArtificiallyIntelligentlyControlled).ToArray();
        }

        public string[] getEngineeringPhaseActions()
        {
            return  new string[]{"Hold It Together"};
        }
    }
}