using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombController : MonoBehaviour
{
    [SerializeField] private float timeToExplode = 6f;
    public static event Action OnBombExploded;
    public bool onTheLeft;
    
    private void Start()
    {
        Invoke(nameof(Explode), timeToExplode);
    }

    private void Explode()
    {
        OnBombExploded?.Invoke();
        Destroy(this.gameObject);
    }
    
    public void ThrowingBomb(Vector3 end, float duration = 0.4f)
    {
        StartCoroutine(MoveBomb(end, duration));
        onTheLeft = !onTheLeft;
    }

    private IEnumerator MoveBomb(Vector3 end, float duration)
    {
        Vector3 start = transform.position;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(start, end, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = end; 
    }


}
