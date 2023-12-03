using System.Collections.Generic;
using UnityEngine;

public class CameraOffset
{
    public float pitch;
    public float yaw;
    
    public CameraOffset()
    {}
    
    public CameraOffset(float p, float y)
    {
        pitch = p;
        yaw = y;
    }

    public CameraOffset clone()
    {
        return new CameraOffset(pitch, yaw);
    }
}

public class CameraController : MonoBehaviour
{
    protected Camera _cam;
    
    protected float _pitch;
    protected float _yaw;
    public CameraOffset mouseDirection = new CameraOffset();
    public List<CameraOffset> offsets = new List<CameraOffset>();

    [SerializeField] protected float sensitivity = 1f;

    public CameraOffset originalOffset
    {
        get { return new CameraOffset(_pitch, _yaw); }
    }

    private void Awake()
    {
        _cam = Camera.main;
    }

    private void Update()
    {
        // Fetch Mouse Input
        FetchInput();
        
        // Clamp Pitch to Prevent Neck Damage
        _pitch = Mathf.Clamp(_pitch, -90, 90);

        // Turn Mouse Input into Camera Rotation
        CameraOffset tally = TallyOffsets();

        _cam.transform.localRotation = Quaternion.Euler(tally.pitch,0,0);
        transform.localRotation = Quaternion.Euler(0,tally.yaw,0);

        // Debug Mouse Window Lock Toggle
        DEBUG_ToggleMouseLock();
    }

    protected CameraOffset GetMouseMovement()
    {
        float h = Input.GetAxis("Mouse X") * sensitivity;
        float v = Input.GetAxis("Mouse Y") * sensitivity;
        return new CameraOffset(v, h);
    }

    protected void FetchInput()
    {
        CameraOffset mouse = GetMouseMovement();
        _yaw += mouse.yaw;
        _pitch -= mouse.pitch;
        // Sync MouseDirection Value
        mouseDirection.pitch = -mouse.pitch;
        mouseDirection.yaw = mouse.yaw;
    }

    protected CameraOffset TallyOffsets()
    {
        float finalPitch = _pitch;
        float finalYaw = _yaw;
        for (int i = 0; i < offsets.Count; ++i) // Tally all offsets
        {
            finalPitch += offsets[i].pitch;
            finalYaw += offsets[i].yaw;
        }

        return new CameraOffset(finalPitch, finalYaw);
    }

    protected void DEBUG_ToggleMouseLock()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            switch (Cursor.lockState)
            {
                case CursorLockMode.Locked:
                    Cursor.lockState = CursorLockMode.None;
                    break;
                case CursorLockMode.None:
                    Cursor.lockState = CursorLockMode.Locked;
                    break;
            }
        }
    }
}
