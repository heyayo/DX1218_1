using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MountableUseHook : MonoBehaviour
{
    public abstract void UpdateHook();

    // Using Abstract Function to Force Implementation
    private void Update()
    {
        UpdateHook();
    }
}
