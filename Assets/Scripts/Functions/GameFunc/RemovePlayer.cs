using System;
using System.Collections.Generic;
using System.Linq;

namespace JamDemo
{
    public class RemovePlayer
    {
        // Events
        public event Action<PlayerModel, int, string> OnPlayerRemoved; // Player, remaining count, reason
        public event Action<string> OnPlayerRemoveFailed; // Error message
        public event Action<int> OnLobbyBelowMinimum; // Player count below 4
        public event Action<PlayerModel> OnLastPlayerLeft; // Last player left

        public RemovePlayer()
        {
        }

        public bool TryRemovePlayerById(List<PlayerModel> playerList, string playerId, string reason = "Player left")
        {
            if (string.IsNullOrEmpty(playerId))
            {
                var error = "Player ID cannot be empty!";
                Console.WriteLine($"❌ {error}");
                OnPlayerRemoveFailed?.Invoke(error);
                return false;
            }

            var player = playerList.FirstOrDefault(p => p.PlayerId == playerId);
            if (player == null)
            {
                var error = $"Player with ID '{playerId}' not found!";
                Console.WriteLine($"❌ {error}");
                OnPlayerRemoveFailed?.Invoke(error);
                return false;
            }

            return RemovePlayerInternal(playerList, player, reason);
        }

        public bool TryRemovePlayerByName(List<PlayerModel> playerList, string playerName, string reason = "Player left")
        {
            if (string.IsNullOrEmpty(playerName))
            {
                var error = "Player name cannot be empty!";
                Console.WriteLine($"❌ {error}");
                OnPlayerRemoveFailed?.Invoke(error);
                return false;
            }

            var player = playerList.FirstOrDefault(p => p.PlayerName.Equals(playerName, StringComparison.OrdinalIgnoreCase));
            if (player == null)
            {
                var error = $"Player '{playerName}' not found!";
                Console.WriteLine($"❌ {error}");
                OnPlayerRemoveFailed?.Invoke(error);
                return false;
            }

            return RemovePlayerInternal(playerList, player, reason);
        }

        private bool RemovePlayerInternal(List<PlayerModel> playerList, PlayerModel player, string reason)
        {
            // Oyuncuyu listeden çıkar
            var wasRemoved = playerList.Remove(player);

            if (!wasRemoved)
            {
                var error = "Failed to remove player from list!";
                Console.WriteLine($"❌ {error}");
                OnPlayerRemoveFailed?.Invoke(error);
                return false;
            }

            Console.WriteLine($"🚪 {player.PlayerName} left the lobby! ({reason})");
            Console.WriteLine($"Remaining players: {playerList.Count}/8");

            // Event'i tetikle
            OnPlayerRemoved?.Invoke(player, playerList.Count, reason);

            // Lobby durumunu kontrol et
            CheckLobbyStatus(playerList);

            // Güncel lobby'i göster
            if (playerList.Count > 0)
            {
                ShowCurrentLobby(playerList);
            }
            else
            {
                Console.WriteLine("🏠 Lobby is now empty.");
                OnLastPlayerLeft?.Invoke(player);
            }

            return true;
        }

        public int RemoveInactivePlayers(List<PlayerModel> playerList)
        {
            // Eliminate olmuş oyuncuları kaldır
            var removedCount = 0;
            var playersToRemove = new List<PlayerModel>();

            foreach (var player in playerList.ToList()) // ToList() ile kopya oluştur
            {
                // Eliminate olmuş oyuncuları kaldır
                if (!player.IsAlive)
                {
                    playersToRemove.Add(player);
                }
            }

            foreach (var player in playersToRemove)
            {
                if (RemovePlayerInternal(playerList, player, "Inactive/Eliminated"))
                {
                    removedCount++;
                }
            }

            if (removedCount > 0)
            {
                Console.WriteLine($"🧹 Removed {removedCount} inactive/eliminated players");
            }

            return removedCount;
        }

        public bool RemoveAllPlayers(List<PlayerModel> playerList, string reason = "Lobby cleared")
        {
            var playerCount = playerList.Count;
            var removedPlayers = new List<PlayerModel>(playerList);

            playerList.Clear();

            foreach (var player in removedPlayers)
            {
                OnPlayerRemoved?.Invoke(player, 0, reason);
            }

            Console.WriteLine($"🧹 All players removed from lobby! ({playerCount} players cleared)");
            Console.WriteLine($"Reason: {reason}");

            return true;
        }

        private void CheckLobbyStatus(List<PlayerModel> playerList)
        {
            var playerCount = playerList.Count;

            if (playerCount == 0)
            {
                Console.WriteLine("⚠️ Lobby is now empty!");
            }
            else if (playerCount < 4)
            {
                var needed = 4 - playerCount;
                Console.WriteLine($"⚠️ Below minimum players! Need {needed} more to start game.");
                OnLobbyBelowMinimum?.Invoke(playerCount);
            }
            else
            {
                Console.WriteLine($"✅ {playerCount} players remaining - can still start game!");
            }
        }

        private void ShowCurrentLobby(List<PlayerModel> players)
        {
            Console.WriteLine("\n--- Updated Lobby ---");
            for (int i = 0; i < players.Count; i++)
            {
                var player = players[i];
                var status = player.IsAlive ? "✅" : "❌";
                Console.WriteLine($"{i + 1}. {player.PlayerName} {status} (ID: {player.PlayerId})");
            }
            Console.WriteLine($"Total: {players.Count}/8 players");

            var spotsLeft = 8 - players.Count;
            Console.WriteLine($"💺 {spotsLeft} spots available");
            Console.WriteLine();
        }

        public bool CanRemovePlayer(List<PlayerModel> playerList, string playerId)
        {
            return playerList.Any(p => p.PlayerId == playerId);
        }

        public List<PlayerModel> GetRemovablePlayers(List<PlayerModel> playerList)
        {
            // Tüm oyuncular kaldırılabilir, ama bazı durumlar olabilir
            return playerList.ToList();
        }

        public RemovalResult PreviewPlayerRemoval(List<PlayerModel> playerList, string playerId)
        {
            var player = playerList.FirstOrDefault(p => p.PlayerId == playerId);
            if (player == null)
            {
                return new RemovalResult
                {
                    CanRemove = false,
                    Message = "Player not found",
                    RemainingCount = playerList.Count
                };
            }

            var afterCount = playerList.Count - 1;
            var canStartAfter = afterCount >= 4;
            var message = canStartAfter ? "Still can start game" : $"Need {4 - afterCount} more players";

            return new RemovalResult
            {
                CanRemove = true,
                PlayerName = player.PlayerName,
                RemainingCount = afterCount,
                CanStartGameAfter = canStartAfter,
                Message = message
            };
        }

        public void HandlePlayerDisconnect(List<PlayerModel> playerList, string playerId)
        {
            TryRemovePlayerById(playerList, playerId, "Disconnected");
        }

        public void HandlePlayerKick(List<PlayerModel> playerList, string playerId, string kickReason = "Kicked by admin")
        {
            TryRemovePlayerById(playerList, playerId, kickReason);
        }

        public bool IsLobbyEmpty(List<PlayerModel> playerList)
        {
            return playerList.Count == 0;
        }

        public bool IsBelowMinimum(List<PlayerModel> playerList)
        {
            return playerList.Count < 4;
        }
    }

    public class RemovalResult
    {
        public bool CanRemove { get; set; }
        public string PlayerName { get; set; }
        public int RemainingCount { get; set; }
        public bool CanStartGameAfter { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            if (!CanRemove)
                return $"❌ {Message}";

            return $"Removing '{PlayerName}' → {RemainingCount}/8 players ({Message})";
        }
    }
}