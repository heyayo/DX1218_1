using System.Collections.Generic;
using UnityEngine;

public class CameraOffset
{
    public float pitch;
    public float yaw;
}

public class CameraController : MonoBehaviour
{
    private Camera _cam;
    
    private float _pitch;
    private float _yaw;
    public CameraOffset mouseDirection = new CameraOffset();
    public List<CameraOffset> offsets = new List<CameraOffset>();

    [SerializeField] private float sensitivity = 1f;

    private void Awake()
    {
        _cam = Camera.main;
    }

    private void Update()
    {
        // Fetch Mouse Input
        float h = Input.GetAxis("Mouse X") * sensitivity;
        float v = Input.GetAxis("Mouse Y") * sensitivity;
        _yaw += h;
        _pitch -= v;
        // Sync MouseDirection Value
        mouseDirection.pitch = -v;
        mouseDirection.yaw = h;

        // Turn Mouse Input into Camera Rotation
        float finalPitch = _pitch;
        float finalYaw = _yaw;
        for (int i = 0; i < offsets.Count; ++i) // Tally all offsets
        {
            finalPitch += offsets[i].pitch;
            finalYaw += offsets[i].yaw;
        }
        _cam.transform.localRotation = Quaternion.Euler(finalPitch,0,0);
        transform.localRotation = Quaternion.Euler(0,finalYaw,0);

        // Debug Mouse Window Lock Toggle
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
