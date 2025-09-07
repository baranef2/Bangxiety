using UnityEngine;
//using Assets.Scripts.Enums.LifeStatus;

[DisallowMultipleComponent]
public class Player : MonoBehaviour
{
    [SerializeField] private int totalAmmo = 0;
    public int TotalAmmo => totalAmmo;

    public bool IsProtected { get; private set; } = false;
    public enum LifeStatus { Alive, Dead }


    public LifeStatus Status { get; private set; } = LifeStatus.Alive;
    public bool IsAlive => Status == LifeStatus.Alive;

    public void AddAmmo(int amount)
    {
        if (!IsAlive) return;
        totalAmmo += amount;
        Debug.Log($"{name} -> AddAmmo(+{amount}) | Ammo={totalAmmo}");
    }

    public bool UseAmmo(int amount)
    {
        if (!IsAlive) return false;
        if (totalAmmo < amount) return false;
        totalAmmo -= amount;
        Debug.Log($"{name} -> UseAmmo(-{amount}) | Ammo={totalAmmo}");
        return true;
    }

    public void EnableProtection() => IsProtected = true;
    public void ResetProtection() => IsProtected = false;

    public void Kill()
    {
        if (!IsAlive) return;
        Status = LifeStatus.Dead;


        // Sahnede kalsýn, görsel olarak "öldü"
        var r = GetComponentInChildren<Renderer>();
        if (r != null) r.material.color = Color.red;

        Debug.Log($"{name} öldü!");
    }
    public int TotalGetAmmoCount { get; private set; } = 0;
    public void IncrementGetAmmoCount() => TotalGetAmmoCount++;

}
