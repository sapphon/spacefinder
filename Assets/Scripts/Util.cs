using System;
using System.Collections.Generic;
using Model;
using UnityEngine;
using UnityEngine.EventSystems;

public class Util
{
    public static int DistanceBetween(Vector3Int origin, Vector3Int destination)
    {
        //this is incorrect
        return Math.Abs(origin.x - destination.x) + Math.Abs(origin.y - destination.y);
    }

    public static T chooseOneRandomlyFrom<T>(T[] choosingFrom, DiceRoller diceRoller)
    {
        return choosingFrom[diceRoller.randomIndex(choosingFrom)];
    }

    public static bool logIfDebugging(string toLog)
    {
        bool isDebugging = true;
        if (isDebugging)
        {
            Debug.Log(toLog);
        }
        return isDebugging;
    }

    public static float getAngleBetweenShips(Ship observer, Ship observed)
    {
        return Vector3.SignedAngle(observer.getForwardVectorInWorld(),
            observed.getWorldSpacePosition() - observer.getWorldSpacePosition(), Vector3.forward);
    }

    public static bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}