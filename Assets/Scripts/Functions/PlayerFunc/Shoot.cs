using System;
using System.Collections.Generic;
using System.Linq;

namespace JamDemo
{
    public class Shoot
    {
        private GameFunc gameFunc;
        private PlayerFunc playerFunc;

        public Shoot(GameFunc gameFunc, PlayerFunc playerFunc)
        {
            this.gameFunc = gameFunc;
            this.playerFunc = playerFunc;
        }

        public bool CanShoot(PlayerModel player)
        {
            if (player == null || !player.IsAlive)
                return false;

            return player.Ammo > 0;
        }

        public bool ExecuteShoot(PlayerModel shooter, PlayerModel target, List<PlayerModel> allPlayers)
        {
            // 1. Temel kontroller
            if (!ValidateShoot(shooter, target, allPlayers))
                return false;

            // 2. Mermi harca
            shooter.Ammo--;
            Console.WriteLine($"💥 {shooter.PlayerName} shoots at {target.PlayerName}! (Ammo left: {shooter.Ammo})");

            // 3. Hedef korunuyorsa
            if (target.IsProtected)
            {
                Console.WriteLine($"🛡️ {target.PlayerName} was protected! Shot blocked.");
                return true; // Shoot successful ama hasar yok
            }

            // 4. Hedef mermi topluyorsa elimine olur
            if (IsTargetCollectingAmmo(target, allPlayers))
            {
                ApplyFatalShot(shooter, target);
                return true;
            }

            // 5. Normal shot (miss)
            Console.WriteLine($"🎯 {shooter.PlayerName} missed {target.PlayerName}!");
            return true;
        }

        public List<PlayerModel> GetValidTargets(PlayerModel shooter, List<PlayerModel> allPlayers)
        {
            if (shooter == null || !shooter.IsAlive)
                return new List<PlayerModel>();

            return allPlayers
                .Where(p => p.IsAlive && p.PlayerId != shooter.PlayerId)
                .ToList();
        }

        public PlayerModel SelectBestTarget(PlayerModel shooter, List<PlayerModel> allPlayers, ShootStrategy strategy = ShootStrategy.Random)
        {
            var validTargets = GetValidTargets(shooter, allPlayers);
            if (!validTargets.Any())
                return null;

            return strategy switch
            {
                ShootStrategy.Random => SelectRandomTarget(validTargets),
                ShootStrategy.HighestAmmo => SelectHighestAmmoTarget(validTargets),
                ShootStrategy.LowestAmmo => SelectLowestAmmoTarget(validTargets),
                ShootStrategy.MostKills => SelectMostKillsTarget(validTargets),
                ShootStrategy.Unprotected => SelectUnprotectedTarget(validTargets),
                ShootStrategy.Weakest => SelectWeakestTarget(validTargets),
                _ => SelectRandomTarget(validTargets)
            };
        }

        private bool ValidateShoot(PlayerModel shooter, PlayerModel target, List<PlayerModel> allPlayers)
        {
            if (shooter == null)
            {
                Console.WriteLine("❌ Shooter is null!");
                return false;
            }

            if (!shooter.IsAlive)
            {
                Console.WriteLine($"❌ {shooter.PlayerName} is dead and cannot shoot!");
                return false;
            }

            if (shooter.Ammo <= 0)
            {
                Console.WriteLine($"❌ {shooter.PlayerName} has no ammo!");
                return false;
            }

            if (target == null)
            {
                Console.WriteLine("❌ Target is null!");
                return false;
            }

            if (!target.IsAlive)
            {
                Console.WriteLine($"❌ {target.PlayerName} is already dead!");
                return false;
            }

            if (shooter.PlayerId == target.PlayerId)
            {
                Console.WriteLine($"❌ {shooter.PlayerName} cannot shoot themselves!");
                return false;
            }

            if (!allPlayers.Contains(shooter) || !allPlayers.Contains(target))
            {
                Console.WriteLine("❌ Player not in game!");
                return false;
            }

            return true;
        }

        private bool IsTargetCollectingAmmo(PlayerModel target, List<PlayerModel> allPlayers)
        {
            // Bu bilgiyi round resolver'dan alacağız
            // Şimdilik false döndür, gerçek implementasyon round resolver'da
            return false;
        }

        private void ApplyFatalShot(PlayerModel shooter, PlayerModel target)
        {
            target.IsAlive = false;
            target.Deaths++;
            shooter.Kills++;

            Console.WriteLine($"💀 {shooter.PlayerName} eliminated {target.PlayerName} while collecting ammo!");
            Console.WriteLine($"📊 {shooter.PlayerName} kills: {shooter.Kills} | {target.PlayerName} deaths: {target.Deaths}");
        }

        private PlayerModel SelectRandomTarget(List<PlayerModel> targets)
        {
            var random = new Random();
            return targets[random.Next(targets.Count)];
        }

        private PlayerModel SelectHighestAmmoTarget(List<PlayerModel> targets)
        {
            return targets.OrderByDescending(p => p.Ammo).First();
        }

        private PlayerModel SelectLowestAmmoTarget(List<PlayerModel> targets)
        {
            return targets.OrderBy(p => p.Ammo).First();
        }

        private PlayerModel SelectMostKillsTarget(List<PlayerModel> targets)
        {
            return targets.OrderByDescending(p => p.Kills).First();
        }

        private PlayerModel SelectUnprotectedTarget(List<PlayerModel> targets)
        {
            var unprotected = targets.Where(p => !p.IsProtected).ToList();
            return unprotected.Any() ? SelectRandomTarget(unprotected) : SelectRandomTarget(targets);
        }

        private PlayerModel SelectWeakestTarget(List<PlayerModel> targets)
        {
            // En az ammo + en az kill = en zayıf
            return targets.OrderBy(p => p.Ammo + p.Kills).First();
        }

        public ShootResult CalculateShootOutcome(PlayerModel shooter, PlayerModel target)
        {
            var result = new ShootResult
            {
                Shooter = shooter,
                Target = target,
                CanShoot = CanShoot(shooter),
                IsTargetProtected = target.IsProtected,
                AmmoAfterShot = shooter.Ammo - 1
            };

            if (!result.CanShoot)
            {
                result.Outcome = "Cannot shoot - no ammo";
            }
            else if (result.IsTargetProtected)
            {
                result.Outcome = "Shot blocked by protection";
            }
            else
            {
                result.Outcome = "Shot missed (normal)";
            }

            return result;
        }

        public bool IsShootViable(PlayerModel shooter, List<PlayerModel> allPlayers)
        {
            if (!CanShoot(shooter))
                return false;

            var validTargets = GetValidTargets(shooter, allPlayers);
            return validTargets.Any();
        }

        public List<ShootResult> GetAllPossibleShots(PlayerModel shooter, List<PlayerModel> allPlayers)
        {
            var results = new List<ShootResult>();
            var validTargets = GetValidTargets(shooter, allPlayers);

            foreach (var target in validTargets)
            {
                results.Add(CalculateShootOutcome(shooter, target));
            }

            return results;
        }
    }

    public enum ShootStrategy
    {
        Random,
        HighestAmmo,
        LowestAmmo,
        MostKills,
        Unprotected,
        Weakest
    }

    public class ShootResult
    {
        public PlayerModel Shooter { get; set; }
        public PlayerModel Target { get; set; }
        public bool CanShoot { get; set; }
        public bool IsTargetProtected { get; set; }
        public int AmmoAfterShot { get; set; }
        public string Outcome { get; set; }

        public override string ToString()
        {
            return $"{Shooter?.PlayerName} → {Target?.PlayerName}: {Outcome}";
        }
    }
}