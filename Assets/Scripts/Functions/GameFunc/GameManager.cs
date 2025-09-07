using UnityEngine;
using System.Collections.Generic;

public enum CardChoice { None, GetAmmo, Shoot, Protect, Execute }

public class RoundAction
{
    public CardChoice choice = CardChoice.None;
    public Player target = null;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Prefabs")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject executerPrefab;
    private void SpawnExecuter(Vector3 from, Transform to)
    {
        var go = Instantiate(executerPrefab, from, Quaternion.identity);
        var b = go.GetComponent<Bullet>(); // ya da özel Executer.cs
        if (b) b.Initialize(to);
    }

    // Bu raundda herkesin seçimi
    private readonly Dictionary<Player, RoundAction> choices = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // ---- SEÇÝM API'si ----
    public bool HasChosen(Player p)
    {
        return choices.TryGetValue(p, out var a) && a.choice != CardChoice.None;
    }

    RoundAction Ensure(Player p)
    {
        if (!choices.TryGetValue(p, out var a))
        {
            a = new RoundAction();
            choices[p] = a;
        }
        return a;
    }

    public bool SelectGetAmmo(Player p)
    {
        var a = Ensure(p);
        if (a.choice != CardChoice.None) { Debug.Log($"{p.name} zaten {a.choice} seçti."); return false; }
        a.choice = CardChoice.GetAmmo;

        p.IncrementGetAmmoCount();  // <<--- EKLENDÝ

        Debug.Log($"{p.name} kart seçti: GET_AMMO");
        return true;
    }
    public bool SelectProtect(Player p)
    {
        var a = Ensure(p);
        if (a.choice != CardChoice.None) { Debug.Log($"{p.name} zaten {a.choice} seçti."); return false; }
        a.choice = CardChoice.Protect;
        Debug.Log($"{p.name} kart seçti: PROTECT");
        return true;
    }

    public bool SelectShoot(Player shooter, Player target)
    {
        var a = Ensure(shooter);

        if (a.choice != CardChoice.None) { Debug.Log($"{shooter.name} zaten {a.choice} seçti."); return false; }
        if (shooter == null || !shooter.IsAlive) { Debug.Log($"{shooter?.name} ölü, SHOOT seçemez."); return false; }
        if (target == null || !target.IsAlive) { Debug.Log("Hedef ölü veya null, SHOOT reddedildi."); return false; }
        if (ReferenceEquals(shooter, target)) { Debug.Log("Oyuncu kendini hedefleyemez."); return false; }

        // Mermiyi seçim ANINDA rezerve et (exploit'i kapat)
        if (!shooter.UseAmmo(1)) { Debug.Log($"{shooter.name} yeterli mermi yok."); return false; }

        a.choice = CardChoice.Shoot;
        a.target = target;
        Debug.Log($"{shooter.name} kart seçti: SHOOT -> {target.name}");
        return true;
    }


    // ---- ROUND UYGULAMA ----
    public void ExecuteRound()
    {
        // 1) Protect'leri etkinleþtir
        foreach (var kv in choices)
            if (kv.Value.choice == CardChoice.Protect)
                kv.Key.EnableProtection();

        // 2) Shoot'larý uygula (mermi + hasar)
        // 2) Shoot'larý uygula (görsel + etki)
        // NOT: Mermi SEÇÝMDE düþürüldü; burada tekrar düþürme yok!
        foreach (var kv in choices)
        {
            var shooter = kv.Key;
            var a = kv.Value;

            if (a.choice != CardChoice.Shoot || !shooter.IsAlive) continue;

            if (bulletPrefab)
                SpawnBullet(shooter.transform.position, a.target.transform);

            if (!a.target.IsProtected && a.target.IsAlive)
                a.target.Kill();
            else
                Debug.Log($"{a.target.name} korundu (Shoot etkisiz).");

        }



        // 3) Get_Ammo'larý uygula
        // 3) Get_Ammo'larý uygula (SADECE YAÞAYANLARA)
        foreach (var kv in choices)
            if (kv.Value.choice == CardChoice.GetAmmo && kv.Key.IsAlive)
                kv.Key.AddAmmo(1);


        // 4) Temizlik
        foreach (var p in FindObjectsOfType<Player>())
            p.ResetProtection();
        foreach (var kv in choices)
        {
            var shooter = kv.Key;
            var a = kv.Value;

            if (a.choice == CardChoice.Execute && shooter.IsAlive)
            {
                if (executerPrefab)
                    SpawnExecuter(shooter.transform.position, a.target.transform);

                // Execute korumayý deliyor
                if (a.target.IsAlive)
                    a.target.Kill();
                else
                    Debug.Log($"{a.target.name} zaten ölüydü (Execute boþa).");
            }
        }


        choices.Clear();
        Debug.Log("Round bitti.");
    }

    private void SpawnBullet(Vector3 from, Transform to)
    {
        var go = Instantiate(bulletPrefab, from, Quaternion.identity);
        var b = go.GetComponent<Bullet>();
        if (b) b.Initialize(to);
    }

    public bool SelectExecute(Player shooter, Player target)
    {
        var a = Ensure(shooter);

        if (a.choice != CardChoice.None) return false;
        if (shooter == null || !shooter.IsAlive) return false;
        if (target == null || !target.IsAlive) return false;
        if (ReferenceEquals(shooter, target)) return false;

        if (shooter.TotalGetAmmoCount < 5)
        {
            Debug.Log($"{shooter.name} yeterli execute hakký yok.");
            return false;
        }

        if (shooter.TotalAmmo < 5)
        {
            Debug.Log($"{shooter.name} execute için yeterli mermiye sahip deðil.");
            return false;
        }
        Debug.Log($"{shooter.name} ammo BEFORE: {shooter.TotalAmmo}");

        bool used = shooter.UseAmmo(5);

        Debug.Log($"UseAmmo(5) sonucu: {used}");
        Debug.Log($"{shooter.name} ammo AFTER: {shooter.TotalAmmo}");

        if (!used)
        {
            return false;            
        }

        a.choice = CardChoice.Execute;
        a.target = target;
        Debug.Log($"{shooter.name} kart seçti: EXECUTE -> {target.name}");
        return true;

    }




}
