using System;
using System.Collections.Generic;
using AI;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Model
{
    public class Ship : MonoBehaviour
    {

        public bool isArtificiallyIntelligentlyControlled = false;
        private static string[] _defaultNames =
        {
            "Scipion", "Formidable", "Duguay Trouin", "Mont Blanc", "Héros", "Bucentaure", "Neptune", "Redoubtable", "Indomptable", "Fougueux", "Intrépide", "Pluton", "Aigle", "Algésiras", "Swiftsure" 
            , "Argonaute", "Achille", "Berwick"
        };
    
        public Vector3Int gridPosition = new Vector3Int(0,0,0);
        public Facing facing = Facing.N;
        public String displayName = Util.chooseOneRandomlyFrom(_defaultNames, new DiceRoller());
        public Affiliation affiliation = Affiliation.Player;
    
        //maneuvering
        public int speed = 6;
        public Maneuverability maneuverability = Maneuverability.Good;
    

        //defenses
        public int armorClass = 12;
        public int targetLock = 12;
        public int hitPoints = 100;
        public int hitPointsTotal = 100;
        public int damageThreshold = 0;

        //shields
        public int shieldCapacity = 60;
        public int shieldForeCurrent = 15;
        public int shieldAftCurrent = 15;
        public int shieldPortCurrent = 15;
        public int shieldStarboardCurrent = 15;
    
        //power core
        public int powerCoreCapacity = 130;
    
        //majel barrett's prison
        public int computerBonusMagnitude = 3;
        public int computerNodeCount = 2;
    
        //kind of meta
        public int tier = 1;
    
        //system status
        [Header("System Statuses")]
        public SystemCondition lifeSupport =  0;
        public SystemCondition sensors = 0; 
        public SystemCondition engines = 0;
        public SystemCondition powerCore = 0;
        public SystemCondition portWeapons = 0; 
        public SystemCondition starboardWeapons = 0;
        public SystemCondition foreWeapons = 0;
        public SystemCondition aftWeapons = 0;
    
        [Header("Crew")] 
        public List<CrewMember> crew;

        [Header("Weaponry")] 
        public List<Weapon> weapons;


        private void Start()
        {
            if (isArtificiallyIntelligentlyControlled)
            {
                this.gameObject.AddComponent<ArtificialIntelligencePlayer>();
            }
        }

        public static Ship[] getAllShips()
        {
            return GameObject.Find("Ship Models").GetComponentsInChildren<Ship>();
        }

        public SystemCondition[] getAllWeaponsSystemConditions()
        {
            return new[]{foreWeapons, aftWeapons, starboardWeapons, portWeapons};
        }

        public Vector3 getWorldSpacePosition()
        {
            return GameObject.Find("Tilemap").GetComponent<Tilemap>().GetCellCenterWorld(this.gridPosition);
        }

        public void TurnToStarboard()
        {

            facing = (Facing) (((int) facing + 300) % 360);
        }
    
        public void TurnToPort()
        {
            facing = (Facing) (((int) facing + 60) % 360);
        }

        public void Advance()
        {
            if (facing == Facing.N)
            {
                gridPosition = new Vector3Int(gridPosition.x + 1, gridPosition.y, gridPosition.z);
            }
            else if (facing == Facing.NW)
            {
                gridPosition = new Vector3Int(gridPosition.x + (gridPosition.y % 2 == 0 ? 0 : 1), gridPosition.y - 1, gridPosition.z);
            }
            else if (facing == Facing.SW)
            {
                gridPosition = new Vector3Int(gridPosition.x - (gridPosition.y % 2 == 0 ? 1 : 0), gridPosition.y - 1, gridPosition.z);
            }
            else if (facing == Facing.S)
            {
                gridPosition = new Vector3Int(gridPosition.x - 1, gridPosition.y, gridPosition.z);
            }
            else if (facing == Facing.SE)
            {
                gridPosition = new Vector3Int(gridPosition.x - (gridPosition.y % 2 == 0 ? 1 : 0), gridPosition.y + 1, gridPosition.z);
            }
            else if (facing == Facing.NE)
            {
                gridPosition = new Vector3Int(gridPosition.x + (gridPosition.y % 2 == 0 ? 0 : 1), gridPosition.y + 1, gridPosition.z);
            }
        }

        public Vector3 getForwardVectorInWorld()
        {
            if (facing == Facing.N)
            {
                return Vector3.up;
            }
            else if (facing == Facing.NW)
            {
                return new Vector3(-Mathf.Sqrt(3)/2f, 0.5f);
            }
            else if (facing == Facing.SW)
            {
                return new Vector3(-Mathf.Sqrt(3)/2f, -0.5f);
            }
            else if (facing == Facing.S)
            {
                return Vector3.down;
            }
            else if (facing == Facing.SE)
            {
                return new Vector3(Mathf.Sqrt(3)/2f, -0.5f);
            }
            else if (facing == Facing.NE)
            {
                return new Vector3(Mathf.Sqrt(3)/2f, 0.5f);
            }

            return Vector3.up;
        }

        public bool isWeaponsSystemWrecked()
        {
            return this.aftWeapons == SystemCondition.Wrecked &&
                   this.foreWeapons == SystemCondition.Wrecked &&
                   this.starboardWeapons == SystemCondition.Wrecked &&
                   this.portWeapons == SystemCondition.Wrecked;
        }
    }

    public enum Affiliation
    {
        Player,Gm
    }

    public enum Maneuverability
    {
        Perfect=0, Good=1, Average=2, Poor=3, Clumsy=4
    }

    public enum Facing
    {
        N=0,NE=300,SE=240,S=180,SW=120,NW=60
    }

    public enum Range
    {
        Short=5, Medium=10, Long=20
    }

    public enum SystemCondition
    {
        Perfect=0,Glitching=1,Malfunctioning=2,Wrecked=3
    }

    public enum Die
    {
        D2=2,D3=3,D4=4,D6=6,D8=8,D10=10,D12=12,D20=20,D100=100
    }

    public enum WeaponFiringArc
    {
        Fore=0,Aft=1,Starboard=2,Port=3,Turret=4
    }
}