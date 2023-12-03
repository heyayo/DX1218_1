using UnityEngine;

[RequireComponent(typeof(MountableCameraController))]
public class Mountable : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private MovementController movCon;
    [SerializeField] private CameraController camCon;
    [SerializeField] private InventoryManager invMan;
    [SerializeField] private Rigidbody playerBody;
    [SerializeField] private Collider playerCollider;
    [SerializeField] private Transform mountPoint;
    [SerializeField] protected MountableUseHook useHook;

    protected MountableCameraController _mCC;
    private Transform _pT;
    private Vector3 _pLastLoc;
    
    protected void Awake()
    {
        // Debug Error Checks
        if (!movCon)
        {
            Debug.LogError("Player Movement Controller not set in a Mountable | " + name);
            Debug.Break();
        }
        if (!camCon)
        {
            Debug.LogError("Player Camera Controller not set in a Mountable | " + name);
            Debug.Break();
        }
        if (!mountPoint)
        {
            Debug.LogError("Mount Point not set in a Mountable | " + name);
            Debug.Break();
        }

        _pT = movCon.transform;
        playerBody = movCon.GetComponent<Rigidbody>();
        playerCollider = movCon.GetComponent<Collider>();
        _mCC = GetComponent<MountableCameraController>();
        _mCC.enabled = false;
        useHook = GetComponent<MountableUseHook>();
        useHook.enabled = false;
    }

    public void Mount()
    {
        _pLastLoc = _pT.position;
        _pT.parent = transform;
        _pT.position = mountPoint.position;
        _pT.rotation = mountPoint.rotation;
        movCon.enabled = false;
        camCon.enabled = false;
        playerBody.isKinematic = true;
        playerCollider.enabled = false;
        invMan.enabled = false;
        _mCC.enabled = true;
        useHook.enabled = true;
    }

    public void Dismount()
    {
        _pT.parent = null;
        _pT.position = _pLastLoc;
        movCon.enabled = true;
        camCon.enabled = true;
        playerBody.isKinematic = false;
        playerCollider.enabled = true;
        invMan.enabled = true;
        _mCC.enabled = false;
        useHook.enabled = false;
    }
}
