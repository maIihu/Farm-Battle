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
    [SerializeField] private Tilemap tileMap;
    [SerializeField] private float time;
    
    private int _stepCount;
    private int _moveDir = 1;
    
    // private bool _isDigging = true;
    // private bool _isSowing = true;
    
    private bool _hasDug;
    private bool _hasSowed;
    private bool _isHarvesting;
    private bool _isChasingBomb;
    
    private KeyValuePair<Vector3, Plant>? _targetPlant;
    private Dictionary<Vector3, Plant> _plants;
    private List<Vector3> _destroyedAreas;
    
    public int score;
    private Vector3 _bombPosition;
    
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
        
        MoveToStartPoint();
        StartCoroutine(MoveToDigRoutine());
        
        PlayerController.OnBombThrown += MoveToBomb;
        BombManager.OnBombExploded += HandleDestroyedAreas;
    }
    
    private void MoveToStartPoint()
    {
        transform.position = new Vector3(13.5f, -0.5f, 0f);
        MapManager.Instance.Dig(transform.GetChild(0).transform.position, tileMap);
    }
    
    private void OnDestroy()
    {
        PlayerController.OnBombThrown -= MoveToBomb; 
        BombManager.OnBombExploded -= HandleDestroyedAreas;
    }
    
    private void HandleDestroyedAreas(List<Vector3> destroyedPositions)
    {
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

            MapManager.Instance.Dig(transform.GetChild(0).transform.position, tileMap);
            MapManager.Instance.Sow(transform.GetChild(0).transform.position);
        }
        _isHarvesting = true;
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
            MapManager.Instance.Sow(transform.GetChild(0).transform.position);
            
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
            Vector3 des = new Vector3(Random.Range(1, 11), - 11 - transform.position.y, 0);
            bomb.GetComponent<BombController>().ThrowingBomb(des);
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
        MapManager.Instance.Dig(transform.GetChild(0).transform.position, tileMap);
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
            MapManager.Instance.Sow(transform.GetChild(0).transform.position);
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
        MapManager.Instance.Sow(transform.GetChild(0).transform.position);
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