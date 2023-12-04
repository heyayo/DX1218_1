using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class RailgunTurret : MonoBehaviour
{
    [SerializeField] private HomingRocket rocket;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Transform head;
    [SerializeField] private Transform headBase;
    [SerializeField] private float detectionRange;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float trackSpeed;
    [SerializeField] private float fireRate;

    private Camera _cam;
    private ObjectPool<HomingRocket> _rockets;
    private Transform _t;
    private Transform _camT;
    private float _rotateProgress;
    private float _lastFire;
    private float _realFireRate;

    private void Awake()
    {
        _realFireRate = 60 / fireRate;
        _lastFire = Time.time - _realFireRate;
        _cam = Camera.main;
        _camT = _cam.transform;
        _t = transform;
        _rotateProgress = 0f;

        _rockets = new ObjectPool<HomingRocket>
            (
            () =>
            {
                var r = Instantiate(rocket);
                r.rocketPool = _rockets;
                return r;
            },
            rocket => { rocket.gameObject.SetActive(true); },
            rocket => { rocket.gameObject.SetActive(false); },
            rocket => { Destroy(rocket.gameObject); }
            );
    }
    
    private void Update()
    {
        Vector3 diff = _t.position - _camT.position;
        if (diff.magnitude < detectionRange)
        {
            TiltTowardsPlayer();
            if (Time.time > _lastFire + _realFireRate)
            {
                var r = _rockets.Get();
                r.InitProjectile();
                Transform rt = r.transform;
                rt.position = shootPoint.position;
                rt.rotation = shootPoint.rotation;
                r.direction = shootPoint.forward;
                r.target = _camT;
                _lastFire = Time.time;
            }
        }
        else
            IdleRotate();
    }

    private void TiltTowardsPlayer()
    {
        Vector3 toPlayer = (_t.position - _camT.position).normalized;
        Vector3 a = Quaternion.LookRotation(toPlayer).eulerAngles;
        a.x = -a.x;
        Quaternion hTP = Quaternion.Euler(a);
        Vector3 headRot = Quaternion
                          .RotateTowards(head.localRotation, hTP, Time.deltaTime * trackSpeed)
                          .eulerAngles;
        headRot.y = 0f;
        headRot.z = 0f;
        head.localRotation = Quaternion.Euler(headRot);
        Quaternion bTP = Quaternion.LookRotation(toPlayer) * Quaternion.Euler(0,0,90);
        Vector3 headBaseRot = Quaternion
                              .RotateTowards(headBase.localRotation, bTP, Time.deltaTime * trackSpeed)
                              .eulerAngles;
        headBaseRot.x = 0f;
        headBaseRot.y = 0f;
        headBase.localRotation = Quaternion.Euler(headBaseRot);
    }

    private void IdleRotate()
    {
        head.localRotation = Quaternion.RotateTowards(head.localRotation, Quaternion.identity, Time.deltaTime * rotateSpeed);
        headBase.Rotate(0,0, Time.deltaTime * rotateSpeed);
    }
    
    private Reaction ClimbHierarchy(Transform start)
    {
        if (start.parent == null) return null;
        Reaction item = start.parent.GetComponent<Reaction>();
        if (item == null)
            return ClimbHierarchy(start.parent);
        return item;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position,detectionRange);
    }
}
