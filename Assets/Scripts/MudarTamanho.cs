using UnityEngine;
using System.Collections;
using UnityEngine.UI; 

public class MudarTamanho : MonoBehaviour
{
    [Header("Crescer")]
    [SerializeField] private float quantidadeCrescimento = 0.4f;
    [SerializeField] private float tamanhoMaximo = 5.0f;
    
    [Header("Diminuir")]
    [SerializeField] private float tamanhoMinimo = 0.8f; // Adicionado para limitar o menor tamanho
     [SerializeField] private float quantidadeReducao = 0.8f;
    private float tamanhoAtual = 1.0f;
    public float TamanhoAtual => tamanhoAtual;
    
    // Função existente de crescimento
    public void Crescer()
    {
        if (tamanhoAtual < tamanhoMaximo)
        {
            tamanhoAtual = Mathf.Min(tamanhoAtual + quantidadeCrescimento, tamanhoMaximo);
            AplicarEscala();
            Debug.Log("Cresceu! Novo tamanho base: " + tamanhoAtual);
        }
    }

    // NOVA FUNÇÃO: Adicionada para o item de diminuir usar!
    public void Diminuir()
    {
        if (tamanhoAtual > tamanhoMinimo)
        {
            // Subtrai a mesma quantidade (ou você pode criar uma variável 'quantidadeReducao' se preferir)
            tamanhoAtual = Mathf.Max(tamanhoAtual - quantidadeReducao, tamanhoMinimo);
            AplicarEscala();
            Debug.Log("Diminuiu! Novo tamanho base: " + tamanhoAtual);
        }
    }

    // Atalho para não repetir código de escala
    private void AplicarEscala()
    {
        float direcaoX = transform.localScale.x >= 0 ? 1f : -1f;
        transform.localScale = new Vector3(tamanhoAtual * direcaoX, tamanhoAtual, 1f);
    }
}
