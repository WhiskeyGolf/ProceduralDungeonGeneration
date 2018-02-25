using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dungeon Tile Set", menuName = "Dungeon/Tile Set", order = 1)]
public class DungeonTileSet : ScriptableObject {

    public GameObject[] wallOneSided;
    public GameObject[] wallTwoSided;
    public GameObject[] wallTwoSidedCorner;
    public GameObject[] wallThreeSided;
    public GameObject[] wallFourSided;
    public GameObject[] floor;
    public GameObject[] door;
    public GameObject[] ceilingTile;

}
