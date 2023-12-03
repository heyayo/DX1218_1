using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Rigidbody))]
public class RPGRocket : Projectile
{
    [Header("Configuration")]
    [SerializeField] private ParticleSystem _exhaust;
    [SerializeField] private float speed;
    [Tooltip("How long in seconds the rocket will propel")]
    [SerializeField] private float fuel;

    // Get From RocketLauncher
    public ObjectPool<RPGRocket> rocketPool;
    private Rigidbody _rigidbody;
    private float _expireTime;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public override void InitProjectile()
    {
        _expireTime = Time.time + fuel;
        _exhaust.Play();
        _rigidbody.velocity = Vector3.zero;
        released = false;
    }

    private void FixedUpdate()
    {
        if (Time.time < _expireTime)
        {
            // Add Force In Direction while there is still fuel
            _rigidbody.AddForce(direction * speed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided With Object");
        released = true;
        rocketPool.Release(this);
    }
}
