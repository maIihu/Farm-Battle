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
    
    [SerializeField] private Tilemap tileMap;
    [SerializeField] private float moveSpeed = 5f;
    
    private int _stepCount;
    private int _moveDir = 1;
    
    private bool _hasDug;
    private bool _hasSowed;
    private bool _isHarvesting;
    private bool _isChasingBomb;
    
    private Vector3 _bombPosition;
    private Transform _pickCell;
    private KeyValuePair<Vector3, Plant>? _targetPlant;
    private Dictionary<Vector3, Plant> _plants;
    private List<Vector3> _destroyedAreas;
    
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
        _plants = new Dictionary<Vector3, Plant>();
        _targetPlant = new KeyValuePair<Vector3, Plant>();
        _pickCell = transform.GetChild(0);
        
        MoveToStartPoint();
        StartCoroutine(MoveToDigRoutine());
        
        BombManager.OnBombExploded += HandleDestroyedAreas;
        BombManager.SpawnBombOnTheRight += MoveToBomb;
        BombController.BombOnTheRight += MoveToBomb;
    }
    
    private void MoveToStartPoint()
    {
        transform.position = new Vector3(13.5f, -0.5f, 0f);
        MapManager.Instance.Dig(_pickCell.position, tileMap);
    }
    
    private void OnDestroy()
    {
        BombManager.OnBombExploded -= HandleDestroyedAreas;
        BombManager.SpawnBombOnTheRight -= MoveToBomb;
        BombController.BombOnTheRight -= MoveToBomb;
    }
    
    private void HandleDestroyedAreas(List<Vector3> destroyedPositions)
    {
        _isChasingBomb = false;
        _destroyedAreas = new List<Vector3>();
        _destroyedAreas.AddRange(destroyedPositions);
        foreach (var dir in _destroyedAreas)
        {
            if (_plants.ContainsKey(dir))
            {
                _plants.Remove(dir);
            }
        }
        _isHarvesting = false;
        StartCoroutine(ReplantCrops());
    }

    private IEnumerator ReplantCrops()
    {
        _targetPlant = null;
        while (_destroyedAreas.Count > 0)
        {
            Vector3 targetPosition = _destroyedAreas[0];
            _destroyedAreas.RemoveAt(0);

            while (Vector3.Distance(transform.position, targetPosition) > 0f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
            MapManager.Instance.Dig(_pickCell.position, tileMap);
            MapManager.Instance.Sow(_pickCell.position);
        }
        _isHarvesting = true;
    }
    
    private void MoveToBomb()
    {
        GameObject bomb = GameObject.FindGameObjectWithTag("Bomb");
        if (bomb)
        {
            _bombPosition = bomb.transform.position;
        }
        
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
            
            StartCoroutine(MoveToPositionLerp(new Vector3(23.5f, -0.5f)));
            MapManager.Instance.Sow(_pickCell.position);
            
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
        
        if (_isHarvesting)
        {
            MoveToNearestPlant();
        }
    }
    
    private void KickBomb()
    {
        GameObject bomb = GameObject.FindGameObjectWithTag("Bomb");
        if (bomb)
        {
            float randomAngle;
            if (bomb.transform.position.y < -6)
                randomAngle = Random.Range(-30f, 0f);
            else
                randomAngle = Random.Range(0f, 30f);
                
            
            Quaternion rotation = Quaternion.Euler(0, 0, randomAngle);
            bomb.GetComponent<BombController>().ThrowingBomb(rotation * Vector3.left);
        }
        _isChasingBomb = false;
        _isHarvesting = true;
    }
    
    private void MoveToNearestPlant()
    {
        if (_plants.Count == 0) return;
        _targetPlant = _plants.OrderBy(p => Vector3.Distance(transform.position, p.Key)).FirstOrDefault();
        
        if (_targetPlant == null) return;

        transform.position = Vector3.MoveTowards(transform.position, _targetPlant.Value.Key, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, _targetPlant.Value.Key) < 0.01f) 
        {
            _targetPlant.Value.Value.Harvest();
            score++;
            _plants.Remove(_targetPlant.Value.Key);
            _targetPlant = null;
        }
    }

    private IEnumerator MoveToDigRoutine()
    {
        while (!_hasDug)
        {
            yield return StartCoroutine(MoveToDig());
        }
    }
    
    private IEnumerator MoveToDig()
    {
        Vector3 currentPosition = transform.position;
        Vector3 targetPosition;
        
        if (_stepCount < 11)
        {
            targetPosition = new Vector3(currentPosition.x, currentPosition.y - 1f * _moveDir, currentPosition.z);
            _stepCount++;
        }
        else
        {
            targetPosition = new Vector3(currentPosition.x + 1f, currentPosition.y , currentPosition.z);
            _moveDir *= -1;
            _stepCount = 0;
        }

        yield return MoveToPositionLerp(targetPosition);
        MapManager.Instance.Dig(_pickCell.position, tileMap);
        
    }

    private IEnumerator MoveToPositionLerp(Vector3 targetPosition)
    {
        Vector3 startPosition = transform.position;
        float distance = Vector3.Distance(targetPosition, startPosition);
        float elapsedTime = 0f;
        while (elapsedTime * moveSpeed < distance)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, (elapsedTime * moveSpeed) / distance);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }

    private IEnumerator MoveToSowRoutine()
    {
        while (!_hasSowed)
        {
            yield return StartCoroutine(MoveToSow());
        }
    }
    
    private IEnumerator MoveToSow()
    {
        Vector3 currentPosition = transform.position;
        Vector3 targetPosition;
        
        if(currentPosition.x == 24.5f && currentPosition.y == -1.5f)
        {
            _hasSowed = true;
            targetPosition = new Vector3(24.5f, -0.5f);
            yield return MoveToPositionLerp(targetPosition);
            
            MapManager.Instance.Sow(_pickCell.position);
            _isHarvesting = true;
            yield break;
        }
        if (currentPosition.y == -0.5f)
        {
            targetPosition = new Vector3(currentPosition.x - 1f, currentPosition.y, currentPosition.z);
            yield return MoveToPositionLerp(targetPosition);
            
            if (currentPosition.x == 13.5f)
            {
                targetPosition = new Vector3(currentPosition.x, currentPosition.y - 1f, currentPosition.z);
                yield return MoveToPositionLerp(targetPosition);
            }
        }
        else
        {
            if (_stepCount < 10)
            {
                targetPosition = new Vector3(currentPosition.x, currentPosition.y - 1f * _moveDir, currentPosition.z);
                _stepCount++;
            }
            else
            {
                targetPosition = new Vector3(currentPosition.x + 1f, currentPosition.y , currentPosition.z);
                _moveDir *= -1;
                _stepCount = 0;
            }
            yield return MoveToPositionLerp(targetPosition);
        }
        MapManager.Instance.Sow(_pickCell.position);
    }
    
    private void CanHarvestPlant()
    {
        for (int i = 0; i < tileMap.transform.childCount; i++)
        {
            Transform child = tileMap.transform.GetChild(i);
            if (child.childCount > 0)
            {
                Plant plant = child.GetChild(0).gameObject.GetComponent<Plant>();
                if (plant != null && plant.isReadyToHarvest && !_plants.ContainsKey(child.transform.position))
                {
                    _plants.Add(child.transform.position, plant);
                }
            }
        }
    }
    
}