using UnityEngine;
using System.Collections;
using UnityEngine.UI; 


public class DiminuirPeixinho : MonoBehaviour
{
    [SerializeField] private float quantidadeReducao = -0.2f;
    [SerializeField] private float tamanhoMaximo = 1.0f;
    private float tamanhoAtual = 1.0f;
    public float TamanhoAtual => tamanhoAtual;
    
    public void Diminuiu()
    {
        if (tamanhoAtual > tamanhoMaximo)
        {
            tamanhoAtual += quantidadeReducao;
            
            float direcaoX = transform.localScale.x >= 0 ? 1f : -1f;
            transform.localScale = new Vector3(tamanhoAtual * direcaoX, tamanhoAtual, 1f);
            
            Debug.Log("Diminuiu, Novo tamanho base: " + tamanhoAtual);
        }
    }
}
    