using UnityEngine;
public class BotsDriver : MonoBehaviour 
{ [SerializeField] private BotPlayer[] bots; 
    private Player[] allPlayers; 
    private void Start() 
    { allPlayers = FindObjectsOfType<Player>(); }


    private void RunBotTurn()
    {
        Debug.Log("[BotsDriver] RunBotTurn çaðrýldý");

        var allPlayers = FindObjectsOfType<Player>();
        foreach (var b in bots)
        {
            if (!b) continue;
            var p = b.GetComponent<Player>();
            Debug.Log($"[BotsDriver] Bot: {b.name}, IsAlive={p.IsAlive}");

            if (p && p.IsAlive)
            {
                b.ChooseCard(allPlayers);
            }
        }

        GameManager.Instance.ExecuteRound();
    }


    private void Update() 


    { if (Input.GetKeyDown(KeyCode.B)) 
        { var allPlayers = FindObjectsOfType<Player>(); 
            foreach (var b in bots) 
            { if (!b) continue; var p = b.GetComponent<Player>(); 
                if (p && p.IsAlive) b.ChooseCard(allPlayers); }
            GameManager.Instance.ExecuteRound(); 
        }
    }
}