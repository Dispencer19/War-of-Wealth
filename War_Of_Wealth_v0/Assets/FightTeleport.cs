using UnityEngine;

public class FightTeleport : MonoBehaviour
{
    [Header("Fight Settings")]
    [SerializeField] private int player1Index = 0;
    [SerializeField] private int player2Index = 1;

    [Header("Spawn Points")]
    [SerializeField] private Transform player1Spawn;
    [SerializeField] private Transform player2Spawn;

    private void Start()
    {
        // Find spawn points by name if not assigned
        if (player1Spawn == null)
        {
            GameObject spawn1 = GameObject.Find("Player1FightSpawn");
            if (spawn1 != null) player1Spawn = spawn1.transform;
        }

        if (player2Spawn == null)
        {
            GameObject spawn2 = GameObject.Find("Player2FightSpawn");
            if (spawn2 != null) player2Spawn = spawn2.transform;
        }
    }

    public void StartFight()
    {
        // Check if PlayerManager exists
        if (PlayerManager.Instance == null)
        {
            Debug.LogError("PlayerManager not found! Cannot start fight.");
            return;
        }

        // Validate player indices
        if (player1Index >= PlayerManager.Instance.GetPlayerCount() ||
            player2Index >= PlayerManager.Instance.GetPlayerCount())
        {
            Debug.LogError($"Invalid player indices for fight: {player1Index}, {player2Index}. Only {PlayerManager.Instance.GetPlayerCount()} players available.");
            return;
        }

        // Check spawn points
        if (player1Spawn == null || player2Spawn == null)
        {
            Debug.LogError("Fight spawn points not set!");
            return;
        }

        // Teleport players using PlayerManager
        PlayerManager.Instance.TeleportPlayer(player1Index, player1Spawn.position, player1Spawn.rotation);
        PlayerManager.Instance.TeleportPlayer(player2Index, player2Spawn.position, player2Spawn.rotation);

        // Disable player movement during fight
        PlayerManager.Instance.DisablePlayerMovement(player1Index);
        PlayerManager.Instance.DisablePlayerMovement(player2Index);

        // Reset health
        PlayerManager.Instance.ResetPlayerHealth(player1Index);
        PlayerManager.Instance.ResetPlayerHealth(player2Index);

        Debug.Log($"Fight started between Player {player1Index + 1} and Player {player2Index + 1}!");
    }

    public void EndFight()
    {
        // Re-enable player movement
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.EnablePlayerMovement(player1Index);
            PlayerManager.Instance.EnablePlayerMovement(player2Index);
        }

        Debug.Log("Fight ended - player movement restored");
    }
}