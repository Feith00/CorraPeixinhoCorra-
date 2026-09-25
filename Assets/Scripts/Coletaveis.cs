using UnityEngine;
using System.Threading.Tasks;

public class GeradorColetavel : MonoBehaviour
{
    public GameObject prefabColetavel; 
    public float tempoGeracao = 2.0f;  
    public Transform pontoSpawn;       
    
    [Header("Configurações de Aleatoriedade")]
    public bool usarAleatoriedade = true;
    public float varianciaX = 2.0f; 
    public float varianciaY = 1.0f; 

    [Header("Tempo de Vida")]
    public float tempodeVida = 5.0f;

    private float cronometro;

    void Start()
    {
        cronometro = tempoGeracao;
    }

    async Task Update()
    {
        cronometro -= Time.deltaTime;

        if (cronometro <= 0f)
        {
            Vector3 posicaoFinal = pontoSpawn.position;

            if (usarAleatoriedade)
            {
                float offsetX = Random.Range(-varianciaX, varianciaX);
                float offsetY = Random.Range(-varianciaY, varianciaY);
                
                posicaoFinal += new Vector3(offsetX, offsetY, 0);
                
                

            }

            GameObject coletavel = Instantiate(prefabColetavel, posicaoFinal, Quaternion.identity);
            Destroy(coletavel, tempodeVida);
            
            cronometro = tempoGeracao;


        }
    }
}