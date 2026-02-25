using UnityEngine;

public class DisableFPS : MonoBehaviour
{
    [SerializeField] GameObject playerCamera;
    [SerializeField] GameObject player;
    [SerializeField] GameObject canvas;
    [SerializeField] MainCam mainCam;
    [SerializeField] GameObject mainCamera;
    [SerializeField] GameObject rollButton;

    [SerializeField] float mainCameraX = 0.0f;
    [SerializeField] float mainCameraY = 69.31f;
    [SerializeField] float mainCameraZ = 10.4f;


    void Start()
    {
        mainCam = MainCam.FindFirstObjectByType<MainCam>();
    }

    public void DisableFPSObjects()
    {
        playerCamera.SetActive(false);
        player.SetActive(false);
        //canvas.SetActive(false);
        mainCam.enabled = false;
        mainCamera.transform.position = new Vector3(mainCameraX, mainCameraY, mainCameraZ);

        rollButton.SetActive(true);
    }

    public void EnableFPSObjects()
    {
        playerCamera.SetActive(true);
        player.SetActive(true);
        //canvas.SetActive(true);
        mainCam.enabled = true;

        rollButton.SetActive(false);
    }
}
