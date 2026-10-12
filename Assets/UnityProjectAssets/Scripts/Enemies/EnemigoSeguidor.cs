using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class EnemigoSeguidor : MonoBehaviour
{
    public Transform jugador;

    [Header("Seguimiento")]
    public float rangoDeteccion = 5f;
    public float velocidad = 2f;
    public float distanciaParada = 0.15f;

    [Header("Orientación")]
    public bool spriteMiraDerecha = true;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sprite;
    private EnemyVida vida;
    private PlayerVida vidaJugador;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        vida = GetComponent<EnemyVida>();
    }

    private void Start()
    {
        if (jugador != null)
            vidaJugador = jugador.GetComponent<PlayerVida>();
    }

    private void FixedUpdate()
    {
        float movimiento = 0f;

        bool puedeSeguir =
            jugador != null &&
            (vida == null || !vida.EstaMuerto) &&
            (vidaJugador == null || !vidaJugador.EstaMuerto);

        if (puedeSeguir)
        {
            float distancia = Vector2.Distance(
                rb.position, jugador.position
            );

            float diferenciaX = jugador.position.x - rb.position.x;

            if (distancia <= rangoDeteccion &&
                Mathf.Abs(diferenciaX) > distanciaParada)
            {
                movimiento = Mathf.Sign(diferenciaX) * velocidad;
            }
        }

        rb.linearVelocity = new Vector2(
            movimiento, rb.linearVelocity.y
        );

        animator.SetFloat("Speed", Mathf.Abs(movimiento));

        if (sprite != null && movimiento != 0f)
        {
            sprite.flipX = spriteMiraDerecha
                ? movimiento < 0f
                : movimiento > 0f;
        }
    }
}