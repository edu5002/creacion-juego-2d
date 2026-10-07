using UnityEngine;

/// <summary>
/// Controla el ataque cuerpo a cuerpo de Elián con el Bastón de Chispa (un solo golpe,
/// sin combo). Requiere: Animator con los parámetros "Attack" (Trigger).
/// </summary>
[RequireComponent(typeof(Animator))]
public class PlayerCombat : MonoBehaviour
{
    [Header("Ataque")]
    [Tooltip("Tiempo mínimo (segundos) entre un golpe y el siguiente. Evita que se pueda re-disparar la animación a medio golpe.")]
    public float attackCooldown = 0.3f;

    private Animator animator;

    private bool isAttacking = false;
    private float cooldownTimer = 0f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Fire1")) // Cambia "Fire1" por el input que prefieras usar para atacar
        {
            TryAttack();
        }
    }

    private void TryAttack()
    {
        if (isAttacking || cooldownTimer > 0f) return;

        isAttacking = true;
        animator.SetTrigger("Attack");
    }

    /// <summary>
    /// Llama a este método desde un Animation Event al final del clip de ataque,
    /// para permitir que se pueda volver a atacar.
    /// </summary>
    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
        cooldownTimer = attackCooldown;
    }
}