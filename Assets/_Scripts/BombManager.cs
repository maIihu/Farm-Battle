using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class BombManager : MonoBehaviour
{
    public static event Action<List<Vector3>> OnBombExploded;
    
    public static BombManager Instance { get; private set; } 
    
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private Tilemap tileMap1;
    [SerializeField] private Tilemap tileMap2;
    
    private const float X1 = 1;
    private const float X2 = 14;
    private const float Y = -11;
    private Transform _targetTileMap;
    private GameObject _bombClone;
    
    // private void Awake()
    // {
    //     if (Instance != null && Instance != this)
    //     {
    //         Destroy(gameObject); 
    //         return;
    //     }
    //
    //     Instance = this;
    // }
    
    private void Start()
    {
        SpawnBomb();
        
    }

    private void Update()
    {
        if (_bombClone != null)
        {
            bool bombOnTheLeft = _bombClone.GetComponent<BombController>().onTheLeft;
            if (bombOnTheLeft)
                _targetTileMap = tileMap1.transform;
            else 
                _targetTileMap = tileMap2.transform;
        }
    }

    private void SpawnBomb()
    {
        int player1Score = PlayerController.Instance.score;
        int player2Score = 0;
        if (BotController.Instance.gameObject.activeSelf)
        {
            player2Score = BotController.Instance.score;
        }
        
        float locationY = Random.Range(Y, Y + 10);
        float locationX = 0;
        _bombClone = Instantiate(bombPrefab);
        if (player1Score >= player2Score)
        {
            locationX = Random.Range(X1, X1 + 10);
            _bombClone.GetComponent<BombController>().onTheLeft = true;
            _targetTileMap = tileMap1.transform;
        }
        else
        {
            locationX = Random.Range(X2, X2 + 10);
            _bombClone.GetComponent<BombController>().onTheLeft = false;
            _targetTileMap = tileMap2.transform;
        }

        _bombClone.transform.position = new Vector3(locationX, locationY, this.transform.position.z);
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
        DestroyMap();
    }

    private void DestroyMap()
    {
        float x = Mathf.FloorToInt(_bombClone.transform.position.x) + 0.5f;
        float y = Mathf.FloorToInt(_bombClone.transform.position.y) + 0.5f;
        
        List<Vector3>destroyedPositions = new List<Vector3>();
        
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                Vector3 location = new Vector3(x + i, y + j, 0); 
                destroyedPositions .Add(location);
            }
        }
        
        for (int k = 0; k < _targetTileMap.childCount; k++)
        {
            Transform plantClone = _targetTileMap.GetChild(k);        
            foreach (var direction in destroyedPositions )
            {
                if(plantClone.position == direction)
                {
                    Destroy(plantClone.gameObject);
                    MapManager.Instance.hasCrop.Remove(direction);
                    MapManager.Instance.map.Remove(direction);
                }
            }
        }

        if (_targetTileMap == tileMap2.transform)
        {
            OnBombExploded?.Invoke(destroyedPositions);
        }
    }
    
}
