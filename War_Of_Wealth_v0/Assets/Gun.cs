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

    [Header("Bloom (Accuracy)")]
    [SerializeField] private float baseSpread = 0.2f;      // Minimum accuracy
    [SerializeField] private float maxSpread = 3f;         // Max bloom
    [SerializeField] private float spreadIncrease = 0.4f;  // Per shot
    [SerializeField] private float spreadRecovery = 6f;    // Recovery speed

    [Header("References")]
    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public LineRenderer Tracer;

    [Header("Player Input")]
    [SerializeField] public int playerIndex = 0;
    public Transform ownerRoot;

    [Header("UI")]
    public TextMeshProUGUI ammoText;

    [Header("Optional Effects")]
    public GameObject hitEffect;
    public int score = 0;

    private float nextTimeToFire = 0f;
    private int currentammo;
    private bool isReloading = false;

    private float currentSpread;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        currentammo = maxammo;
        currentSpread = baseSpread;

        if (fpsCam == null) fpsCam = GetComponentInParent<Camera>();
        if (fpsCam == null && transform.root != null)
            fpsCam = transform.root.GetComponentInChildren<Camera>(true);

        if (ownerRoot == null) ownerRoot = transform.root;

        UpdateAmmoUI();
    }

    void Update()
    {
        // Bloom recovery
        currentSpread = Mathf.Lerp(currentSpread, baseSpread, spreadRecovery * Time.deltaTime);

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

    private bool FirePressed()
    {
        if (playerIndex == 0)
            return Mouse.current != null && Mouse.current.leftButton.isPressed;

        return Gamepad.current != null && Gamepad.current.rightTrigger.ReadValue() > 0.5f;
    }

    private bool ReloadPressed()
    {
        if (playerIndex == 0)
            return Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame;

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

        // Increase bloom
        currentSpread = Mathf.Clamp(currentSpread + spreadIncrease, baseSpread, maxSpread);

        if (muzzleFlash != null)
            muzzleFlash.Play();

        if (audioSource != null && firesound != null)
            audioSource.PlayOneShot(firesound);

        Vector3 startPoint = muzzle != null ? muzzle.position : fpsCam.transform.position;

        // Apply bloom to direction
        Vector3 spread = new Vector3(
            Random.Range(-currentSpread, currentSpread),
            Random.Range(-currentSpread, currentSpread),
            0f
        );

        Vector3 direction = fpsCam.transform.forward +
                            fpsCam.transform.TransformDirection(spread * 0.01f);

        Ray ray = new Ray(fpsCam.transform.position, direction);
        RaycastHit hit;

        Vector3 endPoint;

        if (Physics.Raycast(ray, out hit, range))
        {
            endPoint = hit.point;

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

            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(damage);
                score += 10;
            }

            if (hitEffect != null)
                Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
        }
        else
        {
            endPoint = startPoint + direction * range;
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
        if (!isReloading)
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
        currentSpread = baseSpread;

        UpdateAmmoUI();
    }

    void UpdateAmmoUI()
    {
        if (ammoText == null) return;

        ammoText.text = isReloading
            ? "Reloading..."
            : currentammo + " / " + maxammo;
    }
}