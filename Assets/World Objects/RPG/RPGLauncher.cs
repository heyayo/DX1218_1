using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class RPGLauncher : ProjectileWeapon
{
    [SerializeField] private RPGRocket projectile;
    
    private ObjectPool<RPGRocket> _rockets;

    private void Awake()
    {
        base.Awake();
        _rockets = new ObjectPool<RPGRocket>
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
    
    public override void Shoot()
    {
        Vector3 fPos = firePoint.position;
        Quaternion fRot = firePoint.rotation;
        
        // Get Exhaust Particles from Pool and Play
        var exhaust = _exhaustPool.Get();
        Transform eT = exhaust.transform;
        eT.position = fPos;
        eT.rotation = fRot;
        exhaust.Play();
        
        // Get Rocket Projectile From Pool
        var rocket = _rockets.Get();
        Transform rT = rocket.transform;
        rT.position = fPos;
        rT.rotation = fRot;
        rocket.direction = _camT.forward;
        rocket.InitProjectile();

        StartCoroutine(ExplodeProjectile(rocket));
        StartCoroutine(ReleaseExhaustParticles(exhaust));
    }
    
    private IEnumerator ExplodeProjectile(RPGRocket projectile)
    {
        yield return new WaitForSeconds(projectile.lifeTime);
        if (!projectile.released)
            _rockets.Release(projectile);
    }
}
