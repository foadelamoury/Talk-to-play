using UnityEngine;
using UnityEngine.UI;

public class AttackSettings : MonoBehaviour
{
    public AttackingInPosition attackInPos;
    public SkillsFunctions skills;
    public static AttackSettings instance;
    public CommandToGameCharacters commandProcessor;
    public InputField playerInput;
    public RectTransform rectTransform;

    private void Awake()
    {
        instance = this;
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        if (playerInput != null)
        {
            playerInput.onSubmit.AddListener(ProcessCommand);
        }
    }

    public void ProcessCommand(string command)
    {
        if (string.IsNullOrEmpty(command)) return;

        command = command.ToLower();
        if (commandProcessor != null)
        {
            commandProcessor.onInputFieldSubmit(command);
        }
        else
        {
            Debug.LogError("Command processor not assigned!");
        }
    }
}
