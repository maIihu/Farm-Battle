using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class BotController : MonoBehaviour
{
    [SerializeField] private Tilemap tileMap;
    [SerializeField] private float moveSpeed = 5f;

    private bool _started;
    private bool _moveToStart;
    private bool _planted;
    
    private bool _replant;
    private bool _isHarvesting;
    private bool _movingToReplant;
    private bool _isRaining;

    private bool _moveToBomb;

    private bool _isPlanting;
    
    private Transform _pickCell;
    private Plant _targetPlant;
    private Vector3 _bombPosition;
    
    private Dictionary<Vector3, Plant> _plantsCanHarvest;
    private List<Vector3> _plants;
    private List<Vector3> _plantsDestroyed;

    private Coroutine _currentRoutine;
    
    public static BotController Instance { get; private set; }
    public int score;
    
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
        _pickCell = transform.GetChild(0);
        _plantsCanHarvest = new Dictionary<Vector3, Plant>();
        _plantsDestroyed = new List<Vector3>();
        _plants = new List<Vector3>();
        TileCanPlant();
    }

    private void TileCanPlant()
    {
        for (int i = 13; i <= 24; i++)
        {
            for (int j = -12; j <= -1; j++)
            {
                float xPos = i + 0.5f;
                float yPos = j + 0.5f;
                Vector3 pos = new Vector3(xPos, yPos, 0f);
                _plants.Add(pos);
            }
        }        
    }
    
    private void MouseEatPlant(Vector3 obj)
    {
        if (_targetPlant != null)
        {
            if (_targetPlant.transform.position == obj)
                _targetPlant = null;
        }
    }
    
    private void Rain(int obj)
    {
        if (obj == 2)
            _isRaining = !_isRaining;
    }

    private void StopHarvest(List<Vector3> plantList)
    {
        _replant = true;
        
        _isHarvesting = false;
        _movingToReplant = false;
        _moveToBomb = false;
        
        _targetPlant = null;
        
        _plantsDestroyed.AddRange(plantList);
        
        foreach (var plantPos in plantList)
        {
            MapManager.Instance.map.Remove(plantPos);
            MapManager.Instance.hasCrop.Remove(plantPos);
            _plantsCanHarvest.Remove(plantPos);
        }
    }
    
    private void Update()
    {
        if (GameManager.Instance.currentState == GameState.Playing)
        {
            if (!_started)
            {
                StartCoroutine(MoveToStartPoint());
                _started = true;
            }
            
            if (_moveToStart)
            {
                if (!_planted)
                {
                    StartCoroutine(MoveToPlant(_plants));
                    _planted = true;
                    _moveToStart = false;
                }
            }
        
            if (_isHarvesting)
            {
                CanHarvestPlant();
                if (_plantsCanHarvest.Count > 0)
                {
                    MoveToNearestPlant();
                }
            }

            if (_replant)
            {
                if (!_movingToReplant)
                {
                    StartRoutine(MoveToPlant(_plantsDestroyed));
                    _movingToReplant = true;
                    _replant = false;
                }
            }
        
            if (_isRaining)
                MapManager.Instance.BuffGrowTime(tileMap);
        
            _pickCell.position = new Vector3((int)(transform.position.x) + 0.5f, 
                (int)(transform.position.y) - 0.5f, transform.position.z);

            if (_moveToBomb)
            {
                StartCoroutine(MoveSmooth(_bombPosition));
                if (Vector3.Distance(transform.position, _bombPosition) < 0.1f)
                    KickBomb();
            }
        }
    }
    
    private void KickBomb()
    {
        StartCoroutine(KickBombRoutine());
    }

    private IEnumerator KickBombRoutine()
    {
        yield return new WaitForSeconds(0.2f); 

        GameObject bomb = GameObject.FindGameObjectWithTag("Bomb");
        if (bomb)
        {
            float x = Random.Range(2, 10);
            float y = Random.Range(-10, -2);
            bomb.GetComponent<BombController>().ThrowingBomb(new Vector3(x, y, 0));
        }
        yield return new WaitForSeconds(0.1f); 

        _moveToBomb = false;
        _isHarvesting = true;
    }
    
    private IEnumerator MoveToPlant(List<Vector3> objectsToDig)
    {
        List<Vector3> objectsToSow = new List<Vector3>();
        
        while (objectsToDig.Count > 0)
        {
            Vector3 targetPosition = objectsToDig.OrderBy(p => Vector3.Distance(transform.position, p)).FirstOrDefault();
            objectsToDig.Remove(targetPosition);
            objectsToSow.Add(targetPosition);
            
            yield return MoveSmooth(targetPosition);
            MapManager.Instance.Dig(_pickCell.position, tileMap);
        }

        while (objectsToSow.Count > 0)
        {
            Vector3 targetPosition = objectsToSow.OrderBy(p => Vector3.Distance(transform.position, p)).FirstOrDefault();
            objectsToSow.Remove(targetPosition);
            
            yield return MoveSmooth(targetPosition);
            MapManager.Instance.Sow(_pickCell.position);
        }
        _isHarvesting = true;
    }
    
    private IEnumerator MoveToStartPoint()
    {
        Vector3 currentPosition = transform.position;
        Vector3[] points = { new (13.5f, -0.5f, 0f), new (24.5f, -0.5f, 0f), new (13.5f, -11.5f, 0f), new (24.5f, -11.5f, 0f) };
        Vector3 nearestPoint = points[0];
        
        float minDistance = Vector3.Distance(currentPosition, points[0]);
        for (int i = 1; i < points.Length; i++)
        {
            float distance = Vector3.Distance(currentPosition, points[i]);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestPoint = points[i];
            }
        }

        yield return MoveSmooth(nearestPoint);
        MapManager.Instance.Dig(_pickCell.position, tileMap);
        _moveToStart = true;
    }
    
    private IEnumerator MoveSmooth(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition;
    }
    
    private void CanHarvestPlant()
    {
        for (int i = 0; i < tileMap.transform.childCount; i++)
        {
            Transform child = tileMap.transform.GetChild(i);
            if (child.childCount > 0)
            {
                Plant plant = child.GetChild(0).gameObject.GetComponent<Plant>();
                if (plant != null && plant.isReadyToHarvest && !_plantsCanHarvest.ContainsKey(plant.transform.position))
                {
                    _plantsCanHarvest.Add(plant.transform.position, plant);
                }
            }
        }
    }

    private void MoveToNearestPlant()
    {
        _targetPlant = _plantsCanHarvest.OrderBy(p => Vector3.Distance(transform.position, p.Key)).FirstOrDefault().Value;

        if (_targetPlant == null)
            return;
        
        transform.position = Vector3.MoveTowards(transform.position, _targetPlant.transform.position, moveSpeed * Time.deltaTime);
        
        if (Vector3.Distance(transform.position, _targetPlant.transform.position) < 0.01f) 
        {
            if (_targetPlant.isReadyToHarvest) 
            {
                _targetPlant.Harvest();
                score++;
            }

            _plantsCanHarvest.Remove(_targetPlant.transform.position);
            _targetPlant = null;
        }
    }
    
    private void MoveAndThrowingBomb()
    {
        GameObject bomb = GameObject.FindGameObjectWithTag("Bomb");
        if (bomb)
            _bombPosition = bomb.transform.position;

        _isHarvesting = false;
        _targetPlant = null;
        _moveToBomb = true;
        
    }

    private void StartRoutine(IEnumerator routine)
    {
        if (_currentRoutine != null)
            StopCoroutine(_currentRoutine);
        _currentRoutine = StartCoroutine(routine);
    }
    
    private void OnEnable()
    {
        ItemEffectManager.DestroyMap += StopHarvest;
        BombManager.OnBombExploded += StopHarvest;
        
        ItemEffectManager.IsRaining += Rain;
        Mouse.PlantDestroyed += MouseEatPlant;
        
        BombController.BotHasBomb += MoveAndThrowingBomb;
        BombManager.SpawnBombOnTheRight += MoveAndThrowingBomb;
    }

    private void OnDestroy()
    {
        ItemEffectManager.DestroyMap -= StopHarvest;
        BombManager.OnBombExploded -= StopHarvest;
        
        ItemEffectManager.IsRaining -= Rain;
        Mouse.PlantDestroyed -= MouseEatPlant;
        
        BombController.BotHasBomb -= MoveAndThrowingBomb;
        BombManager.SpawnBombOnTheRight -= MoveAndThrowingBomb;
    }
}
