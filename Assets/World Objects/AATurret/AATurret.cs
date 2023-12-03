using UnityEngine;

[RequireComponent(typeof(AAFire))]
public class AATurret : Mountable
{
    [Header("Mesh Pieces")]
    [SerializeField] private AAFire aaFire;
    [SerializeField] private Transform radarDish;
    [SerializeField] private Transform guns;
    
    [Header("Animation Values")]
    [SerializeField] private float radarScanningSpeed;
    
    private void Awake()
    {
        base.Awake();
        aaFire = GetComponent<AAFire>();
    }
    
    private void FixedUpdate()
    {
        radarDish.Rotate(0,Time.fixedDeltaTime * radarScanningSpeed,0);
        guns.localRotation = _mCC.exposedRotation * Quaternion.Euler(-30.25f,0,0);
    }
}
