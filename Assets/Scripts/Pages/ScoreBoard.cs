using System;

namespace JamDemo.Pages
{
    /// <summary>
    /// Oyun sonu ekraný (konsol).
    /// R2 tarafý bittiðinde buraya kazanan/durum ve özet metrikler gönderilir.
    /// </summary>
    public static class ScoreBoard
    {
        /// <summary>
        /// Kazanan varsa çaðýr.
        /// </summary>
        /// <param name="winnerId">"PLAYER", "BOT_A", "BOT_B" vb.</param>
        /// <param name="roundsPlayed">Toplam tur</param>
        /// <param name="killsByWinner">Kazananýn kill sayýsý (yoksa 0)</param>
        public static void ShowWinner(string winnerId, int roundsPlayed, int killsByWinner = 0)
        {
            Console.Clear();
            Console.WriteLine("=== SCOREBOARD ===");
            Console.WriteLine("Sonuc   : WIN");
            Console.WriteLine($"Kazanan : {winnerId}");
            Console.WriteLine($"Turlar  : {roundsPlayed}");
            Console.WriteLine($"Kills   : {killsByWinner}");
            Console.WriteLine();
            Console.WriteLine("[ENTER] Ana menüye dön");
            Console.ReadLine();
            // Not: MainMenu.Show() tarafýndaki while döngüsü menüyü yeniden gösterecek.
        }

        /// <summary>
        /// Beraberlik durumunda çaðýr.
        /// </summary>
        public static void ShowDraw(int roundsPlayed)
        {
            Console.Clear();
            Console.WriteLine("=== SCOREBOARD ===");
            Console.WriteLine("Sonuc   : DRAW");
            Console.WriteLine($"Turlar  : {roundsPlayed}");
            Console.WriteLine();
            Console.WriteLine("[ENTER] Ana menüye dön");
            Console.ReadLine();
        }
    }
}
