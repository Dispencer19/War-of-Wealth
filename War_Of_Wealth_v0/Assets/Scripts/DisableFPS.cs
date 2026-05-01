using UnityEngine;
using Photon.Pun;

public class DisableFPS : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject playerCamera;
    [SerializeField] GameObject player;

    [SerializeField] MainCam mainCam;
    [SerializeField] GameObject mainCamera;

    [SerializeField] GameObject rollButton;

    [Header("UI References")]
    [SerializeField] GameObject Reticle;

    [Header("Camera Settings")]
    [SerializeField] float mainCameraX = 0.0f;
    [SerializeField] float mainCameraY = 69.31f;
    [SerializeField] float mainCameraZ = 10.4f;

    private PhotonView photonView;

    void Start()
    {
        mainCam = MainCam.FindFirstObjectByType<MainCam>();
        photonView = GetComponent<PhotonView>();
    }

    public void DisableFPSObjects()
    {
        // Always disable FPS for local player (scene object, not network-owned)
        playerCamera.SetActive(false);
        player.SetActive(false);
        mainCam.enabled = false;

        mainCamera.transform.position = new Vector3(mainCameraX, mainCameraY, mainCameraZ);

        //Enable mouse cursor for UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Reticle.SetActive(false);
    }

    public void EnableFPSObjects()
    {
        // Always enable FPS for local player (scene object, not network-owned)
        playerCamera.SetActive(true);
        player.SetActive(true);
        mainCam.enabled = true;

        //Lock cursor again for FPS mode
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Reticle.SetActive(true);
    }
}