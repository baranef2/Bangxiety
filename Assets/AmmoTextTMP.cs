// AmmoUI_TMP.cs
using UnityEngine;
using TMPro;

public class AmmoUI_TMP : MonoBehaviour
{
    [SerializeField] private Player player;       // HumanPlayer
    [SerializeField] private TMP_Text ammoText;   // HIERARCHY�deki ger�ek instance

    private void Awake()
    {
        if (!player)
            player = GameObject.FindGameObjectWithTag("HumanPlayer")?.GetComponent<Player>();
        if (!ammoText)
            ammoText = GetComponentInChildren<TMP_Text>(true);
    }

    private void Update()
    {
        // MissingReference�a kar�� g�venli guard:
        if (!player || !ammoText) return;
        ammoText.text = $"Ammo = {player.TotalAmmo}";
    }
}
