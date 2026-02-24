using UnityEngine;

public class DisableFPS : MonoBehaviour
{
    [SerializeField] GameObject playerCamera;
    [SerializeField] GameObject player;
    [SerializeField] GameObject canvas;
    [SerializeField] MainCam mainCam; 

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
    }
}
