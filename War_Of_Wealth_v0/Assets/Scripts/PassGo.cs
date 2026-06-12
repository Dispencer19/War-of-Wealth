using UnityEngine;

public class PassGo : MonoBehaviour
{
    [Header("References")]
    public BoardTurns boardTurns;
    public GameObject passGoUI;
    public GameObject endTurnUI;

    [Header("Settings")]
    public int passGoAmount = 200;

    public void Collect200()
    {
        if (boardTurns == null)
            boardTurns = FindFirstObjectByType<BoardTurns>();

        if (boardTurns == null)
        {
            Debug.LogError("PassGo: BoardTurns reference not assigned and none found in scene.");
            return;
        }

        if (PlayerManager.Instance == null)
        {
            Debug.LogError("PassGo: PlayerManager instance not found.");
            return;
        }

        int currentPlayer = boardTurns.currPlayer;
        PlayerBankAccounts bank = PlayerManager.Instance.GetPlayerBank(currentPlayer);

        if (bank == null)
        {
            Debug.LogError($"PassGo: No PlayerBankAccounts found for player index {currentPlayer}.");
            return;
        }

        bank.AddMoney(passGoAmount);
        Debug.Log($"PassGo: Player {currentPlayer + 1} collected ${passGoAmount}.");

        if (passGoUI != null)
            passGoUI.SetActive(false);

        if (endTurnUI != null)
            endTurnUI.SetActive(true);
    }
}
