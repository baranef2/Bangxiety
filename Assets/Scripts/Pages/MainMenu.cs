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
                Console.WriteLine("=== BANGXÝETY! JAM DEMO ===");
                Console.WriteLine("[S] Singleplayer (1 Player + 3 Bot)");
                Console.WriteLine("[M] Multiplayer (DISABLED)");
                Console.WriteLine("[Q] Quit");
                Console.Write("Choose: ");

                var key = Console.ReadKey(intercept: true).Key;
                if (key == ConsoleKey.S)
                {
                    Lobby.Show(); // LOBBY'E GEÇ (ayrý dosya)
                    // Lobby'den dönünce menü tekrar gösterilecek.
                }
                else if (key == ConsoleKey.M)
                {
                    Console.WriteLine("\nMultiplayer bu jam buildinde devre dýþý.");
                    Console.WriteLine("Devam etmek için bir tusa basin...");
                    Console.ReadKey();
                }
                else if (key == ConsoleKey.Q)
                {
                    Environment.Exit(0);
                }
                // aksi halde döngü devam eder (menü tekrar görünür)
            }
        }
    }
}
