using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShop : MonoBehaviour
{
    [SerializeField] private GameObject shop1;
    [SerializeField] private GameObject shopItemTemplate;
    private List<GameObject> _items;
    private int _currentIndex;
    private void Start()
    {
        shop1.SetActive(false);
        _items = new List<GameObject>();
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
        
        _items.Add(itemClone);
    }

    private void Update()
    {
        if (shop1.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveSelection(1);
            }
        }
    }

    private void MoveSelection(int direction)
    {
        int newIndex = _currentIndex + direction;
        

            HighlightItem(newIndex);
        
    }

    private void HighlightItem(int index)
    {
        _items[index].GetComponentInChildren<Image>().color = Color.yellow;
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            shop1.SetActive(true);
            _currentIndex = 0;
            HighlightItem(0);
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
