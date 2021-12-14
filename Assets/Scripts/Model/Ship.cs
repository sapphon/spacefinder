using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
    public String displayName = _defaultNames[new System.Random().Next(_defaultNames.Length)];
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

public enum SystemCondition
{
    Perfect=0,Glitching=1,Malfunctioning=2,Wrecked=3
}