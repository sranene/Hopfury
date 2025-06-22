using UnityEngine;

public class RestStation : MonoBehaviour
{
    public GameObject leftBox;
    public GameObject rightBox;

    private SpriteRenderer leftSpriteRenderer;
    private SpriteRenderer rightSpriteRenderer;

    private void Start()
    {
        // Buscar os SpriteRenderers dos filhos
        leftSpriteRenderer = leftBox.GetComponent<SpriteRenderer>();
        rightSpriteRenderer = rightBox.GetComponent<SpriteRenderer>();
    }

    // Este m�todo ser� chamado pelos filhos quando houver colis�o
    public void SwapSprites()
    {
        Sprite temp = leftSpriteRenderer.sprite;
        leftSpriteRenderer.sprite = rightSpriteRenderer.sprite;
        rightSpriteRenderer.sprite = temp;
    }
}
