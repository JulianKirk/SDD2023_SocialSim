using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimData
{
    public float numberOfHumans;
    public float intelligence;
    public float strength;

    public SimData(float humanNum, float intel, float str) 
    {
        numberOfHumans = humanNum;
        intelligence = intel;
        strength = str;
    }
}
