using UnityEngine;
using Photon.Pun;
using TMPro;

public class PayRentUI : MonoBehaviour
{
    [Header("References")]
    public BoardTurns boardTurns;
    public GameMode gameMode;
    public GameObject FPSCanvas;
    public NetworkUIManager networkUI;

    [Header("Fight Settings")]
    public GameObject playerPrefab; // The player prefab to spawn for fights
    public Transform player1FightSpawn;
    public Transform player2FightSpawn;

    [Header("UI")]
    public GameObject fightUI;
    public TextMeshProUGUI promptText;
    public GameObject EndTurnUI;

    private PhotonView photonView;
    private BoardSpace currentSpace;
    private int currentPlayerIndex;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (boardTurns == null)
            boardTurns = FindAnyObjectByType<BoardTurns>();
        
        if (networkUI == null)
            networkUI = FindAnyObjectByType<NetworkUIManager>();
        
        photonView = GetComponent<PhotonView>();
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

        // Show UI locally
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


    // Helper method to get the actual Photon-spawned player by name (Player1, Player2, etc.)
    private GameObject GetPhotonPlayer(int playerIndex)
    {
        string playerName = "Player" + (playerIndex + 1); // playerIndex is 0-based, so add 1
        return GameObject.Find(playerName);
    }

    /// <summary>
    /// Pays rent from the current player to the owner of the space they landed on.
    /// </summary>
    /// <param name="space">The BoardSpace that the current player landed on.</param>
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

        // Get the current player's bank account using Photon-spawned player
        GameObject currentPlayerObj = GetPhotonPlayer(currentPlayer);
        PlayerBankAccounts currentBank = currentPlayerObj != null ? currentPlayerObj.GetComponent<PlayerBankAccounts>() : null;

        // Get the owner's bank account using Photon-spawned player
        GameObject ownerPlayerObj = GetPhotonPlayer(ownerPlayer);
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
    /// Teleports existing players to the fight spawn locations.
    /// </summary>
    /// <param name="opponentIndex">The index of the opponent player.</param>
    public void Fight(int opponentIndex)
    {
        int currentPlayer = boardTurns.currPlayer;

        // Get the current player object (already spawned in scene)
        GameObject currentPlayerObj = GetPhotonPlayer(currentPlayer);
        
        // Get the opponent player object
        GameObject opponentPlayerObj = GetPhotonPlayer(opponentIndex);

        if (currentPlayerObj == null)
        {
            Debug.LogError($"Current player (index {currentPlayer}) not found in scene!");
            return;
        }

        if (opponentPlayerObj == null)
        {
            Debug.LogError($"Opponent player (index {opponentIndex}) not found in scene!");
            return;
        }

        // Get spawn positions based on player index
        Transform currentSpawn = currentPlayer == 0 ? player1FightSpawn : player2FightSpawn;
        Transform opponentSpawn = opponentIndex == 0 ? player1FightSpawn : player2FightSpawn;

        Vector3 currentSpawnPos = currentSpawn != null ? currentSpawn.position : Vector3.zero;
        Quaternion currentSpawnRot = currentSpawn != null ? currentSpawn.rotation : Quaternion.identity;
        
        Vector3 opponentSpawnPos = opponentSpawn != null ? opponentSpawn.position : Vector3.zero;
        Quaternion opponentSpawnRot = opponentSpawn != null ? opponentSpawn.rotation : Quaternion.identity;

        // Stop physics and teleport current player
        Rigidbody currentRb = currentPlayerObj.GetComponent<Rigidbody>();
        if (currentRb != null)
        {
            currentRb.linearVelocity = Vector3.zero;
            currentRb.angularVelocity = Vector3.zero;
        }
        currentPlayerObj.transform.position = currentSpawnPos;
        currentPlayerObj.transform.rotation = currentSpawnRot;

        // Stop physics and teleport opponent
        Rigidbody opponentRb = opponentPlayerObj.GetComponent<Rigidbody>();
        if (opponentRb != null)
        {
            opponentRb.linearVelocity = Vector3.zero;
            opponentRb.angularVelocity = Vector3.zero;
        }
        opponentPlayerObj.transform.position = opponentSpawnPos;
        opponentPlayerObj.transform.rotation = opponentSpawnRot;

        // Switch to FPS mode (all clients)
        gameMode.buttonSwitchGameMode();

        // Hide fight UI and show FPS canvas for all players via network sync
        if (photonView.IsMine)
        {
            if (fightUI != null)
                networkUI.HideUISynced(fightUI.name);
            if (FPSCanvas != null)
                networkUI.ShowUISynced(FPSCanvas.name);
        }

        Debug.Log($"Fight started - teleported Player {currentPlayer + 1} and Player {opponentIndex + 1} to fight locations");
    }
}
