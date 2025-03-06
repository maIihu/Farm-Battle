using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class CropManager : MonoBehaviour
{
    [SerializeField] private GameObject treePrefab;
    [SerializeField] private TileBase dat;
    
    private Dictionary<Vector3Int, bool> _hasCrop;
    
    private void Start()
    {
        _hasCrop = new Dictionary<Vector3Int, bool>();
    }

    // private void Update()
    // {
    //     PlayerController player1C = player1.GetComponentInChildren<PlayerController>();
    //     PlayerController2 player2C = player2.GetComponentInChildren<PlayerController2>();
    //     
    //     if (player1C.isHit)
    //     {
    //         Crop(player1Pick.transform.position, tilemap);
    //     }
    //
    //     if (player2C.isHit)
    //     {
    //         Crop(player2Pick.transform.position, tilemap2);
    //     }
    // }

    public void Crop(Vector3 pos, Tilemap tile)
    {
        Vector3Int cellPos = tile.WorldToCell(pos);
        TileBase currentTile = tile.GetTile(cellPos);
        if (currentTile != null) 
        {
            if(currentTile.name is "Tile2" or "Cliff_Tile_4") 
                tile.SetTile(cellPos, dat);
            else if (tile.GetTile(cellPos) == dat && !_hasCrop.ContainsKey(cellPos))
            {
                GameObject tree = Instantiate(treePrefab, pos, Quaternion.identity, tile.transform);
                
                tree.name = pos.ToString();
                tree.GetComponent<Plant>().gridLocation = pos;
                
                _hasCrop[cellPos] = true;
            }
        }
        foreach (Transform child in tile.transform)
        {
            Plant plant = child.gameObject.GetComponent<Plant>();
            if (plant.isReadyToHarvest && pos.ToString() == child.name)
            {
                plant.Harvest();
                //player1C.Score++;
            }
        }
    }
    
}
