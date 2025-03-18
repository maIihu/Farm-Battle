using System.Collections;
using UnityEngine;

public class Tsunami : ItemBase
{
    public override void ItemEffect(GameObject objectToEffect)
    {
        foreach (Transform child in objectToEffect.transform)
        {
            if (Mathf.Abs(child.position.y - transform.position.y) < 0.1f)
            {
                Destroy(child.gameObject);
            }
        }
    }
    

}