using UnityEngine;

public class DobraPonto : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private float duration = 5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Busca o componente no Player que controla os pontos
            PlayerPointsHandler handler = collision.GetComponent<PlayerPointsHandler>();

            if (handler != null)
            {
                // Ativa o efeito de dobrar os pontos no jogador
                handler.AtivarDobra(duration);
                
                // Destrói este item coletável da cen
                Destroy(gameObject);
            }
        }
    }
}
