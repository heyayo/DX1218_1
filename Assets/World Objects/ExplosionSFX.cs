using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ExplosionSFX : MonoBehaviour
{
    [SerializeField] private AudioClip explosionSFX;
    private AudioSource _source;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        _source.clip = explosionSFX;
    }

    private void OnEnable()
    {
        _source.Play();
    }
}
