using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class AAFire : MountableUseHook
{
    [SerializeField] private float damage;
    [SerializeField] private float fireRate;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private ParticleSystem impactParticles;
    [SerializeField] private Transform[] flashPoints;
    [SerializeField] private AudioClip fireSound;
    
    private ObjectPool<ParticleSystem> _flashes;
    private ObjectPool<ParticleSystem> _impacts;
    private AudioSource _source;
    private AATurret _turret;
    private Camera _cam;
    private float _actualFireRate;

    private float _lastFireTime;

    private void Awake()
    {
        _cam = Camera.main;
        _source = GetComponent<AudioSource>();
        _source.clip = fireSound;
        _source.playOnAwake = false;
        
        _flashes = new ObjectPool<ParticleSystem>
            (
            () =>
            { return Instantiate(muzzleFlash, transform); },
            particle =>
            { particle.gameObject.SetActive(true); },
            particle =>
            { particle.gameObject.SetActive(false); },
            particle =>
            { Destroy(particle.gameObject); }
            );
        _impacts = new ObjectPool<ParticleSystem>
            (
            () =>
            { return Instantiate(impactParticles, transform); },
            particle =>
            { particle.gameObject.SetActive(true); },
            particle =>
            { particle.gameObject.SetActive(false); },
            particle =>
            { Destroy(particle.gameObject); }
            );
        _turret = GetComponent<AATurret>();

        _actualFireRate = 60 / fireRate;

        _lastFireTime = Time.time;
    }
    
    public override void UpdateHook()
    {
        if (Input.GetKeyDown(KeyCode.F))
            _turret.Dismount();
        
        if (Input.GetMouseButton(0))
        {
            if (Time.time > _lastFireTime + _actualFireRate)
            {
                _source.Play();
                foreach (Transform point in flashPoints)
                {
                    var flash = _flashes.Get();
                    Transform ft = flash.transform;
                    ft.position = point.position;
                    ft.rotation = point.rotation;
                    flash.Play();
                    StartCoroutine(KillFlash(flash));

                    Ray ray = new Ray();
                    ray.origin = point.position;
                    ray.direction = _cam.transform.forward;
                    bool hit = Physics.Raycast(ray, out RaycastHit info);

                    if (hit)
                    {
                        var impact = _impacts.Get();
                        Transform impactT = impact.transform;
                        impactT.position = info.point;
                        impactT.LookAt(info.normal);
                        StartCoroutine(HideDecal(impact));
                        if (info.collider.CompareTag("Reactable"))
                        {
                            Debug.Log("Hit something Reactable");
                            var comp = info.collider.GetComponent<Reaction>();
                            comp.onInteract.Invoke();
                            comp.Interact(damage,0);
                        }
                    }
                }

                _lastFireTime = Time.time;
            }
        }
    }

    private IEnumerator KillFlash(ParticleSystem flash)
    {
        yield return new WaitForSeconds(1);
        _flashes.Release(flash);
    }
    private IEnumerator HideDecal(ParticleSystem decal)
    {
        yield return new WaitForSeconds(2);
        decal.Stop();
        _impacts.Release(decal);
    }
}
