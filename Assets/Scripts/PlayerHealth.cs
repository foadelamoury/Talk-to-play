using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth instance;
    public int health = 100;
    public Slider healthBar;
    public RectTransform rectTransform;

    private void Awake()
    {
        instance = this;
        rectTransform = GetComponent<RectTransform>();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Player took " + damage + " damage! HP: " + health);
        if (healthBar != null)
        {
            healthBar.value = health;
        }
        
        if (health <= 0)
        {
            Die();
        }
    }
    
    
    private void Die()
    {
        Debug.Log("Player has been defeated!");
        // Handle player death (game over, respawn, etc.)
    }
}