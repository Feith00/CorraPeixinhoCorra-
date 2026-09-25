using UnityEngine;

public class PointScript : MonoBehaviour
{
    [SerializeField] private int pontosDoItem = 10; 

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
        
            PontosUI.Instance.AdicionarPontos(pontosDoItem);
            
        
            Destroy(gameObject);
        }
    }
}