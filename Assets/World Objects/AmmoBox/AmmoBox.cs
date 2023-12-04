using UnityEngine;

public class AmmoBox : Reaction
{
    [SerializeField] private int giveAmmoCount;
    [SerializeField] private AmmoCounter ammoContainer;

    public override void Interact(float damage, float force)
    {
        ammoContainer.reserve += giveAmmoCount;
        Destroy(gameObject);
    }
}
