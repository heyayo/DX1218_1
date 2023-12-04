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
    [SerializeField] private ParticleSystem explosion;
    [SerializeField] private float explosionRadius;
    [SerializeField] private float damage;
    
    // Get From RocketLauncher
    public ObjectPool<RPGRocket> rocketPool;
    private ObjectPool<ParticleSystem> _explosions;
    private Rigidbody _rigidbody;
    private Transform _t;
    private float _expireTime;
    private float _deathTime;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _t = transform;
        
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
        _expireTime = Time.time + fuel;
        _deathTime = Time.time + lifeTime;
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

    private void Update()
    {
        if (Time.time > _deathTime)
        {
            released = true;
            rocketPool.Release(this);
        }
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
