using UnityEngine;

public class RoundDebugTrigger : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            GameManager.Instance.ExecuteRound();
    }
}
