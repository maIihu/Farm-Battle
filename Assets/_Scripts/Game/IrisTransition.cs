using System;
using UnityEngine;
using System.Collections;

public class IrisTransition : MonoBehaviour
{
    public Material irisMaterial;
    public float duration = 1f;
    private bool isClosing;
    
    void OnEnable()
    {
        StartCoroutine(AnimateIris(isClosing ? 1f : 0f, isClosing ? 0f : 1f));
        isClosing = true;
    }

    IEnumerator AnimateIris(float start, float end)
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            float value = Mathf.Lerp(start, end, t / duration);
            irisMaterial.SetFloat("_Radius", value);
            yield return null;
        }
        irisMaterial.SetFloat("_Radius", end);
        gameObject.SetActive(false);
    }
}