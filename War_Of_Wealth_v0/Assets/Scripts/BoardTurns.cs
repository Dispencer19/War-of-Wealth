using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class BoardTurns : MonoBehaviour
{
    [Header("References")]
    [SerializeField] BoardVariables boardVariables;

    [SerializeField] TextMeshProUGUI currentPlayerStats;
    [SerializeField] TextMeshProUGUI diceRollResultText;
    [SerializeField] TextMeshProUGUI currentplayerMoney;
    [SerializeField] Image[] propertyImages;

    public GameObject[] playerGameObjects;

    [SerializeField] GameObject StartTurnButton;
    [SerializeField] GameObject EndTurnUI;

    public DiceRoller diceRoller;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 12f;
    [SerializeField] float stepPause = 0.1f;

    [Header("Board")]
    public BoardSpace[] boardSpaces;
    public int numTotalLocations = 40;

    // Player state management
    public int[] playerLocations;
    public int currPlayer = 0;
    private bool isMoving = false;
    private List<int>[] ownedPropertyIndices; // Stores property indices for each player

    void Start()
    {
        if (boardVariables == null)
            boardVariables = GetComponent<BoardVariables>();

        // Initialize player locations array
        int playerCount = PlayerManager.Instance != null ? PlayerManager.Instance.GetPlayerCount() : 0;
        playerLocations = new int[playerCount];
        ownedPropertyIndices = new List<int>[playerCount];
        for (int i = 0; i < playerCount; i++)
        {
            ownedPropertyIndices[i] = new List<int>();
        }

        Debug.Log($"BoardTurns initialized for {playerCount} players");
    }

    public void BoardTurnButton()
    {
        if (isMoving) return;

        if (diceRoller == null)
        {
            Debug.LogError("DiceRoller not assigned!");
            return;
        }

        StartCoroutine(diceRoller.RollDice(result =>
        {
            diceRollResultText.text = $"Rolled a {result}!";

            StartCoroutine(MovePlayer(currPlayer, result));
        }));
    }

    IEnumerator MovePlayer(int playerIndex, int steps)
    {
        isMoving = true;

        if (PlayerManager.Instance == null)
        {
            Debug.LogError("PlayerManager not found!");
            isMoving = false;
            yield break;
        }

        if (playerLocations == null || playerIndex >= playerLocations.Length)
        {
            Debug.LogError($"Invalid player index {playerIndex} or playerLocations not initialized");
            isMoving = false;
            yield break;
        }

        GameObject player = playerGameObjects[playerIndex];
        if (player == null)
        {
            Debug.LogError($"Player {playerIndex} not found!");
            isMoving = false;
            yield break;
        }

        for (int i = 0; i < steps; i++)
        {
            // update board index
            playerLocations[playerIndex] =
                (playerLocations[playerIndex] + 1) % numTotalLocations;

            Vector3 targetPos = boardVariables.Location(
                playerIndex,
                playerLocations[playerIndex]
            );

            // smooth glide to tile
            while (Vector3.Distance(player.transform.position, targetPos) > 0.05f)
            {
                player.transform.position = Vector3.MoveTowards(
                    player.transform.position,
                    targetPos,
                    moveSpeed * Time.deltaTime
                );

                yield return null;
            }

            player.transform.position = targetPos;

            yield return new WaitForSeconds(stepPause);
        }

        // land on final tile
        if (boardSpaces != null && playerLocations[playerIndex] < boardSpaces.Length)
        {
            BoardSpace landedSpace = boardSpaces[playerLocations[playerIndex]];
            landedSpace.Land(playerIndex);
        }
        else
        {
            Debug.LogError($"Invalid board space at location {playerLocations[playerIndex]}");
        }

        isMoving = false;
    }

    public void EndTurn()
    {
        isMoving = false;

        if (PlayerManager.Instance == null)
        {
            Debug.LogError("PlayerManager not found!");
            return;
        }

        currPlayer = (currPlayer + 1) % PlayerManager.Instance.GetPlayerCount();
        Debug.Log($"Player {currPlayer + 1}'s turn");

        // Update UI for next player
        EndTurnUI.SetActive(false);
        StartTurnButton.SetActive(true);
    }

    void Update()
    {
        if (PlayerManager.Instance == null || PlayerManager.Instance.GetPlayerCount() == 0) return;

        PlayerBankAccounts bank = PlayerManager.Instance.GetPlayerBank(currPlayer);

        int money = bank != null ? bank.currentBalance : 0;

        // Display owned property images using stored indices
        if (propertyImages != null && ownedPropertyIndices != null && currPlayer < ownedPropertyIndices.Length)
        {
            List<int> playerProperties = ownedPropertyIndices[currPlayer];

            for (int i = 0; i < propertyImages.Length; i++)
            {
                if (i < playerProperties.Count && boardSpaces != null)
                {
                    int propertyIndex = playerProperties[i];
                    propertyImages[i].sprite = boardSpaces[propertyIndex].cardimage;
                    propertyImages[i].enabled = true;
                }
                else
                {
                    propertyImages[i].enabled = false;
                }
            }
        }

        if (currentPlayerStats != null)
        {
            currentPlayerStats.text = $"Player {currPlayer + 1}'s turn";
        }

        if (currentplayerMoney != null)
        {
            currentplayerMoney.text = $"${money}";
        }
    }

    public void AddPropertyToPlayer(int playerIndex, int propertyIndex)
    {
        if (ownedPropertyIndices != null && playerIndex < ownedPropertyIndices.Length)
        {
            ownedPropertyIndices[playerIndex].Add(propertyIndex);
        }
    }

    public void RemovePropertyFromPlayer(int playerIndex, int propertyIndex)
    {
        if (ownedPropertyIndices != null && playerIndex < ownedPropertyIndices.Length)
        {
            ownedPropertyIndices[playerIndex].Remove(propertyIndex);
        }
    }
}