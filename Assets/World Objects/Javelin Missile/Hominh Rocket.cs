using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
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
    [SerializeField] private float chaseRotSpeed;

    [Header("Explosion")]
    [SerializeField] private ParticleSystem explosion;
    [SerializeField] private float explosionRadius;
    [SerializeField] private float damage;

    public ObjectPool<HomingRocket> rocketPool;
    private ObjectPool<ParticleSystem> _explosions;

    private AudioSource _source;
    private Rigidbody _rigidbody;
    private Transform _t;
    private Quaternion _startRot;
    private Quaternion _upRot;
    private float _climbEndTime;
    private float _climbRotationProgress;
    private float _deathTime;

    private delegate void RocketState();
    private RocketState _state;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _t = transform;
        _upRot = Quaternion.LookRotation(-Vector3.up);

        _explosions = new ObjectPool<ParticleSystem>
            (
            () =>
            { return Instantiate(explosion); },
            particle =>
            { particle.gameObject.SetActive(true); },
            particle =>
            { particle.gameObject.SetActive(false); },
            particle =>
            { Destroy(particle.gameObject); }
            );
    }

    public override void InitProjectile()
    {
        _deathTime = Time.time + lifeTime;
        _climbEndTime = Time.time + climbFuel;
        _state = Climbing;
        _startRot = _t.rotation;
        _climbRotationProgress = 0f;
        _rigidbody.AddForce(-_t.forward * initialForce,ForceMode.Impulse);
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.useGravity = true;
    }

    private void FixedUpdate()
    {
        _state();
    }

    private void Update()
    {
        if (Time.time > _deathTime)
        {
            released = true;
            rocketPool.Release(this);
        }
    }
    
    private void Climbing()
    {
        if (Time.time < _climbEndTime)
        {
            _climbRotationProgress += Time.deltaTime;
            _climbRotationProgress = Mathf.Clamp(_climbRotationProgress, 0f, 1f);
            _rigidbody.AddForce(-transform.forward * climbSpeed);
            _t.rotation = Quaternion.Lerp(_startRot,_upRot,_climbRotationProgress);
        }
        // else
        // {
        //     rocketPool.Release(this);
        //     released = true;
        // }
        else
        {
            if (target == null)
            {
                var e = _explosions.Get();
                e.Play();
                Transform et = e.transform;
                et.position = _t.position;
                et.rotation = Quaternion.identity;

                Collider[] colliders = Physics.OverlapSphere(_t.position, explosionRadius);
                foreach (var col in colliders)
                {
                    if (col.CompareTag("Reactable"))
                    {
                        var comp = col.GetComponent<Reaction>();
                        if (comp == null)
                            comp = ClimbHierarchy(col.transform.parent);
                        comp.Interact(damage,0);
                    }
                }
        
                PlayerEffectsManager.Instance.ShakeCam(1,5);
                released = true;
                rocketPool.Release(this);
                return;
            }
            _state = Chasing;
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.useGravity = false;
        }
    }

    private void Chasing()
    {
        // Add Force Towards Target
        direction = target.position - _t.position;
        _rigidbody.velocity = direction.normalized * chaseSpeed;
        Quaternion towards = Quaternion.LookRotation(direction);
        _t.rotation = Quaternion.RotateTowards(_t.rotation,towards,chaseRotSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        var e = _explosions.Get();
        e.Play();
        Transform et = e.transform;
        et.position = _t.position;
        et.rotation = Quaternion.identity;

        Collider[] colliders = Physics.OverlapSphere(_t.position, explosionRadius);
        foreach (var col in colliders)
        {
            if (col.CompareTag("Reactable"))
            {
                var comp = col.GetComponent<Reaction>();
                if (comp == null)
                    comp = ClimbHierarchy(col.transform.parent);
                comp.Interact(damage,0);
            }
        }
        
        PlayerEffectsManager.Instance.ShakeCam(1,5);
        released = true;
        rocketPool.Release(this);
    }

    private Reaction ClimbHierarchy(Transform start)
    {
        if (start.parent == null) return null;
        Reaction reaction = start.parent.GetComponent<Reaction>();
        if (reaction == null)
            return ClimbHierarchy(start.parent);
        return reaction;
    }
}
