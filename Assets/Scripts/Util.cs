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
}