using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    [SerializeField] private GameObject shop1;
    [SerializeField] private GameObject shopItemTemplate;

    private void Start()
    {
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                CreateItemShop(i, j);
            }
        }
    }

    private void CreateItemShop(int x, int y)
    {
        GameObject itemClone = Instantiate(shopItemTemplate, shop1.transform);
        RectTransform rectTransform = itemClone.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(-40 + 80 * x, 110 - 80 * y);
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            shop1.SetActive(true);
        }
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            shop1.SetActive(false);
        }
    }
}
