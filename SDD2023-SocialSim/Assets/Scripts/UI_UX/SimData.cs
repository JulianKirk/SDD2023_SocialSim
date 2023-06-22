using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimData
{
    public float numberOfHumans;

    public float intelligence;
    public float strength;

    public int mapSize;
    public int yearTarget;

    public SimData(float humanNum, int map, float intel, float str, int years) 
    {
        numberOfHumans = humanNum;
        mapSize = map;
        intelligence = intel;
        strength = str;
        yearTarget = years;
    }
}
