using UnityEngine;
using TMPro;

public class PontosUI : MonoBehaviour
{
    public static PontosUI Instance;
    private int pontuacaoTotal;
    public TMP_Text textoPontuacaoUI;
    
    // Configuração do ganho automático de pontos
    public int pontosPorIntervalo = 10;      // Quantos pontos ganha a cada intervalo
    public float intervaloSegundos = 5f;     // Tempo em segundos entre os ganhos
    
    private float tempoAtual = 0f;
    private PlayerScript player;

    void Awake()
    {
        if (Instance == null) 
        {
        Instance = this;
        }
        else
        {
         Destroy(this);
         return;
        }
        
    }

    void Start()
    {
        pontuacaoTotal = 0;
        BuscarPlayer();
        AtualizarUI();
        
    }

    void Update()
    {
        // Lógica de ganho automático de pontos
        tempoAtual += Time.deltaTime;
        
        if (tempoAtual >= intervaloSegundos)
        {
            AdicionarPontos(pontosPorIntervalo);
            tempoAtual = 0f; // Reseta o contador
        }
    }

    public void AdicionarPontos(int pontos)
    {
        pontuacaoTotal += pontos;
        AtualizarUI();
    }

    public void ResetarPontos()
    {
        Time.timeScale = 1f;
        pontuacaoTotal = 0;
        tempoAtual = 0f;
        BuscarPlayer();
        AtualizarUI();
    }
    private void BuscarPlayer()
    {
        player = GameObject.FindAnyObjectByType<PlayerScript>();
    }
    // Método para ajustar dinamicamente o intervalo
    public void AlterarIntervalo(float novoIntervalo)
    {
        intervaloSegundos = novoIntervalo;
    }

    // Método para ajustar dinamicamente os pontos por intervalo
    public void AlterarPontosPorIntervalo(int novosPontos)
    {
        pontosPorIntervalo = novosPontos;
    }

    private void AtualizarUI()
    {
      
        if (textoPontuacaoUI != null)
            textoPontuacaoUI.text = "Pontos: " + pontuacaoTotal;
    }
}