using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class PlayerEffectsManager : MonoBehaviour
{
    public static PlayerEffectsManager Instance;
    
    [System.Serializable]
    public class RecoilData
    {
        public float vertical;
        public float horizontal;
        public float kickTime;
        public float waitTime;
        public float recoveryTime;
    }
    
    private CameraController _camCon;
    private MovementController _movCon;

    private bool _alreadyShaking = false;
    
    private void Awake()
    {
        _camCon = GetComponent<CameraController>();
        _movCon = GetComponent<MovementController>();
        Instance = this;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ShakeCam(float time, float intensity)
    {
        StartCoroutine(ShakeCamera(time, intensity));
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

    public IEnumerator SpikeRecoil(RecoilData r)
    {
        CameraOffset offset = new CameraOffset();
        _camCon.offsets.Add(offset);

        // Kick up Camera as Recoil
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / r.kickTime;
            offset.pitch = t * -r.vertical;
            offset.yaw = t * r.horizontal;
            yield return null;
        }

        // Wait before recovering from Recoil
        yield return new WaitForSeconds(r.waitTime);

        // TODO MAYBE | Improve such that recovery accounts for any changes in Camera Look while recovering or waiting
        // Recoil Recovery back to original position
        while (t > 0)
        {
            t -= Time.deltaTime / r.recoveryTime;
            offset.pitch = t * -r.vertical;
            offset.yaw = t * r.horizontal;
            yield return null;
        }
        
        _camCon.offsets.Remove(offset);
    }
}
