using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 200;
    private int currentHealth;

    public Slider healthBar;
    public GameObject youDiedText;

    void Start()
    {
        ResetHealth();
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        
        // Only update UI if it exists
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
        
        if (youDiedText != null)
        {
            youDiedText.SetActive(false);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        // Only update UI if it exists
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        currentHealth = 0;
        
        // Only update UI if it exists
        if (healthBar != null)
        {
            healthBar.value = 0;
        }
        
        if (youDiedText != null)
        {
            youDiedText.SetActive(true);
        }
        
        Debug.Log($"{gameObject.name} has died!");
    }
    
    // Public getter for checking health
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
    
    public bool IsAlive()
    {
        return currentHealth > 0;
    }
}   