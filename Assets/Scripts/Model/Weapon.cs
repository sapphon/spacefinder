using System;

namespace Model
{
    [Serializable]
    public class Weapon
    {
        public string name;
        public WeaponFiringArc arc;
        public Die damageDieType;
        public int damageDieCount;
        public Range range;
        public int speed;
    }
}