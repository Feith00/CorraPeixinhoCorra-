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
        // Se estiver morto, verifica se o jogador quer voltar ao jogo
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
            rb.linearVelocity = Vector2.zero; // Mantém parado
            return;
        }

        if (isDashing)
        {
            rb.MovePosition(rb.position + movement * dashSpeed * Time.fixedDeltaTime);
            
            // Desativa a área de dano durante o Dash
            if (damegeArea != null) damegeArea.SetActive(false);

            dashTimer -= Time.fixedDeltaTime;

            if (dashTimer <= 0)
            {
                isDashing = false;
                if (canTakeDamage && damegeArea != null) damegeArea.SetActive(true);
            }
        }
        else
        {
            rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
            
            if (canTakeDamage && damegeArea != null && !damegeArea.activeSelf)
                damegeArea.SetActive(true);
        }
    }

    public void TakeDamage()
    {
        if (!canTakeDamage || isDashing) return;

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
        while (!canTakeDamage)
        {
            sprite.enabled = !sprite.enabled;
            yield return new WaitForSeconds(blinkSpeed);
        }
        sprite.enabled = true;
    }

    void EnableDamage()
    {
        canTakeDamage = true;
        if (!isDashing && damegeArea != null) 
            damegeArea.SetActive(true);
    }

    IEnumerator Die()
    {
        if (isDead) yield break;
        isDead = true;

        // 1. Para o movimento imediatamente
        rb.linearVelocity = Vector2.zero;
        movement = Vector2.zero;

        // 2. Mostra a Tela de Morte
        MostrarTeladeMorte(true);

        // 3. Remove a espera automática. Agora o jogo fica parado 
        // esperando o jogador apertar Espaço.
        yield break;
    }

    void ResurreccionarJogador()
    {
        // 1. Reseta o Jogador
        transform.position = startPosition;
        life = 3; 
        AtualizarCoracoes();

        // 2. Esconde a Tela de Morte
        MostrarTeladeMorte(false);

        // 3. Libera o jogador
        canTakeDamage = true;
        isDead = false;

        // Reativa área de dano
        if (damegeArea != null) damegeArea.SetActive(true);
        
        // Garante que o sprite esteja visível
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