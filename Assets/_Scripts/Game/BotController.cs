using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BotController : MonoBehaviour
{
    [SerializeField] private Tilemap tileMap;
    [SerializeField] private float moveSpeed = 5f;
    
    private int _stepCount;
    private int _moveDir = 1;

    private bool _hasDug;
    private bool _hasSowed;
    
    private bool _startDig;
    private bool _startSow;
    private bool _moveToStart;

    private bool _replant;
    
    private bool _isHarvesting;
    
    private Transform _pickCell;
    private Plant _targetPlant;
    
    private List<Plant> _plants;
    
    private List<Vector3> _destroyArea;
    private List<Vector3> _plantArea;
    
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
        _plants = new List<Plant>();
        _plantArea = new List<Vector3>();
        _destroyArea = new List<Vector3>();
        
        StartCoroutine(MoveToStartPoint());
    }

    private void OnEnable()
    {
        ItemEffectManager.DestroyMap += StopHarvest;
    }

    private void OnDestroy()
    {
        ItemEffectManager.DestroyMap -= StopHarvest;
    }
    
    private void StopHarvest()
    {
        StartCoroutine(StopHarvestCoroutine());
    }
    
    private IEnumerator StopHarvestCoroutine()
    {
        yield return new WaitForEndOfFrame(); 

        _isHarvesting = false;
        _replant = true;
        _targetPlant = null;

        List<Vector3> currentPlantList = new List<Vector3>();
        foreach (Transform child in tileMap.transform)
        {
            currentPlantList.Add(child.transform.position);
        }

        for (int i = 13; i <= 24; i++)
        {
            for (int j = -12; j <= -1; j++)
            {
                float xPos = i + 0.5f;
                float yPos = j + 0.5f;
                Vector3 pos = new Vector3(xPos, yPos, 0f);
                if (!currentPlantList.Contains(pos))
                {
                    _destroyArea.Add(pos);
                }
            }
        }

    }

    private bool _movingToPlant = false;
    private void Update()
    {
        if (_moveToStart && !_hasDug && !_startDig)
        {
            _startDig = true;
            StartCoroutine(MoveToDig(1, new Vector3(13.5f, -11.5f, 0f))); 
        }

        if (_hasDug && !_hasSowed && !_startSow)
        {
            _startSow = true;
            StartCoroutine(MoveToSow(-1, new Vector3(13.5f, -0.5f, 0f)));
        }

        if (_isHarvesting)
        {
            CanHarvestPlant();
            if (_plants.Count > 0)
            {
                MoveToNearestPlant();
            }
        }

        // tam thoi ok
        if (_replant && !_movingToPlant)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (_destroyArea.Count > 0)
                {
                    Vector3 plant = _destroyArea.OrderBy(p => Vector3.Distance(transform.position, p)).FirstOrDefault();
                    transform.position = plant;
                    _destroyArea.Remove(plant);
                }
                else
                {
                    _replant = false;
                    _isHarvesting = true;
                }
            }
        }

    }
    
    
    private IEnumerator MoveToStartPoint()
    {
        Vector3 targetPosition = new Vector3(13.5f, -0.5f, 0f);
        yield return MoveSmooth(targetPosition);
        MapManager.Instance.Dig(_pickCell.position, tileMap);
        _plantArea.Add(_pickCell.position);
        _moveToStart = true;
    }

    private IEnumerator MoveToDig(int direction, Vector3 endPosition)
    {
        while (!_hasDug)
        {
            if (transform.position == endPosition)
            {
                _hasDug = true;
                _stepCount = 0;
                _moveDir = 1;
                MapManager.Instance.Sow(_pickCell.position);
                yield break;
            }
            yield return Moving(direction);
            MapManager.Instance.Dig(_pickCell.position, tileMap);
            _plantArea.Add(_pickCell.position);
        }
    }

    private IEnumerator MoveToSow(int direction, Vector3 endPosition)
    {
        while (!_hasSowed)
        {
            if (transform.position == endPosition)
            {
                _hasSowed = true;
                _isHarvesting = true;
                yield break;
            }
            yield return Moving(direction);
            
            MapManager.Instance.Sow(_pickCell.position);
        }
    }
    
    private IEnumerator Moving(int direction)
    {
        Vector3 currentPosition = transform.position;
        Vector3 targetPosition;
        if (_stepCount < 11)
        {
            targetPosition = new Vector3(currentPosition.x + 1f * _moveDir, currentPosition.y, currentPosition.z);
            _stepCount++;
        }
        else
        {
            targetPosition = new Vector3(currentPosition.x, currentPosition.y - 1f * direction, currentPosition.z);
            _moveDir *= -1;
            _stepCount = 0;
        }

        yield return MoveSmooth(targetPosition);
    }

    private IEnumerator MoveSmooth(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01)
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
                if (plant != null && plant.isReadyToHarvest && !_plants.Contains(plant))
                {
                    _plants.Add(plant);
                }
            }
        }
    }

    private void MoveToNearestPlant()
    {
        _targetPlant = _plants.OrderBy(p => Vector3.Distance(transform.position, p.transform.position)).FirstOrDefault();

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

            _plants.Remove(_targetPlant);
            _targetPlant = null;
        }
    }
}
