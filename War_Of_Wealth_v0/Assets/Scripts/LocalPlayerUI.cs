using UnityEngine;
using Photon.Pun;
using TMPro;

/// <summary>
/// Manages local-only UI elements that should NOT be synchronized across network.
/// These include: reticle, personal health, personal money display.
/// </summary>
public class LocalPlayerUI : MonoBehaviour
{
    [Header("Local-Only UI Elements")]
    public GameObject reticle;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI moneyText;
    public GameObject healthBar;

    private PhotonView photonView;
    private PlayerHealth playerHealth;
    private PlayerBankAccounts playerBank;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        playerHealth = GetComponent<PlayerHealth>();
        playerBank = GetComponent<PlayerBankAccounts>();
    }

    private void Start()
    {
        // Only enable local UI for the local player
        if (photonView != null && photonView.IsMine)
        {
            EnableLocalUI();
        }
        else
        {
            DisableLocalUI();
        }
    }

    private void Update()
    {
        // Only update local player's UI
        if (photonView != null && photonView.IsMine)
        {
            UpdateLocalUI();
        }
    }

    private void EnableLocalUI()
    {
        if (reticle != null) reticle.SetActive(true);
        if (healthBar != null) healthBar.SetActive(true);
        if (healthText != null) healthText.gameObject.SetActive(true);
        if (moneyText != null) moneyText.gameObject.SetActive(true);
    }

    private void DisableLocalUI()
    {
        if (reticle != null) reticle.SetActive(false);
        if (healthBar != null) healthBar.SetActive(false);
        if (healthText != null) healthText.gameObject.SetActive(false);
        if (moneyText != null) moneyText.gameObject.SetActive(false);
    }

    private void UpdateLocalUI()
    {
        if (playerHealth != null && healthText != null)
        {
            healthText.text = $"HP: {playerHealth.currentHealth}";
        }

        if (playerBank != null && moneyText != null)
        {
            moneyText.text = $"${playerBank.currentBalance}";
        }
    }
}