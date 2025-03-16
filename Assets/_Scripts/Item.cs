using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item 
{
    public enum ItemType
    {
        Shield,
        Tsunami,
        Rain,
        Thunder,
        Wind
    }

    public static int GetCost(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Shield: return 50;
            case ItemType.Tsunami: return 60;
            case ItemType.Rain: return 70;
            case ItemType.Thunder: return 80;
            case ItemType.Wind: return 90;
        }
        return 0;
    }

    public static string GetDescribe(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Shield: return "Shield";
            case ItemType.Tsunami: return "Tsunami";
            case ItemType.Rain: return "Rain";
            case ItemType.Thunder: return "Thunder";
            case ItemType.Wind: return "Wind";
        }

        return null;
    }
    
}
