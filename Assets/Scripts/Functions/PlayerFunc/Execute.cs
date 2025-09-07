using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class Execute : MonoBehaviour
{
    [SerializeField] private Player humanPlayer;

    private void Awake()
    {
        if (!humanPlayer)
        {
            var go = GameObject.FindGameObjectWithTag("HumanPlayer");
            if (go) humanPlayer = go.GetComponent<Player>();
        }

        if (TryGetComponent<Collider>(out var col) && col.isTrigger)
            col.isTrigger = false;
    }

    private void OnMouseDown()
    {
        if (!humanPlayer) return;
        if (!humanPlayer.IsAlive) return;
        if (GameManager.Instance.HasChosen(humanPlayer)) return;

        if (humanPlayer.TotalGetAmmoCount < 5)
        {
            Debug.Log("Execute hakký yok.");
            return;
        }

        TargetSelectorUI.Instance.ShowTargetOptions(humanPlayer, OnTargetSelected);
    }

    private void OnTargetSelected(Player target)
    {
        bool ok = GameManager.Instance.SelectExecute(humanPlayer, target);
        Debug.Log(ok ? $"[Execute] Hedef seçildi: {target.name}" : "[Execute] Seçim reddedildi.");
    }
}
