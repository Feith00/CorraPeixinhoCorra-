using UnityEngine;

public class AnzolSpawner : MonoBehaviour
{
    public GameObject prefabAnzol; // prefab com o AnzolController já configurado
    public Camera cameraJogo;

    [Header("Configuração de spawn")]
    public float margemForaDaTela = 1f;   // quão longe fora da tela o anzol nasce
    public float margemDentroDaTela = 1f; // não deixa o ponto de parada ficar colado na borda
    public float intervaloEntreSpawns = 1.5f;

    

    void Start()
    {
        if (cameraJogo == null) cameraJogo = Camera.main;
        InvokeRepeating(nameof(SpawnarAnzol), 1f, intervaloEntreSpawns);
    }

    void SpawnarAnzol()
    {
        float altura = cameraJogo.orthographicSize;
        // O correto em 2D é multiplicar orthographicSize pela relação de aspecto:
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
        
        // Define explicitamente os dados no controlador recém-criado:
        controlador.pontoSpawn = pontoSpawn;
        controlador.direcao = direcao;
        controlador.distanciaParada = distancia;
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
