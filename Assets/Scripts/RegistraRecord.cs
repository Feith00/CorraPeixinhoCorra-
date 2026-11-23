
using UnityEngine;

public class RegistrarRecorde : MonoBehaviour
{
    private PlayerScript player;
    private RecordManager leaderboard;

    private bool jaRegistrou = false;

    private void Awake()
    {
        player = GetComponent<PlayerScript>();
    }

    public void RegistrarPontuacaoFinal()
    {
        if (jaRegistrou)
            return;

        if (player == null || player.minhaPontuacaoUI == null)
        {
            Debug.LogWarning("Pontuação do jogador não encontrada.");
            return;
        }

        // Procura o LeaderboardManager na cena
        leaderboard = FindFirstObjectByType<RecordManager>();

        if (leaderboard == null)
        {
            Debug.LogWarning("LeaderboardManager não encontrado!");
            return;
        }

        // Recupera o nome salvo antes da partida
        string nome = PlayerPrefs.GetString("NomeJogador", "JOGADOR");

        int pontuacao = player.minhaPontuacaoUI.PontuacaoTotal;

        leaderboard.AdicionarResultado(nome, pontuacao);

        jaRegistrou = true;

        Debug.Log("Recorde registrado: " + nome + " - " + pontuacao);
    }
}