using UnityEngine;

public class DisableFPS : MonoBehaviour
{
    [SerializeField] public GameObject[] playerObjects; // Array of Player gameobjects
    [SerializeField] private GameObject fpsUICanvas; // FPS UI like reticles, ammo counter, etc

    private void Start()
    {
        // Auto-find player objects if not assigned
        if (playerObjects == null || playerObjects.Length == 0)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            System.Array.Sort(players, (a, b) => a.name.CompareTo(b.name));
            playerObjects = players;
            Debug.Log($"Auto-found {playerObjects.Length} player objects");

            // If no players found by tag, try finding by name pattern
            if (playerObjects.Length == 0)
            {
                Debug.LogWarning("No GameObjects with 'Player' tag found. Make sure your Player objects are tagged!");
                // Try common naming patterns
                GameObject player1 = GameObject.Find("Player 1") ?? GameObject.Find("Player1");
                GameObject player2 = GameObject.Find("Player 2") ?? GameObject.Find("Player2");

                System.Collections.Generic.List<GameObject> foundPlayers = new System.Collections.Generic.List<GameObject>();
                if (player1 != null) foundPlayers.Add(player1);
                if (player2 != null) foundPlayers.Add(player2);

                if (foundPlayers.Count > 0)
                {
                    playerObjects = foundPlayers.ToArray();
                    Debug.Log($"Found {playerObjects.Length} players by name: {string.Join(", ", System.Array.ConvertAll(playerObjects, p => p.name))}");
                }
            }
        }

        // Disable FPS objects immediately on start (game starts in Board mode)
        DisableFPSObjects();
    }

    public void DisableFPSObjects()
    {
        // Disable FPS UI
        if (fpsUICanvas != null)
        {
            fpsUICanvas.SetActive(false);
        }

        // Disable player objects (they shouldn't move/act in board mode)
        foreach (GameObject player in playerObjects)
        {
            if (player != null)
            {
                player.SetActive(false);
            }
        }

        // Enable mouse cursor for UI interaction
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void EnableFPSObjects()
    {
        // Enable FPS UI
        if (fpsUICanvas != null)
        {
            fpsUICanvas.SetActive(true);
        }

        // Enable player objects
        foreach (GameObject player in playerObjects)
        {
            if (player != null)
            {
                player.SetActive(true);
            }
        }

        // Lock cursor for FPS mode
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}