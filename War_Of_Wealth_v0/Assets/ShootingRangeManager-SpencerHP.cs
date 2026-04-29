using UnityEngine;
using System.Collections;

public class ShootingRangeManager : MonoBehaviour
{
    [Header("Player")]
    public Transform player1;
    public Transform shootingRangeSpawn;
    public PlayerMovement playerMovement;

    [Header("Environment")]
    public float backgroundDistance = 15f;
    public Transform backWall;

    [Header("UI / Mode")]
    public GameMode gameMode;
    public GameObject AimChallengeUI;
    public GameObject FPSCanvas;
    public GameObject StatsUI;

    [Header("Target Spawning")]
    public GameObject targetPrefab;
    public Transform spawnStart;
    public Transform spawnEnd;

    public float spawnInterval = 1.5f;
    public int maxTargets = 10;

    private bool spawning = false;

    public void EnterShootingRange()
    {
        // teleport player
        player1.position = shootingRangeSpawn.position;
        player1.rotation = shootingRangeSpawn.rotation;

        // disable movement
        playerMovement.canMove = false;

        // adjust environment
        backWall.localPosition = new Vector3(0, 0, backgroundDistance);

        // switch modes/UI
        gameMode.buttonSwitchGameMode();
        AimChallengeUI.SetActive(false);
        StatsUI.SetActive(false);
        FPSCanvas.SetActive(true);

        // start spawning targets
        StartCoroutine(SpawnTargets());
    }

    IEnumerator SpawnTargets()
    {
        spawning = true;

        int spawned = 0;

        while (spawning && spawned < maxTargets)
        {
            Vector3 spawnPos = Vector3.Lerp(
                spawnStart.position,
                spawnEnd.position,
                Random.Range(0f, 1f)
            );

            GameObject target = Instantiate(targetPrefab, spawnPos, Quaternion.identity);

            // optional: face player
            target.transform.LookAt(player1);

            spawned++;

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}