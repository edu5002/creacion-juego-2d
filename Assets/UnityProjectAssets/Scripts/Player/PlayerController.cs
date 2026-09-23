using UnityEngine;

/// <summary>
/// Movimiento del protagonista: correr, saltar, detección de suelo, flip del sprite
/// y parámetros para el Animator (Speed, IsGrounded, VerticalVelocity).
///
/// Requiere en el mismo GameObject: Rigidbody2D (Body Type = Dynamic, Freeze Rotation Z activado)
/// y un Collider2D (ej. CapsuleCollider2D o BoxCollider2D).
/// Además necesita un hijo vacío llamado "GroundCheck" ubicado a la altura de los pies,
/// asignado en el campo groundCheck del Inspector.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 5f;

    [Header("Salto")]
    public float fuerzaSalto = 12f;

    [Tooltip("Punto en los pies del personaje usado para detectar el suelo.")]
    public Transform groundCheck;

    [Tooltip("Radio del círculo de detección de suelo.")]
    public float groundCheckRadius = 0.15f;

    [Tooltip("Capa(s) que cuentan como suelo.")]
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator animator;
    private bool mirandoDerecha = true;
    private bool enSuelo;

    // Acceso público para que otros scripts (como PlayerCombat) sepan hacia dónde mira
    // el personaje y si está en el suelo, sin duplicar lógica.
    public bool MirandoDerecha => mirandoDerecha;
    public bool EnSuelo => enSuelo;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // puede ser null si aún no agregas el Animator; se valida antes de usarlo
    }

    private void Update()
    {
        float inputX = Input.GetAxisRaw("Horizontal");

        MoverHorizontal(inputX);
        VoltearSprite(inputX);

        if (Input.GetButtonDown("Jump") && enSuelo)
        {
            Saltar();
        }

        ActualizarAnimator(inputX);
    }

    private void FixedUpdate()
    {
        enSuelo = groundCheck != null &&
                  Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void MoverHorizontal(float inputX)
    {
        rb.linearVelocity = new Vector2(inputX * velocidad, rb.linearVelocity.y);
    }

    private void Saltar()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
    }

    private void VoltearSprite(float inputX)
    {
        if (inputX > 0f && !mirandoDerecha)
        {
            mirandoDerecha = true;
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (inputX < 0f && mirandoDerecha)
        {
            mirandoDerecha = false;
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private void ActualizarAnimator(float inputX)
    {
        if (animator == null) return;

        animator.SetFloat("Speed", Mathf.Abs(inputX));
        animator.SetBool("IsGrounded", enSuelo);
        animator.SetFloat("VerticalVelocity", rb.linearVelocity.y);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}