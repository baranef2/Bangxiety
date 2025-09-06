using UnityEngine;

[RequireComponent(typeof(Player))]
public class BotPlayer : MonoBehaviour
{
    private Player player;

    private void Awake() => player = GetComponent<Player>();

    public void ChooseCard(Player[] targets)
    {
        if (player == null || !player.IsAlive) return;

        int choice = Random.Range(0, 3); // 0:Get_Ammo, 1:Shoot, 2:Protect
        if (choice == 1 && player.TotalAmmo <= 0) choice = 0;

        switch (choice)
        {
            case 0: // Get_Ammo
                player.AddAmmo(1);
                Debug.Log($"{name} BOT -> Get_Ammo");
                break;

            case 1: // Shoot
                if (player.UseAmmo(1))
                {
                    // Kendisi dýþýndaki rastgele bir hedef
                    var validTargets = System.Array.FindAll(targets, t => t != null && t != player && t.IsAlive);
                    if (validTargets.Length > 0)
                    {
                        var target = validTargets[Random.Range(0, validTargets.Length)];
                        GameManager.Instance.RegisterShoot(player, target);
                        Debug.Log($"{name} BOT -> Shoot -> {target.name}");
                    }
                }
                break;

            case 2: // Protect
                GameManager.Instance.RegisterProtect(player);
                Debug.Log($"{name} BOT -> Protect");
                break;
        }
    }
}
