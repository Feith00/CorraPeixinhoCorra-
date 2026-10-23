using UnityEngine;
using System.Collections;
using UnityEngine.UI; 


public class CrescimentoPeixinho : MonoBehaviour
{
    [SerializeField] private float quantidadeCrescimento = 0.1f;
    [SerializeField] private float tamanhoMaximo = 3.0f;
    private float tamanhoAtual = 1.0f;
    public float TamanhoAtual => tamanhoAtual;
    
    public void Crescer()
    {
        if (tamanhoAtual < tamanhoMaximo)
        {
            tamanhoAtual += quantidadeCrescimento;
            
            float direcaoX = transform.localScale.x >= 0 ? 1f : -1f;
            transform.localScale = new Vector3(tamanhoAtual * direcaoX, tamanhoAtual, 1f);
            
            Debug.Log("Cresceu, Novo tamanho base: " + tamanhoAtual);
        }
    }
}
    