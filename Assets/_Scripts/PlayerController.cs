using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; } 

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Tilemap tileMap;
    
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private Vector2 _moveInput;
    private Rigidbody2D _rb;
    private Transform _pickCell;
    
    
    public int score;
    
    public static event Action<Vector3> OnBombThrown;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
            return;
        }

        Instance = this;

        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _pickCell = transform.GetChild(0);
    }

    private float _sowDelay = 0.24f;
    private float _lastDigTime = -1f; 
    
    private void Update()
    {
        InputHandle();
        _pickCell.position = new Vector3((int)(transform.position.x) + 0.5f, (int)(transform.position.y) - 0.5f, transform.position.z);
    }

    private void LateUpdate()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            bool hasDug = MapManager.Instance.Dig(_pickCell.position, tileMap);
            if (hasDug)
            {
                _lastDigTime = Time.time;
            }
            else if (Time.time - _lastDigTime >= _sowDelay) 
            {
                MapManager.Instance.Sow(_pickCell.position);
            }

            MapManager.Instance.Harvest(_pickCell.position, tileMap, ref score); 
        }
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
        
        SetAnimation();
        FlipCharacter();
        
        _moveInput.Normalize();
    }

    private void SetAnimation()
    {
        _animator.SetFloat("Horizontal", _moveInput.x);
        _animator.SetFloat("Vertical", _moveInput.y);
        _animator.SetFloat("Speed", _moveInput.magnitude);
    }

    private void FlipCharacter()
    {
        if (_moveInput.x > 0)
        {
            _spriteRenderer.flipX = false;
        }
        else if (_moveInput.x < 0)
        {
            _spriteRenderer.flipX = true;
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Bomb") && Input.GetKey(KeyCode.Space))
        {
            BombController bombClone = other.gameObject.GetComponent<BombController>();
            Vector3 des = new Vector3(Random.Range(14, 24), -11 - transform.position.y, 0);
            bombClone.ThrowingBomb(des);
            
            OnBombThrown?.Invoke(des);
        }
    }
}
