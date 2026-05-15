using UnityEngine;
using System.Collections.Generic;

public enum CameraMode
{
    Board,
    FPS
}

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [Header("Board Camera Settings")]
    [SerializeField] private Transform boardCameraPosition;
    [SerializeField] private Vector3 boardCameraOffset = new Vector3(0, 69.31f, 0);

    [Header("FPS Camera Settings")]
    [SerializeField] private float fpsMouseSensitivity = 100f;

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
            Camera playerCam = playerObj.GetComponentInChildren<Camera>();
            if (playerCam != null)
            {
                playerCameras.Add(playerCam);
            }
        }

        Debug.Log($"Found {playerCameras.Count} player cameras");
    }

    private void ConfigureBoardMode()
    {
        // Disable all player cameras
        foreach (Camera cam in playerCameras)
        {
            if (cam != null)
            {
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
        // Disable main camera
        if (mainCamera != null)
        {
            mainCamera.enabled = false;
        }

        // Enable and configure player cameras
        for (int i = 0; i < playerCameras.Count; i++)
        {
            Camera cam = playerCameras[i];
            if (cam != null)
            {
                cam.enabled = true;

                // Add or configure PlayerCam component for FPS control
                PlayerCam playerCam = cam.GetComponent<PlayerCam>();
                if (playerCam == null)
                {
                    playerCam = cam.gameObject.AddComponent<PlayerCam>();
                }

                playerCam.playerIndex = i;
                playerCam.sensX = fpsMouseSensitivity;
                playerCam.sensY = fpsMouseSensitivity;
            }
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
        if (playerIndex == 0) return mainCamera;
        if (playerIndex > 0 && playerIndex - 1 < playerCameras.Count)
            return playerCameras[playerIndex - 1];
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