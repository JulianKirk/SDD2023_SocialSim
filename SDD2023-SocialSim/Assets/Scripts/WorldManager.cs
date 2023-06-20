using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class WorldManager : MonoBehaviour //GenericSingleton<WorldManager>
{
    // private static readonly Lazy<WorldManager> _instance = new Lazy<WorldManager>(() => new WorldManager());

    // public static WorldManager instance { get { return _instance.Value; } }

    public int TimeGoal = 1000; //This will be set in the "Create new simulation" in-game menu (in years)
    int YearsPassed = 0; //In years

    // public static float yrsPerSecond = 1/12; //Set in the "Create new simulation" menu
    public static float secondsPerYr = 360f; //Set in the "Create new simulation" menu

    float timeSinceLastYear = 0f;
    float timeSinceLastMonth = 0f;

    public static event Action OnNewYear; //Event that will be received by all AI to trigger aging processes
    public static event Action OnNewMonth;

    public HumanStateManager humanPrefab;
    public Transform HumanHolder;

    Vector2Int CivilizationCenter;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastYear += Time.deltaTime;
        timeSinceLastMonth += Time.deltaTime;

        if (timeSinceLastYear > secondsPerYr) 
        {
            StartNewYear();

            timeSinceLastYear = 0;
        }

        if (timeSinceLastMonth > secondsPerYr/12) 
        {
            OnNewMonth?.Invoke();

            timeSinceLastMonth = 0;
        }

        Debug.Log("Time Since Last Month: " + timeSinceLastMonth);

        if (Input.GetKeyDown(KeyCode.J)) 
        {
            SpawnHumans(10, 10, 10, 5, 5, 1, 15);
        }
    }

    public void StartNewYear() 
    {
        Debug.Log("Year: " + YearsPassed);

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

    void SpawnHumans(int numOfHumans, int xPos, int yPos, int iq, int str, float spd, int age) 
    {
        for (int i = 0; i < numOfHumans; i++)
        {
            int xDeviation = UnityEngine.Random.Range(-(numOfHumans)/2, 1 + (numOfHumans/2));
            int yDeviation = UnityEngine.Random.Range(-(numOfHumans/2), 1 + (numOfHumans/2));

            HumanStateManager newHuman = Instantiate<HumanStateManager>(humanPrefab, new Vector3(xPos + xDeviation, yPos + yDeviation, 0), Quaternion.identity, HumanHolder);

            newHuman.OnSpawn(new Vector2Int(xPos + xDeviation, yPos + yDeviation), iq, str, spd, age);
        }
    }
}
