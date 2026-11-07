using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 5f;

    [Header("Salto")]
    public float fuerzaSalto = 12f;

    [Tooltip("Margen para saltar después de abandonar un borde.")]
    [Min(0f)]
    public float tiempoCoyote = 0.12f;

    public bool EnSuelo { get; private set; } = false;
    public bool MirandoDerecha { get; private set; } = true;

    private Rigidbody2D rb;
    private Animator animator;
    private PlayerCombat combate;
    private PlayerAudio sonidos;

    private float movimientoX;
    private float ultimoTiempoEnSuelo = float.NegativeInfinity;
    private bool saltoConsumido;

    private readonly HashSet<Collider2D> contactosSuelo =
        new HashSet<Collider2D>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        combate = GetComponent<PlayerCombat>();
        sonidos = GetComponent<PlayerAudio>();
    }

    private void Update()
    {
        if (Time.timeScale == 0f)
            return;

        if (EnSuelo && !saltoConsumido)
            ultimoTiempoEnSuelo = Time.time;

        if (combate != null && combate.EstaAtacando)
        {
            movimientoX = 0f;
            ActualizarAnimator();
            return;
        }

        movimientoX = Input.GetAxisRaw("Horizontal");

        if (movimientoX > 0f)
            MirandoDerecha = true;
        else if (movimientoX < 0f)
            MirandoDerecha = false;

        if (movimientoX != 0f)
        {
            Vector3 escala = transform.localScale;

            escala.x = Mathf.Abs(escala.x) *
                (MirandoDerecha ? 1f : -1f);

            transform.localScale = escala;
        }

        bool dentroDelMargen =
            Time.time - ultimoTiempoEnSuelo <= tiempoCoyote;

        bool puedeSaltar =
            !saltoConsumido && (EnSuelo || dentroDelMargen);

        if (Input.GetButtonDown("Jump") && puedeSaltar)
        {
            // Consume tanto el salto normal como el margen.
            saltoConsumido = true;
            ultimoTiempoEnSuelo = float.NegativeInfinity;

            // Evita mantener IsGrounded al despegar.
            contactosSuelo.Clear();
            EnSuelo = false;

            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                fuerzaSalto
            );

            if (sonidos != null)
                sonidos.ReproducirSalto();
        }

        ActualizarAnimator();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(
            movimientoX * velocidad,
            rb.linearVelocity.y
        );
    }

    private void ActualizarAnimator()
    {
        if (animator == null)
            return;

        animator.SetFloat("Speed", Mathf.Abs(movimientoX));
        animator.SetBool("IsGrounded", EnSuelo);
        animator.SetFloat(
            "VerticalVelocity", rb.linearVelocity.y
        );
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ActualizarSuelo(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        ActualizarSuelo(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        contactosSuelo.Remove(collision.collider);
        EnSuelo = contactosSuelo.Count > 0;
    }

    private void ActualizarSuelo(Collision2D collision)
    {
        // Unity puede enviar colisiones a componentes desactivados.
        // No recuperamos el salto durante daño o muerte.
        if (!isActiveAndEnabled)
            return;

        bool sostieneAlPersonaje = false;

        foreach (ContactPoint2D contacto in collision.contacts)
        {
            if (contacto.normal.y > 0.5f)
            {
                sostieneAlPersonaje = true;
                break;
            }
        }

        // Ignora contactos residuales al iniciar el salto.
        if (saltoConsumido && rb.linearVelocity.y > 0.01f)
            sostieneAlPersonaje = false;

        if (sostieneAlPersonaje)
        {
            contactosSuelo.Add(collision.collider);

            // El suelo vuelve a habilitar un salto.
            saltoConsumido = false;
            ultimoTiempoEnSuelo = Time.time;
        }
        else
        {
            contactosSuelo.Remove(collision.collider);
        }

        EnSuelo = contactosSuelo.Count > 0;
    }

    private void OnDisable()
    {
        contactosSuelo.Clear();
        EnSuelo = false;
        movimientoX = 0f;

        ultimoTiempoEnSuelo = float.NegativeInfinity;
        saltoConsumido = true;
    }
}