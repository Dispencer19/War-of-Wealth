using UnityEngine;
using Photon.Pun;

public class FightTeleport : MonoBehaviour
{
    public Transform player1;
    public Transform player2;

    public Transform player1Spawn;
    public Transform player2Spawn;

    public PlayerHealth player1Health;
    public PlayerHealth player2Health;

    private PhotonView photonView;

    private void Awake() 
    {
        photonView = GetComponent<PhotonView>();
    }

    private void Start()     
    {
        Invoke("DelayedInitialize",5.0f);
        Invoke("CheckIfInitialized",10.0f);
    }

    public void DelayedInitialize()
    {
        player1 = GameObject.Find("Player1").transform;
        player2 = GameObject.Find("Player2").transform;
        player1Health = player1.GetComponent<PlayerHealth>();
        player2Health = player2.GetComponent<PlayerHealth>();   
    }

    public void CheckIfInitialized()
    {
        if(player1 == null)
            Debug.Log("player1 was not found during start of fightteleport");
        if(player2 == null)
            Debug.Log("player2 was not found during start of fightteleport");
    }

    [PunRPC]
    public void RPC_StartFight()
    {
        StartFightInternal();
    }

    public void StartFight()
    {
        // Call RPC on all clients
        photonView.RPC("RPC_StartFight", RpcTarget.All);
    }

    private void StartFightInternal()
    {
        if (player1 == null || player2 == null)
        {
            Debug.LogError("Players not found!");
            return;
        }

        // Stop physics movement
        Rigidbody rb1 = player1.GetComponent<Rigidbody>();
        Rigidbody rb2 = player2.GetComponent<Rigidbody>();

        if (rb1 != null) rb1.linearVelocity = Vector3.zero;
        if (rb2 != null) rb2.linearVelocity = Vector3.zero;

        // Teleport players
        player1.position = player1Spawn.position;
        player2.position = player2Spawn.position;

        // Reset health
        if (player1Health != null)
            player1Health.ResetHealth();
        if (player2Health != null)
            player2Health.ResetHealth();
    }
}
