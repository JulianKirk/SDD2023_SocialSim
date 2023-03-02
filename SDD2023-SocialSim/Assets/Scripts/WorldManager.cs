using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldManager : MonoBehaviour
{
    private static readonly Lazy<WorldManager> _instance = new Lazy<WorldManager>(() => new WorldManager());

    public static WorldManager instance { get { return _instance.Value; } }

    private WorldManager(){}

    int TimeGoal; //This will be set in the "Create new simulation" in-game menu (in years)
    int TimePassed = 0; //In years

    float yrsPerSecond; //Set in the "Create new simulation" menu

    float timeSinceLastYear;

    public event Action OnNewYear; //Event that will be received by all AI to trigger aging processes

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastYear += Time.deltaTime;

        if (timeSinceLastYear > 1/yrsPerSecond) 
        {
            StartNewYear();
        }
    }

    public void StartNewYear() 
    {
        TimePassed ++;

        if (TimePassed > TimeGoal) 
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
