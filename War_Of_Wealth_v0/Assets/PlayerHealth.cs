using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 200;
    private int currentHealth;

    [Header("UI")]
    public Slider healthBar;
    public GameObject youDiedText;

    [Header("Damage Feedback")]
    [SerializeField] private DamageFlash damageFlash;

    void Start()
    {
        ResetHealth();
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;

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

        // 🔴 Flash red screen on hit
        if (damageFlash != null)
        {
            damageFlash.Flash();
        }

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

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public bool IsAlive()
    {
        return currentHealth > 0;
    }
}