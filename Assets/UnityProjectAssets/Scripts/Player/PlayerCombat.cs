using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 5f;

    [Header("Estado")]
    public bool EnSuelo = true;

    public bool MirandoDerecha { get; private set; } = true;

    private Rigidbody2D rb;
    private float movimientoX;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        movimientoX = Input.GetAxisRaw("Horizontal");

        if (movimientoX > 0f)
        {
            MirandoDerecha = true;
        }
        else if (movimientoX < 0f)
        {
            MirandoDerecha = false;
        }

        if (movimientoX != 0f)
        {
            Vector3 escala = transform.localScale;

            escala.x = Mathf.Abs(escala.x);

            if (!MirandoDerecha)
            {
                escala.x *= -1f;
            }

            transform.localScale = escala;
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(
            movimientoX * velocidad,
            rb.linearVelocity.y
        );
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        EnSuelo = true;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        EnSuelo = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        EnSuelo = false;
    }
}