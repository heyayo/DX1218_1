using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera _cam;
    
    private float _pitch;
    private float _yaw;

    [SerializeField] private float sensitivity = 1f;

    private void Awake()
    {
        _cam = Camera.main;
    }

    private void Update()
    {
        float h = Input.GetAxis("Mouse X") * sensitivity;
        float v = Input.GetAxis("Mouse Y") * sensitivity;
        _yaw += h;
        _pitch -= v;
        
        _cam.transform.localRotation = Quaternion.Euler(_pitch,0,0);
        transform.localRotation = Quaternion.Euler(0,_yaw,0);

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
