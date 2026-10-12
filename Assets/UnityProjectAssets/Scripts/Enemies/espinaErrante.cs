using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class espinaErrante : MonoBehaviour
{
    [Header("Límites de la patrulla")]
    public Transform limiteIzquierdo;
    public Transform limiteDerecho;

    [Header("Movimiento")]
    public float velocidad = 2f;

    [Header("Orientación del dibujo")]
    public bool spriteMiraDerecha = true;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private int direccion = 1;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (limiteIzquierdo == null || limiteDerecho == null)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        float izquierda = Mathf.Min(
            limiteIzquierdo.position.x,
            limiteDerecho.position.x
        );

        float derecha = Mathf.Max(
            limiteIzquierdo.position.x,
            limiteDerecho.position.x
        );

        if (rb.position.x >= derecha)
            direccion = -1;
        else if (rb.position.x <= izquierda)
            direccion = 1;

        rb.linearVelocity = new Vector2(
            direccion * velocidad,
            rb.linearVelocity.y
        );

        if (sprite != null)
        {
            sprite.flipX = spriteMiraDerecha
                ? direccion < 0
                : direccion > 0;
        }
    }
}