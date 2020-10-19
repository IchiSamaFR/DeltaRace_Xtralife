using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapModel
{
    public string name;
    public string wallTiles;
    public string otherTiles;
}

[System.Serializable]
public class TilePrefab
{
    public char name;
    public GameObject prefab;
}