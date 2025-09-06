using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class Get_Ammo : MonoBehaviour
{
    [SerializeField] private Player player;
    private void Reset()
    {
        if (!TryGetComponent<Collider>(out _))
            gameObject.AddComponent<BoxCollider>();
    }

    private void OnMouseDown()
    {
        // Inspector’dan atanmamýþsa sahnede ilk Player’ý bul
        if (player == null)
            player = FindFirstObjectByType<Player>(); // Unity 2022.3’te mevcut

        if (player == null)
        {
            Debug.LogWarning("Get_Ammo: Sahne içinde Player bulunamadý.");
            return;
        }

        player.AddAmmo(1);
    }
}
    
    
    // Start is called before the first frame update
   

    // Update is called once per frame
    

