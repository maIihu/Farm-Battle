using System;
using UnityEngine;

public class BombController : MonoBehaviour
{
    public static event Action<Vector3> PositionBombExploded;
    public static event Action BotHasBomb;
    
    [SerializeField] private float timeToExplode = 6f;
    [SerializeField] private float moveSpeed = 10f;

    private Vector3 _lastPosition;
    private Vector3 _targetPosition;
    private bool _isMoving;
    
    public bool onTheLeft;
    
    private void Start()
    {
        Invoke(nameof(Explode), timeToExplode);
    }

    public void ThrowingBomb(Vector3 destination)
    { 
        onTheLeft = !onTheLeft;
        _targetPosition = destination;
        _isMoving = true;
    }

    private void Update()
    {
        if (_isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, _targetPosition) < 0.001f)
            {
                StopMoving();
            }
        }
    }

    private void StopMoving()
    {
        _isMoving = false;
        if(!onTheLeft)
            BotHasBomb?.Invoke();
    }

    private void Explode()
    {
        _lastPosition = transform.position;
        PositionBombExploded?.Invoke(_lastPosition);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Fence"))
        {
            gameObject.GetComponent<CircleCollider2D>().isTrigger = false;
            Invoke(nameof(EnableTrigger), 0.2f);
        }
    }

    private void EnableTrigger()
    {
        gameObject.GetComponent<CircleCollider2D>().isTrigger = true;
    }

}