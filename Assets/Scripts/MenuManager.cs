using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Painéis do Menu")]
    [SerializeField] private GameObject painelPrincipal;
    [SerializeField] private GameObject painelJogadores;

    private void Start()
    {
        // Garante que o menu sempre comece na tela inicial
        VoltarParaMenuPrincipal();
    }

    // Conecte este método ao botão "JOGAR" do PainelPrincipal
    public void AbrirSelecaoJogadores()
    {
        if (painelPrincipal != null) painelPrincipal.SetActive(false);
        if (painelJogadores != null) painelJogadores.SetActive(true);
    }

    // Conecte este método ao botão "VOLTAR" do PainelJogadores
    public void VoltarParaMenuPrincipal()
    {
        if (painelPrincipal != null) painelPrincipal.SetActive(true);
        if (painelJogadores != null) painelJogadores.SetActive(false);
    }

    // Conecte este método aos botões 1, 2, 3 e 4 jogadores
    // No Inspector do botão OnClick(), digite 1, 2, 3 ou 4 no campo que aparecer!
    public void SelecionarQuantidadeJogadores(int quantidade)
    {
        // Salva a quantidade escolhida para a cena "Jogo" saber quantos peixes criar
        PlayerPrefs.SetInt("QuantidadeJogadores", quantidade);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Jogo");
    }

    public void Sair()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}