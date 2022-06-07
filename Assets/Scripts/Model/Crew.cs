using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    public class Crew : MonoBehaviour
    {
        public int totalCrew = 5;
        public int engineeringCrewRequired = 1;
        public int gunneryCrewRequired = 1;
        public int pilotingCrewRequired = 1;
        public int scientificCrewRequired = 1;
        public Member[] officers;
        public class Member : IGunner, IEngineer, IScientist, IPilot, ICaptain
        {
            public int gunneryBonus = 0;
            public int engineeringBonus = 0;
            public int computersBonus = 0;
            public int pilotingBonus = 0;
            public int diplomacyBonus = 0;
            public int intimidationBonus = 0;
            public int getGunneryBonus()
            {
                return gunneryBonus;
            }

            public int getEngineeringBonus()
            {
                return engineeringBonus;
            }

            public int getComputersBonus()
            {
                return computersBonus;
            }

            public int getPilotingBonus()
            {
                return pilotingBonus;
            }

            public int getDiplomacyBonus()
            {
                return diplomacyBonus;
            }

            public int getIntimidationBonus()
            {
                return intimidationBonus;
            }
        }

        public interface IGunner
        {
            int getGunneryBonus();
        }
        
        public interface IEngineer
        {
            int getEngineeringBonus();
        }
        public interface IScientist
        {
            int getComputersBonus();
        }
        public interface IPilot
        {
            int getPilotingBonus();
        }

        public interface ICaptain : IPilot, IEngineer, IGunner
        {
            int getDiplomacyBonus();
            int getIntimidationBonus();
        }

        public enum Role
        {
            Captain=0,
            Engineer=1,
            Scientist=2,
            Pilot=3,
            Gunner=4
        }
    }
}