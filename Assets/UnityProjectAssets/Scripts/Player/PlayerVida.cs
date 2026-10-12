using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerVida : MonoBehaviour
{
    public int vidaMaxima = 5;
    public float tiempoInvulnerable = 1f;

    [Header("Rebote")]
    public float fuerzaHorizontal = 6f;
    public float fuerzaVertical = 3f;
    public float duracionGolpe = 0.35f;

    [Header("Reaparición")]
    public float esperaTrasMuerte = 0.5f;
    public float proteccionAlReaparecer = 2f;

    public int VidaActual { get; private set; }
    public bool EstaMuerto { get; private set; }

    private Animator animator;
    private Rigidbody2D rb;
    private PlayerController movimiento;
    private PlayerCombat combate;

    private float siguienteDanio;
    private Coroutine reaccion;
    private Vector3 puntoReaparicion;
    private bool reaparicionProgramada;
    private int ordenCheckpointActual = 0;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        movimiento = GetComponent<PlayerController>();
        combate = GetComponent<PlayerCombat>();

        VidaActual = Mathf.Max(1, vidaMaxima);

        // Antes de activar un altar, reaparece donde comenzó.
        puntoReaparicion = transform.position;
    }


    public void ActivarCheckpoint(Vector3 posicion, int orden)
    {
        if (EstaMuerto)
            return;

        if (orden > ordenCheckpointActual)
        {
            ordenCheckpointActual = orden;
            puntoReaparicion = posicion;

            GuardadoPartida.Guardar(gameObject.scene.name, orden);
        }

        VidaActual = Mathf.Max(1, vidaMaxima);
        siguienteDanio = Time.time + tiempoInvulnerable;
    }

    private void Start()
    {
        string nivel = gameObject.scene.name;
        int ordenGuardado = GuardadoPartida.ObtenerCheckpoint(nivel);

        if (ordenGuardado <= 0)
            return;

       Checkpoint[] altares = FindObjectsByType<Checkpoint>();

        Checkpoint altarEncontrado = null;

        foreach (Checkpoint altar in altares)
        {
            if (altar.gameObject.scene == gameObject.scene &&
                altar.orden == ordenGuardado &&
                altar.puntoReaparicion != null)
            {
                altarEncontrado = altar;
                break;
            }
        }

        if (altarEncontrado == null)
        {
            Debug.LogWarning(
                $"No se encontró el checkpoint guardado {ordenGuardado}. " +
                "Elián comenzará en su posición inicial.",
                this
            );
            return;
        }

        ordenCheckpointActual = ordenGuardado;
        puntoReaparicion = altarEncontrado.puntoReaparicion.position;

        rb.linearVelocity = Vector2.zero;
        transform.position = puntoReaparicion;
        rb.position = new Vector2(
            puntoReaparicion.x, puntoReaparicion.y
        );

        VidaActual = Mathf.Max(1, vidaMaxima);
        siguienteDanio = Time.time + proteccionAlReaparecer;

        Debug.Log($"Partida cargada: checkpoint {ordenGuardado}", this);
    }

    public void RecibirDanio(int cantidad, Vector2 origen)
    {
        if (EstaMuerto || cantidad <= 0 ||
            Time.time < siguienteDanio)
            return;

        siguienteDanio = Time.time + tiempoInvulnerable;
        VidaActual = Mathf.Max(0, VidaActual - cantidad);

        if (reaccion != null)
        {
            StopCoroutine(reaccion);
            reaccion = null;
        }

        animator.ResetTrigger("Attack");

        if (combate != null)
        {
            combate.OnAttackAnimationEnd();
            combate.enabled = false;
        }

        if (movimiento != null)
            movimiento.enabled = false;

        Debug.Log($"Vida de Elián: {VidaActual}/{vidaMaxima}", this);

        if (VidaActual == 0)
        {
            Morir();
            return;
        }

        float direccion =
            transform.position.x >= origen.x ? 1f : -1f;

        rb.linearVelocity = new Vector2(
            direccion * fuerzaHorizontal,
            fuerzaVertical
        );

        animator.SetBool("recibeDanio", true);
        reaccion = StartCoroutine(Recuperarse());
    }

    private IEnumerator Recuperarse()
    {
        yield return new WaitForSeconds(duracionGolpe);

        if (!EstaMuerto)
        {
            animator.SetBool("recibeDanio", false);

            if (movimiento != null)
                movimiento.enabled = true;

            if (combate != null)
                combate.enabled = true;
        }

        reaccion = null;
    }

    public void OnHurtAnimationEnd()
    {
        animator.SetBool("recibeDanio", false);
    }

    private void Morir()
    {
        EstaMuerto = true;
        reaparicionProgramada = false;

        animator.SetBool("recibeDanio", false);
        animator.SetBool("muerto", true);

        rb.linearVelocity = Vector2.zero;
    }

    // Evento del último cuadro del clip die.
    public void OnDeathAnimationEnd()
    {
        if (!EstaMuerto || reaparicionProgramada)
            return;

        reaparicionProgramada = true;
        StartCoroutine(Reaparecer());
    }

    private IEnumerator Reaparecer()
    {
        yield return new WaitForSeconds(esperaTrasMuerte);

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        transform.position = puntoReaparicion;
        rb.position = new Vector2(
            puntoReaparicion.x, puntoReaparicion.y
        );

        VidaActual = Mathf.Max(1, vidaMaxima);
        EstaMuerto = false;
        siguienteDanio = Time.time + proteccionAlReaparecer;

        animator.ResetTrigger("Attack");
        animator.SetBool("muerto", false);
        animator.SetBool("recibeDanio", false);
        animator.SetBool("Atacando", false);
        animator.SetBool("IsGrounded", false);
        animator.SetFloat("Speed", 0f);
        animator.SetFloat("VerticalVelocity", 0f);

        // Sale de die sin necesitar una transición de salida.
        animator.Play("idle", 0, 0f);
        animator.Update(0f);

        if (movimiento != null)
            movimiento.enabled = true;

        if (combate != null)
            combate.enabled = true;

        reaparicionProgramada = false;
    }
}