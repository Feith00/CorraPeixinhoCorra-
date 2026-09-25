using UnityEngine;

public class ObstacleBouncer : MonoBehaviour
{
    [Header("Movimento")]
    public float speed = 5f;
    public Vector2 startDirection = Vector2.right; 

    void Start()
    {
        if (startDirection == Vector2.zero)
            startDirection = Vector2.right;
    }

    void FixedUpdate()
    {
        GetComponent<Rigidbody2D>().linearVelocity = startDirection.normalized * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Parede"))
        {
            Vector2 normal = collision.GetContact(0).normal;

           
            if (Mathf.Abs(normal.x) > Mathf.Abs(normal.y))
                startDirection.x *= -1;   
            else
                startDirection.y *= -1;   
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var player = other.GetComponentInParent<PlayerScript>();
        if (player == null) return;

        if (!player.isDashing)
        {
            player.TakeDamage();
        }

    }
}