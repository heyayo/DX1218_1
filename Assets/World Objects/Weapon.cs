using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [Tooltip("Weapon's Fire Rate Measured in RPM (Rounds Per Minute)")]
    [SerializeField] private float fireRate;

    private float _timeSinceLastShot = 0f;
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
    }
    
    // Used By The Item Event
    public void Fire()
    {
        if (Time.time > _timeSinceLastShot + _actualFireRate)
        {
            Shoot();
            _timeSinceLastShot = Time.time;
        }
    }

    // Determined by the weapon specific
    public abstract void Shoot();
}
