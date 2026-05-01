using UnityEngine;
using UnityEngine.UI;

public class StartFightButton : MonoBehaviour
{
    [Header("References")]
    public PayRentUI payRentUI;        // Drag your PayRentUI object here
    public BoardTurns boardTurns;      // Drag BoardTurns here
    public Button fightButton;         // Assign the UI Button

    private void Awake()
    {
        if (boardTurns == null)
            boardTurns = FindAnyObjectByType<BoardTurns>();

        if (payRentUI == null)
            payRentUI = FindAnyObjectByType<PayRentUI>();

        if (fightButton != null)
            fightButton.onClick.AddListener(StartFight);
    }

    public void StartFight()
    {
        int currentPlayer = boardTurns.currPlayer;
        int totalPlayers = boardTurns.playerGameObjects.Length;

        // Automatically pick the next player in turn order
        int opponentIndex = (currentPlayer + 1) % totalPlayers;

        Debug.Log($"StartFightButton: Auto‑fight between Player {currentPlayer + 1} and Player {opponentIndex + 1}");

        payRentUI.Fight(opponentIndex);
    }
}
