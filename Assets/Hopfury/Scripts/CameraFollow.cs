using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject player; // O objeto que a câmera vai seguir
    private Vector3 velocity = Vector3.zero;

    [SerializeField] private float cameraHeightOffset = -3f; // Ajuste a altura da câmera em relação ao jogador
    [SerializeField] private float xOffset = 4f; // Offset no eixo X para que a câmera fique mais à esquerda do jogador
    [SerializeField] private float tiltAngle = -70f; // Ângulo de rotação invertido (agora mais inclinado e no sentido oposto)

    [SerializeField] private float zoomOutAmount = 12f; // Valor do zoom (tamanho ortográfico da câmera)
    private Camera cameraComponent;

    private float minX; // A posição mínima do eixo X onde a câmera pode seguir o jogador

    void Start()
    {
        cameraComponent = GetComponent<Camera>(); // Pega o componente Camera do objeto

        // Inicializa minX como a posição inicial do jogador no eixo X com o offset
        if (player != null)
        {
            minX = player.transform.position.x + xOffset;
        }
    }

    void Update()
    {
        if (player == null) return;

        // Se a câmera for ortográfica, altere o tamanho ortográfico (zoom)
        if (cameraComponent.orthographic)
        {
            cameraComponent.orthographicSize = Mathf.Lerp(cameraComponent.orthographicSize, zoomOutAmount, Time.deltaTime * 5f);
        }

        // Verifica se o jogador passou do limite
        if (player.transform.position.x >= 295f)
        {
            // Trava a câmera e não atualiza mais a posição
            return;
        }

        // Se o jogador voltou ao início (e.g., morreu e recomeçou)
        if (player.transform.position.x + xOffset < minX - 4f)
        {
            minX = player.transform.position.x + xOffset;
        }

        // Atualiza minX conforme o jogador se move para a direita
        minX = Mathf.Max(minX, player.transform.position.x + xOffset);

        // Calcula a posição do jogador no eixo X e aplica o offset
        float targetX = player.transform.position.x + xOffset;  // Adiciona o offset para mover a câmera para a esquerda do jogador

        // Só começa a mover a câmera para a direita se o jogador atingir a posição mínima no eixo X
        targetX = Mathf.Max(targetX, minX);

        // Controla a posição Y da câmera, mantendo-a fixa em relação ao chão
        float targetY = cameraHeightOffset; // Limita o movimento vertical da câmera

        // Define a posição final da câmera
        Vector3 targetPosition = new Vector3(targetX, targetY, -10f); // A posição Z fixada para evitar distorções

        // Move a câmera suavemente para a nova posição
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 0.12f);

        // Aplica a rotação fixa no eixo Z
        transform.rotation = Quaternion.Euler(0, 0, tiltAngle);
    }
}
