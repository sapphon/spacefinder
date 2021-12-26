using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Model;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Random = UnityEngine.Random;

public class Ship : MonoBehaviour
{

    private static string[] _defaultNames =
    {
        "Scipion", "Formidable", "Duguay Trouin", "Mont Blanc", "Héros", "Bucentaure", "Neptune", "Redoubtable", "Indomptable", "Fougueux", "Intrépide", "Pluton", "Aigle", "Algésiras", "Swiftsure" 
        , "Argonaute", "Achille", "Berwick"
    };
    
    public Vector3Int gridPosition = new Vector3Int(0,0,0);
    public Facing facing = Facing.N;
    public String displayName = Util.chooseRandom(_defaultNames);
    public Affiliation affiliation = Affiliation.Player;
    
    //maneuvering
    public int speed = 6;
    public Maneuverability maneuverability = Maneuverability.Good;
    

    //defenses
    public int armorClass = 12;
    public int targetLock = 12;
    public int hitPoints = 100;
    public int damageThreshold = 0;

    //fake-ass made up defenses
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

    [Header("Weapon1")] 
    public string weapon1Name;
    public WeaponFiringArc weapon1Arc;
    public Die weapon1DamageDieType;
    public int weapon1DamageDieCount;
    public Range weapon1Range;
    public int weapon1Speed;
    
    [Header("Weapon2")] 
    public string weapon2Name;
    public WeaponFiringArc weapon2Arc;
    public Die weapon2DamageDieType;
    public int weapon2DamageDieCount;
    public Range weapon2Range;
    public int weapon2Speed;
    
    [Header("Weapon3")] 
    public string weapon3Name;
    public WeaponFiringArc weapon3Arc;
    public Die weapon3DamageDieType;
    public int weapon3DamageDieCount;
    public Range weapon3Range;
    public int weapon3Speed;
    
    [Header("Weapon4")] 
    public string weapon4Name;
    public WeaponFiringArc weapon4Arc;
    public Die weapon4DamageDieType;
    public int weapon4DamageDieCount;
    public Range weapon4Range;
    public int weapon4Speed;

    public static Ship[] getAllShips()
    {
        return GameObject.Find("Ship Models").GetComponentsInChildren<Ship>();
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
    Fore,Aft,Starboard,Port,Turret
}