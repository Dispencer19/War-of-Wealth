using UnityEngine;
using UnityEngine.UI;

public class BoardTurns : MonoBehaviour
{
    // * gonna move a lot of variables into BoardVariables later

    int currPlayer = 0; // rotates thru 0 to 3
    int diceroll = -1;
    [SerializeField] static int numTotalPlayers = 4;
    int[] playerLocations = new int[numTotalPlayers]; // they all start on square 0

    [SerializeField]
    GameObject[] playerGameObjects = new GameObject[numTotalPlayers];

    [SerializeField] int numTotalLocations = 40;
    [Tooltip("Total number of squares on the monopoly board")]
    [SerializeField] int numRowLocations = 40;
    [Tooltip("Number squares per row on the board")]

    [SerializeField] BoardVariables boardVariables;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        boardVariables = GetComponent<BoardVariables>();
    }

    // Update is called once per frame
    void Update()
    {
        // for now just press D to go to next dice roll
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("The 'D' key was pressed. Next turn");

            //                                                           dice roll between 1 to 6 (inclusive)
            //                                                                               idk if it should be modded by this yet i didnt think a lot
            playerLocations[currPlayer] = (playerLocations[currPlayer] + Random.Range(1, 7)) % numRowLocations;
            //                                                 will make a function that returns a location (a transform) based on which player & which square
            playerGameObjects[currPlayer].transform.position = boardVariables.Location(currPlayer, playerLocations[currPlayer]);


            currPlayer = (currPlayer + 1) % numTotalPlayers;
        }
    }

}
