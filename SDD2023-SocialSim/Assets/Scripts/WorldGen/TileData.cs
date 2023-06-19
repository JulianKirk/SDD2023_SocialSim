using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TileData 
{
    public int entropy;

    HashSet<TileType> possibleTiles;

    Dictionary<TileType, HashSet<TileType>> tileRules;

    public int xPos, yPos;

    public TileType finalTile;

    public TileData(HashSet<TileType> tiles, Dictionary<TileType, HashSet<TileType>> rules, int x, int y) 
    {
        possibleTiles = new HashSet<TileType>(tiles);

        // foreach(TileType tile in tiles) 
        // {
        //     possibleTiles.Add(tile);
        // }

        tileRules = rules;

        xPos = x;
        yPos = y;

        UpdateEntropy();
    }

    public void RemovePossibilities(HashSet<TileType> tiles)
    {
        Debug.Log("Bryan1: " + possibleTiles.Count);

        possibleTiles.ExceptWith(tiles);
        
        Debug.Log("Bryan2: " + possibleTiles.Count);

        UpdateEntropy();
    }

    public HashSet<TileType> GenerateImpossibilities() 
    {
        HashSet<TileType> impossibilities = new HashSet<TileType>();

        foreach (TileType tile in possibleTiles) 
        {
            impossibilities.UnionWith(tileRules[tile]);
        }

        return impossibilities;
    }

    private void UpdateEntropy() 
    {
        entropy = possibleTiles.Count;

        if (entropy == 1) 
        {
            Collapse();
        }
    }

    public void Collapse() 
    {
        Debug.Log("Count: " + possibleTiles.Count);
        finalTile = possibleTiles.ElementAt(Random.Range(0, possibleTiles.Count));

        possibleTiles.Clear();
        // possibleTiles.Add(finalTile);

        entropy = 0; //This will flag it as ready to spawn
    }
}