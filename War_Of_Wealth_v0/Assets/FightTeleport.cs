using UnityEngine;

public class FightTeleport : MonoBehaviour
{
    public Transform player1;
    public Transform player2;

    public Transform player1Spawn;
    public Transform player2Spawn;

    public PlayerHealth player1Health;
    public PlayerHealth player2Health;

    void Start()
    {
        // Don't try to find players immediately - they might not exist yet
        // Instead, find spawn points (they exist in the scene from the start)
        
        GameObject spawn1 = GameObject.Find("Player1FightSpawn");
        if (spawn1 != null)
            player1Spawn = spawn1.transform;
        else
            Debug.LogWarning("Player1FightSpawn not found in scene!");
            
        GameObject spawn2 = GameObject.Find("Player2FightSpawn");
        if (spawn2 != null)
            player2Spawn = spawn2.transform;
        else
            Debug.LogWarning("Player2FightSpawn not found in scene!");
    }

    public void StartFight()
    {
        // Find players when the fight actually starts (not in Start())
        // This ensures they've been spawned by Photon first
        
        GameObject p1 = GameObject.Find("Player1");
        GameObject p2 = GameObject.Find("Player2");
        
        // Check if players exist
        if (p1 == null)
        {
            Debug.LogError("Cannot start fight - Player1 not found!");
            return;
        }
        
        if (p2 == null)
        {
            Debug.LogError("Cannot start fight - Player2 not found!");
            return;
        }
        
        // Check if spawn points exist
        if (player1Spawn == null || player2Spawn == null)
        {
            Debug.LogError("Cannot start fight - spawn points not set!");
            return;
        }
        
        // Now we know everything exists, safe to proceed
        player1 = p1.transform;
        player2 = p2.transform;
        
        player1Health = player1.GetComponent<PlayerHealth>();
        player2Health = player2.GetComponent<PlayerHealth>();

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
        else
            Debug.LogWarning("Player1 has no PlayerHealth component!");
            
        if (player2Health != null)
            player2Health.ResetHealth();
        else
            Debug.LogWarning("Player2 has no PlayerHealth component!");
            
        Debug.Log("Fight started successfully!");
    }
}