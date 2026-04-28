using UnityEngine;

public class FightTeleport : MonoBehaviour
{
    public Transform player1;
    public Transform player2;

    public Transform player1Spawn;
    public Transform player2Spawn;

    public PlayerHealth player1Health;
    public PlayerHealth player2Health;

    private void Awake() 
        
    {
        player1 = GameObject.Find("Player1").transform;
        player2 = GameObject.Find("Player2").transform;
        player1Health = player1.GetComponent<PlayerHealth>();
        player2Health = player2.GetComponent<PlayerHealth>();   
    }
    public void StartFight()
    {
        // Stop physics movement
        Rigidbody rb1 = player1.GetComponent<Rigidbody>();
        Rigidbody rb2 = player2.GetComponent<Rigidbody>();

        if (rb1 != null) rb1.linearVelocity = Vector3.zero;
        if (rb2 != null) rb2.linearVelocity = Vector3.zero;

        // Teleport players
        player1.position = player1Spawn.position;
        player2.position = player2Spawn.position;

        // Reset health
        player1Health.ResetHealth();
        player2Health.ResetHealth();
    }
}