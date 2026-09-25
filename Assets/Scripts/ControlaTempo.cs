using UnityEngine;
using System.Collections; 
public class ControlaTempo : MonoBehaviour
{
    [SerializeField] private float velocidadeReduzida = 0.5f;
    [SerializeField] private float duracaoCameraLenta = 5.0f;
    [SerializeField] private float fixedDeltaTimeNormal = 0.02f;

    private bool emCameraLenta = false;
    private Coroutine cameraLentaCoroutine = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (cameraLentaCoroutine != null)
                StopCoroutine(cameraLentaCoroutine);

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

        
        yield return new WaitForSeconds(duracaoCameraLenta);

       
        DesativarCameraLentaLogic();
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
      
        if (cameraLentaCoroutine != null)
            StopCoroutine(cameraLentaCoroutine);
        
        DesativarCameraLentaLogic();
    }
}