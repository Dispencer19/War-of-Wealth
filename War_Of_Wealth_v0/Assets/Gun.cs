using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Gun : MonoBehaviour
{
    [Header("Gun Settings")]
    public float damage = 25f;
    public float range = 100f;
    public float fireRate = 0.2f;
    public AudioClip firesound;
    public Transform muzzle;

    [Header("References")]
    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public LineRenderer Tracer;

    private float nextTimeToFire = 0f;

    // Input System
    private PlayerControls controls;
    private bool isFiring;

    // Audio
    private AudioSource audioSource;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Fire.started += ctx => isFiring = true;
        controls.Player.Fire.canceled += ctx => isFiring = false;

        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    void Update()
    {
        if (isFiring && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        Vector3 startpoint = muzzle.position;
        Vector3 direction = muzzle.forward;
        Vector3 endpoint;

        if (muzzleFlash != null)
            muzzleFlash.Play();

        // Play gunshot sound (restarts instantly)
        audioSource.Stop();
        audioSource.PlayOneShot(firesound);

        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            endpoint = hit.point;

            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
                target.TakeDamage(damage);
        }
        else
        {
            endpoint = startpoint + direction * range;
        }

        StartCoroutine(ShowTracer(startpoint, endpoint));
    }

    private IEnumerator ShowTracer(Vector3 start, Vector3 end)
    {
        Tracer.enabled = true;
        Tracer.SetPosition(0, start);
        Tracer.SetPosition(1, end);

        yield return new WaitForSeconds(0.05f);

        Tracer.enabled = false;
    }
}