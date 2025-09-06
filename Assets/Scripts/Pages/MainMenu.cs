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
                Console.WriteLine("=== 007 JAM DEMO ===");
                Console.WriteLine("[S] Singleplayer (1 Oyuncu + 3 Bot)");
                Console.WriteLine("[M] Multiplayer (DEVRE DISI)");
                Console.WriteLine("[Q] Quit");
                Console.Write("Secim: ");

                var key = Console.ReadKey(intercept: true).Key;
                if (key == ConsoleKey.S)
                {
                    ShowLobby(); // ayrý dosya yerine, ayný sýnýf içindeki method
                }
                else if (key == ConsoleKey.M)
                {
                    Console.WriteLine("\nMultiplayer bu jam buildinde devre dýþý.");
                    Console.ReadKey();
                }
                else if (key == ConsoleKey.Q)
                {
                    Environment.Exit(0);
                }
                // aksi halde döngü tekrar baþa sarar
            }
        }

        private static void ShowLobby()
        {
            Console.Clear();
            Console.WriteLine("=== LOBBY (Singleplayer) ===");
            Console.WriteLine("Kurulum: 1 Oyuncu + 3 Bot");
            Console.WriteLine("Round Secim Süreleri: 4 kisi=5sn, 3 kisi=4sn, 2 kisi=1sn");
            Console.WriteLine();
            Console.WriteLine("[ENTER] Devam (Match akisi sonraki adimda eklenecek)");
            Console.ReadLine();

            // Þimdilik sadece geri dön -> menü döngüsüne
        }
    }
}
