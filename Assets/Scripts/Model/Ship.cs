﻿using System;
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
    public int rotation;
    public String displayName = _defaultNames[new System.Random().Next(_defaultNames.Length)];
    public Affiliation affiliation = Affiliation.Player;
    
    //maneuvering
    public int speed = 6;
    public ManeuverabilityClass maneuverability = ManeuverabilityClass.Good;
    

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

    public static Ship[] getAllShips()
    {
        return GameObject.Find("Ship Models").GetComponentsInChildren<Ship>();
    }

}

public enum Affiliation
{
    Player,Gm
}

public enum ManeuverabilityClass
{
    Perfect=0, Good=1, Average=2, Poor=3, Clumsy=4
}