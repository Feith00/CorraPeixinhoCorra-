using UnityEngine;

public class CollectibleInvincibility : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private float duration = 5f; // Tempo de imortalidade em segundos

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica se o objeto que colidiu tem a Tag "Player"
        if (collision.CompareTag("Player"))
        {
            // Busca o script do Player
            PlayerScript player = collision.GetComponent<PlayerScript>();

            if (player != null)
            {
                // Chama a nova função de imortalidade que adicionamos ao seu Player
                player.ActivatePowerUpInvincibility(duration);
                
                // Destrói o item coletável da cena
                Destroy(gameObject);
            }
        }
    }
}
