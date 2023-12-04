using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthModule : MonoBehaviour
{
    [SerializeField] private float healthValue;

    public float health
    {
        get => healthValue;
        set
        {
            healthValue = value;
            if (healthValue <= 0)
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
