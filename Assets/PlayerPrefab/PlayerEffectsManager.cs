using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerEffectsManager : MonoBehaviour
{
    private CameraController _camCon;
    private MovementController _movCon;

    private bool _alreadyShaking = false;
    
    private void Awake()
    {
        _camCon = GetComponent<CameraController>();
        _movCon = GetComponent<MovementController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            StartCoroutine(ShakeCamera(2, 1));
        if (Input.GetKeyDown(KeyCode.F))
            StartCoroutine(SpikeRecoil(25, 2));
    }
    
    public IEnumerator ShakeCamera(float shakeTime, float shakeIntensity)
    {
        // Lock Shaking to one Stack at a time ( Prevent Over Shaking ) | Possibly Remove in the Future
        if (_alreadyShaking) yield break;
        _alreadyShaking = true;
        
        float startTime = 0f;
        CameraOffset offset = new CameraOffset();
        _camCon.offsets.Add(offset);
        while (startTime < shakeTime)
        {
            startTime += Time.deltaTime;

            offset.pitch = Random.Range(-1f, 1f) * shakeIntensity;
            offset.yaw = Random.Range(-1f, 1f) * shakeIntensity;
            
            yield return null;
        }

        _camCon.offsets.Remove(offset);
        _alreadyShaking = false;
    }

    public IEnumerator SpikeRecoil(float vertical, float horizontal)
    {
        CameraOffset offset = new CameraOffset();
        _camCon.offsets.Add(offset);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 10;
            offset.pitch = t * -vertical;
            offset.yaw = t * horizontal;
            yield return null;
        }

        while (t > 0)
        {
            t -= Time.deltaTime * 20;
            offset.pitch = t * -vertical;
            offset.yaw = t * horizontal;
            yield return null;
        }
        
        _camCon.offsets.Remove(offset);
    }
}
