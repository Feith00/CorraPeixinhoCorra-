using UnityEngine;
using System.Collections;
using UnityEngine.UI; 

public class PlayerScript : MonoBehaviour
{
    // UI multijogador
    [HideInInspector] public PontosUI minhaPontuacaoUI;
    public bool EstaMorto() => isDead;

    public void ConfigurarUI(Image[] coracoes, PontosUI pontosUI)
    {
        coracoesUI = coracoes;
        minhaPontuacaoUI = pontosUI;
        AtualizarCoracoes();

        if (minhaPontuacaoUI != null)
            minhaPontuacaoUI.VincularJogador(this);
    }

    [Header("Controles Individuais")]
    public KeyCode teclaCima = KeyCode.W;
    public KeyCode teclaBaixo = KeyCode.S;
    public KeyCode teclaEsquerda = KeyCode.A;
    public KeyCode teclaDireita = KeyCode.D;
    public KeyCode teclaDash = KeyCode.Space;

    public void ConfigurarControles(KeyCode cima, KeyCode baixo, KeyCode esquerda, KeyCode direita, KeyCode dash)
    {
        teclaCima = cima;
        teclaBaixo = baixo;
        teclaEsquerda = esquerda;
        teclaDireita = direita;
        teclaDash = dash;
    }

    [Header("Movimentação")]
    [SerializeField] private float speed = 15f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 25f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 0.5f;
    
    [Header("Efeitos Visuais")]
    [SerializeField] private ParticleSystem particulaBolhas;
    [SerializeField] private float duracaoFlashCooldown = 0.2f;
    private Color corBrilhoDash = new Color(5f, 5f, 5f, 1f);

    [Header("Vida e Dano")]
    [SerializeField] private int life = 3;
    [SerializeField] private float damageCooldown = 1.5f;
    [SerializeField] private GameObject damegeArea; 
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private float blinkSpeed = 0.1f;
    private Image[] coracoesUI; 

    protected bool isDead = false;
    private Vector3 escalaInicial;

    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 startPosition;

    [HideInInspector] public bool isDashing;
    private float dashTimer;
    private float cooldownTimer;
    private bool canTakeDamage = true;
    private bool aguardandoCooldownDash = false; // Julian
    private Color corOriginalSprite = Color.white;//Julian

    
    // Nova variável para controlar a imortalidade do coletável
    private bool isPowerUpInvincible = false; 

    void Start()
    {
        escalaInicial = transform.localScale;
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;

        if (sprite == null) 
            sprite = GetComponent<SpriteRenderer>();

        if (sprite != null)
            corOriginalSprite = sprite.color;

        if (particulaBolhas != null)
            particulaBolhas.Stop();

        
        AtualizarCoracoes();
    }

    void Update()
    {
        if (isDead) return;
        /*{
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ResurreccionarJogador();
            }
            return;
        }*/

        ViraSprite();

        // Lê apenas as 4 teclas configuradas para ESTE jogador
        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(teclaDireita)) moveX += 1f;
        if (Input.GetKey(teclaEsquerda)) moveX -= 1f;
        if (Input.GetKey(teclaCima)) moveY += 1f;
        if (Input.GetKey(teclaBaixo)) moveY -= 1f;

        movement = new Vector2(moveX, moveY).normalized;

        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0 && aguardandoCooldownDash)
            {
                aguardandoCooldownDash = false;
                StartCoroutine(FlashCooldownDash()); // Ativa o efeito pisca (Julian)
            }
        }
        // Lê a tecla de Dash individual deste jogador
        if (Input.GetKeyDown(teclaDash) && !isDashing && cooldownTimer <= 0)
        {
            if (movement != Vector2.zero)
            {
                isDashing = true;
                dashTimer = dashDuration;
                cooldownTimer = dashCooldown;
                aguardandoCooldownDash = true;

                if (particulaBolhas != null)
                    particulaBolhas.Play();
            }
        }
    }

    void FixedUpdate()
    {
        if (isDead) 
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (isDashing)
        {
            rb.MovePosition(rb.position + movement * dashSpeed * Time.fixedDeltaTime);
            
            if (damegeArea != null) damegeArea.SetActive(false);

            dashTimer -= Time.fixedDeltaTime;

            if (dashTimer <= 0)
            {
                isDashing = false;

                if (particulaBolhas != null) //Julian
                    particulaBolhas.Stop();
                // Só reativa a área de dano se o player NÃO estiver sob efeito do power-up
                if (canTakeDamage && !isPowerUpInvincible && damegeArea != null) 
                    damegeArea.SetActive(true);
            }
        }
        else
        {
            rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
            
            // Só reativa a área de dano se o player NÃO estiver sob efeito do power-up
            if (canTakeDamage && !isPowerUpInvincible && damegeArea != null && !damegeArea.activeSelf)
                damegeArea.SetActive(true);
        }
    }

    IEnumerator FlashCooldownDash()
    {
        if (sprite == null) yield break;

        // 1. Muda bruscamente para branco brilhante no exato milissegundo
        sprite.color = corBrilhoDash;

        float tempo = 0f;

        // 2. Transita gradualmente de volta para a cor original nos 0.2s
        while (tempo < duracaoFlashCooldown)
        {
            tempo += Time.deltaTime;
            sprite.color = Color.Lerp(corBrilhoDash, corOriginalSprite, tempo / duracaoFlashCooldown);
            yield return null;
        }

        // Garante que a cor fique perfeita ao final
        sprite.color = corOriginalSprite;
    }

    public void TakeDamage()
    {
        // Se estiver imortal pelo powerup, tomando dano normal ou dando dash, ignora
        if (isPowerUpInvincible || !canTakeDamage || isDashing) return;

        life--;
        AtualizarCoracoes();

        if (life <= 0)
        {

            StartCoroutine(Die());
            return;
        }

        canTakeDamage = false;
        if (damegeArea != null) damegeArea.SetActive(false);

        StartCoroutine(Blink());
        Invoke(nameof(EnableDamage), damageCooldown);
    }

    // --- NOVAS FUNÇÕES PARA O COLETÁVEL DE IMORTALIDADE ---
    public void ActivatePowerUpInvincibility(float duration)
    {
        // Evita bugs caso ele pegue outro coletável idêntico antes do primeiro acabar
        StopCoroutine(nameof(PowerUpInvincibilityRoutine)); 
        StartCoroutine(PowerUpInvincibilityRoutine(duration));
    }

    private IEnumerator PowerUpInvincibilityRoutine(float duration)
    {
        isPowerUpInvincible = true;
        canTakeDamage = false; // Bloqueia o TakeDamage padrão
        
        if (damegeArea != null) damegeArea.SetActive(false); // Desativa área de dano

        float timer = 0f;
        // Faz o player piscar durante todo o tempo do Power-up
        while (timer < duration)
        {
            sprite.enabled = !sprite.enabled;
            yield return new WaitForSeconds(blinkSpeed);
            timer += blinkSpeed;
        }

        // Restaura o estado normal do player após o fim do tempo
        sprite.enabled = true;
        isPowerUpInvincible = false;
        canTakeDamage = true;

        if (!isDashing && damegeArea != null) 
            damegeArea.SetActive(true);
    }

    void AtualizarCoracoes()
    {
        if (coracoesUI != null)
        {
            for (int i = 0; i < coracoesUI.Length; i++)
            {
                coracoesUI[i].gameObject.SetActive(i < life);
            }
        }
    }

    IEnumerator Blink()
    {
        // O Blink normal de dano só acontece se não estiver com o Power-up ativo
        while (!canTakeDamage && !isPowerUpInvincible)
        {
            sprite.enabled = !sprite.enabled;
            yield return new WaitForSeconds(blinkSpeed);
        }
        if (!isPowerUpInvincible) sprite.enabled = true;
    }

    void EnableDamage()
    {
        // Só reativa por aqui se o power-up não estiver rodando em paralelo
        if (isPowerUpInvincible) return;

        canTakeDamage = true;
        if (!isDashing && damegeArea != null) 
            damegeArea.SetActive(true);
    }

    IEnumerator Die()
    {
        if (isDead) yield break;
        isDead = true;

        GetComponent<RegistrarRecorde>()?.RegistrarPontuacaoFinal();

        rb.linearVelocity = Vector2.zero;
        movement = Vector2.zero;

        if (particulaBolhas != null)
            particulaBolhas.Stop();
        gameObject.SetActive(false);

        if (MultiplayerManager.Instance != null)
        {
            MultiplayerManager.Instance.VerificarFimDeJogo();
        }

        yield break;
        
        // Verifica quantos jogadores ainda estão vivos na cena
        /* int jogadoresVivos = 0;
        foreach (PlayerScript p in FindObjectsByType<PlayerScript>(FindObjectsSortMode.None))
        {
            if (!p.EstaMorto()) jogadoresVivos++;
        }

        if (jogadoresVivos == 0)
        {
            // Só pausa o jogo e abre a tela de morte quando TODOS morrerem (funciona em 1 a 4 players)
            Time.timeScale = 0f;
            MostrarTeladeMorte(true);
        }
        else
        {
            // Se ainda tem amigo jogando, apenas esconde este peixe até o fim da rodada
            gameObject.SetActive(false);
        }

        yield break; */
    }

    void ResurreccionarJogador()
    {
        //Forçando a unity a voltar ao normal
        Time.timeScale = 1f;
        // Forçando o jogador a restar
        transform.localScale = Vector3.one;

        MudarTamanho scriptTamanho = GetComponent<MudarTamanho>();
        if (scriptTamanho != null)
        {
            scriptTamanho.ResetaTamanho();
        }
        else
        {
            // Caso não use o script de tamanho neste objeto específico, força o padrão
            transform.localScale = escalaInicial;
        }
        // Força a camera a recalcular a proporção da tela
        if (Camera.main != null)
        {
            Camera.main.ResetAspect(); 
        }
    
        // Resetando variaveris
        isDead = false;
        transform.position = startPosition;
        life = 3; 
        canTakeDamage = true;
        isPowerUpInvincible = false; // Garante reset do powerup na morte
        escalaInicial = transform.localScale;
        if (particulaBolhas != null) //Julian
            particulaBolhas.Stop();
        
        if(sprite != null) sprite.enabled = true;
        if(damegeArea != null) damegeArea.SetActive(true);
        
       
        //MostrarTeladeMorte(false);
        AtualizarCoracoes();

        if (minhaPontuacaoUI != null)
        {
            minhaPontuacaoUI.ResetarPontos();
        }  
    }

    void ViraSprite()
    {
    Vector3 escalaAtual = transform.localScale;

    if (movement.x > 0)
    {
        
        transform.localScale = new Vector3(Mathf.Abs(escalaAtual.x), escalaAtual.y, escalaAtual.z);
    }
    else if (movement.x < 0)
    {
        
        transform.localScale = new Vector3(-Mathf.Abs(escalaAtual.x), escalaAtual.y, escalaAtual.z);
    }
    }
}
