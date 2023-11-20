using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolScript : Weapon
{
    private void Awake()
    {
        base.Awake();
    }
    
    public override void Use()
    {
        TriggerRecoil();
        _particles.Get(); // Fire Muzzle Particle, AUTO KILLING
        if (ShootRaycast(out RaycastHit info))
        {
            Debug.Log("Shot Something that reacts");
            if (info.collider.CompareTag("Destructable"))
            {
                var comp = info.collider.GetComponent<Destructable>();
                comp.TakeDamage(damage);
            }
        }
    }
}
