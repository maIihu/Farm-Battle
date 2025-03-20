using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    
    private bool _isHarvesting;

    private Transform _pickCell;
    private Dictionary<Vector3, Plant> _plants;
    private Plant _targetPlant;
    
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
        _plants = new Dictionary<Vector3, Plant>();
        
        StartCoroutine(MoveToStartPoint());
    }

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
        
        CanHarvestPlant();

        if (_isHarvesting && _plants.Count > 0)
            MoveToNearestPlant();

    }

    private IEnumerator MoveToStartPoint()
    {
        Vector3 targetPosition = new Vector3(13.5f, -0.5f, 0f);
        yield return MoveSmooth(targetPosition);
        MapManager.Instance.Dig(_pickCell.position, tileMap);
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
            yield return new WaitForSeconds(0.01f);
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
                if (plant != null && plant.isReadyToHarvest && !_plants.ContainsKey(child.transform.position))
                {
                    _plants.Add(child.transform.position, plant);
                }
            }
        }
    }

    private void MoveToNearestPlant()
    {
        _targetPlant = _plants.OrderBy(p => Vector3.Distance(transform.position, p.Key)).FirstOrDefault().Value;

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

            _plants.Remove(_targetPlant.transform.position);
            _targetPlant = null;
        }
    }
}
