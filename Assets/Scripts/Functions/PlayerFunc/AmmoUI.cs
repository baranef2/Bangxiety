using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class AmmoUI : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private TMP_Text ammoText;

    void Update()
    {
        if (player!= null && ammoText != null)
        {
            ammoText.text = "Ammo: " + player.TotalAmmo.ToString();
        }
    }
}
