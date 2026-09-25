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

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        pontuacaoTotal = 0;
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
        pontuacaoTotal = 0;
        AtualizarUI();
        tempoAtual = 0f; // Reseta o tempo também
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