using UnityEngine;
using TMPro;

public class PayRentUI : MonoBehaviour
{
    [Header("References")]
    public BoardTurns boardTurns;
    public GameMode gameMode;
    public GameObject FPSCanvas;

    [Header("Fight Settings")]
    public Transform player1FightSpawn;
    public Transform player2FightSpawn;

    [Header("UI")]
    public GameObject fightUI;
    public TextMeshProUGUI promptText;
    public GameObject EndTurnUI;

    private BoardSpace currentSpace;
    public int currentPlayerIndex;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (boardTurns == null)
            boardTurns = FindAnyObjectByType<BoardTurns>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Called when a player lands on an owned property
    public void Show(BoardSpace space, int playerIndex)
    {
        currentSpace = space;
        currentPlayerIndex = playerIndex;

        int rentAmount = space.rent;
        int ownerPlayer = space.ownerPlayerIndex + 1; // 1-based for display

        promptText.text = $"Pay ${rentAmount} rent to Player {ownerPlayer}?";

        gameObject.SetActive(true);
    }

    // Parameterless method for UI Button OnClick
    public void OnPayRent()
    {
        if (currentSpace == null)
        {
            Debug.LogError("No space set for rent payment!");
            return;
        }

        PayRent(currentSpace);

        // Close UI and show end turn
        gameObject.SetActive(false);
        EndTurnUI.SetActive(true);
    }

    public void OnFight()
    {
        // Fight against player 1 as default (can be changed)
        Fight(1);

        // Close UI
        gameObject.SetActive(false);
    }


    // Helper method to get the player by index
    private GameObject GetPlayer(int playerIndex)
    {
        return PlayerManager.Instance.GetPlayer(playerIndex);
    }

    
    public void PayRent(BoardSpace space)
    {
        if (space == null || !space.isOwned)
            return;

        int currentPlayer = boardTurns.currPlayer;
        int ownerPlayer = space.ownerPlayerIndex;

        // Don't pay rent if landing on your own property
        if (currentPlayer == ownerPlayer)
            return;

        int rentAmount = space.rent;

        // Get the current player's bank account using player
        GameObject currentPlayerObj = GetPlayer(currentPlayer);
        PlayerBankAccounts currentBank = currentPlayerObj != null ? currentPlayerObj.GetComponent<PlayerBankAccounts>() : null;

        // Get the owner's bank account using player
        GameObject ownerPlayerObj = GetPlayer(ownerPlayer);
        PlayerBankAccounts ownerBank = ownerPlayerObj != null ? ownerPlayerObj.GetComponent<PlayerBankAccounts>() : null;

        if (currentBank != null && ownerBank != null)
        {
            // Subtract rent from current player
            currentBank.RemoveMoney(rentAmount);

            // Add rent to property owner
            ownerBank.AddMoney(rentAmount);

            Debug.Log($"Player {currentPlayer + 1} paid ${rentAmount} rent to Player {ownerPlayer + 1}");
        }
    }

    /// <summary>
    /// Initiates a fight between the current player and another player.
    /// Moves the existing scene player objects to their fight spawn locations.
    /// </summary>
    /// <param name="opponentIndex">The index of the opponent player.</param>
    public void Fight(int opponentIndex)
    {
        int currentPlayer = boardTurns.currPlayer;

        // Get spawn positions based on player index
        Transform currentSpawn = currentPlayer == 0 ? player1FightSpawn : player2FightSpawn;
        Transform opponentSpawn = opponentIndex == 0 ? player1FightSpawn : player2FightSpawn;

        GameObject currentFightPlayer = GetPlayer(currentPlayer);
        GameObject opponentFightPlayer = GetPlayer(opponentIndex);

        if (currentFightPlayer == null || opponentFightPlayer == null)
        {
            Debug.LogError("Fight players not found in scene! Make sure PlayerManager has the correct player objects.");
            return;
        }

        if (currentSpawn != null)
        {
            currentFightPlayer.transform.position = currentSpawn.position;
            currentFightPlayer.transform.rotation = currentSpawn.rotation;
        }

        if (opponentSpawn != null)
        {
            opponentFightPlayer.transform.position = opponentSpawn.position;
            opponentFightPlayer.transform.rotation = opponentSpawn.rotation;
        }

        // Switch to FPS mode (all clients)
        gameMode.buttonSwitchGameMode();

        // Hide fight UI and show FPS canvas
        if (fightUI != null)
            fightUI.SetActive(false);
        if (FPSCanvas != null)
            FPSCanvas.SetActive(true);

        Debug.Log($"Fight started between Player {currentPlayer + 1} and Player {opponentIndex + 1}");
    }
}
