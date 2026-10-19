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

    [Header("Ataque")]
    public float distanciaAtaque = 1.5f;
    public float diferenciaVerticalMaxima = 0.8f;
    public float velocidadEmbestida = 5f;
    public float esperaEntreAtaques = 1.2f;

    [Header("Orientación")]
    public bool spriteMiraDerecha = true;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sprite;
    private EnemyVida vida;
    private PlayerVida vidaJugador;

    private bool atacando;
    private bool embistiendo;
    private bool golpeRegistrado;
    private float direccionAtaque;
    private float siguienteAtaque;
    private float limiteAtaque;

    public bool PuedeHacerDanio =>
        isActiveAndEnabled &&
        atacando &&
        embistiendo &&
        !golpeRegistrado &&
        (vida == null || !vida.EstaMuerto);

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
            vidaJugador = jugador.GetComponentInParent<PlayerVida>();
    }

    private void FixedUpdate()
    {
        bool objetivoValido =
            jugador != null &&
            (vida == null || !vida.EstaMuerto) &&
            (vidaJugador == null || !vidaJugador.EstaMuerto);

        if (!objetivoValido)
        {
            CancelarAtaque();
            Mover(0f);
            return;
        }

        if (atacando)
        {
            // Respaldo si falta el evento final de animación.
            if (Time.time >= limiteAtaque)
            {
                FinalizarAtaque();
                return;
            }

            Mover(embistiendo
                ? direccionAtaque * velocidadEmbestida
                : 0f);

            return;
        }

        float diferenciaX = jugador.position.x - rb.position.x;
        float diferenciaY = jugador.position.y - rb.position.y;

        float distancia = Vector2.Distance(
            rb.position, jugador.position
        );

        if (distancia > rangoDeteccion)
        {
            Mover(0f);
            return;
        }

        bool cercaParaAtacar =
            Mathf.Abs(diferenciaX) <= distanciaAtaque &&
            Mathf.Abs(diferenciaY) <= diferenciaVerticalMaxima;

        if (cercaParaAtacar)
        {
            Mover(0f);

            if (Time.time >= siguienteAtaque)
                ComenzarAtaque(diferenciaX);

            return;
        }

        float movimiento = Mathf.Abs(diferenciaX) > distanciaParada
            ? Mathf.Sign(diferenciaX) * velocidad
            : 0f;

        Orientar(movimiento);
        Mover(movimiento);
    }

    private void ComenzarAtaque(float diferenciaX)
    {
        atacando = true;
        embistiendo = false;
        golpeRegistrado = false;

        direccionAtaque = diferenciaX >= 0f ? 1f : -1f;
        Orientar(direccionAtaque);

        limiteAtaque = Time.time + 2f;

        animator.SetBool("Atacando", true);
        animator.ResetTrigger("Attack");
        animator.SetTrigger("Attack");
    }

    // Evento al comenzar el movimiento hacia delante.
    public void IniciarEmbestida()
    {
        if (!isActiveAndEnabled || !atacando)
            return;

        embistiendo = true;
    }

    // Evento cuando comienza la recuperación.
    public void TerminarEmbestida()
    {
        if (!isActiveAndEnabled || !atacando)
            return;

        embistiendo = false;
        Mover(0f);
    }

    // Evento cerca del final del clip.
    public void FinalizarAtaque()
    {
        if (!isActiveAndEnabled || !atacando)
            return;

        CancelarAtaque();
        Mover(0f);
    }

    public void RegistrarImpacto()
    {
        golpeRegistrado = true;
    }

    private void CancelarAtaque()
    {
        if (atacando)
        {
            siguienteAtaque =
                Time.time + esperaEntreAtaques;
        }

        atacando = false;
        embistiendo = false;
        golpeRegistrado = true;

        if (animator != null)
        {
            animator.SetBool("Atacando", false);
            animator.ResetTrigger("Attack");
        }
    }

    private void Mover(float velocidadX)
    {
        rb.linearVelocity = new Vector2(
            velocidadX, rb.linearVelocity.y
        );

        animator.SetFloat(
            "Speed", atacando ? 0f : Mathf.Abs(velocidadX)
        );
    }

    private void Orientar(float direccion)
    {
        if (sprite == null || direccion == 0f)
            return;

        sprite.flipX = spriteMiraDerecha
            ? direccion < 0f
            : direccion > 0f;
    }

    private void OnDisable()
    {
        // EnemyVida desactiva este componente al recibir daño.
        // No modificamos la velocidad aquí para conservar el empuje.
        CancelarAtaque();
    }
}