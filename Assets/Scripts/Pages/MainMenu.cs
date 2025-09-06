using System;

namespace JamDemo.Pages
{
    public static class MainMenu
    {
        public static void Show()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== BANGX�ETY! JAM DEMO ===");
                Console.WriteLine("[S] Singleplayer (1 Player + 3 Bot)");
                Console.WriteLine("[M] Multiplayer (DISABLED)");
                Console.WriteLine("[Q] Quit");
                Console.Write("Choose: ");

                var key = Console.ReadKey(intercept: true).Key;
                if (key == ConsoleKey.S)
                {
                    Lobby.Show(); // LOBBY'E GE� (ayr� dosya)
                    // Lobby'den d�n�nce men� tekrar g�sterilecek.
                }
                else if (key == ConsoleKey.M)
                {
                    Console.WriteLine("\nMultiplayer bu jam buildinde devre d���.");
                    Console.WriteLine("Devam etmek i�in bir tusa basin...");
                    Console.ReadKey();
                }
                else if (key == ConsoleKey.Q)
                {
                    Environment.Exit(0);
                }
                // aksi halde d�ng� devam eder (men� tekrar g�r�n�r)
            }
        }
    }
}
