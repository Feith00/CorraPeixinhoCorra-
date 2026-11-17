using UnityEngine;
using TMPro;

public class PontosUI : MonoBehaviour
{
    private int pontuacaoTotal;
    public TMP_Text textoPontuacaoUI;

    [Header("Ganho Automático de Pontos")]
    public int pontosPorIntervalo = 10;
    public float intervaloSegundos = 5f;

    private float tempoAtual = 0f;
    private PlayerScript donoDestaHUD;

    public void VincularJogador(PlayerScript player)
    {
        donoDestaHUD = player;
        ResetarPontos();
    }

    void Update()
    {
        // Só ganha pontos por tempo se o jogador desta HUD estiver vivo e ativo
        if (donoDestaHUD == null || !donoDestaHUD.gameObject.activeInHierarchy || donoDestaHUD.EstaMorto())
            return;

        tempoAtual += Time.deltaTime;

        if (tempoAtual >= intervaloSegundos)
        {
            AdicionarPontos(pontosPorIntervalo);
            tempoAtual = 0f;
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
        tempoAtual = 0f;
        AtualizarUI();
    }

    public void AlterarIntervalo(float novoIntervalo) => intervaloSegundos = novoIntervalo;
    public void AlterarPontosPorIntervalo(int novosPontos) => pontosPorIntervalo = novosPontos;

    private void AtualizarUI()
    {
        if (textoPontuacaoUI != null)
            textoPontuacaoUI.text = "PONTOS: " + pontuacaoTotal;
    }
}