using UnityEngine;

public class PickerUpper : MonoBehaviour
{
	[SerializeField] private GameObject currentWeapon;
	[SerializeField] private float range = 2.5f;
	[SerializeField] private Transform gunMountPoint;
	
	private Camera cam;

	private void Awake()
	{
		cam = Camera.main;
	}
	
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F))
		{
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
					var comp = info.collider.GetComponent<WeaponPickup>();
					comp.Pickup(gunMountPoint);
					currentWeapon = comp.gameObject;
				}
			}
		}
	}
}
