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

    private bool _starting;
    
    private bool _moveToStart;
    private bool _moveToDig;
    
    private bool _replant;
    private bool _isHarvesting;
    private bool _movingToPlant;
    private bool _isRaining;

    private bool _moveToBomb;
    
    private Transform _pickCell;
    private Plant _targetPlant;
    private Vector3 _bombPosition;
    
    private Dictionary<Vector3, Plant> _plantsCanHarvest;
    private List<Vector3> _destroyArea;
    private List<Vector3> _plantArea;
    private List<Vector3> _plants;
    
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
        _plantArea = new List<Vector3>();
        _destroyArea = new List<Vector3>();
        _plants = new List<Vector3>();

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

    private void StopHarvest(List<Vector3> plantsDestroyed)
    {
        _isHarvesting = false;
        _replant = true;
        _movingToPlant = false;
        _targetPlant = null;
        _destroyArea.AddRange(plantsDestroyed);
        
        foreach (var plantPos in plantsDestroyed)
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
            if (!_starting)
            {
                StartCoroutine(MoveToStartPoint());
                _starting = true;
            }
            
            if (_moveToStart)
            {
                if (!_moveToDig)
                {
                    StartCoroutine(MoveToPlant(_plants));
                    _moveToDig = true;
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
                if (!_movingToPlant)
                {
                    StartCoroutine(MoveToPlant(_destroyArea));
                    _movingToPlant = true;
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
        GameObject bomb = GameObject.FindGameObjectWithTag("Bomb");
        if (bomb)
        {
            float x = Random.Range(2, 10);
            float y = Random.Range(-10, -2);
            bomb.GetComponent<BombController>().ThrowingBomb(new Vector3(x, y, 0));
        }
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
        _plantArea.Add(_pickCell.position);
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
    
    private void OnEnable()
    {
        ItemEffectManager.DestroyMap += StopHarvest;
        ItemEffectManager.IsRaining += Rain;
        Mouse.PlantDestroyed += MouseEatPlant;
        BombController.BotHasBomb += MoveAndThrowingBomb;
        BombManager.OnBombExploded += StopHarvest;
        BombManager.SpawnBombOnTheRight += MoveAndThrowingBomb;
    }

    private void OnDestroy()
    {
        ItemEffectManager.DestroyMap -= StopHarvest;
        ItemEffectManager.IsRaining -= Rain;
        Mouse.PlantDestroyed -= MouseEatPlant;
        BombController.BotHasBomb -= MoveAndThrowingBomb;
        BombManager.OnBombExploded -= StopHarvest;
        BombManager.SpawnBombOnTheRight -= MoveAndThrowingBomb;
    }
}
