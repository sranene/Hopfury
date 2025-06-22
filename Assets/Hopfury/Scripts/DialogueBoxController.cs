using UnityEngine;
using TMPro;

public class DialogueBoxController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogueText;

    [TextArea(2, 4)]
    [SerializeField]
    private string[] dialogues = new string[]
    {
        "Ol�! Parece que est�s pronto para come�ar!",
        "Toca no ecr� para saltares por cima dos obst�culos.",
        "Desliza com o dedo para fazeres um dash r�pido.",
        "Cuidado! Alguns inimigos movem-se mais depressa.",
        "Consegues encontrar todos os segredos escondidos?",
        "Est�s quase l�! Continua!",
        "Parab�ns! Superaste este desafio com estilo!"
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
