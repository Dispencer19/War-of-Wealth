using System.Collections.Generic;
using UnityEngine;

public class PlayerBankAccounts : MonoBehaviour
{
    [Header("Bank Settings")]
    public int startingBalance = 1500;

    [Header("Runtime Data")]
    public int currentBalance;
    public List<BoardSpace> ownedProperties = new List<BoardSpace>();

    public BoardTurns boardTurns;

    private void Awake()
    {
        currentBalance = startingBalance;
        boardTurns = FindAnyObjectByType<BoardTurns>();
    }

    // Money management
    public void AddMoney(int amount)
    {
        currentBalance += amount;
    }

    public void RemoveMoney(int amount)
    {
        currentBalance -= amount;
        if (currentBalance < 0)
            currentBalance = 0;
    }

    // Property management
    public void AddProperty(BoardSpace space)
    {
        if (!ownedProperties.Contains(space))
        {
            ownedProperties.Add(space);
            space.isOwned = true;
            space.ownerPlayerIndex = GetPlayerIndex();
        }
    }

    public void RemoveProperty(BoardSpace space)
    {
        if (ownedProperties.Contains(space))
        {
            ownedProperties.Remove(space);
            space.isOwned = false;
            space.ownerPlayerIndex = -1;
        }
    }

    private int GetPlayerIndex()
    {
        // Optional: if your player GameObjects are named "Player1", "Player2", etc.
        // or you can set this from BoardTurns when you spawn/assign players.
        return boardTurns.currPlayer; // Assuming this script is on the player GameObject and BoardTurns tracks current player index
    }
}
