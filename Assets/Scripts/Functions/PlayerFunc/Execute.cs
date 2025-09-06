using System;
using System.Collections.Generic;
using System.Linq;

namespace JamDemo
{
    public class Execute
    {
        private GameFunc gameFunc;
        private PlayerFunc playerFunc;

        public Execute(GameFunc gameFunc, PlayerFunc playerFunc)
        {
            this.gameFunc = gameFunc;
            this.playerFunc = playerFunc;
        }

        // ---- Public API ----

        public bool CanExecute(PlayerModel player)
        {
            if (player == null || !player.IsAlive) return false;
            return player.Ammo >= 5;
        }

        /// <summary>Koruma EXECUTE'a karşı işlemez.</summary>
        public bool ExecutePlayer(PlayerModel executor, PlayerModel target, List<PlayerModel> allPlayers)
        {
            if (!ValidateExecute(executor, target, allPlayers))
                return false;

            ApplyExecution(executor, target);

            Console.WriteLine($"⚡ {executor.PlayerName} EXECUTED {target.PlayerName}!");
            Console.WriteLine($"📊 {executor.PlayerName} kills: {executor.Kills} | {target.PlayerName} deaths: {target.Deaths}");

            return true;
        }

        public PlayerModel SelectExecuteTarget(
            PlayerModel executor,
            List<PlayerModel> allPlayers,
            ExecuteStrategy strategy = ExecuteStrategy.HighestThreat)
        {
            var validTargets = GetValidTargets(executor, allPlayers);
            if (!validTargets.Any()) return null;

            return strategy switch
            {
                ExecuteStrategy.Random => SelectRandomTarget(validTargets),
                ExecuteStrategy.HighestThreat => SelectHighestThreatTarget(validTargets, allPlayers),
                ExecuteStrategy.HighestAmmo => SelectHighestAmmoTarget(validTargets),
                ExecuteStrategy.MostKills => SelectMostKillsTarget(validTargets),
                ExecuteStrategy.Protected => SelectProtectedTarget(validTargets),
                ExecuteStrategy.Strategic => SelectStrategicTarget(executor, validTargets, allPlayers),
                _ => SelectRandomTarget(validTargets)
            };
        }

        public bool ShouldExecute(
            PlayerModel player,
            List<PlayerModel> allPlayers,
            ExecuteStrategy strategy = ExecuteStrategy.Strategic)
        {
            if (!CanExecute(player)) return false;

            var validTargets = GetValidTargets(player, allPlayers);
            if (!validTargets.Any()) return false;

            return strategy switch
            {
                ExecuteStrategy.Always => true,
                ExecuteStrategy.Never => false,
                ExecuteStrategy.HighestThreat => HasHighThreatTarget(player, validTargets, allPlayers),
                ExecuteStrategy.Protected => HasProtectedTarget(validTargets),
                ExecuteStrategy.Strategic => IsExecuteStrategicallySound(player, validTargets, allPlayers),
                ExecuteStrategy.Endgame => IsEndgameExecute(allPlayers),
                _ => false
            };
        }

        public ExecuteAnalysis AnalyzeExecuteOption(PlayerModel player, List<PlayerModel> allPlayers)
        {
            var analysis = new ExecuteAnalysis
            {
                Player = player,
                CanExecute = CanExecute(player),
                CurrentAmmo = player?.Ammo ?? 0,
                RequiredAmmo = 5,
                AmmoAfterExecute = (player?.Ammo ?? 0) - 5,
                ValidTargets = GetValidTargets(player, allPlayers),
                AlivePlayers = allPlayers.Count(p => p.IsAlive)
            };

            if (!analysis.CanExecute)
            {
                analysis.Recommendation = $"Cannot execute - need {Math.Max(0, 5 - (player?.Ammo ?? 0))} more ammo";
            }
            else if (!analysis.ValidTargets.Any())
            {
                analysis.Recommendation = "No valid targets";
            }
            else
            {
                var bestTarget = SelectExecuteTarget(player, allPlayers, ExecuteStrategy.Strategic);
                var threatScore = CalculateThreatScore(bestTarget, allPlayers);

                analysis.BestTarget = bestTarget;
                analysis.BestTargetThreatScore = threatScore;

                if (threatScore >= 15) analysis.Recommendation = $"EXECUTE {bestTarget.PlayerName} - Critical threat";
                else if (threatScore >= 10) analysis.Recommendation = $"Consider executing {bestTarget.PlayerName} - High threat";
                else if (analysis.AlivePlayers <= 3) analysis.Recommendation = $"Endgame execute {bestTarget.PlayerName}";
                else analysis.Recommendation = "Save ammo - no immediate execute threat";
            }

            return analysis;
        }

        public List<ExecuteResult> GetAllPossibleExecutes(PlayerModel executor, List<PlayerModel> allPlayers)
        {
            var results = new List<ExecuteResult>();
            if (!CanExecute(executor)) return results;

            var validTargets = GetValidTargets(executor, allPlayers);
            foreach (var target in validTargets)
            {
                results.Add(new ExecuteResult
                {
                    Executor = executor,
                    Target = target,
                    CanExecute = true,
                    AmmoAfterExecute = executor.Ammo - 5,
                    ThreatScore = CalculateThreatScore(target, allPlayers),
                    Outcome = target.IsAlive ? "Kill (bypasses protection)" : "Target already dead"
                });
            }
            return results;
        }

        // ---- Internals ----

        private bool ValidateExecute(PlayerModel executor, PlayerModel target, List<PlayerModel> allPlayers)
        {
            if (executor == null) { Console.WriteLine("❌ Executor is null!"); return false; }
            if (!executor.IsAlive) { Console.WriteLine($"❌ {executor.PlayerName} is dead!"); return false; }
            if (executor.Ammo < 5) { Console.WriteLine($"❌ Need 5 ammo (has {executor.Ammo})!"); return false; }
            if (target == null) { Console.WriteLine("❌ Target is null!"); return false; }
            if (!target.IsAlive) { Console.WriteLine($"❌ {target.PlayerName} already dead!"); return false; }
            if (executor.PlayerId == target.PlayerId) { Console.WriteLine("❌ Cannot execute self!"); return false; }
            if (!allPlayers.Contains(executor) || !allPlayers.Contains(target))
            { Console.WriteLine("❌ Player not in game!"); return false; }
            return true;
        }

        private void ApplyExecution(PlayerModel executor, PlayerModel target)
        {
            // 5 mermi harca
            executor.Ammo = Math.Max(0, executor.Ammo - 5);

            // Koruma delinir, doğrudan ölüm
            target.IsAlive = false;
            target.Deaths++;
            executor.Kills++;
        }

        private List<PlayerModel> GetValidTargets(PlayerModel executor, List<PlayerModel> allPlayers)
        {
            if (executor == null || !executor.IsAlive) return new List<PlayerModel>();

            return allPlayers
                .Where(p => p.IsAlive && p.PlayerId != executor.PlayerId)
                .ToList();
        }

        private PlayerModel SelectRandomTarget(List<PlayerModel> targets)
        {
            var r = new Random();
            return targets[r.Next(targets.Count)];
        }

        private PlayerModel SelectHighestThreatTarget(List<PlayerModel> targets, List<PlayerModel> allPlayers) =>
            targets.OrderByDescending(p => CalculateThreatScore(p, allPlayers)).First();

        private PlayerModel SelectHighestAmmoTarget(List<PlayerModel> targets) =>
            targets.OrderByDescending(p => p.Ammo).First();

        private PlayerModel SelectMostKillsTarget(List<PlayerModel> targets) =>
            targets.OrderByDescending(p => p.Kills).First();

        private PlayerModel SelectProtectedTarget(List<PlayerModel> targets)
        {
            var prot = targets.Where(p => p.IsProtected).ToList();
            return prot.Any() ? SelectRandomTarget(prot) : SelectRandomTarget(targets);
        }

        private PlayerModel SelectStrategicTarget(PlayerModel executor, List<PlayerModel> targets, List<PlayerModel> allPlayers)
        {
            var scored = targets.Select(t => new
            {
                Target = t,
                Score = CalculateThreatScore(t, allPlayers) + CalculateStrategicValue(executor, t, allPlayers)
            })
            .OrderByDescending(x => x.Score)
            .First();

            return scored.Target;
        }

        private int CalculateThreatScore(PlayerModel target, List<PlayerModel> allPlayers)
        {
            int score = 0;
            score += target.Ammo * 2;          // mermi
            if (target.Ammo >= 5) score += 15; // execute kabiliyeti
            score += target.Kills * 3;         // deneyim
            if (target.IsProtected) score += 5;
            return score;
        }

        private int CalculateStrategicValue(PlayerModel executor, PlayerModel target, List<PlayerModel> allPlayers)
        {
            int value = 0;
            var remainingAfterKill = allPlayers.Count(p => p.IsAlive) - 1;
            if (remainingAfterKill <= 3) value += 10; // endgame değeri
            var avgAmmo = allPlayers.Where(p => p.IsAlive).Average(p => p.Ammo);
            if (target.Ammo > avgAmmo) value += 5;
            return value;
        }

        private bool HasHighThreatTarget(PlayerModel executor, List<PlayerModel> targets, List<PlayerModel> allPlayers) =>
            targets.Any(t => CalculateThreatScore(t, allPlayers) >= 10);

        private bool HasProtectedTarget(List<PlayerModel> targets) =>
            targets.Any(t => t.IsProtected);

        private bool IsExecuteStrategicallySound(PlayerModel executor, List<PlayerModel> targets, List<PlayerModel> allPlayers)
        {
            int alive = allPlayers.Count(p => p.IsAlive);
            if (alive > 4) return targets.Any(t => CalculateThreatScore(t, allPlayers) >= 15);
            if (alive <= 3) return targets.Any(t => CalculateThreatScore(t, allPlayers) >= 5);
            return targets.Any(t => CalculateThreatScore(t, allPlayers) >= 10);
        }

        private bool IsEndgameExecute(List<PlayerModel> allPlayers) =>
            allPlayers.Count(p => p.IsAlive) <= 3;
    }

    // ---- DTO/Enum ----

    public enum ExecuteStrategy
    {
        Random,
        HighestThreat,
        HighestAmmo,
        MostKills,
        Protected,
        Strategic,
        Always,
        Never,
        Endgame
    }

    public class ExecuteAnalysis
    {
        public PlayerModel Player { get; set; }
        public bool CanExecute { get; set; }
        public int CurrentAmmo { get; set; }
        public int RequiredAmmo { get; set; }
        public int AmmoAfterExecute { get; set; }
        public List<PlayerModel> ValidTargets { get; set; } = new List<PlayerModel>();
        public int AlivePlayers { get; set; }
        public PlayerModel BestTarget { get; set; }
        public int BestTargetThreatScore { get; set; }
        public string Recommendation { get; set; }

        public override string ToString()
        {
            var target = BestTarget != null ? $"{BestTarget.PlayerName} (score {BestTargetThreatScore})" : "—";
            return $"CanExecute={CanExecute}, Ammo={CurrentAmmo}→{AmmoAfterExecute}, Best={target}, Rec={Recommendation}";
        }
    }

    public class ExecuteResult
    {
        public PlayerModel Executor { get; set; }
        public PlayerModel Target { get; set; }
        public bool CanExecute { get; set; }
        public int AmmoAfterExecute { get; set; }
        public int ThreatScore { get; set; }
        public string Outcome { get; set; }

        public override string ToString()
        {
            return $"{Executor?.PlayerName} → {Target?.PlayerName}: {Outcome} (Ammo after: {AmmoAfterExecute}, Threat: {ThreatScore})";
        }
    }
}
