using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathManager : MonoBehaviour
{
    [Header("Painéis do Menu")]
    [SerializeField] private GameObject painelDeMorte;
    public void VoltarParaMenuPrincipal()
    {
        if (painelDeMorte != null) painelDeMorte.SetActive(true);
        SceneManager.LoadScene("Menu");
    }
}
