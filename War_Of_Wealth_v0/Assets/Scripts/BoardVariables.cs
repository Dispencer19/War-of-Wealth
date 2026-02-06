using UnityEngine;

public class BoardVariables : MonoBehaviour
{
    [SerializeField] float distBetweenSquares = 0.1f;
    [Tooltip("How far a piece will travel on the board per square")]
    [SerializeField] float positionY = 0f;
    [Tooltip("How high are the pieces (or the top of the board) in the air ")]

    int row = -1;
    int positionX = 0;
    int positionZ = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 Location(int currPlayer, int currLocation, int numRowLocations)
    {
        // row 0-3 of the board. 0 is first row
        row = currLocation / numRowLocations;
        Vector3 myPosition = new Vector3(positionX, positionY, positionZ);

        // row 0 do nothing. else, rotate vector
        if(row == 1)
        {
            // account for the width of the board
            myPosition.x += distBetweenSquares * numRowLocations;
        }
        else if (row == 2)
        {
            // account for the length of the board
            myPosition.y += distBetweenSquares * numRowLocations;
        }
        else if (row == 3)
        {

        }

        
        return myPosition;
    }

}
