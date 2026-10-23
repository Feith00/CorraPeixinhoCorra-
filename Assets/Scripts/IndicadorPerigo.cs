using UnityEngine;

public class IndicadorPerigo : MonoBehaviour
{
    private Transform anzolAlvo;
    private Camera cam;

    [Header("Ajuste de Rotação")]
    [SerializeField] private float offsetRotacao = -90f; // Ajuste para a seta apontar na direção certa

    public void Configurar(Transform anzol, Vector2 pontoSpawn, Vector2 pontoAlvo, Vector2 direcao)
    {
        anzolAlvo = anzol;
        cam = Camera.main;

        if (cam == null) cam = Camera.main;

        // 1. Aplica a rotação apontando na direção que o anzol vai se mover
        float angulo = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angulo + offsetRotacao);

        // 2. Trava o indicador exatamente na borda da visão da Câmera
        PosicionarNaBordaExata(pontoSpawn, pontoAlvo);
    }

    void Update()
    {
        // Se o anzol for destruído antes de entrar na tela, destrói o indicador
        if (anzolAlvo == null)
        {
            Destroy(gameObject);
            return;
        }

        // Destrói o indicador assim que o anzol entra na visão do jogador
        if (AnzolEntrouNaTela())
        {
            Destroy(gameObject);
        }
    }

    void PosicionarNaBordaExata(Vector2 spawn, Vector2 alvo)
    {
        if (cam == null) return;

        // Converte a origem (fora da tela) e o destino (dentro da tela) para o espaço da Câmera (0 a 1)
        Vector3 vpSpawn = cam.WorldToViewportPoint(spawn);
        Vector3 vpAlvo = cam.WorldToViewportPoint(alvo);

        // Encontra o ponto na linha reta (entre spawn e alvo) que encosta na borda da câmera (0.05 a 0.95)
        Vector3 vpBorda = vpSpawn;

        for (float t = 0f; t <= 1f; t += 0.02f)
        {
            Vector3 pontoPelaLinha = Vector3.Lerp(vpSpawn, vpAlvo, t);

            // Verifica se este ponto na linha entrou na margem visível da tela
            if (pontoPelaLinha.x >= 0.05f && pontoPelaLinha.x <= 0.95f &&
                pontoPelaLinha.y >= 0.05f && pontoPelaLinha.y <= 0.95f)
            {
                vpBorda = pontoPelaLinha;
                break; // Achou a borda exata da tela!
            }
        }

        // Reconverte de Viewport da Câmera para coordenadas do Mundo
        Vector3 posMundo = cam.ViewportToWorldPoint(vpBorda);
        posMundo.z = 0f;

        transform.position = posMundo;
    }

    bool AnzolEntrouNaTela()
    {
        Vector3 posVP = cam.WorldToViewportPoint(anzolAlvo.position);
        
        // Quando o anzol cruza a margem da câmera (entre 2% e 98% da tela), a seta some
        return posVP.x >= 0.02f && posVP.x <= 0.98f && posVP.y >= 0.02f && posVP.y <= 0.98f;
    }
}