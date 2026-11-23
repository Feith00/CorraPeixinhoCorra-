using System;
using UnityEngine;

[Serializable]
public class RecordEntry : MonoBehaviour
{
    public string nome;
    public int pontuacao;

    public RecordEntry(string nome, int pontuacao)
    {
        this.nome = nome;
        this.pontuacao = pontuacao;
    }
}