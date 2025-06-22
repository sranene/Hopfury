using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    private Image imageComponent;
    private Coroutine currentCoroutine;

    void Awake()
    {
        imageComponent = GetComponent<Image>();
    }

    void OnEnable()
    {
        if (sprites.Length > 0 && imageComponent != null)
        {
            currentCoroutine = StartCoroutine(ChangeSpriteLoop());
        }
    }

    void OnDisable()
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
    }

    IEnumerator ChangeSpriteLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0.8f, 1.5f));
            int index = Random.Range(0, sprites.Length);
            imageComponent.sprite = sprites[index];
        }
    }
}
