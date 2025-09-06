using UnityEngine;

public class BotsDriver : MonoBehaviour
{
    [SerializeField] private BotPlayer[] bots;
    private Player[] allPlayers;

    private void Start()
    {
        allPlayers = FindObjectsOfType<Player>();
    }

    private void Update()
    {
        // B tuþuna basýldýðýnda botlar seçim yapar, round oynanýr
        if (Input.GetKeyDown(KeyCode.B))
        {
            foreach (var b in bots)
                b.ChooseCard(allPlayers);

            GameManager.Instance.ExecuteRound();
        }
    }
}
