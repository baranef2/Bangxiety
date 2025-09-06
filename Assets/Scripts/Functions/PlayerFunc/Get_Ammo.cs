using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Get_Ammo : MonoBehaviour
{
    private Camera = mainCamera;
    public void OnMouseDown()
    {
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.getAmmo();
           
        }
    }
}
    
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;   
}

    // Update is called once per frame
    void Update()
    {
        
    }
}
