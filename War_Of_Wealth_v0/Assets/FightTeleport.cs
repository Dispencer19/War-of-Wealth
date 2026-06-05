using UnityEngine;

public class FightTeleport : MonoBehaviour
{
    [Header("Fight Settings")]
    [SerializeField] private int player1Index = 0;
    [SerializeField] private int player2Index = 1;

    [Header("Spawn Points")]
    [SerializeField] private Transform player1Spawn;
    [SerializeField] private Transform player2Spawn;

    [Header("References")]
    [SerializeField] private DisableFPS disableFPS;

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

        if (disableFPS == null)
            disableFPS = FindFirstObjectByType<DisableFPS>();
    }

    // Set which two players fight. Called before StartFight() so a contended
    // property battle is current-player vs. the property owner.
    public void SetFighters(int attackerIndex, int defenderIndex)
    {
        player1Index = attackerIndex;
        player2Index = defenderIndex;
    }

    public void StartFight()
    {
        if (PlayerManager.Instance == null)
        {
            Debug.LogError("PlayerManager not found! Cannot start fight.");
            return;
        }

        if (player1Index >= PlayerManager.Instance.GetPlayerCount() ||
            player2Index >= PlayerManager.Instance.GetPlayerCount())
        {
            Debug.LogError($"Invalid player indices for fight: {player1Index}, {player2Index}. Only {PlayerManager.Instance.GetPlayerCount()} players available.");
            return;
        }

        if (player1Spawn == null || player2Spawn == null)
        {
            Debug.LogError("Fight spawn points not set!");
            return;
        }

        // 1. Enter split-screen FPS mode. SwitchMode also activates the FPS
        //    player objects and FPS UI via DisableFPS.EnableFPSObjects().
        if (CameraManager.Instance != null)
        {
            CameraManager.Instance.SwitchMode(CameraMode.FPS);
        }
        else if (disableFPS != null)
        {
            // Fallback if there's no CameraManager in the scene.
            disableFPS.EnableFPSObjects();
        }

        // 2. Place the fighters at their spawn points.
        PlayerManager.Instance.TeleportPlayer(player1Index, player1Spawn.position, player1Spawn.rotation);
        PlayerManager.Instance.TeleportPlayer(player2Index, player2Spawn.position, player2Spawn.rotation);

        // 3. Enable movement so both players can actually fight.
        PlayerManager.Instance.EnablePlayerMovement(player1Index);
        PlayerManager.Instance.EnablePlayerMovement(player2Index);

        // 4. Fresh health for the duel.
        PlayerManager.Instance.ResetPlayerHealth(player1Index);
        PlayerManager.Instance.ResetPlayerHealth(player2Index);

        Debug.Log($"Fight started between Player {player1Index + 1} and Player {player2Index + 1}!");
    }

    public void EndFight()
    {
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.DisablePlayerMovement(player1Index);
            PlayerManager.Instance.DisablePlayerMovement(player2Index);
        }

        // Return to the shared overhead board view.
        if (CameraManager.Instance != null)
        {
            CameraManager.Instance.SwitchMode(CameraMode.Board);
        }
        else if (disableFPS != null)
        {
            disableFPS.DisableFPSObjects();
        }

        Debug.Log("Fight ended - returned to board view");
    }
}