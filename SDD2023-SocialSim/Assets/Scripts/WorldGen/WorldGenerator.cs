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
    Mountains = 2, 
    Plains = 3, 
    Water = 4
    };

    TileType[] tiles = {
        TileType.Desert, 
        TileType.Forest, 
        TileType.Mountains, 
        TileType.Plains, 
        TileType.Water
    };

    Dictionary<TileType, TileType[]> tileNeighbours = new Dictionary<TileType, TileType[]>()
    {
        // {TileType.Desert, new TileType[] {TileType.Mountains, TileType.Plains, TileType.Desert}},
        // {TileType.Forest, new TileType[] {TileType.Mountains, TileType.Plains, TileType.Forest, TileType.Water}},
        // {TileType.Mountains, new TileType[] {TileType.Mountains, TileType.Forest, TileType.Desert}},
        // {TileType.Plains, new TileType[] {TileType.Plains, TileType.Forest, TileType.Desert, TileType.Water}},
        // {TileType.Water, new TileType[] {TileType.Plains, TileType.Forest, TileType.Water}}

        {TileType.Forest, new TileType[] {TileType.Mountains, TileType.Plains, TileType.Forest, TileType.Water, TileType.Desert}},
        {TileType.Desert, new TileType[] {TileType.Mountains, TileType.Plains, TileType.Forest, TileType.Water, TileType.Desert}},
        {TileType.Mountains, new TileType[] {TileType.Mountains, TileType.Plains, TileType.Forest, TileType.Water, TileType.Desert}},
        {TileType.Plains, new TileType[] {TileType.Mountains, TileType.Plains, TileType.Forest, TileType.Water, TileType.Desert}},
        {TileType.Water, new TileType[] {TileType.Mountains, TileType.Plains, TileType.Forest, TileType.Water, TileType.Desert}},
    };

    // Dictionary<TileType, TileType[]> tileNeighbours = new Dictionary<TileType, TileType[]>();

    Dictionary<(int, int), HashSet<TileType>> tileSuperPositions;

    public AstarTile[] tileRefs; //Assign in editor - have indexes match with TileType

    public Tilemap tilemap;
    
    // Start is called before the first frame update
    void Start()
    {
        tileNeighbours.Add(TileType.Forest, tiles);
        tileNeighbours.Add(TileType.Desert, tiles);
        tileNeighbours.Add(TileType.Mountains, tiles);
        tileNeighbours.Add(TileType.Plains, tiles);
        tileNeighbours.Add(TileType.Water, tiles);

        GenerateMap(100, 100, 11.33f, Random.Range(0, 10000000), Random.Range(0, 10000000), 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1")) 
        {
            GenerateMap(100, 100, 1.33f, Random.Range(0, 10000000), Random.Range(0, 10000000), 0.1f);
        }
    }

    public void GenerateMap(int mapWidth, int mapLength, float multiplier, int heightSeed, int moistSeed, float frequency) 
    {
        m_mapWidth = mapWidth;
        m_mapLength = mapLength;

        FastNoiseLite noise = new FastNoiseLite();
        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        noise.SetFrequency(frequency);

        // May not be 100% necessary
        float[,] heightMap = new float[mapWidth, mapLength];
        float[,] moistureMap = new float[mapWidth, mapLength];

        tileSuperPositions = new Dictionary<(int, int), HashSet<TileType>>(); 

        for (int x = 0; x < mapWidth; x++) 
        {
            for (int y = 0; y < mapLength; y++) 
            {
                tileSuperPositions.Add((x, y), new HashSet<TileType>(tiles)); //Add all the possible tile spots - entropy maximum to start with

                noise.SetSeed(heightSeed);
                heightMap[x, y] = Mathf.Abs(noise.GetNoise(((float)x * multiplier), ((float)y * multiplier)));

                if (heightMap[x, y] < 0.5) 
                {
                    tileSuperPositions[(x, y)].Remove(TileType.Mountains);
                } else {
                    tileSuperPositions[(x, y)].Remove(TileType.Water);
                    tileSuperPositions[(x, y)].Remove(TileType.Desert);
                    tileSuperPositions[(x, y)].Remove(TileType.Plains);
                }

                noise.SetSeed(moistSeed);
                moistureMap[x, y] = Mathf.Abs(noise.GetNoise(((float)x / (float)mapWidth * multiplier), ((float)y / (float)mapLength * multiplier)));
                
                if (moistureMap[x, y] < 0.6) 
                {
                    tileSuperPositions[(x, y)].Remove(TileType.Water);
                } else if (moistureMap[x, y] < 0.3) 
                {
                    tileSuperPositions[(x, y)].Remove(TileType.Forest);
                }
                else
                {
                    tileSuperPositions[(x, y)].Remove(TileType.Desert);
                }

                // tileSuperPositions[(x, y)].Remove(TileType.Forest);
            }
        } 

        /*

            Remove options based on temperature, moisture and height

        */

        for (int i = 0; i < mapWidth * mapLength; i++)
        {
            int minEntropy = tiles.Length + 1; //Add 1 so that it always selects one in the for loop 
            (int, int) positionToCollapse = (10, 10); //Default value: In case all entropy turns out equal (first iteration)
            
            foreach (KeyValuePair<(int, int), HashSet<TileType>> Position in tileSuperPositions) 
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
        HashSet<TileType> superPositions = tileSuperPositions[positionToCollapse];
        Debug.Log(superPositions.Count);

        if(superPositions.Count < 1 ) 
        {
            return;
        }
        TileType selectedTile = new List<TileType>(superPositions)[Random.Range(0, superPositions.Count - 1)];

        if (fullyCollapse)
        {
            tilemap.SetTile(
                new Vector3Int(positionToCollapse.Item1, positionToCollapse.Item2, 0), 
                tileRefs[(int)selectedTile]
            );

            tileSuperPositions[positionToCollapse].Clear();
            tileSuperPositions[positionToCollapse].Add(selectedTile);
        } 

        HashSet<TileType> tilesToRemove = new HashSet<TileType>(tiles);
            
        foreach(TileType pTile in superPositions) //Set Value to propagate changes
        {
            // foreach (TileType tile in tiles) 
            // {
            //     if(!System.Array.Exists(tileNeighbours[pTile], neighbourTile => neighbourTile == tile))
            //     {
            //         if(!tilesToRemove.Contains(tile)) 
            //         {
            //             tilesToRemove.Add(tile);
            //         }
            //     }
            // }

            tilesToRemove.IntersectWith(tileNeighbours[pTile]); //FIX THIS
        }

        for(int x = -1; x <= 1; x++) 
        {
            for(int y = -1; y <=1; y++) 
            {
                (int, int) newPos = (Mathf.Clamp(positionToCollapse.Item1 + x, 0, m_mapWidth - 1), Mathf.Clamp(positionToCollapse.Item2 + y, 0, m_mapLength - 1));

                // foreach (TileType tile in tilesToRemove) 
                // {
                //     tileSuperPositions[newPos].Remove(tile);
                //     // tileSuperPositions[newPos].IntersectWith(tileNeighbours);
                //     changed = true;
                // }

                if(tileSuperPositions[newPos].Overlaps(tilesToRemove))
                {
                    tileSuperPositions[newPos].ExceptWith(tilesToRemove);
                    // CollapseTile(newPos, false);
                    // Debug.Log("RE-COLLAPSING TILE");
                }
            }
        }
    }
}
