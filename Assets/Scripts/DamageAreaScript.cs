using UnityEngine;

public class DamageArea : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var player = other.GetComponentInParent<PlayerScript>();
        var controlador = GetComponentInParent<AnzolController>();

        if (controlador != null)
        {
            if (player != null && !player.isDashing)
            {
                player.TakeDamage();
            }

            // Manda o anzol recolher em alta velocidade
            controlador.IniciarRecolhimento();
        }
    }
}