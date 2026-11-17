using UnityEngine;

public class PointScript : MonoBehaviour
{
    [SerializeField] private int pontosDoItem = 10;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            PlayerPointsHandler handler = col.GetComponentInParent<PlayerPointsHandler>();
            PlayerScript player = col.GetComponentInParent<PlayerScript>();
            MudarTamanho peixe = col.GetComponentInParent<MudarTamanho>();

            int pontosFinais = pontosDoItem;

            if (peixe != null)
                peixe.Crescer();

            if (handler != null && handler.isPowerUpDobrar)
                pontosFinais *= 2;

            // Envia os pontos APENAS para a UI do jogador que coletou o item
            if (player != null && player.minhaPontuacaoUI != null)
            {
                player.minhaPontuacaoUI.AdicionarPontos(pontosFinais);
            }

            Destroy(gameObject);
        }
    }
}