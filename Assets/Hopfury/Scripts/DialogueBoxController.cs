using UnityEngine;
using TMPro;

public class DialogueBoxController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogueText;

    [TextArea(2, 4)]
    [SerializeField]
    private string[] dialogues = new string[]
    {
        "Olá! Parece que estás pronto para começar!",
        "Toca no ecrã para saltares por cima dos obstáculos.",
        "Desliza com o dedo para fazeres um dash rápido.",
        "Cuidado! Alguns inimigos movem-se mais depressa.",
        "Consegues encontrar todos os segredos escondidos?",
        "Estás quase lá! Continua!",
        "Parabéns! Superaste este desafio com estilo!"
    };

    private void Awake()
    {
        if (dialogueText == null)
        {
            Debug.LogWarning("Dialogue Text not assigned!");
        }
    }

    public void ShowDialogue(int index)
    {
        if (index >= 0 && index < dialogues.Length)
        {
            dialogueText.text = dialogues[index];
        }
        else
        {
            Debug.LogWarning("Dialogue index out of range!");
        }
    }

    public void HideDialogue()
    {
        dialogueText.text = "";
    }
}
