using UnityEngine;

public class anzoMovimento : MonoBehaviour
{
    [Header("Configurações da Movimentação")]
    public Rigidbody2D rigidbody2D;
    public float velocidade = 5f;
    public float direcaoY = 1f;
    public float direcaoX = 1f;
    private Vector2 direcao;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        direcao = new Vector2(direcaoX,direcaoY).normalized;
        rigidbody2D = GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
        Movimento();
    }
    void Movimento()
    {
        rigidbody2D.linearVelocity = new Vector2(direcao.x *velocidade, direcao.y * velocidade );
    }
}
