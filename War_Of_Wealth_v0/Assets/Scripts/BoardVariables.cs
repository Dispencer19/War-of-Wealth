using System;
using Unity.VisualScripting;
using UnityEngine;

public class BoardVariables : MonoBehaviour
{
    // THE BOARD NEEDS TO BE ORIENTED IN THE SCENE SO THAT IT'S:
    //  - FLAT ON THE GROUND
    //  - X IS HORIZONTAL
    //  - Z IS VERTICAL
    //  - START IS IN THE BOTTOM LEFT CORNER

    [SerializeField] float distBetweenSquares = 6f;
    [Tooltip("How far a piece will travel on the board per square")]
    [SerializeField] float distBetweenPlayers = 1.8f;

    [SerializeField] float offsetPositionX = 0.0f;
    [Tooltip("If the corner of the board is not at (0,0,0), add offset for where pieces should start on the board")]
    [SerializeField] float offsetPositionY = 0.0f;
    [Tooltip("If the corner of the board is not at (0,0,0), add offset for where pieces should start on the board")]
    [SerializeField] float offsetPositionZ = 0.0f;
    [Tooltip("If the corner of the board is not at (0,0,0), add offset for where pieces should start on the board")]

    float[] offsetsPlayer = new float[4];
    int row = -1;
    float positionY = 0.0f;
    float positionX = 0.0f;
    float positionZ = 0.0f
        ;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // this script is on boardempty. default position is on it
        offsetPositionX = transform.position.x;
        offsetPositionY = transform.position.y;
        offsetPositionZ = transform.position.z;
    }

    public Vector3 Location(int currPlayer, int currLocation, int numRowLocations)
    {
        // X is horizontal movement, Y is constant, Z is vertical
        positionX = offsetPositionX;
        positionY = offsetPositionY;
        positionZ = offsetPositionZ;

        if (currPlayer == 0)
        {
            positionX -= distBetweenPlayers;
            positionZ += distBetweenPlayers;
        }
        else if (currPlayer == 1)
        {
            positionX += distBetweenPlayers;
            positionZ += distBetweenPlayers;
        }
        else if (currPlayer == 2)
        {
            positionX -= distBetweenPlayers;
            positionZ -= distBetweenPlayers;
        }
        else
        {
            positionX += distBetweenPlayers;
            positionZ -= distBetweenPlayers;
        }

        // row 0-3 of the board. 0 is first row
        row = currLocation / numRowLocations;

        if (row == 0)
        {
            positionX -= distBetweenSquares * currLocation;
        }
        if (row == 1)
        {
            // account for the width of the board
            positionX -= distBetweenSquares * numRowLocations;

            //                         NRL is 10 but we would be on index 9 at the end of the row so +1
            positionZ += distBetweenSquares * (currLocation - numRowLocations + 1);
        }
        else if (row == 2)
        {
            // account for the width of the board
            positionX -= distBetweenSquares * numRowLocations;
            // account for the height of the board
            positionZ += distBetweenSquares * numRowLocations;

            positionX += distBetweenSquares * (currLocation - numRowLocations * 2 + 2);
        }
        else // (row == 3)
        {
            positionZ += distBetweenSquares * (numRowLocations - 1);

            positionZ -= distBetweenSquares * (currLocation - numRowLocations * 3 + 3);
        }

        Vector3 myPosition = new Vector3(positionX, positionY, positionZ);

        return myPosition;
    }

    public float lastLocationX()
    {
        return positionX;
    }

    public float lastLocationY()
    {
        return positionY;
    }

    public float lastLocationZ()
    {
        return positionZ;
    }

    public String stringLastLocation()
    {
        return positionX.ToString() + ", " + positionY.ToString() + ", " + positionZ.ToString();
    }

}