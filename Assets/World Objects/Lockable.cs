using System;
using UnityEngine;

public class Lockable : MonoBehaviour
{
    [SerializeField] private JavelinLocker locker;
    [SerializeField] public Transform point;

    private void OnEnable()
    { locker.allPossibleTargets.Add(point); }
    private void OnDisable()
    { locker.allPossibleTargets.Remove(point); }
    private void OnDestroy()
    { locker.allPossibleTargets.Remove(point); }
    private void OnBecameInvisible()
    { locker.allPossibleTargets.Remove(point); }
    private void OnBecameVisible()
    { locker.allPossibleTargets.Add(point); }
}
