using UnityEngine;

public class FightTeleport : MonoBehaviour
{
    public Transform player1;
    public Transform player2;

    public Transform player1Spawn;
    public Transform player2Spawn;

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
    }
}