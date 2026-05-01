using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 200;
    public int currentHealth;

    [Header("UI References")]
    public Slider healthBar;
    public GameObject youDiedText;

    [Header("Network")]
    private PhotonView photonView;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        ResetHealth();
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        
        // Only update local player's health bar
        if (photonView.IsMine)
        {
            if (healthBar != null)
            {
                healthBar.maxValue = maxHealth;
                healthBar.value = currentHealth;
            }
            if (youDiedText != null)
                youDiedText.SetActive(false);
        }
    }

    [PunRPC]
    public void RPC_TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Only update local player's health bar
        if (photonView.IsMine)
        {
            if (healthBar != null)
                healthBar.value = currentHealth;

            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    public void TakeDamage(int damage)
    {
        // Call RPC to sync damage across network
        photonView.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }

    void Die()
    {
        currentHealth = 0;
        
        // Only show death UI for local player
        if (photonView.IsMine)
        {
            if (healthBar != null)
                healthBar.value = 0;
            if (youDiedText != null)
                youDiedText.SetActive(true);
        }
    }
}

    
