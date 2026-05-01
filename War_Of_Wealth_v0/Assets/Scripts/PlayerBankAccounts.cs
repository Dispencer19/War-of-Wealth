using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerBankAccounts : MonoBehaviour
{
    [Header("Bank Settings")]
    public int startingBalance = 1500;

    [Header("Runtime Data")]
    public int currentBalance;
    public List<BoardSpace> ownedProperties = new List<BoardSpace>();

    public BoardTurns boardTurns;

    private PhotonView photonView;

    private void Awake()
    {
        currentBalance = startingBalance;
        boardTurns = FindAnyObjectByType<BoardTurns>();
        photonView = GetComponent<PhotonView>();
    }

    // Money management - synced across network
    [PunRPC]
    public void RPC_AddMoney(int amount)
    {
        currentBalance += amount;
    }

    public void AddMoney(int amount)
    {
        // Sync to all clients
        photonView.RPC("RPC_AddMoney", RpcTarget.All, amount);
    }

    [PunRPC]
    public void RPC_RemoveMoney(int amount)
    {
        currentBalance -= amount;
        if (currentBalance < 0)
            currentBalance = 0;
    }

    public void RemoveMoney(int amount)
    {
        // Sync to all clients
        photonView.RPC("RPC_RemoveMoney", RpcTarget.All, amount);
    }

    // Property management
    [PunRPC]
    public void RPC_AddProperty(int spaceIndex)
    {
        // Find the board space by index
        BoardTurns bt = FindAnyObjectByType<BoardTurns>();
        if (bt != null && bt.boardSpaces != null && spaceIndex < bt.boardSpaces.Length)
        {
            BoardSpace space = bt.boardSpaces[spaceIndex];
            if (!ownedProperties.Contains(space))
            {
                ownedProperties.Add(space);
                space.isOwned = true;
                space.ownerPlayerIndex = GetPlayerIndex();
            }
        }
    }

    public void AddProperty(BoardSpace space)
    {
        // Find the space index
        BoardTurns bt = FindAnyObjectByType<BoardTurns>();
        if (bt != null && bt.boardSpaces != null)
        {
            for (int i = 0; i < bt.boardSpaces.Length; i++)
            {
                if (bt.boardSpaces[i] == space)
                {
                    photonView.RPC("RPC_AddProperty", RpcTarget.All, i);
                    break;
                }
            }
        }
    }

    [PunRPC]
    public void RPC_RemoveProperty(int spaceIndex)
    {
        BoardTurns bt = FindAnyObjectByType<BoardTurns>();
        if (bt != null && bt.boardSpaces != null && spaceIndex < bt.boardSpaces.Length)
        {
            BoardSpace space = bt.boardSpaces[spaceIndex];
            if (ownedProperties.Contains(space))
            {
                ownedProperties.Remove(space);
                space.isOwned = false;
                space.ownerPlayerIndex = -1;
            }
        }
    }

    public void RemoveProperty(BoardSpace space)
    {
        BoardTurns bt = FindAnyObjectByType<BoardTurns>();
        if (bt != null && bt.boardSpaces != null)
        {
            for (int i = 0; i < bt.boardSpaces.Length; i++)
            {
                if (bt.boardSpaces[i] == space)
                {
                    photonView.RPC("RPC_RemoveProperty", RpcTarget.All, i);
                    break;
                }
            }
        }
    }

    private int GetPlayerIndex()
    {
        return boardTurns.currPlayer;
    }
}
