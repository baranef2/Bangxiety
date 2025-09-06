using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class Shoot : MonoBehaviour
{
    [SerializeField] private Player player;

    private void Reset()
    {
        if (!TryGetComponent<Collider>(out _)) gameObject.AddComponent<BoxCollider>();
    }

    private void OnMouseDown()
    {
        Debug.Log("Shoot kart�na t�kland�!"); // TEST

        if (player == null)
        {
            player = FindFirstObjectByType<Player>();
            Debug.Log("Player bulunmaya �al���ld�: " + (player != null ? player.name : "BULUNAMADI"));
        }

        if (player == null) return;

        if (player.TotalAmmo <= 0)
        {
            Debug.Log("Yeterli mermi yok. Ammo=" + player.TotalAmmo);
            return;
        }

        Debug.Log("Shoot kart� se�ildi. TargetSelectorUI �a�r�l�yor...");
        if (TargetSelectorUI.Instance == null)
        {
            Debug.LogError("TargetSelectorUI sahnede yok!");
            return;
        }

        TargetSelectorUI.Instance.ShowTargetOptions(player, OnTargetSelected);
    }

    private void OnTargetSelected(Player target)
    {
        // Mermiyi hemen harca ve aksiyonu kaydet
        if (player.UseAmmo(1))
        {
            GameManager.Instance.RegisterShoot(player, target);
            Debug.Log($"{player.name} -> SHOOT -> {target.name} kaydedildi.");
        }
        else
        {
            Debug.Log("Mermi t�ketilemedi.");
        }
    }
}
