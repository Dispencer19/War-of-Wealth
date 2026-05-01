using UnityEngine;

public class DisableFPS : MonoBehaviour
{
    [SerializeField] GameObject playerCamera;
    [SerializeField] GameObject player;

    [SerializeField] MainCam mainCam;
    [SerializeField] GameObject mainCamera;

    [SerializeField] GameObject rollButton;

    [SerializeField] GameObject Reticle;
    [SerializeField] float mainCameraX = 0.0f;
    [SerializeField] float mainCameraY = 69.31f;
    [SerializeField] float mainCameraZ = 0.0f;

    void Start()
    {

        mainCam = MainCam.FindFirstObjectByType<MainCam>();
    }

    public void DisableFPSObjects()
    {
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
        playerCamera.SetActive(true);
        player.SetActive(true);
        mainCam.enabled = true;

        //Lock cursor again for FPS mode
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Reticle.SetActive(true);
    }
}