using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
public class WorldManager //: GenericSingleton<WorldManager>
{
    // private static readonly Lazy<WorldManager> _instance = new Lazy<WorldManager>(() => new WorldManager());

    // public static WorldManager instance { get { return _instance.Value; } }

    //public WorldManager instance = gameObject.transform.parent.gameobject.GetComponent<WorldManager>();

    public int TimeGoal; //This will be set in the "Create new simulation" in-game menu (in years)
    int TimePassed = 0; //In years

    float yrsPerSecond; //Set in the "Create new simulation" menu

    float timeSinceLastYear;

    public event Action OnNewYear; //Event that will be received by all AI to trigger aging processes

    public Tilemap tileMap;
    public Tile[] tilePresets; //Set in the editor - Desert, Forest, Mountains, Plains, Water
    Dictionary<TileType, Tile> tileAssociations = new Dictionary<TileType, Tile>();

    public int MapWidth;
    public int MapLength;
    
    // private TileData[,] tileData;

    // Start is called before the first frame update
    void Awake()
    {
        tileAssociations.Add(TileType.Desert, tilePresets[0]);
        tileAssociations.Add(TileType.Forest, tilePresets[1]);
        tileAssociations.Add(TileType.Mountains, tilePresets[2]);
        tileAssociations.Add(TileType.Plains, tilePresets[3]);
        tileAssociations.Add(TileType.Water, tilePresets[4]);

        // tileData = WorldGeneration.instance.GenerateMap(20, 20, 1.05f, 21, 16);

        foreach (TileData tile in WorldGeneration.instance.GenerateMap(MapWidth, MapLength, 1.05f, 21, 16)) 
        {
            tileMap.SetTile(new Vector3Int(tile.xPos, tile.yPos, 0), tileAssociations[tile.finalTile]);

            Debug.Log("Spawning in a tile");
        }
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastYear += Time.deltaTime;

        if (timeSinceLastYear > 1/yrsPerSecond) 
        {
            StartNewYear();

            timeSinceLastYear = 0;
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
