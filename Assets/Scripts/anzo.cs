using Unity.VisualScripting;
using UnityEngine;

public class ExpandirHitbox : MonoBehaviour
{
    private BoxCollider2D meuCollider;
    
    [Header("Configurações da Hitbox")]
    public Vector2 tamanhoInicial = new Vector2(1f, 1f);
    public Vector2 tamanhoMaximo = new Vector2(1f, 10f);
    public float velocidadeExpansao = 0.5f;
    


    void Start()
    {
        // Pega a referência do Collider do próprio objeto

        meuCollider = GetComponent<BoxCollider2D>();
        meuCollider.size = tamanhoInicial;
    }

    void Update()
    {
    
        AumentarHitboxAoMover();
        
    }
    
    void AumentarHitboxAoMover()
    {
        // Se o tamanho atual for menor que o máximo, ele expande com o tempo
        if ( meuCollider.size.y < tamanhoMaximo.y)
        {
            //float novoX = Mathf.MoveTowards(meuCollider.size.x, tamanhoMaximo.x, velocidadeExpansao * Time.deltaTime);
            float novoY = Mathf.MoveTowards(meuCollider.size.y, tamanhoMaximo.y, velocidadeExpansao * Time.deltaTime);
            
            meuCollider.size = new Vector2(1, novoY);
        }
    }

    // Opcional: Reseta o tamanho quando o objeto parar ou sumir
    public void ResetarHitbox()
    {
        meuCollider.size = tamanhoInicial;
    }
}
