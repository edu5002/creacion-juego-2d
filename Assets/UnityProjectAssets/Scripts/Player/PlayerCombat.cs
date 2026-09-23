using UnityEngine;

/// <summary>
/// Controla el combo de espada (Attack1 -> Attack2 -> Attack3) y el Dash (animación Slide).
/// Requiere: Animator con los parámetros "Attack" (Trigger), "ComboStep" (Int),
/// "Dash" (Trigger), "IsDashing" (Bool). Ver guía de configuración del Animator Controller.
///
/// Requiere en el mismo GameObject un PlayerController (para saber hacia dónde mira
/// el personaje y si está en el suelo) y un Rigidbody2D.
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerController))]
public class PlayerCombat : MonoBehaviour
{
    [Header("Combo de ataque")]
    [Tooltip("Nivel de combo máximo desbloqueado. Súbelo a 2 o 3 según el nivel actual (Nivel 1 = 1, Nivel 2 = 2, Nivel 3 = 3).")]
    public int maxComboUnlocked = 1;

    [Tooltip("Tiempo máximo (segundos) entre golpes para que el combo siga encadenando. Si se pasa este tiempo, el combo se reinicia.")]
    public float comboBufferTime = 0.6f;

    [Header("Dash")]
    [Tooltip("Velocidad del dash en unidades/segundo.")]
    public float dashSpeed = 14f;

    [Tooltip("Duración del dash en segundos. Debe coincidir aproximadamente con la duración del clip Slide.")]
    public float dashDuration = 0.25f;

    [Tooltip("Tiempo de reutilización (cooldown) del dash en segundos.")]
    public float dashCooldown = 0.5f;

    [Tooltip("¿El dash aéreo (segundo dash en el aire) ya está desbloqueado? Actívalo en Nivel 3.")]
    public bool aerialDashUnlocked = false;

    private Animator animator;
    private Rigidbody2D rb;
    private PlayerController playerController;

    // --- Estado interno del combo ---
    private int comboStep = 0;
    private float lastAttackTime = -999f;
    private bool isAttacking = false;

    // --- Estado interno del dash ---
    private bool isDashing = false;
    private bool canDash = true;
    private bool hasUsedAerialDash = false;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;
    private float dashDirection = 1f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        HandleComboTimeout();
        HandleDashInput();
        HandleDashCooldown();

        if (Input.GetButtonDown("Fire1")) // Cambia "Fire1" por el input que uses para atacar
        {
            TryAttack();
        }

        // PlayerController se desactiva durante el dash (ver StartDash/EndDash),
        // así que aprovechamos para resetear el dash aéreo apenas se vuelve a tocar el suelo.
        if (playerController.EnSuelo)
        {
            hasUsedAerialDash = false;
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            rb.linearVelocity = new Vector2(dashDirection * dashSpeed, rb.linearVelocity.y);

            dashTimer -= Time.fixedDeltaTime;
            if (dashTimer <= 0f)
            {
                EndDash();
            }
        }
    }

    // ---------------- COMBO DE ATAQUE ----------------

    private void TryAttack()
    {
        // No permitir atacar mientras se hace dash
        if (isDashing) return;

        // Si pasó demasiado tiempo desde el último golpe, reinicia el combo
        if (Time.time - lastAttackTime > comboBufferTime)
        {
            comboStep = 0;
        }

        // Avanza al siguiente golpe del combo, respetando el máximo desbloqueado
        if (comboStep < maxComboUnlocked)
        {
            comboStep++;
            lastAttackTime = Time.time;
            isAttacking = true;

            animator.SetInteger("ComboStep", comboStep);
            animator.SetTrigger("Attack");
        }
    }

    /// <summary>
    /// Llama a este método desde un Animation Event al final de cada clip Attack
    /// (Attack1, Attack2, Attack3) para permitir que el combo se reinicie correctamente.
    /// </summary>
    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
    }

    private void HandleComboTimeout()
    {
        if (!isAttacking && comboStep > 0 && Time.time - lastAttackTime > comboBufferTime)
        {
            comboStep = 0;
            animator.SetInteger("ComboStep", 0);
        }
    }

    // ---------------- DASH ----------------

    private void HandleDashInput()
    {
        if (Input.GetButtonDown("Dash")) // Configura este input en Project Settings > Input Manager
        {
            if (isDashing) return;

            bool onGround = playerController.EnSuelo;

            if (onGround && canDash)
            {
                StartDash();
            }
            else if (!onGround && aerialDashUnlocked && !hasUsedAerialDash && canDash)
            {
                StartDash();
                hasUsedAerialDash = true;
            }
        }
    }

    private void StartDash()
    {
        isDashing = true;
        canDash = false;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;

        dashDirection = playerController.MirandoDerecha ? 1f : -1f;

        // Desactiva el control de movimiento normal mientras dura el dash
        playerController.enabled = false;

        animator.SetBool("IsDashing", true);
        animator.SetTrigger("Dash");
    }

    private void EndDash()
    {
        isDashing = false;

        playerController.enabled = true;

        animator.SetBool("IsDashing", false);
    }

    private void HandleDashCooldown()
    {
        if (!canDash)
        {
            dashCooldownTimer -= Time.deltaTime;
            if (dashCooldownTimer <= 0f)
            {
                canDash = true;
            }
        }
    }
}