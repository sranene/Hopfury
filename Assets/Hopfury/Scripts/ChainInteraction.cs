using UnityEngine;

public class ChainInteraction : MonoBehaviour
{
    private bool isSwiping = false;
    private Vector2 swipeStartPos;
    private Vector2 swipeEndPos;
    private bool swipePassedOverChain = false;
    private bool chainWasCut = false;

    private Collider2D chainCollider;

    void Start()
    {
        chainCollider = GetComponent<Collider2D>();
        Debug.Log("Collider do chain inicializado: " + chainCollider);
    }

    void Update()
    {
        if (!GameManager.Instance.IsInChallengeMode() || chainWasCut)
        {
            return;
        } else {
            HandleMouseInput();
            //HandleTouchInput();
        }

    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            swipeStartPos = Input.mousePosition;
            isSwiping = true;
            swipePassedOverChain = chainCollider.OverlapPoint(Camera.main.ScreenToWorldPoint(swipeStartPos));
            Debug.Log("Início do swipe (mouse): " + swipeStartPos);
        }
        else if (Input.GetMouseButton(0) && isSwiping)
        {
            Debug.Log("SWIPING");
            Vector2 currentPos = Input.mousePosition;
            if (chainCollider.OverlapPoint(Camera.main.ScreenToWorldPoint(currentPos)))
            {
                Debug.Log("SWIPING touched chain");
                swipePassedOverChain = true;
            }
        }
        else if (Input.GetMouseButtonUp(0) && isSwiping)
        {
            Debug.Log("Fim do swipe");
            swipeEndPos = Input.mousePosition;
            EndSwipe();
        }
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);
        Vector2 touchPosition = touch.position;

        switch (touch.phase)
        {
            case TouchPhase.Began:
                swipeStartPos = touchPosition;
                isSwiping = true;
                swipePassedOverChain = chainCollider.OverlapPoint(Camera.main.ScreenToWorldPoint(touchPosition));
                Debug.Log("Início do swipe (touch): " + swipeStartPos);
                break;

            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                if (chainCollider.OverlapPoint(Camera.main.ScreenToWorldPoint(touchPosition)))
                {
                    swipePassedOverChain = true;
                }
                break;

            case TouchPhase.Ended:
                if (isSwiping)
                {
                    swipeEndPos = touchPosition;
                    EndSwipe();
                }
                break;
        }
    }

    private void EndSwipe()
    {
        isSwiping = false;

        if (!swipePassedOverChain)
        {
            Debug.Log("Swipe não passou pela chain.");
            return;
        }

        Vector2 swipeDirection = swipeEndPos - swipeStartPos;
        Debug.Log("Direção do swipe: " + swipeDirection);

        if (IsSwipePerpendicularToChain(swipeDirection))
        {
            Debug.Log("Swipe na direção certa!");
            // Acesse o componente KeyMovement do GameObject que foi arrastado
            GameObject key = GameObject.FindWithTag("Key");
            KeyMovement keyMovement = key.GetComponent<KeyMovement>();
            keyMovement.Release();

            chainWasCut = true;
        }
        else
        {
            Debug.Log("Swipe não está perpendicular à chain.");
        }
    }

    private bool IsSwipePerpendicularToChain(Vector2 swipeDirection)
    {
        // Corrigir swipeDirection para o mundo (considerando rotação da câmera)
        Vector3 swipeDirectionWorld = Camera.main.transform.TransformDirection(new Vector3(swipeDirection.x, swipeDirection.y, 0));
        Vector2 correctedSwipeDirection = new Vector2(swipeDirectionWorld.x, swipeDirectionWorld.y).normalized;

        // Direção da corrente (chain) é para cima no mundo
        Vector2 chainDirection = new Vector2(0, 1);

        float angle = Vector2.Angle(correctedSwipeDirection, chainDirection);

        Debug.Log("Ângulo entre swipe e chain: " + angle);

        return Mathf.Abs(angle - 90f) < 45f; // Ainda aceitando ângulos entre 45° e 135°
    }

}
