using UnityEngine;

public class HelicopterRotateAroundPoint : MonoBehaviour
{
    [SerializeField] private float rotateSpeed;

    void FixedUpdate()
    { transform.Rotate(0,Time.deltaTime * rotateSpeed,0); }
}
