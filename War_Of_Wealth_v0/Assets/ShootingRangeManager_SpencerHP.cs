using UnityEngine;
using System.Collections;

public class ShootingRangeManager_SpencerHP : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private int playerIndex = 0; // Which player enters the range

    [Header("Environment")]
    [SerializeField] private Transform shootingRangeSpawn;
    [SerializeField] private float backgroundDistance = 15f;
    [SerializeField] private Transform backWall;

    [Header("UI / Mode")]
    [SerializeField] private GameMode gameMode;
    [SerializeField] private GameObject AimChallengeUI;
    [SerializeField] private GameObject FPSCanvas;
    [SerializeField] private GameObject StatsUI;

    [Header("Target Spawning")]
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private Transform spawnStart;
    [SerializeField] private Transform spawnEnd;

    [SerializeField] private float spawnInterval = 1.5f;
    [SerializeField] private int maxTargets = 10;

    private bool spawning = false;
    private bool inRange = false;

    private void Start()
    {
        if (gameMode == null)
            gameMode = FindFirstObjectByType<GameMode>();
    }

    public void EnterShootingRange()
    {
        if (PlayerManager.Instance == null)
        {
            Debug.LogError("PlayerManager not found!");
            return;
        }

        if (playerIndex >= PlayerManager.Instance.GetPlayerCount())
        {
            Debug.LogError($"Invalid player index {playerIndex}. Only {PlayerManager.Instance.GetPlayerCount()} players available.");
            return;
        }

        // Teleport player to range
        if (shootingRangeSpawn != null)
        {
            PlayerManager.Instance.TeleportPlayer(playerIndex, shootingRangeSpawn.position, shootingRangeSpawn.rotation);
        }

        // Disable movement
        PlayerManager.Instance.DisablePlayerMovement(playerIndex);

        // Adjust environment
        if (backWall != null)
        {
            backWall.localPosition = new Vector3(0, 0, backgroundDistance);
        }

        // Switch to FPS mode
        if (gameMode != null)
        {
            gameMode.buttonSwitchGameMode();
        }

        // Update UI
        if (AimChallengeUI != null) AimChallengeUI.SetActive(false);
        if (StatsUI != null) StatsUI.SetActive(false);
        if (FPSCanvas != null) FPSCanvas.SetActive(true);

        inRange = true;

        // Start spawning targets
        StartCoroutine(SpawnTargets());
    }

    public void ExitShootingRange()
    {
        if (!inRange) return;

        // Re-enable movement
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.EnablePlayerMovement(playerIndex);
        }

        // Switch back to board mode
        if (gameMode != null)
        {
            gameMode.buttonSwitchGameMode();
        }

        // Update UI
        if (AimChallengeUI != null) AimChallengeUI.SetActive(true);
        if (StatsUI != null) StatsUI.SetActive(true);
        if (FPSCanvas != null) FPSCanvas.SetActive(false);

        // Stop spawning
        StopAllCoroutines();
        spawning = false;
        inRange = false;

        Debug.Log($"Player {playerIndex + 1} exited shooting range");
    }

    private IEnumerator SpawnTargets()
    {
        spawning = true;

        for (int i = 0; i < maxTargets; i++)
        {
            if (!spawning) break;

            // Spawn target at random position between start and end
            if (targetPrefab != null && spawnStart != null && spawnEnd != null)
            {
                Vector3 spawnPos = Vector3.Lerp(spawnStart.position, spawnEnd.position, Random.value);
                Instantiate(targetPrefab, spawnPos, Quaternion.identity);
            }

            yield return new WaitForSeconds(spawnInterval);
        }

        spawning = false;
    }

    
}