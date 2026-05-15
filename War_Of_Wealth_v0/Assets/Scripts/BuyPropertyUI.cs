using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyPropertyUI : MonoBehaviour
{
    public TextMeshProUGUI promptText;
    public Image cardImageUI;
    public GameObject EndTurnUI;

    private BoardSpace currentSpace;
    private int currentPlayerIndex;
    private DisableFPS disableFPS;

    private void Awake()
    {
        disableFPS = FindAnyObjectByType<DisableFPS>();
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

        gameObject.SetActive(true);
    }

    public void OnBuy()
    {
        //Debug.Log("Player bought " + currentSpace.spaceName);

        // Get the player's bank account
        PlayerBankAccounts bank = disableFPS.playerObjects[currentPlayerIndex]
            .GetComponent<PlayerBankAccounts>();

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
