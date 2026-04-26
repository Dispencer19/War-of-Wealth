using UnityEngine;

public class ShootingRangeManager : MonoBehaviour
{
    public Transform player1;
    public Transform shootingRangeSpawn;
    public PlayerMovement playerMovement;

    public float backgroundDistance = 15f;
    public Transform backWall;

    public GameMode gameMode;
    public GameObject AimChallengeUI;

    public GameObject FPSCanvas;

    public GameObject StatsUI;

    public void EnterShootingRange()
    {
        // Teleport player
        player1.position = shootingRangeSpawn.position;
        player1.rotation = shootingRangeSpawn.rotation;

        // Disable movement
        playerMovement.canMove = false;

        // Adjust wall distance
        backWall.localPosition = new Vector3(0, 0, backgroundDistance);

        // Switch to FPS mode
        gameMode.buttonSwitchGameMode();
        AimChallengeUI.SetActive(false);
        StatsUI.SetActive(false);
        FPSCanvas.SetActive(true);
        


    }
}