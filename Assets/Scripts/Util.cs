using System;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    public static int DistanceBetween(Vector3Int origin, Vector3Int destination)
    {
        //this is incorrect
        return Math.Abs(origin.x - destination.x) + Math.Abs(origin.y - destination.y);
    }

    public static T chooseRandom<T>(T[] choosingFrom)
    {
        return choosingFrom[new System.Random().Next(choosingFrom.Length)];
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
}