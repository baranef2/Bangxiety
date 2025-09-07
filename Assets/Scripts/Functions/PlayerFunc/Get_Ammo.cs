using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class Get_Ammo : MonoBehaviour
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
            Debug.LogError("Get_Ammo: HumanPlayer bulunamadý (Tag kontrol et).");
            return;
        }

        humanPlayer.AddAmmo(1);
    }
}
