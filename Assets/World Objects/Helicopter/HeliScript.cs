using System.Collections;
using UnityEngine;

public class HeliScript : MonoBehaviour
{
    [SerializeField] private float rotorSpeed;
    [SerializeField] private Transform rotorA;
    [SerializeField] private Transform rotorB;
    [SerializeField] private ParticleSystem explosion;
    [SerializeField] private AudioClip explosionSFX;

    private Rigidbody _rigidbody;
    private AudioSource _source;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _source = GetComponent<AudioSource>();
        _source.clip = explosionSFX;
        _source.playOnAwake = false;
    }
    
    private void FixedUpdate()
    {
        rotorA.Rotate(0,Time.deltaTime * rotorSpeed,0);
        rotorB.Rotate(0,Time.deltaTime * rotorSpeed,0);
    }

    public void Destroyed()
    {
        _source.Play();
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
