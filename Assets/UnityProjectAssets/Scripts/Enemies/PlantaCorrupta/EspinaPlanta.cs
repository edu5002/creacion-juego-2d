using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class EspinaPlanta : MonoBehaviour
{
    [Min(0.1f)] public float velocidad = 6f;
    [Min(0.1f)] public float tiempoDeVida = 4f;

    private Rigidbody2D rb;
    private Collider2D col;
    private Vector2 direccion;
    private int danio;
    private Transform propietario;
    private ContactFilter2D filtro;
    private bool iniciada;
    private bool consumida;

    private readonly List<RaycastHit2D> impactos = new();

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    public void Iniciar(
        Vector2 rumbo,
        int cantidadDanio,
        LayerMask capasImpacto,
        Transform origen)
    {
        direccion = rumbo.sqrMagnitude > 0.001f
            ? rumbo.normalized
            : Vector2.right;

        danio = cantidadDanio;
        propietario = origen;

        filtro = new ContactFilter2D();
        filtro.SetLayerMask(capasImpacto);
        filtro.useTriggers = true;

        // El sprite de la espina debe apuntar hacia la derecha.
        float angulo = Mathf.Atan2(
            direccion.y, direccion.x
        ) * Mathf.Rad2Deg;

        rb.rotation = angulo;

        iniciada = true;
        Destroy(gameObject, tiempoDeVida);
    }

    private void FixedUpdate()
    {
        if (!iniciada || consumida)
            return;

        float distancia = velocidad * Time.fixedDeltaTime;

        impactos.Clear();
        col.Cast(direccion, filtro, impactos, distancia);

        foreach (RaycastHit2D impacto in impactos)
        {
            Collider2D otro = impacto.collider;

            if (otro == null)
                continue;

            if (propietario != null &&
                otro.transform.IsChildOf(propietario))
                continue;

            PlayerVida jugador =
                otro.GetComponentInParent<PlayerVida>();

            // Ignora triggers que no pertenecen al jugador.
            if (jugador == null && otro.isTrigger)
                continue;

            consumida = true;

            if (jugador != null && !jugador.EstaMuerto)
                jugador.RecibirDanio(danio, rb.position);

            gameObject.SetActive(false);
            Destroy(gameObject);
            return;
        }

        rb.MovePosition(
            rb.position + direccion * distancia
        );
    }
}