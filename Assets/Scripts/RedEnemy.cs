using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RedEnemy : MonoBehaviour
{
    public static RedEnemy instance;
    public int health = 50;
    public int attackDamage = 10;
    public Slider healthBar;
    public RectTransform rectTransform;
    public float moveSpeed = 300f;  // Speed for moving towards player
    
    private Vector2 originalPosition;
    private bool isAttacking = false;
    private float returnDelay = 1f;  // Time to wait before returning
    private RectTransform attackTarget; // Target to attack (player)

    private void Awake()
    {
        instance = this;
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
    }

    private void Update()
    {
        if (isAttacking && attackTarget != null)
        {
            // Move towards target (player)
            Vector2 direction = (attackTarget.anchoredPosition - rectTransform.anchoredPosition).normalized;
            rectTransform.anchoredPosition += direction * moveSpeed * Time.deltaTime;
            
            // Rotate towards target
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rectTransform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log(name + " took " + damage + " damage!");
        healthBar.value = health;
        
        if (health <= 0)
        {
            Die();
        }
        else
        {
            // Find the player and start attacking
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                attackTarget = player.GetComponent<RectTransform>();
                StartAttack();
                Debug.Log("Red enemy attacking player!");
            }
        }
    }

    void StartAttack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            Debug.Log($"{name} is attacking {attackTarget.name}!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
{
    if (isAttacking && other.CompareTag("Player"))
    {
        // Handle collision with player
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player != null)
        {
            player.TakeDamage(attackDamage);
            Debug.Log($"{name} hit {other.name}!");
            
            // Stop attacking and start returning to original position
            isAttacking = false;
            Invoke(nameof(ReturnToPosition), returnDelay);
        }
    }
}

    void ReturnToPosition()
    {
        StartCoroutine(MoveToOriginalPosition());
    }

    System.Collections.IEnumerator MoveToOriginalPosition()
    {
        while (Vector2.Distance(rectTransform.anchoredPosition, originalPosition) > 5f)
        {
            // Move back to original position
            Vector2 direction = (originalPosition - rectTransform.anchoredPosition).normalized;
            rectTransform.anchoredPosition += direction * moveSpeed * Time.deltaTime;
            
            // Rotate towards return direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rectTransform.rotation = Quaternion.Euler(0, 0, angle);
            
            yield return null;
        }

        // Ensure exact position
        rectTransform.anchoredPosition = originalPosition;
        Debug.Log($"{name} returned to original position");
        attackTarget = null; // Clear the target
    }

    void Die()
    {
        Debug.Log($"{name} has been defeated!");
        Destroy(gameObject);
    }
}