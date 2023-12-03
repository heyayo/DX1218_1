using System;
using UnityEngine;

[Serializable]
public class Inventory
{
    public Item[] items;
    private int currentIndex;
    private int size;

    public int CurrentIndex
    {
        get => currentIndex;
        set { currentIndex = value; }
    }

    public int Size
    { get => size; }

    public Inventory(int pCapacity)
    {
        items = new Item[pCapacity];
        for (int i = 0; i < pCapacity; ++i)
        {
            items[i] = null;
        }

        currentIndex = 0;
        size = 0;
    }

    public Item currentlyHolding()
    {
        return items[currentIndex];
    }

    public bool hasSpace()
    {
        return items.Length > size;
    }

    public void AddItem(Item item)
    {
        if (!item) return;
        for (int i = 0; i < items.Length; ++i)
        {
            if (!items[i])
            {
                items[i] = item;
                return;
            }
        }

        ++size;
    }

    public void RemoveItem(Item item)
    {
        for (int i = 0; i < items.Length; ++i)
        {
            if (items[i] == item)
            {
                items[i] = null;
                --size;
                return;
            }
        }
    }

    public Item RemoveItem(int item)
    {
        Item t = items[item];
        if (!t) return null;
        items[item] = null;
        --size;
        return t;
    }
}

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private Transform holdPoint;
    [SerializeField] private Inventory inventory;
    
    [Header("Player Values")]
    [SerializeField] private float range;
    [SerializeField] private float throwForce;

    private Camera _cam;
    private Transform _camT;

    private KeyCode[] keys =
    {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
    };

    private delegate bool FireType(int key);
    private FireType _fireType;
    private FireType _altFireType;

    private void Awake()
    {
        inventory = new Inventory(5);
        _cam = Camera.main;
        _camT = _cam.transform;
        _fireType = Input.GetMouseButtonDown;
        _altFireType = Input.GetMouseButtonDown;
    }
    
    private void Update()
    {
        PickupItem();
        DropItem();
        UseItem();
        SwitchItem();
        ReloadItem();
    }

    private void SwitchItem()
    {
        for (int i = 0; i < keys.Length; ++i)
        {
            if (Input.GetKeyDown(keys[i]))
            {
                SwitchTo(i);
            }
        }
    }

    private void UseItem()
    {
        // Left Click to Use Item
        if (_fireType(0))
        {
            var item = inventory.currentlyHolding();
            if (item)
            {
                item.Use();
                item.onUse.Invoke();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            var item = inventory.currentlyHolding();
            if (item != null)
                item.onUseRelease.Invoke();
        }

        if (_altFireType(1))
        {
            var item = inventory.currentlyHolding();
            if (item)
            {
                item.AltUse();
                item.onAltUse.Invoke();
            }
        }
    }
    
    private void DropItem()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Item item = inventory.RemoveItem(inventory.CurrentIndex);
            if (item)
            {
                item.Detach();
                item.rigidbody.AddForce(_cam.transform.forward * throwForce, ForceMode.Impulse);
            }
        }
    }


    private void PickupItem()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!inventory.hasSpace()) return;
            Ray ray = new Ray();
            ray.origin = _camT.position;
            ray.direction = _camT.forward;
            bool hit = Physics.Raycast(ray, out RaycastHit info,range);

            if (hit)
            {
                if (info.collider.CompareTag("Weapon"))
                {
                    Debug.Log("Ray Hit Weapon");
                    var comp = info.collider.GetComponent<Item>();
                    if (comp == null)
                        comp = ClimbHierarchy(info.collider.transform);
                    comp.Attach(holdPoint);
                    inventory.AddItem(comp);
                    comp.gameObject.SetActive(false);
                    SwitchTo(inventory.CurrentIndex);
                }
                else if (info.collider.CompareTag("Mountable"))
                {
                    Debug.Log("Ray Hit Mountable");
                    var comp = info.collider.GetComponent<Mountable>();
                    comp.Mount();
                }
            }
        }
    }

    private void ReloadItem()
    {
        if (Input.GetKeyDown(KeyCode.R))
            inventory.currentlyHolding().onReload.Invoke();
    }

    private Item ClimbHierarchy(Transform start)
    {
        if (start.parent == null) return null;
        Item item = start.parent.GetComponent<Item>();
        if (item == null)
            return ClimbHierarchy(start.parent);
        return item;
    }

    private void SwitchTo(int index)
    {
        Item item = inventory.currentlyHolding();
        if (item)
            item.gameObject.SetActive(false);
        inventory.CurrentIndex = index;
        item = inventory.currentlyHolding();
        if (item)
        {
            inventory.currentlyHolding().gameObject.SetActive(true);
            _fireType = inventory.currentlyHolding().reusable ? Input.GetMouseButton : Input.GetMouseButtonDown;
            _altFireType = inventory.currentlyHolding().reusableAlt ? Input.GetMouseButton : Input.GetMouseButtonDown;
        }
    }

    public void DropHoldingItem()
    {
        var item = inventory.currentlyHolding();
        if (item == null) return;
        item.Detach();
        inventory.RemoveItem(inventory.currentlyHolding());
    }
}
