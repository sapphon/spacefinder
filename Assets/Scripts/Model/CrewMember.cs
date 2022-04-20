using System;

namespace Model
{
    [Serializable]
    public class CrewMember
    {
        
        private static string[] _defaultNames =
        {
            "Charles", "Claude", "Pierre", "Guillaume-Jean-Noël", "Jean-Baptiste-Joseph-René", "Pierre-Charles", "Esprit-Tranquille", "Jean Jacques", "Jean", "Louis", "Louis-Antoine-Cyprien", "Julien", "Pierre-Paulin", "Charles-René", "Charles-Eusèbe", "Jacques", "Louis-Gabriel", "Jean-Gilles", "Michel"
        };
        
        public Crew.Role role = Crew.Role.Gunner;
        public string name = Util.chooseRandom(_defaultNames);
        public int gunneryBonus = 0;
    }
}