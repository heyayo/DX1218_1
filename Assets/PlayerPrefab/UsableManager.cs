using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsableManager : MonoBehaviour
{
    // TODO Make System to determine auto or semi weapon | Maybe make way for grenades instead of guns too
    private PickerUpper _pickerUpper;

    private void Awake()
    {
        _pickerUpper = GetComponent<PickerUpper>();
    }

    private void Update()
    {
        UseItem();
    }

    private void UseItem()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_pickerUpper.currentItem != null)
            {
                if (_pickerUpper.currentItem.TryGetComponent(out Weapon comp))
                {
                    Debug.Log("Used Item");
                    comp.Use();
                }
                else
                    Debug.Log("Item is not usable");
            }
        }
    }
}
