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

    public void GetEffect(string itemName)
    {
        switch (itemName)
        {
            case "Thunder":
                Thunder();
                break;
            case "Rain":
                Rain();
                break;
            case "Tsunami":
                Tsunami();
                break;
            case "Shield":
                Shield();
                break;
        }
    }

    private void Shield()
    {
        Instantiate(shieldPrefab, new Vector3(6, -6, 0), Quaternion.identity, transform.GetChild(2));
        Invoke(nameof(ShieldEnd), 5f);
    }

    private void ShieldEnd()
    {
        Destroy(transform.GetChild(2).GetChild(0).gameObject);
    }

    private void Thunder()
    {
        for (int i = 0; i < 14; i++)
        {
            int x = Random.Range(13, 24);
            int y = Random.Range(-12, -1);
            Instantiate(thunderPrefab, new Vector3(x, y, 0), Quaternion.identity, transform.GetChild(0));
        }
        Invoke(nameof(ThunderEnd), 5f);
    }

    private void ThunderEnd()
    {
        Transform thunder = transform.GetChild(0);
        for (int i = thunder.childCount - 1; i >= 0; i--)
        {
            Destroy(thunder.GetChild(i).gameObject);
        }
    }
    
    private void Rain()
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

    private void Tsunami()
    {
        GameObject tsunami = Instantiate(tsunamiPrefab, new Vector3(19, -30, 0), Quaternion.identity, transform.GetChild(2));
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
            yield return null;
        }
        tsunami.transform.position = targetPosition;
        Destroy(tsunami);
    }
    
}
