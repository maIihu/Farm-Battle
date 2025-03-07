using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class BotController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private CropManager crop;
    [SerializeField] private Tilemap tileMap;
    [SerializeField] private float time;
    
    private int _stepCount = 0;
    private int _moveDir = 1;
    
    private bool _isMoving = true;
    private bool _move3 = true;
    private bool _hasEndedMove = false; 
    private bool _hasPlanted = false;
    
    private Plant _targetPlant;

    private List<Plant> _plants;
    
    private void Start()
    {
        _plants = new List<Plant>();
        
        MoveToStartPoint();
        StartCoroutine(MoveToDigRoutine());
    }
    
    private void Update()
    {
        Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);
        
        if (!_hasEndedMove && currentPos == new Vector2(24.5f, -0.5f))
        {
            _hasEndedMove = true; 
            _isMoving = false;
            this.transform.position = new Vector3(23.5f, -0.5f);
            crop.Crop(this.transform.GetChild(0).transform.position, tileMap);
            _stepCount = 0;
            _moveDir = 1;
            StartCoroutine(MoveToSeedRoutine());
        }

        if (!_hasPlanted && !_move3)
        {
            _hasPlanted = true; 
            this.transform.position = new Vector3(24.5f, -0.5f);
            crop.Crop(this.transform.GetChild(0).transform.position, tileMap);
        }

        if (!_move3)
        {
            CanHarvestPlant();
            MoveToPlant();
        }

    }
    
    private void MoveToStartPoint()
    {
        this.transform.position = new Vector3(13.5f, -0.5f, 0f);
        crop.Crop(this.transform.GetChild(0).transform.position, tileMap);
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
        crop.Crop(this.transform.GetChild(0).transform.position, tileMap);
    }

    private IEnumerator MoveToDigRoutine()
    {
        while (_isMoving)
        {
            MoveToDig();
            yield return new WaitForSeconds(time);
        }
    }
    
    private void MoveToSeed()
    {
        Vector3 currentPosition = transform.position;
        if(currentPosition.x == 24.5f && currentPosition.y == -1.5f) _move3 = false;
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
        crop.Crop(this.transform.GetChild(0).transform.position, tileMap);
    }
    
    private IEnumerator MoveToSeedRoutine()
    {
        while (_move3) 
        {
            MoveToSeed();
            
            yield return new WaitForSeconds(time); 
        }
    }

    private void CanHarvestPlant()
    {
        for (int i = 0; i < tileMap.transform.childCount; i++)
        {
            Transform plantChild = tileMap.transform.GetChild(i);
            Plant plant = plantChild.gameObject.GetComponent<Plant>();
            if (plant.isReadyToHarvest && !_plants.Contains(plant))
            {
                _plants.Add(plant);
            }
        }
    }

    private void MoveToPlant()
    {
        if (_plants.Count == 0 || _targetPlant != null)
            return;

        _targetPlant = _plants.OrderBy(p => Vector3.Distance(transform.position, p.gridLocation)).First();
        StartCoroutine(MoveToTarget(_targetPlant));
    }

    private IEnumerator MoveToTarget(Plant target)
    {
        while (Vector3.Distance(transform.position, target.gridLocation) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.gridLocation, moveSpeed * Time.deltaTime);
            yield return null;
        }

        if (target.isReadyToHarvest)
        {
            target.Harvest();
            _plants.Remove(target);
        }

        _targetPlant = null; 
    }

    
}