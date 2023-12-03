using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    public Vector3 direction;
    public bool released = false;
    public float lifeTime;

    public abstract void InitProjectile();
}
