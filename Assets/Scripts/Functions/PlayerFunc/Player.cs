using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int totalAmmo = 0;
    public bool isHuman = false;

    public int TotalAmmo => totalAmmo;

    public bool IsProtected { get; private set; } = false;
    public bool IsAlive { get; private set; } = true;

    public void AddAmmo(int amount)
    {
        totalAmmo += amount;
        Debug.Log($"{name} Ammo: {totalAmmo}");
    }

    public bool UseAmmo(int amount)
    {
        if (totalAmmo < amount) return false;
        totalAmmo -= amount;
        return true;
    }

    public void EnableProtection() => IsProtected = true;
    public void ResetProtection() => IsProtected = false;

    public void Kill()
    {
        if (!IsAlive) return;
        IsAlive = false;

        // Kaybolmak yerine "�ld�" g�rseli verelim
        var renderer = GetComponentInChildren<Renderer>();
        if (renderer != null)
            renderer.material.color = Color.red; // k�rm�z�ya boyans�n

        // �stersen rigidbody ekleyip yere d��mesini sa�layabilirsin:
        // var rb = gameObject.AddComponent<Rigidbody>();
        // rb.mass = 5f;

        Debug.Log($"{name} �ld�!");
    }

}
