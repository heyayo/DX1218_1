using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class PistolScript : Weapon
{
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private ParticleSystem impactEffect;
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private PlayerEffectsManager.RecoilData recoil;
    [SerializeField] private float damage;

    private Camera _cam;
    private Transform _camT;
    private Transform _t;

    private Vector3 _adsVector = new Vector3(0, 0.1f, -0.32f);
    private float _adsProgress;
    private bool _ads = false;

    private ObjectPool<ParticleSystem> _muzzlePool;
    private ObjectPool<ParticleSystem> _impactPool;

    private void Awake()
    {
        base.Awake();
        
        _cam = Camera.main;
        _camT = _cam.transform;
        _t = transform;

        if (recoil == null)
            recoil = new PlayerEffectsManager.RecoilData();

        _muzzlePool = new ObjectPool<ParticleSystem>
            ( () =>
            { return Instantiate(muzzleFlash,muzzlePoint.position,muzzlePoint.rotation,transform); },
            particle =>
            { particle.gameObject.SetActive(true); },
            particle =>
            { particle.gameObject.SetActive(false); },
            particle =>
            { Destroy(particle.gameObject); });
        _impactPool = new ObjectPool<ParticleSystem>
            ( () =>
            { return Instantiate(impactEffect); },
            particle =>
            { particle.gameObject.SetActive(true); },
            particle =>
            { particle.gameObject.SetActive(false); },
            particle =>
            { Destroy(particle.gameObject); });
    }
    
    public override void Shoot()
    {
        StartCoroutine(PlayerEffectsManager.Instance.SpikeRecoil(recoil));
        
        // Spawn Muzzle Flash and Hide After 2 Seconds
        var flash = _muzzlePool.Get();
        flash.Play();
        StartCoroutine(HideFlash(flash));
        
        // Fire Raycast
        Ray ray = new Ray();
        ray.origin = _camT.position;
        ray.direction = _camT.forward;
        var hit = Physics.Raycast(ray, out RaycastHit info);

        // Handle Hit
        if (hit)
        {
            if (info.collider.CompareTag("Reactable"))
            {
                Debug.Log("Hit something Reactable");
                var comp = info.collider.GetComponent<Reaction>();
                comp.onInteract.Invoke();
                comp.Interact(damage,0);
                var impact = _muzzlePool.Get();
                Transform impactT = impact.transform;
                impactT.position = info.point;
                impactT.LookAt(info.normal);
            }
        }
    }

    public void ADS()
    {
        _ads = !_ads;
        
    }

    private IEnumerator HideFlash(ParticleSystem flash)
    {
        yield return new WaitForSeconds(2);
        flash.Stop();
        _muzzlePool.Release(flash);
    }
}
