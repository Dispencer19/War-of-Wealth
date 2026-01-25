using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cube : MonoBehaviour
{
    
    [SerializeField] private Cube cube;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(0.5f, 1f, 0f);
    }

    void FixedUpdate()
    {

    }
    
}
