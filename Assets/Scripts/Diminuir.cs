using UnityEngine;

public class Diminuir : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            // Procura pelo novo script unificado no Player (ou nos pais/filhos por segurança)
            MudarTamanho scriptTamanho = col.GetComponent<MudarTamanho>();
            if (scriptTamanho == null) scriptTamanho = col.GetComponentInParent<MudarTamanho>();
            if (scriptTamanho == null) scriptTamanho = col.GetComponentInChildren<MudarTamanho>();

            if (scriptTamanho != null)
            {
                // Chama a função que você adicionou
                scriptTamanho.Diminuir();
            }
            else
            {
                Debug.LogError("ERRO: O script 'MudarTamanho' não foi encontrado no Player ou em seus filhos!");
            }

            // Destrói o item coletável da cena
            Destroy(gameObject);
        }
    }
}
