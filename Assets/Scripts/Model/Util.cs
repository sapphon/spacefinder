using System;
using UnityEngine;

namespace Model
{
    public class Util
    {
        public static int DistanceBetween(Vector3Int origin, Vector3Int destination)
        {
            return Math.Abs(origin.x - destination.x) + Math.Abs(origin.y - destination.y);
        }
    }
}