using UnityEngine;
using Photon.Pun;

public class ShootingRangeManager : MonoBehaviour
{
    [Header("References")]
    public Transform player1;
    public Transform shootingRangeSpawn;
    public PlayerMovement_SpencerHP playerMovement;

    [Header("Settings")]
    public float backgroundDistance = 15f;
    public Transform backWall;

    [Header("Game Mode")]
    public GameMode gameMode;
    public NetworkUIManager networkUI;

    [Header("UI Panels")]
    public GameObject AimChallengeUI;
    public GameObject FPSCanvas;
    public GameObject StatsUI;

    private PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        if (networkUI == null)
            networkUI = FindAnyObjectByType<NetworkUIManager>();
    }

    [PunRPC]
    public void RPC_EnterShootingRange()
    {
        EnterShootingRangeInternal();
    }

    public void EnterShootingRange()
    {
        // Call RPC on all clients
        photonView.RPC("RPC_EnterShootingRange", RpcTarget.All);
    }

    private void EnterShootingRangeInternal()
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

        // Sync UI changes
        if (photonView.IsMine)
        {
            networkUI.HideUISynced(AimChallengeUI.name);
            networkUI.HideUISynced(StatsUI.name);
            networkUI.ShowUISynced(FPSCanvas.name);
        }
    }
}