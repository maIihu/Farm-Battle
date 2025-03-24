using System.Collections;
using UnityEngine;

public class Rain : ItemBase
{
    public override void ItemEffect(GameObject objectToEffect)
    {
        foreach (Transform child in objectToEffect.transform)
        {
            Plant plant = child.GetComponentInChildren<Plant>();
            if (plant != null)
            {
                plant.growTimer /= 5;
            }
        }
    }

    public void RollBackTimeGrow(GameObject objectToEffect)
    {
        foreach (Transform child in objectToEffect.transform)
        {
            Plant plant = child.GetComponentInChildren<Plant>();
            if (plant != null)
            {
                plant.growTimer = Random.Range(40, 60) / 5;
            }
        }
    }
    
}