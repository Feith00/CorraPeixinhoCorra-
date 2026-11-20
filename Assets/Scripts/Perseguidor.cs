using UnityEngine;
using System.Collections.Generic;

public class Perseguidor : MonoBehaviour
{
    [Header("Configurações de Velocidade")]
    [SerializeField] private float velocidadeMinima = 2f;
    [SerializeField] private float velocidadeMaxima = 12f;
    [SerializeField] private float aceleracaoPorSegundo = 0.8f;
    [SerializeField] private string tagDoJogador = "Player";

    [Header("Configurações de Knockback (Dash)")]
    [SerializeField] private float forcaKnockback = 25f;
    [SerializeField] private float duracaoKnockback = 0.6f;
    [SerializeField] private float velocidadeGiro = 1080f;

    private float velocidadeAtual;
    private PlayerScript jogadorAlvo; // Em vez de private Transform transformJogador;
    private Rigidbody2D rb;
    private Vector2 direcao;

    // Variáveis de controle do Knockback e Seleção de Alvo
    private bool emKnockback = false;
    private float timerKnockback = 0f;
    private Vector2 direcaoKnockback;
    private PlayerScript ultimoAtacante;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        velocidadeAtual = velocidadeMinima;
        EscolherNovoAlvo(null); // Escolhe um alvo inicial aleatório entre os jogadores presentes
    }

    private void Update()
    {
        // --- ESTADO 1: EM KNOCKBACK (VOANDO E GIRANDO) ---
        if (emKnockback)
        {
            timerKnockback -= Time.deltaTime;
            transform.Rotate(0f, 0f, velocidadeGiro * Time.deltaTime);

            if (timerKnockback <= 0f)
            {
                emKnockback = false;
                transform.rotation = Quaternion.identity;
                velocidadeAtual = velocidadeMinima;

                // Sorteia outro jogador (evitando o ultimoAtacante, se houver outras opções)
                EscolherNovoAlvo(ultimoAtacante);

                // Limpa o ultimoAtacante para que ele possa dar dash novamente no futuro
                ultimoAtacante = null;
            }
            return;
        }

        // --- ESTADO 2: PERSEGUIÇÃO NORMAL ---
        // Só troca de alvo se o jogador realmente morreu ou foi destruído
        if (jogadorAlvo == null || jogadorAlvo.EstaMorto() || !jogadorAlvo.gameObject.activeInHierarchy)
        {
            EscolherNovoAlvo(null);
        }

        velocidadeAtual = Mathf.MoveTowards(velocidadeAtual, velocidadeMaxima, aceleracaoPorSegundo * Time.deltaTime);

        if (jogadorAlvo != null)
        {
            CalcularDirecaoAteJogador();
        }
    }

    private void FixedUpdate()
    {
        if (emKnockback)
        {
            float fatorDesaceleracao = timerKnockback / duracaoKnockback;
            rb.MovePosition(rb.position + direcaoKnockback * (forcaKnockback * fatorDesaceleracao) * Time.fixedDeltaTime);
            return;
        }

        if (jogadorAlvo != null)
        {
            MoverEmDirecaoAoJogador();
        }
        else
        {
            PararMovimento();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag(tagDoJogador)) return;

        PlayerScript player = other.GetComponentInParent<PlayerScript>();
        if (player == null) return;

        if (player.isDashing)
        {
            if (!emKnockback || player != ultimoAtacante)
            {
                AplicarKnockback(player);
            }
        }
        else
        {
            player.TakeDamage();
        }
    }

    private void AplicarKnockback(PlayerScript atacante)
    {
        emKnockback = true;
        timerKnockback = duracaoKnockback;
        ultimoAtacante = atacante;

        direcaoKnockback = ((Vector2)transform.position - (Vector2)atacante.transform.position).normalized;

        if (direcaoKnockback == Vector2.zero)
            direcaoKnockback = Random.insideUnitCircle.normalized;
    }

    private void EscolherNovoAlvo(PlayerScript jogadorParaEvitar)
    {
        // Busca diretamente os scripts PlayerScript na cena (ignora objetos filhos como damegeArea)
        PlayerScript[] todosJogadores = FindObjectsByType<PlayerScript>(FindObjectsSortMode.None);

        if (todosJogadores.Length == 0)
        {
            jogadorAlvo = null;
            return;
        }

        List<PlayerScript> candidatos = new List<PlayerScript>();

        foreach (PlayerScript player in todosJogadores)
        {
            if (player.EstaMorto() || !player.gameObject.activeInHierarchy) continue;

            if (jogadorParaEvitar == null || player != jogadorParaEvitar)
            {
                candidatos.Add(player);
            }
        }

        if (candidatos.Count > 0)
        {
            int indiceAleatorio = Random.Range(0, candidatos.Count);
            jogadorAlvo = candidatos[indiceAleatorio];
        }
        else
        {
            foreach (PlayerScript player in todosJogadores)
            {
                if (!player.EstaMorto() && player.gameObject.activeInHierarchy)
                {
                    jogadorAlvo = player;
                    break;
                }
            }
        }
    }

    private void CalcularDirecaoAteJogador()
    {
        direcao = ((Vector2)jogadorAlvo.transform.position - (Vector2)transform.position).normalized;
    }

    private void MoverEmDirecaoAoJogador()
    {
        rb.MovePosition(rb.position + direcao * velocidadeAtual * Time.fixedDeltaTime);
    }

    private void PararMovimento()
    {
        rb.linearVelocity = Vector2.zero;
    }
}