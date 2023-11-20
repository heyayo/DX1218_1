using UnityEngine;

public class Pickupable : MonoBehaviour
{
	private Rigidbody _rigidbody;
	private Collider _collider;
	
	private void Awake()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_collider = GetComponent<Collider>();
	}
	
	public void Attach(Transform t)
	{
		transform.parent = t;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		_collider.enabled = false;
		_rigidbody.isKinematic = true;
	}

	public void Detach()
	{
		transform.parent = null;
		_collider.enabled = true;
		_rigidbody.isKinematic = false;
	}
}
