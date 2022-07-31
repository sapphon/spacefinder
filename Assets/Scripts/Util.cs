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
        return getAngleBetween(observer.gridPosition, observer.facing, observed.gridPosition);
    }

    public static float getAngleBetween(Vector3Int origin, Facing facing, Vector3Int destination)
    {
        return Vector3.SignedAngle(getVectorFromFacing(facing),
            destination - origin, Vector3.forward);
    }

    public static Vector3 getVectorFromFacing(Facing facing)
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

    public static bool IsMouseOverUI()
    {
        bool isPointerOverGameObject = EventSystem.current.IsPointerOverGameObject();
        logIfDebugging("Click " + (isPointerOverGameObject ? "is" : "is not") + " over the UI.");
        return isPointerOverGameObject;
    }
}