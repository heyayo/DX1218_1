using UnityEngine;
using UnityEngine.Events;

public class Destructable : Reaction
{
	[SerializeField] private GameObject rootObject;
	[SerializeField] private float health;
	[SerializeField] private UnityEvent onDestroyed;

	private void Awake()
	{
		if (!rootObject)
			rootObject = gameObject;
	}
	
	public override void Interact(float damage, float force)
	{
		health -= damage;
		if (health <= 0)
			onDestroyed.Invoke();
	}
}
