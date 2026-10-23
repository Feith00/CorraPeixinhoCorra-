using UnityEngine;

public class AnzolController : MonoBehaviour
{
    [Header("Referências")]
    public Transform pontaAnzol;       
    public LineRenderer linha;         
    public BoxCollider2D colisorLinha; 
    public SpriteRenderer spriteRendererAnzol;

    [Header("Configuração de Comida")]
    public Sprite spriteAnzolNormal;     // Sprite do anzol sem nada
    public Sprite spriteAnzolComComida;  // Sprite do anzol com isca/comida
    [Range(0f, 1f)] 
    public float chanceDeVimComida = 0.3f; // 30% de chance de nascer com comida
    public int pontosComida = 15;        // Pontos ganhos ao comer com Dash

    [Header("Movimento")]
    public float velocidade = 5f;
    public float multiplicadorVelocidadeRetorno = 3f; 

    [Header("Configuração de Retorno Automático")]
    public float tempoEsperaMinimo = 2f; // Tempo que ele fica parado antes de voltar sozinho

    [Header("Ajuste da Linha")]
    public Transform pontoDeAmarra; // Arraste o objeto PontoDeAmarra aqui

    [HideInInspector] public Vector2 direcao;
    [HideInInspector] public float distanciaParada;
    [HideInInspector] public Vector2 pontoSpawn;

    private float tempoEspera = 0f;
    private bool parou;
    private bool recolhendo;
    private bool temComida;

    void Start()
    {
        // Pega o SpriteRenderer da ponta caso não tenha sido arrastado no Inspector
        if (spriteRendererAnzol == null && pontaAnzol != null)
            spriteRendererAnzol = pontaAnzol.GetComponent<SpriteRenderer>();

        // Sorteia se o anzol nasce com comida
        temComida = Random.value <= chanceDeVimComida;

        // Aplica o sprite correspondente
        if (spriteRendererAnzol != null)
        {
            spriteRendererAnzol.sprite = temComida ? spriteAnzolComComida : spriteAnzolNormal;
        }
    }

    void Update()
    {
        // --- LÓGICA DE RETORNO AUTOMÁTICO ---
        if (parou && !recolhendo)
        {
            tempoEspera += Time.deltaTime;
            
            // Se passou o tempo mínimo, inicia o recolhimento automaticamente
            if (tempoEspera >= tempoEsperaMinimo)
            {
                IniciarRecolhimento();
                tempoEspera = 0f; // Reseta para evitar cálculos desnecessários
            }
        }

        // --- LÓGICA DE MOVIMENTO ---
        if (recolhendo)
        {
            pontaAnzol.position -= (Vector3)(direcao * velocidade * multiplicadorVelocidadeRetorno * Time.deltaTime);

            // Condição de parada segura
            float distRestante = Vector2.Distance(pontoSpawn, pontaAnzol.position);
            // Verifica se já chegou perto ou se está se movendo na direção errada (caso de erro de cálculo)
            if (distRestante <= 0.2f || Vector2.Dot(direcao, (Vector2)pontaAnzol.position - pontoSpawn) <= 0)
            {
                LimparEDestruir();
            }
        }
        else if (!parou)
        {
            pontaAnzol.position += (Vector3)(direcao * velocidade * Time.deltaTime);

            if (Vector2.Distance(pontoSpawn, pontaAnzol.position) >= distanciaParada)
            {
                pontaAnzol.position = pontoSpawn + direcao * distanciaParada;
                parou = true;
                tempoEspera = 0f; // Inicia a contagem da espera
            }
        }

        AtualizarLinha();
    }

    // Centraliza a lógica de limpeza e destruição
    void LimparEDestruir()
    {
        if (linha != null) linha.enabled = false;
        if (colisorLinha != null) colisorLinha.enabled = false;
        Destroy(gameObject);
    }

    void AtualizarLinha()
    {
        // Usa o PontoDeAmarra se ele existir, senão usa o próprio transform da ponta
        Vector2 pontaReal = (pontoDeAmarra != null) ? (Vector2)pontoDeAmarra.position : (Vector2)pontaAnzol.position;
        Vector2 meio = (pontoSpawn + pontaReal) / 2f;
        float distancia = Vector2.Distance(pontoSpawn, pontaReal);
        float angulo = Mathf.Atan2(pontaReal.y - pontoSpawn.y, pontaReal.x - pontoSpawn.x) * Mathf.Rad2Deg;

        pontaAnzol.rotation = Quaternion.Euler(0, 0, angulo + 90f);

        if (colisorLinha != null)
        {
            colisorLinha.transform.position = meio;
            colisorLinha.transform.rotation = Quaternion.Euler(0, 0, angulo);
            colisorLinha.offset = Vector2.zero;
            colisorLinha.size = new Vector2(distancia, 0.3f);
        }

        if (linha != null)
        {
            // Garante que a linha não seja desenhada se o objeto for destruído no mesmo frame
            if (linha.gameObject.activeSelf)
            {
                linha.SetPosition(0, pontoSpawn);
                linha.SetPosition(1, pontaReal);
            }
        }
    }

    public void IniciarRecolhimento()
    {
        if (recolhendo) return;

        recolhendo = true;

        // Desativa o colisor para evitar dano duplo ou colisão fantasma
        if (colisorLinha != null) 
            colisorLinha.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var player = other.GetComponentInParent<PlayerScript>();
        if (player == null) return;

        if (player.isDashing)
        {
            // Se o peixe deu dash e o anzol tem comida
            if (temComida)
            {
                temComida = false; // Remove a comida
                
                // Troca o sprite visual na hora para mostrar que a comida foi comida
                if (spriteRendererAnzol != null && spriteAnzolNormal != null)
                    spriteRendererAnzol.sprite = spriteAnzolNormal;

                // Adiciona os 15 pontos no Singleton do PontosUI
                if (PontosUI.Instance != null)
                {
                    PontosUI.Instance.AdicionarPontos(pontosComida);
                }
            }
        }
        else
        {
            // Sem dash: toma dano normal, mesmo se tiver comida
            player.TakeDamage();
        }

        IniciarRecolhimento();
    }
}