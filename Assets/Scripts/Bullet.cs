using UnityEngine;
using UnityEngine.UI;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float speed = 500f;  // Increased speed for UI space

    [SerializeField]
    private int damage = 10;

    private RectTransform rectTransform;
    private Vector2 direction;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("RectTransform not found on bullet!");
        }
    }

    public void Initialize(Vector2 targetPos, Vector2 startPos)
    {
        try
        {
            // Set initial position
            rectTransform.anchoredPosition = startPos;

            // Calculate direction
            direction = (targetPos - startPos).normalized;
            
            // Set rotation
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rectTransform.rotation = Quaternion.Euler(0, 0, angle);

            Debug.Log($"Bullet initialized: Start={startPos}, Target={targetPos}, Direction={direction}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in bullet initialization: {e.Message}");
            throw;
        }
    }

    void Update()
    {
        // Move the bullet
        rectTransform.anchoredPosition += direction * speed * Time.deltaTime;
        
        // Destroy if off screen
        if (Mathf.Abs(rectTransform.anchoredPosition.x) > 2000 || 
            Mathf.Abs(rectTransform.anchoredPosition.y) > 2000)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            RedEnemy enemy = other.GetComponent<RedEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
     
    }
}