
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RecordManager : MonoBehaviour
{
    public List<RecordEntry> ranking = new List<RecordEntry>();

    private const int MAX_JOGADORES = 10;

    private string CaminhoArquivo =>
        Path.Combine(Application.persistentDataPath, "leaderboard.json");

    private void Awake()
    {
        if (ranking == null)
        {
            ranking = new List<RecordEntry>();
        }

        CarregarRanking();
    }

    public void AdicionarResultado(string nome, int pontuacao)
    {
        RecordEntry novoJogador = new RecordEntry(nome, pontuacao);

        ranking.Add(novoJogador);

        ranking.Sort((a, b) =>
            b.pontuacao.CompareTo(a.pontuacao));

        if (ranking.Count > MAX_JOGADORES)
        {
            ranking.RemoveAt(ranking.Count - 1);
        }

        SalvarRanking();
    }

    private void SalvarRanking()
    {
        LeaderboardData dados = new LeaderboardData();
        dados.ranking = ranking;

        string json = JsonUtility.ToJson(dados, true);

        File.WriteAllText(CaminhoArquivo, json);

        Debug.Log("Ranking salvo em: " + CaminhoArquivo);
    }

    private void CarregarRanking()
    {
        Debug.Log("Caminho do arquivo: " + CaminhoArquivo);

        if (!File.Exists(CaminhoArquivo))
        {
            Debug.Log("Arquivo não encontrado. Criando ranking vazio.");
            ranking = new List<RecordEntry>();
            return;
        }

        Debug.Log("Arquivo encontrado!");

        string json = File.ReadAllText(CaminhoArquivo);
        Debug.Log("JSON lido: " + json);

        LeaderboardData dados =
            JsonUtility.FromJson<LeaderboardData>(json);

        if (dados == null || dados.ranking == null)
        {
            Debug.LogError("Os dados do ranking estão nulos!");
            ranking = new List<RecordEntry>();
            return;
        }

        dados.ranking.RemoveAll(jogador => jogador == null);
        ranking = dados.ranking;

        Debug.Log("Ranking carregado: " + ranking.Count);
    }
}

[System.Serializable]
public class LeaderboardData
{
    public List<RecordEntry> ranking = new List<RecordEntry>();
}
