using UnityEngine;
using System.Collections;

public class FlyBehaviour : MonoBehaviour
{
    [SerializeField] private float moveDistance = 0.5f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Sprite sprite1;
    [SerializeField] private Sprite sprite2;
    [SerializeField] private Sprite deadSprite;

    private Vector3 startPosition;
    private SpriteRenderer spriteRenderer;
    private float timer = 0f;
    private float timeToSwitch = 0.3f;

    private bool isDead = false;
    private float fallSpeed = 0f;
    private float gravity = -2f;

    private AudioSource deathEnemySound;

    private bool isVisible = false; // <- controla visibilidade

    void Start()
    {
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        deathEnemySound = GameObject.Find("DeathEnemySound").GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!isVisible) return; // <- só executa se estiver visível
        if (isDead)
        {
            fallSpeed += gravity * Time.deltaTime;
            transform.Translate(new Vector3(0, fallSpeed * Time.deltaTime, 0));
            return;
        }

        float newY = Mathf.Sin(Time.time * moveSpeed) * moveDistance + startPosition.y;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        timer += Time.deltaTime;
        if (timer >= timeToSwitch)
        {
            timer = 0f;
            spriteRenderer.sprite = (spriteRenderer.sprite == sprite1) ? sprite2 : sprite1;
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
