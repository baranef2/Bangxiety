using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField]
    public float totalBullets = 0;
    [SerializeField]
    public void getAmmo()
        {
        totalBullets +=1;
        Debug.Log("Ammo: " + totalBullets);
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
