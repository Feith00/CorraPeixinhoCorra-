
using UnityEngine;
using TMPro;

public class NomeJogadorUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField campoNome;

    public void SalvarNome()
    {
        string nome = campoNome.text.Trim().ToUpper();

        if (string.IsNullOrEmpty(nome))
        {
            Debug.LogWarning("Digite um nome!");
            return;
        }

        PlayerPrefs.SetString("NomeJogador", nome);
        PlayerPrefs.Save();

        Debug.Log("Nome salvo: " + nome);
    }
}