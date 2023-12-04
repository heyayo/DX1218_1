using System.Collections;
using UnityEngine;

public class HeliScript : MonoBehaviour
{
    [SerializeField] private float rotorSpeed;
    [SerializeField] private Transform rotorA;
    [SerializeField] private Transform rotorB;
    [SerializeField] private ParticleSystem explosion;

    private Rigidbody _rigidbody;

    private void Awake()
    { _rigidbody = GetComponent<Rigidbody>(); }
    
    private void FixedUpdate()
    {
        rotorA.Rotate(0,Time.deltaTime * rotorSpeed,0);
        rotorB.Rotate(0,Time.deltaTime * rotorSpeed,0);
    }

    public void Destroyed()
    {
        StartCoroutine(DestroySelf());
        transform.parent = null;
        explosion.Play();
        _rigidbody.isKinematic = false;
        _rigidbody.useGravity = true;
    }

    private IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }
}
