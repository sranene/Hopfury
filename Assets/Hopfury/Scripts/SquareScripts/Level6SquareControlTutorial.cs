using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using FingerNew = UnityEngine.InputSystem.EnhancedTouch.Finger;

public class Level6SquareControlTutorial : MonoBehaviour
{

    [SerializeField]
    private Rigidbody2D rb = null;
    [SerializeField]
    private BoxCollider2D boxCollider = null;
    [SerializeField]
    private ParticleSystem jumpParticle = null; // Simple particle effect when player jumps
    [SerializeField]
    private ParticleSystem dustTrail = null; // Simple particle effect when player walks
    [SerializeField]
    private ParticleSystem deathParticle = null; // Simple particle effect when player dies
    [SerializeField]
    private ParticleSystem rechargeFireParticle = null; // Simple particle effect when player recharges fireball
    [SerializeField]
    private float moveSpeed = 8f; // Speed of the ball's movement
    [SerializeField]
    private GameObject fireballPrefab;
    [SerializeField]
    private Transform firePoint;

    [SerializeField] private Sprite blockerHappy;
    [SerializeField] private Sprite blockerMad;
    [SerializeField] private Sprite blockerJump;
    [SerializeField] private Sprite blockerDead;


    private SpriteRenderer spriteRenderer;

    private bool canJump = true; // Ball needs to be grounded to be able to jump
    private bool canCollide = true; // If the value of this variable is false, the ball will be able to pass through the platform.
    private bool shouldBoost;
    private bool moveRight = true; // Começa a mover para a direita
    private bool isJumping = false;

    private AudioSource jumpSound;
    private AudioSource hitSound;
    private AudioSource fireballSound;

    //private bool isWaitingForGesture = false;
    private float gestureStartTime;
    private bool fireballOnCooldown = false;
    public float swipeThreshold = 50f;
    public float fireballCooldown = 1f;
    private Vector2 touchStartPos;
    private Vector2 touchEndPos;

    //private bool isRotating = false; // Indica se a rotação está ativa
    //private float targetRotation = -180f; // A rotação alvo (90 graus)
    //private float rotationSpeed = 8f; // A velocidade de rotação (ajuste conforme necessário)
    private List<Vector2> earlyTouchPositions = new List<Vector2>();
    private const int requiredPoints = 3;
    private bool gestureIsSwipeCandidate = false;
    private bool alreadyJumpedThisTouch = false;
    private float startTouchTime;
    private Coroutine tapTimeoutCoroutine;

    private float tapStartTime; // Timestamp para o início do toque
    private float tapEndTime;   // Timestamp para o fim do toque
    private float squareXAtTap;
    private float squareYAtTap;
    private int tapId = 0; // Contador de IDs de toques
    private float landedTime;
    private bool isTap = true;

    private bool waitingForJump = false;
    private bool waitingForTapToSkip = false;
    private bool waitingForSwipe = false;
    private bool waitingtest = false;

    private bool firstSwipe = false;
    private bool waitingForFirstReset = false;

    bool isDead = false;

    int counter = 8;

    private void Awake()
    {
        EnhancedTouchSupport.Enable();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = blockerHappy;
    }

    private void OnEnable()
    {
        GameSessionManager.Instance.LogToFile($"on enable ball control");
    }

    private void OnDisable()
    {
        GameSessionManager.Instance.LogToFile($"on disable ball control");
    }


    void Start()
    {
        GameSessionManager.Instance.SetElapsedTime();
        ResetTutorialState();
        jumpSound = GameObject.Find("JumpSound").GetComponent<AudioSource>();
        hitSound = GameObject.Find("HitSound").GetComponent<AudioSource>();
        fireballSound = GameObject.Find("FireballSound").GetComponent<AudioSource>();
        SetBallVelocity();
        StartCoroutine(StartTutorial());
    }

    public void ResetTutorialState()
    {
        fireballOnCooldown = false;
        firstSwipe = true;
        canJump = true;
        isJumping = false;

        if (!isDead)
        {
            spriteRenderer.sprite = blockerHappy;
        }
    }


    void SetBallVelocity()
    {
        GameSessionManager.Instance.LogToFile("continue to walk");
        if (moveRight)
            rb.velocity = new Vector2(Mathf.Abs(moveSpeed), rb.velocity.y);
        else
            rb.velocity = new Vector2(-Mathf.Abs(moveSpeed), rb.velocity.y);
    }

    void StopSquareVelocity()
    {
        rb.velocity = new Vector2(Mathf.Abs(0), rb.velocity.y);
        GameSessionManager.Instance.LogToFile("stopping");
    }
    
    private void ReverseDirection()
    {
        moveRight = !moveRight; // Inverte a direção
        SetBallVelocity();      // Atualiza a velocidade com a nova direção

        if (moveRight)
            canJump = true;
        else
            canJump = false;
    }

    IEnumerator StartTutorial()
    {
        yield return new WaitForSeconds(1f);
        TriggerTapDialogueStep(false);
    }

    IEnumerator EndTutorial()
    {
        TriggerTapDialogueStep(false);
        yield return new WaitForSeconds(2f);
        firstSwipe = false;
        HideTutorialUI();
        GameObject.Find("GameManager").GetComponent<Menus>().LevelComplete();
        GetComponent<Level0SquareControlTutorial>().enabled = false;
        GameObject.Find("LevelCompleteSound").GetComponent<AudioSource>().Play();
    }

    private void TriggerTapDialogueStep(bool withJump)
    {
        StopSquareVelocity();
        Menus menus = GameObject.Find("GameManager").GetComponent<Menus>();
        if (withJump)
        {
            waitingForJump = true;
            if (menus != null)
            {
                menus.ShowDialogue(counter);
                menus.ShowTapHint();
            }
        } else
        {
            waitingForTapToSkip = true;
            if (menus != null)
            {
                menus.ShowDialogue(counter);
            }
        }
        counter++;
    }

    private void TriggerSwipeDialogueStep()
    {
        waitingForSwipe = true;
        StopSquareVelocity();
        Menus menus = GameObject.Find("GameManager").GetComponent<Menus>();
        if (menus != null)
        {
            menus.ShowDialogue(counter);
            menus.ShowSwipeHint();
        }
        counter++;
    }

    private void HideTutorialUI()
    {
        SetBallVelocity();
        waitingForJump = false;
        waitingForTapToSkip = false;
        waitingForSwipe = false;

        Menus menus = GameObject.Find("GameManager").GetComponent<Menus>();
        if (menus != null)
        {
            menus.HideDialogue();
            menus.HideAllHints();
        }
    }


    void FixedUpdate()
    {

        if (Mathf.Abs(rb.position.x - 23) < 0.05f && counter == 9)
        {
            TriggerSwipeDialogueStep();
            firstSwipe = true;
        }
        else if (Mathf.Abs(rb.position.x - 48) < 0.05f && counter == 11)
        {
            TriggerSwipeDialogueStep();
        }
        else if (Mathf.Abs(rb.position.x - 73) < 0.05f && counter == 12)
        {
            TriggerSwipeDialogueStep();
        }
        else if (Mathf.Abs(rb.position.x - 95) < 0.05f && counter == 13)
        {
            TriggerTapDialogueStep(false);
        }

    }

    void Update()
    {
        HandleTouchInput();

        if (rb.velocity.y < 0 && canCollide)
        {
            boxCollider.isTrigger = false;
        }
    }

    private IEnumerator StaticTapTimeout()
    {
        yield return new WaitForSeconds(0.3f);

        if (!gestureIsSwipeCandidate && !alreadyJumpedThisTouch && waitingForJump)
        {
            GameSessionManager.Instance.LogToFile("TAP ESTÁTICO DETETADO APÓS 0.3s → SALTAR");
            OnTapGesture();
            alreadyJumpedThisTouch = true;
        }
    }


    private void HandleTouchInput()
    {
        if (UnityEngine.Input.touchCount > 0)
        {
            UnityEngine.Touch touch = UnityEngine.Input.GetTouch(0);

            if (touch.phase == UnityEngine.TouchPhase.Began)
            {
                earlyTouchPositions.Clear();
                earlyTouchPositions.Add(touch.position);
                gestureIsSwipeCandidate = false;
                alreadyJumpedThisTouch = false;
                startTouchTime = Time.time;

                GameSessionManager.Instance.LogToFile("TOQUE COMEÇADO");

                // Iniciar coroutine para tap estático (tap sem mover)
                if (tapTimeoutCoroutine != null) StopCoroutine(tapTimeoutCoroutine);
                tapTimeoutCoroutine = StartCoroutine(StaticTapTimeout());
            }
            else if (touch.phase == UnityEngine.TouchPhase.Moved)
            {
                if (earlyTouchPositions.Count < requiredPoints)
                {
                    earlyTouchPositions.Add(touch.position);

                    if (earlyTouchPositions.Count == requiredPoints)
                    {
                        Vector2 delta = earlyTouchPositions[2] - earlyTouchPositions[0];
                        float distance = delta.magnitude;
                        float duration = Time.time - startTouchTime;
                        float speed = distance / duration;

                        bool isFast = speed > 500f;
                        bool isVertical = Mathf.Abs(delta.y) > Mathf.Abs(delta.x);
                        bool isUpward = delta.y > 0;

                        if (isFast && isVertical && isUpward)
                        {
                            gestureIsSwipeCandidate = true;
                            GameSessionManager.Instance.LogToFile($"POTENCIAL SWIPE DETETADO (speed={speed:F1}) → AGUARDAR FIM");
                        }
                        else if (duration < 0.5f && speed < 200f && !alreadyJumpedThisTouch)
                        {
                            // TAP identificado com pouco movimento
                            GameSessionManager.Instance.LogToFile($"TAP DETETADO COM POUCA VELOCIDADE (speed={speed:F1}) → SALTAR JÁ");
                            OnTapGesture();
                            isTap = true;
                            alreadyJumpedThisTouch = true;
                        }
                    }
                }
            }
            else if (touch.phase == UnityEngine.TouchPhase.Ended)
            {
                if (tapTimeoutCoroutine != null)
                {
                    StopCoroutine(tapTimeoutCoroutine);
                    tapTimeoutCoroutine = null;
                }

                Vector2 endPos = touch.position;

                if (gestureIsSwipeCandidate)
                {
                    Vector2 swipeVector = endPos - earlyTouchPositions[0];

                    if (swipeVector.y > Mathf.Abs(swipeVector.x) && swipeVector.y > 50f && waitingForSwipe)
                    {
                        GameSessionManager.Instance.LogToFile("SWIPE PARA CIMA CONFIRMADO → LANÇAR FIREBALL");
                        OnSwipeUpGesture();
                    }
                    else //probably nao chega aqui
                    {
                        GameSessionManager.Instance.LogToFile("SWIPE MAL DIRECIONADO NO FIM → IGNORAR");
                    }
                }
                else if (!alreadyJumpedThisTouch)
                {
                    // Só se não tiver sido já considerado TAP antes
                    Vector2 delta = endPos - earlyTouchPositions[0];
                    float distance = delta.magnitude;
                    float duration = Time.time - startTouchTime;
                    float speed = distance / duration;

                    if (duration < 0.5f && speed < 200f)
                    {
                        GameSessionManager.Instance.LogToFile($"TAP CURTO NO ENDED (speed={speed:F1}) → SALTAR");
                        OnTapGesture();
                    }
                    else //probably nao chega aqui
                    {
                        GameSessionManager.Instance.LogToFile($"GESTO DEMASIADO LENTO OU LONGO (duration={duration:F2}, speed={speed:F1}) → IGNORAR");
                    }
                }
            }
        }
    }

    public void OnTapGesture()
    {
        if (waitingForFirstReset)
        {
            return;
        }

        if (waitingForTapToSkip)
        {
            if (fireballOnCooldown && firstSwipe)
            {
                Invoke(nameof(ResetFirstFireballCooldownPt2), 0.9f);
                firstSwipe = false;
            }
            HideTutorialUI();
            return;
        }

        if (waitingForSwipe)
            return;

        // Executa ações apenas se não estivermos à espera de um swipe
        SetBallVelocity();
        HideTutorialUI();

        isJumping = true;

        if (canJump)
        {
            Jump();
        }
    }

    public void OnSwipeUpGesture()
    {
        HideTutorialUI();
        SetBallVelocity();
        if (firstSwipe)
        {
            waitingForFirstReset = true;
            TryFirstLaunchFireball();
        } else
        {
            TryLaunchFireball();
        }
    }

    private void TryLaunchFireball()
    {
        if (fireballOnCooldown)
        {
            GameSessionManager.Instance.LogToFile("Fireball attempt failed: on cooldown.");
            return;
        }
        if (!isDead)
        {
            spriteRenderer.sprite = blockerMad;
        }

        GameObject fireballObj = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        fireballObj.GetComponent<Fireball>().Launch(Vector2.right); // ou ajusta consoante o movimento

        fireballOnCooldown = true;
        fireballSound.Play();

        Invoke(nameof(ResetFireballCooldown), fireballCooldown);

    }

    private void ResetFireballCooldown()
    {
        rechargeFireParticle.Play();
        fireballOnCooldown = false;
        if (!isDead)
        {
            spriteRenderer.sprite = blockerHappy;
        }
    }

    private void TryFirstLaunchFireball()
    {
        if (fireballOnCooldown)
        {
            GameSessionManager.Instance.LogToFile("Fireball attempt failed: on cooldown.");
            return;
        }
        if (!isDead)
        {
            spriteRenderer.sprite = blockerMad;
        }

        GameObject fireballObj = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        fireballObj.GetComponent<Fireball>().Launch(Vector2.right); // ou ajusta consoante o movimento

        fireballOnCooldown = true;
        fireballSound.Play();

        canJump = false;
        Invoke(nameof(ResetFirstFireballCooldown), 1.1f);
    }

    private void ResetFirstFireballCooldown()
    {
        waitingForFirstReset = false;
        canJump = true;
        if (counter == 10)
        {
            TriggerTapDialogueStep(false);

        }
    }

    private void ResetFirstFireballCooldownPt2()
    {
        rechargeFireParticle.Play();
        fireballOnCooldown = false;
        if (!isDead)
        {
            spriteRenderer.sprite = blockerHappy;
        }
    }

    private void Jump()
    {
        if (!isDead)
        {
            spriteRenderer.sprite = blockerJump;
        }
        dustTrail.Stop();
        GameSessionManager.Instance.LogToFile("comecou o salto");
        rb.velocity = new Vector2(rb.velocity.x, 0); // Reseta a velocidade no eixo Y, mas mantém a velocidade no eixo X
        canJump = false;

        boxCollider.isTrigger = true;
        rb.AddForce(new Vector2(0, 2450));

        jumpSound.Play();
        jumpParticle.Play();

    }

    void OnCollisionEnter2D(Collision2D col)
    {
        GameSessionManager.Instance.LogToFile($"SQUARE CONTROL colidiu com '{col.gameObject.name}");

        if (col.gameObject.CompareTag("Ground"))
        {
            // Agora verifica se o contato veio de cima ou de lado
            foreach (ContactPoint2D contact in col.contacts)
            {
                if (!isDead && contact.normal.y <= 0.5f) // Se a normal não for quase vertical, é de lado
                {
                    isDead = true;
                    StartCoroutine(DeathSequence());
                    break;
                }
                else
                {
                    if (fireballOnCooldown && !isDead)
                    {
                        spriteRenderer.sprite = blockerMad;
                    }
                    else if (!isDead)
                    {
                        spriteRenderer.sprite = blockerHappy;
                    }

                    dustTrail.Play();
                    canJump = true;
                    if (isJumping)
                    {
                        GameSessionManager.Instance.LogToFile("is jumping true e vai apra false");
                        landedTime = GameSessionManager.Instance.GetElapsedTime();
                        isJumping = false;
                    }
                    GameSessionManager.Instance.LogToFile("can jump a true");
                    GetComponent<Rigidbody2D>().freezeRotation = true;
                    //isRotating = false;
                    //transform.rotation = Quaternion.Euler(0, 0, 0);
                    jumpParticle.Stop();

                    if (transform.position.y > Vars.cameraMaxYPos)
                        Vars.cameraMaxYPos = transform.position.y;
                    // Caso a colisão seja de cima, nada acontece (pousou na box)
                    GameSessionManager.Instance.LogToFile("Safe landing");
                }
            }
        }
        else if (!isDead && col.gameObject.CompareTag("Deadly")) // Verifica se a colisão foi com spikes ou lava
        {
            isDead = true;
            StartCoroutine(DeathSequence());
        }
        else if (!isDead && col.gameObject.CompareTag("Enemy")) //  Verifica se a colisão foi com obstaculos ou inimigos
        {
            isDead = true;
            StartCoroutine(DeathSequence());
        }
        else if (!isDead && col.gameObject.name == "BottomCollider") // When the ball falls from the platform and hits the bottom collider the game is over
        {
            isDead = true;
            StartCoroutine(DeathSequence());
        }
        else if (!isDead && col.gameObject.name == "FinishBottom") // When the ball jumps on the finish platform
        {
            GameObject.Find("GameManager").GetComponent<Menus>().LevelComplete();
            GetComponent<Level6SquareControlTutorial>().enabled = false;
            GameObject.Find("LevelCompleteSound").GetComponent<AudioSource>().Play();

        }
        else if (!isDead && col.gameObject.name == "FinishTutorial") // When the ball jumps on the finish platform
        {
            StartCoroutine(EndTutorial());

        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.name == "Floor")
        {
            //Invoke("disableJumping", 0.1f); // Disable jumping 0.1 seconds after the ball left the bottom of the platform
        }
    }

    private void disableJumping()
    {
        canJump = false;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!isDead && col.CompareTag("Enemy"))
        {
            isDead = true;
            StartCoroutine(DeathSequence());
        }
        else if (!isDead && col.CompareTag("Deadly"))
        {
            isDead = true;
            StartCoroutine(DeathSequence());
        }
    }


    void OnTriggerExit2D(Collider2D col)
    {
        canCollide = true;
    }

    IEnumerator DeathSequence()
    {
        spriteRenderer.sprite = blockerDead;
        canCollide = false;

        // PARA MOVIMENTO IMEDIATAMENTE
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        hitSound.Play();
        Debug.Log("morreu");

        GameSessionManager.Instance.SetTimeOfDeath();

        deathParticle.Play();

        // Espera 0.3 segundos
        yield return new WaitForSeconds(0.4f);

        // Deixa o sprite invisível
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.enabled = false;

        // Espera mais 0.2 segundos para as partículas continuarem visíveis
        yield return new WaitForSeconds(0.3f);
        HideTutorialUI();
        // Chama o Game Over depois da espera
        GameObject.Find("GameManager").GetComponent<Menus>().GameOver();

        // Destrói o square
        Destroy(this.gameObject);
    }

}
