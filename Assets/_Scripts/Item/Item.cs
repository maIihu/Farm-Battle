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
        Nutty,
        Exit
    }

    public static int GetCost(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Shield: return 30;
            case ItemType.Tsunami: return 40;
            case ItemType.Rain: return 20;
            case ItemType.Thunder: return 15;
            case ItemType.Nutty: return 30;
        }
        return 0;
    }

    public static string GetName(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Shield: return "Shield";
            case ItemType.Tsunami: return "Tsunami";
            case ItemType.Rain: return "Rain";
            case ItemType.Thunder: return "Thunder";
            case ItemType.Nutty: return "Nutty";
            case ItemType.Exit: return "Exit";
        }
        return null;
    }

    public static string GetDescription(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Shield: return "Bảo vệ bạn một lần";
            case ItemType.Tsunami: return "Phá hủy vườn đối thủ";
            case ItemType.Rain: return "Hạt giống phát triển nhanh";
            case ItemType.Thunder: return "Phá hủy 15 ô đối thủ";
            case ItemType.Nutty: return "Ăn hạt giống đối thủ";
            case ItemType.Exit: return "Thoát";
        }
        return null;
    }
    
}
