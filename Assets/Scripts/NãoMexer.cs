 using UnityEngine;
using System.Collections;
using UnityEngine.UI; 
 public class PlayerPointsHandler : MonoBehaviour
{
    // Indica se o modificador de dobrar pontos está ativo
    public bool isPowerUpDobrar { get; private set; } = false;

    public void AtivarDobra(float duration)
    {
        // Evita bugs caso pegue outro coletável antes do primeiro acabar
        StopAllCoroutines(); 
        StartCoroutine(DobraPontosRoutine(duration));
    }

    private IEnumerator DobraPontosRoutine(float duration)
    {
        isPowerUpDobrar = true;
        Debug.Log("Pontos em dobro ATIVADO!");

        yield return new WaitForSeconds(duration);

        isPowerUpDobrar = false;
        Debug.Log("Pontos em dobro ACABOU!");
    }
}