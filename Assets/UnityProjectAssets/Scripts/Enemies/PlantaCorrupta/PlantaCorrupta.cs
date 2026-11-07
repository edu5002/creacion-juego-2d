using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(EnemyVida))]
public class PlantaCorrupta : MonoBehaviour
{
    [Header("Referencias")]
    public PlayerVida jugador;
    public Transform centroDeteccion;
    public Transform puntoLatigazo;
    public Transform puntoDisparo;
    public EspinaPlanta prefabEspina;

    [Header("Rangos en unidades del mundo")]
    [Min(0.1f)] public float rangoDeteccion = 6f;
    [Min(0.1f)] public float rangoLatigazo = 1.8f;
    [Min(0.01f)] public float radioGolpe = 0.7f;

    [Header("Ataques")]
    [Min(1)] public int danioLatigazo = 1;
    [Min(1)] public int danioEspina = 1;
    [Min(0.1f)] public float esperaEntreAtaques = 1.5f;
    [Min(0f)] public float tiempoAlertaInicial = 0.6f;
    [Min(0.5f)] public float tiempoMaximoAtaque = 5f;

    [Header("Capas")]
    public LayerMask capaJugador;
    public LayerMask capasImpactoEspina;

    [Header("Orientación")]
    public bool spriteMiraDerecha = true;
    
    [Header("Sonidos de ataque")]
    public AudioClip sonidoLatigazo;
    public AudioClip sonidoEspina;

    [Range(0f, 1f)]
    public float volumenLatigazo = 0.6f;

    [Range(0f, 1f)]
    public float volumenEspina = 0.6f;

    private AudioSource fuenteAudio;

    private Animator anim;
    private EnemyVida vida;
    private Collider2D colliderJugador;
    private Vector3 escalaInicial;

    private bool detectadoAntes;
    private bool atacando;
    private bool efectoAplicado;
    private bool ataqueFisico;
    private float siguienteAtaque;
    private float limiteAtaque;
    private Vector2 objetivoDisparo;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        vida = GetComponent<EnemyVida>();
        escalaInicial = transform.localScale;
        fuenteAudio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (jugador == null)
        {
            Debug.LogWarning(
                "Asigna a Elián en el campo Jugador de la planta.",
                this
            );
            return;
        }

        colliderJugador = jugador.GetComponent<Collider2D>();
    }

    private Vector2 PosicionJugador()
    {
        if (colliderJugador != null)
            return colliderJugador.bounds.center;

        return jugador.transform.position;
    }

    private Vector2 Centro()
    {
        return centroDeteccion != null
            ? (Vector2)centroDeteccion.position
            : (Vector2)transform.position;
    }

    private void Update()
    {
        if (Time.timeScale == 0f)
            return;

        if (vida.EstaMuerto || jugador == null ||
            jugador.EstaMuerto)
        {
            CancelarAtaque();
            anim.SetBool("Detectado", false);
            detectadoAntes = false;
            return;
        }

        Vector2 objetivo = PosicionJugador();
        float distancia = Vector2.Distance(Centro(), objetivo);
        bool detectado = distancia <= rangoDeteccion;

        anim.SetBool("Detectado", detectado);

        if (detectado && !detectadoAntes)
        {
            siguienteAtaque = Mathf.Max(
                siguienteAtaque,
                Time.time + tiempoAlertaInicial
            );
        }

        detectadoAntes = detectado;

        if (atacando)
        {
            if (Time.time >= limiteAtaque)
            {
                Debug.LogWarning(
                    "La planta no recibió FinalizarAtaque. " +
                    "Revisa los eventos de animación.",
                    this
                );

                FinalizarAtaque();
            }

            return;
        }

        if (!detectado || anim.GetBool("recibeDanio"))
            return;

        // Espera a salir de Herida o del ataque anterior.
        AnimatorStateInfo estado =
            anim.GetCurrentAnimatorStateInfo(0);

        if (!estado.IsName("Idle") &&
            !estado.IsName("Alerta"))
            return;

        Orientar(objetivo.x);

        if (Time.time < siguienteAtaque)
            return;

        bool cerca = distancia <= rangoLatigazo;

        if (cerca && puntoLatigazo == null)
            return;

        if (!cerca &&
            (puntoDisparo == null || prefabEspina == null))
            return;

        atacando = true;
        ataqueFisico = cerca;
        efectoAplicado = false;
        objetivoDisparo = objetivo;
        limiteAtaque = Time.time + tiempoMaximoAtaque;

        anim.SetBool("Atacando", true);
        anim.ResetTrigger("AtaqueFisico");
        anim.ResetTrigger("AtaqueDistancia");

        anim.SetTrigger(
            cerca ? "AtaqueFisico" : "AtaqueDistancia"
        );
    }

    private void Orientar(float objetivoX)
    {
        bool mirarDerecha = objetivoX >= transform.position.x;
        float signo = mirarDerecha == spriteMiraDerecha
            ? 1f : -1f;

        transform.localScale = new Vector3(
            Mathf.Abs(escalaInicial.x) * signo,
            escalaInicial.y,
            escalaInicial.z
        );
    }

    private bool PuedeAplicarAtaque()
    {
        return isActiveAndEnabled &&
               Time.timeScale > 0f &&
               !vida.EstaMuerto &&
               !anim.GetBool("recibeDanio") &&
               jugador != null &&
               !jugador.EstaMuerto &&
               atacando &&
               !efectoAplicado;
    }

    // Evento del fotograma de impacto del latigazo.
    public void AplicarLatigazo()
    {
        if (!PuedeAplicarAtaque() || !ataqueFisico ||
            puntoLatigazo == null)
            return;

        efectoAplicado = true;
        if (fuenteAudio != null && sonidoLatigazo != null)
        {
            fuenteAudio.PlayOneShot(
                sonidoLatigazo,
                volumenLatigazo
            );
        }

        Collider2D[] contactos = Physics2D.OverlapCircleAll(
            puntoLatigazo.position,
            radioGolpe,
            capaJugador
        );

        HashSet<PlayerVida> golpeados = new();

        foreach (Collider2D contacto in contactos)
        {
            PlayerVida objetivo =
                contacto.GetComponentInParent<PlayerVida>();

            if (objetivo != null &&
                !objetivo.EstaMuerto &&
                golpeados.Add(objetivo))
            {
                objetivo.RecibirDanio(
                    danioLatigazo,
                    transform.position
                );
            }
        }
    }

    // Evento del fotograma donde sale la espina.
    public void DispararEspina()
    {
        if (!PuedeAplicarAtaque() || ataqueFisico ||
            puntoDisparo == null || prefabEspina == null)
            return;

        efectoAplicado = true;
        if (fuenteAudio != null && sonidoEspina != null){
            fuenteAudio.PlayOneShot(
                sonidoEspina,
                volumenEspina
            );
        }

        Vector2 direccion =
            objetivoDisparo - (Vector2)puntoDisparo.position;

        EspinaPlanta espina = Instantiate(
            prefabEspina,
            puntoDisparo.position,
            Quaternion.identity
        );

        espina.Iniciar(
            direccion,
            danioEspina,
            capasImpactoEspina,
            transform
        );
    }

    // Evento cercano al final de ambos ataques.
    public void FinalizarAtaque()
    {
        if (!atacando)
            return;

        CancelarAtaque();
    }

    private void CancelarAtaque()
    {
        if (atacando)
            siguienteAtaque = Time.time + esperaEntreAtaques;

        atacando = false;
        efectoAplicado = true;

        if (anim != null)
        {
            anim.SetBool("Atacando", false);
            anim.ResetTrigger("AtaqueFisico");
            anim.ResetTrigger("AtaqueDistancia");
        }
    }

    private void OnDisable()
    {
        CancelarAtaque();
        detectadoAntes = false;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 centro = centroDeteccion != null
            ? centroDeteccion.position
            : transform.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(centro, rangoDeteccion);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(centro, rangoLatigazo);

        if (puntoLatigazo != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(
                puntoLatigazo.position, radioGolpe
            );
        }
    }
}