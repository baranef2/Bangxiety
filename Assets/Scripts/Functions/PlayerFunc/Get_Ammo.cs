using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class Get_Ammo : MonoBehaviour
{
    [SerializeField] private Player player;

    private void Reset()
    {
        if (!TryGetComponent<Collider>(out _)) gameObject.AddComponent<BoxCollider>();
    }

    private void OnMouseDown()
    {
        if (player == null) player = FindFirstObjectByType<Player>();
        if (player == null) { Debug.LogWarning("Get_Ammo: Sahnede Player yok."); return; }

        player.AddAmmo(1);
    }
}
