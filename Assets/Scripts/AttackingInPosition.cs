using UnityEngine;

public class AttackingInPosition : MonoBehaviour
{
    public float attackRate = 2f;
    private float nextAttackTime = 0f;
    private RedEnemy currentEnemy;
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (Time.time >= nextAttackTime && currentEnemy != null)
        {
            AttackEnemy();
            nextAttackTime = Time.time + attackRate;
        }
    }

    public void SetTarget(RedEnemy enemy)
    {
        currentEnemy = enemy;
    }

    void AttackEnemy()
    {
        if (currentEnemy != null)
        {
            // Calculate direction to enemy
            Vector2 direction = (currentEnemy.rectTransform.anchoredPosition - rectTransform.anchoredPosition).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rectTransform.rotation = Quaternion.Euler(0, 0, angle);

            currentEnemy.TakeDamage(10);
            Debug.Log("Attacked " + currentEnemy.name);
        }
    }
}
