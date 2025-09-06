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
            // 1. Önce tüm aksiyonlarý kategorize et
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
                        // Art arda 3'ten fazla protect kontrolü
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

            // 2. Önce Execute aksiyonlarýný çözümle (en yüksek öncelik)
            var eliminatedPlayers = new HashSet<string>();
            foreach (var (executor, target) in executeActions)
            {
                if (!eliminatedPlayers.Contains(executor.PlayerId) && !eliminatedPlayers.Contains(target.PlayerId))
                {
                    // Executor mermilerini harca
                    executor.Ammo -= 5;

                    // Target'ý elimine et (koruma Execute'a karþý iþe yaramaz)
                    eliminatedPlayers.Add(target.PlayerId);
                    target.IsAlive = false;

                    // Ýstatistikleri güncelle
                    executor.Kills++;
                    target.Deaths++;

                    Console.WriteLine($"{executor.PlayerName} executed {target.PlayerName}!");
                }
            }

            // 3. Protect aksiyonlarýný uygula
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

            // 4. Shoot aksiyonlarýný çözümle
            foreach (var (shooter, target) in shootActions)
            {
                if (eliminatedPlayers.Contains(shooter.PlayerId) || eliminatedPlayers.Contains(target.PlayerId))
                    continue;

                // Mermi harca
                shooter.Ammo--;

                // Hedef korunuyorsa ateþ etkisiz
                if (target.IsProtected)
                {
                    Console.WriteLine($"{shooter.PlayerName} shot {target.PlayerName}, but {target.PlayerName} was protected!");
                    continue;
                }

                // Hedef mermi alýyorsa elimine olur
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

            // 5. Ammo aksiyonlarýný uygula (elimine olmayan oyuncular için)
            foreach (var player in ammoActions)
            {
                if (!eliminatedPlayers.Contains(player.PlayerId))
                {
                    player.Ammo++;
                    Console.WriteLine($"{player.PlayerName} collected ammo! Total ammo: {player.Ammo}");
                }
            }

            // 6. Raunt sonrasý temizlik
            foreach (var player in players)
            {
                if (!eliminatedPlayers.Contains(player.PlayerId))
                {
                    // Protect durumunu sýfýrla
                    if (!protectedPlayers.Contains(player.PlayerId))
                    {
                        player.ConsecutiveProtects = 0;
                    }
                    player.IsProtected = false;
                }
            }

            // 7. Elimine edilen oyuncularý listeden kaldýr
            players.RemoveAll(p => eliminatedPlayers.Contains(p.PlayerId));

            // 8. Tüm oyuncularýn raunt sayýsýný artýr
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
        public string TargetPlayerId { get; set; } // Shoot ve Execute için
    }
}