using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JamDemo
{
    public class StartGame
    {
        private GameFunc gameFunc;
        private PlayerFunc playerFunc;
        private RoundResolver roundResolver;

        public StartGame(GameFunc gameFunc, PlayerFunc playerFunc)
        {
            this.gameFunc = gameFunc;
            this.playerFunc = playerFunc;
            this.roundResolver = new RoundResolver(gameFunc, playerFunc);
        }

        public bool InitializeGame(List<PlayerModel> players)
        {
            if (players == null || players.Count < 4)
            {
                Console.WriteLine($"Need at least 4 players to start! Currently have {players?.Count ?? 0} players.");
                return false;
            }

            if (players.Count > 8)
            {
                Console.WriteLine("Maximum 8 players allowed!");
                return false;
            }

            // Oyuncu durumlarını başlangıç değerlerine sıfırla
            foreach (var player in players)
            {
                ResetPlayerForNewGame(player);
            }

            Console.WriteLine($"Game initialized with {players.Count} players!");
            return true;
        }

        public GameSession CreateGameSession(List<PlayerModel> players)
        {
            if (!InitializeGame(players))
                return null;

            var gameSession = new GameSession
            {
                GameId = Guid.NewGuid().ToString(),
                Players = players.ToList(),
                IsActive = true,
                CurrentRound = 0,
                StartTime = DateTime.Now,
                MaxRounds = 100, // Güvenlik için maksimum raunt sayısı
                TimeLimit = GetInitialTimeLimit(players.Count)
            };

            Console.WriteLine($"Game session created: {gameSession.GameId}");
            Console.WriteLine($"Players: {string.Join(", ", players.Select(p => p.PlayerName))}");
            Console.WriteLine($"Initial time limit: {gameSession.TimeLimit} seconds");

            return gameSession;
        }

        public async Task<bool> StartRound(GameSession gameSession, Dictionary<string, PlayerAction> playerActions)
        {
            if (gameSession == null || !gameSession.IsActive)
            {
                Console.WriteLine("No active game session!");
                return false;
            }

            var alivePlayers = gameSession.Players.Where(p => p.IsAlive).ToList();
            if (alivePlayers.Count < 2)
            {
                Console.WriteLine("Not enough alive players to continue!");
                return false;
            }

            gameSession.CurrentRound++;
            Console.WriteLine($"\n=== ROUND {gameSession.CurrentRound} START ===");
            Console.WriteLine($"Alive players: {string.Join(", ", alivePlayers.Select(p => p.PlayerName))}");
            Console.WriteLine($"Time limit: {GetCurrentTimeLimit(alivePlayers.Count)} seconds");

            // Raunt öncesi oyuncu durumlarını göster
            ShowPlayerStatus(alivePlayers);

            // Aksiyonları doğrula
            ValidatePlayerActions(alivePlayers, playerActions);

            // Raunt çözümlemesi
            roundResolver.ResolveRound(alivePlayers, playerActions);

            // Raunt sonrası durum
            Console.WriteLine($"=== ROUND {gameSession.CurrentRound} END ===");
            ShowRoundResults(gameSession.Players);

            // Zaman limitini güncelle
            gameSession.TimeLimit = GetCurrentTimeLimit(alivePlayers.Count(p => p.IsAlive));

            // Oyun bitiş kontrolü
            if (IsGameOver(gameSession))
            {
                return false; // Oyun bitti
            }

            // Maksimum raunt kontrolü
            if (gameSession.CurrentRound >= gameSession.MaxRounds)
            {
                Console.WriteLine("Maximum rounds reached!");
                return false;
            }

            return true; // Oyun devam ediyor
        }

        private void ResetPlayerForNewGame(PlayerModel player)
        {
            player.IsAlive = true;
            player.Ammo = 0;
            player.IsProtected = false;
            player.ConsecutiveProtects = 0;
            player.Kills = 0;
            player.Deaths = 0;
            player.RoundsPlayed = 0;
        }

        private int GetInitialTimeLimit(int playerCount)
        {
            return roundResolver.GetRoundTimeLimit(playerCount);
        }

        private int GetCurrentTimeLimit(int alivePlayerCount)
        {
            return roundResolver.GetRoundTimeLimit(alivePlayerCount);
        }

        private void ValidatePlayerActions(List<PlayerModel> alivePlayers, Dictionary<string, PlayerAction> playerActions)
        {
            foreach (var player in alivePlayers)
            {
                if (!playerActions.ContainsKey(player.PlayerId))
                {
                    // Varsayılan aksiyon: Ammo toplama
                    playerActions[player.PlayerId] = new PlayerAction
                    {
                        PlayerId = player.PlayerId,
                        ActionType = ActionType.Ammo
                    };
                    Console.WriteLine($"⚠️ {player.PlayerName} didn't choose an action! Default: Ammo");
                }
            }
        }

        private void ShowPlayerStatus(List<PlayerModel> players)
        {
            Console.WriteLine("\n--- Player Status ---");
            foreach (var player in players)
            {
                var protectInfo = player.ConsecutiveProtects > 0
                    ? $"(Protected {player.ConsecutiveProtects}/3)"
                    : "";

                Console.WriteLine($"🎮 {player.PlayerName}: Ammo={player.Ammo} {protectInfo}");
            }
        }

        private void ShowRoundResults(List<PlayerModel> allPlayers)
        {
            var alivePlayers = allPlayers.Where(p => p.IsAlive).ToList();
            var eliminatedPlayers = allPlayers.Where(p => !p.IsAlive).ToList();

            Console.WriteLine($"\n--- Round Results ---");
            Console.WriteLine($"✅ Alive ({alivePlayers.Count}): {string.Join(", ", alivePlayers.Select(p => $"{p.PlayerName}(Ammo:{p.Ammo})"))}");

            if (eliminatedPlayers.Any())
            {
                Console.WriteLine($"❌ Eliminated ({eliminatedPlayers.Count}): {string.Join(", ", eliminatedPlayers.Select(p => p.PlayerName))}");
            }
        }

        private bool IsGameOver(GameSession gameSession)
        {
            return roundResolver.IsGameOver(gameSession.Players);
        }

        public List<PlayerModel> GetAvailableActions(PlayerModel player)
        {
            var actions = new List<string> { "Ammo" };

            if (player.CanShoot)
                actions.Add("Shoot");

            if (player.CanProtect)
                actions.Add("Protect");

            if (player.CanExecute)
                actions.Add("Execute");

            Console.WriteLine($"{player.PlayerName} available actions: {string.Join(", ", actions)}");
            return null; // Actions listesi string olarak döndürülüyor, gerekirse PlayerAction listesi yapılabilir
        }
    }

    public class GameSession
    {
        public string GameId { get; set; }
        public List<PlayerModel> Players { get; set; }
        public bool IsActive { get; set; }
        public int CurrentRound { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int MaxRounds { get; set; }
        public int TimeLimit { get; set; }
        public PlayerModel Winner { get; set; }
        public string EndReason { get; set; }

        public GameSession()
        {
            Players = new List<PlayerModel>();
        }

        public TimeSpan GameDuration => (EndTime ?? DateTime.Now) - StartTime;
        public List<PlayerModel> AlivePlayers => Players.Where(p => p.IsAlive).ToList();
        public List<PlayerModel> EliminatedPlayers => Players.Where(p => !p.IsAlive).ToList();
    }
}