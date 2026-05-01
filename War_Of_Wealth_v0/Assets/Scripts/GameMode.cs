using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class GameMode : MonoBehaviour
{
    [SerializeField] bool isFPSMode = false;
    [SerializeField] DisableFPS disableFPS;
    [SerializeField] GameObject boardEmpty;

    [SerializeField] public NetworkUIManager networkUI;

    private PhotonView photonView;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        disableFPS = DisableFPS.FindFirstObjectByType<DisableFPS>();
        
        if (networkUI == null)
            networkUI = FindAnyObjectByType<NetworkUIManager>();

        if(!isFPSMode)
        {
            disableFPS.DisableFPSObjects();
        }
        else // isFPSMode
        {
            disableFPS.EnableFPSObjects();
        }
    }

    //public void switchGameModeButton()
    void Update()
    {
        //if (Keyboard.current.gKey.wasPressedThisFrame)
    }

    [PunRPC]
    public void RPC_SwitchGameMode()
    {
        SwitchGameModeInternal();
    }

    // switch to other game mode
    public void buttonSwitchGameMode()
    {
        // Call RPC on all clients to sync game mode
        photonView.RPC("RPC_SwitchGameMode", RpcTarget.All);
    }

    private void SwitchGameModeInternal()
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
