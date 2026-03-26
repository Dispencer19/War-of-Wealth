using Unity.AppUI.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.UI;
using TMPro;

public class BoardTurns : MonoBehaviour
{
    // FOR TESTING AM AUTOMATICALLY DISABLING FPS IN START(). When testing FPS just disable BoardEmpty in hierarchy

    // * gonna move a lot of variables into BoardVariables later

    int currPlayer = 0; // rotates thru 0 to 3
    [SerializeField] public static int numTotalPlayers = 4;
    int[] playerLocations = new int[numTotalPlayers]; // they all start on square 0

    [SerializeField] GameObject[] playerGameObjects = new GameObject[numTotalPlayers];
    [SerializeField] int numTotalLocations = 40;
    [SerializeField] BoardSpace[] boardSpaces;
    [Tooltip("Total number of squares on the monopoly board")]
    [SerializeField] int numRowLocations = 10;
    [Tooltip("Number squares per row on the board")]

    [SerializeField] BoardVariables boardVariables;
    [SerializeField] Cooldown cooldown;
    [SerializeField] DisableFPS disableFPS;

    [SerializeField] TextMeshProUGUI currentPlayerStats;

    void Start()
    {
        boardVariables = GetComponent<BoardVariables>();
        cooldown = GetComponent<Cooldown>();
    }

    public void BoardTurnButton()
    {
        // when press button (ideally a dice image later)
        // can disable/enable buttons for each player depending on whose turn it is?

        //if (!cooldown.IsCoolingDown)
        //{
        //    cooldown.StartCooldown();

            Debug.Log("BoardTurnButton pressed. Next turn");

            //curr location      dice roll between 2 to 12 (inclusive)     overflow 40 squares
            
            //int diceRoll = Random.Range(2,12);
            int diceRoll = 3; // for testing purposes, just move 2 spaces every turn
            playerLocations[currPlayer] = 
                (playerLocations[currPlayer] + diceRoll) % numTotalLocations;

            playerGameObjects[currPlayer].transform.position =
                boardVariables.Location(currPlayer, playerLocations[currPlayer], numRowLocations);

            BoardSpace landedSpace = boardSpaces[playerLocations[currPlayer]];
            landedSpace.Land(currPlayer);
        //}
        //else
        //{
        //    Debug.Log("BoardTurns dice roll still on cooldown");
        //}
    }

    public int getCurrPlayer()
    {
        return currPlayer;
    }

    void Update()
    {
        currentPlayerStats.text = $"Player {currPlayer + 1}'s turn\nMoney: ${currPlayer + 1}"; // for testing purposes, just display player number and money. Will add properties owned later
    }


}
