using UnityEngine;


[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class Shoot : MonoBehaviour
{
    [SerializeField] private Player humanPlayer;

    private void Awake()
    {
        if (!humanPlayer)
        {
            var go = GameObject.FindGameObjectWithTag("HumanPlayer");
            if (go) humanPlayer = go.GetComponent<Player>();
        }

        // Kart mutlaka t�klanabilir olsun
        if (TryGetComponent<Collider>(out var col) && col.isTrigger)
            col.isTrigger = false;
    }

    private void Reset()
    {
        if (!TryGetComponent<Collider>(out _)) gameObject.AddComponent<BoxCollider>();
    }

    private void OnMouseDown()
    {
        // TE�H�S LOG�U: Neden a��lm�yor?
        Debug.Log($"[Shoot] Click -> " +
            $"human={(humanPlayer ? humanPlayer.name : "NULL")}, " +
            $"alive={(humanPlayer ? humanPlayer.IsAlive : false)}, " +
            $"hasChosen={(humanPlayer ? GameManager.Instance.HasChosen(humanPlayer) : false)}, " +
            $"ammo={(humanPlayer ? humanPlayer.TotalAmmo : -1)}");

        if (!humanPlayer) { Debug.LogError("Shoot: HumanPlayer bulunamad� (Tag 'HumanPlayer' verildi mi?)."); return; }
        if (!humanPlayer.IsAlive) { Debug.Log("�l� oyuncu kart se�emez."); return; }
        if (GameManager.Instance.HasChosen(humanPlayer)) { Debug.Log("Bu raundda zaten se�im yapt�n."); return; }
        if (humanPlayer.TotalAmmo <= 0) { Debug.Log("Yeterli mermi yok."); return; }

        if (TargetSelectorUI.Instance == null) { Debug.LogError("Shoot: TargetSelectorUI sahnede yok."); return; }

        Debug.Log("[Shoot] TargetSelectorUI a��l�yor�");
        TargetSelectorUI.Instance.ShowTargetOptions(humanPlayer, OnTargetSelected);
        

    }

    private void OnTargetSelected(Player target)
    {
        bool ok = GameManager.Instance.SelectShoot(humanPlayer, target);
        Debug.Log(ok ? $"[Shoot] Hedef se�ildi: {target.name}" : "[Shoot] Se�im reddedildi.");
    }
}
