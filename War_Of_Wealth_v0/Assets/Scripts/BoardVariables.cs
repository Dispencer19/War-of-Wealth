using UnityEngine;

public class BoardVariables : MonoBehaviour
{
    public Transform[] tilePositions; // size 40

    public Vector3 Location(int currPlayer, int currLocation)
{
    Vector3 basePos = tilePositions[currLocation].position;

    // Offsets for 4 players
    Vector3[] offsets = new Vector3[]
    {
        new Vector3(-0.5f, 1f,  0.5f),
        new Vector3( 0.5f, 1f,  0.5f),
        new Vector3(-0.5f, 1f, -0.5f),
        new Vector3( 0.5f, 1f, -0.5f)
    };

    return basePos + offsets[currPlayer];
}
}