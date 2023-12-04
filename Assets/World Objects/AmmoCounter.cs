using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Ammo/Ammo",fileName = "Generic Ammo", order = 0)]
public class AmmoCounter : ScriptableObject
{
    [SerializeField] private int real_reserve;
    public UnityEvent onReserveChanged = new UnityEvent();

    public int reserve
    {
        get => real_reserve;
        set
        {
            real_reserve = value;
            onReserveChanged.Invoke();
        }
    }
}
