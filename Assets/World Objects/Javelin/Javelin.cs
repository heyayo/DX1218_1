using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Javelin : ProjectileWeapon
{
    [Header("Configuration")]
    [SerializeField] private HomingRocket projectile;
    [SerializeField] private Transform firePoint;
    [SerializeField] private JavelinLocker lockingUI;
    
    private ObjectPool<HomingRocket> _rockets;
    private bool _locking = false;

    private void Awake()
    {
        base.Awake();
        _rockets = new ObjectPool<HomingRocket>
            (() =>
            {
                var rocket = Instantiate(projectile);
                rocket.rocketPool = _rockets;
                return rocket;
            },
            rocket =>
            { rocket.gameObject.SetActive(true); },
            rocket =>
            { rocket.gameObject.SetActive(false); },
            rocket =>
            { Destroy(rocket.gameObject); } );
    }

    private void Start()
    { lockingUI.EnableLocker(false); }

    public override void Shoot()
    {
        if (_locking)
        {
            if (lockingUI.lockedOnTarget == null)
                return;
        }
        else return;
        
        StartCoroutine(PlayerEffectsManager.Instance.SpikeRecoil(recoil));
        
        Vector3 ePos = exhaustPoint.position;
        Quaternion eRot = exhaustPoint.rotation;
        Vector3 fPos = firePoint.position;
        Quaternion fRot = firePoint.rotation;
        
        // Get Exhaust Particles from Pool and Play
        var exhaust = _exhaustPool.Get();
        Transform eT = exhaust.transform;
        eT.position = ePos;
        eT.rotation = eRot;
        exhaust.Play();
        
        // Get Rocket Projectile From Pool
        var rocket = _rockets.Get();
        Transform rT = rocket.transform;
        rT.position = fPos;
        rT.rotation = fRot;
        rocket.direction = _camT.forward;
        rocket.InitProjectile();
        rocket.target = lockingUI.lockedOnTarget;

        StartCoroutine(ExplodeProjectile(rocket));
        StartCoroutine(ReleaseExhaustParticles(exhaust));
    }

    public void TargetLockToggle()
    {
        _locking = !_locking;
        lockingUI.gameObject.SetActive(_locking);
        lockingUI.EnableLocker(_locking);
    }

    public void DisableLocking()
    {
        _locking = false;
        lockingUI.gameObject.SetActive(false);
        lockingUI.EnableLocker(false);
    }
    
    private IEnumerator ExplodeProjectile(HomingRocket projectile)
    {
        yield return new WaitForSeconds(projectile.lifeTime);
        if (!projectile.released)
            _rockets.Release(projectile);
    }
}
