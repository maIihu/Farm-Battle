using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2 : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private GameObject pickCell;
    
    private Vector2 _moveInput;
    private Rigidbody2D _rb;
    public int score = 0;
    public bool isHit;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }
    
    private void Update()
    {
        InputHandle();
        isHit = Input.GetKeyDown(KeyCode.P);
        pickCell.transform.position = new Vector3((int)(this.transform.position.x) +0.5f, (int)(this.transform.position.y) - 0.5f, this.transform.position.z);
    }

    private void FixedUpdate()
    {
        _rb.velocity = _moveInput * moveSpeed;
    }

    private void InputHandle()
    {
        _moveInput = Vector2.zero;
        if (Input.GetKey(KeyCode.LeftArrow))
            _moveInput.x = -1f;
        if (Input.GetKey(KeyCode.RightArrow))
            _moveInput.x = 1f;
        if (Input.GetKey(KeyCode.UpArrow))
            _moveInput.y = 1f;
        if (Input.GetKey(KeyCode.DownArrow))
            _moveInput.y = -1f;
        
        _moveInput.Normalize();
        
    }
}
