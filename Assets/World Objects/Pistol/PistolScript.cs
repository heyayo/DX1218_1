using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class PistolScript : Weapon
{
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private ParticleSystem impactEffect;
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private Transform mesh;
    [SerializeField] private PlayerEffectsManager.RecoilData recoil;
    [SerializeField] private float damage;
    
    [Header("ADS Values")]
    [SerializeField] private float adsSpeed;
    [SerializeField] private Vector3 ADSVector;
    [SerializeField] private Vector3 nonADSVector;
    [SerializeField] private float adsFOV;

    [SerializeField] private AudioClip shootSFX;
    [SerializeField] private AudioClip reloadSFX;
    
    private Camera _cam;
    private Transform _camT;
    private Transform _t;
    protected AudioSource _source;

    private float _adsProgress;
    private float _nonADSFov;

    private ObjectPool<ParticleSystem> _muzzlePool;
    private ObjectPool<ParticleSystem> _impactPool;

    private void Awake()
    {
        base.Awake();
        
        _source = GetComponent<AudioSource>();
        _cam = Camera.main;
        _camT = _cam.transform;
        _t = transform;
        nonADSVector = mesh.localPosition;
        _nonADSFov = _cam.fieldOfView;

        if (recoil == null)
            recoil = new PlayerEffectsManager.RecoilData();

        _muzzlePool = new ObjectPool<ParticleSystem>
            ( () =>
            { return Instantiate(muzzleFlash,muzzlePoint.position,muzzlePoint.rotation,muzzlePoint); },
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
        _source.clip = shootSFX;
        _source.Play();
        --clip;
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
            Debug.Log("Hit Something");
            var impact = _impactPool.Get();
            Transform impactT = impact.transform;
            impactT.position = info.point;
            impactT.LookAt(info.normal);
            StartCoroutine(HideDecal(impact));
            if (info.collider.CompareTag("Reactable"))
            {
                var comp = info.collider.GetComponent<Reaction>();
                comp.onInteract.Invoke();
                comp.Interact(damage,0);
            }
        }
    }

    public void ADS()
    {
        _adsProgress += adsSpeed * 2 * Time.deltaTime;
    }

    public void SniperADS()
    {
        _adsProgress += adsSpeed * 2 * Time.deltaTime;
    }

    public void ReloadSFX()
    {
        _source.clip = reloadSFX;
        _source.Play();
    }

    private void Update()
    {
        _adsProgress -= adsSpeed * Time.deltaTime;
        _adsProgress = Mathf.Clamp(_adsProgress, 0, 1);
        mesh.localPosition = Vector3.Lerp(nonADSVector, ADSVector, _adsProgress);
        _cam.fieldOfView = Mathf.Lerp(_nonADSFov, adsFOV, _adsProgress);
    }
    
    private IEnumerator HideFlash(ParticleSystem flash)
    {
        yield return new WaitForSeconds(2);
        flash.Stop();
        _muzzlePool.Release(flash);
    }
    private IEnumerator HideDecal(ParticleSystem decal)
    {
        yield return new WaitForSeconds(2);
        decal.Stop();
        _impactPool.Release(decal);
    }
}
