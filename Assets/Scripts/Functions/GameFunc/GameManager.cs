using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Mermi Prefab")]
    [SerializeField] private GameObject bulletPrefab;

    // shooter -> target
    private readonly Dictionary<Player, Player> shootActions = new();
    private readonly HashSet<Player> protectActions = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // --- Round API ---
    public void RegisterShoot(Player shooter, Player target)
    {
        if (shooter == null || target == null) return;
        shootActions[shooter] = target;
    }

    public void RegisterProtect(Player player)
    {
        if (player == null) return;
        protectActions.Add(player);
    }

    public void ExecuteRound()
    {
        // 1) Protect’leri etkinleþtir
        foreach (var p in protectActions)
            p.EnableProtection();

        // 2) Shoot aksiyonlarýný uygula (mermi spawn + sonuç)
        foreach (var kv in shootActions)
        {
            var shooter = kv.Key;
            var target = kv.Value;

            // Mermi görseli
            SpawnBullet(shooter.transform.position, target.transform);

            // Sonuç: Protect varsa ölmez, yoksa ölür
            if (!target.IsProtected && target.IsAlive)
                target.Kill();
            else
                Debug.Log($"{target.name} korundu.");
        }

        // 3) Temizlik ve bir sonraki round'a hazýrlýk
        shootActions.Clear();
        protectActions.Clear();

        foreach (var p in FindObjectsOfType<Player>())
            p.ResetProtection();

        Debug.Log("Round bitti.");
    }

    private void SpawnBullet(Vector3 from, Transform to)
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning("GameManager: bulletPrefab atanmamýþ.");
            return;
        }

        var bulletGO = Instantiate(bulletPrefab, from, Quaternion.identity);
        if (bulletGO.TryGetComponent<Bullet>(out var bullet))
        {
            bullet.Initialize(to);
        }
        else
        {
            Debug.LogWarning("Bullet prefabýnda Bullet scripti yok.");
        }
    }
}
