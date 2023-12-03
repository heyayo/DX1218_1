using UnityEngine;
using UnityEngine.Events;

public class Item : MonoBehaviour
{
    [SerializeField] public bool reusable;
    [SerializeField] public bool reusableAlt;
    
    [HideInInspector] public Rigidbody rigidbody;
    private Collider[] _collider;
    private Collider[] _childColliders;
    private Transform _t;

    public UnityEvent onUse;
    public UnityEvent onAltUse;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponents<Collider>();
        _childColliders = GetComponentsInChildren<Collider>();
        _t = transform;
        if (onUse == null) // null Check to prevent overwriting Unity Inspector
            onUse = new UnityEvent();
        if (onAltUse == null)
            onAltUse = new UnityEvent();
    }
    
    public void Attach(Transform t)
    {
        // Attach To Hold Point
        _t.parent = t;
        // Reset Transform to be Parent's
        _t.localPosition = Vector3.zero;
        _t.localRotation = Quaternion.identity;
        // Disable Physics
        rigidbody.isKinematic = true;
        foreach (var col in _collider)
            col.enabled = false;
        foreach (var col in _childColliders)
            col.enabled = false;
    }

    public void Detach()
    {
        _t.localPosition = Vector3.zero;
        _t.localRotation = Quaternion.identity;
        _t.parent = null;
        rigidbody.isKinematic = false;
        foreach (var col in _collider)
            col.enabled = true;
        foreach (var col in _childColliders)
            col.enabled = true;
    }

    public virtual void Use()
    {
        // Debug Checks
        // Debug.Log("This Item Does Not Have A Use");
    }

    public virtual void AltUse()
    {
        // Debug Checks
        // Debug.Log("This Item Does Not Have An Alternate Use");
    }
}
