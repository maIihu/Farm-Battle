using System;
using UnityEngine;

public class BombController : MonoBehaviour
{
    public static event Action<Vector3> PositionBombExploded;
    public static event Action BombOnTheRight;
    
    [SerializeField] private float timeToExplode = 6f;
    [SerializeField] private float throwForce = 5f;
    [SerializeField] private float stopTime = 1.5f;

    private Vector3 _lastPosition;
    private Rigidbody2D _rb;
    private bool _isThrown = false; 

    public bool onTheLeft;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Invoke(nameof(Explode), timeToExplode);
    }

    public void ThrowingBomb(Vector3 direction)
    { 
        onTheLeft = !onTheLeft;
        _rb.velocity = direction.normalized * throwForce;

        Invoke(nameof(StopMoving), stopTime);
    }

    private void StopMoving()
    {
        _rb.velocity = Vector2.zero; 
        _rb.angularVelocity = 0f; 
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