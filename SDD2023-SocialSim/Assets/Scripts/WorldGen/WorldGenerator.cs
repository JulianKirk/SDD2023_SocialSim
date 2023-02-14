using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGenerator : MonoBehaviour
{
    private int m_mapWidth, m_mapLength; //For the pathfinding to collect later

    enum TileType:int 
    {
    Desert = 0, 
    Forest = 1, 
    Ice = 2, 
    Mountains = 3, 
    Plains = 4, 
    Water = 5
    };

    TileType[] tiles = {
        TileType.Desert, 
        TileType.Forest, 
        TileType.Ice, 
        TileType.Mountains, 
        TileType.Plains, 
        TileType.Water
    };

    Dictionary<TileType, TileType[]> tileNeighbours = new Dictionary<TileType, TileType[]>()
    {
        {TileType.Desert, new TileType[] {TileType.Mountains, TileType.Plains, TileType.Desert}},
        {TileType.Forest, new TileType[] {TileType.Mountains, TileType.Plains, TileType.Forest, TileType.Water}},
        {TileType.Ice, new TileType[] {TileType.Mountains, TileType.Plains, TileType.Water, TileType.Ice}},
        {TileType.Mountains, new TileType[] {TileType.Mountains, TileType.Ice, TileType.Forest, TileType.Desert, TileType.Water}},
        {TileType.Plains, new TileType[] {TileType.Ice, TileType.Plains, TileType.Forest, TileType.Desert, TileType.Water}},
        {TileType.Water, new TileType[] {TileType.Ice, TileType.Plains, TileType.Forest, TileType.Mountains, TileType.Water}}
    };

    Dictionary<(int, int), List<TileType>> tileSuperPositions;

    public AstarTile[] tileRefs; //Assign in editor - have indexes match with TileType

    public Tilemap tilemap;
    
    // Start is called before the first frame update
    void Start()
    {
        GenerateMap(100, 100, 11f, 1233, 5312, 3213, 0.03f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateMap(int mapWidth, int mapLength, float multiplier, int heightSeed, int moistSeed, int heatSeed, float frequency) 
    {
        m_mapWidth = mapWidth;
        m_mapLength = mapLength;

        FastNoiseLite noise = new FastNoiseLite();
        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        noise.SetFrequency(frequency);

        // float[,] heightMap = new float[mapWidth, mapLength];
        // float[,] moistureMap = new float[mapWidth, mapLength];
        // float[,] heatMap = new float[mapWidth, mapLength];

        tileSuperPositions = new Dictionary<(int, int), List<TileType>>(); 

        for (int x = 0; x < mapWidth; x++) 
        {
            for (int y = 0; y < mapLength; y++) 
            {
                // noise.SetSeed(heightSeed);
                // heightMap[x, y] = noise.GetNoise(((float)x / (float)mapWidth * multiplier), ((float)y / (float)mapLength * multiplier));
                // Debug.Log(heightMap[x, y]);

                // noise.SetSeed(moistSeed);
                // moistureMap[x, y] = noise.GetNoise(((float)x / (float)mapWidth * multiplier), ((float)y / (float)mapLength * multiplier));
                // Debug.Log(moistureMap[x, y]);

                // noise.SetSeed(heatSeed);
                // heatMap[x, y] = noise.GetNoise(((float)x / (float)mapWidth * multiplier), ((float)y / (float)mapLength * multiplier));
                // Debug.Log(heatMap[x, y]);
                
                tileSuperPositions.Add((x, y), new List<TileType>(tiles)); //Add all the possible tile spots - entropy maximum to start with
            }
        } 

        /*

            Remove options based on temperature, moisture and height

        */

        for (int i = 0; i < mapWidth * mapLength; i++)
        {
            int minEntropy = tiles.Length + 1; //Add 1 so that it always selects one in the for loop 
            (int, int) positionToCollapse = (1, 1); //Default value: In case all entropy turns out equal (first iteration)
            
            foreach (KeyValuePair<(int, int), List<TileType>> Position in tileSuperPositions) 
            {
                if (Position.Value.Count < minEntropy && Position.Value.Count > 1) //Doesn't have to be <= as it can just collapse the first instance of the smallest entropy it finds
                {
                    minEntropy = Position.Value.Count;
                    positionToCollapse = Position.Key;
                }
            }

            CollapseTile(positionToCollapse, true);
        }
    }

    void CollapseTile((int, int) positionToCollapse, bool fullyCollapse)
    {
        List<TileType> superPositions = tileSuperPositions[positionToCollapse];
        TileType selectedTile = superPositions[Random.Range(0, superPositions.Count - 1)];

        List<TileType> tilesToRemove = new List<TileType>();

        if (fullyCollapse)
        {
            tilemap.SetTile(
                new Vector3Int(positionToCollapse.Item1, positionToCollapse.Item2, 0), 
                tileRefs[(int)selectedTile]
            );

            tileSuperPositions[positionToCollapse].Clear();
            tileSuperPositions[positionToCollapse].Add(selectedTile);
        } 
            
        foreach(TileType pTile in superPositions) //Set Value to propagate changes
        {
            foreach (TileType tile in tiles) 
            {
                if(!System.Array.Exists(tileNeighbours[pTile], neighbourTile => neighbourTile == tile))
                {
                    if(!tilesToRemove.Contains(tile)) 
                    {
                        tilesToRemove.Add(tile);
                    }
                }
            }
        }

        for(int x = -1; x <= 1; x++) 
        {
            for(int y = -1; y <=1; y++) 
            {
                bool changed = false;
                (int, int) newPos = (Mathf.Clamp(positionToCollapse.Item1 + x, 0, m_mapWidth - 1), Mathf.Clamp(positionToCollapse.Item2 + y, 0, m_mapLength - 1));

                foreach (TileType tile in tilesToRemove) 
                {
                    tileSuperPositions[newPos].Remove(tile);
                    changed = true;
                }

                if(changed)
                {
                    // CollapseTile(newPos, false);
                }
            }
        }
    }
}
