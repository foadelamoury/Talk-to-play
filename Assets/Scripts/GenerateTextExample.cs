using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Networking;
using TMPro;
using Newtonsoft.Json.Linq; 
using UnityEngine.UI;

public class GenerateTextExample : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI TextGenerated;
    [SerializeField] private TMP_InputField inputText;
    [SerializeField] private Button GenerateTextButton;

    private string token = "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";
    private string url = "https://api-inference.huggingface.co/models/google/gemma-2-2b-it";

    private void Awake()
    {
        GenerateTextButton.onClick.AddListener(OnButtonClick_GenerateText);
    }

    private void OnDestroy()
    {
        GenerateTextButton.onClick.RemoveListener(OnButtonClick_GenerateText);
    }

    public async Task GenerateText()
    {
        try
        {
            string input = inputText.text;
            Debug.Log(input);

            GenerateTextButton.interactable = false;

            // Create the JSON payload
            string jsonPayload = $"{{\"inputs\": \"{input}\"}}";
            byte[] jsonPayloadBytes = System.Text.Encoding.UTF8.GetBytes(jsonPayload);

            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(jsonPayloadBytes);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", $"Bearer {token}");

                await request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Error: {request.error}");
                    TextGenerated.text = "Failed to fetch text";
                }
                else
                {
                    string responseText = request.downloadHandler.text;
                    // Debug.Log("API Response: " + responseText);

                    // Parse the JSON response
                    JArray jsonArray = JArray.Parse(responseText);
                    // Debug.Log("Json Array " + jsonArray);
                    if (jsonArray.Count > 0)
                    {
                        // Extract the "generated_text" field from the first item in the array
                        string generatedText = jsonArray[0]["generated_text"]?.ToString();
                        
                        if (!string.IsNullOrEmpty(generatedText))
                        {
                            
                            int newlineIndex = generatedText.IndexOf("\n");
        
                            if (newlineIndex != -1)
                            {
                                string result = generatedText.Substring(newlineIndex + 1);
                                // Debug.Log(result);
                                TextGenerated.text = result;

                            }
                            else
                            {
                                TextGenerated.text = generatedText;
                            }
                                    
                        
                        }
                        else
                        {
                            TextGenerated.text = "No generated text found in response.";
                        }
                    }
                    else
                    {
                        TextGenerated.text = "Empty response from API.";
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error fetching text: {e.Message}");
            TextGenerated.text = "Failed to fetch text";
        }
        finally
        {
            GenerateTextButton.interactable = true;
        }
    }

    public void OnButtonClick_GenerateText()
    {
        _ = GenerateText();
    }
}