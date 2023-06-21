using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimData
{
    public float numberOfHumans;

    public float intelligence;
    public float strength;

    public int mapSize;

    public SimData(float humanNum, int map, float intel, float str) 
    {
        numberOfHumans = humanNum;
        mapSize = map;
        intelligence = intel;
        strength = str;
    }
}
