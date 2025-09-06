using System;

namespace JamDemo
{
    public class PlayerModel
    {
        public string PlayerId { get; set; }
        public string PlayerName { get; set; }
        public bool IsAlive { get; set; }
        public int Ammo { get; set; }
        public bool IsProtected { get; set; }
        public int ConsecutiveProtects { get; set; } // Art arda kaç raunt protect aldýðýný takip eder

        // Ýstatistikler
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int RoundsPlayed { get; set; }

        public PlayerModel()
        {
            IsAlive = true;
            Ammo = 0;
            IsProtected = false;
            ConsecutiveProtects = 0;
            Kills = 0;
            Deaths = 0;
            RoundsPlayed = 0;
        }

        public PlayerModel(string playerId, string playerName) : this()
        {
            PlayerId = playerId;
            PlayerName = playerName;
        }

        public bool CanExecute => Ammo >= 5;
        public bool CanShoot => Ammo > 0;
        public bool CanProtect => ConsecutiveProtects < 3;
    }
}