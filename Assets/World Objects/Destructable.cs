using UnityEngine;
using UnityEngine.Events;

public class Destructable : MonoBehaviour
{
	[SerializeField] private float health;
	[SerializeField] private UnityEvent onDestroyed;

	public void TakeDamage(float dmg)
	{
		health -= dmg;
		if (health <= 0)
		{
			Destroy(gameObject);
			onDestroyed.Invoke();
		}
	}
}
