using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

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
        Vector3 randomPosition = new Vector3(
            Random.Range(minX, maxX),
            y,
            Random.Range(minZ, maxZ)
        );

        // Instantiate the player
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity);

        // Assign name based on join order
        int playerNumber = PhotonNetwork.CurrentRoom.PlayerCount;
        player.name = "Player" + playerNumber;
    }
}
