using UnityEngine;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [Header("Player References")]
    [SerializeField] public List<GameObject> playerObjects = new List<GameObject>();
    [SerializeField] private List<Transform> playerTransforms = new List<Transform>();
    [SerializeField] private List<PlayerMovement_SpencerHP> playerMovements = new List<PlayerMovement_SpencerHP>();
    [SerializeField] private List<PlayerHealth> playerHealths = new List<PlayerHealth>();
    [SerializeField] private List<PlayerBankAccounts> playerBanks = new List<PlayerBankAccounts>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializePlayers();
    }

    private void InitializePlayers()
    {
        // Find all players by tag
        GameObject[] foundPlayers = GameObject.FindGameObjectsWithTag("Player");

        // Sort by name to ensure consistent ordering
        System.Array.Sort(foundPlayers, (a, b) => a.name.CompareTo(b.name));

        foreach (GameObject player in foundPlayers)
        {
            playerObjects.Add(player);
            playerTransforms.Add(player.transform);

            // Get components
            PlayerMovement_SpencerHP movement = player.GetComponent<PlayerMovement_SpencerHP>();
            if (movement != null)
            {
                playerMovements.Add(movement);
                // Set player index based on order
                movement.playerIndex = playerMovements.Count - 1;
            }

            PlayerHealth health = player.GetComponent<PlayerHealth>();
            if (health != null)
                playerHealths.Add(health);

            PlayerBankAccounts bank = player.GetComponent<PlayerBankAccounts>();
            if (bank != null)
                playerBanks.Add(bank);
        }

        Debug.Log($"PlayerManager initialized with {playerObjects.Count} players");
    }

    public int GetPlayerCount()
    {
        return playerObjects.Count;
    }

    public GameObject GetPlayer(int index)
    {
        if (index >= 0 && index < playerObjects.Count)
            return playerObjects[index];
        return null;
    }

    public Transform GetPlayerTransform(int index)
    {
        if (index >= 0 && index < playerTransforms.Count)
            return playerTransforms[index];
        return null;
    }

    public PlayerMovement_SpencerHP GetPlayerMovement(int index)
    {
        if (index >= 0 && index < playerMovements.Count)
            return playerMovements[index];
        return null;
    }

    public PlayerHealth GetPlayerHealth(int index)
    {
        if (index >= 0 && index < playerHealths.Count)
            return playerHealths[index];
        return null;
    }

    public PlayerBankAccounts GetPlayerBank(int index)
    {
        if (index >= 0 && index < playerBanks.Count)
            return playerBanks[index];
        return null;
    }

    public void EnablePlayerMovement(int index)
    {
        PlayerMovement_SpencerHP movement = GetPlayerMovement(index);
        if (movement != null)
            movement.canMove = true;
    }

    public void DisablePlayerMovement(int index)
    {
        PlayerMovement_SpencerHP movement = GetPlayerMovement(index);
        if (movement != null)
            movement.canMove = false;
    }

    public void TeleportPlayer(int index, Vector3 position, Quaternion rotation = default)
    {
        PlayerMovement_SpencerHP movement = GetPlayerMovement(index);
        if (movement != null)
        {
            movement.TeleportPlayer(position, rotation);
        }
        else
        {
            // Fallback if no movement component
            Transform playerTransform = GetPlayerTransform(index);
            if (playerTransform != null)
            {
                playerTransform.position = position;
                if (rotation != default)
                    playerTransform.rotation = rotation;
            }
        }
    }

    public void ResetPlayerHealth(int index)
    {
        PlayerHealth health = GetPlayerHealth(index);
        if (health != null)
            health.ResetHealth();
    }
}