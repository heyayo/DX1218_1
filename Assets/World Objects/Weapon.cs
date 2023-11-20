using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] private LayerMask shootableMask;

    [SerializeField] protected PlayerEffectsManager effectsManager;
    [SerializeField] protected ParticleSystem toEmit;
    [SerializeField] protected Transform emissionPoint;
    [SerializeField] protected float damage;
    [SerializeField] protected PlayerEffectsManager.RecoilData recoil;
    
    protected Camera _cam;
    protected ObjectPool<ParticleSystem> _particles;

    public UnityEvent onShoot;

    protected void Awake()
    {
        // Debug Error Checking
        if (effectsManager == null)
        {
            Debug.LogError("Player Effects Manager on Object " + gameObject + " is not assigned");
            Debug.Break();
        }

        if (toEmit == null)
        {
            Debug.LogWarning("This Gun does not have a fire effect");
        }
        else
            if (emissionPoint == null && toEmit != null)
            {
                Debug.LogError("This Gun does not have an Emission Point but is assigned a Particle Effect");
                Debug.Break();
            }
        
        _cam = Camera.main;
        _particles = new ObjectPool<ParticleSystem>(
            () =>
            {
                var system = Instantiate(toEmit,emissionPoint.position,emissionPoint.rotation,emissionPoint);
                var timer = system.GetComponent<ParticleTimer>();
                timer.onDeath = _particles.Release;
                timer.BeginTimer();
                system.transform.parent = null;
                return system;
            },
            particle =>
            {
                particle.gameObject.SetActive(true);
                var timer = particle.GetComponent<ParticleTimer>();
                timer.BeginTimer();
                particle.Clear();
                particle.Play();
                particle.transform.position = emissionPoint.position;
                particle.transform.rotation = emissionPoint.rotation;
            },
            particle =>
            {
                particle.Stop();
                particle.gameObject.SetActive(false);
            },
            particle =>
            {
                Destroy(particle.gameObject);
            }
            );
        
        onShoot = new UnityEvent();
    }
    
    public bool ShootRaycast(out RaycastHit output, float maxDist = Single.MaxValue)
    {
        onShoot.Invoke();
        
        Ray ray = new Ray();
        ray.origin = _cam.transform.position;
        ray.direction = _cam.transform.forward;

        bool returnValue = Physics.Raycast(ray, out RaycastHit hitInfo, maxDist,shootableMask);
        output = hitInfo;
        return returnValue;
    }

    public void TriggerRecoil()
    {
        StartCoroutine(effectsManager.SpikeRecoil(recoil));
    }

    public abstract void Use();
}
