using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JavelinLocker : MonoBehaviour
{
    [SerializeField] private MeshRenderer self;
    [SerializeField] private Material lockingOn;
    [SerializeField] private Material lockedOn;
    [SerializeField] private Transform player;
    [SerializeField] private float lockingSpeed;
    [SerializeField] private float lockingDelta;

    private Camera _cam;
    private Transform _t;
    private Transform _camT;
    private Transform _closestTarget;
    private bool _enabled;

    public List<Transform> allPossibleTargets;
    public Transform lockedOnTarget;

    private void Awake()
    {
        _cam = Camera.main;
        _t = transform;
        _camT = _cam.transform;
        self = GetComponent<MeshRenderer>();
        gameObject.SetActive(false);
    }

    private void Update()
    {
        _t.LookAt(_camT.position);
        _t.rotation *= Quaternion.Euler(90,0,0);
        
        if (_closestTarget == null) return;

        _t.position = Vector3.MoveTowards(_t.position, _closestTarget.position, lockingSpeed * Time.deltaTime);
        Vector3 diff = _closestTarget.position - _t.position;
        if (diff.magnitude < lockingDelta)
        {
            lockedOnTarget = _closestTarget;
            self.material = lockedOn;
        }
        else
        {
            lockedOnTarget = null;
            self.material = lockingOn;
        }
    }

    private void FindClosestTarget()
    {
        if (allPossibleTargets.Count <= 0) return;
        Transform closest = allPossibleTargets[0];
        float closestDist = Single.MaxValue;

        foreach (var target in allPossibleTargets)
        {
            var distToCenter = GetDistToCenter(target.position);
            if (distToCenter.magnitude < closestDist)
            {
                closest = target;
                closestDist = distToCenter.magnitude;
            }
        }

        _closestTarget = closest;
    }

    private Vector3 GetDistToCenter(Vector3 pos)
    {
        Vector3 vpos = _cam.WorldToViewportPoint(pos);
        return new Vector3
            (
            Mathf.Abs(vpos.x - 0.5f),
            Mathf.Abs(vpos.y - 0.5f),
            0
            );
    }

    public void EnableLocker(bool enable)
    {
        _enabled = enable;
        self.material = lockingOn;
        if (enable)
            InvokeRepeating("FindClosestTarget",0,1);
        else
            CancelInvoke("FindClosestTarget");
    }
}
