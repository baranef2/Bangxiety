using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField]
    public float totalAmmo = 0;
    [SerializeField]
    public void AddAmmo(int amount)
        {
        totalAmmo += amount;
        Debug.Log("Player Ammo: " + totalAmmo);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
