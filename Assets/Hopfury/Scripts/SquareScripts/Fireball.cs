using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 20f;            // Velocidade da fireball
    public float lifetime = 5f;          // Tempo antes de ser destru�da automaticamente
    public float rotationSpeed = 90f;
    private Vector2 direction = Vector2.right; // Dire��o do movimento

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.velocity = direction * speed;
        }

        Destroy(gameObject, lifetime); // Autodestrui��o
    }

    private void Update()
    {
        // Roda no sentido dos ponteiros do rel�gio (negativo no eixo Z)
        transform.Rotate(0f, 0f, -rotationSpeed * Time.deltaTime);
    }

    // Chamada pelo BallControl para lan�ar a fireball
    public void Launch(Vector2 dir)
    {
        direction = dir.normalized;
        if (rb != null)
        {
            rb.velocity = direction * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        GameSessionManager.Instance.LogToFile("Fireball atingiu: " + col.name);

        if (col.CompareTag("Enemy") || col.CompareTag("Ground") || col.CompareTag("Deadly"))
        {
            Destroy(gameObject);
        }
    }


    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

}
