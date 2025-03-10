using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class BotController : MonoBehaviour
{
    public static BotController Instance { get; private set; } 
    
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private MapManager map;
    [SerializeField] private Tilemap tileMap;
    [SerializeField] private float time;
    
    private int _stepCount;
    private int _moveDir = 1;
    
    // private bool _isDigging = true;
    // private bool _isSowing = true;
    
    private bool _hasDug;
    private bool _hasSowed;
    private bool _isHarvesting;
    private bool _isMovingToPlant;
    
    private Plant _targetPlant;

    private List<Plant> _plants;

    public int score;

    private Vector3 _bombPosition;
    private bool _isChasingBomb;
    
    private bool _replanting;
    private List<Vector3> destroyedAreas;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
            return;
        }
        Instance = this;
    }
    
    private void Start()
    {
        _plants = new List<Plant>();
        
        MoveToStartPoint();
        StartCoroutine(MoveToDigRoutine());
        
        PlayerController.OnBombThrown += MoveToBomb;
        BombManager.OnBombExploded += HandleDestroyedAreas;
    }
    
    private void MoveToStartPoint()
    {
        transform.position = new Vector3(13.5f, -0.5f, 0f);
        map.Crop(transform.GetChild(0).transform.position, tileMap, ref score);
    }
    
    private void OnDestroy()
    {
        PlayerController.OnBombThrown -= MoveToBomb; 
        BombManager.OnBombExploded -= HandleDestroyedAreas;
    }
    
    private void HandleDestroyedAreas(List<Vector3> destroyedPositions)
    {
        destroyedAreas = new List<Vector3>();
        _replanting = true;
        destroyedAreas.AddRange(destroyedPositions);
        StartCoroutine(ReplantCrops());
    }

    private IEnumerator ReplantCrops()
    {
        while (destroyedAreas.Count > 0)
        {
            Vector3 targetPosition = destroyedAreas[0];
            destroyedAreas.RemoveAt(0);

            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }

            map.Crop(transform.GetChild(0).transform.position, tileMap, ref score);
        }
    }

    
    private void MoveToBomb(Vector3 bombPos)
    {
        _bombPosition = bombPos;
        _isChasingBomb = true;
        _isHarvesting = false;
        _targetPlant = null;
    }
    
    private void Update()
    {
        Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);
        
        if (!_hasDug && currentPos == new Vector2(24.5f, -0.5f))
        {
            _hasDug = true; 
            
            transform.position = new Vector3(23.5f, -0.5f);
            map.Crop(transform.GetChild(0).transform.position, tileMap, ref score);
            
            _stepCount = 0;
            _moveDir = 1;
            StartCoroutine(MoveToSowRoutine());
        }
        
        if (_isChasingBomb)
        {
            transform.position = Vector3.MoveTowards(transform.position, _bombPosition, moveSpeed * Time.deltaTime);
            
            if (Vector3.Distance(transform.position, _bombPosition) < 0.5f)
            {
                KickBomb();
            }
            
        }
        
        CanHarvestPlant();
        
        // if (_isHarvesting && !_isMovingToPlant)
        // {
        //     FindNearestPlant();
        // }
        // if (_isHarvesting && _isMovingToPlant) 
        // {
        //     MoveToTargetPlant();
        // }

        if (_isHarvesting)
        {
            FindNearestPlant();
            MoveToTargetPlant();
        }
    }

    private void FindNearestPlant()
    {
        if (_plants.Count == 0) return;

        _targetPlant = _plants.OrderBy(p => Vector3.Distance(transform.position, p.gridLocation)).FirstOrDefault();
        if (_targetPlant != null)
        {
            _isMovingToPlant = true; 
        }
    }

    private void KickBomb()
    {
        GameObject bomb = GameObject.FindGameObjectWithTag("Bomb");
        if (bomb)
        {
            Vector3 des = new Vector3(Random.Range(1, 11), - 11 - transform.position.y, 0);
            bomb.GetComponent<BombController>().ThrowingBomb(des);
        }
        _isChasingBomb = false;
        _isHarvesting = true;
    }
    
    private void MoveToTargetPlant()
    {
        if (_targetPlant == null) return;

        transform.position = Vector3.MoveTowards(transform.position, _targetPlant.gridLocation, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, _targetPlant.gridLocation) < 0.1f) 
        {
            _targetPlant.Harvest();
            score++;
            _plants.Remove(_targetPlant);
            _targetPlant = null;
            _isMovingToPlant = false; 
        }
    }

    private IEnumerator MoveToDigRoutine()
    {
        while (!_hasDug)
        {
            MoveToDig();
            yield return new WaitForSeconds(time);
        }
    }
    
    private void MoveToDig()
    {
        Vector3 currentPosition = transform.position;
        
        if (_stepCount < 11)
        {
            transform.position = new Vector3(currentPosition.x, currentPosition.y - 1f * _moveDir, currentPosition.z);
            _stepCount++;
        }
        else
        {
            transform.position = new Vector3(currentPosition.x + 1f, currentPosition.y , currentPosition.z);
            _moveDir *= -1;
            _stepCount = 0;
        }
        map.Crop(this.transform.GetChild(0).transform.position, tileMap, ref score);
    }
    
    private IEnumerator MoveToSowRoutine()
    {
        while (!_hasSowed)
        {
            MoveToSow();
            yield return new WaitForSeconds(time); 
        }
    }
    
    private void MoveToSow()
    {
        Vector3 currentPosition = transform.position;
        if(currentPosition.x == 24.5f && currentPosition.y == -1.5f)
        {
            _hasSowed = true;
            transform.position = new Vector3(24.5f, -0.5f);
            map.Crop(transform.GetChild(0).transform.position, tileMap, ref score);
            _isHarvesting = true;
            return;
        }
        if (currentPosition.y == -0.5f)
        {
            transform.position = new Vector3(currentPosition.x - 1f, currentPosition.y, currentPosition.z);
            if (currentPosition.x == 13.5f)
                transform.position = new Vector3(currentPosition.x, currentPosition.y - 1f, currentPosition.z);
        }
        else
        {
            if (_stepCount < 10)
            {
                transform.position = new Vector3(currentPosition.x, currentPosition.y - 1f * _moveDir, currentPosition.z);
                _stepCount++;
            }
            else
            {
                transform.position = new Vector3(currentPosition.x + 1f, currentPosition.y , currentPosition.z);
                _moveDir *= -1;
                _stepCount = 0;
            }
        }
        map.Crop(this.transform.GetChild(0).transform.position, tileMap, ref score);
    }
    
    private void CanHarvestPlant()
    {
        for (int i = 0; i < tileMap.transform.childCount; i++)
        {
            Transform child = tileMap.transform.GetChild(i);
            if (child.childCount > 0)
            {
                Plant plant = child.GetChild(0).gameObject.GetComponent<Plant>();
                if (plant != null && plant.isReadyToHarvest && !_plants.Contains(plant))
                {
                    _plants.Add(plant);
                }
            }
        }
    }


    
}