using UnityEngine;
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

    void Start()
    {
        if (boardVariables == null)
            boardVariables = GetComponent<BoardVariables>();

        // Initialize player locations array
        int playerCount = PlayerManager.Instance != null ? PlayerManager.Instance.GetPlayerCount() : 0;
        playerLocations = new int[playerCount];

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

        List<string> ownedProperties = new List<string>();

        if (boardSpaces != null)
        {
            for (int i = 0; i < boardSpaces.Length; i++)
            {
                if (boardSpaces[i] != null && boardSpaces[i].ownerPlayerIndex == currPlayer)
                {
                    ownedProperties.Add(boardSpaces[i].spaceName);
                }
            }
        }

        string propertiesText =
            ownedProperties.Count > 0
            ? "\nProperties: " + string.Join(", ", ownedProperties)
            : "\nNo properties owned";

        if (currentPlayerStats != null)
        {
            currentPlayerStats.text = $"Player {currPlayer + 1}'s turn{propertiesText}";
        }

        if (currentplayerMoney != null)
        {
            currentplayerMoney.text = $"${money}";
        }
    }
}