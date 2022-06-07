using System;

namespace Model.Crew
{
    [Serializable]
    public class CrewMember : ICaptain, IGunner, IPilot, IScientist, IEngineer
    {
        
        private static string[] _defaultNames =
        {
            "Charles", "Claude", "Pierre", "Guillaume-Jean-Noël", "Jean-Baptiste-Joseph-René", "Pierre-Charles", "Esprit-Tranquille", "Jean Jacques", "Jean", "Louis", "Louis-Antoine-Cyprien", "Julien", "Pierre-Paulin", "Charles-René", "Charles-Eusèbe", "Jacques", "Louis-Gabriel", "Jean-Gilles", "Michel"
        };
        
        public Crew.Role role = Crew.Role.Gunner;
        public string name = Util.chooseOneRandomlyFrom(_defaultNames, new DiceRoller());
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
}