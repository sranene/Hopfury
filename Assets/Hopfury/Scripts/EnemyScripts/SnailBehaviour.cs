using UnityEngine;
using System.Collections;

public class SnailBehaviour : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1.4f;
    [SerializeField] private Sprite sprite1;
    [SerializeField] private Sprite sprite2;
    [SerializeField] private Sprite deadSprite;

    private SpriteRenderer spriteRenderer;
    private float timer = 0f;
    private float timeToSwitch = 0.5f;
    private bool usingSprite1 = true;
    private bool isDead = false;
    private bool isVisible = false; // <- visibilidade controlada aqui
    private bool coroutineStarted = false;
    private bool isMovingTutorial = true;

    private AudioSource deathEnemySound;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        deathEnemySound = GameObject.Find("DeathEnemySound").GetComponent<AudioSource>();

    }

    IEnumerator StopAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        isMovingTutorial = false; // <- Aqui é onde o caracol realmente para
    }


    void Update()
    {
        if (!isVisible || isDead) return;

        if (isMovingTutorial)
        {
            transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
        }

        timer += Time.deltaTime;
        if (timer >= timeToSwitch)
        {
            timer = 0f;
            usingSprite1 = !usingSprite1;
            spriteRenderer.sprite = usingSprite1 ? sprite1 : sprite2;
        }
    }

    void OnBecameVisible()
    {
        isVisible = true;

        if (!coroutineStarted && Mathf.Approximately(moveSpeed, 0.9f))
        {
            coroutineStarted = true;
            StartCoroutine(StopAfterSeconds(5f)); // só começa após estar visível
        }
    }

    void OnBecameInvisible()
    {
        isVisible = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        if (other.CompareTag("Fireball"))
        {
            Die();
        }
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

    private void Die()
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
