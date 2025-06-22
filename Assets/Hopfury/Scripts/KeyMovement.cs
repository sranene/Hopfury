using UnityEngine;

public class KeyMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isDragging = false; // Para controlar se a chave está sendo arrastada
    private Vector3 offset; // Para armazenar a diferença entre o toque e a posição da chave
    private Camera cam; // Para obter a posição do mouse ou toque na tela
    private int touchId = -1; // Para identificar o toque específico
    private bool isFree = false;

    [SerializeField] private GameObject door;
    public GameObject lockObject;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true; // Impede de cair no início
        cam = Camera.main; // Para converter a posição do mouse/touch para a posição do mundo
    }

    private void Update()
    {
        if (!isFree)
        {
            return;
        }
        // Se o toque é detectado em dispositivos móveis
        /*if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); // Obtém o primeiro toque (pode adicionar lógica para múltiplos toques)

            // Caso o toque comece em cima da chave
            if (touch.phase == TouchPhase.Began && IsTouchOnKey(touch))
            {
                isDragging = true;
                touchId = touch.fingerId; // Salva o ID do toque para acompanhar
                offset = transform.position - GetTouchWorldPosition(touch); // Calcula a diferença entre a chave e o toque
            }
            // Se o toque está sendo arrastado
            else if (touch.phase == TouchPhase.Moved && touch.fingerId == touchId && isDragging)
            {
                transform.position = GetTouchWorldPosition(touch) + offset; // Faz a chave seguir o toque
            }
            // Se o toque foi solto
            else if (touch.phase == TouchPhase.Ended && touch.fingerId == touchId)
            {
                isDragging = false; // Para de arrastar
                touchId = -1; // Reseta o ID do toque
            }
        }
        else // Para o caso de não estar em dispositivos móveis (ou seja, no PC)
        {
            */
            // Detecção de clique do mouse
            if (Input.GetMouseButtonDown(0) && IsMouseOnKey())
            {
                isDragging = true;
                offset = transform.position - GetMouseWorldPosition(); // Calcula a diferença entre o mouse e a posição da chave
            }
            else if (Input.GetMouseButton(0) && isDragging)
            {
                transform.position = GetMouseWorldPosition() + offset; // Faz a chave seguir o mouse
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isDragging = false; // Para de arrastar
            }
        //}
    }

    // Verifica se o toque foi feito em cima da chave
    private bool IsTouchOnKey(Touch touch)
    {
        Vector3 touchPosition = GetTouchWorldPosition(touch);
        Collider2D keyCollider = GetComponent<Collider2D>();
        return keyCollider.OverlapPoint(touchPosition); // Verifica se o toque está dentro do collider da chave
    }

    // Verifica se o mouse foi feito em cima da chave
    private bool IsMouseOnKey()
    {
        Vector3 mousePosition = GetMouseWorldPosition();
        Collider2D keyCollider = GetComponent<Collider2D>();
        return keyCollider.OverlapPoint(mousePosition); // Verifica se o mouse está dentro do collider da chave
    }

    // Método para obter a posição do mouse na tela e converter para a posição do mundo
    private Vector3 GetMouseWorldPosition()
    {
        return cam.ScreenToWorldPoint(Input.mousePosition);
    }

    // Método para obter a posição do toque na tela e converter para a posição do mundo
    private Vector3 GetTouchWorldPosition(Touch touch)
    {
        return cam.ScreenToWorldPoint(touch.position);
    }

    // Lógica de "Release" quando o jogador interage com a chave
    public void Release()
    {
        isFree = true;
        Debug.Log("Chave a cair");
        rb.isKinematic = false; // Deixa a chave cair
    }

    // Detectar colisão com o Lock
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Chave colidiu com {other.gameObject.name}");
        if (other.CompareTag("Lock")) // Verifica se colidiu com o Lock
        {
            // Adicione o que acontece quando a chave entra no Lock
            Debug.Log("Chave colidiu com o Lock!");

            // 1. Destruir o Lock
            Destroy(lockObject);

            // 2. Alterar os sprites da Door
            door.GetComponent<DoorTrigger>().ChangeSprite();

            // 3. End do Challenge Mode
            GameManager.Instance.ExitChallengeMode();

            // 4. Destruir a chave
            Destroy(gameObject);
            // Aqui você pode executar a ação desejada, como "travar" a chave ou outra coisa
        }
    }
}
