using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum TileType:int 
{
Desert = 0, 
Forest = 1, 
Mountains = 2, 
Plains = 3, 
Water = 4
}

public class WorldGeneration
{
    private static readonly Lazy<WorldGeneration> _instance = new Lazy<WorldGeneration>(() => new WorldGeneration());

    public static WorldGeneration instance { get { return _instance.Value; } }

    HashSet<TileType> tileTypes = new HashSet<TileType>{
        TileType.Desert, 
        TileType.Forest, 
        TileType.Mountains, 
        TileType.Plains, 
        TileType.Water
    };

    Dictionary<TileType, HashSet<TileType>> tileBannedNeighbours = new Dictionary<TileType, HashSet<TileType>>() //Tiles that it CANNOT be next to
    {
        {TileType.Forest, new HashSet<TileType> {/*TileType.Desert*/}},
        {TileType.Desert, new HashSet<TileType> {/*TileType.Mountains, TileType.Water, TileType.Forest*/}},
        {TileType.Mountains, new HashSet<TileType> {/*TileType.Desert*/}},
        {TileType.Plains, new HashSet<TileType>()},
        {TileType.Water, new HashSet<TileType> {/*TileType.Desert*/}},
    };

    public TileData[,] GenerateMap(int mapWidth, int mapLength, float frequency, int moistSeed, int heightSeed) 
    {
        TileData[,] allTiles = new TileData[mapWidth, mapLength];

        FastNoiseLite noise = new FastNoiseLite();
        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        noise.SetFrequency(frequency);

        float[,] heightMap = new float[mapWidth, mapLength];
        float[,] moistureMap = new float[mapWidth, mapLength];

        for (int x = 0; x < mapWidth; x++) 
        {
            for (int y = 0; y < mapLength; y++) 
            {
                allTiles[x, y] = new TileData(tileTypes, tileBannedNeighbours, x, y);

                // noise.SetSeed(heightSeed);
                // heightMap[x, y] = Mathf.Abs(noise.GetNoise(((float)x * 1.01f), ((float)y * 1.01f)));

                // noise.SetSeed(moistSeed);
                // moistureMap[x, y] = Mathf.Abs(noise.GetNoise(((float)x / (float)mapWidth * 1.01f), ((float)y / (float)mapLength * 1.01f)));
            }
        }

        bool allTilesCollapsed = false;

        TileData minEntropyTile = allTiles[1, 1]; //All tiles are equal in entropy to start with
        int minEntropy = tileTypes.Count;

        while (!allTilesCollapsed) 
        {
            foreach (TileData data in allTiles) 
            {
                if (data.entropy != 0) 
                {
                    allTilesCollapsed = false;

                    break;
                } 
            }

            minEntropy = tileTypes.Count;

            if (tileTypes.Count == 0)
            {
                Debug.Log("Aavinland");
            }

            Debug.Log("Aavin: " + tileTypes.Count);

            foreach (TileData data in allTiles) 
            {
                if ((data.entropy > 0) && (data.entropy <= minEntropy)) 
                {
                    minEntropyTile = data;
                }
            }

            bool[,] propagationMap = new bool[mapWidth, mapLength]; //Default all false

            //if (minEntropyTile.entropy == 0) {
            //    continue;
            //}

            CollapseTile(minEntropyTile, allTiles, propagationMap);

            Debug.Log("Aavin2: " + tileTypes.Count);
        }

        return allTiles;
    }

    void CollapseTile(TileData tileToCollapse, TileData[,] tileGrid, bool[,] propGrid) 
    {
        tileToCollapse.Collapse();

        HashSet<TileType> tilesToRemove = tileToCollapse.GenerateImpossibilities();

        //Neighbours
        for(int x = -1; x <= 1; x++) 
        {

            for(int y = -1; y <=1; y++) 
            {
                int xCoord = Mathf.Clamp(tileToCollapse.xPos + x, 0, tileGrid.GetLength(0) - 1);
                int yCoord = Mathf.Clamp(tileToCollapse.yPos + y, 0, tileGrid.GetLength(1) - 1);

                // tileGrid[xCoord, yCoord].RemovePossibilities(tilesToRemove);

                if (!propGrid[xCoord, yCoord])
                {
                    tileGrid[xCoord, yCoord].RemovePossibilities(tilesToRemove);
                    propGrid[xCoord, yCoord] = true;

                    if (tileToCollapse.entropy > 0) 
                    {
                        CollapseTile(tileGrid[xCoord, yCoord], tileGrid, propGrid);
                    }
                }
            }
        }
    }
}