using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Mouse : ItemBase
{
    private Dictionary<Vector3, Plant> _plants;
    private Plant _targetPlant;
    private GameObject _tileMap;
    private bool _moveToPlant;
    
    public float moveSpeed = 5f;
    public static event Action<Vector3> PlantDestroyed;
    
    private void Start()
    {
        _plants = new Dictionary<Vector3, Plant>();
        FindPlant();
    }

    private void Update()
    {
        if (_plants.Count > 0 && !_moveToPlant)
        {
            MoveToPlant();
        }

        if (_moveToPlant && _targetPlant != null)
        {
            MoveTowardsTarget();
        }
    }

    private void MoveToPlant()
    {
        if (_plants.Count == 0) return;

        int randomIndex = Random.Range(0, _plants.Count);
        _targetPlant = _plants.ElementAt(randomIndex).Value;

        if (_targetPlant != null)
        {
            _moveToPlant = true;
        }
    }

    private void MoveTowardsTarget()
    {
        if (_targetPlant == null) return;

        transform.position = Vector3.MoveTowards(transform.position, _targetPlant.transform.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, _targetPlant.transform.position) < 0.1f)
        {
            if (_targetPlant != null)
            {
                Vector3 position = _targetPlant.transform.position;
                _plants.Remove(_targetPlant.transform.position);
                Destroy(_targetPlant.gameObject);
                _targetPlant = null;
                if(_tileMap.name == "Garden2")
                    PlantDestroyed?.Invoke(position);
            }
            _moveToPlant = false;
        }
        
    }
    

    private void FindPlant()
    {
        foreach (Transform child in _tileMap.transform)
        {
            if (child.childCount > 0)
            {
                Transform plant = child.GetChild(0);
                _plants.Add(plant.position, plant.GetComponent<Plant>());
            }
        }
    }
    
    public override void ItemEffect(GameObject objectToEffect)
    {
        _tileMap = objectToEffect;
    }
}
