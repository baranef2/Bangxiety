using System;
using System.Collections.Generic;
using System.Linq;

namespace JamDemo
{
    public class AddPlayer
    {
        private GameFunc gameFunc;
        private PlayerFunc playerFunc;

        // Events
        public event Action<PlayerModel, int> OnPlayerAdded; // Player, current count
        public event Action<string> OnPlayerAddFailed; // Error message
        public event Action<int> OnLobbyFull;

        public AddPlayer(GameFunc gameFunc, PlayerFunc playerFunc)
        {
            this.gameFunc = gameFunc;
            this.playerFunc = playerFunc;
        }

        public bool TryAddPlayer(List<PlayerModel> playerList, string playerId, string playerName)
        {
            // Null kontrolleri
            if (string.IsNullOrEmpty(playerId))
            {
                var error = "Player ID cannot be empty!";
                Console.WriteLine($"❌ {error}");
                OnPlayerAddFailed?.Invoke(error);
                return false;
            }

            if (string.IsNullOrEmpty(playerName))
            {
                var error = "Player name cannot be empty!";
                Console.WriteLine($"❌ {error}");
                OnPlayerAddFailed?.Invoke(error);
                return false;
            }

            // Oyuncu adı uzunluk kontrolü
            if (playerName.Length > 20)
            {
                var error = "Player name too long! Maximum 20 characters.";
                Console.WriteLine($"❌ {error}");
                OnPlayerAddFailed?.Invoke(error);
                return false;
            }

            // Oyuncu adı karakter kontrolü
            if (!IsValidPlayerName(playerName))
            {
                var error = "Invalid characters in player name! Use only letters, numbers, spaces, and basic symbols.";
                Console.WriteLine($"❌ {error}");
                OnPlayerAddFailed?.Invoke(error);
                return false;
            }

            // Maksimum oyuncu sayısı kontrolü
            if (playerList.Count >= 8)
            {
                var error = "Lobby is full! Maximum 8 players allowed.";
                Console.WriteLine($"❌ {error}");
                OnLobbyFull?.Invoke(playerList.Count);
                return false;
            }

            // Aynı ID ile oyuncu var mı kontrolü
            if (playerList.Any(p => p.PlayerId == playerId))
            {
                var error = $"Player with ID '{playerId}' is already in the game!";
                Console.WriteLine($"❌ {error}");
                OnPlayerAddFailed?.Invoke(error);
                return false;
            }

            // Aynı isimde oyuncu var mı kontrolü
            if (playerList.Any(p => p.PlayerName.Equals(playerName, StringComparison.OrdinalIgnoreCase)))
            {
                var error = $"Player name '{playerName}' is already taken!";
                Console.WriteLine($"❌ {error}");
                OnPlayerAddFailed?.Invoke(error);
                return false;
            }

            // Oyuncuyu oluştur ve ekle
            var newPlayer = CreateNewPlayer(playerId, playerName);
            playerList.Add(newPlayer);

            Console.WriteLine($"✅ {playerName} joined the lobby! ({playerList.Count}/8 players)");
            ShowCurrentLobby(playerList);

            OnPlayerAdded?.Invoke(newPlayer, playerList.Count);

            // 4 oyuncuya ulaştığında bildirim
            if (playerList.Count == 4)
            {
                Console.WriteLine("🎮 Minimum players reached! Game can be started.");
            }

            return true;
        }

        private PlayerModel CreateNewPlayer(string playerId, string playerName)
        {
            var player = new PlayerModel(playerId, playerName);

            // Yeni oyuncu için başlangıç değerleri zaten PlayerModel constructor'ında ayarlı
            // Ekstra ayarlamalar burada yapılabilir

            Console.WriteLine($"🆕 New player created: {playerName} (ID: {playerId})");
            return player;
        }

        private bool IsValidPlayerName(string playerName)
        {
            // Sadece harf, rakam, boşluk ve temel semboller
            return playerName.All(c => char.IsLetterOrDigit(c) || c == ' ' || c == '_' || c == '-' || c == '.');
        }

        private void ShowCurrentLobby(List<PlayerModel> players)
        {
            Console.WriteLine("\n--- Current Lobby ---");
            for (int i = 0; i < players.Count; i++)
            {
                var player = players[i];
                Console.WriteLine($"{i + 1}. {player.PlayerName} (ID: {player.PlayerId})");
            }
            Console.WriteLine($"Total: {players.Count}/8 players");

            var spotsLeft = 8 - players.Count;
            if (spotsLeft > 0)
            {
                Console.WriteLine($"💺 {spotsLeft} spots remaining");
            }

            // Oyun başlatma durumu
            if (players.Count >= 4)
            {
                Console.WriteLine("✅ Ready to start game!");
            }
            else
            {
                var needed = 4 - players.Count;
                Console.WriteLine($"⏳ Need {needed} more players to start");
            }
            Console.WriteLine();
        }

        public bool CanAddMorePlayers(List<PlayerModel> playerList)
        {
            return playerList.Count < 8;
        }

        public int GetRemainingSlots(List<PlayerModel> playerList)
        {
            return 8 - playerList.Count;
        }

        public bool IsReadyToStart(List<PlayerModel> playerList)
        {
            return playerList.Count >= 4;
        }

        public List<string> GetPlayerNames(List<PlayerModel> playerList)
        {
            return playerList.Select(p => p.PlayerName).ToList();
        }

        public List<string> GetPlayerIds(List<PlayerModel> playerList)
        {
            return playerList.Select(p => p.PlayerId).ToList();
        }

        public PlayerModel GetPlayerById(List<PlayerModel> playerList, string playerId)
        {
            return playerList.FirstOrDefault(p => p.PlayerId == playerId);
        }

        public PlayerModel GetPlayerByName(List<PlayerModel> playerList, string playerName)
        {
            return playerList.FirstOrDefault(p => p.PlayerName.Equals(playerName, StringComparison.OrdinalIgnoreCase));
        }

        public bool BatchAddPlayers(List<PlayerModel> playerList, Dictionary<string, string> playersToAdd)
        {
            var successCount = 0;
            var failCount = 0;

            Console.WriteLine($"🔄 Adding {playersToAdd.Count} players to lobby...");

            foreach (var kvp in playersToAdd)
            {
                var playerId = kvp.Key;
                var playerName = kvp.Value;

                if (TryAddPlayer(playerList, playerId, playerName))
                {
                    successCount++;
                }
                else
                {
                    failCount++;
                    Console.WriteLine($"❌ Failed to add: {playerName} (ID: {playerId})");
                }

                // Lobby doluysa dur
                if (playerList.Count >= 8)
                {
                    break;
                }
            }

            Console.WriteLine($"📊 Batch add complete: {successCount} added, {failCount} failed");
            return failCount == 0;
        }

        public LobbyStatus GetLobbyStatus(List<PlayerModel> playerList)
        {
            return new LobbyStatus
            {
                CurrentPlayerCount = playerList.Count,
                MaxPlayerCount = 8,
                RemainingSlots = GetRemainingSlots(playerList),
                CanStartGame = IsReadyToStart(playerList),
                IsLobbyFull = playerList.Count >= 8,
                PlayerNames = GetPlayerNames(playerList)
            };
        }

        // Oyuncu önizlemesi için
        public string PreviewPlayerAddition(List<PlayerModel> currentPlayers, string newPlayerName)
        {
            var afterCount = currentPlayers.Count + 1;
            var status = afterCount >= 4 ? "Ready to start!" : $"Need {4 - afterCount} more";

            return $"Adding '{newPlayerName}' → {afterCount}/8 players ({status})";
        }
    }

    public class LobbyStatus
    {
        public int CurrentPlayerCount { get; set; }
        public int MaxPlayerCount { get; set; }
        public int RemainingSlots { get; set; }
        public bool CanStartGame { get; set; }
        public bool IsLobbyFull { get; set; }
        public List<string> PlayerNames { get; set; }

        public LobbyStatus()
        {
            PlayerNames = new List<string>();
        }

        public override string ToString()
        {
            var status = CanStartGame ? "✅ Ready" : "⏳ Waiting";
            return $"Lobby ({CurrentPlayerCount}/{MaxPlayerCount}) - {status}";
        }
    }
}