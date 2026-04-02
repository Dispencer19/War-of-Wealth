using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;

    public float minX;
    public float maxX;
    public float y;
    public float minZ;
    public float maxZ;

    private void Start()
    {
        Vector3 randomPosition = new Vector3(Random.Range(minX, maxX), y, Random.Range(minZ, maxZ));
        PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity);
    }
}
