using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemEffectManager : MonoBehaviour
{
    [SerializeField] private GameObject thunderPrefab;
    [SerializeField] private GameObject lightningPrefab;
    [SerializeField] private GameObject tsunamiPrefab;
    [SerializeField] private GameObject shieldPrefab;
    [SerializeField] private GameObject nuttyPrefab;
    [SerializeField] private GameObject rainPrefab;
    
    [SerializeField] private GameObject tileMap1;
    [SerializeField] private GameObject tileMap2;

    private bool _shieldEffect1;
    private bool _shieldEffect2;
    
    public static event Action DestroyMap;
    
    public void GetEffect(string itemName, int player)
    {
        switch (itemName)
        {
            case "Thunder":
                ThunderStart(player);
                break;
            case "Rain":
                RainStart(player);
                break;
            case "Tsunami":
                TsunamiStart(player);
                break;
            case "Shield":
                ShieldStart(player);
                break;
            case "Nutty":
                NuttyStart(player);
                break;
        }
    }

    private void NuttyStart(int player)
    {
        Vector3 position;
        if (player == 1)
            position = new Vector3(18.5f, -5.5f, 0f);
        else
            position = new Vector3(5.5f, -5.5f, 0f);
        
        GameObject nutty = Instantiate(nuttyPrefab, position, Quaternion.identity, transform);
        nutty.GetComponent<Nutty>().ItemEffect(tileMap2);
        Invoke(nameof(NuttyEnd), 5);
    }

    private void NuttyEnd()
    {
        Destroy(transform.GetChild(0).gameObject);
    }
    
    private void ShieldStart(int player)
    {
        Vector3 position;
        
        if (player == 1)
        {
            _shieldEffect1 = true;
            position = new Vector3(6, -6, 0);
        }
        else
        {
            _shieldEffect2 = true;
            position = new Vector3(19, -6, 0);
        }
        
        Instantiate(shieldPrefab, position, Quaternion.identity, transform);
        
        Invoke(nameof(ShieldEnd), 10f);
    }

    private void ShieldEnd()
    {
        Destroy(transform.GetChild(0).gameObject);
        _shieldEffect1 = false;
        _shieldEffect2 = false;
    }

    private void ThunderStart(int player) 
    {
        for (int i = 0; i < 14; i++)
        {
            float x;  
            float y = Random.Range(-12, 0) + 0.5f;
            
            if (player == 1)
                x = Random.Range(13, 25) + 0.5f;  
            else
                x = Random.Range(0, 12) + 0.5f;
            
            Instantiate(thunderPrefab, new Vector3(x, y, 0), Quaternion.identity, transform);
        }
        
        if(player == 1)
            Instantiate(lightningPrefab, new Vector3(19, 0, 0), Quaternion.identity, transform);
        else
            Instantiate(lightningPrefab, new Vector3(6, 0, 0), Quaternion.identity, transform);

        StartCoroutine(ThunderEnd(player, 1));
    }
    
    private IEnumerator ThunderEnd(int player, float time)
    {
        yield return new WaitForSeconds(time);
        
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            if (!_shieldEffect1 && player == 2)
                if(transform.GetChild(i).GetComponent<Thunder>() != null)
                    transform.GetChild(i).GetComponent<Thunder>().ItemEffect(tileMap1);
            
            if(!_shieldEffect2 && player == 1)
                if(transform.GetChild(i).GetComponent<Thunder>() != null)
                    transform.GetChild(i).GetComponent<Thunder>().ItemEffect(tileMap2);
            
            Destroy(transform.GetChild(i).gameObject);
        }
        
        if(player == 1)
            DestroyMap?.Invoke();
    }
    
    private void RainStart(int player)
    {
        Vector3 position;
        if(player == 1)
            position = new Vector3(6, 4, 0);
        else
            position = new Vector3(19, 4, 0);

        GameObject rain = Instantiate(rainPrefab, position, Quaternion.Euler(90, 0, 0), transform);

        rain.gameObject.GetComponent<ParticleSystem>().Play();
        
        Invoke(nameof(RainEnd), 10f);
    }
    
    private void RainEnd()
    {
        transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().Stop();
        Destroy(transform.GetChild(0).gameObject);
    }

    private void TsunamiStart(int player)
    {
        Vector3 position, targetPosition;
        
        if (player == 1)
        {
            position = new Vector3(19, -30, 0);
            targetPosition = new Vector3(19, 4, 0);
        }
        else
        {
            position = new Vector3(6, -30, 0);
            targetPosition = new Vector3(6, 4, 0);
        }
            
        GameObject tsunami = Instantiate(tsunamiPrefab, position, Quaternion.identity, transform);
        
        StartCoroutine(MoveTsunami(tsunami, targetPosition, player));
    }

    private IEnumerator MoveTsunami(GameObject tsunami, Vector3 targetPosition, int player)
    {
        float speed = 18f;
        while (Vector3.Distance(tsunami.transform.position, targetPosition) > 0.1)
        {
            tsunami.transform.position =
                Vector3.MoveTowards(tsunami.transform.position, targetPosition, speed * Time.deltaTime);
            
            if (player == 1 && !_shieldEffect2)
                tsunami.GetComponent<Tsunami>().ItemEffect(tileMap2);
            if (player == 2 && !_shieldEffect1) 
                tsunami.GetComponent<Tsunami>().ItemEffect(tileMap1);
        
            yield return null;
        }
        tsunami.transform.position = targetPosition;

        Destroy(tsunami);
    }
    
}
