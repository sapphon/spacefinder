using UnityEngine;
using Random = System.Random;

public class DiceRoller
{
    private Random _random;

    public DiceRoller()
    {
        _random = new System.Random(Time.frameCount);
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