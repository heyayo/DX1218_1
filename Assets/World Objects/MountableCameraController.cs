using UnityEngine;

public class MountableCameraController : CameraController
{
    public Quaternion exposedRotation;
    
    private void Update()
    {
        // Fetch Mouse Input
        FetchInput();
        
        // Clamp Pitch to Prevent Neck Damage
        _pitch = Mathf.Clamp(_pitch, -90, 90);

        // Turn Mouse Input into Camera Rotation
        CameraOffset tally = TallyOffsets();

        Quaternion camRot = Quaternion.Euler(tally.pitch, 0, 0);
        Quaternion rot = Quaternion.Euler(0, tally.yaw, 0);
        _cam.transform.localRotation = camRot;
        transform.localRotation = rot;
        exposedRotation = camRot;

        // Debug Mouse Window Lock Toggle
        DEBUG_ToggleMouseLock();
    }
}
