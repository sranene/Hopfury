using UnityEngine;

public class BoosterTrigger : MonoBehaviour
{
    [SerializeField] private Sprite newTopSprite;
    [SerializeField] private Sprite newBottomSprite;

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            // Acessar os filhos
            Transform top = transform.Find("SpringBoard");
            Transform bottom = transform.Find("Box");

            if (top != null && bottom != null)
            {
                // Mudar o sprite de cada filho
                top.GetComponent<SpriteRenderer>().sprite = newTopSprite;
                bottom.GetComponent<SpriteRenderer>().sprite = newBottomSprite;
            }
        }
    }
}
