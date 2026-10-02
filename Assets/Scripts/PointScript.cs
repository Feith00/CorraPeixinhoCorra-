using UnityEngine;

public class PointScript : MonoBehaviour
{
    [SerializeField] private int pontosDoItem = 10; 

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            // Verifica se o Player tem o componente que gerencia a dobra de pontos
            PlayerPointsHandler handler = col.GetComponent<PlayerPointsHandler>();
            
            int pontosFinais = pontosDoItem;

            // Se o componente existir e o power-up estiver ativo, dobra o valor
            if (handler != null && handler.isPowerUpDobrar)
            {
                pontosFinais *= 2;
            }

            // Envia a pontuação correta para a sua UI
            if (PontosUI.Instance != null)
            {
                PontosUI.Instance.AdicionarPontos(pontosFinais);
            }
            
            // Destrói o item coletável
            Destroy(gameObject);
        }
    }
}
