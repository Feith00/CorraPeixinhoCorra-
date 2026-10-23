using UnityEngine;

public class Perseguidor : MonoBehaviour
{
        [Header("Configurações de Movimento")]
        [SerializeField] private float velocidade = 5f;
        [SerializeField] private string tagDojogador = "Player";

        private Transform transformJogador;
        private Rigidbody2D rb;
        private Vector2 direcao;
        private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        BuscarJogadorPorTag();
    }
    private void Update()
    {

        if (transformJogador != null)
        {
            CalcularDirecaoAteJogador();
        }
    }

    
    private void FixedUpdate()
    {
        if (transformJogador != null)
        {
            MoverEmDirecaoAoJogador();
        }
        else
        {
            PararMovimento();
        }
    }


    private void BuscarJogadorPorTag()
    {
        GameObject jogadorObjeto = GameObject.FindGameObjectWithTag(tagDojogador);
        if (jogadorObjeto != null)
        {
            transformJogador = jogadorObjeto.transform;
        }
        else
        {
            Debug.LogWarning($"Aviso: Nenhum objeto com a tag '{tagDojogador}' foi encontrado na cena!");
        }
    }
    private void CalcularDirecaoAteJogador()
    {
        Vector2 posicaoAtual = transform.position;
        Vector2 posicaoJogador = transformJogador.position;

        direcao = (posicaoJogador - posicaoAtual).normalized;
    }

    private void MoverEmDirecaoAoJogador()
    {
        rb.MovePosition(rb.position + direcao * velocidade * Time.fixedDeltaTime);
    }
  

    private void PararMovimento()
    {
        rb.linearVelocity = Vector2.zero;
    }
}
