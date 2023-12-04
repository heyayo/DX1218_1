using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovementController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private Transform footPoint;
    
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float jumpStrength = 1f;
    [SerializeField] private float jumpTime;

    [Header("Deformation")]
    [SerializeField] private float crouchSpeed;

    [Header("Camera Movement")]
    [SerializeField] private float headBobRange;
    [SerializeField] private float headBobRate;

    private Camera _cam;
    private Rigidbody _rigidbody;
    private BoxCollider _boxCollider;
    private Transform _camT;
    private float _realMoveSpeed;
    private bool _onGround = false;
    private bool _isProne = false;

    // Crouching
    private readonly Vector3 _standCenter = new Vector3(0, -.7f, 0);
    private readonly Vector3 _proneCenter = new Vector3(0, -.1f, 0);
    private readonly Vector3 _standHeight = new Vector3(.6f,2f,.6f);
    private readonly Vector3 _proneHeight = new Vector3(.6f,.4f,.6f);
    private readonly Vector3 _standFootPoint = new Vector3(0, -1.6f, 0);
    private readonly Vector3 _proneFootPoint = new Vector3(0, -.2f, 0);
    private float _bodyState = 0f; // Used to Determine Stand/Crouch/Prone Heights
    private float _jumpTime;
    
    // Camera Movement (Head Bobbing)
    private float _headBobProgress = 0f;
    private Vector3 _headBobCameraPosition = new Vector3();

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _boxCollider = GetComponent<BoxCollider>();
        _cam = Camera.main;
        _camT = _cam.transform;
    }

    private void Start()
    {
        _boxCollider.center = _standCenter;
        _boxCollider.size = _standHeight;
    }

    private void Update()
    {
        // Sprinting
        _realMoveSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;
        
        // Jump
        if (Input.GetKey(KeyCode.Space))
        {
            if (_onGround)
            {
                if (_jumpTime > 0)
                {
                    _rigidbody.AddForce(new Vector3(0, jumpStrength, 0));
                    _jumpTime -= Time.deltaTime;
                }
            }
            else _jumpTime = 0;
        }
        else
        {
            _jumpTime = jumpTime;
        }
        
        // Going Prone
        if (Input.GetKeyDown(KeyCode.LeftControl))
            _isProne = !_isProne;
     
        // Crouching
        if (!_isProne)
        {
            if (Input.GetKey(KeyCode.C)) _bodyState += Time.deltaTime * crouchSpeed;
            else _bodyState -= Time.deltaTime * crouchSpeed;
            _bodyState = Mathf.Clamp(_bodyState, 0, 0.625f); // 0.625f Gives 1 Height Which is Crouching Height
        }
        else // Prone(ing)
        {
            _bodyState = Mathf.Clamp(_bodyState, 0, 1);
            _bodyState += Time.deltaTime * crouchSpeed;
            if (Input.GetKey(KeyCode.C)) _isProne = false;
        }
        _boxCollider.center = Vector3.Lerp(_standCenter, _proneCenter, _bodyState);
        _boxCollider.size = Vector3.Lerp(_standHeight, _proneHeight, _bodyState);
        footPoint.localPosition = Vector3.Lerp(_standFootPoint, _proneFootPoint, _bodyState);
        
        // Head Bobbing
        Vector3 velocity = _rigidbody.velocity;
        velocity.y = 0;
        if (velocity.sqrMagnitude > 0.1)
        {
            _headBobProgress += Time.deltaTime * headBobRate * _realMoveSpeed;
            _headBobCameraPosition.y = Mathf.Sin(_headBobProgress) * headBobRange;
            _camT.localPosition = _headBobCameraPosition;
        }
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
            _rigidbody.AddForce(transform.forward * _realMoveSpeed);
        if (Input.GetKey(KeyCode.D))
            _rigidbody.AddForce(transform.right * _realMoveSpeed);
        if (Input.GetKey(KeyCode.S))
            _rigidbody.AddForce(-transform.forward * _realMoveSpeed);
        if (Input.GetKey(KeyCode.A))
            _rigidbody.AddForce(-transform.right * _realMoveSpeed);
    }

    private void GroundDetection()
    {
        _onGround = Physics.Raycast(footPoint.position, Vector3.down, 1f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(footPoint.position,footPoint.position + Vector3.down);
    }
}
