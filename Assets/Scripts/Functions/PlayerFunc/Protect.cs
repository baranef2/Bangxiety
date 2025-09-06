using System;
using System.Collections.Generic;
using System.Linq;

namespace JamDemo
{
    public class Protect
    {
        private GameFunc gameFunc;
        private PlayerFunc playerFunc;

        public Protect(GameFunc gameFunc, PlayerFunc playerFunc)
        {
            this.gameFunc = gameFunc;
            this.playerFunc = playerFunc;
        }

        public bool CanProtect(PlayerModel player)
        {
            if (player == null || !player.IsAlive)
                return false;

            // Art arda 3'ten fazla protect alamaz
            return player.ConsecutiveProtects < 3;
        }

        public bool ExecuteProtect(PlayerModel player)
        {
            if (!ValidateProtect(player))
                return false;

            // Protection uygula
            ApplyProtection(player);

            Console.WriteLine($"🛡️ {player.PlayerName} activates protection! ({player.ConsecutiveProtects}/3)");

            // Uyarı mesajları
            if (player.ConsecutiveProtects == 2)
            {
                Console.WriteLine($"⚠️ {player.PlayerName} can only protect 1 more time!");
            }
            else if (player.ConsecutiveProtects == 3)
            {
                Console.WriteLine($"⚠️ {player.PlayerName} has reached maximum consecutive protections!");
            }

            return true;
        }

        public bool ShouldProtect(PlayerModel player, List<PlayerModel> allPlayers, ProtectStrategy strategy = ProtectStrategy.Threat)
        {
            if (!CanProtect(player))
                return false;

            return strategy switch
            {
                ProtectStrategy.Always => true,
                ProtectStrategy.Never => false,
                ProtectStrategy.Threat => AssessThreatLevel(player, allPlayers) >= ThreatLevel.High,
                ProtectStrategy.LowAmmo => player.Ammo <= 1,
                ProtectStrategy.HighValue => IsHighValueTarget(player, allPlayers),
                ProtectStrategy.Conservative => player.ConsecutiveProtects == 0 && AssessThreatLevel(player, allPlayers) >= ThreatLevel.Medium,
                ProtectStrategy.Aggressive => player.ConsecutiveProtects < 2 && HasImmediateThreat(player, allPlayers),
                _ => false
            };
        }

        private bool ValidateProtect(PlayerModel player)
        {
            if (player == null)
            {
                Console.WriteLine("❌ Player is null!");
                return false;
            }

            if (!player.IsAlive)
            {
                Console.WriteLine($"❌ {player.PlayerName} is dead and cannot protect!");
                return false;
            }

            if (player.ConsecutiveProtects >= 3)
            {
                Console.WriteLine($"❌ {player.PlayerName} has reached maximum consecutive protections!");
                return false;
            }

            return true;
        }

        private void ApplyProtection(PlayerModel player)
        {
            player.IsProtected = true;
            player.ConsecutiveProtects++;
        }

        public void RemoveProtection(PlayerModel player)
        {
            if (player != null)
            {
                player.IsProtected = false;
                // ConsecutiveProtects burada sıfırlanmaz, sadece protect almadığı durumda sıfırlanır
            }
        }

        public void ResetConsecutiveProtects(PlayerModel player)
        {
            if (player != null)
            {
                player.ConsecutiveProtects = 0;
                Console.WriteLine($"🔄 {player.PlayerName} consecutive protections reset");
            }
        }

        private ThreatLevel AssessThreatLevel(PlayerModel player, List<PlayerModel> allPlayers)
        {
            var enemies = allPlayers.Where(p => p.IsAlive && p.PlayerId != player.PlayerId).ToList();

            int totalThreat = 0;
            int immediateThreats = 0;

            foreach (var enemy in enemies)
            {
                // Mermi tehdidi
                if (enemy.Ammo > 0)
                {
                    totalThreat += enemy.Ammo;
                    immediateThreats++;
                }

                // Execute tehdidi (çok yüksek tehdit)
                if (enemy.Ammo >= 5)
                {
                    totalThreat += 10; // Execute çok tehlikeli
                }
            }

            // Tehdit seviyesini belirle
            if (immediateThreats == 0)
                return ThreatLevel.None;
            else if (immediateThreats == 1 && totalThreat <= 2)
                return ThreatLevel.Low;
            else if (immediateThreats <= 2 && totalThreat <= 5)
                return ThreatLevel.Medium;
            else if (totalThreat <= 10)
                return ThreatLevel.High;
            else
                return ThreatLevel.Critical;
        }

        private bool IsHighValueTarget(PlayerModel player, List<PlayerModel> allPlayers)
        {
            var alivePlayers = allPlayers.Where(p => p.IsAlive).ToList();

            // Yüksek ammo
            var avgAmmo = alivePlayers.Average(p => p.Ammo);
            if (player.Ammo > avgAmmo * 1.5)
                return true;

            // Yüksek kill sayısı
            var avgKills = alivePlayers.Average(p => p.Kills);
            if (player.Kills > avgKills * 1.5)
                return true;

            // Execute yapabilme kabiliyeti
            if (player.Ammo >= 5)
                return true;

            return false;
        }

        private bool HasImmediateThreat(PlayerModel player, List<PlayerModel> allPlayers)
        {
            return allPlayers
                .Where(p => p.IsAlive && p.PlayerId != player.PlayerId)
                .Any(enemy => enemy.Ammo >= 5); // Execute tehdidi
        }

        public ProtectAnalysis AnalyzeProtectOption(PlayerModel player, List<PlayerModel> allPlayers)
        {
            var analysis = new ProtectAnalysis
            {
                Player = player,
                CanProtect = CanProtect(player),
                CurrentConsecutiveProtects = player.ConsecutiveProtects,
                MaxConsecutiveProtects = 3,
                RemainingProtects = Math.Max(0, 3 - player.ConsecutiveProtects),
                ThreatLevel = AssessThreatLevel(player, allPlayers),
                IsHighValueTarget = IsHighValueTarget(player, allPlayers),
                HasImmediateThreat = HasImmediateThreat(player, allPlayers)
            };

            // Recommendation
            if (!analysis.CanProtect)
            {
                analysis.Recommendation = "Cannot protect - max consecutive reached";
            }
            else if (analysis.ThreatLevel >= ThreatLevel.Critical)
            {
                analysis.Recommendation = "STRONGLY RECOMMENDED - Critical threat";
            }
            else if (analysis.ThreatLevel >= ThreatLevel.High)
            {
                analysis.Recommendation = "Recommended - High threat";
            }
            else if (analysis.IsHighValueTarget && analysis.ThreatLevel >= ThreatLevel.Medium)
            {
                analysis.Recommendation = "Consider - High value target under threat";
            }
            else if (analysis.RemainingProtects == 1)
            {
                analysis.Recommendation = "Save for critical moment - Last protection";
            }
            else
            {
                analysis.Recommendation = "Not recommended - Low threat";
            }

            return analysis;
        }

        public List<PlayerModel> GetPlayersWhoShouldProtect(List<PlayerModel> allPlayers, ProtectStrategy strategy = ProtectStrategy.Threat)
        {
            var candidates = new List<PlayerModel>();

            foreach (var player in allPlayers.Where(p => p.IsAlive))
            {
                if (ShouldProtect(player, allPlayers, strategy))
                {
                    candidates.Add(player);
                }
            }

            return candidates;
        }

        public bool IsProtectionEffective(PlayerModel player, List<PlayerModel> allPlayers)
        {
            // Kimse shoot yapamıyorsa protection gereksiz
            var shooters = allPlayers.Where(p => p.IsAlive && p.PlayerId != player.PlayerId && p.Ammo > 0).ToList();

            return shooters.Any();
        }
    }

    public enum ProtectStrategy
    {
        Always,      // Her zaman protect
        Never,       // Hiç protect alma
        Threat,      // Tehdit seviyesine göre
        LowAmmo,     // Az mermide protect
        HighValue,   // Yüksek değerli oyuncuysa
        Conservative, // Muhafazakar (sadece yüksek tehditte)
        Aggressive   // Agresif (orta tehditte bile)
    }

    public enum ThreatLevel
    {
        None,
        Low,
        Medium,
        High,
        Critical
    }

    public class ProtectAnalysis
    {
        public PlayerModel Player { get; set; }
        public bool CanProtect { get; set; }
        public int CurrentConsecutiveProtects { get; set; }
        public int MaxConsecutiveProtects { get; set; }
        public int RemainingProtects { get; set; }
        public ThreatLevel ThreatLevel { get; set; }
        public bool IsHighValueTarget { get; set; }
        public bool HasImmediateThreat { get; set; }
        public string Recommendation { get; set; }

        public override string ToString()
        {
            return $"{Player?.PlayerName}: {Recommendation} (Threat: {ThreatLevel}, Remaining: {RemainingProtects})";
        }
    }
}