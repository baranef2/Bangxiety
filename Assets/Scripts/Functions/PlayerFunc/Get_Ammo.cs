using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class Get_Ammo : MonoBehaviour
{
    [SerializeField] private Player humanPlayer;

    private void Awake()
    {
        if (!humanPlayer)
        {
            var go = GameObject.FindGameObjectWithTag("HumanPlayer");
            if (go) humanPlayer = go.GetComponent<Player>();
        }
    }

    private void Reset()
    {
        if (!TryGetComponent<Collider>(out _)) gameObject.AddComponent<BoxCollider>();
    }

    private void OnMouseDown()
    {
        
        if (!humanPlayer.IsAlive) { Debug.Log("Ölü oyuncu kart seçemez."); return; }
        if (!humanPlayer) { Debug.LogError("Get_Ammo: HumanPlayer yok."); return; }
        if (GameManager.Instance.HasChosen(humanPlayer)) { Debug.Log("Bu raundda zaten seçim yaptýn."); return; }

        GameManager.Instance.SelectGetAmmo(humanPlayer); // etkisi ExecuteRound’da
    }
}
