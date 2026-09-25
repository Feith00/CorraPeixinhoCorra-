using UnityEngine;

public class MovimentoInfinito : MonoBehaviour
{
    [SerializeField] private float velocidade = 5f;

    private float largura;

    private void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (sr == null)
        {
            Debug.LogError("O objeto precisa ter um SpriteRenderer!");
            return;
        }

      
        largura = sr.bounds.size.x;
    }

    private void Update()
    {

        transform.position += Vector3.left * velocidade * Time.deltaTime;

 
        if (transform.position.x <= -largura)
        {
            transform.position += Vector3.right * largura * 2f;
        }
    }
}