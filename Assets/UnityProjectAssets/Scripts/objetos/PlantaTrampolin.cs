using UnityEngine;

[RequireComponent(typeof(Animator), typeof(BoxCollider2D))]
public class PlantaTrampolin : MonoBehaviour
{
    [Min(0.1f)]
    public float velocidadImpulso = 18f;

    [Min(0.1f)]
    public float tiempoEntreRebotes = 0.5f;

    [Tooltip("Margen permitido de los pies debajo del borde superior.")]
    [Min(0f)]
    public float toleranciaPies = 0.25f;

    [Header("Sonido")]
    public AudioClip sonidoRebote;

    [Range(0f, 1f)]
    public float volumenRebote = 0.6f;

    private AudioSource fuenteAudio;

    private Animator anim;
    private BoxCollider2D zona;
    private float siguienteRebote;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        zona = GetComponent<BoxCollider2D>();
        zona.isTrigger = true;
        fuenteAudio = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IntentarRebote(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        IntentarRebote(other);
    }

    private void IntentarRebote(Collider2D other)
    {
        if (!isActiveAndEnabled || Time.timeScale == 0f)
            return;

        if (Time.time < siguienteRebote || other.isTrigger)
            return;

        PlayerVida vida = other.GetComponentInParent<PlayerVida>();

        if (vida == null || vida.EstaMuerto)
            return;

        PlayerController control = vida.GetComponent<PlayerController>();
        Rigidbody2D rb = vida.GetComponent<Rigidbody2D>();

        if (control == null || !control.enabled ||
            rb == null || !rb.simulated)
            return;

        // Solo activa la planta mientras el jugador baja.
        if (rb.linearVelocity.y >= 0f)
            return;

        Bounds cuerpo = other.bounds;
        Bounds superficie = zona.bounds;

        // Evita activarla al tocar el tallo o entrar desde abajo.
        if (cuerpo.center.y <= superficie.max.y)
            return;

        if (cuerpo.min.y < superficie.max.y - toleranciaPies)
            return;

        siguienteRebote = Time.time + tiempoEntreRebotes;

        // Conserva el movimiento horizontal.
        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            velocidadImpulso
        );

        anim.ResetTrigger("Rebotar");
        anim.SetTrigger("Rebotar");
        
        if (fuenteAudio != null && sonidoRebote != null)
        {
            fuenteAudio.PlayOneShot(sonidoRebote, volumenRebote);
        }
    }
}