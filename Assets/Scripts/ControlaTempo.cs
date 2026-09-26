using UnityEngine;
using System.Collections;

public class ControlaTempo : MonoBehaviour 
{
    [SerializeField] private float velocidadeReduzida = 0.5f;
    [SerializeField] private float duracaoCameraLenta = 5.0f;
    [SerializeField] private float fixedDeltaTimeNormal = 0.02f;

    private bool emCameraLenta = false;
    private Coroutine cameraLentaCoroutine = null;

    private void OnTriggerEnter2D(Collider2D col) // Corrigido para Collider2D
    {
        // Corrigido: Assegure-se de que a tag "Player" está entre aspas simples/duplas corretamente
        if (col.CompareTag("Player")) 
        {
            GetComponent<Collider2D>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
            if (cameraLentaCoroutine != null)
            {
                StopCoroutine(cameraLentaCoroutine);
            }
            IniciarCameraLenta();
        }
    }

    private void IniciarCameraLenta()
    {
        if (emCameraLenta) return;
        emCameraLenta = true;
        cameraLentaCoroutine = StartCoroutine(CicloCameraLenta());
    }

    private IEnumerator CicloCameraLenta()
    {
        AtivarCameraLentaLogic();
        
        // OBRIGATÓRIO: Usar tempo real, senão os 5 segundos viram 10 segundos no jogo desacelerado
        yield return new WaitForSecondsRealtime(duracaoCameraLenta);


        DesativarCameraLentaLogic();
        
        // Destrói o objeto após normalizar o tempo de forma segura
        Destroy(this.gameObject);
    }

    private void AtivarCameraLentaLogic()
    {
        Time.timeScale = velocidadeReduzida;
        Time.fixedDeltaTime = fixedDeltaTimeNormal * velocidadeReduzida;
        
    }

    private void DesativarCameraLentaLogic()
    {
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = fixedDeltaTimeNormal;
        emCameraLenta = false;
        cameraLentaCoroutine = null;
    }

    private void OnDisable()
    {
        // Se o objeto for destruído ou desativado inesperadamente, garante que o jogo não fique em câmera lenta para sempre
        DesativarCameraLentaLogic();
    }
}
