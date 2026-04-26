using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class BoardTurns : MonoBehaviour
{
    public int currPlayer = 0;
    public static int numTotalPlayers = 4;

    public int[] playerLocations = new int[numTotalPlayers];

    [SerializeField] public GameObject[] playerGameObjects = new GameObject[numTotalPlayers];
    [SerializeField] int numTotalLocations = 40;
    [SerializeField] BoardSpace[] boardSpaces;

    [SerializeField] int numRowLocations = 10;

    [SerializeField] BoardVariables boardVariables;

    [SerializeField] TextMeshProUGUI currentPlayerStats;
    [SerializeField] TextMeshProUGUI diceRollResultText;

    [SerializeField] TextMeshProUGUI currentplayerMoney;

    [SerializeField] public GameObject endTurnUI;
    [SerializeField] public GameObject StartTurnUI;

    public DiceRoller diceRoller;

    void Start()
    {
        boardVariables = GetComponent<BoardVariables>();

    }

    public void BoardTurnButton()
    {
        //Debug.Log("BoardTurnButton pressed.");

        // Disable the button here if needed

        StartCoroutine(diceRoller.RollDice(result =>
        {
            int diceRoll = result;

            //Debug.Log($"Player {currPlayer + 1} rolled a {diceRoll}");

            diceRollResultText.text = $"Rolled a {diceRoll}!";

            // Update location
            playerLocations[currPlayer] =
                (playerLocations[currPlayer] + diceRoll) % numTotalLocations;

            // Move the player visually
            playerGameObjects[currPlayer].transform.position =
                boardVariables.Location(currPlayer, playerLocations[currPlayer]);

            // Trigger board space logic
            BoardSpace landedSpace = boardSpaces[playerLocations[currPlayer]];
            landedSpace.Land(currPlayer);



        }));
    }


    public void EndTurn()
    {
        Debug.Log($"Ending Player {currPlayer + 1}'s turn");

        endTurnUI.SetActive(false);

        currPlayer = (currPlayer + 1) % numTotalPlayers;

        Debug.Log($"Now Player {currPlayer + 1}'s turn");

        StartTurnUI.SetActive(true);
    }

    void Update()
    {
        PlayerBankAccounts bank = playerGameObjects[currPlayer].GetComponent<PlayerBankAccounts>();
        int money = bank != null ? bank.currentBalance : 0;

        List<string> ownedProperties = new List<string>();
        for (int i = 0; i < boardSpaces.Length; i++)
        {
            if (boardSpaces[i].ownerPlayerIndex == currPlayer)
            {
                ownedProperties.Add(boardSpaces[i].spaceName);
            }
        }

        string propertiesText = ownedProperties.Count > 0 ? "\nProperties: " + string.Join(", ", ownedProperties) : "\nNo properties owned";

        currentPlayerStats.text =
            $"Player {currPlayer + 1}'s turn\n{propertiesText}";
        currentplayerMoney.text = $"${money}";
    }

}