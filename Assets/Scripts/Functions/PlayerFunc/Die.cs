using System;
using System.Collections.Generic;
using System.Linq;

namespace JamDemo
{
    public class Die
    {
        private GameFunc gameFunc;
        private PlayerFunc playerFunc;

        public Die(GameFunc gameFunc, PlayerFunc playerFunc)
        {
            this.gameFunc = gameFunc;
            this.playerFunc = playerFunc;
        }

        public void ProcessPlayerDeath(PlayerModel player, PlayerModel killer, DeathCause cause, List<PlayerModel> allPlayers)
        {
            if (player == null || !player.IsAlive)
                return;

            // Ölüm işlemini uygula
            ApplyDeath(player, killer, cause);

            // Ölüm mesajını göster
            DisplayDeathMessage(player, killer, cause);

            // İstatistikleri güncelle
            UpdateDeathStatistics(player, killer, cause);

            // Ölüm sonrası temizlik
            HandlePostDeathCleanup(player, allPlayers);

            // Oyun durumunu kontrol et
            CheckGameStateAfterDeath(allPlayers);
        }

        public void KillPlayer(PlayerModel player, PlayerModel killer, DeathCause cause)
        {
            if (player == null || killer == null)
                return;

            player.IsAlive = false;
            player.Deaths++;
            killer.Kills++;

            Console.WriteLine($"💀 {player.PlayerName} was eliminated by {killer.PlayerName}!");
        }

        public void EliminatePlayer(PlayerModel player, DeathCause cause = DeathCause.Unknown)
        {
            if (player == null || !player.IsAlive)
                return;

            player.IsAlive = false;
            player.Deaths++;

            Console.WriteLine($"💀 {player.PlayerName} was eliminated! ({cause})");
        }

        private void ApplyDeath(PlayerModel player, PlayerModel killer, DeathCause cause)
        {
            player.IsAlive = false;
            player.Deaths++;
            player.IsProtected = false; // Ölünce koruma kaybolur

            if (killer != null && killer != player)
            {
                killer.Kills++;
            }
        }

        private void DisplayDeathMessage(PlayerModel player, PlayerModel killer, DeathCause cause)
        {
            string message = cause switch
            {
                DeathCause.Shot => killer != null ?
                    $"💥 {player.PlayerName} was shot by {killer.PlayerName}!" :
                    $"💥 {player.PlayerName} was shot!",

                DeathCause.Execute => killer != null ?
                    $"⚡ {player.PlayerName} was EXECUTED by {killer.PlayerName}!" :
                    $"⚡ {player.PlayerName} was EXECUTED!",

                DeathCause.ShotWhileCollectingAmmo => killer != null ?
                    $"💀 {player.PlayerName} was shot by {killer.PlayerName} while collecting ammo!" :
                    $"💀 {player.PlayerName} was shot while collecting ammo!",

                DeathCause.Timeout =>
                    $"⏰ {player.PlayerName} was eliminated due to inactivity!",

                DeathCause.Disconnect =>
                    $"🔌 {player.PlayerName} disconnected and was eliminated!",

                DeathCause.SystemElimination =>
                    $"🖥️ {player.PlayerName} was eliminated by the system!",

                _ =>
                    $"💀 {player.PlayerName} was eliminated!"
            };

            Console.WriteLine(message);
        }

        private void UpdateDeathStatistics(PlayerModel player, PlayerModel killer, DeathCause cause)
        {
            // Ölüm nedeni istatistikleri
            LogDeathStatistic(player, cause);

            if (killer != null && killer != player)
            {
                LogKillStatistic(killer, cause);
                Console.WriteLine($"📊 {killer.PlayerName} kills: {killer.Kills} | {player.PlayerName} deaths: {player.Deaths}");
            }
        }

        private void HandlePostDeathCleanup(PlayerModel player, List<PlayerModel> allPlayers)
        {
            // Ölü oyuncunun ammo'sunu sıfırla (isteğe bağlı)
            // player.Ammo = 0;

            // Koruma durumunu sıfırla
            player.IsProtected = false;
            player.ConsecutiveProtects = 0;

            // Ölü oyuncuya yönelik olan aksiyonları iptal et
            CancelActionsTargetingDeadPlayer(player, allPlayers);
        }

        private void CancelActionsTargetingDeadPlayer(PlayerModel deadPlayer, List<PlayerModel> allPlayers)
        {
            // Bu metod round resolver tarafından handle edilecek
            // Burada sadece log mesajı
            Console.WriteLine($"🚫 All actions targeting {deadPlayer.PlayerName} have been cancelled");
        }

        private void CheckGameStateAfterDeath(List<PlayerModel> allPlayers)
        {
            var alivePlayers = allPlayers.Where(p => p.IsAlive).ToList();

            Console.WriteLine($"💭 Game state: {alivePlayers.Count} players remaining");

            if (alivePlayers.Count == 1)
            {
                var winner = alivePlayers.First();
                Console.WriteLine($"🏆 {winner.PlayerName} is the last player standing!");
            }
            else if (alivePlayers.Count == 0)
            {
                Console.WriteLine("😵 No players remaining!");
            }

            // Oyuncu sayısı azaldıkça zaman sınırını güncelle
            if (gameFunc != null)
            {
                var newTimeLimit = gameFunc.CalculateTimeLimit(alivePlayers.Count);
                Console.WriteLine($"⏱️ Time limit updated: {newTimeLimit} seconds");
            }
        }

        private void LogDeathStatistic(PlayerModel player, DeathCause cause)
        {
            Console.WriteLine($"📈 Death logged: {player.PlayerName} - {cause}");
            // Burada gerçek istatistik sistemi ile entegre edilebilir
        }

        private void LogKillStatistic(PlayerModel killer, DeathCause cause)
        {
            Console.WriteLine($"📈 Kill logged: {killer.PlayerName} - {cause}");
            // Burada gerçek istatistik sistemi ile entegre edilebilir
        }

        public DeathAnalysis AnalyzePlayerDeath(PlayerModel player, List<PlayerModel> allPlayers)
        {
            var analysis = new DeathAnalysis
            {
                Player = player,
                IsAlive = player.IsAlive,
                CurrentDeaths = player.Deaths,
                CurrentKills = player.Kills,
                RoundsPlayed = player.RoundsPlayed,
                PotentialKillers = GetPotentialKillers(player, allPlayers),
                DeathProbability = CalculateDeathProbability(player, allPlayers),
                SurvivalRecommendations = GetSurvivalRecommendations(player, allPlayers)
            };

            return analysis;
        }

        private List<PlayerModel> GetPotentialKillers(PlayerModel player, List<PlayerModel> allPlayers)
        {
            return allPlayers
                .Where(p => p.IsAlive && p.PlayerId != player.PlayerId)
                .Where(p => p.Ammo > 0) // Shoot edebilen
                .OrderByDescending(p => p.Ammo >= 5 ? 100 : p.Ammo) // Execute > Shoot
                .ToList();
        }

        private double CalculateDeathProbability(PlayerModel player, List<PlayerModel> allPlayers)
        {
            if (!player.IsAlive)
                return 1.0; // Zaten ölü

            var threats = GetPotentialKillers(player, allPlayers);
            if (!threats.Any())
                return 0.0; // Tehdit yok

            double probability = 0.0;

            foreach (var threat in threats)
            {
                if (threat.Ammo >= 5)
                {
                    // Execute tehdidi - koruma işe yaramaz
                    probability += 0.8; // %80 execute riski
                }
                else if (threat.Ammo > 0)
                {
                    // Normal shoot tehdidi
                    if (player.IsProtected)
                        probability += 0.1; // Koruma varsa %10
                    else
                        probability += 0.3; // Koruma yoksa %30
                }
            }

            return Math.Min(1.0, probability); // Max %100
        }

        private List<string> GetSurvivalRecommendations(PlayerModel player, List<PlayerModel> allPlayers)
        {
            var recommendations = new List<string>();

            if (!player.IsAlive)
            {
                recommendations.Add("Player is already dead");
                return recommendations;
            }

            var threats = GetPotentialKillers(player, allPlayers);

            // Execute tehdidi varsa
            if (threats.Any(t => t.Ammo >= 5))
            {
                recommendations.Add("⚡ CRITICAL: Execute threat detected - Consider aggressive action");
                recommendations.Add("🎯 Target high-ammo players immediately");
            }

            // Çok sayıda tehdit varsa
            if (threats.Count >= 3)
            {
                recommendations.Add("🛡️ High threat count - Use protection if available");
            }

            // Koruma önerileri
            if (player.ConsecutiveProtects < 3 && threats.Any())
            {
                recommendations.Add("🛡️ Consider using protection");
            }

            // Ammo önerileri
            if (player.Ammo == 0)
            {
                recommendations.Add("💰 Critical: Collect ammo but beware of shooters");
            }
            else if (player.Ammo < 3)
            {
                recommendations.Add("💰 Low ammo - Consider collecting more");
            }

            // Execute önerileri
            if (player.Ammo >= 5)
            {
                var bestTarget = threats.OrderByDescending(t => t.Ammo).FirstOrDefault();
                if (bestTarget != null)
                {
                    recommendations.Add($"⚡ Execute option available - Consider targeting {bestTarget.PlayerName}");
                }
            }

            if (!recommendations.Any())
            {
                recommendations.Add("✅ Relatively safe position");
            }

            return recommendations;
        }

        public bool IsPlayerInDanger(PlayerModel player, List<PlayerModel> allPlayers, double threshold = 0.5)
        {
            var deathProbability = CalculateDeathProbability(player, allPlayers);
            return deathProbability >= threshold;
        }

        public List<PlayerModel> GetPlayersInDanger(List<PlayerModel> allPlayers, double threshold = 0.5)
        {
            return allPlayers
                .Where(p => p.IsAlive)
                .Where(p => IsPlayerInDanger(p, allPlayers, threshold))
                .ToList();
        }
    }

    public enum DeathCause
    {
        Shot,                      // Normal ateş
        Execute,                   // İnfaz
        ShotWhileCollectingAmmo,   // Mermi toplarken vurulma
        Timeout,                   // Zaman aşımı
        Disconnect,                // Bağlantı kopması
        SystemElimination,         // Sistem tarafından eleme
        Unknown                    // Bilinmeyen sebep
    }

    public class DeathAnalysis
    {
        public PlayerModel Player { get; set; }
        public bool IsAlive { get; set; }
        public int CurrentDeaths { get; set; }
        public int CurrentKills { get; set; }
        public int RoundsPlayed { get; set; }
        public List<PlayerModel> PotentialKillers { get; set; }
        public double DeathProbability { get; set; }
        public List<string> SurvivalRecommendations { get; set; }

        public DeathAnalysis()
        {
            PotentialKillers = new List<PlayerModel>();
            SurvivalRecommendations = new List<string>();
        }

        public string GetRiskLevel()
        {
            return DeathProbability switch
            {
                >= 0.8 => "CRITICAL",
                >= 0.6 => "HIGH",
                >= 0.4 => "MEDIUM",
                >= 0.2 => "LOW",
                _ => "SAFE"
            };
        }

        public override string ToString()
        {
            return $"{Player?.PlayerName}: {GetRiskLevel()} risk ({DeathProbability:P0}) - {PotentialKillers.Count} threats";
        }
    }
}