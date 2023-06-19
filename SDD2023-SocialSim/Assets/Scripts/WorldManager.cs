using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
public class WorldManager : GenericSingleton<WorldManager>
{
    // private static readonly Lazy<WorldManager> _instance = new Lazy<WorldManager>(() => new WorldManager());

    // public static WorldManager instance { get { return _instance.Value; } }

    public int TimeGoal; //This will be set in the "Create new simulation" in-game menu (in years)
    int YearsPassed = 0; //In years

    float yrsPerSecond; //Set in the "Create new simulation" menu

    float timeSinceLastYear;
    float timeSinceLastMonth;

    public event Action OnNewYear; //Event that will be received by all AI to trigger aging processes
    public event Action OnNewMonth;

    // Start is called before the first frame update
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastYear += Time.deltaTime;
        timeSinceLastMonth += Time.deltaTime;

        if (timeSinceLastYear > 1/yrsPerSecond) 
        {
            StartNewYear();

            timeSinceLastYear = 0;
        }

        if (timeSinceLastMonth > (1/yrsPerSecond)/12) 
        {
            OnNewMonth?.Invoke();

            timeSinceLastMonth = 0;
        }
    }

    public void StartNewYear() 
    {

        YearsPassed ++;

        if (YearsPassed > TimeGoal) 
        {
            FinishSimulation();
            return;
        }

        OnNewYear?.Invoke();
    }

    void FinishSimulation() //Transition to the next Results menu scene
    {
        //Make sure that the results are saved - either write to a JSON file or use a persistant singleton GameObject

        //Change Scene
    }
}
