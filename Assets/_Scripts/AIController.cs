using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AIController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private CropController crop;
    [SerializeField] private Tilemap tilemap;
    
    private int currentI = 0;
    private int currentJ = 0;
    private int moveDir = 1;
    private bool isMoving = true;
    [SerializeField] private float time;
    private bool move3 = true;
    
    private void Start()
    {
        MoveToStartPoint();
        StartCoroutine(MoveRoutine());
    }
    
    private IEnumerator MoveRoutine()
    {
        while (isMoving)
        {
            Move2();
            yield return new WaitForSeconds(time);
        }
    }
    private bool hasEndedMove = false; 
    private bool hasPlanted = false; 

    private void Update()
    {
        Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);
        
        if (!hasEndedMove && currentPos == new Vector2(24.5f, -0.5f))
        {
            hasEndedMove = true; // Đánh dấu đã xử lý xong
            isMoving = false;
            this.transform.position = new Vector3(23.5f, -0.5f);
            crop.Crop(this.transform.GetChild(0).transform.position, tilemap);
            currentI = currentJ = 0;
            moveDir = 1;
            StartCoroutine(Move3Routine());
        }

        if (!hasPlanted && !move3)
        {
            hasPlanted = true; 
            this.transform.position = new Vector3(24.5f, -0.5f);
            crop.Crop(this.transform.GetChild(0).transform.position, tilemap);
        }
    }

    
    private void MoveToStartPoint()
    {
        this.transform.position = new Vector3(13.5f, -0.5f, 0f);
        crop.Crop(this.transform.GetChild(0).transform.position, tilemap);
    }
    
    // private void Move1()
    // {
    //     Vector3 currentPosition = transform.position;
    //     if (currentI < 11)
    //     {
    //         transform.position = new Vector3(currentPosition.x + 1f * moveDir, currentPosition.y , currentPosition.z);
    //         currentI++;
    //     }
    //     else
    //     {
    //         transform.position = new Vector3(currentPosition.x, currentPosition.y - 1f, currentPosition.z);
    //         moveDir *= -1;
    //         currentI = 0;
    //     }
    // }

    private void Move2()
    {
        Vector3 currentPosition = transform.position;
        
        if (currentJ < 11)
        {
            transform.position = new Vector3(currentPosition.x, currentPosition.y - 1f * moveDir, currentPosition.z);
            currentJ++;
        }
        else
        {
            transform.position = new Vector3(currentPosition.x + 1f, currentPosition.y , currentPosition.z);
            moveDir *= -1;
            currentJ = 0;
        }
        crop.Crop(this.transform.GetChild(0).transform.position, tilemap);
    }

    private void Move3()
    {
        Vector3 currentPosition = transform.position;
        if(currentPosition.x == 24.5f && currentPosition.y == -1.5f) move3 = false;
        if (currentPosition.y == -0.5f)
        {
            transform.position = new Vector3(currentPosition.x - 1f, currentPosition.y, currentPosition.z);
            if (currentPosition.x == 13.5f)
                transform.position = new Vector3(currentPosition.x, currentPosition.y - 1f, currentPosition.z);
        }
        else
        {
            if (currentJ < 10)
            {
                transform.position = new Vector3(currentPosition.x, currentPosition.y - 1f * moveDir, currentPosition.z);
                currentJ++;
            }
            else
            {
                transform.position = new Vector3(currentPosition.x + 1f, currentPosition.y , currentPosition.z);
                moveDir *= -1;
                currentJ = 0;
            }
        }
        crop.Crop(this.transform.GetChild(0).transform.position, tilemap);
        
    }
    private IEnumerator Move3Routine()
    {
        while (move3) 
        {
            Move3();
            
            yield return new WaitForSeconds(time); 
        }
    }

}