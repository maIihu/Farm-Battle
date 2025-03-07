using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Plant : MonoBehaviour
{
    public bool isReadyToHarvest = false;
    public Vector3 gridLocation;
    [SerializeField] private float growTimer = 5f;
    [SerializeField] private Sprite[] sprites;
    
    private void Start()
    {
        StartCoroutine(Grow());
        
    }
    
    private IEnumerator Grow()
    {
        this.gameObject.GetComponent<SpriteRenderer>().sprite = sprites[0];
        for (int i = 1; i < sprites.Length; i++)
        {
            yield return new WaitForSeconds(growTimer);
            this.gameObject.GetComponent<SpriteRenderer>().sprite = sprites[i];
        }
        
        isReadyToHarvest = true;
        //Debug.Log("Cây " + this.transform.position + " đã chín");
        yield return new WaitUntil(() => !isReadyToHarvest); 
        
    }

    public void Harvest()
    {
        //Debug.Log("Đã thu hoạch");
        isReadyToHarvest = false;
        StartCoroutine(Grow()); 
        
    }
}
