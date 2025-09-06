using System;

namespace JamDemo.Pages
{
    /// <summary>
    /// Jam için tek mod: Singleplayer (1 Oyuncu + 3 Bot).
    /// Þimdilik sadece iskelet. Devamýnda Match.Show(...) eklenecek.
    /// </summary>
    public static class Lobby
    {
        // Sabit konfig (ileride gerekirse parametre alacak yapýya çevrilebilir)
        private const int HumanCount = 1;
        private const int BotCount = 3;

        // Tur seçim süreleri (dokümandaki kurala göre)
        // 4 kiþi: 5 sn, 3 kiþi: 4 sn, 2 kiþi: 1 sn
        public static int GetRoundSelectSeconds(int aliveCount)
        {
            if (aliveCount >= 4) return 5;
            if (aliveCount == 3) return 4;
            return 1; // 2 ve altý
        }

        /// <summary>
        /// Lobi ekraný: konfigürasyonu gösterir, devam komutunu bekler.
        /// </summary>
        public static void Show()
        {
            Console.Clear();
            Console.WriteLine("=== LOBBY (Singleplayer) ===");
            Console.WriteLine($"Kurulum: {HumanCount} Oyuncu + {BotCount} Bot");
            Console.WriteLine("Round Süreleri: 4 kisi=5sn, 3 kisi=4sn, 2 kisi=1sn");
            Console.WriteLine();
            Console.WriteLine("[ENTER] Devam (Match akisi sonraki adimda eklenecek)");
            Console.ReadLine();

            // NOT: Match henüz yok. Sonraki adýmda:
            // Match.Show(HumanCount, BotCount);
            // Þimdilik MainMenu'ye geri dönmesi için sadece return ediyoruz.
        }
    }
}
