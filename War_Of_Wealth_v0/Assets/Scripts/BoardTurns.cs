using UnityEngine;
using TMPro;

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

    [SerializeField] public GameObject endTurnUI;
    [SerializeField] public GameObject StartTurnUI;

    void Start()
    {
        boardVariables = GetComponent<BoardVariables>();
    }

    public void BoardTurnButton()
    {
        Debug.Log("BoardTurnButton pressed.");

        int diceRoll = Random.Range(2, 13);
        Debug.Log($"Player {currPlayer + 1} rolled a {diceRoll}");

        playerLocations[currPlayer] =
            (playerLocations[currPlayer] + diceRoll) % numTotalLocations;

        playerGameObjects[currPlayer].transform.position =
        boardVariables.Location(currPlayer, playerLocations[currPlayer]);
        
        BoardSpace landedSpace = boardSpaces[playerLocations[currPlayer]];
        landedSpace.Land(currPlayer);
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
        currentPlayerStats.text =
            $"Player {currPlayer + 1}'s turn\nMoney: ${currPlayer + 1}";
    }
}