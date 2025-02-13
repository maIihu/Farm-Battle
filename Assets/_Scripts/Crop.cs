using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    public bool isReadyToHarvest = false;
    [SerializeField] private float growTimer = 5f;
    [SerializeField] private Sprite[] _sprites;
    
    private void Start()
    {
        StartCoroutine(Grow());
        
    }
    

    private IEnumerator Grow()
    {
        this.gameObject.GetComponent<SpriteRenderer>().sprite = _sprites[0];
        for (int i = 1; i < _sprites.Length; i++)
        {
            yield return new WaitForSeconds(growTimer);
            this.gameObject.GetComponent<SpriteRenderer>().sprite = _sprites[i];
        }
        
        isReadyToHarvest = true;
        Debug.Log("Cây " + this.transform.position + " đã chín");
        yield return new WaitUntil(() => !isReadyToHarvest); // Chờ cho đến khi thu hoạch
        
    }

    public void Harvest()
    {
        Debug.Log("Đã thu hoạch");
        isReadyToHarvest = false;
        StartCoroutine(Grow()); // Sau khi thu hoạch, cây lại phát triển lại từ đầu
        
    }
}
