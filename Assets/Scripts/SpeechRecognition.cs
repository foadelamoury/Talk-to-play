using System.IO;
using HuggingFace.API;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq; 

public class SpeechRecognitionTest : MonoBehaviour {
    [SerializeField] private Button startButton;
    [SerializeField] private Button stopButton;
    [SerializeField] private InputField playerText;

    private TextMeshProUGUI outputText;
    private string token = "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";
    private string url = "https://api-inference.huggingface.co/models/google/gemma-2-2b-it";

    private AudioClip clip;
    private byte[] bytes;
    private bool recording;

    private void Start() {
        startButton.onClick.AddListener(StartRecording);
        stopButton.onClick.AddListener(StopRecording);
        stopButton.interactable = false;
       outputText = playerText.GetComponentInChildren<TextMeshProUGUI>();
      Debug.Log("Mesh Pro"+ outputText);
    }

    private void Update() {
        if (recording && Microphone.GetPosition(null) >= clip.samples) {
            StopRecording();
        }
    }

    private void StartRecording() {
        outputText.text = "Recording...";
        startButton.interactable = false;
        stopButton.interactable = true;
        clip = Microphone.Start(null, false, 10, 44100);
        recording = true;
    }

    private void StopRecording() {
        outputText.text = "";
        var position = Microphone.GetPosition(null);
        Microphone.End(null);
        var samples = new float[position * clip.channels];
        clip.GetData(samples, 0);
        bytes = EncodeAsWAV(samples, clip.frequency, clip.channels);
        recording = false;
        File.WriteAllBytes(Application.dataPath + "/test.wav", bytes);
        SendRecording();
    }

    private void SendRecording() {
        outputText.text = "Sending...";
        stopButton.interactable = false;
        HuggingFaceAPI.AutomaticSpeechRecognition(bytes, response => {
            outputText.text = response;
            playerText.text = response;  // Set the text in the InputField
            playerText.onSubmit.Invoke(response);  // Manually trigger the submit event
            startButton.interactable = true;
        }, error => {
            outputText.text = error;
            startButton.interactable = true;
        });
    }

    private byte[] EncodeAsWAV(float[] samples, int frequency, int channels) 
    {
        using (var memoryStream = new MemoryStream(44 + samples.Length * 2)) {
            using (var writer = new BinaryWriter(memoryStream)) {
                writer.Write("RIFF".ToCharArray());
                writer.Write(36 + samples.Length * 2);
                writer.Write("WAVE".ToCharArray());
                writer.Write("fmt ".ToCharArray());
                writer.Write(16);
                writer.Write((ushort)1);
                writer.Write((ushort)channels);
                writer.Write(frequency);
                writer.Write(frequency * channels * 2);
                writer.Write((ushort)(channels * 2));
                writer.Write((ushort)16);
                writer.Write("data".ToCharArray());
                writer.Write(samples.Length * 2);

                foreach (var sample in samples) {
                    writer.Write((short)(sample * short.MaxValue));
                }
            }
            return memoryStream.ToArray();
        }
    }
    // public void AnalyzeText(string responseAudio)
    // {
    //      string jsonPayload = $"{{\"inputs\": \"{responseAudio}\"}}";
    //     byte[] jsonPayloadBytes = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
    //     HuggingFaceAPI.TextGeneration(jsonPayload, response => {
    //     text.color = Color.white;
    //     text.text = response;
    //     //  JArray jsonArray = JArray.Parse(response);
    //     //  string generatedText = jsonArray[0]["generated_text"]?.ToString();
    //     //      int newlineIndex = generatedText.IndexOf("\n");
        
    //     //     if (newlineIndex != -1)
    //     //     {
    //     //         string result = generatedText.Substring(newlineIndex + 1);
    //     //         // Debug.Log(result);
    //     //         text.text = result;

    //     //     }
    //     //     else
    //     //     {
    //     //         text.text = generatedText;
    //     //     }

    // }, error => {
    //     text.color = Color.red;
    //     text.text = error;
    // });
    // }
    public async Task GenerateText()
    {
        try
        {

            string input = outputText.text;
            Debug.Log(input);

            

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
                    outputText.text = "Failed to fetch text";
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
                                outputText.text = result;

                            }
                            else
                            {
                                outputText.text = generatedText;
                            }
                                    
                        
                        }
                        else
                        {
                            outputText.text = "No generated text found in response.";
                        }
                    }
                    else
                    {
                        outputText.text = "Empty response from API.";
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error fetching text: {e.Message}");
            outputText.text = "Failed to fetch text";
        }
        finally
        {
            startButton.interactable = true;
        }
    }
}
