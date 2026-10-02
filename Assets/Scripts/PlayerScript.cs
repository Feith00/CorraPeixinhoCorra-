using UnityEngine;
using System.Collections;
using UnityEngine.UI; 

public class PlayerScript : MonoBehaviour
{
    [Header("Movimentação")]
    [SerializeField] private float speed = 15f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 25f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 0.5f;

    [Header("Vida e Dano")]
    [SerializeField] private int life = 3;
    [SerializeField] private float damageCooldown = 1.5f;
    [SerializeField] private GameObject damegeArea; 
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private float blinkSpeed = 0.1f;
    [SerializeField] private Image[] coracoesUI; 
    [SerializeField] private Image[] telademorte;
    [SerializeField] private float tempotelademorte = 0.5f;
    private bool isDead = false;

    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 startPosition;

    [HideInInspector] public bool isDashing;
    private float dashTimer;
    private float cooldownTimer;
    private bool canTakeDamage = true;
    
    // Nova variável para controlar a imortalidade do coletável
    private bool isPowerUpInvincible = false; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;

        if (sprite == null) 
            sprite = GetComponent<SpriteRenderer>();
        
        AtualizarCoracoes();
    }

    void Update()
    {
        if (isDead)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ResurreccionarJogador();
            }
            return;
        }

        ViraSprite();

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;

        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space) && !isDashing && cooldownTimer <= 0)
        {
            if (movement != Vector2.zero)
            {
                isDashing = true;
                dashTimer = dashDuration;
                cooldownTimer = dashCooldown;
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
    // -----------------------------------------------------

    void AltualizarCoracoes() // Mantido o nome original do seu script (caso use em outros cantos)
    {
        AtualizarCoracoes();
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

        rb.linearVelocity = Vector2.zero;
        movement = Vector2.zero;

        MostrarTeladeMorte(true);
        yield break;
    }

    void ResurreccionarJogador()
    {
        transform.position = startPosition;
        life = 3; 
        AtualizarCoracoes();

        MostrarTeladeMorte(false);

        canTakeDamage = true;
        isPowerUpInvincible = false; // Garante reset do powerup na morte
        isDead = false;

        if (damegeArea != null) damegeArea.SetActive(true);
        sprite.enabled = true;
    }

    void MostrarTeladeMorte(bool mostrar)
    {
        if (telademorte == null || telademorte.Length == 0) return;

        foreach (var tela in telademorte)
        {
            if (tela != null)
                tela.gameObject.SetActive(mostrar);
        }
    }

    void ViraSprite()
    {
        if (movement.x > 0)
            transform.localScale = Vector3.one;
        else if (movement.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }
}
