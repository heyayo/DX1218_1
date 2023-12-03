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

    private Transform _t;
    private Transform _closestTarget;
    private bool _enabled;

    public List<Transform> allPossibleTargets;
    public Transform lockedOnTarget;

    private void Awake()
    {
        _t = transform;
        self = GetComponent<MeshRenderer>();
        gameObject.SetActive(false);
    }

    private void Update()
    {
        _t.rotation = Quaternion.LookRotation(-(player.position.normalized)) * Quaternion.Euler(-90,0,0);
        
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
        Vector3 lastDiff = closest.position - player.position;
        foreach (var target in allPossibleTargets)
        {
            Vector3 diff = target.position - player.position;
            if (lastDiff.magnitude > diff.magnitude)
                closest = target;
        }

        _closestTarget = closest;
    }

    public void EnableLocker(bool enable)
    {
        _enabled = enable;
        self.material = lockingOn;
        if (enable)
            InvokeRepeating("FindClosestTarget",0,2);
        else
            CancelInvoke("FindClosestTarget");
    }
}
