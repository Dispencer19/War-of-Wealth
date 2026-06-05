using UnityEngine;
using System.Collections.Generic;

public enum CameraMode
{
    Board,
    FPS
}

public enum SplitOrientation
{
    Horizontal, // stacked: player 1 top, player 2 bottom
    Vertical    // side by side: player 1 left, player 2 right
}

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [Header("Board Camera Settings")]
    [SerializeField] private Transform boardCameraPosition;
    [SerializeField] private Vector3 boardCameraOffset = new Vector3(0, 69.31f, 0);

    [Header("FPS Camera Settings")]
    [SerializeField] private float fpsMouseSensitivity = 100f;

    [Header("Split Screen")]
    [SerializeField] private SplitOrientation splitOrientation = SplitOrientation.Horizontal;

    [Header("References")]
    [SerializeField] private GameMode gameMode;
    [SerializeField] private DisableFPS disableFPS;

    private Camera mainCamera;
    private List<Camera> playerCameras = new List<Camera>();
    private CameraMode currentMode = CameraMode.Board;
    private int currentPlayerCount = 1;

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

        mainCamera = Camera.main;
        gameMode = FindFirstObjectByType<GameMode>();
        disableFPS = FindFirstObjectByType<DisableFPS>();
    }

    private void Start()
    {
        // Initialize with current player count
        int playerCount = PlayerManager.Instance != null ? PlayerManager.Instance.GetPlayerCount() : 1;
        SetupCameras(playerCount, currentMode);
    }

    public void SetupCameras(int playerCount, CameraMode mode)
    {
        currentPlayerCount = playerCount;
        currentMode = mode;

        // Find existing player cameras from Player objects
        FindPlayerCameras();

        // Configure cameras based on mode
        if (mode == CameraMode.Board)
        {
            ConfigureBoardMode();
        }
        else // FPS mode
        {
            ConfigureFPSMode();
        }
    }

    private void FindPlayerCameras()
    {
        playerCameras.Clear();

        // Find all Player objects and get their cameras
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        System.Array.Sort(playerObjects, (a, b) => a.name.CompareTo(b.name)); // Sort by name to ensure consistent order

        foreach (GameObject playerObj in playerObjects)
        {
            Camera playerCam = playerObj.GetComponentInChildren<Camera>(true);
            if (playerCam != null)
            {
                playerCameras.Add(playerCam);
            }
        }

        Debug.Log($"Found {playerCameras.Count} player cameras");
    }

    private void ConfigureBoardMode()
    {
        // Disable all player cameras and restore their viewport to full screen
        foreach (Camera cam in playerCameras)
        {
            if (cam != null)
            {
                cam.rect = new Rect(0, 0, 1, 1);
                cam.enabled = false;
            }
        }

        // Enable and configure main camera for board view
        if (mainCamera != null)
        {
            mainCamera.enabled = true;
            mainCamera.rect = new Rect(0, 0, 1, 1);

            // Position camera to look at board
            if (boardCameraPosition != null)
            {
                mainCamera.transform.position = boardCameraPosition.position;
            }
            else
            {
                mainCamera.transform.position = boardCameraOffset;
            }

            mainCamera.clearFlags = CameraClearFlags.Skybox;
        }
    }

    private void ConfigureFPSMode()
    {
        // Disable main (board) camera
        if (mainCamera != null)
        {
            mainCamera.enabled = false;
        }

        int activeCount = playerCameras.Count;

        // Enable and configure player cameras into split-screen viewports
        for (int i = 0; i < playerCameras.Count; i++)
        {
            Camera cam = playerCameras[i];
            if (cam == null) continue;

            cam.enabled = true;
            cam.rect = GetViewportRect(i, activeCount);

            // Add or configure PlayerCam component for FPS control
            PlayerCam playerCam = cam.GetComponent<PlayerCam>();
            if (playerCam == null)
            {
                playerCam = cam.gameObject.AddComponent<PlayerCam>();
            }

            playerCam.playerIndex = i;
            playerCam.sensX = fpsMouseSensitivity;
            playerCam.sensY = fpsMouseSensitivity;

            // Only player 1's camera keeps an active AudioListener to avoid
            // Unity's "multiple audio listeners" warning in split screen.
            AudioListener listener = cam.GetComponent<AudioListener>();
            if (listener != null) listener.enabled = (i == 0);
        }
    }

    // Returns the normalized viewport rect (origin bottom-left) for a player.
    private Rect GetViewportRect(int index, int count)
    {
        // Single player (or fallback): full screen.
        if (count <= 1) return new Rect(0, 0, 1, 1);

        // Two players: clean split based on chosen orientation.
        if (count == 2)
        {
            if (splitOrientation == SplitOrientation.Horizontal)
            {
                // index 0 = top, index 1 = bottom
                return index == 0 ? new Rect(0, 0.5f, 1, 0.5f)
                                  : new Rect(0, 0f, 1, 0.5f);
            }
            else
            {
                // index 0 = left, index 1 = right
                return index == 0 ? new Rect(0f, 0, 0.5f, 1)
                                  : new Rect(0.5f, 0, 0.5f, 1);
            }
        }

        // 3-4 players: quadrants (defensive fallback; not required for 2P).
        float halfW = 0.5f, halfH = 0.5f;
        switch (index)
        {
            case 0: return new Rect(0f, 0.5f, halfW, halfH);
            case 1: return new Rect(0.5f, 0.5f, halfW, halfH);
            case 2: return new Rect(0f, 0f, halfW, halfH);
            default: return new Rect(0.5f, 0f, halfW, halfH);
        }
    }

    public void SwitchMode(CameraMode newMode)
    {
        if (currentMode == newMode) return;

        currentMode = newMode;
        SetupCameras(currentPlayerCount, newMode);

        // Update UI and controls
        if (gameMode != null)
        {
            if (newMode == CameraMode.Board)
            {
                gameMode.isFPSMode = false;
                if (disableFPS != null) disableFPS.DisableFPSObjects();
            }
            else // FPS
            {
                gameMode.isFPSMode = true;
                if (disableFPS != null) disableFPS.EnableFPSObjects();
            }
        }
    }

    public Camera GetPlayerCamera(int playerIndex)
    {
        if (playerIndex >= 0 && playerIndex < playerCameras.Count)
            return playerCameras[playerIndex];
        return null;
    }

    public void UpdatePlayerCount()
    {
        int newCount = PlayerManager.Instance != null ? PlayerManager.Instance.GetPlayerCount() : 1;
        if (newCount != currentPlayerCount)
        {
            SetupCameras(newCount, currentMode);
        }
    }
}