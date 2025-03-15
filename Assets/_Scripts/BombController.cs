using System;
using UnityEngine;

public class BombController : MonoBehaviour
{
    public static event Action<Vector3> PositionBombExploded;
    public static event Action BombOnTheRight;
    
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
<<<<<<< HEAD
        _isMoving = false;
=======
        _rb.velocity = Vector2.zero; 
        _rb.angularVelocity = 0f; 
>>>>>>> parent of 38be943 (Update Bot)
        if(!onTheLeft)
            BombOnTheRight?.Invoke();
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
            Debug.Log("Va cham voi Fence");
            gameObject.GetComponent<CircleCollider2D>().isTrigger = false;
        }
    }



}