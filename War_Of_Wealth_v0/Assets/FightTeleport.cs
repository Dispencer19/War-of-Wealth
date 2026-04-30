using UnityEngine;
using Photon.Pun;

public class FightTeleport : MonoBehaviourPunCallbacks
{
    [Header("Fight Spawn Points")]
    public Transform player1Spawn;
    public Transform player2Spawn;

    [Header("Auto-Find Settings")]
    public string player1SpawnName = "Player1FightSpawn";
    public string player2SpawnName = "Player2FightSpawn";

    private bool isInitialized = false;

    void Start()
    {
        // Try to find spawn points
        if (player1Spawn == null)
        {
            GameObject spawnObj = GameObject.Find(player1SpawnName);
            if (spawnObj != null)
                player1Spawn = spawnObj.transform;
        }

        if (player2Spawn == null)
        {
            GameObject spawnObj = GameObject.Find(player2SpawnName);
            if (spawnObj != null)
                player2Spawn = spawnObj.transform;
        }

        // Delay initialization to ensure players are spawned
        Invoke("CheckInitialization", 1f);
    }

    void CheckInitialization()
    {
        // Verify spawn points exist
        if (player1Spawn == null)
        {
            Debug.LogError("FightTeleport: Player1FightSpawn not found! Make sure you have a GameObject named 'Player1FightSpawn' in the scene.");
        }

        if (player2Spawn == null)
        {
            Debug.LogError("FightTeleport: Player2FightSpawn not found! Make sure you have a GameObject named 'Player2FightSpawn' in the scene.");
        }

        isInitialized = (player1Spawn != null && player2Spawn != null);

        if (isInitialized)
        {
            Debug.Log("FightTeleport initialized successfully");
        }
    }

    /// <summary>
    /// Start a fight between two specific players (by their player index 0-3)
    /// </summary>
    public void StartFight(int player1Index, int player2Index)
    {
        if (!isInitialized)
        {
            Debug.LogError("FightTeleport: Cannot start fight - not initialized!");
            return;
        }

        // Find players by their index
        GameObject player1Obj = FindPlayerByIndex(player1Index);
        GameObject player2Obj = FindPlayerByIndex(player2Index);

        if (player1Obj == null)
        {
            Debug.LogError($"FightTeleport: Player {player1Index + 1} not found!");
            return;
        }

        if (player2Obj == null)
        {
            Debug.LogError($"FightTeleport: Player {player2Index + 1} not found!");
            return;
        }

        // Only the Master Client should trigger the fight
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("RPC_TeleportPlayers", RpcTarget.AllBuffered, player1Index, player2Index);
        }
    }

    [PunRPC]
    void RPC_TeleportPlayers(int player1Index, int player2Index)
    {
        GameObject player1Obj = FindPlayerByIndex(player1Index);
        GameObject player2Obj = FindPlayerByIndex(player2Index);

        if (player1Obj != null && player2Obj != null)
        {
            TeleportPlayer(player1Obj, player1Spawn);
            TeleportPlayer(player2Obj, player2Spawn);

            Debug.Log($"Fight started: Player {player1Index + 1} vs Player {player2Index + 1}");
        }
    }

    void TeleportPlayer(GameObject playerObj, Transform spawnPoint)
    {
        // Stop physics movement
        Rigidbody rb = playerObj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Teleport player
        playerObj.transform.position = spawnPoint.position;
        playerObj.transform.rotation = spawnPoint.rotation;

        // Reset health if exists
        PlayerHealth health = playerObj.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.ResetHealth();
        }
    }

    /// <summary>
    /// Find a player GameObject by their player index (0-3)
    /// Works with both networked and local players
    /// </summary>
    GameObject FindPlayerByIndex(int playerIndex)
    {
        // Try multiple naming conventions
        string[] possibleNames = new string[]
        {
            "Player" + (playerIndex + 1),           // "Player1", "Player2", etc.
            "Player" + playerIndex,                  // "Player0", "Player1", etc.
            "Player " + (playerIndex + 1),           // "Player 1", "Player 2", etc.
        };

        foreach (string name in possibleNames)
        {
            GameObject found = GameObject.Find(name);
            if (found != null)
            {
                Debug.Log($"Found player with name: {name}");
                return found;
            }
        }

        // If not found by name, try finding by PhotonView owner
        if (PhotonNetwork.IsConnected)
        {
            PhotonView[] allViews = FindObjectsOfType<PhotonView>();
            foreach (PhotonView pv in allViews)
            {
                if (pv.Owner != null && pv.Owner.ActorNumber - 1 == playerIndex)
                {
                    // Check if this is a player object (has PlayerHealth or similar component)
                    if (pv.gameObject.GetComponent<PlayerHealth>() != null ||
                        pv.gameObject.name.Contains("Player"))
                    {
                        Debug.Log($"Found player by PhotonView: ActorNumber {pv.Owner.ActorNumber}");
                        return pv.gameObject;
                    }
                }
            }
        }

        Debug.LogWarning($"Could not find player with index {playerIndex}");
        return null;
    }

    /// <summary>
    /// End the fight and return players to the board
    /// </summary>
    public void EndFight(int player1Index, int player2Index, Vector3 player1ReturnPos, Vector3 player2ReturnPos)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("RPC_ReturnPlayers", RpcTarget.AllBuffered, 
                player1Index, player2Index, player1ReturnPos, player2ReturnPos);
        }
    }

    [PunRPC]
    void RPC_ReturnPlayers(int player1Index, int player2Index, Vector3 pos1, Vector3 pos2)
    {
        GameObject player1Obj = FindPlayerByIndex(player1Index);
        GameObject player2Obj = FindPlayerByIndex(player2Index);

        if (player1Obj != null)
            player1Obj.transform.position = pos1;

        if (player2Obj != null)
            player2Obj.transform.position = pos2;

        Debug.Log("Players returned to board");
    }

    // Optional: Debug method to test if everything is set up correctly
    [ContextMenu("Test Find All Players")]
    void TestFindPlayers()
    {
        Debug.Log("=== Testing Player Detection ===");
        for (int i = 0; i < 4; i++)
        {
            GameObject player = FindPlayerByIndex(i);
            if (player != null)
            {
                Debug.Log($"✓ Player {i + 1} found: {player.name}");
            }
            else
            {
                Debug.Log($"✗ Player {i + 1} NOT found");
            }
        }
        Debug.Log("=== Test Complete ===");
    }
}