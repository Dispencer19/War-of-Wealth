using UnityEngine;
using UnityEngine.InputSystem;

public class GameMode : MonoBehaviour
{
    bool isFPSMode = false;
    [SerializeField] DisableFPS disableFPS;
    [SerializeField] GameObject boardEmpty;

    void Start()
    {
        disableFPS = DisableFPS.FindFirstObjectByType<DisableFPS>();
        if(!isFPSMode)
        {
            disableFPS.DisableFPSObjects();
        }
    }

    //public void switchGameModeButton()
    void Update()
    {
        //if (Keyboard.current.gKey.wasPressedThisFrame)
    }

    // switch to other game mode
    public void buttonSwitchGameMode()
    {
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
    }

}
