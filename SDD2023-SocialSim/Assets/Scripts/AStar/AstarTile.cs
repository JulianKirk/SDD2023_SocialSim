using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//For an old idea of implementing A*, I didn't use this
[CreateAssetMenu(fileName = "New Astar Tile", menuName = "Tiles/AstarTile")]
public class AstarTile : Tile
{
    public List<AstarTile> allowedNeighbours;
}
