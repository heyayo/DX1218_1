using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Rigidbody))]
public class HomingRocket : Projectile
{
    [Header("Lock On")]
    [SerializeField] public Transform target;

    [Header("Launch Settings")]
    [SerializeField] private float initialForce;
    
    [Header("Missile Climb Configuration")]
    [SerializeField] private float climbFuel;
    [SerializeField] private float climbSpeed;

    [Header("Track Configuration")]
    [SerializeField] private float chaseSpeed;

    public ObjectPool<HomingRocket> rocketPool;
    
    private Rigidbody _rigidbody;
    private Transform _t;
    private Quaternion _startRot;
    private float _climbEndTime;
    private float _climbRotationProgress;

    private delegate void RocketState();
    private RocketState _state;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _t = transform;
    }

    public override void InitProjectile()
    {
        _climbEndTime = Time.time + climbFuel;
        _state = Climbing;
        _startRot = _t.rotation;
        _climbRotationProgress = 0f;
        _rigidbody.AddForce(_t.forward * initialForce,ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        _state();
    }

    private void Climbing()
    {
        if (Time.time < _climbEndTime)
        {
            _rigidbody.AddForce(Vector3.forward * climbSpeed);
            _t.rotation = Quaternion.Lerp(_startRot,Quaternion.Euler(Vector3.up),_climbRotationProgress);
        }
        else _state = Chasing;
    }

    private void Chasing()
    {
        // Add Force Towards Target
        Vector3 direction = target.position - _t.position;
        _rigidbody.AddForce(direction.normalized * chaseSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        released = true;
        rocketPool.Release(this);
    }
}
