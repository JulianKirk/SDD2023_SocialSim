using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Newtonsoft.Json;
using TMPro;

public class WorldManager : MonoBehaviour //GenericSingleton<WorldManager>
{
    // private static readonly Lazy<WorldManager> _instance = new Lazy<WorldManager>(() => new WorldManager());

    // public static WorldManager instance { get { return _instance.Value; } }

    public int TimeGoal; //This will be set in the "Create new simulation" in-game menu (in years)
    public static int YearsPassed = 0; //In years
    public TMP_Text YearsPassedText;
    public TMP_Text TimeGoalText;

    public static float secondsPerYr = 360f; //Each month is 30 seconds

    float timeSinceLastYear = 0f;
    float timeSinceLastMonth = 0f;

    public static event Action OnNewYear; //Event that will be received by all AI to trigger aging processes
    public static event Action OnNewMonth;

    public HumanStateManager HumanPrefab;
    public static Transform HumanHolder;

    public static bool[,] landOwned; //Indicates a house or other building there, set in the MapGenerator class

    public static List<HumanStateManager> HumanCollective = new List<HumanStateManager>();
    public static List<Vector2Int> HousePositions = new List<Vector2Int>();
    
    public MapGenerator mapGenerator; //Assigned in editor

    public static ResultsData RESULTS;

    // Start is called before the first frame update
    void Awake()
    {
        RESULTS = new ResultsData();
        
        RESULTS.deathsByCauses.Add("Starvation", 0);
        RESULTS.deathsByCauses.Add("Childbirth", 0);
        RESULTS.deathsByCauses.Add("Disease", 0);
        RESULTS.populationByYear.Add(YearsPassed, 0);
        RESULTS.housesBuiltByYear.Add(YearsPassed, 0);
        RESULTS.housesByYear.Add(YearsPassed, 0);
        RESULTS.deathsByYear.Add(YearsPassed, 0);
        RESULTS.birthsByYear.Add(YearsPassed, 0);

        YearsPassed = 0; //Always start at zero

        RetrieveOptions();
    }
    
    void Start()
    {
        HumanHolder = GameObject.Find("HumanHolder").transform;

        YearsPassedText.text = "0";

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
            Debug.Log("NEW YEAR");
            StartNewYear();

            timeSinceLastYear = 0;
        }

        if (timeSinceLastMonth > secondsPerYr/12) 
        {
            OnNewMonth?.Invoke();

            if (HumanHolder.childCount < 1) 
            {
                FinishSimulation();
            }

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

        RESULTS.housesByYear[YearsPassed] += HousePositions.Count;
        RESULTS.populationByYear[YearsPassed] += HumanCollective.Count;

        YearsPassed ++;

        YearsPassedText.text = YearsPassed.ToString();

        if (YearsPassed >= TimeGoal) 
        {
            FinishSimulation();
            return;
        }

        RESULTS.populationByYear.Add(YearsPassed, 0);
        RESULTS.housesBuiltByYear.Add(YearsPassed, 0);
        RESULTS.housesByYear.Add(YearsPassed, 0);
        RESULTS.deathsByYear.Add(YearsPassed, 0);
        RESULTS.birthsByYear.Add(YearsPassed, 0);

        OnNewYear?.Invoke();
    }

    public void FinishSimulation() //Transition to the next Results menu scene
    {
        RESULTS.totalYears = YearsPassed;

        string savePath = Application.persistentDataPath + "/simResults.json";

        string jsonText = JsonConvert.SerializeObject(RESULTS);

        File.WriteAllText(savePath, jsonText);

        SceneManager.LoadScene(2);
    }

    public static void SpawnHumans(HumanStateManager humanPrefab, GameObject homeObject, int numOfHumans, int xPos, int yPos, int locationVariance, int iq, int str, int age) 
    {

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

            // HousePositions.Add(new Vector2Int(xSpawnPos, ySpawnPos));
        }
    }

    public static void SpawnHuman(HumanStateManager humanPrefab, GameObject homeObject, int xPos, int yPos, int iq, int str, int age)
    {
        HumanStateManager newHuman = Instantiate<HumanStateManager>(humanPrefab, new Vector3(xPos, yPos, 0), Quaternion.identity, HumanHolder);

        HumanCollective.Add(newHuman);

        newHuman.OnSpawn(new Vector2Int(xPos, yPos), homeObject, iq, str, age);

        RESULTS.totalPopulation += 1;

        HousePositions.Add(new Vector2Int(xPos, yPos));
    }

    void StartSimulation(SimData data) 
    {
        //Set the time target and display it on the top bar
        TimeGoal = data.yearTarget;
        TimeGoalText.text = TimeGoal.ToString();

        //Randomly generate seeds for heat and heigh maps for world generation
        int heatSeed = UnityEngine.Random.Range(0, 10000);
        int heightSeed = UnityEngine.Random.Range(0, 10000);

        mapGenerator.GenerateMap(data.mapSize, data.mapSize, heatSeed, heightSeed, 1, 1, 1);

        int xSpawnPos = UnityEngine.Random.Range(0, data.mapSize);
        int ySpawnPos =  UnityEngine.Random.Range(0, data.mapSize);

        while (!MapGenerator.walkableGrid[xSpawnPos, ySpawnPos]) //Ensures that xSpawnPos and ySpawnPos always end up somewhere walkable
        {
            xSpawnPos = UnityEngine.Random.Range(0, data.mapSize);
            ySpawnPos =  UnityEngine.Random.Range(0, data.mapSize);
        }

        SpawnHumans(HumanPrefab, null, (int)data.numberOfHumans, xSpawnPos, ySpawnPos, 15, (int)data.intelligence, (int)data.strength, 20);

        //Start the simulation with the camera focused on the humans
        Camera.main.orthographicSize = 5;
        Camera.main.transform.position = new Vector3(xSpawnPos, ySpawnPos, Camera.main.transform.position.z);
        CamController.MapSize = data.mapSize;
    }

    void RetrieveOptions() 
    {
        string path = Application.persistentDataPath + "/options.json";

        string jsonText = File.ReadAllText(path);

        OptionsData optionsData = JsonConvert.DeserializeObject<OptionsData>(jsonText);

        CamController.CameraPanSensitivity = optionsData.panSensitivity/4; //Too high otherwise
    }
}
