using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyVida : MonoBehaviour
{
    [Header("Vida")]
    public int vidaMaxima = 3;

    [Header("Referencias")]
    public MonoBehaviour movimiento;
    public GameObject zonaDanio;

    [Header("Reacción al golpe")]
    public bool usarAnimacionHerida = false;
    public float empujeHorizontal = 3f;
    public float duracionReaccion = 0.25f;

    [Header("Muerte")]
    public float tiempoAntesDeDesaparecer = 0.5f;

    public int VidaActual { get; private set; }
    public bool EstaMuerto { get; private set; }

    private Animator animator;
    private Rigidbody2D rb;
    private bool reaccionando;
    private bool reactivarMovimiento;
    private float finReaccion;
    private bool desaparicionProgramada;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // Mantiene compatibilidad con Espina Errante.
        if (movimiento == null)
            movimiento = GetComponent<espinaErrante>();

        VidaActual = Mathf.Max(1, vidaMaxima);
    }

    private void Update()
    {
        if (EstaMuerto || !reaccionando || Time.time < finReaccion)
            return;

        reaccionando = false;

        if (usarAnimacionHerida)
            animator.SetBool("recibeDanio", false);

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        if (movimiento != null && reactivarMovimiento)
            movimiento.enabled = true;
    }

    // Conserva la firma anterior.
    public void RecibirDanio(int cantidad)
    {
        RecibirDanio(cantidad, (Vector2)transform.position - Vector2.right);
    }

    public void RecibirDanio(int cantidad, Vector2 origenGolpe)
    {
        if (EstaMuerto || cantidad <= 0)
            return;

        VidaActual = Mathf.Max(0, VidaActual - cantidad);

        Debug.Log($"Vida de {name}: {VidaActual}/{vidaMaxima}", this);

        if (VidaActual == 0)
        {
            Morir();
            return;
        }

        if (!reaccionando)
        {
            reactivarMovimiento =
                movimiento != null && movimiento.enabled;
        }

        if (movimiento != null)
            movimiento.enabled = false;

        reaccionando = true;
        finReaccion = Time.time + Mathf.Max(0.01f, duracionReaccion);

        float direccion =
            transform.position.x >= origenGolpe.x ? 1f : -1f;

        rb.linearVelocity = new Vector2(
            direccion * empujeHorizontal,
            rb.linearVelocity.y
        );

        if (usarAnimacionHerida)
        {
            animator.SetFloat("Speed", 0f);
            animator.SetBool("recibeDanio", true);
        }
    }

    private void Morir()
    {
        EstaMuerto = true;
        reaccionando = false;

        if (movimiento != null)
            movimiento.enabled = false;

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        if (zonaDanio != null)
            zonaDanio.SetActive(false);

        if (usarAnimacionHerida)
            animator.SetBool("recibeDanio", false);

        animator.SetBool("muerto", true);
    }

    public void FinalizarMuerte()
    {
        if (!EstaMuerto || desaparicionProgramada)
            return;

        desaparicionProgramada = true;

        Destroy(gameObject, Mathf.Max(0f, tiempoAntesDeDesaparecer));
    }
}