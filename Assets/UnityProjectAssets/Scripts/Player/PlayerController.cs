using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerController))]
public class PlayerCombat : MonoBehaviour
{
    [Header("Combo de ataque")]
    public int maxComboUnlocked = 1;
    public float comboBufferTime = 0.6f;

    [Header("Dash")]
    public float dashSpeed = 14f;
    public float dashDuration = 0.25f;
    public float dashCooldown = 0.5f;
    public bool aerialDashUnlocked = false;

    private Animator animator;
    private Rigidbody2D rb;
    private PlayerController playerController;

    private int comboStep = 0;
    private float lastAttackTime = -999f;
    private bool isAttacking = false;

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

        if (Input.GetButtonDown("Fire1"))
        {
            TryAttack();
        }

        if (playerController.EnSuelo)
        {
            hasUsedAerialDash = false;
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            rb.linearVelocity = new Vector2(
                dashDirection * dashSpeed,
                rb.linearVelocity.y
            );

            dashTimer -= Time.fixedDeltaTime;

            if (dashTimer <= 0f)
            {
                EndDash();
            }
        }
    }

    private void TryAttack()
    {
        if (isDashing)
        {
            return;
        }

        if (Time.time - lastAttackTime > comboBufferTime)
        {
            comboStep = 0;
        }

        if (comboStep < maxComboUnlocked)
        {
            comboStep++;
            lastAttackTime = Time.time;
            isAttacking = true;

            animator.SetInteger("ComboStep", comboStep);
            animator.SetTrigger("Attack");
        }
    }

    public void OnAttackAnimationEnd()
    {
        isAttacking = false;

        if (comboStep >= maxComboUnlocked)
        {
            comboStep = 0;
            animator.SetInteger("ComboStep", 0);
        }
    }

    private void HandleComboTimeout()
    {
        if (!isAttacking &&
            comboStep > 0 &&
            Time.time - lastAttackTime > comboBufferTime)
        {
            comboStep = 0;
            animator.SetInteger("ComboStep", 0);
        }
    }

    private void HandleDashInput()
    {
        if (Input.GetButtonDown("Dash"))
        {
            if (isDashing)
            {
                return;
            }

            bool onGround = playerController.EnSuelo;

            if (onGround && canDash)
            {
                StartDash();
            }
            else if (!onGround &&
                     aerialDashUnlocked &&
                     !hasUsedAerialDash &&
                     canDash)
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