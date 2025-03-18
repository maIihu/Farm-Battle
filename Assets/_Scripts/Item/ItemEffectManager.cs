using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemEffectManager : MonoBehaviour
{
    [SerializeField] private GameObject thunderPrefab;
    [SerializeField] private GameObject tsunamiPrefab;
    [SerializeField] private GameObject shieldPrefab;
    [SerializeField] private GameObject nuttyPrefab;

    [SerializeField] private GameObject tileMap2;

    private bool _shieldEffect;
    
    public void GetEffect(string itemName)
    {
        switch (itemName)
        {
            case "Thunder":
                ThunderStart();
                break;
            case "Rain":
                RainStart();
                break;
            case "Tsunami":
                TsunamiStart();
                break;
            case "Shield":
                ShieldStart();
                break;
            case "Nutty":
                NuttyStart();
                break;
        }
    }

    private void NuttyStart() // 5.5, -5.5
    {
        GameObject nutty = Instantiate(nuttyPrefab, new Vector3(18.5f, -5.5f, 0f), Quaternion.identity, transform.GetChild(5));
        nutty.GetComponent<Nutty>().ItemEffect(tileMap2);
        Invoke(nameof(NuttyEnd), 100);
    }

    private void NuttyEnd()
    {
        Destroy(transform.GetChild(5).GetChild(0).gameObject);
    }
    
    private void ShieldStart()
    {
        Instantiate(shieldPrefab, new Vector3(6, -6, 0), Quaternion.identity, transform.GetChild(2));
        _shieldEffect = true;
        Invoke(nameof(ShieldEnd), 10f);
    }

    private void ShieldEnd()
    {
        Destroy(transform.GetChild(2).GetChild(0).gameObject);
        _shieldEffect = false;
    }

    private void ThunderStart()
    {
        for (int i = 0; i < 14; i++)
        {
            float x = Random.Range(13, 25) + 0.5f;  
            float y = Random.Range(-12, 0) + 0.5f; 
            Instantiate(thunderPrefab, new Vector3(x, y, 0), Quaternion.identity, transform.GetChild(0));
        }
        transform.GetChild(4).gameObject.SetActive(true);
        Invoke(nameof(ThunderEnd), 1f);
    }


    private void ThunderEnd()
    {
        transform.GetChild(4).gameObject.SetActive(false);
        Transform thunder = transform.GetChild(0);
        for (int i = thunder.childCount - 1; i >= 0; i--)
        {
            if (!_shieldEffect)
                thunder.GetChild(i).GetComponent<Thunder>().ItemEffect(tileMap2);
            Destroy(thunder.GetChild(i).gameObject);
        }
    }
    
    private void RainStart()
    {
        GameObject rain = transform.GetChild(1).gameObject;
        rain.GetComponent<ParticleSystem>().Play();
        Invoke(nameof(RainEnd), 10f);
    }
    
    private void RainEnd()
    {
        GameObject rain = transform.GetChild(1).gameObject;
        rain.GetComponent<ParticleSystem>().Stop();
    }

    private void TsunamiStart()
    {
        GameObject tsunami = Instantiate(tsunamiPrefab, new Vector3(19, -30, 0), Quaternion.identity);
        StartCoroutine(MoveTsunami(tsunami));
    }

    private IEnumerator MoveTsunami(GameObject tsunami)
    {
        Vector3 targetPosition = new Vector3(19, 4, 0);
        float speed = 18f;
        while (Vector3.Distance(tsunami.transform.position, targetPosition) > 0.1)
        {
            tsunami.transform.position =
                Vector3.MoveTowards(tsunami.transform.position, targetPosition, speed * Time.deltaTime);
            
            if (!_shieldEffect)
                tsunami.GetComponent<Tsunami>().ItemEffect(tileMap2);
        
            yield return null;
        }
        tsunami.transform.position = targetPosition;

        Destroy(tsunami);
    }
    
}
