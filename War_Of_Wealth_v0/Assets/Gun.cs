using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;

public class Gun : MonoBehaviour
{
    [Header("Gun Settings")]
    public float damage = 25f;
    public float range = 100f;
    public float fireRate = 0.2f;

    public AudioClip firesound;
    public AudioClip reloadSound;

    public Transform muzzle;
    public float reloadtime = 1.5f;
    public int maxammo = 6;

    [Header("References")]
    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public LineRenderer Tracer;

    [Header("UI")]
    public TextMeshProUGUI ammoText;

    [Header("Optional Effects")]
    public GameObject hitEffect;
    public int score = 0;

    private float nextTimeToFire = 0f;
    private int currentammo;
    private bool isReloading = false;

    private PlayerControls controls;
    private bool isFiring;

    private AudioSource audioSource;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Fire.started += ctx => isFiring = true;
        controls.Player.Fire.canceled += ctx => isFiring = false;
        controls.Player.Reload.performed += ctx => Reload();

        audioSource = GetComponent<AudioSource>();
        currentammo = maxammo;

        UpdateAmmoUI();
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    void Update()
    {
        if (isReloading)
            return;

        if (isFiring && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        if (currentammo <= 0)
        {
            Reload();
            return;
        }

        currentammo--;
        UpdateAmmoUI();

        Vector3 startPoint = muzzle.position;
        Vector3 endPoint;

        if (muzzleFlash != null)
            muzzleFlash.Play();

        audioSource.Stop();
        audioSource.PlayOneShot(firesound);

        Ray ray = new Ray(fpsCam.transform.position, fpsCam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range))
        {
            endPoint = hit.point;

            // HIT TARGET CHECK
            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(damage);

                // SCORE SYSTEM
                score += 10;
            }

            // HIT EFFECT
            if (hitEffect != null)
            {
                Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }
        else
        {
            endPoint = startPoint + fpsCam.transform.forward * range;
        }

        StartCoroutine(ShowTracer(startPoint, endPoint));
    }

    private IEnumerator ShowTracer(Vector3 start, Vector3 end)
    {
        Tracer.enabled = true;
        Tracer.SetPosition(0, start);
        Tracer.SetPosition(1, end);

        yield return new WaitForSeconds(0.05f);

        Tracer.enabled = false;
    }

    void Reload()
    {
        if (isReloading)
            return;

        StartCoroutine(Reloading());
    }

    IEnumerator Reloading()
    {
        isReloading = true;
        UpdateAmmoUI();

        if (reloadSound != null)
            audioSource.PlayOneShot(reloadSound);

        yield return new WaitForSeconds(reloadtime);

        currentammo = maxammo;
        isReloading = false;

        UpdateAmmoUI();
    }

    void UpdateAmmoUI()
    {
        if (ammoText == null)
            return;

        ammoText.text = isReloading
            ? "Reloading..."
            : currentammo + " / " + maxammo;
    }
}