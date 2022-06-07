using System.Collections.Generic;
using UnityEngine;

namespace Model.Crew
{
    public class Crew : MonoBehaviour
    {
        public int totalCrew = 5;
        public int engineeringCrewRequired = 1;
        public int gunneryCrewRequired = 1;
        public int pilotingCrewRequired = 1;
        public int scientificCrewRequired = 1;
        public CrewMember[] officers;

        public enum Role
        {
            Captain=0,
            Engineer=1,
            Scientist=2,
            Pilot=3,
            Gunner=4
        }

        public List<CrewMember> getMembers()
        {
            return new List<CrewMember>(officers);
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
}