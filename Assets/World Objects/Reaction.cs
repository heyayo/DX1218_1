using UnityEngine;
using UnityEngine.Events;

public abstract class Reaction : MonoBehaviour
{
    public UnityEvent onInteract;

    private void Awake()
    {
        if (onInteract == null)
            onInteract = new UnityEvent();
    }

    public abstract void Interact(float damage, float force);
}
