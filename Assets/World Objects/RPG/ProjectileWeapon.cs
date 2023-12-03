using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

public abstract class ProjectileWeapon : Weapon
{
    [SerializeField] protected PlayerEffectsManager.RecoilData recoil;
    [SerializeField] protected ParticleSystem rocketExhaust;
    [SerializeField] protected Transform exhaustPoint;

    protected ObjectPool<ParticleSystem> _exhaustPool;
    protected Transform _camT;

    protected void Awake()
    {
        base.Awake();

        _camT = Camera.main.transform;

        _exhaustPool = new ObjectPool<ParticleSystem>
            ( () => { return Instantiate(rocketExhaust); },
            particle =>
            { particle.gameObject.SetActive(true); },
            particle =>
            { particle.gameObject.SetActive(false); },
            particle =>
            { Destroy(particle.gameObject); } );
    }
    
    protected IEnumerator ReleaseExhaustParticles(ParticleSystem exhaust)
    {
        yield return new WaitUntil(() => { return exhaust.isStopped; });
        _exhaustPool.Release(exhaust);
    }
}
