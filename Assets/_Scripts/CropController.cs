using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class CropController : MonoBehaviour
{
    [SerializeField] private GameObject treePrefab;
    [SerializeField] private TileBase dat;
    
    private Dictionary<Vector3Int, bool> hasCop = new Dictionary<Vector3Int, bool>();
    
    private void Start()
    {
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
            else if (tile.GetTile(cellPos) == dat && !hasCop.ContainsKey(cellPos))
            {
                GameObject tree = Instantiate(treePrefab, pos, 
                    Quaternion.identity, tile.transform);
                tree.name = pos.ToString();
                hasCop[cellPos] = true;
            }
        }
        foreach (Transform child in tile.transform)
        {
            Crop crop = child.gameObject.GetComponent<Crop>();
            if (crop.isReadyToHarvest && pos.ToString() == child.name)
            {
                crop.Harvest();
                //player1C.Score++;
            }
        }
    }
    
}
