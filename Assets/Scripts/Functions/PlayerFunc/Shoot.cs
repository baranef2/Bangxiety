using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class Shoot : MonoBehaviour
{
    [SerializeField] private Player humanPlayer;

    private void Awake()
    {
        if (humanPlayer == null)
        {
            var go = GameObject.FindGameObjectWithTag("HumanPlayer");
            if (go != null) humanPlayer = go.GetComponent<Player>();
        }
    }

    private void Reset()
    {
        if (!TryGetComponent<Collider>(out _)) gameObject.AddComponent<BoxCollider>();
    }

    private void OnMouseDown()
    {
        if (humanPlayer == null)
        {
            Debug.LogError("Shoot: HumanPlayer bulunamadý (Tag kontrol et).");
            return;
        }

        if (humanPlayer.TotalAmmo <= 0)
        {
            Debug.Log("Yeterli mermi yok. (HumanPlayer)");
            return;
        }

        if (TargetSelectorUI.Instance == null)
        {
            Debug.LogError("Shoot: TargetSelectorUI sahnede yok.");
            return;
        }

        Debug.Log("Shoot: TargetSelectorUI çaðrýlýyor...");
        TargetSelectorUI.Instance.ShowTargetOptions(humanPlayer, OnTargetSelected);
    }

    private void OnTargetSelected(Player target)
    {
        if (!humanPlayer.UseAmmo(1))
        {
            Debug.Log("Mermi tüketilemedi.");
            return;
        }

        GameManager.Instance.RegisterShoot(humanPlayer, target);
        Debug.Log($"{humanPlayer.name} -> SHOOT -> {target.name} kaydedildi.");
    }
}
