using System;
using System.Collections.Generic;
using System.Linq;

namespace JamDemo
{
    public class PlayerFunc
    {
        public PlayerFunc()
        {
        }

        public bool ValidatePlayer(PlayerModel player)
        {
            return player != null &&
                   !string.IsNullOrEmpty(player.PlayerId) &&
                   !string.IsNullOrEmpty(player.PlayerName);
        }

        public void ResetPlayerForNewGame(PlayerModel player)
        {
            if (player == null) return;

            player.IsAlive = true;
            player.Ammo = 0;
            player.IsProtected = false;
            player.ConsecutiveProtects = 0;
            player.Kills = 0;
            player.Deaths = 0;
            player.RoundsPlayed = 0;
        }

        public void ApplyDamage(PlayerModel target, PlayerModel attacker)
        {
            if (target == null || attacker == null) return;

            target.IsAlive = false;
            target.Deaths++;
            attacker.Kills++;

            Console.WriteLine($"{attacker.PlayerName} eliminated {target.PlayerName}");
        }

        public bool CanPerformAction(PlayerModel player, ActionType action)
        {
            if (player == null || !player.IsAlive)
                return false;

            return action switch
            {
                ActionType.Shoot => player.Ammo > 0,
                ActionType.Execute => player.Ammo >= 5,
                ActionType.Protect => player.ConsecutiveProtects < 3,
                ActionType.Ammo => true,
                _ => false
            };
        }

        public void GiveAmmo(PlayerModel player, int amount = 1)
        {
            if (player == null || !player.IsAlive) return;

            player.Ammo += amount;
            Console.WriteLine($"{player.PlayerName} gained {amount} ammo. Total: {player.Ammo}");
        }

        public void ConsumeAmmo(PlayerModel player, int amount = 1)
        {
            if (player == null || !player.IsAlive) return;

            player.Ammo = Math.Max(0, player.Ammo - amount);
        }

        public void ApplyProtection(PlayerModel player)
        {
            if (player == null || !player.IsAlive) return;

            player.IsProtected = true;
            player.ConsecutiveProtects++;
            Console.WriteLine($"{player.PlayerName} is now protected ({player.ConsecutiveProtects}/3)");
        }

        public void RemoveProtection(PlayerModel player)
        {
            if (player == null) return;

            player.IsProtected = false;
        }

        public void ResetConsecutiveProtects(PlayerModel player)
        {
            if (player == null) return;

            player.ConsecutiveProtects = 0;
        }

        public void IncrementRoundsPlayed(PlayerModel player)
        {
            if (player == null) return;

            player.RoundsPlayed++;
        }

        public PlayerModel FindPlayerById(List<PlayerModel> players, string playerId)
        {
            return players?.FirstOrDefault(p => p.PlayerId == playerId);
        }

        public PlayerModel FindPlayerByName(List<PlayerModel> players, string playerName)
        {
            return players?.FirstOrDefault(p => p.PlayerName.Equals(playerName, StringComparison.OrdinalIgnoreCase));
        }

        public List<PlayerModel> GetAlivePlayers(List<PlayerModel> players)
        {
            return players?.Where(p => p.IsAlive).ToList() ?? new List<PlayerModel>();
        }

        public List<PlayerModel> GetDeadPlayers(List<PlayerModel> players)
        {
            return players?.Where(p => !p.IsAlive).ToList() ?? new List<PlayerModel>();
        }

        public bool IsPlayerNameTaken(List<PlayerModel> players, string playerName, string excludePlayerId = null)
        {
            return players?.Any(p => p.PlayerName.Equals(playerName, StringComparison.OrdinalIgnoreCase)
                                   && p.PlayerId != excludePlayerId) ?? false;
        }

        public bool IsPlayerIdTaken(List<PlayerModel> players, string playerId)
        {
            return players?.Any(p => p.PlayerId == playerId) ?? false;
        }

        public void UpdatePlayerStats(PlayerModel player, bool won)
        {
            if (player == null) return;

            // StatModel güncellemesi burada yapýlabilir
            Console.WriteLine($"Updated stats for {player.PlayerName}: Kills={player.Kills}, Deaths={player.Deaths}, Won={won}");
        }

        public double GetKillDeathRatio(PlayerModel player)
        {
            if (player == null || player.Deaths == 0)
                return player?.Kills ?? 0;

            return (double)player.Kills / player.Deaths;
        }

        public string GetPlayerStatus(PlayerModel player)
        {
            if (player == null) return "Unknown";

            var status = player.IsAlive ? "Alive" : "Dead";
            var protection = player.IsProtected ? " (Protected)" : "";
            return $"{status}{protection} - Ammo: {player.Ammo}";
        }
    }
}