using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 200;
    private int currentHealth;

    public Slider healthBar;
    // public Canvas canvas;
    public GameObject youDiedText;

    void Start()
    {
        ResetHealth();
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
        // canvas = Object.FindFirstObjectByType<Canvas>();
        // canvas = transform.Find("FPS Canvas").canvas;
        // youDiedText = transform.Find("FPS Canvas/HealthBar").GetComponent<Slider>;
        // youDiedText = GameObject.Find("HealthBar");
        youDiedText.SetActive(false);
        Invoke("CheckIfInitialized", 3.0f);
    }

    public void CheckIfInitialized()
    {
        if(healthBar == null)
            Debug.Log("healthbar not found");
        if(youDiedText == null)
            Debug.Log("youDiedText not found");
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        currentHealth = 0;
        healthBar.value = 0;
        youDiedText.SetActive(true);
    }
}