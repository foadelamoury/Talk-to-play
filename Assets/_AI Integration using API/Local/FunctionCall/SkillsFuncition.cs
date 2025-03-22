using UnityEngine;
using System.Collections;

public class SkillsFunctions
{
    private static Quaternion blueOriginalRotation = Quaternion.identity;  // Default value

    // Initialize the coroutine runner
    void Awake()
    {
        // Store original rotation if blue square exists
        GameObject blueSquare = GameObject.FindGameObjectWithTag("Player");
        if (blueSquare != null)
        {
            blueOriginalRotation = blueSquare.GetComponent<RectTransform>().rotation;
        }
    }

    public static void Attack(GameObject bulletPrefab, RectTransform attacker, RectTransform target)
    {
        try
        {
            if (bulletPrefab == null || attacker == null)
            {
                Debug.LogError("Missing references for Attack function");
                return;
            }
            
            if(target == null)
            {
                 Debug.LogError("Missing Enemy for Attack function");
               
            }
            // Calculate direction using anchoredPosition
            Vector2 direction = (target.anchoredPosition - attacker.anchoredPosition).normalized;
            
            // Calculate rotation angle based on direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            attacker.rotation = Quaternion.Euler(0, 0, angle);

            // Create bullet as a child of the canvas
            Canvas canvas = GameObject.FindGameObjectWithTag("UIScene")?.GetComponent<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("Canvas not found!");
                return;
            }

            GameObject bullet = GameObject.Instantiate(bulletPrefab, canvas.transform);
            if (bullet == null)
            {
                Debug.LogError("Failed to instantiate bullet");
                return;
            }

            // Set bullet position and initialize
            RectTransform bulletRect = bullet.GetComponent<RectTransform>();
            bulletRect.anchoredPosition = attacker.anchoredPosition;

            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.Initialize(target.anchoredPosition, attacker.anchoredPosition);
                
                // Instantly reset attacker rotation
                if (attacker.CompareTag("Player"))
                {
                    attacker.rotation = Quaternion.identity;
                }
            }
            else
            {
                Debug.LogError("Bullet script not found on prefab");
                GameObject.Destroy(bullet);
            }
            
            Debug.Log($"Attacking {target.name} at angle: {angle}, direction: {direction}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in Attack function: {e.Message}\nStack trace: {e.StackTrace}");
        }
    }

    public static void Shield(GameObject shieldPrefab, RectTransform defender, Canvas canvas)
    {
        if (shieldPrefab == null || defender == null)
        {
            Debug.LogError("Missing references for Shield function");
            return;
        }

        GameObject shield = GameObject.Instantiate(shieldPrefab, defender.position, Quaternion.identity);
        
        if (canvas != null)
        {
            shield.transform.SetParent(canvas.transform, false);
        }
        
        shield.transform.position = defender.position;
        GameObject.Destroy(shield, 5f);
    }

    public static void NoAction()
    {
        Debug.Log("No action taken");
    }
}