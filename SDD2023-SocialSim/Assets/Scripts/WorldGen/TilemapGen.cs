using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapGen : MonoBehaviour
{
    public List<AstarTile> tiles; //The tiles are added to the array in the editor
    public Tilemap tilemap;

    Dictionary<(int, int), List<AstarTile>> tilePossibilities;

    // Start is called before the first frame update
    void Start()
    {
        GenerateMap(100, 100, 11f, 1233, 5312, 3213, 0.03f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateMap(int mapWidth, int mapLength, float multiplier, int heightSeed, int moistSeed, int heatSeed, float frequency) //params makes the seed optional
    {
        FastNoiseLite noise = new FastNoiseLite();
        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        noise.SetFrequency(frequency);

        // float[,] heightMap = new float[mapWidth, mapLength];
        // float[,] moistureMap = new float[mapWidth, mapLength];
        // float[,] heatMap = new float[mapWidth, mapLength];

        tilePossibilities = new Dictionary<(int, int), List<AstarTile>>(); 

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
                
                // List<AstarTile> tempTileList = new List<AstarTile>(tiles);
                tilePossibilities.Add((x, y), tiles); //Add all the possible tile spots - entropy maximum to start with
            }
        }

        /*

            Remove options based on temperature, moisture and height

        */

        //Spawn in the tiles
        for (int i = 0; i < mapWidth * mapLength; i++) //Repeat for every spot on the grid so that everything is eventaully collapsed
        {
            int minEntropy = tiles.Count + 1; //Add 1 so that it always selects one in the for loop 
            (int, int) positionToCollapse = (1, 1);
            
            foreach (KeyValuePair<(int, int), List<AstarTile>> Position in tilePossibilities) 
            {
                if (Position.Value.Count < minEntropy && Position.Value.Count > 1) //Doesn't have to be <= as it can just collapse the first instance of the smallest entropy it finds
                {
                    minEntropy = Position.Value.Count;
                    positionToCollapse = Position.Key;
                }
            }

            CollapseTile(positionToCollapse, minEntropy, true, mapWidth, mapLength);
        }
    }

    void CollapseTile((int, int) positionToCollapse, int entropy, bool collapse, int mapWidth, int mapLength) 
    {
        List<AstarTile> tilePoss = tilePossibilities[positionToCollapse];
        AstarTile newTile = tilePoss[Random.Range(0, entropy - 1)];
        
        List<AstarTile> tilesToRemove = new List<AstarTile>(); //Refers to the tile possibilities for an individual tile spot
        if(collapse) 
        {
            tilemap.SetTile(
            new Vector3Int(positionToCollapse.Item1, positionToCollapse.Item2, 0), 
            newTile);
            
            // List<AstarTile> newTilePoss = new List<AstarTile>(); 
            // newTilePoss.Add(newTile);

            // tilePossibilities[positionToCollapse] = newTilePoss; 

            tilePossibilities[positionToCollapse].Clear();
            tilePossibilities[positionToCollapse].Add(newTile);
            

            foreach(AstarTile tile in tiles) //Propogate changes if collapsed
            {
                if (!tilePossibilities[positionToCollapse][0].allowedNeighbours.Contains(tile)) 
                {
                    tilesToRemove.Add(tile);
                }
            }
        } else {
            foreach(AstarTile pTile in tilePoss) //Propogate changes if collapsed
            {
                foreach(AstarTile tile in tiles) 
                {
                    if (!pTile.allowedNeighbours.Contains(tile)) 
                    {
                        if(!tilesToRemove.Contains(tile)) 
                        {
                            tilesToRemove.Add(tile);
                        }
                    }
                }
            }
        }

        //Adjust the tile possibilities of nearby tiles by incrementing the coordinates - USE RECURSION
        for(int x = -1; x <= 1; x++) 
        {
            for(int y = -1; y <=1; y++) 
            {
                bool changed = false;
                (int, int) newPos = (Mathf.Clamp(positionToCollapse.Item1 + x, 0, mapWidth - 1), Mathf.Clamp(positionToCollapse.Item2 + y, 0, mapLength - 1));
                
                foreach(AstarTile tile in tilesToRemove) 
                {
                    if(tilePossibilities[newPos].Contains(tile)) 
                    {
                        tilePossibilities[newPos].Remove(tile);
                        changed = true;
                    }
                }

                if(changed) //Keeps giving "ArgumentOutOfRangeException: Index was out of range"
                {
                    CollapseTile(newPos, tilePossibilities[newPos].Count, false, mapWidth, mapLength);
                }
            }
        }
    }
}
