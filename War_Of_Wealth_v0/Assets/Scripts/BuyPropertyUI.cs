using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class BuyPropertyUI : MonoBehaviour
{
    public TextMeshProUGUI promptText;
    public Image cardImageUI;
    public GameObject EndTurnUI;
    public NetworkUIManager networkUI;

    private BoardSpace currentSpace;
    private int currentPlayerIndex;
    private BoardTurns boardTurns;
    private PhotonView photonView;

    private void Awake()
    {
        boardTurns = FindObjectOfType<BoardTurns>();
        photonView = GetComponent<PhotonView>();
        if (networkUI == null)
            networkUI = FindAnyObjectByType<NetworkUIManager>();
    }

    // Called when a player lands on a property
    public void Show(BoardSpace space, int playerIndex)
    {
        currentSpace = space;
        currentPlayerIndex = playerIndex;

        promptText.text = $"Buy {space.spaceName} for ${space.price}?";

        if (space.cardimage != null)
        {
            cardImageUI.sprite = space.cardimage;
            cardImageUI.enabled = true;
        }
        else
        {
            cardImageUI.sprite = null;
            cardImageUI.enabled = false;
        }

        // Show UI locally
        gameObject.SetActive(true);
    }

    public void OnBuy()
    {
        
        //Debug.Log("Player bought " + currentSpace.spaceName);

        // Get the player's bank account
        //PlayerBankAccounts bank = boardTurns.playerGameObjects[currentPlayerIndex]
           //.GetComponent<PlayerBankAccounts>();
        Debug.Log("OnBuy() called. Player index = " + currentPlayerIndex);

    if (boardTurns == null)
        Debug.LogError("boardTurns is NULL!");

    if (boardTurns.playerGameObjects == null)
        Debug.LogError("playerGameObjects array is NULL!");

    if (currentPlayerIndex < 0 || currentPlayerIndex >= boardTurns.playerGameObjects.Length)
        Debug.LogError("currentPlayerIndex is OUT OF RANGE!");

    GameObject playerObj = boardTurns.playerGameObjects[currentPlayerIndex];
    Debug.Log("Player object = " + playerObj);

    PlayerBankAccounts bank = playerObj?.GetComponent<PlayerBankAccounts>();
    Debug.Log("Bank component = " + bank);

    if (bank == null)
    {
        Debug.LogError("PlayerBankAccounts component is MISSING on player " + currentPlayerIndex);
        return;
    }

        
        // Deduct money
        bank.RemoveMoney(currentSpace.price);

        // Assign property
        bank.AddProperty(currentSpace);

        // Mark the board space as owned
        currentSpace.isOwned = true;
        currentSpace.ownerPlayerIndex = currentPlayerIndex;

        // Close UI
        gameObject.SetActive(false);
        EndTurnUI.SetActive(true);
    }

    public void OnCancel()
    {
        Debug.Log("Player declined to buy.");

        gameObject.SetActive(false);
        EndTurnUI.SetActive(true);
    }
}
