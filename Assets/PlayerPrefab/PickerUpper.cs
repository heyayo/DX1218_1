using System.Collections.Generic;
using UnityEngine;

public class PickerUpper : MonoBehaviour
{
	private static readonly int _inventorySize = 5;
	[SerializeField] private int inventoryIndex = 0;
	[SerializeField] private List<Pickupable> inventory;
	
	[SerializeField] private float range = 2.5f;
	[SerializeField] private float throwForce = 5f;
	[SerializeField] private Transform gunMountPoint;
	
	private Camera cam;

	public Pickupable currentItem
	{
		get
		{
			if (inventory.Count <= 0) return null;
			return inventory[inventoryIndex];
		}
	}
	
	private void Awake()
	{
		cam = Camera.main;
	}
	
	private void Update()
	{
		PickupRayCast();
		DropWeapon();
	}

	private void DropWeapon()
	{
		if (Input.GetKeyDown(KeyCode.G))
		{
			if (inventory.Count <= 0) return;
			inventory[inventoryIndex].Detach();
			Rigidbody rb = inventory[inventoryIndex].GetComponent<Rigidbody>();
			rb.AddForce(cam.transform.forward * throwForce, ForceMode.Impulse);
			inventory.RemoveAt(inventoryIndex);
		}
	}

	private void PickupRayCast()
	{
		if (Input.GetKeyDown(KeyCode.F))
		{
			if (inventory.Count >= _inventorySize)
			{
				Debug.Log("Inventory Full");
				return;
			}
			Ray ray = new Ray();
			ray.origin = cam.transform.position;
			ray.direction = cam.transform.forward;
			bool hit = Physics.Raycast(ray, out RaycastHit info,range);

			Debug.Log("Shot Ray");
			if (hit)
			{
				if (info.collider.CompareTag("Weapon"))
				{
					Debug.Log("Ray Hit Weapon");
					var comp = info.collider.GetComponent<Pickupable>();
					comp.Attach(gunMountPoint);
					inventory.Add(comp);
				}
			}
		}
	}
}
