using System;
using System.Collections.Generic;
using System.Linq;

namespace JamDemo
{
    public class StatModel
    {
        public int TotalKills { get; set; }
        public int TotalDeaths { get; set; }
        public int TotalAmmoCollected { get; set; }
        public int TotalRoundsPlayed { get; set; }
        public int GamesPlayed { get; set; }
        public int GamesWon { get; set; }
        public DateTime LastGamePlayed { get; set; }

        // Oyun baþýna istatistikler
        public Dictionary<string, GameStats> GameHistory { get; set; }

        public StatModel()
        {
            TotalKills = 0;
            TotalDeaths = 0;
            TotalAmmoCollected = 0;
            TotalRoundsPlayed = 0;
            GamesPlayed = 0;
            GamesWon = 0;
            LastGamePlayed = DateTime.Now;
            GameHistory = new Dictionary<string, GameStats>();
        }

        public double KillDeathRatio => TotalDeaths == 0 ? TotalKills : (double)TotalKills / TotalDeaths;
        public double WinRate => GamesPlayed == 0 ? 0 : (double)GamesWon / GamesPlayed * 100;
        public double AverageRoundsPerGame => GamesPlayed == 0 ? 0 : (double)TotalRoundsPlayed / GamesPlayed;

        public void UpdateStats(PlayerModel player, bool isWinner, string gameId)
        {
            TotalKills += player.Kills;
            TotalDeaths += player.Deaths;
            TotalRoundsPlayed += player.RoundsPlayed;
            TotalAmmoCollected += player.Ammo; // Son durumda kalan mermi
            GamesPlayed++;

            if (isWinner)
            {
                GamesWon++;
            }

            LastGamePlayed = DateTime.Now;

            // Bu oyunun istatistiklerini kaydet
            GameHistory[gameId] = new GameStats
            {
                GameId = gameId,
                Kills = player.Kills,
                Deaths = player.Deaths,
                RoundsPlayed = player.RoundsPlayed,
                FinalAmmo = player.Ammo,
                IsWinner = isWinner,
                GameDate = DateTime.Now
            };
        }

        public GameStats GetBestGame()
        {
            return GameHistory.Values
                .Where(g => g.IsWinner)
                .OrderByDescending(g => g.Kills)
                .ThenBy(g => g.RoundsPlayed)
                .FirstOrDefault();
        }

        public List<GameStats> GetRecentGames(int count = 10)
        {
            return GameHistory.Values
                .OrderByDescending(g => g.GameDate)
                .Take(count)
                .ToList();
        }
    }

    public class GameStats
    {
        public string GameId { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int RoundsPlayed { get; set; }
        public int FinalAmmo { get; set; }
        public bool IsWinner { get; set; }
        public DateTime GameDate { get; set; }

        public double KillDeathRatio => Deaths == 0 ? Kills : (double)Kills / Deaths;
    }
}