using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class Protect : MonoBehaviour
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

    private void Reset()
    {
        if (!TryGetComponent<Collider>(out _))
            gameObject.AddComponent<BoxCollider>();
    }

    private void OnMouseDown()
    {
        Debug.Log($"[Protect] Click -> " +
            $"human={(humanPlayer ? humanPlayer.name : "NULL")}, " +
            $"alive={(humanPlayer ? humanPlayer.IsAlive : false)}, " +
            $"hasChosen={(humanPlayer ? GameManager.Instance.HasChosen(humanPlayer) : false)}");

        if (!humanPlayer)
        {
            Debug.LogError("Protect: HumanPlayer bulunamadý (Tag 'HumanPlayer' verildi mi?).");
            return;
        }

        if (!humanPlayer.IsAlive)
        {
            Debug.Log("Ölü oyuncu kart seçemez.");
            return;
        }

        if (GameManager.Instance.HasChosen(humanPlayer))
        {
            Debug.Log("Bu raundda zaten seçim yaptýn.");
            return;
        }

        bool ok = GameManager.Instance.SelectProtect(humanPlayer);
        Debug.Log(ok ? "[Protect] Kart seçildi: PROTECT" : "[Protect] Seçim reddedildi.");
    }
}
