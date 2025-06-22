using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public Sprite newTopSprite;     // Novo sprite para o "top"
    public Sprite newBottomSprite;  // Novo sprite para o "bottom"

    // Método para alterar os sprites da porta
    public void ChangeSprite()
    {
        // Acessa o GameObject "top" (se tiver esse nome na hierarquia)
        Transform top = transform.Find("Top");  // Substitua "top" pelo nome real se necessário
        if (top != null)
        {
            SpriteRenderer topRenderer = top.GetComponent<SpriteRenderer>();
            if (topRenderer != null)
            {
                topRenderer.sprite = newTopSprite; // Altera o sprite do top
            }
            else
            {
                Debug.LogWarning("Não há SpriteRenderer no 'top'!");
            }
        }
        else
        {
            Debug.LogWarning("Não foi encontrado o GameObject 'top'!");
        }

        // Acessa o GameObject "bottom" (se tiver esse nome na hierarquia)
        Transform bottom = transform.Find("Bottom");  // Substitua "bottom" pelo nome real se necessário
        if (bottom != null)
        {
            SpriteRenderer bottomRenderer = bottom.GetComponent<SpriteRenderer>();
            if (bottomRenderer != null)
            {
                bottomRenderer.sprite = newBottomSprite; // Altera o sprite do bottom
            }
            else
            {
                Debug.LogWarning("Não há SpriteRenderer no 'bottom'!");
            }
        }
        else
        {
            Debug.LogWarning("Não foi encontrado o GameObject 'bottom'!");
        }
    }
}
