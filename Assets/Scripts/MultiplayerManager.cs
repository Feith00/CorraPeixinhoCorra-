using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MultiplayerManager : MonoBehaviour
{
    public static MultiplayerManager Instance;

    [System.Serializable]
    public struct ConfiguracaoJogador
    {
        public string nome;
        public Transform pontoSpawn;
        public GameObject painelHUD;
        public Image[] coracoesHUD;
        public PontosUI pontosHUD;

        [Header("Teclas deste Jogador")]
        public KeyCode cima;
        public KeyCode baixo;
        public KeyCode esquerda;
        public KeyCode direita;
        public KeyCode dash;
    }

    [Header("Prefab do Peixe")]
    [SerializeField] private GameObject prefabPeixe;

    [Header("Configuração dos 4 Jogadores")]
    [SerializeField] private ConfiguracaoJogador[] jogadores = new ConfiguracaoJogador[4];

    [Header("Fim de Jogo e Spawners")]
    [SerializeField] private GameObject painelGameOver; // Arraste o painel/imagens da tela de morte aqui
    [SerializeField] private GameObject[] objetosSpawnersParaParar;

    private List<PlayerScript> jogadoresNaPartida = new List<PlayerScript>();

    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1f; // Garante que a partida sempre comece na velocidade normal
    }

    private void Start()
    {
        if (painelGameOver != null)
            painelGameOver.SetActive(false);

        int quantidade = PlayerPrefs.GetInt("QuantidadeJogadores", 1);
        quantidade = Mathf.Clamp(quantidade, 1, jogadores.Length);

        for (int i = 0; i < jogadores.Length; i++)
        {
            bool jogadorAtivo = (i < quantidade);

            if (jogadores[i].painelHUD != null)
                jogadores[i].painelHUD.SetActive(jogadorAtivo);

            if (jogadorAtivo && prefabPeixe != null)
            {
                Vector3 posSpawn = jogadores[i].pontoSpawn != null
                    ? jogadores[i].pontoSpawn.position
                    : Vector3.zero;

                GameObject peixeObj = Instantiate(prefabPeixe, posSpawn, Quaternion.identity);
                peixeObj.name = "Peixinho_P" + (i + 1);

                PlayerScript player = peixeObj.GetComponent<PlayerScript>();
                if (player != null)
                {
                    player.ConfigurarControles(
                        jogadores[i].cima,
                        jogadores[i].baixo,
                        jogadores[i].esquerda,
                        jogadores[i].direita,
                        jogadores[i].dash
                    );

                    player.ConfigurarUI(jogadores[i].coracoesHUD, jogadores[i].pontosHUD);
                    jogadoresNaPartida.Add(player);
                }
            }
        }
    }

    public void VerificarFimDeJogo()
    {
        // Verifica se ainda existe algum jogador vivo
        foreach (PlayerScript p in jogadoresNaPartida)
        {
            if (p != null && !p.EstaMorto())
            {
                return; // Ainda tem pelo menos 1 vivo, o jogo continua!
            }
        }

        // Todos morreram! Encerra a rodada:
        PararGeracaoDeObjetos();
        Time.timeScale = 0f;

        if (painelGameOver != null)
            painelGameOver.SetActive(true);
    }

    private void PararGeracaoDeObjetos()
    {
        if (objetosSpawnersParaParar == null) return;

        foreach (GameObject obj in objetosSpawnersParaParar)
        {
            if (obj != null)
            {
                // Para todos os scripts/coroutines dentro daquele objeto e desativa ele
                foreach (MonoBehaviour script in obj.GetComponents<MonoBehaviour>())
                {
                    script.CancelInvoke();
                    script.StopAllCoroutines();
                }
                obj.SetActive(false);
            }
        }
    }

    // Já deixa pronto para você conectar no botão "Rejogar" no futuro!
    public void RejogarPartida()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}