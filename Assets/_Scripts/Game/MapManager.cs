using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    [SerializeField] private GameObject plantPrefab;
    [SerializeField] private GameObject dirtPrefab;

    public Dictionary<Vector3, GameObject> map;
    public Dictionary<Vector3, bool> hasCrop;

    private static MapManager _instance;
    public static MapManager Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        hasCrop = new Dictionary<Vector3, bool>();
        map = new Dictionary<Vector3, GameObject>();
    }
    

    public bool Dig(Vector3 location, Tilemap tileMap)
    {
        Vector3Int cellPos = tileMap.WorldToCell(location);
        TileBase currentTile = tileMap.GetTile(cellPos);

        if (currentTile != null)
        {
            if (currentTile.name is "Tile2" or "Cliff_Tile_4" && !map.ContainsKey(location))
            {
                GameObject dirtClone = Instantiate(dirtPrefab, location, Quaternion.identity, tileMap.transform);
                dirtClone.name = $"{location.x}_{location.y}_{location.z}";
                map[location] = dirtClone;
                return true;
            }
        }

        return false;
    }

    public void Sow(Vector3 location)
    {
        if (map.ContainsKey(location) && !hasCrop.ContainsKey(location))
        {
            GameObject plantClone = Instantiate(plantPrefab, location, Quaternion.identity);
            plantClone.transform.SetParent(map[location].transform);
            plantClone.GetComponent<Plant>().gridLocation = location;
            hasCrop[location] = true;
        }
    }

    public void Harvest(Vector3 location, Tilemap tileMap,  ref int score)
    {
        for (int i = 0; i < tileMap.transform.childCount; i++)
        {
            Transform child = tileMap.transform.GetChild(i);
            if (child.childCount > 0)
            {
                Plant plant = child.GetChild(0).gameObject.GetComponent<Plant>();
                if (plant != null && plant.isReadyToHarvest && location.ToString() == child.name)
                {
                    plant.Harvest();
                    score++;
                }
            }
        }
    }
}
