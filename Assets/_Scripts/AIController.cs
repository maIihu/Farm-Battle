using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private int currentI = 0;
    private int currentJ = 0;
    private int moveDir = 1;

    private void Start()
    {
        MoveToStartPoint();
    }

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.K))
        {
            Move2();
        }
    }

    private void MoveToStartPoint()
    {
        this.transform.position = new Vector3(13.5f, -0.5f, 0f);
    }
    private void Move1()
    {
        Vector3 currentPosition = transform.position;
        if (currentI < 11)
        {
            transform.position = new Vector3(currentPosition.x + 1f * moveDir, currentPosition.y , currentPosition.z);
            currentI++;
        }
        else
        {
            transform.position = new Vector3(currentPosition.x, currentPosition.y - 1f, currentPosition.z);
            moveDir *= -1;
            currentI = 0;
        }
    }

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
        Debug.Log(currentJ);
    }
    
}