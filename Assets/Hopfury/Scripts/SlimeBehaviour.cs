using UnityEngine;
using System.Collections;

public class SlimeBehaviour : MonoBehaviour
{
    [SerializeField] private float moveDistance = 1f;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float stopDuration = 0.7f;
    [SerializeField] private Sprite sprite1;
    [SerializeField] private Sprite sprite2;
    [SerializeField] private Sprite deadSprite;

    private SpriteRenderer spriteRenderer;
    private float movedDistance = 0f;
    private bool isMoving = true;
    private float stopTimer = 0f;
    private float spriteTimer = 0f;
    private float spriteSwitchInterval = 0.5f;
    private bool usingSprite1 = true;
    private bool isDead = false;
    private bool isVisible = false; // <- Visibilidade controlada

    private AudioSource deathEnemySound;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        deathEnemySound = GameObject.Find("DeathEnemySound").GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!isVisible || isDead) return;

        if (isMoving)
        {
            float moveStep = moveSpeed * Time.deltaTime;
            transform.Translate(Vector2.left * moveStep);
            movedDistance += moveStep;

            spriteTimer += Time.deltaTime;
            if (spriteTimer >= spriteSwitchInterval)
            {
                spriteTimer = 0f;
                usingSprite1 = !usingSprite1;
                spriteRenderer.sprite = usingSprite1 ? sprite1 : sprite2;
            }

            if (movedDistance >= moveDistance)
            {
                isMoving = false;
                movedDistance = 0f;
            }
        }
        else
        {
            stopTimer += Time.deltaTime;
            if (stopTimer >= stopDuration)
            {
                stopTimer = 0f;
                isMoving = true;
            }
        }
    }

    void OnBecameVisible()
    {
        isVisible = true;
    }

    void OnBecameInvisible()
    {
        isVisible = false;
    }

    private IEnumerator FadeOutAndDestroy(float duration)
    {
        deathEnemySound.Play();
        float elapsed = 0f;
        Color originalColor = spriteRenderer.color;

        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            elapsed += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        if (other.CompareTag("Fireball"))
        {
            isDead = true;

            foreach (Collider2D col in GetComponents<Collider2D>())
            {
                if (!col.isTrigger)
                    col.enabled = false;
            }

            spriteRenderer.sprite = deadSprite;
            gameObject.layer = 0;
            gameObject.tag = "Untagged";
            StartCoroutine(FadeOutAndDestroy(1f));
        }
    }
}
