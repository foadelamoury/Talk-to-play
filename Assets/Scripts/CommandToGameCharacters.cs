using System.Collections.Generic;
using System.Reflection;
using LLMUnity;
using UnityEngine;
using UnityEngine.UI;

public class CommandToGameCharacters : MonoBehaviour
{
    public LLMCharacter llmCharacter;
    public InputField playerText;
    public RectTransform blueSquare;  // Player
    public RectTransform enemySquare;   // Enemy
    public Text responseText;
    public GameObject bulletPrefab;
    public GameObject shieldPrefab;

    public Canvas canvas;

    public PlayerHealth playerHealth;
    public RedEnemy redEnemy;

    void Start()
    {
        if (llmCharacter == null)
        {
            llmCharacter = FindFirstObjectByType<LLMCharacter>();
            if (llmCharacter == null)
            {
                Debug.LogError("LLMCharacter not found in scene!");
                return;
            }
        }

        if (playerText != null)
        {
            playerText.onSubmit.AddListener(onInputFieldSubmit);
            playerText.Select();
        }
    }

    string[] GetFunctionNames<T>()
    {
        List<string> functionNames = new List<string>();
        foreach (var function in typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly))
            functionNames.Add(function.Name);
        return functionNames.ToArray();
    }

    string ConstructColorPrompt(string message)
    {
        string[] colorFunctions = GetFunctionNames<ColorFunctions>();

        string prompt = "Command: " + message + "\n\n";
        prompt += "Reply with EXACTLY one line:\n";
        prompt += "Choose ONE of these color functions:\n";
        
        foreach(string col in colorFunctions)
            prompt += col + " ";
        
        prompt += "\n\nExample:\nBlueColor";

        return prompt;
    }

    string ConstructDirectionPrompt(string message)
    {
        string[] directionFunctions = GetFunctionNames<DirectionFunctions>();

        string prompt = "Command: " + message + "\n\n";
        prompt += "Reply with EXACTLY one line:\n";
        prompt += "Choose ONE of these direction functions:\n";
        
        foreach(string dir in directionFunctions)
            prompt += dir + " ";
        
        prompt += "\n\nExample:\nMoveUp";
        
        return prompt;
    }

    string ConstructSkillPrompt(string message)
    {
        string[] skillFunctions = GetFunctionNames<SkillsFunctions>();

        string prompt = "Command: " + message + "\n\n";
        prompt += "Reply with EXACTLY one line:\n";
        prompt += "Choose ONE of these skill functions:\n";
        
        foreach(string skill in skillFunctions)
            prompt += skill + " ";
        
        prompt += "\n\nExample:\nAttack";

        return prompt;
    }

    public async void onInputFieldSubmit(string message)
    {
        if (string.IsNullOrEmpty(message)) return;
        
        playerText.interactable = false;

        try
        {
            Debug.Log("Processing message: " + message);
            
            // Get color response
            string colorResponse = await llmCharacter.Chat(ConstructColorPrompt(message));
            string colorFunctionName = colorResponse.Split('\n')[0];
            Debug.Log("Color function: " + colorFunctionName);

            // Get direction response
            string directionResponse = await llmCharacter.Chat(ConstructDirectionPrompt(message));
            string directionFunctionName = directionResponse.Split('\n')[0];
            Debug.Log("Direction function: " + directionFunctionName);

            // Get skill response
            string skillResponse = await llmCharacter.Chat(ConstructSkillPrompt(message));
            string skillFunctionName = skillResponse.Split('\n')[0];
            Debug.Log("Skill function: " + skillFunctionName);

            // Safely invoke the color function
            var colorMethod = typeof(ColorFunctions).GetMethod(colorFunctionName);
            if (colorMethod == null)
            {
                Debug.LogError($"Color function '{colorFunctionName}' not found!");
                return;
            }
            Color color = (Color)colorMethod.Invoke(null, null);

            // Safely invoke the direction function
            var directionMethod = typeof(DirectionFunctions).GetMethod(directionFunctionName);
            if (directionMethod == null)
            {
                Debug.LogError($"Direction function '{directionFunctionName}' not found!");
                return;
            }
            Vector2 direction = (Vector2)directionMethod.Invoke(null, null);

            // Move the square
            RectTransform square = GetObjectByColor(color);
            Debug.Log("square: " + square.name);

            if (square != null)
            {
                square.anchoredPosition += direction * 50f;

                if (skillFunctionName == "Attack")
                {
                    if (ColorMatch(color, Color.blue))
                    {
                        Debug.Log("Blue square attacking enemy");
                        if(enemySquare != null)
                        SkillsFunctions.Attack(bulletPrefab, blueSquare, enemySquare);
                        else
                        SkillsFunctions.Attack(bulletPrefab, blueSquare, null);
                        responseText.text = "Blue square attacks enemy!";
                    }
                }
                else if (skillFunctionName == "Shield")
                {
                    if (ColorMatch(color, Color.blue))
                    {
                        // Only blue square can use shield
                        SkillsFunctions.Shield(shieldPrefab, square,canvas);
                        responseText.text = "Blue square uses shield!";
                    }
                  
                }
                else 
                {
                    SkillsFunctions.NoAction();
                    responseText.text = "No action taken";
                }
                
            }
        }
        catch (System.Exception e)
        {
                Debug.LogError($"Error processing command: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
            responseText.text = "Error: Could not process command";
        }
        finally
        {
            playerText.text = "";
            playerText.interactable = true;
        }
    }

    private RectTransform GetObjectByColor(Color color)
    {
        Debug.Log($"Getting object for color: {color}"); // Debug log
        
        // Normalize the input color to handle potential alpha variations
        Color normalizedColor = new Color(color.r, color.g, color.b, 1f);
        
        if (ColorMatch(normalizedColor, Color.blue))
        {
            Debug.Log("Returning blue square"); // Debug log
            return blueSquare;
        }
        else if (ColorMatch(normalizedColor, Color.red) || ColorMatch(normalizedColor, Color.yellow))
        {
            Debug.Log("Returning enemy square"); // Debug log
            Debug.Log("enemy square: " + enemySquare.name);
            return enemySquare;
        }
        Debug.Log("No matching square found"); // Debug log
        return null;
    }

    private bool ColorMatch(Color a, Color b, float tolerance = 0.1f)  // Increased tolerance
    {
        // Compare only RGB values, ignore alpha
        bool match = Mathf.Abs(a.r - b.r) < tolerance &&
                    Mathf.Abs(a.g - b.g) < tolerance &&
                    Mathf.Abs(a.b - b.b) < tolerance;
        Debug.Log($"Color match between {a} and {b}: {match}"); // Debug log
        return match;
    }

    public void CancelRequests()
    {
        llmCharacter.CancelRequests();
    }

    public void ExitGame()
    {
        Debug.Log("Exit button clicked");
        Application.Quit();
    }

    bool onValidateWarning = true;
    void OnValidate()
    {
        if (onValidateWarning && !llmCharacter.remote && llmCharacter.llm != null && llmCharacter.llm.model == "")
        {
            Debug.LogWarning($"Please select a model in the {llmCharacter.llm.gameObject.name} GameObject!");
            onValidateWarning = false;
        }
    }
}