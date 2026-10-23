using UnityEngine;

public class AnzolSpawner : MonoBehaviour
{
    public GameObject prefabAnzol; // prefab com o AnzolController já configurado
    public GameObject prefabIndicadorSeta; // ARRASTE O PREFAB DA SETA AQUI
    public Camera cameraJogo;

    [Header("Configuração de spawn")]
    public float margemForaDaTela = 4f;   // quão longe fora da tela o anzol nasce
    public float margemDentroDaTela = 1f; // não deixa o ponto de parada ficar colado na borda

    [Header("Dificuldade Progressiva")]
    public float intervaloInicial = 1.5f; // Tempo inicial entre spawns
    public float intervaloMinimo = 0.2f;  // Limite para o jogo não travar gerando anzóis demais
    public float tempoParaDobrarVelocidade = 60f; // Tempo em segundos para a frequência dobrar (1 minuto)

    [Header("Velocidade dos Anzóis")]
    public float velocidadeMinima = 3f;   // Velocidade mais lenta de entrada
    public float velocidadeMaxima = 8f;   // Velocidade mais rápida de entrada

    private float timerSpawn = 0f;
    private float tempoDeJogo = 0f;

    void Start()
    {
        if (cameraJogo == null) cameraJogo = Camera.main;
    }

    void Update()
    {
        // Conta o tempo total de partida
        tempoDeJogo += Time.deltaTime;

        // Calcula o intervalo dinâmico: reduz pela metade a cada 'tempoParaDobrarVelocidade' (60s)
        float fatorAceleracao = Mathf.Pow(2f, -tempoDeJogo / tempoParaDobrarVelocidade);
        float intervaloAtual = Mathf.Max(intervaloInicial * fatorAceleracao, intervaloMinimo);

        // Timer manual para disparar os spawns
        timerSpawn += Time.deltaTime;

        if (timerSpawn >= intervaloAtual)
        {
            SpawnarAnzol();
            timerSpawn = 0f;
        }
    }

    void SpawnarAnzol()
    {
        float altura = cameraJogo.orthographicSize;
        float largura = altura * cameraJogo.aspect; 
        Vector2 centro = cameraJogo.transform.position;

        int borda = Random.Range(0, 4);
        Vector2 pontoSpawn = ObterPontoSpawn(borda, centro, largura, altura);

        float xAlvo = Random.Range(centro.x - largura + margemDentroDaTela, centro.x + largura - margemDentroDaTela);
        float yAlvo = Random.Range(centro.y - altura + margemDentroDaTela, centro.y + altura - margemDentroDaTela);
        Vector2 pontoAlvo = new Vector2(xAlvo, yAlvo);

        Vector2 direcao = (pontoAlvo - pontoSpawn).normalized;
        float distancia = Vector2.Distance(pontoSpawn, pontoAlvo);

        GameObject obj = Instantiate(prefabAnzol, pontoSpawn, Quaternion.identity);
        AnzolController controlador = obj.GetComponent<AnzolController>();
        
        if (controlador != null)
        {
            controlador.pontoSpawn = pontoSpawn;
            controlador.direcao = direcao;
            controlador.distanciaParada = distancia;
            controlador.velocidade = Random.Range(velocidadeMinima, velocidadeMaxima);
            // === CRIAÇÃO DO INDICADOR DE PERIGO CORRIGIDA ===
            if (prefabIndicadorSeta != null)
            {
                GameObject indObj = Instantiate(prefabIndicadorSeta);
                IndicadorPerigo indicador = indObj.GetComponent<IndicadorPerigo>();
                if (indicador != null)
                {
                    // Agora passa o pontoSpawn e o pontoAlvo para traçar a linha perfeita!
                    indicador.Configurar(controlador.pontaAnzol, pontoSpawn, pontoAlvo, direcao);
                }
            }
        }
    }

    Vector2 ObterPontoSpawn(int borda, Vector2 centro, float largura, float altura)
    {
        switch (borda)
        {
            case 0: // cima
                return new Vector2(Random.Range(centro.x - largura, centro.x + largura), centro.y + altura + margemForaDaTela);
            case 1: // baixo
                return new Vector2(Random.Range(centro.x - largura, centro.x + largura), centro.y - altura - margemForaDaTela);
            case 2: // esquerda
                return new Vector2(centro.x - largura - margemForaDaTela, Random.Range(centro.y - altura, centro.y + altura));
            default: // direita
                return new Vector2(centro.x + largura + margemForaDaTela, Random.Range(centro.y - altura, centro.y + altura));
        }
    }
}