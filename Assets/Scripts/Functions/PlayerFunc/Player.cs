using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int totalAmmo = 0;
    public int TotalAmmo => totalAmmo;

    public bool IsProtected { get; private set; } = false;
    public bool IsAlive { get; private set; } = true;

    public void AddAmmo(int amount)
    {
        totalAmmo += amount;
        Debug.Log($"{name} -> AddAmmo(+{amount}) | Ammo={totalAmmo}");
    }

    public bool UseAmmo(int amount)
    {
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
        IsAlive = false;

        // Sahnede kalsýn, görsel olarak "öldü"
        var r = GetComponentInChildren<Renderer>();
        if (r != null) r.material.color = Color.red;

        Debug.Log($"{name} öldü!");
    }
}
