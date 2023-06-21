using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Newtonsoft.Json;

public class WorldManager : MonoBehaviour //GenericSingleton<WorldManager>
{
    // private static readonly Lazy<WorldManager> _instance = new Lazy<WorldManager>(() => new WorldManager());

    // public static WorldManager instance { get { return _instance.Value; } }

    public int TimeGoal = 100000; //This will be set in the "Create new simulation" in-game menu (in years)
    int YearsPassed = 0; //In years

    // public static float yrsPerSecond = 1/12; //Set in the "Create new simulation" menu
    public static float secondsPerYr = 360f; //Set in the "Create new simulation" menu

    float timeSinceLastYear = 0f;
    float timeSinceLastMonth = 0f;

    public static event Action OnNewYear; //Event that will be received by all AI to trigger aging processes
    public static event Action OnNewMonth;

    public HumanStateManager HumanPrefab;
    public static Transform HumanHolder;

    public static bool[,] landOwned; //Indicates a house or other building there, set in the MapGenerator class

    public static List<HumanStateManager> HumanCollective = new List<HumanStateManager>();
    public static List<Vector2Int> HousePositions = new List<Vector2Int>();

    public static int yearlyDeathCount;
    Dictionary<int, int> deathCountsByYear = new Dictionary<int, int>();
    
    public MapGenerator mapGenerator; //Assigned in editor

    // public static Vector2Int CivilizationCenter;

    // Start is called before the first frame update
    
    void Start()
    {
        HumanHolder = GameObject.Find("HumanHolder").transform;

        yearlyDeathCount = 0;

        //Fetch simulation data created in the "Create new simulation" menu
        string savePath = Application.persistentDataPath + "/simData.json";
        string jsonText = File.ReadAllText(savePath);

        SimData simData = JsonConvert.DeserializeObject<SimData>(jsonText);

        StartSimulation(simData);
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

        // if (Input.GetKeyDown(KeyCode.J)) 
        // {
        //     Debug.Log("Attempting to spawn humans.");

        //     Vector3 position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));

        //     SpawnHumans(HumanPrefab, null, 10, (int)position.x, (int)position.y, 15, 5, 5, 20);
        // }
    }

    public void StartNewYear() 
    {
        deathCountsByYear[YearsPassed] = yearlyDeathCount;

        yearlyDeathCount = 0;

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

    public static void SpawnHumans(HumanStateManager humanPrefab, GameObject homeObject, int numOfHumans, int xPos, int yPos, int locationVariance, int iq, int str, int age) 
    {
        // CivilizationCenter = new Vector2Int(xPos, yPos);

        for (int i = 0; i < numOfHumans; i++)
        {
            int xDeviation = UnityEngine.Random.Range(-(locationVariance)/2, 1 + (locationVariance/2));
            int yDeviation = UnityEngine.Random.Range(-(locationVariance/2), 1 + (locationVariance/2));

            int xSpawnPos = Mathf.Clamp(xPos + xDeviation, 0, MapGenerator.walkableGrid.GetLength(0) - 1);
            int ySpawnPos = Mathf.Clamp(yPos + yDeviation, 0, MapGenerator.walkableGrid.GetLength(1) - 1);

            while (!landOwned[xSpawnPos, ySpawnPos] 
                && !MapGenerator.walkableGrid[xSpawnPos, ySpawnPos] 
                && !HousePositions.Contains(new Vector2Int(xSpawnPos, ySpawnPos)))
            {
                xDeviation = UnityEngine.Random.Range(-(locationVariance)/2, 1 + (locationVariance/2));
                yDeviation = UnityEngine.Random.Range(-(locationVariance/2), 1 + (locationVariance/2));

                xSpawnPos = Mathf.Clamp(xPos + xDeviation, 0, MapGenerator.walkableGrid.GetLength(0) - 1);
                ySpawnPos = Mathf.Clamp(yPos + yDeviation, 0, MapGenerator.walkableGrid.GetLength(1) - 1);
            }

            SpawnHuman(humanPrefab, homeObject, xSpawnPos, ySpawnPos, iq, str, age);

            HousePositions.Add(new Vector2Int(xSpawnPos, ySpawnPos));
        }
    }

    public static void SpawnHuman(HumanStateManager humanPrefab, GameObject homeObject, int xPos, int yPos, int iq, int str, int age)
    {
        HumanStateManager newHuman = Instantiate<HumanStateManager>(humanPrefab, new Vector3(xPos, yPos, 0), Quaternion.identity, HumanHolder);

        HumanCollective.Add(newHuman);

        newHuman.OnSpawn(new Vector2Int(xPos, yPos), homeObject, iq, str, age);
    }

    void StartSimulation(SimData data) 
    {
        int heatSeed = UnityEngine.Random.Range(0, 10000);
        int heightSeed = UnityEngine.Random.Range(0, 10000);

        mapGenerator.GenerateMap(data.mapSize, data.mapSize, heatSeed, heightSeed, 1, 1, 1);

        int xSpawnPos = UnityEngine.Random.Range(0, data.mapSize);
        int ySpawnPos =  UnityEngine.Random.Range(0, data.mapSize);

        // while ()

        // SpawnHumans(HumanPrefab, null, 10, (int)position.x, (int)position.y, 15, simData.intelligence, simData.strength, 20);
    }
}
