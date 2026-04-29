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

    public DiceRoller diceRoller;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] float stepPause = 0.1f;

    [Header("Players")]
    public GameObject[] playerGameObjects;
    public BoardSpace[] boardSpaces;
    public int[] playerLocations;

    public int currPlayer = 0;
    public int numTotalLocations = 40;

    bool isMoving = false;

    void Start()
    {
        if (boardVariables == null)
            boardVariables = GetComponent<BoardVariables>();
    }

    public void BoardTurnButton()
    {
        if (isMoving) return;

        StartCoroutine(diceRoller.RollDice(result =>
        {
            diceRollResultText.text = $"Rolled a {result}!";

            StartCoroutine(MovePlayer(currPlayer, result));
        }));
    }

    IEnumerator MovePlayer(int playerIndex, int steps)
    {
        isMoving = true;

        GameObject player = playerGameObjects[playerIndex];

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
        BoardSpace landedSpace = boardSpaces[playerLocations[playerIndex]];
        landedSpace.Land(playerIndex);

        isMoving = false;
    }

    public void EndTurn()
    {
        if (isMoving) return;

        currPlayer = (currPlayer + 1) % playerGameObjects.Length;
        Debug.Log($"Player {currPlayer + 1}'s turn");
    }

    void Update()
    {
        if (playerGameObjects.Length == 0) return;

        PlayerBankAccounts bank =
            playerGameObjects[currPlayer].GetComponent<PlayerBankAccounts>();

        int money = bank != null ? bank.currentBalance : 0;

        List<string> ownedProperties = new List<string>();

        for (int i = 0; i < boardSpaces.Length; i++)
        {
            if (boardSpaces[i].ownerPlayerIndex == currPlayer)
            {
                ownedProperties.Add(boardSpaces[i].spaceName);
            }
        }

        string propertiesText =
            ownedProperties.Count > 0
            ? "\nProperties: " + string.Join(", ", ownedProperties)
            : "\nNo properties owned";

        currentPlayerStats.text =
            $"Player {currPlayer + 1}'s turn{propertiesText}";

        currentplayerMoney.text = $"${money}";
    }
}