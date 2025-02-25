using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private GameObject pickCell;
    [SerializeField] private CropController crop;
    [SerializeField] private Tilemap tilemap;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    
    private Vector2 _moveInput;
    private Rigidbody2D _rb;
    public int Score { get; set; }
    public bool isHit;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Score = 0;
    }

    private void Update()
    {
        InputHandle();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            crop.Crop(this.transform.GetChild(0).transform.position, tilemap);
        }
        pickCell.transform.position = new Vector3((int)(this.transform.position.x) + 0.5f, 
            (int)(this.transform.position.y) - 0.5f, this.transform.position.z);
    }

    private void FixedUpdate()
    {
        _rb.velocity = _moveInput * moveSpeed;
    }

    private void InputHandle()
    {
        _moveInput = Vector2.zero;
        if (Input.GetKey(KeyCode.A))
            _moveInput.x = -1f;
        if (Input.GetKey(KeyCode.D))
            _moveInput.x = 1f;
        if (Input.GetKey(KeyCode.W))
            _moveInput.y = 1f;
        if (Input.GetKey(KeyCode.S))
            _moveInput.y = -1f;


        // animation of player
        _animator.SetFloat("Horizontal", _moveInput.x);
        _animator.SetFloat("Vertical", _moveInput.y);
        _animator.SetFloat("Speed", _moveInput.magnitude);
        if (_moveInput.x > 0)
        {
            _spriteRenderer.flipX = false;
        }
        else if (_moveInput.x < 0)
        {
            _spriteRenderer.flipX = true;
        }


        _moveInput.Normalize();


    }
}
