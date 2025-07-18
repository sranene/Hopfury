using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using FingerNew = UnityEngine.InputSystem.EnhancedTouch.Finger;

public class SquareControl : MonoBehaviour {
    
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

    private int stationaryTouchCount = 0;
    private const int stationaryTouchThreshold = 1; // nº de frames stationary antes de considerar tap
    private Dictionary<int, bool> alreadyJumpedForTouch = new Dictionary<int, bool>();
    private int? activeTouchId = null; // dedo ativo que estamos a processar

    private float touchStartTime;


    bool isDead = false;

    private float jumpXPosition = 14.15f; //test


    private void Awake()
    {
        EnhancedTouchSupport.Enable();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = blockerHappy;
    }
    
    private void OnEnable()
    {
        GameSessionManager.Instance.LogToFile($"on enable ball control");
        TouchInputManager.Instance.OnTapStart += HandleTapStart;
        TouchInputManager.Instance.OnTapEnd += HandleTapEnd;
    }

    private void OnDisable()
    {
        GameSessionManager.Instance.LogToFile($"on disable ball control");
        TouchInputManager.Instance.OnTapStart -= HandleTapStart;
        TouchInputManager.Instance.OnTapEnd -= HandleTapEnd;
    }


    void Start() 
    {
        GameSessionManager.Instance.SetElapsedTime();
        jumpSound = GameObject.Find("JumpSound").GetComponent<AudioSource>();
        hitSound = GameObject.Find("HitSound").GetComponent<AudioSource>();
        fireballSound = GameObject.Find("FireballSound").GetComponent<AudioSource>();
        SetBallVelocity();
    }

    void SetBallVelocity()
    {
        if (moveRight)
            rb.velocity = new Vector2(Mathf.Abs(moveSpeed), rb.velocity.y);
        else
            rb.velocity = new Vector2(-Mathf.Abs(moveSpeed), rb.velocity.y);
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

    void FixedUpdate()
    {
        if (shouldBoost)
        {
            JumpBooster();
            shouldBoost = false;
        }



        /*if (Mathf.Abs(rb.position.x - jumpXPosition) < 0.05f)
        {
            Jump();
            Debug.Log("SALTOU EM X = " + rb.position.x);
            GameSessionManager.Instance.LogToFile("SALTOU EM X = " + rb.position.x);
        }*/

    }

    void Update()
    {

        //HandleMouseInput();
        HandleTouchInput();

        if (rb.velocity.y < 0 && canCollide)
        {
            boxCollider.isTrigger = false;
        }

        /*// Checa se passou tempo suficiente para verificar o gesto
        if (isWaitingForGesture && Time.time - gestureStartTime >= 0.05f)
        {
            float movement = Vector2.Distance(touchStartPos, touchEndPos);

            if (movement < swipeThreshold)
            {
                // É um tap: salta
                if (canJump)
                    Jump();
            }
            else
            {
                // É um swipe para cima
                Vector2 swipeDir = touchEndPos - touchStartPos;
                if (swipeDir.y > Mathf.Abs(swipeDir.x))  // Swipe prioritariamente para cima
                {
                    TryLaunchFireball();
                }
            }

            isWaitingForGesture = false;  // Reseta o estado de gesto
        }*/
    }

    private IEnumerator StaticTapTimeout(int fingerId)
    {
        yield return new WaitForSeconds(0.2f);

        if (!gestureIsSwipeCandidate && !alreadyJumpedForTouch.GetValueOrDefault(fingerId, false))
        {
            GameSessionManager.Instance.LogToFile("TAP ESTÁTICO DETETADO APÓS 0.2s → SALTAR");
            OnTapGesture();
            alreadyJumpedForTouch[fingerId] = true;
        }
    }


    private void HandleTouchInput()
    {
        if (UnityEngine.Input.touchCount > 0)
        {
            UnityEngine.Touch touch = UnityEngine.Input.GetTouch(0);
            int fingerId = touch.fingerId;

            // Se não temos dedo ativo, registamos este dedo ao começar o toque
            if (touch.phase == UnityEngine.TouchPhase.Began)
            {
                activeTouchId = fingerId;
                alreadyJumpedForTouch[fingerId] = false;
                touchStartTime = Time.time;
                stationaryTouchCount = 0;
                earlyTouchPositions.Clear();
                earlyTouchPositions.Add(touch.position);
                gestureIsSwipeCandidate = false;
                startTouchTime = Time.time;

                GameSessionManager.Instance.LogToFile($"TOQUE COMEÇADO (fingerId={fingerId})");

                if (tapTimeoutCoroutine != null) StopCoroutine(tapTimeoutCoroutine);
                tapTimeoutCoroutine = StartCoroutine(StaticTapTimeout(fingerId));

            }
            else
            {
                // Se este evento não é do dedo ativo, ignorar tudo
                if (activeTouchId == null || fingerId != activeTouchId)
                {
                    GameSessionManager.Instance.LogToFile($"IGNORAR evento do fingerId={fingerId}, ativo={activeTouchId}");
                    return;
                }

                // Agora processamos só o dedo ativo
                if (touch.phase == UnityEngine.TouchPhase.Stationary)
                {
                    stationaryTouchCount++;
                    GameSessionManager.Instance.LogToFile($"Touch estático detetado (count={stationaryTouchCount})");

                    if (stationaryTouchCount >= stationaryTouchThreshold && !alreadyJumpedForTouch.GetValueOrDefault(fingerId, false))
                    {
                        float timeHeld = Time.time - touchStartTime;
                        GameSessionManager.Instance.LogToFile($"TOQUE ESTÁTICO DETETADO DURANTE VÁRIOS FRAMES → SALTAR ({timeHeld})");
                        OnTapGesture();
                        alreadyJumpedForTouch[fingerId] = true;
                        stationaryTouchCount = 0;

                        if (tapTimeoutCoroutine != null)
                        {
                            StopCoroutine(tapTimeoutCoroutine);
                            tapTimeoutCoroutine = null;
                        }

                        // Limpar dados para não detetar swipe logo após salto
                        gestureIsSwipeCandidate = false;
                        earlyTouchPositions.Clear();
                    }
                }
                else if (touch.phase == UnityEngine.TouchPhase.Moved)
                {
                    stationaryTouchCount = 0;
                    if (alreadyJumpedForTouch.GetValueOrDefault(fingerId, false))
                    {
                        GameSessionManager.Instance.LogToFile($"MOVIMENTO IGNORADO → JÁ FOI FEITO UM SALTO NESTE TOQUE (fingerId={fingerId})");
                        return;
                    }

                    GameSessionManager.Instance.LogToFile("Contagem de toques estáticos CANCELADA POR MOVIMENTO → NÃO É TAP ESTÁTICO");
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
                            else if (duration < 0.5f && speed < 200f && !alreadyJumpedForTouch.GetValueOrDefault(fingerId, false))
                            {
                                GameSessionManager.Instance.LogToFile($"TAP DETETADO COM POUCA VELOCIDADE (speed={speed:F1}) → SALTAR JÁ");
                                OnTapGesture();
                                alreadyJumpedForTouch[fingerId] = true;

                                // Limpar dados após salto
                                gestureIsSwipeCandidate = false;
                                earlyTouchPositions.Clear();
                            }
                        }
                    }
                }
                else if (touch.phase == UnityEngine.TouchPhase.Ended)
                {
                    stationaryTouchCount = 0;

                    if (alreadyJumpedForTouch.GetValueOrDefault(fingerId, false))
                    {
                        GameSessionManager.Instance.LogToFile($"IGNORAR SWIPE OU TAP FINAL → JÁ FOI FEITO UM SALTO (fingerId={fingerId})");
                        activeTouchId = null;
                        return;
                    }

                    if (tapTimeoutCoroutine != null)
                    {
                        StopCoroutine(tapTimeoutCoroutine);
                        tapTimeoutCoroutine = null;
                    }

                    Vector2 endPos = touch.position;

                    if (gestureIsSwipeCandidate)
                    {
                        Vector2 swipeVector = endPos - earlyTouchPositions[0];

                        if (swipeVector.y > Mathf.Abs(swipeVector.x) && swipeVector.y > 50f && !alreadyJumpedForTouch.GetValueOrDefault(fingerId, false))
                        {
                            GameSessionManager.Instance.LogToFile($"SWIPE PARA CIMA CONFIRMADO → LANÇAR FIREBALL (fingerId={fingerId})");
                            OnSwipeUpGesture();
                        }
                        else
                        {
                            GameSessionManager.Instance.LogToFile("SWIPE MAL DIRECIONADO NO FIM → IGNORAR");
                        }
                    }
                    else
                    {
                        Vector2 delta = endPos - earlyTouchPositions[0];
                        float distance = delta.magnitude;
                        float duration = Time.time - startTouchTime;
                        float speed = distance / duration;

                        if (duration < 0.5f && speed < 200f)
                        {
                            GameSessionManager.Instance.LogToFile($"TAP CURTO NO ENDED (speed={speed:F1}) → SALTAR");
                            OnTapGesture();
                            alreadyJumpedForTouch[fingerId] = true;
                        }
                        else
                        {
                            GameSessionManager.Instance.LogToFile($"GESTO DEMASIADO LENTO OU LONGO (duration={duration:F2}, speed={speed:F1}) → IGNORAR");
                        }
                    }

                    activeTouchId = null; // Reset quando o toque acaba
                }
            }
        }
    }



    public void OnTapGesture()
    {
        if (GameManager.Instance.IsInChallengeMode())
        {
            return;
        }
        if (canJump)
        {
            canJump = false;
            isJumping = true;
            Jump();
        }
    }

    public void OnSwipeUpGesture()
    {
        if (GameManager.Instance.IsInChallengeMode())
        {
            return;
        }
        TryLaunchFireball();
    }

    private void HandleTapStart(FingerNew finger)
    {
        GameSessionManager.Instance.LogToFile($"detetou tap");

        tapStartTime = (float)(finger.currentTouch.startTime - Time.realtimeSinceStartup + GameSessionManager.Instance.GetElapsedTime());

        // Como este script está no próprio jogador, podemos aceder diretamente ao transform
        squareXAtTap = transform.position.x;
        squareYAtTap = transform.position.y;

        GameSessionManager.Instance.LogToFile($"[TapStart] Posição do square: X={squareXAtTap}, Y={squareYAtTap}");
    }


    private void HandleTapEnd(FingerNew finger)
    {
        GameSessionManager.Instance.LogToFile($"tap terminou");
        tapEndTime = GameSessionManager.Instance.GetElapsedTime();

        float radius = 0f;
        float radiusVar = 0f;
        UnityEngine.Touch legacyTouch = Input.touches[0];
        radius = legacyTouch.radius;
        radiusVar = legacyTouch.radiusVariance;

        // Verificação direta de swipe com posição inicial e final
        Vector2 startPos = finger.currentTouch.startScreenPosition;
        Vector2 endPos = finger.screenPosition;
        Vector2 swipeVector = endPos - startPos;
        if (swipeVector.y > Mathf.Abs(swipeVector.x) && swipeVector.y > 50f)
        {
            GameSessionManager.Instance.LogToFile("SWIPE PARA CIMA → NÃO É TAP"); 
            isTap = false;
        }
        else if (swipeVector.magnitude > 10f)
        {
            GameSessionManager.Instance.LogToFile("SWIPE MAL DIRECIONADO → NÃO É TAP");
            isTap = false;
        }

        Tap tap = new Tap
        {
            id = tapId++,
            isTap = isTap,
            x = finger.screenPosition.x,             
            y = finger.screenPosition.y,             
            timeStart = tapStartTime,               
            timeHold = tapEndTime - tapStartTime, 
            timeEnd = tapEndTime,                   
            pressure = finger.currentTouch.pressure,  
            radius = radius,     
            radiusVariance = radiusVar,
            square_x = squareXAtTap,
            square_y = squareYAtTap,
            landedTime = landedTime
        };

        GameSessionManager.Instance.RegisterPendingTap(tap);
       // issavingdata = false;
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


    private void Jump()
    {
        Debug.Log("salto");
        if (!isDead)
        {
            spriteRenderer.sprite = blockerJump;
        }
        dustTrail.Stop();
        GameSessionManager.Instance.LogToFile("Posição X no salto: " + transform.position.x);
        GameSessionManager.Instance.LogToFile($"Jump triggered at X: {transform.position.x}");
        GameSessionManager.Instance.LogToFile("comecou o salto");
        rb.velocity = new Vector2(rb.velocity.x, 0); // Reseta a velocidade no eixo Y, mas mantém a velocidade no eixo X

        //GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        //isRotating = true; // Ativa a rotação quando salta
        // Aplicando a rotação inicial (180 graus no eixo Z)
        //targetRotation = transform.rotation.eulerAngles.z + 180f; // Define o ângulo de destino

        boxCollider.isTrigger = true;
        rb.AddForce(new Vector2(0, 2450));

        jumpSound.Play();
        jumpParticle.Play();
    }

    private void JumpBooster()
    {
        if (!isDead)
        {
            spriteRenderer.sprite = blockerJump;
        }
        dustTrail.Stop();
        GameSessionManager.Instance.LogToFile("comecou o salto do booster");
        rb.velocity = new Vector2(rb.velocity.x, 0); // Reseta a velocidade no eixo Y, mas mantém a velocidade no eixo X
        canJump = false;

        //GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        //isRotating = true; // Ativa a rotação quando salta
        // Aplicando a rotação inicial (180 graus no eixo Z)
        //targetRotation = transform.rotation.eulerAngles.z + 180f; // Define o ângulo de destino

        boxCollider.isTrigger = true;
        rb.AddForce(new Vector2(0, 2650));

        jumpSound.Play();
        jumpParticle.Play();
    }


    void OnCollisionEnter2D(Collision2D col)
    {
        GameSessionManager.Instance.LogToFile($"BALL CONTROL colidiu com '{col.gameObject.name}");
        
        if (col.gameObject.name == "Left Box" || col.gameObject.name == "Right Box") //REST STATION
        {
            foreach (ContactPoint2D contact in col.contacts)
            {
                // Obtém o ponto de contacto e o centro do objeto com o qual colidiste
                float contactX = contact.point.x;
                float boxCenterX = col.collider.bounds.center.x;

                bool hitLeftSide = contactX < boxCenterX;
                bool hitRightSide = contactX > boxCenterX;

                if (Mathf.Abs(contact.normal.x) >= 0.3f && Mathf.Abs(contact.normal.y) <= 0.3f)
                {
                    if (col.gameObject.name == "Left Box")
                    {
                        if (!isDead && hitLeftSide)
                        {
                            isDead = true;
                            StartCoroutine(DeathSequence());
                            break;
                        }
                        else
                        {
                            ReverseDirection(); // lado direito → muda de direção
                        }
                    }
                    else if (col.gameObject.name == "Right Box")
                    {
                        if (!isDead && hitRightSide)
                        {
                            isDead = true;
                            StartCoroutine(DeathSequence());
                            break;
                        }
                        else
                        {
                            ReverseDirection(); // lado esquerdo → muda de direção
                        }
                    }

                    return;
                }
            }
        }
        else if (col.gameObject.name == "Booster")
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
                    shouldBoost = true;
                }
            }
        }
       else if (col.gameObject.CompareTag("Ground"))
        {
            foreach (ContactPoint2D contact in col.contacts)
            {
                Vector2 normal = contact.normal;

                // Considera letal se a colisão for praticamente horizontal (vindo de lado)
                bool isSideHit = Mathf.Abs(normal.y) < 0.3f;

                if (!isDead && isSideHit)
                {
                    isDead = true;
                    StartCoroutine(DeathSequence());
                    break;
                }
                else if (!isDead)
                {
                    if (fireballOnCooldown)
                    {
                        spriteRenderer.sprite = blockerMad;
                    }
                    else
                    {
                        spriteRenderer.sprite = blockerHappy;
                    }

                    dustTrail.Play();
                    Debug.Log("tocou no ground e ja pode saltar");
                    canJump = true;

                    if (isJumping)
                    {
                        GameSessionManager.Instance.LogToFile("is jumping true e vai para false");
                        landedTime = GameSessionManager.Instance.GetElapsedTime();
                        isJumping = false;
                    }

                    GameSessionManager.Instance.LogToFile("can jump a true");

                    GetComponent<Rigidbody2D>().freezeRotation = true;
                    jumpParticle.Stop();

                    if (transform.position.y > Vars.cameraMaxYPos)
                        Vars.cameraMaxYPos = transform.position.y;

                    GameSessionManager.Instance.LogToFile("Safe landing");
                }
            }
        }
        else if(col.gameObject.CompareTag("Platform"))
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
                    } else if (!isDead)
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

                    if(transform.position.y > Vars.cameraMaxYPos)
                        Vars.cameraMaxYPos = transform.position.y;
                    // Caso a colisão seja de cima, nada acontece (pousou na box)
                    GameSessionManager.Instance.LogToFile("Safe landing");
                }
            }

        }
        else if (!isDead && col.gameObject.CompareTag("Deadly")) // Verifica se a colisão foi com spikes ou lava
        {
            isDead = true;

            Transform colTransform = col.transform;
            Transform nearColStartTransform = colTransform.Find("NearCol start");
            Transform nearColEndTransform = colTransform.Find("NearCol end");

            if (nearColStartTransform != null && nearColEndTransform != null)
            {
                NearColTrigger startTrigger = nearColStartTransform.GetComponent<NearColTrigger>();
                NearColTrigger endTrigger = nearColEndTransform.GetComponent<NearColTrigger>();

                if (startTrigger != null && endTrigger != null)
                {
                    float start = startTrigger.GetTimeStart();
                    float end = endTrigger.GetTimeEnd();
                    float stimuli = startTrigger.GetTimeStimuli();
                    string name = startTrigger.GetName();
                    float finishTime = GameSessionManager.Instance.GetElapsedTime();

                    float x = startTrigger.GetX();
                    float y = startTrigger.GetY();
                    //float width = startTrigger.GetWidth();
                    float width = -1;

                    GameSessionManager.Instance.LogToFile($"[Finish] Saving obstacle (filhos) com X={x}, Y={y}, Width={width}");
                    GameSessionManager.Instance.SetObstacle(name, start, end, stimuli, finishTime, x, y, width);
                }
                else
                {
                    GameSessionManager.Instance.LogToFile("Script NearColTrigger não encontrado nos filhos NearCol start ou NearCol end.");
                }
            }
            else
            {
                // Verifica se o pai tem NearColTrigger (caso seja um obstáculo "autónomo")
                Transform parent = colTransform.parent != null ? colTransform.parent : colTransform;
                NearColTrigger nearCol = parent.GetComponent<NearColTrigger>();

                if (nearCol != null)
                {
                    float start = nearCol.GetTimeStart();
                    float end = nearCol.GetTimeEnd();
                    float stimuli = nearCol.GetTimeStimuli();
                    string name = nearCol.GetName();
                    float finishTime = GameSessionManager.Instance.GetElapsedTime();

                    float x = nearCol.GetX();
                    float y = nearCol.GetY();
                    //float width = nearCol.GetWidth();
                    float width = -1;

                    GameSessionManager.Instance.LogToFile($"[Finish] Saving obstacle (pai com NearColTrigger) com X={x}, Y={y}, Width={width}");
                    GameSessionManager.Instance.SetObstacle(name, start, end, stimuli, finishTime, x, y, width);
                }
                else
                {
                    GameSessionManager.Instance.LogToFile("Nenhum NearColTrigger encontrado nem nos filhos nem no pai.");
                }
            }

            StartCoroutine(DeathSequence());
        }
        else if(!isDead && col.gameObject.CompareTag("Enemy")) //  Verifica se a colisão foi com obstaculos ou inimigos
        {
            isDead = true;
            StartCoroutine(DeathSequence());
        }
        else if(!isDead && col.gameObject.CompareTag("Lock")) //  Verifica se a colisão foi com o lock
        {
            isDead = true;
            StartCoroutine(DeathSequence());
        }
        else if(!isDead && col.gameObject.name == "BottomCollider") // When the ball falls from the platform and hits the bottom collider the game is over
        {
            isDead = true;
            StartCoroutine(DeathSequence());
        }
        else if(!isDead && col.gameObject.name == "FinishBottom") // When the ball jumps on the finish platform
        {
            GameObject.Find("GameManager").GetComponent<Menus>().LevelComplete();
            GetComponent<SquareControl>().enabled = false;
            GameObject.Find("LevelCompleteSound").GetComponent<AudioSource>().Play();

        } else if(!isDead && col.gameObject.name == "FinishTutorial") // When the ball jumps on the finish platform
        {
            GameObject.Find("GameManager").GetComponent<Menus>().LevelComplete();
            GetComponent<SquareControl>().enabled = false;
            GameObject.Find("LevelCompleteSound").GetComponent<AudioSource>().Play();

        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if(col.gameObject.name == "Floor") 
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

        // Chama o Game Over depois da espera
        GameObject.Find("GameManager").GetComponent<Menus>().GameOver();

        // Destrói o square
        Destroy(this.gameObject);
    }

}
