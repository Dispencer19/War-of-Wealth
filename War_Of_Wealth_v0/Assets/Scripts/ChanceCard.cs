using UnityEngine;
using TMPro;

public class ChanceCard : MonoBehaviour
{
    public ChanceOption[] cards;   // Assign in Inspector
    public BoardTurns boardTurns;  // Drag your BoardTurns object here

    public BoardVariables boardVariables; // Drag your BoardVariables object here

    public GameObject ChanceCardUI; // UI panel to show card details (optional)
    public GameObject endTurnUI; // UI panel for end turn options (optional)

    public GameObject OKbutton;

    public GameObject Draw;

    public TextMeshProUGUI cardTitleText; // UI text for card title
    public TextMeshProUGUI cardDescriptionText; // UI text for card description


    // Resets the panel to its pre-draw state. Call this each time the card UI opens.
    public void ResetCardUI()
    {
        if (Draw != null) Draw.SetActive(true);          // show the Draw button
        if (OKbutton != null) OKbutton.SetActive(false); // hide OK until a card is drawn
        if (cardTitleText != null) cardTitleText.text = "";
        if (cardDescriptionText != null) cardDescriptionText.text = "";
    }

    private void OnEnable()
    {
        // Backup reset in case the ChanceCard component lives on the panel itself.
        ResetCardUI();
    }


    public void DrawCard()
    {
        OKbutton.SetActive(true); // Show OK button to proceed after reading card
        Draw.SetActive(false); // Hide draw button after drawing a card
        if (cards.Length == 0)
        {
            Debug.LogError("No chance cards assigned!");
            return;
        }

        // Pick a random card
        int index = Random.Range(0, cards.Length);
        ChanceOption card = cards[index];

        //Debug.Log($"Drew Chance Card: {card.cardName} - {card.description}");

        cardTitleText.text = card.cardName;
        cardDescriptionText.text = card.description;

        int player = boardTurns.currPlayer;

        // Apply movement
        if (card.moveTo > 0)
        {
            boardTurns.playerLocations[player] = card.moveTo;
            boardTurns.playerGameObjects[player].transform.position =
                boardVariables.Location(player, card.moveTo);
        }

        // Apply money change (if you add money system later)
        if (card.moneyChange != 0)
        {
            PlayerBankAccounts bank = boardTurns.playerGameObjects[player]
                .GetComponent<PlayerBankAccounts>();

            if (bank != null)
            {
                if (card.moneyChange > 0)
                    bank.AddMoney(card.moneyChange);
                else
                    bank.RemoveMoney(-card.moneyChange);
            }    
            
        }

        
        
    }

    public void Proceed()
    {
        OKbutton.SetActive(false); // Hide OK button
        ChanceCardUI.SetActive(false); // Hide card UI
        endTurnUI.SetActive(true);
    }

    
}