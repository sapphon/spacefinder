using System;
using System.Collections;
using Model;
using UnityEngine;

public class DiceRoller
{
    private System.Random _random;

    public DiceRoller()
    {
        _random = new System.Random();
    }

    public int randomIndex(IList collection)
    {
        return _random.Next(collection.Count);
    }

    public int rollAndTotal(int dice, Die dieType)
    {
        int sum = 0;
        for(int i = 0; i < dice; i++)
        {
            sum += _random.Next((int)dieType) + 1;
        }
        return sum;
    }
}