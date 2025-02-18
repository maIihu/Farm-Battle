using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class CropController : MonoBehaviour
{
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;
    [SerializeField] private GameObject treePrefab;
    [SerializeField] private TileBase dat;
    [SerializeField] private TileBase hat;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tilemap tilemap2;

    private GameObject player1Pick;
    private GameObject player2Pick;

    private Dictionary<Vector3Int, bool> hasCop = new Dictionary<Vector3Int, bool>();
    
    private void Start()
    {
        player1Pick = player1.transform.gameObject;
        player2Pick = player2.transform.gameObject;
    }

    private void Update()
    {
        PlayerController player1C = player1.GetComponentInChildren<PlayerController>();
        PlayerController2 player2C = player2.GetComponentInChildren<PlayerController2>();
        
        if (player1C.isHit)
        {
            Vector3 pos = player1Pick.transform.position;
            Vector3Int cellPos = tilemap.WorldToCell(pos);
            TileBase currentTile = tilemap.GetTile(cellPos);
            if (currentTile != null) 
            {
                if(currentTile.name is "Tile2" or "Cliff_Tile_4") 
                    tilemap.SetTile(cellPos, dat);
                else if (tilemap.GetTile(cellPos) == dat && !hasCop.ContainsKey(cellPos))
                {
                    GameObject tree = Instantiate(treePrefab, player1Pick.transform.position, 
                        Quaternion.identity, this.tilemap.transform);
                    tree.name = pos.ToString();
                    hasCop[cellPos] = true;
                }
            }
            foreach (Transform child in tilemap.transform)
            {
                Crop crop = child.gameObject.GetComponent<Crop>();
                if (crop.isReadyToHarvest && pos.ToString() == child.name)
                {
                    crop.Harvest();
                    player1C.Score++;
                    
                }
            }
        }

        if (player2C.isHit)
        {
            Vector3 pos = player2Pick.transform.position;
            Vector3Int cellPos = tilemap2.WorldToCell(pos);
            TileBase currentTile = tilemap2.GetTile(cellPos);
            
            if (currentTile != null) 
            {
                if(currentTile.name is "Tile2" or "Cliff_Tile_4") 
                    tilemap2.SetTile(cellPos, dat);
                else if (tilemap2.GetTile(cellPos) == dat || !hasCop[cellPos])
                {
                    GameObject tree = Instantiate(treePrefab, player2Pick.transform.position, Quaternion.identity,
                        this.tilemap2.transform);
                    tree.name = pos.ToString();
                    hasCop[cellPos] = true;
                }
            }
            foreach (Transform child in tilemap2.transform)
            {
                Crop crop = child.gameObject.GetComponent<Crop>();
                if (crop.isReadyToHarvest && pos.ToString() == child.name)
                {
                    crop.Harvest();
                    player2C.Score++;
                }
            }
            
        }
        
    }
    
}
