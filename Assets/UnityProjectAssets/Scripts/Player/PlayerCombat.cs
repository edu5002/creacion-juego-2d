using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerController))]
public class PlayerCombat : MonoBehaviour
{
    [Header("Ataque")]
    public float attackCooldown = 0.3f;
    public int danio = 1;

    [Header("Zona del golpe")]
    public Transform puntoAtaque;
    public float radioAtaque = 0.6f;
    public LayerMask capasEnemigos;
    public bool EstaAtacando => isAttacking;

    private Animator animator;
    private PlayerController movimiento;


    private bool isAttacking;
    private bool golpeAplicado;
    private float siguienteAtaque;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        movimiento = GetComponent<PlayerController>();
    }

    private void Update()
    {

        if (Time.timeScale == 0f)
            return;

        if (Input.GetButtonDown("Fire1"))
            TryAttack();
    }

    private void TryAttack()
    {
        if (UnityEngine.EventSystems.EventSystem.current != null &&
           UnityEngine.EventSystems.EventSystem.current
               .IsPointerOverGameObject())
        {
            return;
        }

        if (isAttacking || Time.time < siguienteAtaque)
            return;

        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.01f)
            return;

        if (!movimiento.EnSuelo ||
            animator.GetBool("muerto") ||
            animator.GetBool("recibeDanio"))
            return;

        if (puntoAtaque == null)
        {
            Debug.LogWarning("Asigna Punto Ataque en PlayerCombat.", this);
            return;
        }

        isAttacking = true;
        golpeAplicado = false;

        animator.SetBool("Atacando", true);
        animator.ResetTrigger("Attack");
        animator.SetTrigger("Attack");
    }

    // Evento en el cuadro donde el bastón golpea.
    public void AplicarGolpe()
    {
        if (!enabled || !isAttacking || golpeAplicado)
            return;

        if (animator.GetBool("muerto") ||
            animator.GetBool("recibeDanio"))
            return;

        golpeAplicado = true;

        Collider2D[] impactos = Physics2D.OverlapCircleAll(
            puntoAtaque.position,
            radioAtaque,
            capasEnemigos
        );

        HashSet<EnemyVida> enemigosGolpeados =
            new HashSet<EnemyVida>();

        foreach (Collider2D impacto in impactos)
        {
            EnemyVida vida = impacto.GetComponentInParent<EnemyVida>();

            if (vida != null && enemigosGolpeados.Add(vida))
                vida.RecibirDanio(danio, transform.position);
        }
    }

    // Evento cerca del final de la animación.
    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
        siguienteAtaque = Time.time + attackCooldown;

        animator.SetBool("Atacando", false);
        animator.ResetTrigger("Attack");
    }

    private void OnDisable()
    {
        isAttacking = false;
        golpeAplicado = true;

        if (animator != null)
        {
            animator.SetBool("Atacando", false);
            animator.ResetTrigger("Attack");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (puntoAtaque == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(puntoAtaque.position, radioAtaque);
    }
}