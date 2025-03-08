using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombController : MonoBehaviour
{
    [SerializeField] private float timeToExplode = 6f;

    public static event Action OnBombExploded;
    

    private void Start()
    {
        Invoke(nameof(Explode), timeToExplode);
    }

    private void Explode()
    {
        OnBombExploded?.Invoke();
        Destroy(this.gameObject);
    }

    public void ThrowingBomb(Vector3 end)
    {
        this.transform.position = end;
    }
}
