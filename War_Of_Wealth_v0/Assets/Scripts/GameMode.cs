using UnityEngine;
using UnityEngine.InputSystem;


public class GameMode : MonoBehaviour
{
    [SerializeField] public bool isFPSMode = false; // Default to Board mode for UI interaction
    [SerializeField] DisableFPS disableFPS;
    [SerializeField] GameObject boardEmpty;
    [SerializeField] CameraManager cameraManager;

    public bool IsFPSMode => isFPSMode;

    void Start()
    {
        disableFPS = DisableFPS.FindFirstObjectByType<DisableFPS>();
        cameraManager = CameraManager.Instance;

        // Start in Board mode by default so players can interact with UI
        isFPSMode = false;
        CameraMode initialMode = CameraMode.Board;
        
        if (cameraManager != null)
        {
            cameraManager.SetupCameras(
                PlayerManager.Instance != null ? PlayerManager.Instance.GetPlayerCount() : 1,
                initialMode
            );
        }

        // Note: DisableFPS.DisableFPSObjects() is called automatically in DisableFPS.Start()
        // No need to call it here again
    }

    //public void switchGameModeButton()
    void Update()
    {
        //if (Keyboard.current.gKey.wasPressedThisFrame)
    }

    // switch to other game mode
    public void buttonSwitchGameMode()
    {
        CameraMode newMode = !isFPSMode ? CameraMode.FPS : CameraMode.Board;

        if (!isFPSMode)
        {
            boardEmpty.SetActive(false);
            disableFPS.EnableFPSObjects();

            isFPSMode = true;
            Debug.Log("switched from board to fps mode");
        }
        else // isFPSMode
        {
            disableFPS.DisableFPSObjects();
            boardEmpty.SetActive(true);

            isFPSMode = false;
            Debug.Log("switched from fps to board mode");
        }

        // Update camera manager
        if (cameraManager != null)
        {
            cameraManager.SwitchMode(newMode);
        }
    }

}
