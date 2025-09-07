using UnityEngine;

[RequireComponent(typeof(Player))]
public class BotPlayer : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
        Debug.Log($"[BotPlayer] {name} - Baþlangýç: Status={player.Status}, IsAlive={player.IsAlive}");
    }


    public void ChooseCard(Player[] allPlayers)
    {
        Debug.Log($"[BotPlayer] {name} ChooseCard() çaðrýldý");
        if (GameManager.Instance.HasChosen(player)) return;
        if (player == null || !player.IsAlive)
        {
            Debug.LogWarning($"[BotPlayer] {name} iþlem yapmýyor çünkü ölü.");
            return;
        }
            int choice = Random.Range(0, 3); // 0:GetAmmo 1:Shoot 2:Protect
        if (choice == 1 && player.TotalAmmo <= 0) choice = 0;

        switch (choice)
        {
            case 0:
                GameManager.Instance.SelectGetAmmo(player);
                break;

            case 1:
                var targets = System.Array.FindAll(
                    allPlayers,
                    p => p && p.IsAlive && !ReferenceEquals(p, player)
                );
                if (targets.Length == 0) { GameManager.Instance.SelectGetAmmo(player); break; }
                var t = targets[Random.Range(0, targets.Length)];
                GameManager.Instance.SelectShoot(player, t);  // SelectShoot tekrar canlý/kendisi kontrol eder
                break;

            case 2:
                GameManager.Instance.SelectProtect(player);
                break;
        }
    }

}

