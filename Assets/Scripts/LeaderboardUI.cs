
using TMPro;
using UnityEngine;

public class LeaderboardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text textoRanking;

    private RecordManager records;

    private void OnEnable()
    {
        records = FindFirstObjectByType<RecordManager>();
        AtualizarRanking();
    }

    public void AtualizarRanking()
    {
        if (records == null)
            return;

        string texto = "";

        for (int i = 0; i < 10; i++)
        {
            if (i < records.ranking.Count)
            {
                RecordEntry jogador = records.ranking[i];

                texto += $"{i + 1:00}   {jogador.nome}   {jogador.pontuacao}\n";
            }
            else
            {
                texto += $"{i + 1:00}   ---   ---\n";
            }
        }

        textoRanking.text = texto;
    }
}
