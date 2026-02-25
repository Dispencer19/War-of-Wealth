using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.UI;

public class BoardTurns : MonoBehaviour
{
    // FOR TESTING AM AUTOMATICALLY DISABLING FPS IN START(). When testing FPS just disable BoardEmpty in hierarchy

    // * gonna move a lot of variables into BoardVariables later

    int currPlayer = 0; // rotates thru 0 to 3
    [SerializeField] public static int numTotalPlayers = 4;
    int[] playerLocations = new int[numTotalPlayers]; // they all start on square 0

    [SerializeField]
    GameObject[] playerGameObjects = new GameObject[numTotalPlayers];

    [SerializeField] int numTotalLocations = 40;
    [Tooltip("Total number of squares on the monopoly board")]
    [SerializeField] int numRowLocations = 10;
    [Tooltip("Number squares per row on the board")]

    [SerializeField] BoardVariables boardVariables;
    [SerializeField] Cooldown cooldown;
    [SerializeField] DisableFPS disableFPS;

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

            //                             curr location      dice roll between 1 to 6 (inclusive)     overflow 40 squares
            playerLocations[currPlayer] = (playerLocations[currPlayer] + Random.Range(1, 7)) % numTotalLocations;
            playerGameObjects[currPlayer].transform.position = boardVariables.Location(currPlayer, playerLocations[currPlayer], numRowLocations);

            Debug.Log("Moving player " + currPlayer + " to (" + boardVariables.stringLastLocation() + ")");

            currPlayer = (currPlayer + 1) % numTotalPlayers;
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
}
