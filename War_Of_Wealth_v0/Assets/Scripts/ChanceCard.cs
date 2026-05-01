using UnityEngine;
using Photon.Pun;

public class ChanceCard : MonoBehaviour
{
    public ChanceOption[] cards;   // Assign in Inspector
    public BoardTurns boardTurns;  // Drag your BoardTurns object here

    public BoardVariables boardVariables; // Drag your BoardVariables object here

    public GameObject ChanceCardUI; // UI panel to show card details (optional)
    public GameObject endTurnUI; // UI panel for end turn options (optional)

    public NetworkUIManager networkUI;

    private PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        if (networkUI == null)
            networkUI = FindAnyObjectByType<NetworkUIManager>();
    }

    public void DrawCard()
    {
        if (cards.Length == 0)
        {
            Debug.LogError("No chance cards assigned!");
            return;
        }

        // Pick a random card
        int index = Random.Range(0, cards.Length);
        ChanceOption card = cards[index];

        //Debug.Log($"Drew Chance Card: {card.cardName} - {card.description}");

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
            Debug.Log($"Player {player + 1} money change: {card.moneyChange}");
            // TODO: integrate with your money system
        }

        // After drawing, show End Turn UI for all players
        if (photonView.IsMine)
        {
            networkUI.HideUISynced(ChanceCardUI.name);
            networkUI.ShowUISynced(endTurnUI.name);
        }
    }
}