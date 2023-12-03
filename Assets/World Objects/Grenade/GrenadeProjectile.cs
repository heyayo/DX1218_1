using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class GrenadeProjectile : MonoBehaviour
{
    [SerializeField] private ParticleSystem explosion;
    [SerializeField] private GameObject mesh;
    [SerializeField] private float throwForce;
    [SerializeField] private float fuse;
    [SerializeField] private float radius;
    [SerializeField] private float damage;

    private Camera _cam;
    private Rigidbody _rigidbody;
    private float _fuseProgress;

    public UnityEvent onThrow;
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _cam = Camera.main;
    }

    public void Cook()
    {
        StartCoroutine(CookGrenade());
    }

    public void Throw()
    {
        Debug.Log("Throwing Grenade");
        onThrow.Invoke();
        _rigidbody.AddForce(_cam.transform.forward * throwForce,ForceMode.Impulse);
    }

    private IEnumerator CookGrenade()
    {
        _fuseProgress = Time.time;
        while (Time.time < _fuseProgress + fuse)
        {
            yield return null;
        }
        Explode();
    }

    private void Explode()
    {
        throwForce = 0;
        Throw();
        explosion.Play();
        mesh.SetActive(false);
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
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

        StartCoroutine(KillGrenade());
    }

    private IEnumerator KillGrenade()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
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
