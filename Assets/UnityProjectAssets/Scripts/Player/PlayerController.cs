using UnityEngine;

/// <summary>
/// Movimiento del protagonista: correr, saltar, detección de suelo por colisión física,
/// flip del sprite y parámetros para el Animator (Speed, IsGrounded, VerticalVelocity).
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 5f;

    [Header("Salto")]
    public float fuerzaSalto = 12f;

    [Header("Estado (solo lectura)")]
    public bool EnSuelo { get; private set; } = false;
    public bool MirandoDerecha { get; private set; } = true;

    private Rigidbody2D rb;
    private Animator animator;
    private float movimientoX;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // puede ser null si aún no agregas el Animator
    }

    private void Update()
    {
        movimientoX = Input.GetAxisRaw("Horizontal");

        if (movimientoX > 0f) MirandoDerecha = true;
        else if (movimientoX < 0f) MirandoDerecha = false;

        if (movimientoX != 0f)
        {
            Vector3 escala = transform.localScale;
            escala.x = Mathf.Abs(escala.x) * (MirandoDerecha ? 1f : -1f);
            transform.localScale = escala;
        }

        if (Input.GetButtonDown("Jump") && EnSuelo)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
        }

        ActualizarAnimator();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(movimientoX * velocidad, rb.linearVelocity.y);
    }

    private void ActualizarAnimator()
    {
        if (animator == null) return;

        animator.SetFloat("Speed", Mathf.Abs(movimientoX));
        animator.SetBool("IsGrounded", EnSuelo);
        animator.SetFloat("VerticalVelocity", rb.linearVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision) => ActualizarSuelo(collision);
    private void OnCollisionStay2D(Collision2D collision) => ActualizarSuelo(collision);

    private void OnCollisionExit2D(Collision2D collision)
    {
        EnSuelo = false;
    }
    

    // Solo cuenta como "suelo" si el contacto viene de abajo (normal apuntando hacia arriba),
    // así evita marcar EnSuelo=true al chocar de lado contra una pared en el aire.
    private void ActualizarSuelo(Collision2D collision)
    {
        foreach (ContactPoint2D contacto in collision.contacts)
        {
            if (contacto.normal.y > 0.5f)
            {
                EnSuelo = true;
                return;
            }
        }
    }
}