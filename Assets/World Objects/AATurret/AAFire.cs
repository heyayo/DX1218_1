using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class AAFire : MountableUseHook
{
    [SerializeField] private float fireRate;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private Transform[] flashPoints;
    
    private ObjectPool<ParticleSystem> _flashes;
    private AATurret _turret;
    private float _actualFireRate;

    private float _lastFireTime;

    private void Awake()
    {
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
                foreach (Transform point in flashPoints)
                {
                    var flash = _flashes.Get();
                    Transform ft = flash.transform;
                    ft.position = point.position;
                    ft.rotation = point.rotation;
                    flash.Play();
                    StartCoroutine(KillFlash(flash));
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
}
