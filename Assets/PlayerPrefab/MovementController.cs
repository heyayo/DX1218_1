using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovementController : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private bool _onGround = false;

    [SerializeField] private Transform footPoint;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float jumpStrength = 1f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Jump
        if (Input.GetKeyDown(KeyCode.Space))
            if (_onGround)
                _rigidbody.AddForce(new Vector3(0,jumpStrength,0),ForceMode.Impulse);
    }
    
    private void FixedUpdate()
    {
        // Update _onGround Variable using Raycast to Ground
        GroundDetection();

        // On Ground Drag Change
        if (_onGround)
            _rigidbody.drag = 8f;
        else
            _rigidbody.drag = 1f;

        // WASD Movement
        if (Input.GetKey(KeyCode.W))
            _rigidbody.AddForce(transform.forward * moveSpeed);
        if (Input.GetKey(KeyCode.S))
            _rigidbody.AddForce(-transform.forward * moveSpeed);
        if (Input.GetKey(KeyCode.D))
            _rigidbody.AddForce(transform.right * moveSpeed);
        if (Input.GetKey(KeyCode.A))
            _rigidbody.AddForce(-transform.right * moveSpeed);
    }

    private void GroundDetection()
    {
        _onGround = Physics.Raycast(footPoint.position, Vector3.down, 1f);
    }
}
