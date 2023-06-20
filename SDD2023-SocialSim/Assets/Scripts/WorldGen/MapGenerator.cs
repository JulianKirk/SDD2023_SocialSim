using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public Tilemap tileMap;

    public Tile[] tilePrefabs; //Set in editor, order matters: Desert, Forest, Mountains, Plains, Water

    public static bool[,] walkableGrid;

    public GameObject[] stoneResources;
    public GameObject[] woodResources;
    public GameObject[] fruitResources;

    private enum TileResourceClass
    {
        Stone,
        Wood,
        Fruit,
        Meat,
        None
    }

    private TileResourceClass[,] mapResourceClasses;

    FastNoiseLite m_noise = new FastNoiseLite();

    //For display in the results menu
    int m_heatSeed;
    int m_heightSeed;
    int m_mapLength;
    int m_mapWidth;

    void GenerateMap(int mapWidth, int mapLength, int heightSeed, int heatSeed, float frequency, float horizontalDilation, float verticalDilation) 
    {
        walkableGrid = new bool[mapWidth, mapLength];

        float[,] heatMap = new float[mapWidth, mapLength];
        float[,] heightMap = new float[mapWidth, mapLength];

        mapResourceClasses = new TileResourceClass[mapWidth, mapLength];

        m_mapLength = mapLength;
        m_mapWidth = mapWidth;

        m_heatSeed = heatSeed;
        m_heightSeed = heightSeed;

        for (int y = 0; y < mapLength; y++) 
        {
            for (int x = 0; x < mapWidth; x++) 
            {
                walkableGrid[x, y] = true;

                m_noise.SetSeed(heightSeed);
                heightMap[x, y] = verticalDilation * m_noise.GetNoise(((float)x * horizontalDilation), ((float)y * horizontalDilation));

                m_noise.SetSeed(heatSeed);
                heatMap[x, y] = verticalDilation * m_noise.GetNoise(((float)x * horizontalDilation), ((float)y * horizontalDilation));

                if (heightMap[x, y] > 0.4) 
                {
                    tileMap.SetTile(new Vector3Int(x, y), tilePrefabs[2]);

                    mapResourceClasses[x, y] = TileResourceClass.Stone;
                }
                else if (heightMap[x, y] > 0) 
                {
                    if (heatMap[x, y] > 0.2) 
                    {
                        tileMap.SetTile(new Vector3Int(x, y), tilePrefabs[0]);

                        mapResourceClasses[x, y] = TileResourceClass.Meat;
                    } 
                    else if (heatMap[x, y] > -0.3) 
                    {
                        tileMap.SetTile(new Vector3Int(x, y), tilePrefabs[1]);

                        mapResourceClasses[x, y] = TileResourceClass.Wood;
                    } 
                    else if (heatMap[x, y] > -0.6) 
                    {
                        tileMap.SetTile(new Vector3Int(x, y), tilePrefabs[3]);

                        mapResourceClasses[x, y] = TileResourceClass.Fruit;
                    }
                    else
                    {
                        tileMap.SetTile(new Vector3Int(x, y), tilePrefabs[2]);

                        mapResourceClasses[x, y] = TileResourceClass.Stone;
                    }
                }
                else if (heightMap[x, y] > -0.3) 
                {
                    if (heatMap[x, y] > 0.2) 
                    {
                        tileMap.SetTile(new Vector3Int(x, y), tilePrefabs[0]);

                        mapResourceClasses[x, y] = TileResourceClass.Meat;
                    } else 
                    {
                        tileMap.SetTile(new Vector3Int(x, y), tilePrefabs[3]);

                        mapResourceClasses[x, y] = TileResourceClass.Fruit;
                    }
                }
                else
                {
                    if (heatMap[x, y] > 0.2)
                    {
                        tileMap.SetTile(new Vector3Int(x, y), tilePrefabs[0]);

                        mapResourceClasses[x, y] = TileResourceClass.Meat;
                    } 
                    else if (heatMap[x, y] > -0.2)
                    {
                        tileMap.SetTile(new Vector3Int(x, y), tilePrefabs[3]);

                        mapResourceClasses[x, y] = TileResourceClass.Fruit;
                    }
                    else
                    { 
                        tileMap.SetTile(new Vector3Int(x, y), tilePrefabs[4]);

                        mapResourceClasses[x, y] = TileResourceClass.None;

                        walkableGrid[x, y] = false;
                    }
                }
            }
        }

        GenerateNewResources();
    }

    void GenerateNewResources() 
    {
        for (int y = 0; y < m_mapLength; y++) 
        {
            for (int x = 0; x < m_mapWidth; x++)
            {
                switch(mapResourceClasses[x, y])
                {
                    case TileResourceClass.Stone: 
                        if (Random.Range(0, 100) < 5)
                        {
                            Instantiate(stoneResources[Random.Range(0, stoneResources.Length)], new Vector3(x, y, 0), Quaternion.identity);
                        }
                        break;
                    case TileResourceClass.Wood: 
                        if (Random.Range(0, 100) < 13)
                        {
                            Instantiate(woodResources[Random.Range(0, woodResources.Length)], new Vector3(x, y, 0), Quaternion.identity);
                        }
                        break;
                    case TileResourceClass.Fruit: 
                        if (Random.Range(0, 100) < 10)
                        {
                            Instantiate(fruitResources[Random.Range(0, fruitResources.Length)], new Vector3(x, y, 0), Quaternion.identity);
                        }
                        break;
                    case TileResourceClass.Meat: 
                        //Instantiate(stoneResources[Random.Range(0, stoneResources.Length)], new Vector3(x, y, 0), Quaternion.identity);
                        //Spawn animals - maybe change this as we can give them breeding behaviour
                        break;
                    default:
                        break;
                }
            }
        }
    }

    void Awake()
    {
        m_noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        m_noise.SetFractalType(FastNoiseLite.FractalType.FBm);
        m_noise.SetFractalOctaves(5);

        GenerateMap(100, 100, 983, 920, 1f, 1f, 1f);
    }
}
