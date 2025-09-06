using System;
using System.Collections.Generic;
using System.Linq;

namespace JamDemo
{
    public class RoundResolver
    {
        private GameFunc gameFunc;
        private PlayerFunc playerFunc;

        public RoundResolver(GameFunc gameFunc, PlayerFunc playerFunc)
        {
            this.gameFunc = gameFunc;
            this.playerFunc = playerFunc;
        }

        public void ResolveRound(List<PlayerModel> players, Dictionary<string, PlayerAction> playerActions)
        {
            // 1. �nce t�m aksiyonlar� kategorize et
            var shootActions = new List<(PlayerModel shooter, PlayerModel target)>();
            var protectActions = new List<PlayerModel>();
            var executeActions = new List<(PlayerModel executor, PlayerModel target)>();
            var ammoActions = new List<PlayerModel>();

            foreach (var player in players)
            {
                if (!playerActions.ContainsKey(player.PlayerId))
                    continue;

                var action = playerActions[player.PlayerId];

                switch (action.ActionType)
                {
                    case ActionType.Shoot:
                        var shootTarget = players.FirstOrDefault(p => p.PlayerId == action.TargetPlayerId);
                        if (shootTarget != null && player.Ammo > 0)
                        {
                            shootActions.Add((player, shootTarget));
                        }
                        break;

                    case ActionType.Protect:
                        // Art arda 3'ten fazla protect kontrol�
                        if (player.ConsecutiveProtects < 3)
                        {
                            protectActions.Add(player);
                        }
                        break;

                    case ActionType.Execute:
                        var executeTarget = players.FirstOrDefault(p => p.PlayerId == action.TargetPlayerId);
                        if (executeTarget != null && player.Ammo >= 5)
                        {
                            executeActions.Add((player, executeTarget));
                        }
                        break;

                    case ActionType.Ammo:
                        ammoActions.Add(player);
                        break;
                }
            }

            // 2. �nce Execute aksiyonlar�n� ��z�mle (en y�ksek �ncelik)
            var eliminatedPlayers = new HashSet<string>();
            foreach (var (executor, target) in executeActions)
            {
                if (!eliminatedPlayers.Contains(executor.PlayerId) && !eliminatedPlayers.Contains(target.PlayerId))
                {
                    // Executor mermilerini harca
                    executor.Ammo -= 5;

                    // Target'� elimine et (koruma Execute'a kar�� i�e yaramaz)
                    eliminatedPlayers.Add(target.PlayerId);
                    target.IsAlive = false;

                    // �statistikleri g�ncelle
                    executor.Kills++;
                    target.Deaths++;

                    Console.WriteLine($"{executor.PlayerName} executed {target.PlayerName}!");
                }
            }

            // 3. Protect aksiyonlar�n� uygula
            var protectedPlayers = new HashSet<string>();
            foreach (var player in protectActions)
            {
                if (!eliminatedPlayers.Contains(player.PlayerId))
                {
                    player.IsProtected = true;
                    player.ConsecutiveProtects++;
                    protectedPlayers.Add(player.PlayerId);
                    Console.WriteLine($"{player.PlayerName} is protected this round!");
                }
            }

            // 4. Shoot aksiyonlar�n� ��z�mle
            foreach (var (shooter, target) in shootActions)
            {
                if (eliminatedPlayers.Contains(shooter.PlayerId) || eliminatedPlayers.Contains(target.PlayerId))
                    continue;

                // Mermi harca
                shooter.Ammo--;

                // Hedef korunuyorsa ate� etkisiz
                if (target.IsProtected)
                {
                    Console.WriteLine($"{shooter.PlayerName} shot {target.PlayerName}, but {target.PlayerName} was protected!");
                    continue;
                }

                // Hedef mermi al�yorsa elimine olur
                if (ammoActions.Contains(target))
                {
                    eliminatedPlayers.Add(target.PlayerId);
                    target.IsAlive = false;
                    shooter.Kills++;
                    target.Deaths++;
                    Console.WriteLine($"{shooter.PlayerName} shot {target.PlayerName} while collecting ammo! {target.PlayerName} eliminated!");
                }
                else
                {
                    Console.WriteLine($"{shooter.PlayerName} shot {target.PlayerName}, but missed!");
                }
            }

            // 5. Ammo aksiyonlar�n� uygula (elimine olmayan oyuncular i�in)
            foreach (var player in ammoActions)
            {
                if (!eliminatedPlayers.Contains(player.PlayerId))
                {
                    player.Ammo++;
                    Console.WriteLine($"{player.PlayerName} collected ammo! Total ammo: {player.Ammo}");
                }
            }

            // 6. Raunt sonras� temizlik
            foreach (var player in players)
            {
                if (!eliminatedPlayers.Contains(player.PlayerId))
                {
                    // Protect durumunu s�f�rla
                    if (!protectedPlayers.Contains(player.PlayerId))
                    {
                        player.ConsecutiveProtects = 0;
                    }
                    player.IsProtected = false;
                }
            }

            // 7. Elimine edilen oyuncular� listeden kald�r
            players.RemoveAll(p => eliminatedPlayers.Contains(p.PlayerId));

            // 8. T�m oyuncular�n raunt say�s�n� art�r
            foreach (var player in players)
            {
                player.RoundsPlayed++;
            }
        }

        public int GetRoundTimeLimit(int playerCount)
        {
            return playerCount switch
            {
                >= 4 => 5,
                3 => 4,
                2 => 1,
                _ => 1
            };
        }

        public bool IsGameOver(List<PlayerModel> players)
        {
            return players.Count(p => p.IsAlive) <= 1;
        }

        public PlayerModel GetWinner(List<PlayerModel> players)
        {
            return players.FirstOrDefault(p => p.IsAlive);
        }
    }

    public enum ActionType
    {
        Shoot,
        Protect,
        Execute,
        Ammo
    }

    public class PlayerAction
    {
        public string PlayerId { get; set; }
        public ActionType ActionType { get; set; }
        public string TargetPlayerId { get; set; } // Shoot ve Execute i�in
    }
}