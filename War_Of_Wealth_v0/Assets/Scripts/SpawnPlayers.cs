using UnityEngine;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;

    public float minX;
    public float maxX;
    public float y;
    public float minZ;
    public float maxZ;

    private void Awake()
    {
        Vector3 randomPosition = new Vector3(
            Random.Range(minX, maxX),
            y,
            Random.Range(minZ, maxZ)
        );

    }
}
