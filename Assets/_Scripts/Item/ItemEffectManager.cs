using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemEffectManager : MonoBehaviour
{
    [SerializeField] private GameObject tileMap1;
    [SerializeField] private GameObject tileMap2;

    [SerializeField] private List<GameObject> effectPrefabs;

    private Dictionary<string, Transform> _effects;
    private Dictionary<string, GameObject> _effectPrefabs;

    private bool _shieldEffectActive1;
    private bool _shieldEffectActive2;

    public static event Action<List<Vector3>> DestroyMap;
    public static event Action<int> isRaining;
    
    
    private void Start()
    {
        _effects = new Dictionary<string, Transform>();
        _effectPrefabs = new Dictionary<string, GameObject>();
        
        foreach (var prefab in effectPrefabs)
        {
            _effectPrefabs[prefab.name] = prefab;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            _effects[transform.GetChild(i).name] = transform.GetChild(i);
        }
    }
    
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
        
        GameObject nutty = Instantiate(_effectPrefabs["Nutty"], position, Quaternion.identity, _effects["NuttyEffect"]);
        nutty.GetComponent<Nutty>().ItemEffect(tileMap2);
        Invoke(nameof(NuttyEnd), 5);
    }

    private void NuttyEnd()
    {
        Destroy(_effects["NuttyEffect"].GetChild(0).gameObject);
    }
    
    private void ShieldStart(int player)
    {
        Vector3 position;
        
        if (player == 1)
        {
            _shieldEffectActive1 = true;
            position = new Vector3(6, -6, 0);
        }
        else
        {
            _shieldEffectActive2 = true;
            position = new Vector3(19, -6, 0);
        }
        
        Instantiate(_effectPrefabs["Shield"], position, Quaternion.identity, _effects["ShieldEffect"]);
        
        Invoke(nameof(ShieldEnd), 10f);
    }

    private void ShieldEnd()
    {
        Destroy(_effects["ShieldEffect"].GetChild(0).gameObject);
        _shieldEffectActive1 = false;
        _shieldEffectActive2 = false;
    }

    private void ThunderStart(int player)
    {
        Transform container = _effects["ThunderEffect"];
        for (int i = 0; i < 14; i++)
        {
            float x;  
            float y = Random.Range(-12, 0) + 0.5f;
            
            if (player == 1)
                x = Random.Range(13, 25) + 0.5f;  
            else
                x = Random.Range(0, 12) + 0.5f;
            
            Instantiate(_effectPrefabs["Thunder"], new Vector3(x, y, 0), Quaternion.identity, container);
        }
        
        if(player == 1)
            Instantiate(_effectPrefabs["Lightning"], new Vector3(19, 0, 0), Quaternion.identity, container);
        else
            Instantiate(_effectPrefabs["Lightning"], new Vector3(6, 0, 0), Quaternion.identity, container);

        StartCoroutine(ThunderEnd(player, 1, container));
    }
    
    private IEnumerator ThunderEnd(int player, float time, Transform container)
    {
        yield return new WaitForSeconds(time);

        List<Vector3> plantsDestroyed = new List<Vector3>();
        
        for (int i = container.childCount - 1; i >= 0; i--)
        {
            if (!_shieldEffectActive1 && player == 2)
                if(container.GetChild(i).GetComponent<Thunder>() != null)
                    container.GetChild(i).GetComponent<Thunder>().ItemEffect(tileMap1);
            
            if(!_shieldEffectActive2 && player == 1)
                if(container.GetChild(i).GetComponent<Thunder>() != null)
                {
                    container.GetChild(i).GetComponent<Thunder>().ItemEffect(tileMap2);
                    plantsDestroyed.Add(container.GetChild(i).position);
                }
            
            Destroy(container.GetChild(i).gameObject);
        }
        
        if(plantsDestroyed.Count > 0)
            DestroyMap?.Invoke(plantsDestroyed);
    }
    
    private void RainStart(int player)
    {
        Vector3 position;
        GameObject tileMapTarget;
        
        if(player == 1)
        {
            position = new Vector3(6, 4, 0);
            tileMapTarget = tileMap1;
        }
        else
        {
            position = new Vector3(19, 4, 0);
            tileMapTarget = tileMap2;
        }

        GameObject rain = Instantiate(_effectPrefabs["Rain"], position, Quaternion.Euler(90, 0, 0), _effects["RainEffect"]);

        rain.gameObject.GetComponent<ParticleSystem>().Play();
        //rain.gameObject.GetComponent<Rain>().ItemEffect(tileMapTarget);
        isRaining?.Invoke(player);
        StartCoroutine(RainEnd(tileMapTarget, player, 10f));
    }
    
    private IEnumerator RainEnd(GameObject tileMapTarget, int player, float time)
    {
        yield return new WaitForSeconds(time);
        _effects["RainEffect"].GetChild(0).gameObject.GetComponent<ParticleSystem>().Stop();
        
        isRaining?.Invoke(player);
        
        MapManager.Instance.DeBuffGrowTime(tileMapTarget.transform);
        
        Destroy(_effects["RainEffect"].GetChild(0).gameObject);
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
            
        GameObject tsunami = Instantiate(_effectPrefabs["Tsunami"], position, Quaternion.identity, _effects["TsunamiEffect"]);
        
        StartCoroutine(MoveTsunami(tsunami, targetPosition, player));
    }

    private IEnumerator MoveTsunami(GameObject tsunami, Vector3 targetPosition, int player)
    {
        float speed = 18f;
        
        List<Vector3> plantsDestroyed = new List<Vector3>();
        foreach (Transform child in tileMap2.transform)
        {
            plantsDestroyed.Add(child.position);
        }
        
        while (Vector3.Distance(tsunami.transform.position, targetPosition) > 0.1)
        {
            tsunami.transform.position =
                Vector3.MoveTowards(tsunami.transform.position, targetPosition, speed * Time.deltaTime);
            
            if (player == 1 && !_shieldEffectActive2)
                tsunami.GetComponent<Tsunami>().ItemEffect(tileMap2);
            if (player == 2 && !_shieldEffectActive1) 
                tsunami.GetComponent<Tsunami>().ItemEffect(tileMap1);
        
            yield return null;
        }
        
        tsunami.transform.position = targetPosition;

        Destroy(tsunami);
        
        if (player == 1 && !_shieldEffectActive2)
        {
            DestroyMap?.Invoke(plantsDestroyed);
        }

    }
    
}
