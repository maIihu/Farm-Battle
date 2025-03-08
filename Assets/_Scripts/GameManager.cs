using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private Tilemap tileMap1;
    [SerializeField] private Tilemap tileMap2;
    
    private const float X1 = 1;
    private const float X2 = 14;
    private const float Y = -11;

    private Vector3 _bombLocation;
    private void Start()
    {
        SpawnBomb();
    }

    private void SpawnBomb()
    {
        float locationY = Random.Range(Y, Y + 10);
        // them dieu kien neu Diem(player > bot) tai day
        float locationX = Random.Range(X1, X1 + 10);
        _bombLocation = new Vector3(locationX, locationY, this.transform.position.z);
        GameObject bombClone = Instantiate(bombPrefab, _bombLocation, Quaternion.identity);
        bombClone.GetComponent<BombController>();
    }
    
    private void OnEnable()
    {
        BombController.OnBombExploded += HandleBombExplosion;
    }

    private void OnDisable()
    {
        BombController.OnBombExploded -= HandleBombExplosion;
    }

    private void HandleBombExplosion()
    {
        Debug.Log("No?!!!");
        DestroyMap();
    }

    private void DestroyMap()
    {
        float x = (int)_bombLocation.x + 0.5f;
        float y = (int)_bombLocation.y + 0.5f;

        List<Vector3> directions = new List<Vector3>();
        
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                Vector3 location = new Vector3(x + i, y + j, 0); 
                directions.Add(location);
            }
        }
        
        Transform plantContainer = tileMap1.transform;
        for (int k = 0; k < plantContainer.childCount; k++)
        {
            Transform plantClone = plantContainer.GetChild(k);
            foreach (var direction in directions)
            {
                if(plantClone.position == direction)
                {
                    Destroy(plantClone.gameObject);
                    MapManager.Instance.hasCrop.Remove(direction);
                    MapManager.Instance.map.Remove(direction);
                }
            }
            
        }
    }
    
}
