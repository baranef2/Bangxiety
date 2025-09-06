using System;
using System.Collections.Generic;
using System.Linq;

namespace JamDemo
{
    public class EndGame
    {
        private GameFunc gameFunc;
        private PlayerFunc playerFunc;

        public EndGame(GameFunc gameFunc, PlayerFunc playerFunc)
        {
            this.gameFunc = gameFunc;
            this.playerFunc = playerFunc;
        }

        public GameResult FinalizeGame(GameSession gameSession, string endReason = "Game completed")
        {
            if (gameSession == null)
            {
                Console.WriteLine("No game session to finalize!");
                return null;
            }

            gameSession.IsActive = false;
            gameSession.EndTime = DateTime.Now;
            gameSession.EndReason = endReason;

            // Kazananı belirle
            var winner = DetermineWinner(gameSession.Players);
            gameSession.Winner = winner;

            // Oyun sonucu objesi oluştur
            var gameResult = CreateGameResult(gameSession);

            // İstatistikleri güncelle
            UpdateAllPlayerStats(gameSession, gameResult);

            // Sonuçları göster
            DisplayGameResults(gameResult);

            Console.WriteLine($"\n🏁 Game Ended: {endReason}");
            return gameResult;
        }

        private PlayerModel DetermineWinner(List<PlayerModel> players)
        {
            var alivePlayers = players.Where(p => p.IsAlive).ToList();

            if (alivePlayers.Count == 1)
            {
                return alivePlayers.First();
            }
            else if (alivePlayers.Count > 1)
            {
                // Birden fazla oyuncu kaldıysa en yüksek kill sayısına sahip olan kazanır
                return alivePlayers
                    .OrderByDescending(p => p.Kills)
                    .ThenBy(p => p.Deaths)
                    .ThenByDescending(p => p.Ammo)
                    .First();
            }

            return null; // Kimse kalmadıysa
        }

        private GameResult CreateGameResult(GameSession gameSession)
        {
            var gameResult = new GameResult
            {
                GameId = gameSession.GameId,
                Winner = gameSession.Winner,
                TotalRounds = gameSession.CurrentRound,
                GameDuration = gameSession.GameDuration,
                StartTime = gameSession.StartTime,
                EndTime = gameSession.EndTime.Value,
                EndReason = gameSession.EndReason,
                PlayerResults = new List<PlayerResult>()
            };

            // Oyuncuları sırala (kazanan en üstte, sonra kill sayısına göre)
            var sortedPlayers = gameSession.Players
                .OrderByDescending(p => p == gameSession.Winner ? 1 : 0)
                .ThenByDescending(p => p.Kills)
                .ThenBy(p => p.Deaths)
                .ThenByDescending(p => p.RoundsPlayed)
                .ToList();

            for (int i = 0; i < sortedPlayers.Count; i++)
            {
                var player = sortedPlayers[i];
                var playerResult = new PlayerResult
                {
                    PlayerId = player.PlayerId,
                    PlayerName = player.PlayerName,
                    Rank = i + 1,
                    IsWinner = player == gameSession.Winner,
                    Kills = player.Kills,
                    Deaths = player.Deaths,
                    RoundsPlayed = player.RoundsPlayed,
                    FinalAmmo = player.Ammo,
                    IsAlive = player.IsAlive,
                    KillDeathRatio = CalculateKDR(player.Kills, player.Deaths)
                };

                gameResult.PlayerResults.Add(playerResult);
            }

            return gameResult;
        }

        private void UpdateAllPlayerStats(GameSession gameSession, GameResult gameResult)
        {
            foreach (var playerResult in gameResult.PlayerResults)
            {
                var player = gameSession.Players.First(p => p.PlayerId == playerResult.PlayerId);

                // StatModel güncelle (normalde veritabanından gelecek)
                var statModel = GetOrCreatePlayerStats(player.PlayerId);
                statModel.UpdateStats(player, playerResult.IsWinner, gameSession.GameId);

                Console.WriteLine($"📊 {player.PlayerName} stats updated:");
                Console.WriteLine($"   Total Games: {statModel.GamesPlayed}");
                Console.WriteLine($"   Win Rate: {statModel.WinRate:F1}%");
                Console.WriteLine($"   Overall K/D: {statModel.KillDeathRatio:F2}");
            }
        }

        private void DisplayGameResults(GameResult gameResult)
        {
            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine("🏆 FINAL SCOREBOARD 🏆");
            Console.WriteLine(new string('=', 50));

            Console.WriteLine($"Game Duration: {gameResult.GameDuration.Minutes}m {gameResult.GameDuration.Seconds}s");
            Console.WriteLine($"Total Rounds: {gameResult.TotalRounds}");

            if (gameResult.Winner != null)
            {
                Console.WriteLine($"👑 WINNER: {gameResult.Winner.PlayerName}");
            }
            else
            {
                Console.WriteLine("😵 NO WINNER - All players eliminated!");
            }

            Console.WriteLine("\n--- Player Rankings ---");
            foreach (var playerResult in gameResult.PlayerResults)
            {
                var status = playerResult.IsWinner ? "👑 WINNER" :
                           playerResult.IsAlive ? "✅ Survivor" : "❌ Eliminated";

                Console.WriteLine($"{playerResult.Rank}. {playerResult.PlayerName} - {status}");
                Console.WriteLine($"   Kills: {playerResult.Kills} | Deaths: {playerResult.Deaths} | K/D: {playerResult.KillDeathRatio:F2}");
                Console.WriteLine($"   Rounds Played: {playerResult.RoundsPlayed} | Final Ammo: {playerResult.FinalAmmo}");
                Console.WriteLine();
            }

            // En iyi performansları göster
            ShowGameHighlights(gameResult);
        }

        private void ShowGameHighlights(GameResult gameResult)
        {
            Console.WriteLine("--- Game Highlights ---");

            var mostKills = gameResult.PlayerResults.OrderByDescending(p => p.Kills).First();
            if (mostKills.Kills > 0)
            {
                Console.WriteLine($"🔥 Most Kills: {mostKills.PlayerName} ({mostKills.Kills} kills)");
            }

            var bestKDR = gameResult.PlayerResults
                .Where(p => p.Deaths > 0)
                .OrderByDescending(p => p.KillDeathRatio)
                .FirstOrDefault();

            if (bestKDR != null)
            {
                Console.WriteLine($"⚔️ Best K/D Ratio: {bestKDR.PlayerName} ({bestKDR.KillDeathRatio:F2})");
            }

            var mostSurvival = gameResult.PlayerResults.OrderByDescending(p => p.RoundsPlayed).First();
            Console.WriteLine($"🛡️ Longest Survivor: {mostSurvival.PlayerName} ({mostSurvival.RoundsPlayed} rounds)");

            var mostAmmo = gameResult.PlayerResults.OrderByDescending(p => p.FinalAmmo).First();
            if (mostAmmo.FinalAmmo > 0)
            {
                Console.WriteLine($"💰 Most Final Ammo: {mostAmmo.PlayerName} ({mostAmmo.FinalAmmo} ammo)");
            }
        }

        private double CalculateKDR(int kills, int deaths)
        {
            return deaths == 0 ? kills : (double)kills / deaths;
        }

        private StatModel GetOrCreatePlayerStats(string playerId)
        {
            // Bu normalde veritabanından gelecek
            // Şimdilik yeni bir StatModel döndürüyoruz
            return new StatModel();
        }

        public void SaveGameResult(GameResult gameResult)
        {
            // Oyun sonucunu veritabanına kaydet
            Console.WriteLine($"💾 Game result saved: {gameResult.GameId}");

            // JSON olarak kaydetmek isterseniz:
            // string json = JsonSerializer.Serialize(gameResult);
            // File.WriteAllText($"game_{gameResult.GameId}.json", json);
        }

        public bool IsValidGameEnd(GameSession gameSession)
        {
            if (gameSession == null || !gameSession.IsActive)
                return false;

            var alivePlayers = gameSession.Players.Count(p => p.IsAlive);

            // 1 veya 0 oyuncu kaldıysa oyun bitebilir
            if (alivePlayers <= 1)
                return true;

            // Maksimum raunt sayısına ulaşıldıysa
            if (gameSession.CurrentRound >= gameSession.MaxRounds)
                return true;

            return false;
        }
    }

    public class GameResult
    {
        public string GameId { get; set; }
        public PlayerModel Winner { get; set; }
        public int TotalRounds { get; set; }
        public TimeSpan GameDuration { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string EndReason { get; set; }
        public List<PlayerResult> PlayerResults { get; set; }

        public GameResult()
        {
            PlayerResults = new List<PlayerResult>();
        }
    }

    public class PlayerResult
    {
        public string PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int Rank { get; set; }
        public bool IsWinner { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int RoundsPlayed { get; set; }
        public int FinalAmmo { get; set; }
        public bool IsAlive { get; set; }
        public double KillDeathRatio { get; set; }
    }
}