using UnityEngine;
using UnityEngine.UI;

public class MultiplayerManager : MonoBehaviour
{
    [System.Serializable]
    public struct ConfiguracaoJogador
    {
        public string nome;
        public Transform pontoSpawn;      // Onde este jogador nasce na tela
        public GameObject painelHUD;      // HUD_Player1, 2, 3 ou 4
        public Image[] coracoesHUD;       // Os 3 corações deste jogador
        public PontosUI pontosHUD;        // O script PontosUI deste jogador

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

    private void Start()
    {
        // Lê a quantidade escolhida no Menu (se testar a cena direto, assume 1)
        int quantidade = PlayerPrefs.GetInt("QuantidadeJogadores", 1);
        quantidade = Mathf.Clamp(quantidade, 1, jogadores.Length);

        for (int i = 0; i < jogadores.Length; i++)
        {
            bool jogadorAtivo = (i < quantidade);

            // 1. Ativa a HUD apenas se o jogador estiver participando
            if (jogadores[i].painelHUD != null)
            {
                jogadores[i].painelHUD.SetActive(jogadorAtivo);
            }

            // 2. Se estiver participando, cria o peixe e conecta controles + HUD
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
                }
            }
        }
    }
}