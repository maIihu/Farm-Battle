using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Nutty : ItemBase
{
    private Dictionary<Vector3, Plant> _plants = new Dictionary<Vector3, Plant>();
    private Plant _targetPlant;
    public float moveSpeed = 5f;

    private GameObject tileMap;

    private void Start()
    {
        StartCoroutine(UpdatePlantList());
    }

    private void Update()
    {
        MoveToNearestPlant();
    }

    /// <summary>
    /// Liên tục cập nhật danh sách cây sẵn sàng để thu hoạch
    /// </summary>
    private IEnumerator UpdatePlantList()
    {
        while (true)
        {
            FindPlantsInTileMap();
            yield return new WaitForSeconds(1f); 
        }
    }

    private void FindPlantsInTileMap()
    {
        foreach (Transform child in tileMap.transform)
        {
            if (child.childCount > 0)
            {
                Plant plant = child.GetChild(0).gameObject.GetComponent<Plant>();
                if (plant != null && plant.isReadyToHarvest)
                {
                    _plants[child.transform.position] = plant;
                }
            }
        }
    }

    private void MoveToNearestPlant()
    {
        if (_plants.Count == 0) return;

        _targetPlant = 
            _plants.OrderBy(p => Vector3.Distance(transform.position, p.Key)).FirstOrDefault().Value;

        if (_targetPlant == null) return;

        transform.position = Vector3.MoveTowards(transform.position, _targetPlant.transform.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, _targetPlant.transform.position) < 0.1f)
        {
            if (_targetPlant.isReadyToHarvest) 
            {
                _targetPlant.Harvest();
                Debug.Log("Nutty đã ăn một quả!");
            }
            _plants.Remove(_targetPlant.transform.position);
            _targetPlant = null;
        }
    }

    public override void ItemEffect(GameObject objectToEffect)
    {
        tileMap = objectToEffect;
    }
}
