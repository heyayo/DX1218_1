using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Barrel : MonoBehaviour
{
    [SerializeField] private ParticleSystem explosion;
    [SerializeField] private GameObject mesh;
    [SerializeField] private AudioClip explosionClip;

    private AudioSource _source;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        _source.clip = explosionClip;
    }
    
    public void ExplodeBarrel()
    {
        explosion.Play();
        mesh.SetActive(false);
        StartCoroutine(DestroySelf());
        _source.Play();
    }

    private IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }
}
