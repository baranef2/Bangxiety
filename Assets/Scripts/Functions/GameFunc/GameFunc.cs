using System;
using System.Collections.Generic;
using System.Linq;

namespace JamDemo
{
    public class GameFunc
    {
        public GameFunc()
        {
        }

        public bool ValidateGameState(List<PlayerModel> players)
        {
            if (players == null || players.Count == 0)
                return false;

            return players.Any(p => p.IsAlive);
        }

        public void LogGameEvent(string eventMessage)
        {
            Console.WriteLine($"[GAME] {DateTime.Now:HH:mm:ss} - {eventMessage}");
        }

        public bool CanStartGame(List<PlayerModel> players)
        {
            return players != null && players.Count >= 4 && players.Count <= 8;
        }

        public void UpdateGameStatistics(List<PlayerModel> players)
        {
            foreach (var player in players)
            {
                // Ýstatistik güncelleme mantýðý
                LogGameEvent($"Updated stats for {player.PlayerName}");
            }
        }

        public List<PlayerModel> GetEligiblePlayers(List<PlayerModel> players)
        {
            return players?.Where(p => p.IsAlive).ToList() ?? new List<PlayerModel>();
        }

        public bool IsValidAction(PlayerModel player, ActionType actionType)
        {
            if (player == null || !player.IsAlive)
                return false;

            return actionType switch
            {
                ActionType.Shoot => player.Ammo > 0,
                ActionType.Execute => player.Ammo >= 5,
                ActionType.Protect => player.ConsecutiveProtects < 3,
                ActionType.Ammo => true,
                _ => false
            };
        }

        public void ProcessGameEnd(List<PlayerModel> players, PlayerModel winner)
        {
            LogGameEvent($"Game ended. Winner: {winner?.PlayerName ?? "None"}");

            foreach (var player in players)
            {
                LogGameEvent($"Final stats - {player.PlayerName}: K:{player.Kills} D:{player.Deaths} R:{player.RoundsPlayed}");
            }
        }

        public int CalculateTimeLimit(int playerCount)
        {
            return playerCount switch
            {
                >= 4 => 5,
                3 => 4,
                2 => 1,
                _ => 1
            };
        }

        public bool CheckWinCondition(List<PlayerModel> players)
        {
            var alivePlayers = players.Count(p => p.IsAlive);
            return alivePlayers <= 1;
        }

        public void ResetPlayerStates(List<PlayerModel> players)
        {
            foreach (var player in players)
            {
                player.IsProtected = false;
                if (player.ConsecutiveProtects > 0 && !player.IsProtected)
                {
                    // Reset consecutive protects only if not currently protected
                }
            }
        }
    }
}