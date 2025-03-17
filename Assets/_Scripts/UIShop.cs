using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIShop : MonoBehaviour
{
    [SerializeField] private GameObject shop1;
    [SerializeField] private GameObject shopItemTemplate;
    [SerializeField] private ItemEffectManager itemEffectManager;
    
    private List<GameObject> _items;
    private int _currentIndex;

    private GameObject _characterInShop;
    
    private void Start()
    {
        shop1.SetActive(false);
        _items = new List<GameObject>();
        CreateShop();
    }

    private void CreateShop()
    {
        CreateItemShop(Item.GetName(Item.ItemType.Shield),0, 0);
        CreateItemShop(Item.GetName(Item.ItemType.Rain),0, 1);
        CreateItemShop(Item.GetName(Item.ItemType.Exit),0, 2);

        CreateItemShop(Item.GetName(Item.ItemType.Tsunami),1, 0);
        CreateItemShop(Item.GetName(Item.ItemType.Nutty),1, 1);
        CreateItemShop(Item.GetName(Item.ItemType.Thunder),1, 2);
    }
    
    private void CreateItemShop(string text, int x, int y)
    {
        GameObject itemClone = Instantiate(shopItemTemplate, shop1.transform);
        itemClone.name = text;
        RectTransform rectTransform = itemClone.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(-40 + 80 * x, 110 - 80 * y);
        itemClone.GetComponentInChildren<TextMeshProUGUI>().text = text;
        itemClone.transform.GetChild(1).gameObject.SetActive(false);
        
        _items.Add(itemClone);
    }

    private void Update()
    {
        if (shop1.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.W)) MoveSelection(-1); 
            if (Input.GetKeyDown(KeyCode.S)) MoveSelection(1);  
            if (Input.GetKeyDown(KeyCode.A)) MoveSelection(-3); 
            if (Input.GetKeyDown(KeyCode.D)) MoveSelection(3);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                ApplyItem();
            }
            
        }
    }

    private void ApplyItem()
    {
        if (_items[_currentIndex].name == "Exit")
        {
            Exit();
            return;
        }
        itemEffectManager.GetEffect(_items[_currentIndex].name);
    }

    private void Exit()
    {
        _characterInShop.gameObject.GetComponent<PlayerController>().isShopping = false;
        foreach (var item in _items)
        {
            item.GetComponentInChildren<Image>().color = Color.white;
            item.transform.GetChild(1).gameObject.SetActive(false);
        }
        shop1.SetActive(false);
    }

    private void MoveSelection(int direction)
    {
        int row = _currentIndex % 3; // 0 1 2
        int col = _currentIndex / 3; // 0 1

        int newRow = row;
        int newCol = col;

        if (direction == -1 && row > 0) newRow--;
        if (direction == 1 && row < 2) newRow++;
        if (direction == -3 && col > 0) newCol--;
        if (direction == 3 && col < 1) newCol++;

        int newIndex = newCol * 3 + newRow;
        
        HighlightItem(newIndex);
        _currentIndex = newIndex;
    }

    private void HighlightItem(int index)
    {
        _items[_currentIndex].GetComponentInChildren<Image>().color = Color.white;
        _items[_currentIndex].transform.GetChild(1).gameObject.SetActive(false);
        
        _items[index].GetComponentInChildren<Image>().color = Color.yellow;
        _items[index].transform.GetChild(1).gameObject.SetActive(true);
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _characterInShop = other.gameObject;
            
            _characterInShop.GetComponent<PlayerController>().isShopping = true;
            shop1.SetActive(true);
            _currentIndex = 0;
            HighlightItem(0);
        }
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

        }
    }
}
