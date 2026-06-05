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

    [Header("Player Input")]
    [SerializeField] public int playerIndex = 0; // 0 = player 1 (mouse + R), 1 = player 2 (gamepad)
    [Tooltip("Root transform of the player that owns this gun, used to prevent self-damage.")]
    public Transform ownerRoot;

    [Header("UI")]
    public TextMeshProUGUI ammoText;

    [Header("Optional Effects")]
    public GameObject hitEffect;
    public int score = 0;

    private float nextTimeToFire = 0f;
    private int currentammo;
    private bool isReloading = false;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        currentammo = maxammo;

        // Fallbacks so the gun still works if references aren't wired in the inspector.
        if (fpsCam == null) fpsCam = GetComponentInParent<Camera>();
        if (fpsCam == null && transform.root != null) fpsCam = transform.root.GetComponentInChildren<Camera>(true);
        if (ownerRoot == null) ownerRoot = transform.root;

        UpdateAmmoUI();
    }

    void Update()
    {
        if (isReloading)
            return;

        if (ReloadPressed())
        {
            Reload();
            return;
        }

        if (FirePressed() && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + fireRate;
            Shoot();
        }
    }

    // ---- Per-player input ----
    private bool FirePressed()
    {
        if (playerIndex == 0)
            return Mouse.current != null && Mouse.current.leftButton.isPressed;

        // Player 2: gamepad right trigger
        return Gamepad.current != null && Gamepad.current.rightTrigger.ReadValue() > 0.5f;
    }

    private bool ReloadPressed()
    {
        if (playerIndex == 0)
            return Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame;

        // Player 2: gamepad west button (X on Xbox / Square on PlayStation)
        return Gamepad.current != null && Gamepad.current.buttonWest.wasPressedThisFrame;
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

        Vector3 startPoint = muzzle != null ? muzzle.position : fpsCam.transform.position;
        Vector3 endPoint;

        if (muzzleFlash != null)
            muzzleFlash.Play();

        if (audioSource != null && firesound != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(firesound);
        }

        Ray ray = new Ray(fpsCam.transform.position, fpsCam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range))
        {
            endPoint = hit.point;

            // PVP: damage the other player.
            PlayerHealth playerHealth = hit.transform.GetComponentInParent<PlayerHealth>();
            if (playerHealth != null)
            {
                bool isSelf = ownerRoot != null && playerHealth.transform.root == ownerRoot.root;
                if (!isSelf)
                {
                    playerHealth.TakeDamage(Mathf.RoundToInt(damage));
                    score += 10;
                }
            }

            // Shooting-range targets (kept from the original behaviour).
            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(damage);
                score += 10;
            }

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
        if (Tracer == null) yield break;

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

        if (reloadSound != null && audioSource != null)
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