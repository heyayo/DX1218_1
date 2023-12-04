using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [Tooltip("Weapon's Fire Rate Measured in RPM (Rounds Per Minute)")]
    [SerializeField] private float fireRate;
    [SerializeField] protected AmmoCounter ammo;
    [SerializeField] protected int clip;
    [SerializeField] protected int clipMax;
    [SerializeField] protected float reloadTime;

    private float _timeSinceLastShot = 0f;
    private float _reloadTime = 0f;
    private float _actualFireRate;

    public float fire_rate
    {
        get { return fireRate; }
        set
        {
            fireRate = value;
            _actualFireRate = 60f / fireRate;
        }
    }
    
    protected void Awake()
    {
        _actualFireRate = 60f / fireRate;
        _timeSinceLastShot = Time.time - _actualFireRate;
        _reloadTime = Time.time - reloadTime;
    }
    
    // Used By The Item Event
    public void Fire()
    {
        float time = Time.time;
        if (time > _timeSinceLastShot + _actualFireRate && time > _reloadTime + reloadTime && clip > 0)
        {
            Shoot();
            _timeSinceLastShot = Time.time;
        }
        if (clip <= 0)
            Reload();
    }

    // Reload, no clip dump
    public void Reload()
    {
        int delta = clipMax - clip;
        if (ammo.reserve < delta)
            return;
        ammo.reserve -= delta;
        clip = clipMax;
        _reloadTime = Time.time;
    }

    // Determined by the weapon specific
    public abstract void Shoot();
}
