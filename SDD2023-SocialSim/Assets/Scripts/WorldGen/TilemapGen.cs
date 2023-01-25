using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapGen : MonoBehaviour
{
    public Tile[] tiles; //The tiles are added to the array in the editor
    public Tilemap tilemap;

    // Start is called before the first frame update
    void Start()
    {
        GenerateMap(20, 20, 11f, 1233, 5312, 3213, 0.03f);
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

        Dictionary<(int, int), Tile[]> tilePossibilities = new Dictionary<(int, int), Tile[]>(); 

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

                tilePossibilities.Add((x, y), tiles); //Add all the posible tile spots - entropy maximum to start with
            }
        }

        /*

            Remove options based on temperature, moisture and height

        */

        //Spawn in the tiles
        for (int i = 0; i < mapWidth * mapLength; i++) //Repeat for every spot on the grid so that everything is eventaully collapsed
        {
            int minEntropy = tiles.Length + 1; //Add 1 so that it always selects one in the for loop 
            (int, int) positionToCollapse = (1, 1);
            
            foreach (KeyValuePair<(int, int), Tile[]> Position in tilePossibilities) 
            {
                if (Position.Value.Length < minEntropy) //Doesn't have to be <= as it can just collapse the first instance of the smallest entropy it finds
                {
                    minEntropy = Position.Value.Length;
                    positionToCollapse = Position.Key;
                }
            }

            tilemap.SetTile(
            new Vector3Int(positionToCollapse.Item1, positionToCollapse.Item2, 0), 
            tilePossibilities[positionToCollapse][Random.Range(0, minEntropy - 1)]
            );

            tilePossibilities.Remove(positionToCollapse);

            if(tilePossibilities.Count <= 0) 
            {
                break;
            }
        }
    }
}
