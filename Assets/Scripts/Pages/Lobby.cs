using System;

namespace JamDemo.Pages
{
    /// <summary>
    /// Jam i�in tek mod: Singleplayer (1 Oyuncu + 3 Bot).
    /// �imdilik sadece iskelet. Devam�nda Match.Show(...) eklenecek.
    /// </summary>
    public static class Lobby
    {
        // Sabit konfig (ileride gerekirse parametre alacak yap�ya �evrilebilir)
        private const int HumanCount = 1;
        private const int BotCount = 3;

        // Tur se�im s�releri (dok�mandaki kurala g�re)
        // 4 ki�i: 5 sn, 3 ki�i: 4 sn, 2 ki�i: 1 sn
        public static int GetRoundSelectSeconds(int aliveCount)
        {
            if (aliveCount >= 4) return 5;
            if (aliveCount == 3) return 4;
            return 1; // 2 ve alt�
        }

        /// <summary>
        /// Lobi ekran�: konfig�rasyonu g�sterir, devam komutunu bekler.
        /// </summary>
        public static void Show()
        {
            Console.Clear();
            Console.WriteLine("=== LOBBY (Singleplayer) ===");
            Console.WriteLine($"Kurulum: {HumanCount} Oyuncu + {BotCount} Bot");
            Console.WriteLine("Round S�releri: 4 kisi=5sn, 3 kisi=4sn, 2 kisi=1sn");
            Console.WriteLine();
            Console.WriteLine("[ENTER] Devam (Match akisi sonraki adimda eklenecek)");
            Console.ReadLine();

            // NOT: Match hen�z yok. Sonraki ad�mda:
            // Match.Show(HumanCount, BotCount);
            // �imdilik MainMenu'ye geri d�nmesi i�in sadece return ediyoruz.
        }
    }
}
