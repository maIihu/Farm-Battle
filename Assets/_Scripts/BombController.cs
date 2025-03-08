using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombController : MonoBehaviour
{
    private const float TimeToExplode = 6f;

    public static event Action OnBombExploded;
    

    private void Start()
    {
        Invoke(nameof(Explode), TimeToExplode);
    }

    private void Explode()
    {
        OnBombExploded?.Invoke();
        Destroy(this.gameObject);
    }
}
