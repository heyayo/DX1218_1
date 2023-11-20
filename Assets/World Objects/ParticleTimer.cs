using System;
using System.Collections;
using UnityEngine;

public class ParticleTimer : MonoBehaviour
{
    [SerializeField] private ParticleSystem selfParticle;
    [SerializeField] private float lifetime;
    public Action<ParticleSystem> onDeath;

    private void Awake()
    {
        selfParticle = GetComponent<ParticleSystem>();
    }
    
    public void BeginTimer()
    {
        StartCoroutine(TimeTilDeath());
    }

    private IEnumerator TimeTilDeath()
    {
        yield return new WaitForSeconds(lifetime);
        onDeath(selfParticle);
    }
}
