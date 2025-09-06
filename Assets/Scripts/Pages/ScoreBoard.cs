using System;

namespace JamDemo.Pages
{
    /// <summary>
    /// Oyun sonu ekran� (konsol).
    /// R2 taraf� bitti�inde buraya kazanan/durum ve �zet metrikler g�nderilir.
    /// </summary>
    public static class ScoreBoard
    {
        /// <summary>
        /// Kazanan varsa �a��r.
        /// </summary>
        /// <param name="winnerId">"PLAYER", "BOT_A", "BOT_B" vb.</param>
        /// <param name="roundsPlayed">Toplam tur</param>
        /// <param name="killsByWinner">Kazanan�n kill say�s� (yoksa 0)</param>
        public static void ShowWinner(string winnerId, int roundsPlayed, int killsByWinner = 0)
        {
            Console.Clear();
            Console.WriteLine("=== SCOREBOARD ===");
            Console.WriteLine("Sonuc   : WIN");
            Console.WriteLine($"Kazanan : {winnerId}");
            Console.WriteLine($"Turlar  : {roundsPlayed}");
            Console.WriteLine($"Kills   : {killsByWinner}");
            Console.WriteLine();
            Console.WriteLine("[ENTER] Ana men�ye d�n");
            Console.ReadLine();
            // Not: MainMenu.Show() taraf�ndaki while d�ng�s� men�y� yeniden g�sterecek.
        }

        /// <summary>
        /// Beraberlik durumunda �a��r.
        /// </summary>
        public static void ShowDraw(int roundsPlayed)
        {
            Console.Clear();
            Console.WriteLine("=== SCOREBOARD ===");
            Console.WriteLine("Sonuc   : DRAW");
            Console.WriteLine($"Turlar  : {roundsPlayed}");
            Console.WriteLine();
            Console.WriteLine("[ENTER] Ana men�ye d�n");
            Console.ReadLine();
        }
    }
}
