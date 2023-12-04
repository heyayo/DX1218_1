using TMPro;
using UnityEngine;

public class AmmoTextCounterManagerScript : MonoBehaviour
{
    [Header("Pistol")]
    [SerializeField] private TMP_Text pistolText;
    [SerializeField] private AmmoCounter pistolAmmo;
    [Header("Rifle")]
    [SerializeField] private TMP_Text rifleText;
    [SerializeField] private AmmoCounter rifleAmmo;
    [Header("Sniper")]
    [SerializeField] private TMP_Text sniperText;
    [SerializeField] private AmmoCounter sniperAmmo;
    [Header("Rocket")]
    [SerializeField] private TMP_Text rocketText;
    [SerializeField] private AmmoCounter rocketAmmo;

    public void UpdatePistol()
    { pistolText.text = "Pistol Ammo: " + pistolAmmo.reserve; }
    public void UpdateRifle()
    { rifleText.text = "Rifle Ammo: " + rifleAmmo.reserve; }
    public void UpdateSniper()
    { sniperText.text = "Sniper Ammo: " + sniperAmmo.reserve; }
    public void UpdateRocket()
    { rocketText.text = "Rocket Ammo: " + rocketAmmo.reserve; }

    private void Start()
    {
        pistolAmmo.onReserveChanged.AddListener(UpdatePistol);
        rifleAmmo.onReserveChanged.AddListener(UpdateRifle);
        sniperAmmo.onReserveChanged.AddListener(UpdateSniper);
        rocketAmmo.onReserveChanged.AddListener(UpdateRocket);
        
        UpdatePistol();
        UpdateRifle();
        UpdateSniper();
        UpdateRocket();
    }
}
